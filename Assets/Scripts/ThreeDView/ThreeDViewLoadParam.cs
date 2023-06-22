using Assets.Scripts.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ThreeDView
{
    [Serializable]
    public class ThreeDViewLoadParam : BaseSceneLoadParam
    {
        [Serializable]
        public class Argment : BaseArgment
        {
            [SerializeField]
            private int _resourceId = -1;
            public int resourceId {
                get {
                    return _resourceId;
                }
                set {
                    _resourceId = value;
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
        private string _nextSceneName = "ThreeDView";

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
