using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Assets.Scripts.Common;
using Assets.Scripts.Database.DataRow;
using Assets.Scripts.Network.Response;
using UnityEngine.Assertions;
using System.Collections;
using Assets.Scripts.Graphics;
using Database.DataRow.BodyComposition;

namespace Assets.Scripts.ThreeDView
{
    public enum BodyType : byte
    {
        Normal = 0,
        Short = 1,
        Long = 2
    }

    public class BodyTypeSelector : MonoBehaviour
    {
        [Serializable]
        public class AvatarUpdateEvent : UnityEvent<AvatarController> { }
        public AvatarUpdateEvent OnAvatarUpdateEvent;

        [Serializable]
        public class BodyTypeChangeEvent : UnityEvent<Gender, BodyType, BaseHuman> { }
        public BodyTypeChangeEvent OnChangeBodyType;

        [SerializeField]
        private List<BaseHuman> _humanSetList = default;
        private Dictionary<int, BaseHuman> _humanSetDic;

        [SerializeField]
        private Text _bodyTypeText = default;

        [SerializeField]
        private HumanInitParam _humanInitParam = new HumanInitParam();

        [SerializeField]
        private PostureAdviceAssetConfig _adviceConfig = default;

        public float GetAlphaOfLayer(HumanLayer layer)
        {
            return _humanInitParam.GetAlpha(layer);
        }

        public void SetAlphaOfLayer(HumanLayer layer, float alpha)
        {
            _humanInitParam.SetAlpha(layer, alpha);
            if (SelectedHuman != null)
            {
                SelectedHuman.SetHumanInitParam(_humanInitParam);
            }
        }

        private BaseHuman _selectedHuman;
        public BaseHuman SelectedHuman {
            get {
                return _selectedHuman;
            }
            set {
                _selectedHuman = value;
            }
        }
        
        private Measurement _measurement;
        private BodyComposition _bodyComposition;
        
        private const float BlendShapeScale = 100;

        private BodyType? _bodyType = null;

        private bool _isRunningDataLoadWaitProcess = false;

        private bool _isRunningBodyTypeSelectWaitProcess = false;

        private Dictionary<BonePart, MaterialColorSettings> _boneMaterialMap = new Dictionary<BonePart, MaterialColorSettings>();

        void Awake()
        {
            _humanSetDic = new Dictionary<int, BaseHuman>();
            foreach (BaseHuman human in _humanSetList)
            {
                int key = GetHumanId(human.Gender, human.BodyType);
                _humanSetDic.Add(key, human);
                human.AvatarController.onAvatarUpdated.AddListener(OnAvatarUpdated);
            }

            if (_adviceConfig != null)
            {
                var boneParts = Enum.GetValues(typeof(BonePart));
                foreach (BonePart p in boneParts)
                {
                    _boneMaterialMap[p] = p.isCartilage() ? _adviceConfig.cartilageNormalMaterialSetting : _adviceConfig.boneNormalMaterialSetting;
                }
            }
        }

        void Start()
        {
            StartCoroutine(WaitForDataLoadProcess(() =>
            {
                UpdateHuman();
            }));
        }
        
        void Update()
        {
        }

        /// <summary>
        /// アクティブ/非アクティブの設定.
        /// </summary>
        /// <param name="active"></param>
        public void SetActive(bool active)
        {
            // APIによるデータ取得処理が完了していないケースがあるため、StartCoroutineで設定.
            StartCoroutine(WaitForBodyTypeSelectProcess(() =>
            {
                SetActiveAllLayer(active);
            }));
        }

        /// <summary>
        /// 体組成データ更新イベント
        /// </summary>
        /// <param name="loader"></param>
        public void OnUpdateBodyCompositionData(BodyCompositionDataLoader loader)
        {
            DebugUtil.Log(gameObject.name + " : OnLoadBodyCompositionData");

            _bodyComposition = loader.BodyComposition;
        }

        /// <summary>
        /// 採寸データ更新イベント
        /// </summary>
        /// <param name="loader"></param>
        public void OnUpdateMeasurementData(MeasurementDataLoader loader)
        {
            DebugUtil.Log(gameObject.name + " : OnLoadMeasurementData");
            _measurement = loader.Measurement;
        }

