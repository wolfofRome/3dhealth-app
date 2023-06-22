using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts.Common;

namespace Assets.Scripts.Network.Response
{
    [Serializable]
    public class Resource
    {
        [SerializeField]
        private int resource_id = default;
        public int ResourceId 
        {
            get => resource_id;
            set => resource_id = value;
        }

        [SerializeField]
        private long create_time = default;
        public long CreateTime
        {
            get => create_time;
            set => create_time = value;
        }

        [SerializeField]
        private long modified_time = default;
        public long ModifiedTime
        {
            get => modified_time;
            set => modified_time = value;
        }

        [SerializeField]
        private string store_id = default;
        public string StoreId
        {
            get => store_id;
            set => store_id = value;
        }

        [SerializeField]
        private string store_name = default;
        public string StoreName
        {
            get => store_name;
            set => store_name = value;
        }

        [SerializeField]
        private string device_id = default;
        public string DeviceId
        {
            get => device_id;
            set => device_id = value;
        }

        [SerializeField]
        private string device_name = default;
        public string DeviceName
        {
            get => device_name;
            set => device_name = value;
        }
    }
}
