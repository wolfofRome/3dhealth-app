using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Network.Response
{
    [Serializable]
    public class GetTalentResponse : BaseResponse
    {
        [SerializeField]
        private List<TalentoContents> talent_resource_list = default;
        public List<TalentoContents> ContentsList
        {
            get
            {
                return talent_resource_list;
            }
            set
            {
                talent_resource_list = value;
            }
        }
    }
}
