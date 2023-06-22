using Assets.Scripts.Common;
using Assets.Scripts.Common.Config;
using Assets.Scripts.Graphics.UI;
using Assets.Scripts.Network;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX)
using UnityEngine.Purchasing.Security;
#endif

namespace Assets.Scripts.PointPurchase
{
    public enum ProductID : int
    {
        Points1000,
        Points5000,
        Points10000,
        Points10000Dummy
    }

    public static class ProductIDExtension
    {
        private static readonly Dictionary<ProductID, ProductInfo> _productMap = new Dictionary<ProductID, ProductInfo>() {
            { ProductID.Points1000, new ProductInfo("points1000y", 1100, 1000, "") },
            { ProductID.Points5000, new ProductInfo("points5000y", 5500, 5000, "") },
            { ProductID.Points10000, new ProductInfo("points10000y", 11000, 10000, "") },
            { ProductID.Points10000Dummy, new ProductInfo("points10000y",11000, 10000, "ダミー購入用") }
        };

        private struct ProductInfo
        {
            public string id { get; set; }
            public int price { get; set; }
            public int point { get; set; }
            public string summay { get; set; }

            public ProductInfo(string id, int price, int point, string summay)
            {
                this.id = id;
                this.price = price;
                this.point = point;
                this.summay = summay;
            }
        }
        
        public static string ToID(this ProductID id)
        {
            return _productMap[id].id;
        }

        public static int ToPrice(this ProductID id)
        {
            return _productMap[id].price;
        }

        public static int ToPoint(this ProductID id)
        {
            return _productMap[id].point;
        }

        public static string ToSummay(this ProductID id)
        {
            return _productMap[id].summay;
        }
    }

    
    public class PointPurchaseController : MonoBehaviour, IStoreListener
    {
        [SerializeField]
        private PointPurchaseDataSource _pointDataSource = default;
        private PointPurchaseDataSource pointDataSource
        {
            get
            {
                return _pointDataSource;
            }
        }

        [SerializeField]
        private TitleBar _titleBar = default;
        private TitleBar titleBar
        {
            get
            {
                return _titleBar;
            }
        }

        [SerializeField]
        private Image _purchaseCompleteImage = default;
        private Image purchaseCompleteImage
        {
            get
            {
                return _purchaseCompleteImage;
            }
        }

        [SerializeField]
        private Text _purchaseCompleteText = default;
        private Text purchaseCompleteText
        {
            get
            {
                return _purchaseCompleteText;
            }
        }

        [SerializeField]
        private Text _pointBalanceText = default;
        private Text pointBalanceText
        {
            get
            {
                return _pointBalanceText;
            }
        }

        private IStoreController _storeController;
        private IExtensionProvider _storeExtensionProvider;

        void Start()
        {
            pointDataSource.onUpdate.AddListener(ApplyPointBalance);

            StartCoroutine(ApiHelper.Instance.GetPoint(res =>
            {
                if (res.ErrorCode != null)
                {
                    DebugUtil.LogError("GetPoint Error -> " + res.ErrorCode + " : " + res.ErrorMessage);
                    return;
                }
                pointDataSource.pointBalance = res.Point;
            }));

            if (_storeController == null)
            {
                InitializePurchasing();
            }
        }
        
        /// <summary>
        /// ポイント残高の更新.
        /// </summary>
        /// <param name="pointBalance"></param>
        private void ApplyPointBalance(int pointBalance)
        {
            var pointText = "ポイント残高" + pointBalance.ToString(AppConst.PointFormat);
            titleBar.summary.text = pointText;
            pointBalanceText.text = pointText;
        }

        /// <summary>
        /// IAPの初期化.
        /// </summary>
        public void InitializePurchasing()
        {
            if (IsInitialized())
            {
                return;
            }
            
            var spm = StandardPurchasingModule.Instance();

#if UNITY_EDITOR
            spm.useFakeStoreUIMode = FakeStoreUIMode.DeveloperUser;
#endif
            var builder = ConfigurationBuilder.Instance(spm);

            // 販売するプロダクトの追加.
            builder.AddProduct(ProductID.Points1000.ToID(), ProductType.Consumable);
            builder.AddProduct(ProductID.Points5000.ToID(), ProductType.Consumable);
            builder.AddProduct(ProductID.Points10000.ToID(), ProductType.Consumable);

            // 非同期呼び出しで、設定およびこのクラスのインスタンスをパスして 
            // セットアップの残りを開始する。OnInitialized か OnInitializeFailed 内でレスポンスが起こる。
            UnityPurchasing.Initialize(this, builder);
        }

        /// <summary>
        /// 初期化済判定.
        /// </summary>
        /// <returns></returns>
        private bool IsInitialized()
        {
            return _storeController != null && _storeExtensionProvider != null;
        }

        /// <summary>
        /// ポイント購入処理.
        /// </summary>
        /// <param name="product"></param>
        [EnumAction(typeof(ProductID))]
        public void BuyConsumablePoint(int p)
        {
            // レスポンスは ProcessPurchase または OnPurchaseFailed 経由で非同期的に発生します。
            BuyProductID(((ProductID)p).ToID());
        }

