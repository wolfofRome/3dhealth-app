using System;
using UnityEngine;

namespace Assets.Scripts.Network.Response
{
    [Serializable]
    public class GetLoginDataResponse : BaseResponse
    {
        [SerializeField]
        private Profile profile = default;
        public Profile Profile {
            get {
                return profile;
            }
            set {
                profile = value;
            }
        }
    }
}
