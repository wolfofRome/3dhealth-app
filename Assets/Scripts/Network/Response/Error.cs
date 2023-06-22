using System;
using UnityEngine;

namespace Assets.Scripts.Network.Response
{
    [Serializable]
    public class Error
    {
        [SerializeField]
        private string code = default;
        public string Code {
            get {
                return code;
            }
            set {
                code = value;
            }
        }
        [SerializeField]
        private string message = default;
        public string Message {
            get {
                return message;
            }
            set {
                message = value;
            }
        }
    }
}
