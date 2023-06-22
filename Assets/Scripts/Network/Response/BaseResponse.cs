using System;
using UnityEngine;

namespace Assets.Scripts.Network.Response
{
    [Serializable]
    public abstract class BaseResponse
    {
        [SerializeField]
        private Error error = default;
        public Error Error {
            get {
                return error;
            }
            set {
                error = value;
            }
        }

        public string ErrorCode {
            get {
                return error == null ? null : error.Code;
            }
        }

        public string ErrorMessage {
            get {
                return error == null ? null : error.Message;
            }
        }
    }
}
