using Assets.Scripts.Common.Config;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PointPurchase
{
    public class PurchaseItem : MonoBehaviour
    {
        [SerializeField]
        private ProductID _productID = default;
        public ProductID productID
        {
            get
            {
                return _productID;
            }
            set
            {
                _productID = value;
                priceText.text = value.ToPrice().ToString(AppConst.PriceFormat);
                pointText.text = value.ToPoint().ToString(AppConst.PointFormat);
                summaryText.text = value.ToSummay();
                summaryText.gameObject.SetActive(!string.IsNullOrEmpty(summaryText.text));
            }
        }

        [SerializeField]
        private Text _pointText = default;
        private Text pointText
        {
            get
            {
                return _pointText;
            }
        }

        [SerializeField]
        private Text _priceText = default;
        private Text priceText
        {
            get
            {
                return _priceText;
            }
        }

        [SerializeField]
        private Text _summaryText = default;
        private Text summaryText
        {
            get
            {
                return _summaryText;
            }
        }
        
        private void OnValidate()
        {
            productID = productID;
        }
    }
}
