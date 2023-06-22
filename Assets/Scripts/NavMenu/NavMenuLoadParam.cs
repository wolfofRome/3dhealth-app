using Assets.Scripts.Common;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.NavMenu
{
    [Serializable]
    public class NavMenuLoadParam : BaseSceneLoadParam
    {
        [Serializable]
        public class Argment : BaseArgment
        {
            [SerializeField]
            private UnityEvent _onMenuOpened = default;
            public UnityEvent onMenuOpened {
                get {
                    return _onMenuOpened;
                }
                set {
                    _onMenuOpened = value;
                }
            }

            [SerializeField]
            private UnityEvent _onMenuClosed = default;
            public UnityEvent onMenuClosed {
                get {
                    return _onMenuClosed;
                }
                set {
                    _onMenuClosed = value;
                }
            }

            [SerializeField]
            private UnityEvent _onMenuItemClicked = default;
            public UnityEvent onMenuItemClicked {
                get {
                    return _onMenuItemClicked;
                }
                set {
                    _onMenuItemClicked = value;
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
        private string _nextSceneName = "NavMenu";

        public override LoadSceneMode mode {
            get {
                return LoadSceneMode.Additive;
            }
        }

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
