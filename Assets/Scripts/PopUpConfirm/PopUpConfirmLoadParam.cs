using Assets.Scripts.Common;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.PopUpConfirm
{
    [Serializable]
    public class PopUpConfirmLoadParam : BaseSceneLoadParam
    {
        [Serializable]
        public class Argment : BaseArgment
        {
            [SerializeField, Multiline]
            private string _message;
            public string message {
                get {
                    return _message;
                }
                set {
                    _message = value;
                }
            }

            [SerializeField]
            private bool _usePositiveButton = true;
            public bool usePositiveButton {
                get {
                    return _usePositiveButton;
                }
                set {
                    _usePositiveButton = value;
                }
            }

            [SerializeField]
            private bool _useNegativeButton = true;
            public bool useNegativeButton {
                get {
                    return _useNegativeButton;
                }
                set {
                    _useNegativeButton = value;
                }
            }

            [SerializeField]
            private Button.ButtonClickedEvent _positiveClickAction = default;
            public Button.ButtonClickedEvent positiveClickAction {
                get {
                    return _positiveClickAction;
                }
                set {
                    _positiveClickAction = value;
                }
            }

            [SerializeField]
            private Button.ButtonClickedEvent _negativeClickAction = default;
            public Button.ButtonClickedEvent negativeClickAction {
                get {
                    return _negativeClickAction;
                }
                set {
                    _negativeClickAction = value;
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

        public override LoadSceneMode mode {
            get {
                return LoadSceneMode.Additive;
            }
        }

        [SceneName]
        private string _nextSceneName = "PopUpConfirm";

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
