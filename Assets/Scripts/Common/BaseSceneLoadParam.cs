using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Common
{
    public abstract class BaseSceneLoadParam : MonoBehaviour
    {
        public abstract BaseArgment GetArgment();
        public abstract string GetNextSceneName();

        public virtual LoadSceneMode mode {
            get {
                return LoadSceneMode.Single;
            }
        }
    }

    public abstract class BaseArgment
    {
        [SceneName]
        private string _callerSceneName;
        public string callerSceneName {
            get {
                return _callerSceneName;
            }
            set {
                _callerSceneName = value;
            }
        }
    }
}
