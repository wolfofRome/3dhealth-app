using System;
using UnityEngine;

namespace Assets.Scripts.Network.Response
{
    [Serializable]
    public class RegisterDeviceResponse : BaseResponse
    {
        [SerializeField]
        private int token_id = default;
        public int TokenId {
            get {
                return token_id;
            }
        }
    }
}
