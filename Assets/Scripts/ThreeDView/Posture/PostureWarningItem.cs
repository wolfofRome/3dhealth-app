using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Graphics.UI;

namespace Assets.Scripts.ThreeDView.Posture
{
    public class PostureWarningItem : MonoBehaviour
    {
        [SerializeField]
        private MultifunctionalButton _warningButton = default;
        public MultifunctionalButton warningButton
        {
            get
            {
                return _warningButton;
            }
        }
    }
}
