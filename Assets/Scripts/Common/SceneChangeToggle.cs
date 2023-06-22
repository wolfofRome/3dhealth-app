using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Common
{
    [RequireComponent(typeof(BaseSceneLoadParam))]
    [RequireComponent(typeof(Toggle))]
    public class SceneChangeToggle : MonoBehaviour
    {
        private bool _prevValue;
        private Toggle _toggle;
        private Toggle ToggleButton {
            get {
                return _toggle = _toggle ?? GetComponent<Toggle>();
            }
        }

        void Start()
        {
            _prevValue = ToggleButton.isOn;
        }

        public void OnValueChanged(bool value)
        {
            if (value && value != _prevValue)
            {
                var param = GetComponent<BaseSceneLoadParam>();
                if (!SceneManager.GetSceneByName(param.GetNextSceneName()).isLoaded)
                {
                    SceneLoader.Instance.LoadSceneWithParams(param);
                }
            }
            _prevValue = value;
        }
    }
}