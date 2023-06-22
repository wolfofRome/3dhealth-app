using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common
{
    [RequireComponent(typeof(ToggleGroup))]
    public class AlphaChangeToggleGroup : BaseAlphaSwitcher
    {
        [SerializeField]
        private Toggle _alphaZero = default;
        [SerializeField]
        private Toggle _alphaZeroPointFive = default;
        [SerializeField]
        private Toggle _alphaOne = default;


        public void OnValueChanged(bool value)
        {
            if (value)
            {
                Toggle toggle = GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault();
                if (toggle == _alphaZero) _state = MeshState.AlphaZero;
                if (toggle == _alphaZeroPointFive) _state = MeshState.AlphaZeroPointFive;
                if (toggle == _alphaOne) _state = MeshState.AlphaOne;

                OnAlphaChanged.Invoke(_alphaTable[(int)_state]);
            }
        }

        protected override void ChangeButtonState(MeshState state)
        {
            switch (state)
            {
                case MeshState.AlphaZero:
                    _alphaZero.isOn = true;
                    break;
                case MeshState.AlphaZeroPointFive:
                    _alphaZeroPointFive.isOn = true;
                    break;
                case MeshState.AlphaOne:
                    _alphaOne.isOn = true;
                    break;
            }
        }
    }
}