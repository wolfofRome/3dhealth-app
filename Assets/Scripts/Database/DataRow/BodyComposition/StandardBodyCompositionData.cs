using System;
using Assets.Scripts.Common;
using UnityEngine;

namespace Database.DataRow.BodyComposition
{
    public class StandardBodyCompositionData
    {
        public enum BodyPart
        {
            Trunk,
            RightArm,
            LeftArm,
            RightLeg,
            LeftLeg
        }
        
        private readonly Gender _gender;
        private readonly int _age;

        public StandardBodyCompositionData(Gender gender, int age)
        {
            _gender = gender;
            _age = age;
        }

        public string GetBodyFatDescription(double bodyFatRate)
        {
            return "年代平均の体脂肪率　" + BodyFat.GetBodyFatDescription(_gender, _age, Mathf.RoundToInt((float)bodyFatRate));
        }

        public string GetBasalMetabolicDescription()
        {
            return "年代平均の基礎代謝 " + BasalMetabolic.GetBasalMetabolicDescription(_gender, _age);
        }
        
        public string GetBmiDescription(double bmi)
        {
            const string defaultText = "BMI ";
            if (bmi < 18.5)
                return defaultText + "痩せ";
            if (bmi >= 18.5 && bmi < 25)
                return defaultText + "標準";
            if (bmi >= 25 && bmi < 30)
                return defaultText + "軽肥満";
            return defaultText + "肥満";
        }

        public string GetVisceralFatLevelDescription(double visceralFatLevel)
        {
            int value = Mathf.RoundToInt((float) visceralFatLevel);
            const string defaultText = "内臓脂肪レベル ";
            if (value >= 1 && value <= 9)
                return defaultText + "標準";
            if (value >= 10 && value <= 14)
                return defaultText + "やや高い";
            if (value >= 15 && value <= 30)
                return defaultText + "高い";
            return defaultText;
        }

        public string GetMuscleMassDescription(BodyPart bodyPart)
        {
            switch (bodyPart)
            {
                case BodyPart.Trunk:
                    return "年代平均の筋肉量（体幹） " + MuscleMass.GetMuscleMassDescription(_gender, _age, bodyPart);
                case BodyPart.RightArm:
                    return "年代平均の筋肉量（左腕） " + MuscleMass.GetMuscleMassDescription(_gender, _age, bodyPart);
                case BodyPart.LeftArm:
                    return "年代平均の筋肉量（右腕） " + MuscleMass.GetMuscleMassDescription(_gender, _age, bodyPart);
                case BodyPart.RightLeg:
                    return "年代平均の筋肉量（左脚） " + MuscleMass.GetMuscleMassDescription(_gender, _age, bodyPart);
                case BodyPart.LeftLeg:
                    return "年代平均の筋肉量（右脚） " + MuscleMass.GetMuscleMassDescription(_gender, _age, bodyPart);
                default:
                    throw new ArgumentOutOfRangeException(nameof(bodyPart), bodyPart, null);
            }
        }
    }
}