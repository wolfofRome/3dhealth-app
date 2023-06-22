using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Assets.Scripts.Database.DataRow;
using System;
using System.Linq;
using Database.DataRow.BodyComposition;

namespace Assets.Scripts.Common
{
    public enum Gender : int
    {
        Male = 1,
        Female = 2,
        Unknown = 3
    }

    public static class GenderExtension
    {
        private static readonly Dictionary<Gender, string> _genderTable = new Dictionary<Gender, string>()
        {
            {Gender.Male, "male" },
            {Gender.Female, "female" },
            {Gender.Unknown, "unknown" }
        };

        public static string ToString(this Gender gender)
        {
            return _genderTable[gender];
        }

        public static Gender Parse(string gender)
        {
            return _genderTable.First(x => x.Value == gender).Key;
        }
    }

    /// <summary>
    /// 体組成データの項目.
    /// </summary>
    /// <remarks> 
    /// リフレクションを使用するため<see cref="BodyComposition"/>のプロパティ名と合わせる.
    /// </remarks>
    public enum BodyCompositionItem
    {
        ResourceId = 0,
        Weight = 1,
        FatPercentage = 2,
        BasalMetabolism = 3,
        TrunkMuscle = 4,
        TrunkMuscleScore = 5,
        TrunkFat = 6,
        TrunkFatPercentage = 7,
        TrunkFatScore = 8,
        LeftArmMuscle = 9,
        LeftArmMuscleScore = 10,
        LeftArmFat = 11,
        LeftArmFatPercentage = 12,
        LeftArmFatScore = 13,
        RightArmMuscle = 14,
        RightArmMuscleScore = 15,
        RightArmFat = 16,
        RightArmFatPercentage = 17,
        RightArmFatScore = 18,
        LeftLegMuscle = 19,
        LeftLegMuscleScore = 20,
        LeftLegFat = 21,
        LeftLegFatPercentage = 22,
        LeftLegFatScore = 23,
        RightLegMuscle = 24,
        RightLegMuscleScore = 25,
        RightLegFat = 26,
        RightLegFatPercentage = 27,
        RightLegFatScore = 28,
        VisceralFatLevel = 29,
        Fat = 30,
        Muscle = 31,
        Type = 32,
        Skeletal = 33,
        Bmi = 34
    }

    public abstract class BaseBodyCompositionDataLoader : MonoBehaviour
    {
        private static readonly Dictionary<BodyCompositionItem, string> _csvKeys = new Dictionary<BodyCompositionItem, string>()
        {
            { BodyCompositionItem.ResourceId,           "resource_id"},
            { BodyCompositionItem.Weight,               "体重"},
            { BodyCompositionItem.FatPercentage,        "体脂肪率"},
            { BodyCompositionItem.BasalMetabolism,      "基礎代謝量"},
            { BodyCompositionItem.TrunkMuscle,          "体幹部－筋肉量"},
            { BodyCompositionItem.TrunkMuscleScore,     "体幹部－筋肉量スコア"},
            { BodyCompositionItem.TrunkFat,             "体幹部－脂肪量"},
            { BodyCompositionItem.TrunkFatPercentage,   "体幹部－体脂肪率"},
            { BodyCompositionItem.TrunkFatScore,        "体幹部－体脂肪率スコア"},
            { BodyCompositionItem.LeftArmMuscle,        "左腕－筋肉量"},
            { BodyCompositionItem.LeftArmMuscleScore,   "左腕－筋肉量スコア"},
            { BodyCompositionItem.LeftArmFat,           "左腕－脂肪量"},
            { BodyCompositionItem.LeftArmFatPercentage, "左腕－体脂肪率"},
            { BodyCompositionItem.LeftArmFatScore,      "左腕－体脂肪率スコア"},
            { BodyCompositionItem.RightArmMuscle,       "右腕－筋肉量"},
            { BodyCompositionItem.RightArmMuscleScore,  "右腕－筋肉量スコア"},
            { BodyCompositionItem.RightArmFat,          "右腕－脂肪量"},
            { BodyCompositionItem.RightArmFatPercentage,"右腕－体脂肪率"},
            { BodyCompositionItem.RightArmFatScore,     "右腕－体脂肪率スコア"},
            { BodyCompositionItem.LeftLegMuscle,        "左足－筋肉量"},
            { BodyCompositionItem.LeftLegMuscleScore,   "左足－筋肉量スコア"},
            { BodyCompositionItem.LeftLegFat,           "左足－脂肪量"},
            { BodyCompositionItem.LeftLegFatPercentage, "左足－体脂肪率"},
            { BodyCompositionItem.LeftLegFatScore,      "左足－体脂肪率スコア"},
            { BodyCompositionItem.RightLegMuscle,       "右足－筋肉量"},
            { BodyCompositionItem.RightLegMuscleScore,  "右足－筋肉量スコア"},
            { BodyCompositionItem.RightLegFat,          "右足－脂肪量"},
            { BodyCompositionItem.RightLegFatPercentage,"右足－体脂肪率"},
            { BodyCompositionItem.RightLegFatScore,     "右足－体脂肪率スコア"},
            { BodyCompositionItem.VisceralFatLevel,     "内臓脂肪レベル"},
            { BodyCompositionItem.Fat,                  "脂肪量"},
            { BodyCompositionItem.Muscle,               "筋肉量"},
            { BodyCompositionItem.Type,                 "body_composition_type"},
            { BodyCompositionItem.Skeletal,             "骨格量"},
            { BodyCompositionItem.Bmi,                  "BMI"},
        };

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {

        }

