using System;
using UnityEngine;

namespace Assets.Scripts.Network.Response
{
    [Serializable]
    public class LoginResponse : BaseResponse
    {
        [SerializeField]
        private string user_body_composition_csv = default;
        public string UserBodyCompositionCsv {
            get {
                return user_body_composition_csv;
            }
            set {
                user_body_composition_csv = value;
            }
        }

        [SerializeField]
        private string user_measurement_csv = default;
        public string UserMeasurementCsv {
            get {
                return user_measurement_csv;
            }
            set {
                user_measurement_csv = value;
            }
        }

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

        [SerializeField]
        private Contents[] contents = default;
        public Contents[] ContentsList {
            get {
                return contents;
            }
            set {
                contents = value;
            }
        }

        [SerializeField]
        private Maintenance maintenance;
        public Maintenance Maintenance{
            get{
                return maintenance;
            }
            set{
                maintenance = value;
            }
        }

        [SerializeField]
        private string session_id = default;
        public string SessionId {
            get {
                return session_id;
            }
            set {
                session_id = value;
            }
        }
    }
}
