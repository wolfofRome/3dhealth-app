using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace Assets.Scripts.Network.Response
{
    [Serializable]
    public class Contents
    {
        [SerializeField]
        private int resource_id = default;
        public int ResourceId {
            get {
                return resource_id;
            }
            set {
                resource_id = value;
            }
        }

        [SerializeField]
        private long create_time = default;
        public long CreateTime {
            get {
                return create_time;
            }
            set {
                create_time = value;
            }
        }

        public DateTime CreateTimeAsDateTime {
            get {
                return DateTimeExtension.UnixTimestamp2LocalTime(CreateTime);
            }
        }

        [SerializeField]
        private long modified_time = default;
        public long ModifiedTime {
            get {
                return modified_time;
            }
            set {
                modified_time = value;
            }
        }

        public DateTime ModifiedTimeAsDateTime {
            get {
                return DateTimeExtension.UnixTimestamp2LocalTime(ModifiedTime);
            }
        }

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
        
        public Device Device
        {
            get
            {
                return DataManager.Instance.GetDevice(DeviceId);
            }
        }

        [SerializeField]
        private string scan_data_zip_path = default;
        public string ScanDataZipPath {
            get {
                return scan_data_zip_path;
            }
            set {
                scan_data_zip_path = value;
            }
        }

        [SerializeField]
        private string body_composition_path = default;
        public string BodyCompositionPath {
            get {
                return body_composition_path;
            }
            set {
                body_composition_path = value;
            }
        }

        [SerializeField]
        private string measurement_path = default;
        public string MeasurementPath {
            get {
                return measurement_path;
            }
            set {
                measurement_path = value;
            }
        }

        [SerializeField]
        private bool _isTalent = false;
        public bool isTalent {
            get {
                return _isTalent;
            }
        }
    }
}
