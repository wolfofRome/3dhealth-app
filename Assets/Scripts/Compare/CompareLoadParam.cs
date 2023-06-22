using Assets.Scripts.Common;
using Assets.Scripts.Network.Response;
using System;
using UnityEngine;

namespace Assets.Scripts.Compare
{
    [Serializable]
    public class CompareLoadParam : BaseSceneLoadParam
    {
        [Serializable]
        public class Argment : BaseArgment
        {
            [SerializeField]
            private Contents _leftContents = default;
            public Contents leftContents {
                get {
                    return _leftContents;
                }
                set {
                    _leftContents = value;
                }
            }

            [SerializeField]
            private Contents _rightContents = default;
            public Contents rightContents {
                get {
                    return _rightContents;
                }
                set {
                    _rightContents = value;
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
        private string _nextSceneName = "Compare";

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
