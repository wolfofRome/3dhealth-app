using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common
{
    [RequireComponent(typeof(Image))]
    public class AlphaChangeButton : BaseAlphaSwitcher
    {
        [SerializeField]
        private Sprite _alphaZeroSprite = default;
        [SerializeField]
        private Sprite _alphaZeroPointFiveSprite = default;
        [SerializeField]
        private Sprite _alphaOneSprite = default;
        private Image _targetImage;
        private Image TargetImage {
            get {
                return _targetImage = _targetImage ?? GetComponent<Image>();
            }
        }

        public void OnClick()
        {
            switch (_state)
            {
                case MeshState.AlphaZero:
                    _state = MeshState.AlphaOne;
                    break;
                case MeshState.AlphaZeroPointFive:
                    _state = MeshState.AlphaZero;
                    break;
                case MeshState.AlphaOne:
                    _state = MeshState.AlphaZeroPointFive;
                    break;
            }
            ChangeButtonState(_state);
            OnAlphaChanged.Invoke(_alphaTable[(int)_state]);
        }

        protected override void ChangeButtonState(MeshState state)
        {
            if (TargetImage == null)
            {
                return;
            }
            switch (state)
            {
                case MeshState.AlphaZero:
                    TargetImage.sprite = _alphaZeroSprite;
                    break;
                case MeshState.AlphaZeroPointFive:
                    TargetImage.sprite = _alphaZeroPointFiveSprite;
                    break;
                case MeshState.AlphaOne:
                    TargetImage.sprite = _alphaOneSprite;
                    break;
            }
        }
    }
}