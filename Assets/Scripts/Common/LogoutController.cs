using Assets.Scripts.Network;
using Assets.Scripts.PopUpConfirm;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Common
{
    [RequireComponent(typeof(SceneLoadParam))]
    [RequireComponent(typeof(PopUpConfirmLoadParam))]
    public class LogoutController : MonoBehaviour
    {
        private PopUpConfirmLoadParam _loadParam;
        protected PopUpConfirmLoadParam loadParam {
            get {
                return _loadParam = _loadParam ?? GetComponent<PopUpConfirmLoadParam>();
            }
        }

        protected virtual void Awake()
        {
            loadParam.argment.positiveClickAction.AddListener(() =>
            {
                Logout();
            });
        }

        public void ShowConfirmDialog()
        {
            SceneLoader.Instance.LoadSceneWithParams(loadParam);
        }

        public void Logout()
        {
            StartCoroutine(LogoutProcess(() =>
            {
                SceneLoader.Instance.LoadSceneWithParams(GetComponent<SceneLoadParam>());
            }));
        }

        protected IEnumerator LogoutProcess(Action callback)
        {
            int savedTokenId = UserData.RegistrationTokenId;
            if (savedTokenId > 0)
            {
                yield return ApiHelper.Instance.DeleteDevice(savedTokenId, res =>
                {
                    if (res.ErrorCode != null)
                    {
                        DebugUtil.LogError("DeleteDevice -> " + res.ErrorCode + " : " + res.ErrorMessage);
                    }
                });
            }

            yield return ApiHelper.Instance.Logout(res =>
            {
                if (res.ErrorCode != null)
                {
                    DebugUtil.LogError("Logout -> " + res.ErrorCode + " : " + res.ErrorMessage);
                }

                // オートログイン用のユーザー情報をクリア.
                UserData.AutoLoginUserId = -1;
                UserData.Save();
                DataManager.Instance.Clear();
                callback.Invoke();
            });
        }
    }
}
