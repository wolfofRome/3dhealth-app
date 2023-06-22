using System;
using System.Collections.Generic;
using Assets.Scripts.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.History
{
    public enum DataType
    {
        BodyComposition,
        Size
    }

    public enum MeasurementItem
    {
        Weight,
        FatPercentage,
        BasalMetabolism,
        TrunkFat,
        TrunkMuscle,
        RightArmFat,
        RightArmMuscle,
        LeftArmFat,
        LeftArmMuscle,
        RightLegFat,
        RightLegMuscle,
        LeftLegFat,
        LeftLegMuscle,
        Height,
        Inseam,
        Neck,
        Chest,
        MinWaist,
        Hip,
        LeftThigh,
        RightThigh,
        LeftShoulder,
        RightShoulder,
        LeftSleeve,
        RightSleeve,
        LeftArm,
        RightArm,
        Fat,
        Muscle,
        LeftCalf,
        RightCalf,
        Skeletal,
        VisceralFatLevel,
        Bmi,
        MaxWaist
    }


    public static class DataTypeExtension
    {
        private class Config {
            public string title{ get; set; }
            public DataType type { get; set; }

            public Config(string title, DataType type)
            {
                this.title = title;
                this.type = type;
            }
        }

        private static readonly Dictionary<MeasurementItem, Config> _titleMap = new Dictionary<MeasurementItem, Config>
        {
            /* // 体組成データ
            { MeasurementItem.Weight,           new Config("体重", DataType.BodyComposition) },
            { MeasurementItem.FatPercentage,    new Config("体脂肪率", DataType.BodyComposition) },
            { MeasurementItem.BasalMetabolism,  new Config("基礎代謝", DataType.BodyComposition) },
            { MeasurementItem.TrunkFat,         new Config("体脂肪率(体幹)", DataType.BodyComposition) },
            { MeasurementItem.TrunkMuscle,      new Config("筋肉量(体幹)", DataType.BodyComposition) },
            { MeasurementItem.RightArmFat,      new Config("体脂肪率(右腕)", DataType.BodyComposition)},
            { MeasurementItem.RightArmMuscle,   new Config("筋肉量(右腕)", DataType.BodyComposition) },
            { MeasurementItem.LeftArmFat,       new Config("体脂肪率(左腕)", DataType.BodyComposition) },
            { MeasurementItem.LeftArmMuscle,    new Config("筋肉量(左腕)", DataType.BodyComposition) },
            { MeasurementItem.RightLegFat,      new Config("体脂肪率(右足)", DataType.BodyComposition) },
            { MeasurementItem.RightLegMuscle,   new Config("筋肉量(右足)", DataType.BodyComposition) },
            { MeasurementItem.LeftLegFat,       new Config("体脂肪率(左足)", DataType.BodyComposition) },
            { MeasurementItem.LeftLegMuscle,    new Config("筋肉量(左足)", DataType.BodyComposition) },
            { MeasurementItem.Fat,              new Config("体脂肪量", DataType.BodyComposition) },
            { MeasurementItem.Muscle,           new Config("筋肉量", DataType.BodyComposition) },
            { MeasurementItem.Skeletal,         new Config("骨格量", DataType.BodyComposition) },
            { MeasurementItem.VisceralFatLevel, new Config("内臓脂肪レベル", DataType.BodyComposition) },
            { MeasurementItem.Bmi,              new Config("BMI", DataType.BodyComposition) },

            //採寸データ
            { MeasurementItem.Height,           new Config("身長", DataType.Size) },
            { MeasurementItem.Inseam,           new Config("股下高", DataType.Size) },
            { MeasurementItem.Neck,             new Config("首回り", DataType.Size) },
            { MeasurementItem.Chest,            new Config("バスト", DataType.Size) },
            { MeasurementItem.Waist,            new Config("ウエスト", DataType.Size) },
            { MeasurementItem.Hip,              new Config("ヒップ", DataType.Size) },
            { MeasurementItem.LeftShoulder,     new Config("肩幅(左)", DataType.Size) },
            { MeasurementItem.RightShoulder,    new Config("肩幅(右)", DataType.Size) },
            { MeasurementItem.LeftSleeve,       new Config("袖丈(左)", DataType.Size) },
            { MeasurementItem.RightSleeve,      new Config("袖丈(右)", DataType.Size) },
            { MeasurementItem.LeftArm,          new Config("二の腕(左)", DataType.Size) },
            { MeasurementItem.RightArm,         new Config("二の腕(右)", DataType.Size)},
            { MeasurementItem.LeftThigh,        new Config("太もも(左)", DataType.Size) },
            { MeasurementItem.RightThigh,       new Config("太もも(右)", DataType.Size) },
            { MeasurementItem.LeftCalf,         new Config("ふくらはぎ(左)", DataType.Size) },
            { MeasurementItem.RightCalf,        new Config("ふくらはぎ(右)", DataType.Size) },//*/
            // 体組成データ
            { MeasurementItem.Weight,           new Config(HistoryTitles.bc[0], DataType.BodyComposition) },
            { MeasurementItem.FatPercentage,    new Config(HistoryTitles.bc[1], DataType.BodyComposition) },
            { MeasurementItem.BasalMetabolism,  new Config(HistoryTitles.bc[2], DataType.BodyComposition) },
            { MeasurementItem.TrunkFat,         new Config(HistoryTitles.bc[3], DataType.BodyComposition) },
            { MeasurementItem.TrunkMuscle,      new Config(HistoryTitles.bc[4], DataType.BodyComposition) },
            { MeasurementItem.RightArmFat,      new Config(HistoryTitles.bc[5], DataType.BodyComposition)},
            { MeasurementItem.RightArmMuscle,   new Config(HistoryTitles.bc[6], DataType.BodyComposition) },
            { MeasurementItem.LeftArmFat,       new Config(HistoryTitles.bc[7], DataType.BodyComposition) },
            { MeasurementItem.LeftArmMuscle,    new Config(HistoryTitles.bc[8], DataType.BodyComposition) },
            { MeasurementItem.RightLegFat,      new Config(HistoryTitles.bc[9], DataType.BodyComposition) },
            { MeasurementItem.RightLegMuscle,   new Config(HistoryTitles.bc[10], DataType.BodyComposition) },
            { MeasurementItem.LeftLegFat,       new Config(HistoryTitles.bc[11], DataType.BodyComposition) },
            { MeasurementItem.LeftLegMuscle,    new Config(HistoryTitles.bc[12], DataType.BodyComposition) },
            { MeasurementItem.Fat,              new Config(HistoryTitles.bc[13], DataType.BodyComposition) },
            { MeasurementItem.Muscle,           new Config(HistoryTitles.bc[14], DataType.BodyComposition) },
            { MeasurementItem.Skeletal,         new Config(HistoryTitles.bc[15], DataType.BodyComposition) },
            { MeasurementItem.VisceralFatLevel, new Config(HistoryTitles.bc[16], DataType.BodyComposition) },
            { MeasurementItem.Bmi,              new Config(HistoryTitles.bc[17], DataType.BodyComposition) },

            //採寸データ
            { MeasurementItem.Height,           new Config(HistoryTitles.size[0], DataType.Size) },
            { MeasurementItem.Inseam,           new Config(HistoryTitles.size[1], DataType.Size) },
            { MeasurementItem.Neck,             new Config(HistoryTitles.size[2], DataType.Size) },
            { MeasurementItem.Chest,            new Config(HistoryTitles.size[3], DataType.Size) },
            { MeasurementItem.MinWaist,         new Config(HistoryTitles.size[4], DataType.Size) },
            { MeasurementItem.Hip,              new Config(HistoryTitles.size[5], DataType.Size) },
            { MeasurementItem.LeftShoulder,     new Config(HistoryTitles.size[6], DataType.Size) },
            { MeasurementItem.RightShoulder,    new Config(HistoryTitles.size[7], DataType.Size) },
            { MeasurementItem.LeftSleeve,       new Config(HistoryTitles.size[8], DataType.Size) },
            { MeasurementItem.RightSleeve,      new Config(HistoryTitles.size[9], DataType.Size) },
            { MeasurementItem.LeftArm,          new Config(HistoryTitles.size[10], DataType.Size) },
            { MeasurementItem.RightArm,         new Config(HistoryTitles.size[11], DataType.Size)},
            { MeasurementItem.LeftThigh,        new Config(HistoryTitles.size[12], DataType.Size) },
            { MeasurementItem.RightThigh,       new Config(HistoryTitles.size[13], DataType.Size) },
            { MeasurementItem.LeftCalf,         new Config(HistoryTitles.size[14], DataType.Size) },
            { MeasurementItem.RightCalf,        new Config(HistoryTitles.size[15], DataType.Size) },
            { MeasurementItem.MaxWaist,         new Config(HistoryTitles.size[16], DataType.Size) }
        };

        public static string GetTitle(this MeasurementItem type)
        {
            return _titleMap[type].title;
        }

        public static DataType GetDataType(this MeasurementItem type)
        {
            return _titleMap[type].type;
        }
    }

    [Serializable]
    public class HistoryLoadParam : BaseSceneLoadParam
    {
        [Serializable]
        public class Argment : BaseArgment
        {
            [SerializeField]
            private MeasurementItem _dispItem;
            public MeasurementItem dispItem {
                get {
                    return _dispItem;
                }
                set {
                    _dispItem = value;
                }
            }
            
            private DateTime _selectedDate;
            public DateTime selectedDate {
                get {
                    return _selectedDate;
                }
                set {
                    _selectedDate = value;
                }
            }
        }

        [SerializeField]
        private Argment _argment;
        public Argment argment {
            get {
                return _argment;
            }
            set {
                _argment = value;
            }
        }

        [SceneName]
        private string _nextSceneName = "History";

        public override BaseArgment GetArgment()
        {
            return _argment;
        }

        public override LoadSceneMode mode
        {
            get
            {
                return LoadSceneMode.Additive;
            }
        }

        public override string GetNextSceneName()
        {
            return _nextSceneName;
        }
    }
}
