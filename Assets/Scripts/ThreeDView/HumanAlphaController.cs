using UnityEngine;

namespace Assets.Scripts.ThreeDView
{
    public class HumanAlphaController : MonoBehaviour
    {
        [SerializeField]
        private HumanLayer Layer = HumanLayer.Fat;
        [SerializeField]
        private BodyTypeSelector _selector = default;
        [SerializeField]
        private float _alpha = 1;

        // Use this for initialization
        void Start()
        {
            _selector.SetAlphaOfLayer(Layer, _alpha);
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void OnAlphaChanged(float alpha)
        {
            _alpha = alpha;
            _selector.SetAlphaOfLayer(Layer, _alpha);
        }
    }
}