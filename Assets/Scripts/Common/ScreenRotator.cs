using UnityEngine;

namespace Assets.Scripts.Common
{
    public class ScreenRotator : MonoBehaviour
    {
        public ScreenOrientation Orientation;

        public enum OrientationBehaviour
        {
            Revert,
            Hold
        }
        public OrientationBehaviour SceneEndBehaviour = OrientationBehaviour.Revert;

        private ScreenOrientation _prevOrientation;

        void Awake()
        {
            _prevOrientation = Screen.orientation;
            Screen.orientation = Orientation;
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnDestroy()
        {
            if (SceneEndBehaviour == OrientationBehaviour.Revert)
            {
                // 画面の向きを元に戻す
                Screen.orientation = _prevOrientation;
            }
        }
    }
}
