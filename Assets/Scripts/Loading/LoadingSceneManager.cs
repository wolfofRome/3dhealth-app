using Assets.Scripts.Common;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Loading
{
    public class LoadingSceneManager
    {
        private static LoadingSceneManager _instance;

        private const string SceneName = "Loading";
        private Action _callback;

        private List<ProgressAdapter> _progressAdapterList = new List<ProgressAdapter>();
        public List<ProgressAdapter> ProgressAdapterList {
            get {
                return _progressAdapterList;
            }
        }

        private bool isLoaded = false;

        public static LoadingSceneManager Instance {
            get {
                return _instance = _instance ?? new LoadingSceneManager();
            }
        }

        private LoadingSceneManager()
        {
        }
        
        public void Show()
        {
            if (!isLoaded)
            {
                SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);
                isLoaded = true;
            }
        }

        public void Hide()
        {
            if (SceneManager.GetSceneByName(SceneName).isLoaded)
            {
                _progressAdapterList.Clear();
                SceneManager.UnloadSceneAsync(SceneName);

                if (_callback != null)
                {
                    _callback();
                }
                isLoaded = false;
            }
        }

        public void AddProgressAdapter(ProgressAdapter adapter)
        {
            _progressAdapterList.Add(adapter);
        }

        public void SetHideListener(Action callback)
        {
            _callback = callback;
        }
    }
}
