using System;
using UnityEngine;

namespace Assets.Scripts.Network.Response
{
    [Serializable]
    public class PurchaseTalentResponse : BaseResponse
    {
        [SerializeField]
        private int point = default;
        public int PointBalance
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
