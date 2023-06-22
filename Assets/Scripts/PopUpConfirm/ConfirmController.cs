using Assets.Scripts.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.PopUpConfirm
{
    public class ConfirmController : MonoBehaviour
    {
        [SerializeField]
        private PopUpController _popUpController = default;

        [SerializeField]
        private Text _confirmMessage = default;

        [SerializeField]
        private Button _positiveButton = default;

        [SerializeField]
        private Button _negativeButton = default;

        void Awake()
        {
            _popUpController.OnCloseAnimationFinish.AddListener(() =>
            {
                SceneManager.UnloadSceneAsync(gameObject.scene.name);
            });
        }

        void Start()
        {
            var args = SceneLoader.Instance.GetArgment<PopUpConfirmLoadParam.Argment>(gameObject.scene.name);
            if (args != null)
            {
                _confirmMessage.text = args.message;
                _positiveButton.gameObject.SetActive(args.usePositiveButton);
                _negativeButton.gameObject.SetActive(args.useNegativeButton);
                if (args.positiveClickAction != null)
                {
                    _positiveButton.onClick = args.positiveClickAction;
                }
                if (args.negativeClickAction != null)
                {
                    _negativeButton.onClick = args.negativeClickAction;
                }
            }
            _positiveButton.onClick.AddListener(_popUpController.StartCloseAnimation);
            _negativeButton.onClick.AddListener(_popUpController.StartCloseAnimation);
        }
    }
}
