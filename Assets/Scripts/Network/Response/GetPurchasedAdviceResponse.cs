using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Network.Response
{
    [Serializable]
    public class GetPurchasedAdviceResponse : BaseResponse
    {
        [SerializeField]
        private List<string> position_purchases = default;
        public List<string> PurchasedPoints
        {
            get
            {
                return position_purchases;
            }
            set
            {
                position_purchases = value;
            }
        }
    }
}
