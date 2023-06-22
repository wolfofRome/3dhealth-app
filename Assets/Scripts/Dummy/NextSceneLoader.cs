using System;
using System.Collections;
using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts.Dummy {
    public class NextSceneLoader : BaseLoginHandler {

        [SerializeField, SceneName]
        private string _firstScene = "Login";
        [SerializeField, SceneName]
        private string _afterAutoLoginScene = "ThreeDView";

        private SceneLoadParam _loadParam;

        public float
            waitSec = 2.5f;

        protected override void Awake() {
            base.Awake();
            StartCoroutine(StartNext());
        }

        IEnumerator StartNext() {
            yield return new WaitForSeconds(waitSec);
#if UNITY_WEBGL
            LoadNextScene(_afterAutoLoginScene);
#else
            var dm = DataManager.Instance;
            var user = dm.Cache.GetUser(UserData.AutoLoginUserId);
            if (user != null) {
                Login(user.LoginId, user.Password, res => {
                    if (res.ErrorCode == null) {
                        LoadNextScene(_afterAutoLoginScene);
                    } else {
                        LoadNextScene(_firstScene);
                    }
                });
            } else {
                LoadNextScene(_firstScene);
            }
#endif
        }

        private void LoadNextScene(string sceneName) {
            _loadParam = _loadParam ?? gameObject.AddComponent<SceneLoadParam>();
            _loadParam.nextSceneName = sceneName;
            LoadNextScene();
        }

        protected override BaseSceneLoadParam GetLoadParam() {
            return _loadParam;
        }
    }
}

