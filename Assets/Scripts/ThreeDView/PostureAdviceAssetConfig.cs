using Assets.Scripts.Common;
using Assets.Scripts.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.ThreeDView {
    public enum PostureAdviceTraining : int {
        /// <summary>
        /// CAT運動.
        /// </summary>
        Cat,

        /// <summary>
        /// Cross Crunch（左手右膝）.
        /// </summary>
        CrossCrunchLeftHandRightKnee,

        /// <summary>
        /// Cross Crunch（右手左膝）.
        /// </summary>
        CrossCrunchRightHandLeftKnee,

        /// <summary>
        /// 股関節外転筋エクササイズ（左足）.
        /// </summary>
        HipAbductorMuscleLeft,

        /// <summary>
        /// 股関節外転筋エクササイズ（右足）.
        /// </summary>
        HipAbductorMuscleRight,

        /// <summary>
        /// 股関節内転エクササイズ.
        /// </summary>
        HipJointInternalRotation,

        /// <summary>
        /// Knee to Chest
        /// </summary>
        KneeToChest,

        /// <summary>
        /// Knee to Elbow（左手右膝）.
        /// </summary>
        KneeToElbowLeftHandRightKnee,

        /// <summary>
        /// Knee to elbow（右手左膝）.
        /// </summary>
        KneeToElbowRightHandLeftKnee,

        /// <summary>
        /// 側屈・左中殿筋トレーニング.
        /// </summary>
        SideBendingMidlineLeft,

        /// <summary>
        /// 側屈・右中殿筋トレーニング.
        /// </summary>
        SideBendingMidlineRight,

        /// <summary>
        /// Sit up.
        /// </summary>
        SitUp,

        /// <summary>
        /// チューブ開き運動
        /// </summary>
        TubeOpening,

        /// <summary>
        /// 肩回し運動（チューブ）.
        /// </summary>
        TubeShoulderRotation,

        /// <summary>
        /// Knee to Elbow（CrossCrunch（左手右膝））.
        /// </summary>
        KneeToElbowCrossCrunchLeftHandRightKnee,

        /// <summary>
        /// Knee to elbow（CrossCrunch（右手左膝））.
        /// </summary>
        KneeToElbowCrossCrunchRightHandLeftKnee,

        /// <summary>
        /// 股関節外転筋エクササイズ.
        /// </summary>
        HipAbductorMuscle,
    }

    public static class PostureAdviceTrainingExtension {
        private struct TrainingInfo {
            public string name { get; set; }
            public string description { get; set; }

            public TrainingInfo(string name, string description) {
                this.name = name;
                this.description = description;
            }
        }

        /// <summary>
        /// アドバイス用トレーニングのテーブル.
        /// </summary>
        private static readonly Dictionary<PostureAdviceTraining, TrainingInfo>
            _trainingInfoTable = new Dictionary<PostureAdviceTraining, TrainingInfo>(){
                // TODO:説明文は展開して頂き次第、差し替える.
                { PostureAdviceTraining.Cat,                                        new TrainingInfo("CAT運動", "")},
                { PostureAdviceTraining.CrossCrunchLeftHandRightKnee,               new TrainingInfo("Cross Crunch", "")},
                { PostureAdviceTraining.CrossCrunchRightHandLeftKnee,               new TrainingInfo("Cross Crunch", "")},
                { PostureAdviceTraining.HipAbductorMuscleLeft,                      new TrainingInfo("左足股関節外転筋エクササイズ", "")},
                { PostureAdviceTraining.HipAbductorMuscleRight,                     new TrainingInfo("右足股関節外転筋エクササイズ", "")},
                { PostureAdviceTraining.HipJointInternalRotation,                   new TrainingInfo("股関節内転エクササイズ", "")},
                { PostureAdviceTraining.KneeToChest,                                new TrainingInfo("Knee to Chest", "")},
                { PostureAdviceTraining.KneeToElbowLeftHandRightKnee,               new TrainingInfo("Knee to Elbow（左手右膝）", "")},
                { PostureAdviceTraining.KneeToElbowRightHandLeftKnee,               new TrainingInfo("Knee to elbow（右手左膝）", "")},
                { PostureAdviceTraining.SideBendingMidlineLeft,                     new TrainingInfo("側屈・左中殿筋トレーニング", "")},
                { PostureAdviceTraining.SideBendingMidlineRight,                    new TrainingInfo("側屈・右中殿筋トレーニング", "")},
                { PostureAdviceTraining.SitUp,                                      new TrainingInfo("Sit up", "")},
                { PostureAdviceTraining.TubeOpening,                                new TrainingInfo("チューブ開き運動", "")},
                { PostureAdviceTraining.TubeShoulderRotation,                       new TrainingInfo("肩回し運動（チューブ）", "")},
                { PostureAdviceTraining.KneeToElbowCrossCrunchLeftHandRightKnee,    new TrainingInfo("Knee to Elbow（CrossCrunch）", "")},
                { PostureAdviceTraining.KneeToElbowCrossCrunchRightHandLeftKnee,    new TrainingInfo("Knee to Elbow（CrossCrunch）", "")},
                { PostureAdviceTraining.HipAbductorMuscle,                          new TrainingInfo("股関節外転エクササイズ", "")},
            },
            _trainingInfoTableCHN = new Dictionary<PostureAdviceTraining, TrainingInfo>(){
                { PostureAdviceTraining.Cat,                                        new TrainingInfo("中国式CAT運動", "")},
                { PostureAdviceTraining.CrossCrunchLeftHandRightKnee,               new TrainingInfo("中国式Cross Crunch", "")},
                { PostureAdviceTraining.CrossCrunchRightHandLeftKnee,               new TrainingInfo("中国式Cross Crunch", "")},
                { PostureAdviceTraining.HipAbductorMuscleLeft,                      new TrainingInfo("中国式左足股関節外転筋エクササイズ", "")},
                { PostureAdviceTraining.HipAbductorMuscleRight,                     new TrainingInfo("中国式右足股関節外転筋エクササイズ", "")},
                { PostureAdviceTraining.HipJointInternalRotation,                   new TrainingInfo("中国式股関節内転エクササイズ", "")},
                { PostureAdviceTraining.KneeToChest,                                new TrainingInfo("中国式Knee to Chest", "")},
                { PostureAdviceTraining.KneeToElbowLeftHandRightKnee,               new TrainingInfo("中国式Knee to Elbow（左手右膝）", "")},
                { PostureAdviceTraining.KneeToElbowRightHandLeftKnee,               new TrainingInfo("中国式Knee to elbow（右手左膝）", "")},
                { PostureAdviceTraining.SideBendingMidlineLeft,                     new TrainingInfo("中国式側屈・左中殿筋トレーニング", "")},
                { PostureAdviceTraining.SideBendingMidlineRight,                    new TrainingInfo("中国式側屈・右中殿筋トレーニング", "")},
                { PostureAdviceTraining.SitUp,                                      new TrainingInfo("中国式Sit up", "")},
                { PostureAdviceTraining.TubeOpening,                                new TrainingInfo("中国式チューブ開き運動", "")},
                { PostureAdviceTraining.TubeShoulderRotation,                       new TrainingInfo("中国式肩回し運動（チューブ）", "")},
                { PostureAdviceTraining.KneeToElbowCrossCrunchLeftHandRightKnee,    new TrainingInfo("中国式Knee to Elbow（CrossCrunch）", "")},
                { PostureAdviceTraining.KneeToElbowCrossCrunchRightHandLeftKnee,    new TrainingInfo("中国式Knee to Elbow（CrossCrunch）", "")},
                { PostureAdviceTraining.HipAbductorMuscle,                          new TrainingInfo("中国式股関節外転エクササイズ", "")},
            };

        public static string ToName(this PostureAdviceTraining training) {
            switch (PlayerPrefs.GetString("Lang")) {
                case "Japanese":
                default:
                    return _trainingInfoTable[training].name;
                case "Chinese":
                    return _trainingInfoTableCHN[training].name;
            }
        }

        public static string ToDescription(this PostureAdviceTraining training) {
            switch (PlayerPrefs.GetString("Lang")) {
                case "Japanese":
                default:
                    return _trainingInfoTable[training].description;
                case "Chinese":
                    return _trainingInfoTable[training].description;
            }
        }
    }

    public enum PostureAdviceStretch : int {
        /// <summary>
        /// ハムストリングスストレッチ.
        /// </summary>
        Hamstrings,

        /// <summary>
        /// 股関節開脚ストレッチ.
        /// </summary>
        HipJointOpening,

        /// <summary>
        /// 腸腰筋ストレッチ.
        /// </summary>
        Iliopsoas,

        /// <summary>
        /// 腰ねじりストレッチ（腰部左捻転）.
        /// </summary>
        LumbarTorsionLeft,

        /// <summary>
        /// 腰ねじりストレッチ（腰部右捻転）.
        /// </summary>
        LumbarTorsionRight,

        /// <summary>
        /// 大胸筋ストレッチ.
        /// </summary>
        MajorPectoralMuscle,

        /// <summary>
        /// 頸部後屈ストレッチ(タオル).
        /// </summary>
        NeckBackFlexion,

        /// <summary>
        /// 頸部前屈ストレッチ(タオル).
        /// </summary>
        NeckFrontFlexion,

        /// <summary>
        /// 頸部回旋ストレッチ（頸部左回旋).
        /// </summary>
        NeckRotatedLeft,

        /// <summary>
        /// 頸部回旋ストレッチ（頸部右回旋).
        /// </summary>
        NeckRotatedRight,

        /// <summary>
        /// 側屈ストレッチ(左に倒す).
        /// </summary>
        SideFlexionLeft,

        /// <summary>
        /// 側屈ストレッチ(右に倒す).
        /// </summary>
        SideFlexionRight,

        /// <summary>
        /// 割座.
        /// </summary>
        Split,

        /// <summary>
        /// 僧帽筋上部・肩甲挙筋ストレッチ(左側をストレッチ).
        /// </summary>
        TrapeziusMuscleLeft,

        /// <summary>
        /// 僧帽筋上部・肩甲挙筋ストレッチ(右側をストレッチ).
        /// </summary>
        TrapeziusMuscleRight,

        /// <summary>
        /// 腰方形筋ストレッチ（左腰）.
        /// </summary>
        WaistRectangularMuscleLeft,

        /// <summary>
        /// 腰方形筋ストレッチ（右腰）.
        /// </summary>
        WaistRectangularMuscleRight,

        /// <summary>
        /// ハム前屈ストレッチ(座位).
        /// </summary>
        HamstringsFrontFlexion,
    }

    public static class PostureAdviceStretchExtension {
        private struct StretchInfo {
            public string name { get; set; }
            public string description { get; set; }

            public StretchInfo(string name, string description) {
                this.name = name;
                this.description = description;
            }
        }

        /// <summary>
        /// アドバイス用ストレッチのテーブル.
        /// </summary>
        private static readonly Dictionary<PostureAdviceStretch, StretchInfo>
            _trainingInfoTable = new Dictionary<PostureAdviceStretch, StretchInfo>(){
                // TODO:説明文は展開して頂き次第、差し替える.
                { PostureAdviceStretch.Hamstrings,                  new StretchInfo("ハムストリングスストレッチ", "")},
                { PostureAdviceStretch.HipJointOpening,             new StretchInfo("股関節開脚ストレッチ", "")},
                { PostureAdviceStretch.Iliopsoas,                   new StretchInfo("腸腰筋ストレッチ", "")},
                { PostureAdviceStretch.LumbarTorsionLeft,           new StretchInfo("腰ねじりストレッチ（腰部左捻転）", "")},
                { PostureAdviceStretch.LumbarTorsionRight,          new StretchInfo("腰ねじりストレッチ（腰部右捻転）", "")},
                { PostureAdviceStretch.MajorPectoralMuscle,         new StretchInfo("大胸筋ストレッチ", "")},
                { PostureAdviceStretch.NeckBackFlexion,             new StretchInfo("頸部後屈ストレッチ(タオル)", "")},
                { PostureAdviceStretch.NeckFrontFlexion,            new StretchInfo("頸部前屈ストレッチ(タオル)", "")},
                { PostureAdviceStretch.NeckRotatedLeft,             new StretchInfo("頸部回旋ストレッチ（頸部左回旋)", "")},
                { PostureAdviceStretch.NeckRotatedRight,            new StretchInfo("頸部回旋ストレッチ（頸部右回旋)", "")},
                { PostureAdviceStretch.SideFlexionLeft,             new StretchInfo("側屈ストレッチ(左に倒す)", "")},
                { PostureAdviceStretch.SideFlexionRight,            new StretchInfo("側屈ストレッチ(右に倒す)", "")},
                { PostureAdviceStretch.Split,                       new StretchInfo("割座", "")},
                { PostureAdviceStretch.TrapeziusMuscleLeft,         new StretchInfo("僧帽筋上部・肩甲挙筋ストレッチ(左側をストレッチ)", "")},
                { PostureAdviceStretch.TrapeziusMuscleRight,        new StretchInfo("僧帽筋上部・肩甲挙筋ストレッチ(右側をストレッチ)", "")},
                { PostureAdviceStretch.WaistRectangularMuscleLeft,  new StretchInfo("左腰腰方形筋ストレッチ", "")},
                { PostureAdviceStretch.WaistRectangularMuscleRight, new StretchInfo("右腰部方形筋ストレッチ", "")},
                { PostureAdviceStretch.HamstringsFrontFlexion,      new StretchInfo("ハム前屈ストレッチ(座位)", "")},
            },
            _trainingInfoTableCHN = new Dictionary<PostureAdviceStretch, StretchInfo>(){
                { PostureAdviceStretch.Hamstrings,                  new StretchInfo("中国式ハムストリングスストレッチ", "")},
                { PostureAdviceStretch.HipJointOpening,             new StretchInfo("中国式股関節開脚ストレッチ", "")},
                { PostureAdviceStretch.Iliopsoas,                   new StretchInfo("中国式腸腰筋ストレッチ", "")},
                { PostureAdviceStretch.LumbarTorsionLeft,           new StretchInfo("中国式腰ねじりストレッチ（腰部左捻転）", "")},
                { PostureAdviceStretch.LumbarTorsionRight,          new StretchInfo("中国式腰ねじりストレッチ（腰部右捻転）", "")},
                { PostureAdviceStretch.MajorPectoralMuscle,         new StretchInfo("中国式大胸筋ストレッチ", "")},
                { PostureAdviceStretch.NeckBackFlexion,             new StretchInfo("中国式頸部後屈ストレッチ(タオル)", "")},
                { PostureAdviceStretch.NeckFrontFlexion,            new StretchInfo("中国式頸部前屈ストレッチ(タオル)", "")},
                { PostureAdviceStretch.NeckRotatedLeft,             new StretchInfo("中国式頸部回旋ストレッチ（頸部左回旋)", "")},
                { PostureAdviceStretch.NeckRotatedRight,            new StretchInfo("中国式頸部回旋ストレッチ（頸部右回旋)", "")},
                { PostureAdviceStretch.SideFlexionLeft,             new StretchInfo("中国式側屈ストレッチ(左に倒す)", "")},
                { PostureAdviceStretch.SideFlexionRight,            new StretchInfo("中国式側屈ストレッチ(右に倒す)", "")},
                { PostureAdviceStretch.Split,                       new StretchInfo("中国式割座", "")},
                { PostureAdviceStretch.TrapeziusMuscleLeft,         new StretchInfo("中国式僧帽筋上部・肩甲挙筋ストレッチ(左側をストレッチ)", "")},
                { PostureAdviceStretch.TrapeziusMuscleRight,        new StretchInfo("中国式僧帽筋上部・肩甲挙筋ストレッチ(右側をストレッチ)", "")},
                { PostureAdviceStretch.WaistRectangularMuscleLeft,  new StretchInfo("中国式左腰腰方形筋ストレッチ", "")},
                { PostureAdviceStretch.WaistRectangularMuscleRight, new StretchInfo("中国式右腰部方形筋ストレッチ", "")},
                { PostureAdviceStretch.HamstringsFrontFlexion,      new StretchInfo("中国式ハム前屈ストレッチ(座位)", "")},
            };

        public static string ToName(this PostureAdviceStretch stretch) {
            switch (PlayerPrefs.GetString("Lang")) {
                case "Japanese":
                default:
                    return _trainingInfoTable[stretch].name;
                case "Chinese":
                    return _trainingInfoTableCHN[stretch].name;
            }
        }

        public static string ToDescription(this PostureAdviceStretch stretch) {
            switch (PlayerPrefs.GetString("Lang")) {
                case "Japanese":
                default:
                    return _trainingInfoTable[stretch].description;
                case "Chinese":
                    return _trainingInfoTableCHN[stretch].description;
            }
        }
    }

    public class PostureAdviceAssetConfig : ScriptableObject {
        [Serializable]
        public class TrainingAsset {
            [SerializeField]
            private PostureAdviceTraining _id = default;
            public PostureAdviceTraining id {
                get {
                    return _id;
                }
            }

            [SerializeField]
            private Sprite[] _images = default;
            public Sprite[] images {
                get {
                    return _images;
                }
            }
        }

        [Serializable]
        public class StretchAsset {
            [SerializeField]
            private PostureAdviceStretch _id = default;
            public PostureAdviceStretch id {
                get {
                    return _id;
                }
            }

            [SerializeField]
            private Sprite[] _images = default;
            public Sprite[] images {
                get {
                    return _images;
                }
            }
        }

        /// <summary>
        /// 各症状に対するトレーニングとストレッチのMAP生成用のクラス.
        /// ※詳細は下記資料参照
        ///   (Dropbox)\姿勢機能\アドバイス\症状と施術(写真割り当て).xlsx
        /// </summary>
        [Serializable]
        public class PostureConditionAsset {
            [SerializeField]
            private PostureCondition _id = default;
            public PostureCondition id {
                get {
                    return _id;
                }
            }

            [SerializeField]
            private PostureAdviceTraining _training = default;
            public PostureAdviceTraining training {
                get {
                    return _training;
                }
            }

            [SerializeField]
            private PostureAdviceStretch _stretch = default;
            public PostureAdviceStretch stretch {
                get {
                    return _stretch;
                }
            }
        }

        [SerializeField]
        private List<TrainingAsset> _trainingAssetList = default;

        private Dictionary<PostureAdviceTraining, TrainingAsset> _trainingAssetMap;
        private Dictionary<PostureAdviceTraining, TrainingAsset> trainingAssetMap {
            get {
                return _trainingAssetMap = _trainingAssetMap ?? _trainingAssetList.ToDictionary(x => x.id);
            }
        }

        [SerializeField]
        private List<StretchAsset> _stretchAssetList = default;

        private Dictionary<PostureAdviceStretch, StretchAsset> _stretchAssetMap;
        private Dictionary<PostureAdviceStretch, StretchAsset> stretchAssetMap {
            get {
                return _stretchAssetMap = _stretchAssetMap ?? _stretchAssetList.ToDictionary(x => x.id);
            }
        }

        [SerializeField]
        private List<PostureConditionAsset> _postureConditionAssetList = default;

        private Dictionary<PostureCondition, PostureConditionAsset> _postureConditionAssetMap;
        private Dictionary<PostureCondition, PostureConditionAsset> postureConditionAssetMap {
            get {
                return _postureConditionAssetMap = _postureConditionAssetMap ?? _postureConditionAssetList.ToDictionary(x => x.id);
            }
        }

        /// <summary>
        /// 姿勢検証における正常個所の骨のマテリアル設定.
        /// </summary>
        private MaterialColorSettings _boneNormalMaterialSetting = new MaterialColorSettings(
            new Color(0.7216f, 0.651f, 0.5961f),
            new Color(0, 0, 0));
        public MaterialColorSettings boneNormalMaterialSetting {
            get {
                return _boneNormalMaterialSetting;
            }
        }

        /// <summary>
        /// 姿勢検証における異常個所の骨のマテリアル設定.
        /// </summary>
        private MaterialColorSettings _boneAbnormalMaterialSetting = new MaterialColorSettings(
            new Color(0.8667f, 0.0275f, 0),
            new Color(0.7373f, 0.2588f, 0.2588f));
        public MaterialColorSettings boneAbnormalMaterialSetting {
            get {
                return _boneAbnormalMaterialSetting;
            }
        }

        /// <summary>
        /// 姿勢検証における正常個所の軟骨のマテリアル設定.
        /// </summary>
        private MaterialColorSettings _cartilageNormalMaterialSetting = new MaterialColorSettings(
            new Color(0.9843f, 0.9647f, 0.8471f),
            new Color(0, 0, 0));
        public MaterialColorSettings cartilageNormalMaterialSetting {
            get {
                return _cartilageNormalMaterialSetting;
            }
        }

        /// <summary>
        /// 姿勢検証における異常正常個所の軟骨のマテリアル設定.
        /// </summary>
        private MaterialColorSettings _cartilageAbnormalMaterialSetting = new MaterialColorSettings(
            new Color(1, 0, 0),
            new Color(0.8471f, 0.5098f, 0.5098f));
        public MaterialColorSettings cartilageAbnormalMaterialSetting {
            get {
                return _cartilageAbnormalMaterialSetting;
            }
        }


        private void OnEnable() {
#if UNITY_EDITOR
            DebugUtil.Log(trainingAssetMap.ToString());
            DebugUtil.Log(stretchAssetMap.ToString());
            DebugUtil.Log(postureConditionAssetMap.ToString());
#endif
        }

        /// <summary>
        /// 姿勢異常個所に関連するトレーニング画像の取得.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public TrainingAsset GetTrainingAsset(PostureCondition condition) {
            var postureConditionAsset = GetPostureConditionAsset(condition);
            if (postureConditionAsset != null) {
                TrainingAsset trainingAsset;
                return trainingAssetMap.TryGetValue(postureConditionAsset.training, out trainingAsset) ? trainingAsset : null;
            }
            return null;
        }

        /// <summary>
        /// 姿勢異常個所に関連するストレッチ画像の取得.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public StretchAsset GetStretchAsset(PostureCondition condition) {
            var postureConditionAsset = GetPostureConditionAsset(condition);
            if (postureConditionAsset != null) {
                StretchAsset stretchAsset;
                return stretchAssetMap.TryGetValue(postureConditionAsset.stretch, out stretchAsset) ? stretchAsset : null;
            }
            return null;
        }

        /// <summary>
        /// 姿勢異常個所に関連するストレッチ画像の取得.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public PostureConditionAsset GetPostureConditionAsset(PostureCondition condition) {
            PostureConditionAsset postureConditionAsset;
            return postureConditionAssetMap.TryGetValue(condition, out postureConditionAsset) ? postureConditionAsset : null;
        }
    }
}
