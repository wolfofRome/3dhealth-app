using System;
using UnityEngine;

namespace Assets.Scripts.Network.Response
{
    [Serializable]
    public class TalentCategory
    {
        [SerializeField]
        private int category_id = default;
        public int CategoryId
        {
            get
            {
                return category_id;
            }
        }
    }
}
