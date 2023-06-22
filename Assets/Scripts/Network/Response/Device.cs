using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Network.Response
{
    /// <summary>
    /// 筐体単位の明るさ設定.
    /// </summary>
    public enum Brightness : int
    {
        /// <summary>
        /// 標準.
        /// </summary>
        Standard = 0,

        /// <summary>
        /// 暗め.
        /// </summary>
        Dark = 1,

        /// <summary>
        /// 明るめ.
        /// </summary>
        Bright = 2
    }

    public static class BrightnessExtension
    {
        private static readonly Dictionary<Brightness, float> _brightnessThresholdMap = new Dictionary<Brightness, float>()
        {
            {Brightness.Standard, 1f},
            {Brightness.Dark, 0.5f },
            {Brightness.Bright, 1.5f }
        };

        private static readonly Dictionary<Brightness, string> _brightnessNameMap = new Dictionary<Brightness, string>()
        {
            {Brightness.Standard, "standard" },
            {Brightness.Dark, "dark" },
            {Brightness.Bright, "bright" }
        };

        public static float ToThreshold(this Brightness brightness)
        {
            return _brightnessThresholdMap[brightness];
        }

        public static string ToString(this Brightness brightness)
        {
            return _brightnessNameMap[brightness];
        }

        public static Brightness Parse(string brightness)
        {
            return _brightnessNameMap.First(x => x.Value == brightness).Key;
        }
    }

    [Serializable]
    public class Device
    {

        [SerializeField]
        private int store_id = default;
        public int StoreId
        {
            get
            {
                return store_id;
            }
            set
            {
                store_id = value;
            }
        }

        [SerializeField]
        private int device_id = default;
        public int DeviceId
        {
            get
            {
                return device_id;
            }
            set
            {
                device_id = value;
            }
        }

        [SerializeField]
        private string name = default;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        [SerializeField]
        private string software = default;
        public string Software
        {
            get
            {
                return software;
            }
            set
            {
                software = value;
            }
        }

        [SerializeField]
        private string specification = default;
        public string Specification
        {
            get
            {
                return specification;
            }
            set
            {
                specification = value;
            }
        }
        
        [SerializeField]
        private string brightness = default;
        public Brightness Brightness
        {
            get
            {
                return BrightnessExtension.Parse(brightness);
            }
            set
            {
                brightness = value.ToString();
            }
        }
    }
}
