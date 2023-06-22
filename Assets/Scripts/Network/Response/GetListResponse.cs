using System;
using UnityEngine;

namespace Assets.Scripts.Network.Response
{
    [Serializable]
    public class GetListResponse : BaseResponse
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
        private Contents[] contents = default;
        public Contents[] ContentsList {
            get {
                return contents;
            }
            set {
                contents = value;
            }
        }
    }
}
