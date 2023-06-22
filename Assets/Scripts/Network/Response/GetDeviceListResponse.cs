using System;
using UnityEngine;

namespace Assets.Scripts.Network.Response
{
    [Serializable]
    public class GetDeviceListResponse : BaseResponse
    {
        [SerializeField]
        private Device[] device_list = default;
        public Device[] DeviceList
        {
            get
            {
                return device_list;
            }
            set
            {
                device_list = value;
            }
        }
    }
}