        /// <summary>
        /// Productの購入処理.
        /// </summary>
        /// <param name="productId"></param>
        void BuyProductID(string productId)
        {
            if (IsInitialized())
            {
                // プロダクト識別子と Purchasing システムの商品コレクションからProduct の参照を取得.
                Product product = _storeController.products.WithID(productId);

                // このデバイスのストア用のプロダクトが存在し、そのプロダクトが販売可能な状態であれば ... 
                if (product != null && product.availableToPurchase)
                {
                    DebugUtil.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    // ... プロダクトを購入します。ProcessPurchase または OnPurchaseFailed 経由で非同期的にレスポンスが発生します。
                    _storeController.InitiatePurchase(product);
                }
                else
                {
                    DebugUtil.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else
            {
                DebugUtil.Log("BuyProductID FAIL. Not initialized.");
            }
        }

        /// <summary>
        /// IAP初期化成功イベント.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="extensions"></param>
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            DebugUtil.Log("OnInitialized: PASS");            
            _storeController = controller;
            _storeExtensionProvider = extensions;
        }

        /// <summary>
        /// IAP初期化失敗イベント.
        /// </summary>
        /// <param name="error"></param>
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            DebugUtil.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }

        /// <summary>
        /// 購入イベント.
        /// </summary>
        /// <param name="purchaseEvent"></param>
        /// <returns>プロダクト購入が完全に受領されたかどうか</returns>
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            var processingResult = PurchaseProcessingResult.Complete;

            // レシートの検証.
            if (!isValidPurchase(purchaseEvent))
            {
                return processingResult;
            }

            ProductID? purchasedProduct = null;
            var ProductIDlist = Enum.GetValues(typeof(ProductID));
            foreach (ProductID product in ProductIDlist)
            {
                if (string.Equals(purchaseEvent.purchasedProduct.definition.id, product.ToID(), StringComparison.Ordinal))
                {
                    purchasedProduct = product;
                    break;
                }
            }

            // ある消耗型プロダクトがユーザーによって購入されました。
            if (purchasedProduct != null)
            {
                DebugUtil.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", purchaseEvent.purchasedProduct.definition.id));

                // アプリサーバーにポイントを登録.
                RegisterPurchasedPoint((ProductID)purchasedProduct, () =>
                {
                    // 購入済に更新.
                    DebugUtil.Log(string.Format("ConfirmPendingPurchase Product: '{0}'", purchaseEvent.purchasedProduct.definition.id));
                    _storeController.ConfirmPendingPurchase(purchaseEvent.purchasedProduct);
                });

                // サーバーに保存するまではPendingにする.
                processingResult = PurchaseProcessingResult.Pending;
            }
            else
            {
                DebugUtil.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", purchaseEvent.purchasedProduct.definition.id));
            }
            
            return processingResult;
        }

        /// <summary>
        /// アプリサーバーへのポイント登録.
        /// </summary>
        /// <param name="p"></param>
        public void RegisterPurchasedPoint(ProductID p, Action callback)
        {
            StartCoroutine(ApiHelper.Instance.SetPoint(p.ToPoint(), p.ToPrice(), res =>
            {
                if (res.ErrorCode != null)
                {
                    DebugUtil.LogError("SetPoint Error -> " + res.ErrorCode + " : " + res.ErrorMessage);
                    return;
                }

                pointDataSource.pointBalance = res.Point;
                titleBar.title.text = "ポイント購入完了";
                purchaseCompleteText.text = "決済が完了し、\n" + p.ToPoint().ToString(AppConst.PointFormat) + "が加算されました。";
                purchaseCompleteImage.gameObject.SetActive(true);

                if (callback != null)
                {
                    callback();
                }
            }));
        }

#if DEVELOP
        /// <summary>
        /// アプリサーバーへのポイント登録(ダミー購入用).
        /// </summary>
        /// <param name="p"></param>
        [EnumAction(typeof(ProductID))]
        public void RegisterPurchasedPoint(int p)
        {
            RegisterPurchasedPoint((ProductID)p, null);
        }
#endif

        /// <summary>
        /// レシートの検証処理.
        /// https://docs.unity3d.com/ja/current/Manual/UnityIAPValidatingReceipts.html
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool isValidPurchase(PurchaseEventArgs purchaseEvent)
        {
            bool validPurchase = true; // Presume valid for platforms with no R.V.
            
            // Unity IAP's validation logic is only included on these platforms.
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX)
            // Prepare the validator with the secrets we prepared in the Editor obfuscation window.
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);

            try
            {
                // On Google Play, result has a single product ID.
                // On Apple stores, receipts contain multiple products.
                var result = validator.Validate(purchaseEvent.purchasedProduct.receipt);
                // For informational purposes, we list the receipt(s)
                DebugUtil.Log("Receipt is valid. Contents:");
                foreach (IPurchaseReceipt productReceipt in result)
                {
                    DebugUtil.Log(productReceipt.productID);
                    DebugUtil.Log(productReceipt.purchaseDate);
                    DebugUtil.Log(productReceipt.transactionID);
                }
            }
            catch (IAPSecurityException)
            {
                DebugUtil.Log("Invalid receipt, not unlocking content");
                validPurchase = false;
            }
#endif
            return validPurchase;
        }
        
        /// <summary>
        /// 購入失敗イベント.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="failureReason"></param>
        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            DebugUtil.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        }
    }
}
