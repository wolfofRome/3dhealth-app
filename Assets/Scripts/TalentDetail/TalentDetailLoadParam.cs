using Assets.Scripts.Common;
using Assets.Scripts.Network.Response;
using Assets.Scripts.TalentList;
using System;
using UnityEngine;

namespace Assets.Scripts.TalentDetail
{
    [Serializable]
    public class TalentDetailLoadParam : BaseSceneLoadParam
    {
        [Serializable]
        public class Argment : BaseArgment
        {
            [SerializeField]
            private TalentoContents _talentInfo = default;
            public TalentoContents talentInfo {
                get {
                    return _talentInfo;
                }
                set {
                    _talentInfo = value;
                }
            }
        }

        [SerializeField]
        private Argment _argment = default;
        public Argment argment {
            get {
                return _argment;
            }
            set {
                _argment = value;
            }
        }

        [SceneName]
        private string _nextSceneName = "TalentDetail";

        public override BaseArgment GetArgment()
        {
            return _argment;
        }

        public override string GetNextSceneName()
        {
            return _nextSceneName;
        }
    }
}
