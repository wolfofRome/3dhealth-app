using Assets.Scripts.Database.DataRow;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public enum MeasurementItem
    {
        ResourceId,
        Height,
        Inseam,
        Neck,
        Chest,
        MinWaist,
        MaxWaist,
        Hip,
        LeftThigh,
        RightThigh,
        LeftShoulder,
        RightShoulder,
        LeftSleeve,
        RightSleeve,
        LeftArm,
        RightArm,
        LeftCalf,
        RightCalf,
        Type,
    }

    public abstract class BaseMeasurementDataLoader : MonoBehaviour
    {
        private static readonly Dictionary<MeasurementItem, string> CsvKeys = new Dictionary<MeasurementItem, string>()
        {
            { MeasurementItem.ResourceId, "resource_id"},
            { MeasurementItem.Height, "身長"},
            { MeasurementItem.Inseam, "股下高"},
            { MeasurementItem.Neck, "頚囲"},
            { MeasurementItem.Chest, "胸囲"},
            { MeasurementItem.MinWaist, "ウエスト囲(水平)"},
            { MeasurementItem.MaxWaist, "ウエスト囲(最大)"},
            { MeasurementItem.Hip, "ヒップ"},
            { MeasurementItem.LeftThigh, "大腿囲(左)"},
            { MeasurementItem.RightThigh, "大腿囲(右)"},
            { MeasurementItem.LeftShoulder, "肩幅(左)"},
            { MeasurementItem.RightShoulder, "肩幅(右)"},
            { MeasurementItem.LeftSleeve, "袖丈(左)"},
            { MeasurementItem.RightSleeve, "袖丈(右)"},
            { MeasurementItem.LeftArm, "上腕最大囲(左)"},
            { MeasurementItem.RightArm, "上腕最大囲(右)"},
            { MeasurementItem.LeftCalf, "ふくらはぎ(左)"},
            { MeasurementItem.RightCalf, "ふくらはぎ(右)"},
            { MeasurementItem.Type, "measurement_type"},
        };

        protected virtual void Awake()
        {
        }

        protected static Measurement ParseCsvRow(Dictionary<string, string> csvRow)
        {
            return new Measurement
            {
                ResourceId = ParseCsvValue(csvRow, MeasurementItem.ResourceId, 0),
                Height = ParseCsvValue(csvRow, MeasurementItem.Height, 0),
                Inseam = ParseCsvValue(csvRow, MeasurementItem.Inseam, 0),
                Neck = ParseCsvValue(csvRow, MeasurementItem.Neck, 0),
                Chest = ParseCsvValue(csvRow, MeasurementItem.Chest, 0),
                MinWaist = ParseCsvValue(csvRow, MeasurementItem.MinWaist, 0),
                MaxWaist = ParseCsvValue(csvRow, MeasurementItem.MaxWaist, 0),
                Hip = ParseCsvValue(csvRow, MeasurementItem.Hip, 0),
                LeftThigh = ParseCsvValue(csvRow, MeasurementItem.LeftThigh, 0),
                RightThigh = ParseCsvValue(csvRow, MeasurementItem.RightThigh, 0),
                LeftShoulder = ParseCsvValue(csvRow, MeasurementItem.LeftShoulder, 0),
                RightShoulder = ParseCsvValue(csvRow, MeasurementItem.RightShoulder, 0),
                LeftSleeve = ParseCsvValue(csvRow, MeasurementItem.LeftSleeve, 0),
                RightSleeve = ParseCsvValue(csvRow, MeasurementItem.RightSleeve, 0),
                LeftArm = ParseCsvValue(csvRow, MeasurementItem.LeftArm, 0),
                RightArm = ParseCsvValue(csvRow, MeasurementItem.RightArm, 0),
                LeftCalf = ParseCsvValue(csvRow, MeasurementItem.LeftCalf, 0),
                RightCalf = ParseCsvValue(csvRow, MeasurementItem.RightCalf, 0),
                Type = ParseCsvValue(csvRow, MeasurementItem.Type, Measurement.TypeSpaceVision),
            };
        }

        private static T ParseCsvValue<T>(IReadOnlyDictionary<string, string> csvRow, MeasurementItem part, T defaultValue)
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

        private static string GetCsvDataKey(MeasurementItem part)
        {
            return CsvKeys[part];
        }
    }
}