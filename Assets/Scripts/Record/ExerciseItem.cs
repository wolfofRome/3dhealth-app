using Assets.Scripts.Network.Response;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Record
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class ExerciseItem : MonoBehaviour
    {
        [SerializeField]
        private ExerciseId _id = default;
        public ExerciseId id {
            get {
                return _id;
            }
        }

        [SerializeField]
        private Toggle _toggle = default;
        public Toggle toggle {
            get {
                return _toggle;
            }
        }

        private Button _button;
        public Button button
        {
            get
            {
                return _button = _button ?? GetComponent<Button>();
            }
        }

        private void Start()
        {
            button.onClick.AddListener(() => {
                toggle.isOn = !toggle.isOn;
            });
        }
    }
}
