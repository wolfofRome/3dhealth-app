using UnityEngine;

namespace Assets.Scripts.Common.Data
{
    public class TrialAccountConfig : ScriptableObject
    {
        [SerializeField]
        private string _loginId = default;
        public string LoginId {
            get {
                return _loginId;
            }
            set {
                _loginId = value;
            }
        }

        [SerializeField]
        private string _password = default;
        public string Password {
            get {
                return _password;
            }
            set {
                _password = value;
            }
        }
    }
}
