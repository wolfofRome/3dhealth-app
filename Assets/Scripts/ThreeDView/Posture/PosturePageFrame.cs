using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Common.Config;
using Assets.Scripts.Common.Graphics.UI.ViewPager;
using Assets.Scripts.Common;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Assets.Scripts.Graphics;

namespace Assets.Scripts.ThreeDView.Posture
{
    public enum Angle
    {
        Left = 0,
        Right = 1,
        Front = 2,
        Back = 3,
        Top = 4,
        Under = 5
    }
    
    public class PosturePageFrame : PageFrame
    {
        [SerializeField]
        private RenderTextureController _renderTextureController = default;
        protected RenderTextureController renderTextureController {
            get {
                return _renderTextureController;
            }
        }

        [SerializeField]
        private BodyTypeSelector _bodyTypeSelector = default;
        protected BodyTypeSelector bodyTypeSelector {
            get {
                return _bodyTypeSelector;
            }
        }
        
        [SerializeField]
        private MeshAlphaController _objAlphaController = default;

        [SerializeField]
        private Badge _warningBadge = default;
        
        [SerializeField]
        private SlideMenuController _postureDetailMenu = default;
        
        [SerializeField]
        private ViewPager _postureDetailViewPager = default;

        [SerializeField]
        private PostureWarningItem _prefabWarningItem = default;

        [SerializeField]
        private PostureDetailPageFrame _prefabFrame = default;

        [SerializeField]
        private ContentsLoader _loader = default;

        [SerializeField]
        private PostureAdviceAssetConfig _adviceConfig = default;

        private Dictionary<PostureVerifyPoint, PostureWarningItem> _postureWarningItemMap = new Dictionary<PostureVerifyPoint, PostureWarningItem>();


        private Angle _angle;

        private bool _isRunning = false;

        private Dictionary<PostureVerifyPoint, PostureVerifyer.Result> _results;

        private PostureVerifyer.Result[] _abnormalResultsByAngle;
        
        private Dictionary<BonePart, MaterialColorSettings> _boneMaterialMap = new Dictionary<BonePart, MaterialColorSettings>();

        /// <summary>
        /// ViewPagerのページ生成イベント.
        /// </summary>
        /// <param name="pagePosition"></param>
        /// <param name="anchorMin"></param>
        /// <param name="anchorMax"></param>
        /// <returns></returns>
        public override GameObject Instantiate(int pagePosition, Vector2 anchorMin, Vector2 anchorMax)
        {
            base.Instantiate(pagePosition, anchorMin, anchorMax);

            _angle = (Angle)pagePosition;

            StartCoroutine(WaitForPostureVerifyProcess(()=>
            {
                _abnormalResultsByAngle = GetAbnormalResultsByAngle(_angle);

                // 骨、軟骨のマテリアルの色設定を初期化.
                var boneParts = Enum.GetValues(typeof(BonePart));
                foreach (BonePart p in boneParts)
                {
                    _boneMaterialMap[p] = p.isCartilage() ? _adviceConfig.cartilageNormalMaterialSetting : _adviceConfig.boneNormalMaterialSetting;
                }

                foreach (var r in _abnormalResultsByAngle)
                {
                    // 骨、軟骨のマテリアルの異常個所の色を設定.
                    var parts = r.point.GetBoneParts();
                    foreach (var p in parts)
                    {
                        _boneMaterialMap[p] = p.isCartilage() ? _adviceConfig.cartilageAbnormalMaterialSetting : _adviceConfig.boneAbnormalMaterialSetting;
                    }
                    // 異常個所の「!」とアドバイス画面を追加.
                    AddWarning(_angle, r);
                }
                _warningBadge.number = _abnormalResultsByAngle.Length;
                _warningBadge.gameObject.SetActive(_abnormalResultsByAngle.Length > 0);

                var max = (int)Enum.GetValues(typeof(Angle)).Cast<Angle>().Max();
                if (pagePosition == max)
                {
                    _postureDetailViewPager.adapter.NotifyDataSetChanged();
                }
            }));

            _loader.onLoadCompleted.AddListener(contents =>
            {
                renderTextureController.UpdateRenderTexture();
            });
            
            return gameObject;
        }

        /// <summary>
        /// ViewPagerのデータバインドイベント.
        /// </summary>
        public override void Bind(int pagePosition, Vector2 anchorMin, Vector2 anchorMax)
        {
            base.Bind(pagePosition, anchorMin, anchorMax);
            
            renderTextureController.UpdateRenderTexture();
        }

