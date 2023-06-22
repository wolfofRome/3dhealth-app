using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.Scripts.Common
{
    public class FontSizeSwitcher : MonoBehaviour
    {
        [SerializeField]
        private List<FontSizePresetParam> _presetList = default;
        
        void Awake ()
        {
            UpdateFontSize();
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void UpdateFontSize()
        {
            foreach (var preset in _presetList)
            {
                foreach (var container in preset.TextContainerList)
                {
                    var textArray = container.GetComponentsInChildren<Text>();
                    foreach (var text in textArray)
                    {
#if UNITY_WEBGL
                        text.fontSize = preset.WebGLFontSize;
#else
                        text.fontSize = preset.MobileFontSize;
#endif
                    }
                }
            }
        }
    }
}
