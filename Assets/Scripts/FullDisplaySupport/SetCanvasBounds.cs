using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Common.Animators;
using Assets.Scripts.ThreeDView;

namespace Assets.Scripts.FullDisplaySupport
{
    public class SetCanvasBounds : MonoBehaviour
    {

        public GameObject header;
        public GameObject footer;

        private const float HeaderHeight = SafeArea.HeaderHeight;
        private const float FooterHeight = SafeArea.FooterHeight;

        public GameObject contentArea;

        // Use this for initialization
        private void Start()
        {
#if UNITY_IPHONE
            if (SafeArea.HasSafeArea())
            {
                ApplySafeArea();
            }
#endif
            
        }

        private void ApplySafeArea()
        {
            if (header != null)
            {
                LayoutElement headerLayout = header.GetComponent<LayoutElement>();
                headerLayout.preferredHeight = HeaderHeight;
            }

            if (footer != null)
            {
                LayoutElement footerLayout = footer.GetComponent<LayoutElement>();
                footerLayout.preferredHeight = FooterHeight;
            }


            if (gameObject.CompareTag("ThreeDView"))
            {
                float totalOffset = HeaderHeight + FooterHeight;

                Transform childTransform = contentArea.transform;

                Transform defaultPoseControlPanel = null;
                Transform defaultPoseImages = null;
                Transform titleBar = null;
                Transform menuGroupContent = null;
                Transform distortionDescription = null;
                foreach (Transform child in childTransform)
                {
                    switch (child.transform.name)
                    {
                        case "DefaultPoseControlPanel":
                            defaultPoseControlPanel = child;
                            break;
                        case "DefaultPoseImages":
                            defaultPoseImages = child;
                            break;
                        case "MenuGroup":
                            titleBar = child.transform.Find("TitleBar").GetComponent<Transform>();
                            menuGroupContent = child.Find("Content").GetComponent<Transform>();
                            break;
                        case "DistortionDescription":
                            distortionDescription = child;
                            break;
                    }
                }

                if (defaultPoseImages != null && defaultPoseControlPanel != null && titleBar != null)
                {
                    RectTransform defaultPoseImagesRect = defaultPoseImages.GetComponent<RectTransform>();

                    defaultPoseControlPanel.GetComponent<SlideAnimator>().inPosition = new Vector2(defaultPoseControlPanel.GetComponent<SlideAnimator>().inPosition.x, 452.0f);
                    
                    const float defaultPoseImagesHeight = 1000.0f;

                    //姿勢比較の比較画像のサイズ調整
                    defaultPoseImagesRect.sizeDelta = new Vector2(contentArea.GetComponent<RectTransform>().sizeDelta.x / 2, defaultPoseImagesHeight);

                    //姿勢比較の比較画像のy座標(コントロールパネルの高さを基準に足の位置を指定)
                    defaultPoseImagesRect.anchoredPosition = new Vector2(0, defaultPoseControlPanel.GetComponent<SlideAnimator>().inPosition.y + 60);

                    //姿勢比較のカメラの調整
                    defaultPoseImages.GetComponent<DefaultPoseController>()._objCameraOffset = new Vector3(0.37f, -0.2f, 1.95f);
                }

                if (menuGroupContent != null)
                {
                    // MenuGroup/Contentの高さを調整
                    LayoutElement menuGroupContentLayout = menuGroupContent.GetComponent<LayoutElement>();
                    menuGroupContentLayout.preferredHeight -= totalOffset;

                    // スクロール領域の下部にフッター分の余白を追加
                    Transform dataPanel = menuGroupContent.Find("DataPanel").GetComponent<Transform>();
                    if (dataPanel != null)
                    {
                        // データ - サイズ
                        ScrollRect measurementScrollRect = dataPanel.Find("MeasurementDataPanel").GetComponent<ScrollRect>();
                        VerticalLayoutGroup measurementContentLayout = measurementScrollRect.content.GetComponent<VerticalLayoutGroup>();
                        measurementContentLayout.padding.bottom += (int)FooterHeight;

                        // データ - 体組成
                        ScrollRect bodyCompositionScrollRect = dataPanel.Find("BodyCompositionDataPanel").GetComponent<ScrollRect>();
                        VerticalLayoutGroup bodyCompositionContentLayout = bodyCompositionScrollRect.content.GetComponent<VerticalLayoutGroup>();
                        bodyCompositionContentLayout.padding.bottom += (int)FooterHeight;
                    }
                }

                if (distortionDescription != null)
                {
                    SlideAnimator slideAnimator = distortionDescription.GetComponent<SlideAnimator>();
                    RectTransform rectTransform = distortionDescription.GetComponent<RectTransform>();
                    Vector2 sizeDelta = rectTransform.sizeDelta;
                    sizeDelta = new Vector2(sizeDelta.x, sizeDelta.y - (SafeArea.HeaderHeight + SafeArea.FooterHeight));
                    rectTransform.sizeDelta = sizeDelta;
                    slideAnimator.inPosition = new Vector2(0, slideAnimator.inPosition.y - (SafeArea.HeaderHeight + SafeArea.FooterHeight));
                }
            }

            // ReSharper disable once InvertIf
            if (gameObject.CompareTag("Compare"))
            {
                Transform childTransform = contentArea.transform;
                foreach (Transform child in childTransform)
                {
                    if (child.transform.name == "TitleBar")
                    {
                        child.GetComponent<SlideAnimator>().inPosition = new Vector2(0, -HeaderHeight);
                        child.GetComponent<RectTransform>().anchoredPosition = new Vector2(child.GetComponent<RectTransform>().anchoredPosition.x, child.GetComponent<RectTransform>().anchoredPosition.y - HeaderHeight);
                    }
                    if (child.transform.name == "HomeMenu")
                    {
                        child.GetComponent<RectTransform>().anchoredPosition = new Vector2(child.GetComponent<RectTransform>().anchoredPosition.x, child.GetComponent<RectTransform>().anchoredPosition.y + FooterHeight);
                    }
                }

                Transform rightObjCamera = GameObject.Find("Humans/CameraPivot/ObjCameraRight").GetComponent<Transform>();
                Transform leftObjCamera = GameObject.Find("Humans/CameraPivot/ObjCameraLeft").GetComponent<Transform>();

                //カメラの距離調整
                var rightPosition = rightObjCamera.position;
                rightPosition = new Vector3(rightPosition.x, rightPosition.y, 2.5f);
                rightObjCamera.position = rightPosition;
                var leftPosition = leftObjCamera.position;
                leftPosition = new Vector3(leftPosition.x, leftPosition.y, 2.5f);
                leftObjCamera.position = leftPosition;
            }
        }
    }
}