        /// <summary>
        /// FBXのポーズ更新イベント
        /// </summary>
        /// <param name="loader"></param>
        public void OnAvatarUpdated(AvatarController avatar)
        {
            DebugUtil.Log(gameObject.name + " : OnAvatarUpdated");
            OnAvatarUpdateEvent.Invoke(avatar);
        }

        /// <summary>
        /// データロード完了待機処理.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IEnumerator WaitForDataLoadProcess(Action action)
        {
            if (_isRunningDataLoadWaitProcess)
            {
                yield break;
            }
            _isRunningDataLoadWaitProcess = true;

            // データロード完了まで待機.
            while (_measurement == null || _bodyComposition == null)
            {
                yield return null;
            }

            action();

            _isRunningDataLoadWaitProcess = false;
        }

        /// <summary>
        /// 体型種別選択の待機処理.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IEnumerator WaitForBodyTypeSelectProcess(Action action)
        {
            if (_isRunningBodyTypeSelectWaitProcess)
            {
                yield break;
            }
            _isRunningBodyTypeSelectWaitProcess = true;

            // データロード完了まで待機.
            while (SelectedHuman == null)
            {
                yield return null;
            }

            action();

            _isRunningBodyTypeSelectWaitProcess = false;
        }

        /// <summary>
        /// 人体モデルの更新.
        /// </summary>
        private void UpdateHuman()
        {
            var gender = DataManager.Instance.Profile.Gender;
            var type = GetBodyType(gender, _measurement.correctedHeight,
                                    _measurement.correctedInseam, (_measurement.LeftSleeve + _measurement.RightSleeve) / 2f);
            if (_bodyType != type)
            {
                // 指定されたFBXのみ表示する
                var humanId = GetHumanId(gender, type);
                foreach (var human in _humanSetDic)
                {
                    if (humanId == human.Key)
                    {
                        SelectedHuman = human.Value;
                        SelectedHuman.gameObject.SetActive(true);
                        SelectedHuman.SetHumanInitParam(_humanInitParam);
#if DEBUG
                        if (_bodyTypeText != null)
                        {
                            _bodyTypeText.text = SelectedHuman.name;
                        }
#endif
                    }
                    else
                    {
                        human.Value.gameObject.SetActive(false);
                    }
                }

                DeformHuman(SelectedHuman, _bodyComposition);

                if (OnChangeBodyType != null)
                {
                    OnChangeBodyType.Invoke(gender, type, SelectedHuman);
                }
            }
            _bodyType = type;
        }

        /// <summary>
        /// 人体モデルのID取得.
        /// </summary>
        /// <param name="gender"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private int GetHumanId(Gender gender, BodyType type)
        {
            return ((byte)gender << 8 | (byte)type);
        }

        /// <summary>
        /// 全てのレイヤーのアクティブ/非アクティブ切替.
        /// </summary>
        /// <param name="active"></param>
        private void SetActiveAllLayer(bool active)
        {
            foreach (HumanLayer layer in Enum.GetValues(typeof(HumanLayer)))
            {
                SelectedHuman.SetActive(layer, active);
            }
        }

        /// <summary>
        /// 骨の色の初期化.
        /// </summary>
        public void ResetBoneColor()
        {
            // 骨、軟骨の色を変更.
            foreach (var b in _boneMaterialMap)
            {
                SelectedHuman.SetMaterialColor(
                    HumanLayer.Bone,
                    b.Key.ToString(),
                    b.Value.albedoColor,
                    b.Value.specularColor);
            }
        }

        /// <summary>
        /// 体組成データにより、脂肪・筋肉のブレンドシェイプを調整するための構造体.
        /// </summary>
        struct DeformMap
        {
            public BodyCompositionItem Item { get; set; }
            public object PositiveScoreArg { get; set; }
            public object NegativeScoreArg { get; set; }
            public DeformMap(BodyCompositionItem item, object positive_score_arg, object negative_score_arg)
            {
                Item = item;
                PositiveScoreArg = positive_score_arg;
                NegativeScoreArg = negative_score_arg;
            }
        }

