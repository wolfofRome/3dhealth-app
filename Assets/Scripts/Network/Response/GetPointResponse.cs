using System;
using UnityEngine;

namespace Assets.Scripts.Network.Response
{
    [Serializable]
    public class GetPointResponse : BaseResponse
    {
        [SerializeField]
        private int point = default;
        public int Point
        {
            get {
                return point;
            }
            set {
                point = value;
            }
        }
    }
}