        protected static BodyComposition PerseCsvRow(Dictionary<string, string> csvRow)
        {
            return new BodyComposition
            {
                ResourceId = ParseCsvValue(csvRow, BodyCompositionItem.ResourceId, 0),
                Weight = ParseCsvValue(csvRow, BodyCompositionItem.Weight, 0.0),
                FatPercentage = ParseCsvValue(csvRow, BodyCompositionItem.FatPercentage, 0.0),
                BasalMetabolism = ParseCsvValue(csvRow, BodyCompositionItem.BasalMetabolism, 0),
                TrunkMuscle = ParseCsvValue(csvRow, BodyCompositionItem.TrunkMuscle, 0.0),
                TrunkMuscleScore = ParseCsvValue(csvRow, BodyCompositionItem.TrunkMuscleScore, 0),
                TrunkFat = ParseCsvValue(csvRow, BodyCompositionItem.TrunkFat, 0.0),
                TrunkFatPercentage = ParseCsvValue(csvRow, BodyCompositionItem.TrunkFatPercentage, 0.0),
                TrunkFatScore = ParseCsvValue(csvRow, BodyCompositionItem.TrunkFatScore, 0),
                LeftArmMuscle = ParseCsvValue(csvRow, BodyCompositionItem.LeftArmMuscle, 0.0),
                LeftArmMuscleScore = ParseCsvValue(csvRow, BodyCompositionItem.LeftArmMuscleScore, 0),
                LeftArmFat = ParseCsvValue(csvRow, BodyCompositionItem.LeftArmFat, 0.0),
                LeftArmFatPercentage = ParseCsvValue(csvRow, BodyCompositionItem.LeftArmFatPercentage, 0.0),
                LeftArmFatScore = ParseCsvValue(csvRow, BodyCompositionItem.LeftArmFatScore, 0),
                RightArmMuscle = ParseCsvValue(csvRow, BodyCompositionItem.RightArmMuscle, 0.0),
                RightArmMuscleScore = ParseCsvValue(csvRow, BodyCompositionItem.RightArmMuscleScore, 0),
                RightArmFat = ParseCsvValue(csvRow, BodyCompositionItem.RightArmFat, 0.0),
                RightArmFatPercentage = ParseCsvValue(csvRow, BodyCompositionItem.RightArmFatPercentage, 0.0),
                RightArmFatScore = ParseCsvValue(csvRow, BodyCompositionItem.RightArmFatScore, 0),
                LeftLegMuscle = ParseCsvValue(csvRow, BodyCompositionItem.LeftLegMuscle, 0.0),
                LeftLegMuscleScore = ParseCsvValue(csvRow, BodyCompositionItem.LeftLegMuscleScore, 0),
                LeftLegFat = ParseCsvValue(csvRow, BodyCompositionItem.LeftLegFat, 0.0),
                LeftLegFatPercentage = ParseCsvValue(csvRow, BodyCompositionItem.LeftLegFatPercentage, 0.0),
                LeftLegFatScore = ParseCsvValue(csvRow, BodyCompositionItem.LeftLegFatScore, 0),
                RightLegMuscle = ParseCsvValue(csvRow, BodyCompositionItem.RightLegMuscle, 0.0),
                RightLegMuscleScore = ParseCsvValue(csvRow, BodyCompositionItem.RightLegMuscleScore, 0),
                RightLegFat = ParseCsvValue(csvRow, BodyCompositionItem.RightLegFat, 0.0),
                RightLegFatPercentage = ParseCsvValue(csvRow, BodyCompositionItem.RightLegFatPercentage, 0.0),
                RightLegFatScore = ParseCsvValue(csvRow, BodyCompositionItem.RightLegFatScore, 0),
                VisceralFatLevel = ParseCsvValue(csvRow, BodyCompositionItem.VisceralFatLevel, 0),
                Fat = ParseCsvValue(csvRow, BodyCompositionItem.Fat, 0.0),
                Muscle = ParseCsvValue(csvRow, BodyCompositionItem.Muscle, 0.0),
                Type = ParseCsvValue(csvRow, BodyCompositionItem.Type, BodyComposition.TypeMc780),
                Skeletal = ParseCsvValue(csvRow, BodyCompositionItem.Skeletal, 0.0),
                Bmi = ParseCsvValue(csvRow, BodyCompositionItem.Bmi, 0.0),
            };
        }

        private static T ParseCsvValue<T>(Dictionary<string, string> csvRow, BodyCompositionItem part, T defaultValue)
        {
            T value = defaultValue;
            var key = GetCsvDataKey(part);

            try
            {
                if (csvRow.ContainsKey(key) && !string.IsNullOrEmpty(csvRow[key]))
                {
                    value = (T)Convert.ChangeType(csvRow[key], value.GetType());
                }
            }
            catch
            {
                throw new Exception("ParseCsvValue failed -> " + key + " type -> " + value.GetType() + " value -> [" + csvRow[key] + "]");
            }

            return value;
        }

        private static string GetCsvDataKey(BodyCompositionItem part)
        {
            return _csvKeys[part];
        }
    }
}