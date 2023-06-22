using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Common
{
    public class SceneLoader : BaseSingletonMonoBehaviour<SceneLoader>
    {
        private Dictionary<string, BaseArgment> _argments = new Dictionary<string, BaseArgment>();

        /// <summary>
        /// シーン起動パラメータの取得.
        /// ※※ Start以降で使用可 ※※
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public BaseArgment GetArgment(string sceneName)
        {
            return _argments.ContainsKey(sceneName) ? _argments[sceneName] : null;
        }

        /// <summary>
        /// シーン起動時の引数取得.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public T GetArgment<T>(string sceneName) where T : BaseArgment
        {
            object args = null;
            if (_argments.ContainsKey(sceneName) && _argments[sceneName] is T)
            {
                args = _argments[sceneName];
            }
            return (T)args;
        }

        /// <summary>
        /// シーンの読込.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="mode"></param>
        public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            Assert.IsFalse(string.IsNullOrEmpty(sceneName));
            LoadSceneWithParams(sceneName, mode);
        }

        /// <summary>
        /// シーンの読込.
        /// </summary>
        /// <param name="loadParam"></param>
        public void LoadSceneWithParams(BaseSceneLoadParam loadParam)
        {
            Assert.IsNotNull(loadParam);
            LoadSceneWithParams(loadParam.GetNextSceneName(), loadParam.mode, loadParam.GetArgment());
        }

        /// <summary>
        /// シーンの読込.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="mode"></param>
        /// <param name="argment"></param>
        public static void LoadSceneWithParams(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, BaseArgment argment = null)
        {
            Assert.IsFalse(string.IsNullOrEmpty(sceneName));

            if (argment != null && string.IsNullOrEmpty(argment.callerSceneName))
            {
                argment.callerSceneName = SceneManager.GetActiveScene().name;
            }

            var scene = SceneManager.GetSceneByName(sceneName);
            if (!string.IsNullOrEmpty(scene.name) && scene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(scene.name);
            }

            Instance._argments[sceneName] = argment;
            SceneManager.LoadScene(sceneName, mode);

        }

        /// <summary>
        /// シーンの非同期読込.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            Assert.IsFalse(string.IsNullOrEmpty(sceneName));
            return LoadSceneAsyncWithParams(sceneName, mode);
        }

        /// <summary>
        /// シーンの非同期読込.
        /// </summary>
        /// <param name="loadParam"></param>
        /// <returns></returns>
        public AsyncOperation LoadSceneAsyncWithParams(BaseSceneLoadParam loadParam)
        {
            Assert.IsNotNull(loadParam);
            return LoadSceneAsyncWithParams(loadParam.GetNextSceneName(), loadParam.mode, loadParam.GetArgment());
        }

        /// <summary>
        /// シーンの非同期読込.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="mode"></param>
        /// <param name="argment"></param>
        /// <returns></returns>
        public static AsyncOperation LoadSceneAsyncWithParams(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, BaseArgment argment = null)
        {
            Assert.IsFalse(string.IsNullOrEmpty(sceneName));

            if (argment != null && string.IsNullOrEmpty(argment.callerSceneName))
            {
                argment.callerSceneName = SceneManager.GetActiveScene().name;
            }

            var scene = SceneManager.GetSceneByName(sceneName);
            if (!string.IsNullOrEmpty(scene.name) && scene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(scene.name);
            }

            Instance._argments[sceneName] = argment;
            var async = SceneManager.LoadSceneAsync(sceneName, mode);
            async.allowSceneActivation = false;

            return async;
        }
        
        /// <summary>
        /// シーンの破棄.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public bool UnloadScene(string sceneName)
        {
            Assert.IsFalse(string.IsNullOrEmpty(sceneName));
            return SceneManager.UnloadScene(sceneName);
        }

        public void UnloadSceneAsync(string sceneName)
        {
            Assert.IsFalse(string.IsNullOrEmpty(sceneName));
            SceneManager.UnloadSceneAsync(sceneName);
        }

        void Awake()
        {
            if (this != Instance)
            {
                Destroy(this);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }
    }
}
