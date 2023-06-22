using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts.ThreeDView
{
    public class AlphaControllMenu : MonoBehaviour
    {
        [SerializeField]
        private BaseAlphaSwitcher _physiqueAlphaButton = default;
        public BaseAlphaSwitcher PhysiqueAlphaButton {
            get {
                return _physiqueAlphaButton;
            }
            set {
                _physiqueAlphaButton = value;
            }
        }

        [SerializeField]
        private BaseAlphaSwitcher _skeletonAlphaButton = default;
        public BaseAlphaSwitcher SkeletonAlphaButton {
            get {
                return _skeletonAlphaButton;
            }
            set {
                _skeletonAlphaButton = value;
            }
        }

        [SerializeField]
        private BaseAlphaSwitcher _muscleAlphaButton = default;
        public BaseAlphaSwitcher MuscleAlphaButton {
            get {
                return _muscleAlphaButton;
            }
            set {
                _muscleAlphaButton = value;
            }
        }

        [SerializeField]
        private BaseAlphaSwitcher _fatAlphaButton = default;
        public BaseAlphaSwitcher FatAlphaButton {
            get {
                return _fatAlphaButton;
            }
            set {
                _fatAlphaButton = value;
            }
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}