using Assets.Scripts.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Login
{
    [RequireComponent(typeof(SceneLoadParam))]
    public class LoginButton : BaseLoginHandler
    {
        [SerializeField]
        private InputField _id = default;
        [SerializeField]
        private InputField _password = default;
        [SerializeField]
        private Toggle _autoLogin = default;
        [SerializeField]
        private Text _alertText = default;

        private SceneLoadParam _loadParam;
        public static string bodyId;

        public void OnClick()
        {
            _alertText.enabled = false;

            Login(_id.text, _password.text, res =>
            {
                if (res.ErrorCode != null)
                {
                    _alertText.enabled = true;
                }
                else
                {
                    UserData.AutoLoginUserId = _autoLogin.isOn ? res.Profile.UserId : -1;
                    UserData.Save();
                    LoadNextScene();
                }
            });
        }

        protected override BaseSceneLoadParam GetLoadParam()
        {
            return _loadParam = _loadParam ?? gameObject.GetComponent<SceneLoadParam>();
        }
    }
}