using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Graphics.UI
{
    public class ImageTextHolder : MonoBehaviour
    {
        [SerializeField]
        private Image _image = default;
        public Image image {
            get {
                return _image;
            }
        }

        [SerializeField]
        private Text _text = default;
        public Text text {
            get {
                return _text;
            }
        }

        public virtual void Start()
        {
        }
        
        public virtual void Update()
        {
        }
    }
}