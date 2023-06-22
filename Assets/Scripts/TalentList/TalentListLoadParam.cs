using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace Assets.Scripts.TalentList
{
    [Serializable]
    public class TalentListLoadParam : BaseSceneLoadParam
    {
        [Serializable]
        public class Argment : BaseArgment
        {
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
        private string _nextSceneName = "TalentList";

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
