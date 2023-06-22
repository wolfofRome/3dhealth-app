using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Graphics.UI
{
    [RequireComponent(typeof(Button))]
    public class MultiSelectableController : MonoBehaviour
    {
        [SerializeField]
        private List<Selectable> _selectableList = new List<Selectable>();

        private Button _button;
        private Button button {
            get {
                return _button = _button ?? GetComponent<Button>();
            }
        }

        private bool _interactable;
        private bool interactable {
            get {
                return _interactable;
            }
            set {
                foreach (var selectable in _selectableList)
                {
                    selectable.interactable = value;
                }
                _interactable = value;
            }
        }
        
        void OnValidate()
        {
            interactable = button.interactable;
        }

        void Start()
        {
            button.onClick.AddListener(OnClick);
            interactable = button.interactable;
        }
        
        void Update()
        {
            if (interactable != button.interactable)
            {
                interactable = button.interactable;
            }
        }

        public void OnClick()
        {
            foreach (var selectable in _selectableList)
            {
                selectable.Select();
            }
        }
    }
}