        /// <summary>
        /// 筋肉のブレンドシェイプ名.
        /// </summary>
        private readonly List<DeformMap> msParamList = new List<DeformMap>() {
            new DeformMap( BodyCompositionItem.TrunkMuscleScore, "ms_trunk_large", "ms_trunk_small" ),
            new DeformMap( BodyCompositionItem.RightArmMuscleScore, "ms_arm_R_large", "ms_arm_R_small" ),
            new DeformMap( BodyCompositionItem.LeftArmMuscleScore, "ms_arm_L_large", "ms_arm_L_small" ),
            new DeformMap( BodyCompositionItem.RightLegMuscleScore, "ms_leg_R_large", "ms_leg_R_small" ),
            new DeformMap( BodyCompositionItem.LeftLegMuscleScore, "ms_leg_L_large", "ms_leg_L_small" ),
        };

        /// <summary>
        /// 内臓脂肪に関連するブレンドシェイプのテーブル.
        /// </summary>
        private readonly List<DeformMap> visceralFatParamList = new List<DeformMap>() {
            new DeformMap( BodyCompositionItem.VisceralFatLevel, new string[] { "ms_stomach_Z_large", "ms_stomach_X_large" }, null ),
        };

        /// <summary>
        /// 体脂肪スコアに関連するブランドシェイプのテーブル.
        /// </summary>
        private readonly List<DeformMap> ftParamList = new List<DeformMap>() {
            new DeformMap( BodyCompositionItem.TrunkFatScore, new string[] { "trank_X_large", "trank_Z_large", "breast_X_large", "breast_Z_large" }, FatPart.Trunk),
            new DeformMap( BodyCompositionItem.RightArmFatScore, new string[] { "arm_R_X_large", "arm_R_Z_large" }, FatPart.ArmR),
            new DeformMap( BodyCompositionItem.LeftArmFatScore, new string[] { "arm_L_X_large", "arm_L_Z_large" }, FatPart.ArmL),
            new DeformMap( BodyCompositionItem.RightLegFatScore, new string[] { "leg_R_X_large", "leg_R_Z_large" }, FatPart.LegR),
            new DeformMap( BodyCompositionItem.LeftLegFatScore, new string[] { "leg_L_X_large", "leg_L_Z_large" }, FatPart.LegL),
        };

        /// <summary>
        /// 体脂肪率に関連するブランドシェイプのテーブル.
        /// </summary>
        private readonly List<DeformMap> ftParamListForYoung = new List<DeformMap>() {
            new DeformMap( BodyCompositionItem.TrunkFatPercentage, new string[] { "trank_X_large", "trank_Z_large", "breast_X_large", "breast_Z_large" }, FatPart.Trunk),
            new DeformMap( BodyCompositionItem.RightArmFatPercentage, new string[] { "arm_R_X_large", "arm_R_Z_large" }, FatPart.ArmR),
            new DeformMap( BodyCompositionItem.LeftArmFatPercentage, new string[] { "arm_L_X_large", "arm_L_Z_large" }, FatPart.ArmL),
            new DeformMap( BodyCompositionItem.RightLegFatPercentage, new string[] { "leg_R_X_large", "leg_R_Z_large" }, FatPart.LegR),
            new DeformMap( BodyCompositionItem.LeftLegFatPercentage, new string[] { "leg_L_X_large", "leg_L_Z_large" }, FatPart.LegL),
        };

        /// <summary>
        /// 年齢によって変形方法を変えるための境界値(現状は17歳)
        /// </summary>
        private const int DeformBoundaryAge = 17;

        struct ThresholdValues
        {
            public float standard { get; set; }
            public float min { get; set; }
            public float max { get; set; }
            public ThresholdValues(float standard, float min, float max)
            {
                this.standard = standard;
                this.min = min;
                this.max = max;
            }
        }
        
        /// <summary>
        /// 性別毎の体脂肪率のテーブル(下記の範囲を標準として計算).
        /// 男性:  6 ~ 40%.
        /// 女性: 14 ~ 50%.
        /// </summary>
        private readonly Dictionary<Gender, ThresholdValues> GenderFatPercentageThresholdMap = new Dictionary<Gender, ThresholdValues>(){
            { Gender.Male, new ThresholdValues(16f, 6f, 40f) },
            { Gender.Female, new ThresholdValues(26f, 14f, 50f) },
        };

