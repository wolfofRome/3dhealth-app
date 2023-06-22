using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.Database.DataRow;
using System;
using System.Collections.Generic;
using System.Collections;

namespace Assets.Scripts.Common
{
    public class FirebaseAnalyticsManager : MonoBehaviour
    {
        private void Start()
        {
            SceneManager.sceneLoaded += SendScreenName;
        }

        private bool IsEventScreen(Scene nextScene)
        {
            return !(nextScene.name == "Dummy" || nextScene.name == "Loading");
        }

        private void SendScreenName(Scene scene, LoadSceneMode mode)
        {
            if (!IsEventScreen(scene)) { return; }

            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {

                var dependencyStatus = task.Result;

                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    Firebase.Analytics.FirebaseAnalytics.LogEvent(scene.name);
                    Firebase.Analytics.FirebaseAnalytics.SetCurrentScreen(scene.name, null);
                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                }
            });
        }
    }
}