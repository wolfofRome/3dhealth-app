using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Common
{
    [Serializable]
    public class SceneLoadParam : BaseSceneLoadParam
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

        [SerializeField, SceneName]
        private string _nextSceneName;
        public string nextSceneName {
            get {
                return _nextSceneName;
            }
            set {
                _nextSceneName = value;
            }
        }

        [SerializeField]
        private LoadSceneMode _mode = LoadSceneMode.Single;
        public override LoadSceneMode mode {
            get {
                return _mode;
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