        /// <summary>
        /// ブレンドシェイプの設定
        /// </summary>
        /// <param name="csvData"></param>
        public void DeformHuman(BaseHuman human, BodyComposition bodyComposition)
        {
            switch (bodyComposition.Type)
            {
                case BodyComposition.TypeMc780:
                    DeformHumanTypeMC780(human, bodyComposition);
                    break;
                case BodyComposition.TypeIScale10:
                    DeformHumanTypeIScale10(human, bodyComposition);
                    break;
            }
        }

        private void DeformHumanTypeMC780(BaseHuman human, BodyComposition bodyComposition)
        {
            // 測定日時点の年齢を取得.
            Profile profile = DataManager.Instance.Profile;
            var age = profile.GetAge(bodyComposition.CreateTimeAsDateTime);

            // 17歳を境にFBXの変形方法を変える.
            // 17歳以下の変形方法については「17歳以下対応.pdf」参照.
            if (age <= DeformBoundaryAge)
            {
                // 体脂肪率に応じた変形.
                foreach (DeformMap param in ftParamListForYoung)
                {
                    var parcentage = (float)bodyComposition.GetValue<double>(param.Item.ToString());
                    var standard = GenderFatPercentageThresholdMap[profile.Gender].standard;
                    var min = GenderFatPercentageThresholdMap[profile.Gender].min;
                    var max = GenderFatPercentageThresholdMap[profile.Gender].max;
                    
                    // ブレンドシェイプは standard ⇒ max 間でモーフィング.
                    int blendShape = (int)(Normalize(parcentage, standard, max) * BlendShapeScale);
                    // テクスチャのアルファ値は min ⇒ standard 間でモーフィング.
                    float textureAlpha = Normalize(parcentage, min, standard);

                    string[] blendShapeNames = (string[])param.PositiveScoreArg;
                    foreach (string name in blendShapeNames)
                    {
                        human.SetBlendShape(name, blendShape);
                    }
                    FatPart part = (FatPart)param.NegativeScoreArg;
                    human.MulFatAlpha(part, textureAlpha);
                    //DebugUtil.Log("[" + parcentage + "]" + " blendShape:" + blendShape + " textureAlpha:" + textureAlpha);
                }
            }
            else
            {
                // 18歳以上.
                // 筋肉量スコアに応じた変形.
                foreach (DeformMap param in msParamList)
                {
                    var score = bodyComposition.GetValue<int>(param.Item.ToString());
                    var bsName = (string)(score >= 0 ? param.PositiveScoreArg : param.NegativeScoreArg);
                    human.SetBlendShape(bsName, MuscleScoreToBlendShapeWeight(score));
                }

                // 内臓脂肪レベルに応じた変形(9～15の範囲で段階的に変化させる).
                foreach (DeformMap param in visceralFatParamList)
                {
                    var score = (float)bodyComposition.GetValue<double>(param.Item.ToString());
                    var blendShapeNames = (string[])param.PositiveScoreArg;

                    foreach (string name in blendShapeNames)
                    {
                        human.SetBlendShape(name, VisceralFatLevelToBlendShapeWeight(score));
                    }
                }

                // 体脂肪率スコアに応じた変形.
                foreach (DeformMap param in ftParamList)
                {
                    var score = bodyComposition.GetValue<int>(param.Item.ToString());
                    if (score >= 0)
                    {
                        string[] blendShapeNames = (string[])param.PositiveScoreArg;
                        foreach (string name in blendShapeNames)
                        {
                            human.SetBlendShape(name, MuscleScoreToBlendShapeWeight(score));
                        }
                    }
                    else
                    {
                        // スコアが0未満の場合はアルファ値を変更.
                        FatPart part = (FatPart)param.NegativeScoreArg;
                        human.MulFatAlpha(part, FatScoreToAlphaRatio(score));
                    }
                }
            }
        }

