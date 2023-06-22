using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ThreeDView.Posture
{
    public class RecommendExerciseItem : MonoBehaviour
    {
        [SerializeField]
        private Image _image = default;
        public Image image
        {
            get
            {
                return _image;
            }
        }
        
        [SerializeField]
        private Text _description = default;
        public Text description
        {
            get
            {
                return _description;
            }
        }
    }
}
