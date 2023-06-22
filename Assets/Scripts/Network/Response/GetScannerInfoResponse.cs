using System;
using UnityEngine;

namespace Assets.Scripts.Network.Response
{
    [Serializable]
    public class GetScannerInfoResponse : BaseResponse
    {
        [SerializeField]
        private Resource resource = default;
        public Resource Resource
        {
            get => resource;
            set => resource = value;
        }
    }
}