        private void DeformHumanTypeIScale10(BaseHuman human, BodyComposition bodyComposition)
        {
            // i-scaleは体脂肪率、体脂肪量、筋肉量が部位毎に分かれていない為、全体の値で変形を適用する。

            // 測定日時点の年齢を取得.
            Profile profile = DataManager.Instance.Profile;
            var age = profile.GetAge(bodyComposition.CreateTimeAsDateTime);

            // 17歳を境にFBXの変形方法を変える.
            // 17歳以下の変形方法については「17歳以下対応.pdf」参照.
            if (age <= DeformBoundaryAge)
            {
                // 体脂肪率に応じた変形.
                var standard = GenderFatPercentageThresholdMap[profile.Gender].standard;
                var min = GenderFatPercentageThresholdMap[profile.Gender].min;
                var max = GenderFatPercentageThresholdMap[profile.Gender].max;
                
                // 体脂肪率
                var parcentage = (float)bodyComposition.FatPercentage;

                // ブレンドシェイプは standard ⇒ max 間でモーフィング.
                int blendShape = (int)(Normalize(parcentage, standard, max) * BlendShapeScale);

                // テクスチャのアルファ値は min ⇒ standard 間でモーフィング.
                float textureAlpha = Normalize(parcentage, min, standard);
                
                foreach (DeformMap param in ftParamListForYoung)
                {
                    string[] blendShapeNames = (string[])param.PositiveScoreArg;
                    foreach (string name in blendShapeNames)
                    {
                        human.SetBlendShape(name, blendShape);
                    }
                    FatPart part = (FatPart)param.NegativeScoreArg;
                    human.MulFatAlpha(part, textureAlpha);
                }
            }
            else
            {
                // 18歳以上.

                // 筋肉率 = 筋肉量 / 体重
                var muscleParcentage = (float)(bodyComposition.Muscle / bodyComposition.Weight);

                // 筋肉量スコア
                int muscleScore = bodyComposition.MuscleScore;

                // 筋肉量スコアに応じた変形.
                foreach (DeformMap param in msParamList)
                {
                    var bsName = (string)(muscleScore >= 0 ? param.PositiveScoreArg : param.NegativeScoreArg);
                    human.SetBlendShape(bsName, MuscleScoreToBlendShapeWeight(muscleScore));
                }

                // 内臓脂肪レベルに応じた変形(9～15の範囲で段階的に変化させる).
                foreach (DeformMap param in visceralFatParamList)
                {
                    var score = (float)bodyComposition.GetValue<double>(param.Item.ToString());
                    var blendShapeNames = (string[])param.PositiveScoreArg;

                    foreach (string name in blendShapeNames)
                    {
                        human.SetBlendShape(name, VisceralFatLevelToBlendShapeWeight(score));
                    }
                }
                
                // 体脂肪率スコア
                var fatScore = bodyComposition.FatScore;

                // 体脂肪率スコアに応じた変形.
                foreach (DeformMap param in ftParamList)
                {
                    if (fatScore >= 0)
                    {
                        string[] blendShapeNames = (string[])param.PositiveScoreArg;
                        foreach (string name in blendShapeNames)
                        {
                            human.SetBlendShape(name, MuscleScoreToBlendShapeWeight(fatScore));
                        }
                    }
                    else
                    {
                        // スコアが0未満の場合はアルファ値を変更.
                        FatPart part = (FatPart)param.NegativeScoreArg;
                        human.MulFatAlpha(part, FatScoreToAlphaRatio(fatScore));
                    }
                }
            }
        }

        /// <summary>
        /// 筋肉量スコアのブレンドシェイプ値変換
        /// </summary>
        /// <param name="muscleScore"></param>
        /// <returns>ブレンドシェイプ値(0～100)</returns>
        private int MuscleScoreToBlendShapeWeight(int muscleScore)
        {
            return (int)(Normalize(Math.Abs(muscleScore), 0, 4) * BlendShapeScale);
        }
        
        /// <summary>
        /// 体脂肪スコアをアルファ値変換
        /// スコア(0～-4)をアルファ値(1～0)に変換する
        /// </summary>
        /// <param name="fatScore"></param>
        /// <returns>アルファ値(0～1)</returns>
        private float FatScoreToAlphaRatio(int fatScore)
        {
            return Normalize(fatScore, -4, 0);
        }

        /// <summary>
        /// 内臓脂肪レベルからブレンドシェイプ値への変換.
        /// </summary>
        /// <param name="visceralFatLevel"></param>
        /// <returns></returns>
        private int VisceralFatLevelToBlendShapeWeight(float visceralFatLevel)
        {
            return (int)(Normalize(visceralFatLevel, 9, 15) * BlendShapeScale);
        }
        
