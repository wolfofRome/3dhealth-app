using Assets.Scripts.Database.DataRow;
using Assets.Scripts.Network;
using Assets.Scripts.Network.Response;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public abstract class BaseLoginHandler : MonoBehaviour
    {
        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
        }

        public void Login(string loginId, string password, Action<LoginResponse> callback)
        {
            DataManager dm = DataManager.Instance;
            dm.ContentsList = new List<Contents>();

            StartCoroutine(ApiHelper.Instance.Login(loginId, password, ApiHelper.GetFlgUserData | ApiHelper.GetFlgResource, res =>
            {
                SetLoginData(loginId, password, res);
                callback(res);
            }));
        }

        private void SetLoginData(string loginId, string password, LoginResponse response)
        {
            if (response.ErrorCode != null)
            {
                // ログインエラー.
                DebugUtil.Log("Login Error -> " + response.ErrorCode + " : " + response.ErrorMessage);
            }
            else
            {
                DataManager dm = DataManager.Instance;

                // ユーザー情報を設定.
                dm.Cache.InsertOrReplace(new User
                {
                    Id = response.Profile.UserId,
                    LoginId = loginId,
                    Password = password,
                    Height = response.Profile.Height
                });

                dm.Profile = response.Profile;

                dm.UserBodyCompositionPath = response.UserBodyCompositionCsv;
                dm.UserMeasurementPath = response.UserMeasurementCsv;
                dm.ContentsList.Clear();
                dm.ContentsList.AddRange(response.ContentsList);

#if !UNITY_WEBGL
                if (!dm.isTrialAccount)
                {
                    NotificationRegistrar.Instance.RegisterDeviceToken();
                }
#endif
            }
        }

        protected virtual void LoadNextScene()
        {
            var param = GetLoadParam();
            if (param != null)
            {
                SceneLoader.Instance.LoadSceneWithParams(param);
            }
        }

        protected abstract BaseSceneLoadParam GetLoadParam();
    }
}