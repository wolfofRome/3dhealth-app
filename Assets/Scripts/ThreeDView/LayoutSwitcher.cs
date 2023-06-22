using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ThreeDView
{
    public class LayoutSwitcher : MonoBehaviour
    {
        [SerializeField]
        private Camera _mainUICamera = default;
        [SerializeField]
        private Camera _mainObjCamera = default;
        [SerializeField]
        private Camera _arrowCamera = default;
        [SerializeField]
        private List<GameObject> _enableOnWebGL = default;

        void Awake ()
        {
#if UNITY_WEBGL
            var leftScreenRect = new Rect(0, 0, 0.4f, 1);
            var rightScreenRect = new Rect(0.4f, 0, 1, 1);
            _mainUICamera.rect = rightScreenRect;
            _mainObjCamera.rect = rightScreenRect;
            _arrowCamera.rect = rightScreenRect;
            _subUICamera.rect = leftScreenRect;
            _subObjCamera.rect = leftScreenRect;
            foreach (var obj in _enableOnWebGL)
            {
                obj.SetActive(true);
            }
#else
            foreach (var obj in _enableOnWebGL)
            {
                obj.SetActive(false);
            }
#endif
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