        /// <summary>
        /// src値の正規化処理.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private float Normalize(float src, float min, float max)
        {
            return Mathf.Clamp((src - min) / (max - min), 0, 1);
        }

        /// <summary>
        /// 体型判定で使用する測定箇所.
        /// </summary>
        private enum MeasurementParts : byte
        {
            Inseam = 0,
            Sleeve = 1
        }

        /// <summary>
        /// 体型判定用閾値テーブル
        /// 標準の値を基準として、[短め=標準の95%] [長め=標準の105%]とした値
        /// </summary>
        private static readonly Dictionary<int, float> DiscriminationThreshold = new Dictionary<int, float> {
            {GetTableKey(Gender.Male,    MeasurementParts.Inseam, BodyType.Short),   0.424f },    // 男性/股下割合 短め
            {GetTableKey(Gender.Male,    MeasurementParts.Inseam, BodyType.Normal),  0.446f },    // 男性/股下割合 標準
            {GetTableKey(Gender.Male,    MeasurementParts.Inseam, BodyType.Long),    0.469f },    // 男性/股下割合 長め
            {GetTableKey(Gender.Male,    MeasurementParts.Sleeve, BodyType.Short),   0.306f },    // 男性/袖丈割合 短め
            {GetTableKey(Gender.Male,    MeasurementParts.Sleeve, BodyType.Normal),  0.322f },    // 男性/袖丈割合 標準
            {GetTableKey(Gender.Male,    MeasurementParts.Sleeve, BodyType.Long),    0.338f },    // 男性/袖丈割合 長め
            {GetTableKey(Gender.Female,  MeasurementParts.Inseam, BodyType.Short),   0.425f },    // 女性/股下割合 短め
            {GetTableKey(Gender.Female,  MeasurementParts.Inseam, BodyType.Normal),  0.447f },    // 女性/股下割合 標準
            {GetTableKey(Gender.Female,  MeasurementParts.Inseam, BodyType.Long),    0.469f },    // 女性/股下割合 長め
            {GetTableKey(Gender.Female,  MeasurementParts.Sleeve, BodyType.Short),   0.301f },    // 女性/股下割合 短め
            {GetTableKey(Gender.Female,  MeasurementParts.Sleeve, BodyType.Normal),  0.317f },    // 女性/股下割合 標準
            {GetTableKey(Gender.Female,  MeasurementParts.Sleeve, BodyType.Long),    0.332f }     // 女性/股下割合 長め
        };

        /// <summary>
        /// 体型IDの取得.
        /// </summary>
        /// <param name="gender"></param>
        /// <param name="parts"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static int GetTableKey(Gender gender, MeasurementParts parts, BodyType type)
        {
            return (((byte)gender) << 16) | (((byte)parts) << 8) | ((byte)type);
        }

        /// <summary>
        /// 体型の判定処理.
        /// </summary>
        /// <param name="gender">性別</param>
        /// <param name="height">身長</param>
        /// <param name="inseamLength">股下高</param>
        /// <param name="sleeveLength">袖丈</param>
        /// <returns></returns>
        public static BodyType GetBodyType(Gender gender, float height, float inseamLength, float sleeveLength)
        {
            Assert.IsTrue(height > 0);
            Assert.IsTrue(inseamLength > 0);
            Assert.IsTrue(sleeveLength > 0);

            float inseamRatio = inseamLength / height;
            float sleeveRatio = sleeveLength / height;

            float inseamRatioShort = DiscriminationThreshold[GetTableKey(gender, MeasurementParts.Inseam, BodyType.Short)];
            float sleeveRatioShort = DiscriminationThreshold[GetTableKey(gender, MeasurementParts.Sleeve, BodyType.Short)];
            float inseamRatioLong = DiscriminationThreshold[GetTableKey(gender, MeasurementParts.Inseam, BodyType.Long)];
            float sleeveRatioLong = DiscriminationThreshold[GetTableKey(gender, MeasurementParts.Sleeve, BodyType.Long)];

            BodyType type = BodyType.Normal;
            if (inseamRatio >= inseamRatioLong && sleeveRatio >= sleeveRatioLong)
            {
                type = BodyType.Long;
            }
            else if (inseamRatio <= inseamRatioShort || sleeveRatio <= sleeveRatioShort)
            {
                type = BodyType.Short;
            }
            return type;
        }
    }
}