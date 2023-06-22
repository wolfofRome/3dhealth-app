using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Network.Response;
using Assets.Scripts.Common;
using Assets.Scripts.TalentList;

namespace Assets.Scripts.TalentDetail
{
    public class TalentMeasurementDataPanel : MeasurementDataPanel
    {
        private const string UnpurchasedText = "****";

        private PurchaseStatus _talentPurchaseStatus = PurchaseStatus.Unpurchased;

        protected override void Start()
        {
            base.Start();

            var args = SceneLoader.Instance.GetArgment<TalentDetailLoadParam.Argment>(gameObject.scene.name);
            _talentPurchaseStatus = args.talentInfo.purchaseStatus;
        }

        /// <summary>
        /// 採寸データの表示更新.
        /// </summary>
        protected override void Apply()
        {
            base.Apply();

            if (_talentPurchaseStatus != PurchaseStatus.Purchased)
            {
                SetUnpurchasedText(neck);
                SetUnpurchasedText(chest);
                SetUnpurchasedText(minWaist);
                SetUnpurchasedText(hip);
                SetUnpurchasedText(leftThigh);
                SetUnpurchasedText(rightThigh);
                SetUnpurchasedText(leftShoulder);
                SetUnpurchasedText(rightShoulder);
                SetUnpurchasedText(leftSleeve);
                SetUnpurchasedText(rightSleeve);
                SetUnpurchasedText(leftArm);
                SetUnpurchasedText(rightArm);
            }
        }

        /// <summary>
        /// 未購入時のテキスト更新処理.
        /// </summary>
        /// <param name="text"></param>
        private void SetUnpurchasedText(Text text)
        {
            text.text = UnpurchasedText;
        }

        /// <summary>
        /// 購入完了イベント受信処理.
        /// </summary>
        /// <param name="content"></param>
        public void OnPurchaseCompleted(TalentoContents content)
        {
            _talentPurchaseStatus = content.purchaseStatus;
            Apply();
        }
    }
}