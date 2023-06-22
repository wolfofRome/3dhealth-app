using Assets.Scripts.Common;
using Assets.Scripts.Network;
using Assets.Scripts.Network.Response;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.PopUpHeight
{
    [RequireComponent(typeof(PopUpController))]
    public class PopUpHeightController : MonoBehaviour
    {
        private PopUpController _popUpController;

        [SerializeField]
        private InputField _heightInputField = default;

        [SerializeField]
        private Text _errorText = default;

        private Action _successAction;

        private Action _failedAction;

        private const float MinHeight = 50.0f;
        private const float MaxHeight = 300.0f;

        private void Awake()
        {
            _popUpController = GetComponent<PopUpController>();
            _popUpController.OnCloseAnimationFinish.AddListener(() =>
            {
                SceneManager.UnloadSceneAsync(gameObject.scene.name);
            });

            _errorText.text = "";
        }

        private void Start()
        {
            var args = SceneLoader.Instance.GetArgment<PopUpHeightLoadParam.Argment>(gameObject.scene.name);
            if (args != null)
            {
                _successAction = args.successAction;
                _failedAction = args.failedAction;
            }
        }
        
        public void OnValueChangedHeightInputField(string value)
        {
            int index = value.IndexOf(".");
            if (index >= 0 && value.Length - (index + 1) > 1)
            {
                // 小数点第2位以降を削除
                _heightInputField.text = value.Substring(0, value.Length - 1);
            }
        }

        public void OnClickOkButton()
        {
            string heightString = _heightInputField.text;
            if (heightString.Length == 0)
            {
                _errorText.text = "身長を入力してください。";
                return;
            }

            float heightFloat;
            if (!float.TryParse(heightString, out heightFloat)) {
                _errorText.text = "身長の入力が正しくありません。";
                return;
            }

            if (heightFloat < MinHeight || heightFloat > MaxHeight)
            {
                _errorText.text = MinHeight.ToString() + "cm~" + MaxHeight.ToString() + "cmの間で入力してください。";
                return;
            }
            
            var height = (int)(heightFloat * 10);

            StartCoroutine(ApiHelper.Instance.AddUserHeight(height, res =>
            {
                if (res.ErrorCode == null)
                {
                    _popUpController.StartCloseAnimation();

                    if (_successAction != null)
                    {
                        _successAction();
                    }
                }
                else
                {
                    _errorText.text = res.ErrorMessage;

                    if (_failedAction != null)
                    {
                        _failedAction();
                    }
                }
            }));
        }
    }
}