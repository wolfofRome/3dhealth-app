using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Common.Graphics.UI.ViewPager;

namespace Assets.Scripts.ThreeDView.Posture
{
    [RequireComponent(typeof(ViewPager))]
    public class PostureDetailController : MonoBehaviour
    {
        private ViewPager _detailViewPager;
        protected ViewPager detailViewPager {
            get {
                return _detailViewPager = _detailViewPager ?? GetComponent<ViewPager>();
            }
        }

        [SerializeField]
        private Text _description = default;
        
        [SerializeField]
        private AdviceController _adviceController = default;

        void Start()
        {
            detailViewPager.onPageChanged.AddListener(OnPageChanged);
        }

        void Update()
        {
        }

        /// <summary>
        /// 姿勢検証のページ切替イベント.
        /// </summary>
        /// <param name="pagePosition"></param>
        void OnPageChanged(int pagePosition)
        {
            var frame = (PostureDetailPageFrame)detailViewPager.adapter.GetItem(pagePosition);
            if (frame != null)
            {
                _description.text = frame.Result.description;
                _adviceController.postureCondition = frame.Result.condition;
            }
        }
    }
}
