using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Common
{
    public class SceneEndButton : MonoBehaviour
    {
        public virtual void OnClick()
        {
            BackToPreviousScene();
        }

        public virtual void UnloadScene()
        {
            SceneLoader.Instance.UnloadScene(gameObject.scene.name);
        }

        public virtual void UnloadSceneAsync()
        {
            SceneLoader.Instance.UnloadSceneAsync(gameObject.scene.name);
        }

        protected void BackToPreviousScene()
        {
            var prevSceneName = GetPreviousSceneName();
            if (prevSceneName != null)
            {
                SceneLoader.LoadSceneWithParams(prevSceneName, LoadSceneMode.Single, SceneLoader.Instance.GetArgment(prevSceneName));
            }
        }

        protected AsyncOperation BackToPreviousSceneAsync()
        {
            AsyncOperation ret = null;
            var prevSceneName = GetPreviousSceneName();
            if (prevSceneName != null)
            {
                ret = SceneLoader.LoadSceneAsyncWithParams(prevSceneName, LoadSceneMode.Single, SceneLoader.Instance.GetArgment(prevSceneName));
            }
            return ret;
        }

        protected string GetPreviousSceneName()
        {
            // 起動パラメータから呼び出し元のシーン名を取得.
            var args = SceneLoader.Instance.GetArgment(gameObject.scene.name);
            Assert.IsFalse(string.IsNullOrEmpty(args.callerSceneName));
            return (args == null ? null : args.callerSceneName);
        }
    }
}