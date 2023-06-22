using System;
using UnityEngine;

namespace Assets.Scripts.Network.Response
{
    [Serializable]
    public class GetFileResponse<T> : BaseResponse
    {
        [SerializeField]
        private T data = default;
        public T Data {
            get {
                return data;
            }
            set {
                data = value;
            }
        }
    }
}