        /// <summary>
        /// ViewPagerのページ切替におけるフォーカス取得イベント.
        /// </summary>
        public override void OnGotFocus()
        {
            base.OnGotFocus();
            
            // 骨、軟骨の色を変更.
            foreach (var b in _boneMaterialMap)
            {
                bodyTypeSelector.SelectedHuman.SetMaterialColor(
                    HumanLayer.Bone,
                    b.Key.ToString(), 
                    b.Value.albedoColor, 
                    b.Value.specularColor);
            }

            // 再描画.
            renderTextureController.UpdateRenderTexture();
        }

        /// <summary>
        /// 姿勢検証の完了待機処理.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IEnumerator WaitForPostureVerifyProcess(Action action)
        {
            if (_isRunning)
            {
                yield break;
            }
            _isRunning = true;

            while (_results == null)
            {
                yield return null;
            }
            _isRunning = false;

            action();
        }

        /// <summary>
        /// 姿勢検証完了イベント.
        /// </summary>
        /// <param name="verifyer"></param>
        public void OnPostureVerifyCompleted(PostureVerifyer verifyer)
        {
            _results = verifyer.results;
        }

        /// <summary>
        /// 姿勢検証における異常個所UI「!」の追加.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="result"></param>
        private void AddWarning(Angle angle, PostureVerifyer.Result result)
        {
            // 警告ボタンの追加.
            var item = Instantiate(_prefabWarningItem);
            item.transform.SetParent(transform, false);
            item.transform.position = renderTextureController.targetCamera.WorldToScreenPoint(result.midScaledFocusPoint);
            item.transform.Translate(transform.position);

            // 異常個所の詳細画面を追加.
            AddDetailPage(item, angle, result);

            _postureWarningItemMap.Add(result.point, item);
        }

        /// <summary>
        /// 姿勢検証の詳細ページの追加.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="angle"></param>
        /// <param name="result"></param>
        private void AddDetailPage(PostureWarningItem item, Angle angle, PostureVerifyer.Result result)
        {
            var pageNum = _postureDetailViewPager.adapter.GetCount();
            item.warningButton.onDown.AddListener(() =>
            {
                // アドバイス画面を表示.
                _postureDetailViewPager.SetPagePosition(pageNum, false);
                _postureDetailMenu.gameObject.SetActive(true);
                _postureDetailMenu.Show();
            });

            var frame = Instantiate(_prefabFrame);
            frame.transform.SetParent(_postureDetailViewPager.adapter.transform, false);
            frame.angle = angle;
            frame.Result = result;
            frame.CameraParent = renderTextureController.transform;
            frame.objAlphaController = _objAlphaController;
            _postureDetailViewPager.adapter.AddItem(frame);
        }

        /// <summary>
        /// 各視点における異常個所の取得.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        private PostureVerifyer.Result[] GetAbnormalResultsByAngle(Angle angle)
        {
            PostureVerifyPoint[] points = null;
            var resultList = new List<PostureVerifyer.Result>();

            switch (angle)
            {
                case Angle.Left:
                case Angle.Right:
                    points = new PostureVerifyPoint[]
                    {
                        PostureVerifyPoint.UpperInclinationRatio,
                        PostureVerifyPoint.LowerInclinationRatio,
                        PostureVerifyPoint.PelvisInclinationAngle,
                        PostureVerifyPoint.ThoracicVertebraAngle,
                        PostureVerifyPoint.LumbarVertebraAngle,
                        PostureVerifyPoint.NeckCurveAngle,
                    };
                    break;
                case Angle.Front:
                case Angle.Back:
                    points = new PostureVerifyPoint[]
                    {
                        PostureVerifyPoint.BodyInclinationRatio,
                        PostureVerifyPoint.UpperLegLengthRatio,
                        PostureVerifyPoint.LowerLegLengthRatio,
                        PostureVerifyPoint.ShoulderInclinationRatio,
                        PostureVerifyPoint.PelvisInclinationRatio,
                        PostureVerifyPoint.KneeJointAngle,
                    };
                    break;
                case Angle.Top:
                    points = new PostureVerifyPoint[]
                    {
                        PostureVerifyPoint.ShoulderDistortionAngle,
                        PostureVerifyPoint.NeckDistortionAngle,
                    };
                    break;
                case Angle.Under:
                    points = new PostureVerifyPoint[]
                    {
                        PostureVerifyPoint.UpperBodyDistortionAngle,
                        PostureVerifyPoint.KneeAnteroposteriorDiffRatio,
                    };
                    break;
            }

            if (points != null)
            {
                PostureVerifyer.Result r = null;
                foreach (var p in points)
                {
                    if (_results.TryGetValue(p, out r) && r.condition != PostureCondition.Normal)
                    {
                        resultList.Add(r);
                    }
                }
            }

            return resultList.ToArray();
        }
    }
}
