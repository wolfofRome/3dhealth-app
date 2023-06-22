using Assets.Scripts.Common.Data;
using Assets.Scripts.Network;
using System;
using UnityEngine;

namespace Assets.Scripts.Common
{
    [RequireComponent(typeof(BaseSceneLoadParam))]
    public class TrialButton : BaseLoginHandler
    {
        [SerializeField]
        private TrialAccountConfig config = default;
        
        public void OnClick()
        {
            Action loginAction = () =>
            {
                Login(config.LoginId, config.Password, loginRes =>
                {
                    LoadNextScene();
                });
            };

            // ログイン済の場合はログアウト後、体験版アカウントにてログイン.
            if (ApiHelper.Instance.isLogin)
            {
                StartCoroutine(ApiHelper.Instance.Logout(res =>
                {
                    if (res.ErrorCode != null)
                    {
                        DebugUtil.LogError("Logout -> " + res.ErrorCode + " : " + res.ErrorMessage);
                    }

                    // オートログイン用のユーザー情報をクリア.
                    UserData.AutoLoginUserId = -1;
                    UserData.Save();
                    DataManager.Instance.Clear();
                    loginAction();
                }));
            }
            else
            {
                loginAction();
            }
        }

        protected override BaseSceneLoadParam GetLoadParam()
        {
            return GetComponent<BaseSceneLoadParam>();
        }
    }
}