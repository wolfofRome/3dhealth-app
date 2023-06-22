using Assets.Scripts.Common;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.PopUpHeight
{
    [Serializable]
    public class PopUpHeightLoadParam : BaseSceneLoadParam
    {
        [Serializable]
        public class Argment : BaseArgment
        {
            private Action _successAction;
            public Action successAction {
                get {
                    return _successAction;
                }
                set {
                    _successAction = value;
                }
            }

            private Action _failedAction;
            public Action failedAction {
                get {
                    return _failedAction;
                }
                set {
                    _failedAction = value;
                }
            }
        }

        [SerializeField]
        private Argment _argment = new Argment();
        public Argment argment {
            get {
                return _argment;
            }
            set {
                _argment = value;
            }
        }

        public override LoadSceneMode mode {
            get {
                return LoadSceneMode.Additive;
            }
        }

        [SceneName]
        private string _nextSceneName = "PopUpHeight";

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
