using Assets.Scripts.Common.Config;
using Assets.Scripts.Common.Graphics.UI.ViewPager;
using Assets.Scripts.Graphics.UI;
using Assets.Scripts.PointPurchase;
using UnityEngine;

namespace Assets.Scripts.TalentList
{
    public class TalentViewPager : ViewPager
    {
        [SerializeField]
        private TitleBar _titleBar = default;
        private TitleBar titleBar {
            get {
                return _titleBar;
            }
        }

        [SerializeField]
        private PointPurchaseDataSource _pointDataSource = default;
        private PointPurchaseDataSource pointDataSource {
            get {
                return _pointDataSource;
            }
        }
        
        [SerializeField]
        private TalentDataSource _talentDataSource = default;
        private TalentDataSource talentDataSource
        {
            get
            {
                return _talentDataSource;
            }
        }

        protected override void Start()
        {
            base.Start();

            // ポイント情報の更新.
            pointDataSource.onUpdate.AddListener(ApplyPointBalance);
            StartCoroutine(pointDataSource.UpdateDataSource());

            // 有名人データの更新.
            talentDataSource.onUpdate.AddListener(ApplyTalentData);
            StartCoroutine(talentDataSource.UpdateDataSource());
        }

        private void ApplyPointBalance(int pointBalance)
        {
            titleBar.summary.text = "ポイント残高" + pointDataSource.pointBalance.ToString(AppConst.PointFormat);
        }
        
        private void ApplyTalentData(TalentDataSource data)
        {
            var frame = (TalentPageFrame)adapter.GetItem(0);
            frame.contentsList = data.talentoInfoList;
        }
    }
}
