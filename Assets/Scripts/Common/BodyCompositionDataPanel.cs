using Assets.Scripts.Common.Config;
using Assets.Scripts.Database.DataRow;
using Assets.Scripts.History;
using Assets.Scripts.Network.Response;
using Database.DataRow.BodyComposition;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common
{
    public class BodyCompositionDataPanel : MonoBehaviour
    {
        [SerializeField]
        protected Text height;
        [SerializeField]
        protected Text weight;
        [SerializeField]
        protected Text fatPercentage;
        [SerializeField]
        protected Text fatPercentageDescription;
        [SerializeField]
        protected Text basalMetabolism;
        [SerializeField]
        protected Text basalMetabolismDescription;
        [SerializeField]
        protected Text fat;
        [SerializeField]
        protected Text muscle;
        [SerializeField]
        protected Text trunkFat;
        [SerializeField]
        protected Text trunkMuscle;
        [SerializeField]
        protected Text trunkMuscleDescription;
        [SerializeField]
        protected Text rightArmFat;
        [SerializeField]
        protected Text rightArmMuscle;
        [SerializeField]
        protected Text rightArmMuscleDescription;
        [SerializeField]
        protected Text leftArmFat;
        [SerializeField]
        protected Text leftArmMuscle;
        [SerializeField]
        protected Text leftArmMuscleDescription;
        [SerializeField]
        protected Text rightLegFat;
        [SerializeField]
        protected Text rightLegMuscle;
        [SerializeField]
        protected Text rightLegMuscleDescription;
        [SerializeField]
        protected Text leftLegFat;
        [SerializeField]
        protected Text leftLegMuscle;
        [SerializeField]
        protected Text leftLegMuscleDescription;
        [SerializeField]
        protected Text skeletal;
        [SerializeField]
        protected Text visceralFatLevel;
        [SerializeField]
        protected Text visceralFatLevelDescription;
        [SerializeField]
        protected Text bmi;
        [SerializeField]
        protected Text bmiDescription;
        [SerializeField]
        protected Text dummy;

        private BodyComposition _bodyComposition;
        private Measurement _measurement;

        [SerializeField]
        private Color descriptionTextColor;

        protected virtual void Start()
        {
        }

        /// <summary>
        /// 体組成データロード開始イベント.
        /// </summary>
        /// <param name="contents"></param>
        public void OnLoadStart(Contents contents)
        {
            var paramList = GetComponentsInChildren<HistoryLoadParam>(true);
            foreach (var param in paramList)
            {
                param.argment.selectedDate = contents.CreateTimeAsDateTime;
            }
        }

        /// <summary>
        /// 体組成データロード完了イベント.
        /// </summary>
        /// <param name="loader"></param>
        public void OnUpdateBodyCompositionData(BodyCompositionDataLoader loader)
        {
            _bodyComposition = loader.BodyComposition;
            ApplyBodyCompositionData();
        }

        /// <summary>
        /// 採寸データロード完了イベント.
        /// </summary>
        /// <param name="loader"></param>
        public void OnUpdateMeasurementData(MeasurementDataLoader loader)
        {
            _measurement = loader.Measurement;
            SetValue(height, _measurement.correctedHeight * AppConst.MeasurementDataScale, "cm");
        }

        /// <summary>
        /// 採寸データの表示更新.
        /// </summary>
        protected virtual void ApplyBodyCompositionData()
        {
            DataManager dataManager = DataManager.Instance;
            int age = dataManager.Profile.GetAge(_bodyComposition.CreateTimeAsDateTime);
            Gender gender = dataManager.Profile.Gender;
            StandardBodyCompositionData standardBodyCompositionData = new StandardBodyCompositionData(gender, age);
            
            SetValue(weight, _bodyComposition.Weight, "kg");
            
            SetValue(fatPercentage, _bodyComposition.FatPercentage, "％");
            SetDescriptionText(fatPercentageDescription, standardBodyCompositionData.GetBodyFatDescription(_bodyComposition.FatPercentage));
            
            SetValue(basalMetabolism, _bodyComposition.BasalMetabolism, "kcal");
            SetDescriptionText(basalMetabolismDescription, standardBodyCompositionData.GetBasalMetabolicDescription());

            switch (_bodyComposition.Type)
            {
                case BodyComposition.TypeMc780:
                    SetValue(trunkMuscle, _bodyComposition.TrunkMuscle, "kg");
                    SetValue(trunkFat, _bodyComposition.TrunkFat, "kg");
                    SetValue(leftArmMuscle, _bodyComposition.LeftArmMuscle, "kg");
                    SetValue(leftArmFat, _bodyComposition.LeftArmFat,"kg");
                    SetValue(rightArmMuscle, _bodyComposition.RightArmMuscle, "kg");
                    SetValue(rightArmFat, _bodyComposition.RightArmFat, "kg");
                    SetValue(leftLegMuscle, _bodyComposition.LeftLegMuscle, "kg");
                    SetValue(leftLegFat, _bodyComposition.LeftLegFat, "kg");
                    SetValue(rightLegMuscle, _bodyComposition.RightLegMuscle, "kg");
                    SetValue(rightLegFat, _bodyComposition.RightLegFat, "kg");
                    
                    SetDescriptionText(trunkMuscleDescription, standardBodyCompositionData.GetMuscleMassDescription(StandardBodyCompositionData.BodyPart.Trunk));
                    SetDescriptionText(leftArmMuscleDescription, standardBodyCompositionData.GetMuscleMassDescription(StandardBodyCompositionData.BodyPart.LeftArm));
                    SetDescriptionText(rightArmMuscleDescription, standardBodyCompositionData.GetMuscleMassDescription(StandardBodyCompositionData.BodyPart.RightArm));
                    SetDescriptionText(leftLegMuscleDescription, standardBodyCompositionData.GetMuscleMassDescription(StandardBodyCompositionData.BodyPart.LeftLeg));
                    SetDescriptionText(rightLegMuscleDescription, standardBodyCompositionData.GetMuscleMassDescription(StandardBodyCompositionData.BodyPart.RightLeg));
                    break;
                    
                case BodyComposition.TypeIScale10:
                    SetValue(fat, _bodyComposition.Fat, "kg");
                    SetValue(muscle, _bodyComposition.Muscle, "kg");
                    SetValue(skeletal, _bodyComposition.Skeletal, "kg");
                    SetValue(visceralFatLevel, _bodyComposition.VisceralFatLevel, "");
                    SetValue(bmi, _bodyComposition.Bmi, "");
                    SetValue(dummy, null, "");

                    SetDescriptionText(visceralFatLevelDescription, standardBodyCompositionData.GetVisceralFatLevelDescription(_bodyComposition.VisceralFatLevel));
                    SetDescriptionText(bmiDescription, standardBodyCompositionData.GetBmiDescription(_bodyComposition.Bmi));
                    break;
            }
        }

        private void SetDescriptionText(Text text, string value)
        {
            if (text == null) return;
            text.gameObject.transform.parent.gameObject.SetActive(true);
            text.color = descriptionTextColor;
            text.text = value;
        }

        /// <summary>
        /// 体組成データのテキスト設定.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <param name="unitText"></param>
        private static void SetValue(Text text, double? value, string unitText)
        {
            if (text == null) return;
            text.gameObject.transform.parent.gameObject.SetActive(true);

            if (value != null) 
            {
                text.text = FormatValue((double)value) + unitText;
            }
        }

        /// <summary>
        /// 体組成データの表示フォーマット変換.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string FormatValue(double value)
        {
            return $"{value:0.0}";
        }
    }
}