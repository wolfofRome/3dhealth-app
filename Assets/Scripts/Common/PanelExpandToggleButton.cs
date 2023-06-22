using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common
{
    [RequireComponent(typeof(Toggle))]
    public class PanelExpandToggleButton : MonoBehaviour
    {
        [SerializeField]
        private LayoutExpander _expader = default;
        [SerializeField]
        private List<GameObject> _inactivationObjects = default;
        [SerializeField]
        private Image _backgroundImage = default;

        void Awake()
        {
        }

        // Use this for initialization
        void Start()
        {
            GetComponent<Toggle>().onValueChanged.AddListener(OnChangeValue);
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void OnChangeValue(bool isExpand)
        {
            if (_backgroundImage != null)
            {
                _backgroundImage.transform.SetAsLastSibling();
                _backgroundImage.gameObject.SetActive(isExpand);
            }
            _expader.OnChangeValue(isExpand);
            foreach(var go in _inactivationObjects)
            {
                go.SetActive(!isExpand);
            }
        }
    }
}