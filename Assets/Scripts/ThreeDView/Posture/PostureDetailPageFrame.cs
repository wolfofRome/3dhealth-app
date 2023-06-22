using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Common;
using Assets.Scripts.Graphics.UI;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.ThreeDView.Posture
{
    [RequireComponent(typeof(RawImage))]
    public class PostureDetailPageFrame : BaseDescriptionPageFrame
    {
        public Angle angle { get; set; }

        [SerializeField]
        private LayerMask _cullingMaskForLength = default;

        [SerializeField]
        private LayerMask _cullingMaskForAngle = default;

        [SerializeField]
        private RenderTextureController _prefabLineRenderTextureController = default;

        [SerializeField]
        private ImageTextHolder _prefabDiffValue = default;

        [SerializeField]
        private LineRenderer _prefabLineRenderer = default;

        [SerializeField]
        private Color _baseLineColor = new Color(0f, 0.796f, 1f);

        [SerializeField]
        private Color _measurementLineColor = new Color(1f, 0.1607f, 0.1607f);


        [SerializeField]
        private RawImage _lineImage = default;
        protected RawImage lineImage {
            get {
                return _lineImage;
            }
        }
        
        private MeshAlphaController _objAlphaController;
        public MeshAlphaController objAlphaController
        {
            get
            {
                return _objAlphaController;
            }
            set
            {
                _objAlphaController = value;
            }
        }

        private RenderTextureController _lineRenderTextureController;
        
        private List<LineRenderer> _baseLineRendererList;

        private List<LineRenderer> _measurementLineRendererList;

        private List<ImageTextHolder> _diffValueList;

        /// <summary>
        /// ページ生成イベント.
        /// </summary>
        /// <param name="pagePosition"></param>
        /// <param name="anchorMin"></param>
        /// <param name="anchorMax"></param>
        /// <returns></returns>
        public override GameObject Instantiate(int pagePosition, Vector2 anchorMin, Vector2 anchorMax)
        {
            base.Instantiate(pagePosition, anchorMin, anchorMax);

            return gameObject;
        }

        /// <summary>
        /// ページバインドイベント.
        /// </summary>
        /// <param name="pagePosition"></param>
        /// <param name="anchorMin"></param>
        /// <param name="anchorMax"></param>
        public override void Bind(int pagePosition, Vector2 anchorMin, Vector2 anchorMax)
        {
            base.Bind(pagePosition, anchorMin, anchorMax);

            // バインド時に子オブジェクトが生成されていなければ生成する
            InstantiateDetailObject();
        }

        /// <summary>
        /// ページアンバインド
        /// </summary>
        /// <param name="pagePosition"></param>
        public override void UnBind(int pagePosition)
        {
            base.UnBind(pagePosition);

            // アンバインド時に子オブジェクトを削除する
            DestroyDetailObject();
        }

        /// <summary>
        /// フォーカス取得イベント.
        /// </summary>
        public override void OnGotFocus()
        {
            base.OnGotFocus();
            SetLineEnable(true);
        }

        /// <summary>
        /// フォーカス消失イベント.
        /// </summary>
        public override void OnLostFocus()
        {
            base.OnLostFocus();
            SetLineEnable(false);
        }

        /// <summary>
        /// 子オブジェクトの生成
        /// </summary>
        private void InstantiateDetailObject()
        {
            if (objRenderTextureController != null)
            {
                return;
            }
            
            // OBJ描画用のカメラを生成.
            objRenderTextureController = Instantiate(PrefabObjRenderTextureController);
            objRenderTextureController.transform.SetParent(CameraParent, false);
            objRenderTextureController.targetImage = objImage;

            switch (angle)
            {
                case Angle.Left:
                case Angle.Right:
                case Angle.Front:
                case Angle.Back:
                    // カメラをフォーカス位置に合わせて調整.
                    objRenderTextureController.transform.Translate(0, Result.midScaledFocusPoint.y - CameraParent.transform.position.y - CameraPositionOffsetY, 0);
                    break;
                case Angle.Top:
                    // カメラをフォーカス位置に合わせて調整.
                    objRenderTextureController.transform.Translate(0, -0.1f, 0);
                    break;
                case Angle.Under:
                default:
                    break;
            }

            // CullingMaskの設定にて、姿勢検証のタイプ毎に表示対象を変更する.
            switch (Result._point.ToRangeThreshold().type)
            {
                default:
                case VerifyType.Length:
                    objRenderTextureController.targetCamera.cullingMask = _cullingMaskForLength;
                    objAlphaController.AddObjCamera(objRenderTextureController.targetCamera);
                    break;
                case VerifyType.Angle:
                    objRenderTextureController.targetCamera.cullingMask = _cullingMaskForAngle;
                    break;
            }

            // OBJを描画
            objRenderTextureController.UpdateRenderTexture();

            var basePoints = Result.scaledBaseLinePoints;
            var measurementPoints = Result.scaledMeasurementLinePoints;

            // ライン描画.
            if ((basePoints.Count > 0) || (measurementPoints.Count > 0))
            {
                _lineRenderTextureController = _lineRenderTextureController ?? Instantiate(_prefabLineRenderTextureController);

                _lineRenderTextureController.transform.SetParent(CameraParent, false);
                _lineRenderTextureController.targetImage = lineImage;
                lineImage.transform.SetAsLastSibling();

                // 基準となるラインを描画
                if (_baseLineRendererList == null && basePoints.Count > 0)
                {
                    _baseLineRendererList = new List<LineRenderer>();
                    foreach (var linPoints in basePoints)
                    {
                        var renderer = Instantiate(_prefabLineRenderer);
                        renderer.transform.SetParent(transform, false);
                        renderer.startColor = _baseLineColor;
                        renderer.endColor = _baseLineColor;
                        renderer.positionCount = linPoints.Length;
                        renderer.SetPositions(TransformCoordinate(linPoints, objRenderTextureController.targetCamera, _lineRenderTextureController.targetCamera));
                        renderer.gameObject.SetActive(false);
                        _baseLineRendererList.Add(renderer);
                    }
                }

                // 測定対象のラインを描画
                if (_measurementLineRendererList == null && measurementPoints.Count > 0)
                {
                    _measurementLineRendererList = new List<LineRenderer>();
                    foreach (var linPoints in measurementPoints)
                    {
                        var renderer = Instantiate(_prefabLineRenderer);
                        renderer.transform.SetParent(transform, false);
                        renderer.startColor = _measurementLineColor;
                        renderer.endColor = _measurementLineColor;
                        renderer.positionCount = linPoints.Length;
                        renderer.SetPositions(TransformCoordinate(linPoints, objRenderTextureController.targetCamera, _lineRenderTextureController.targetCamera));
                        renderer.gameObject.SetActive(false);
                        _measurementLineRendererList.Add(renderer);
                    }
                }
            }
            else
            {
                lineImage.gameObject.SetActive(false);
            }

            // 値を表示.
            if (_diffValueList == null && Result.dispValues.Length > 0)
            {
                _diffValueList = new List<ImageTextHolder>();
                var length = Result.dispValues.Length;
                for (var i = 0; i < length; i++)
                {
                    var value = Result.dispValues[i];
                    var diffValue = Instantiate(_prefabDiffValue);
                    diffValue.transform.SetParent(transform, false);
                    diffValue.text.text = string.Format("{0:0.0}" + Result.threshold.type.ToMeasurementUnit(), Mathf.Abs(value));

                    if (basePoints.Count > 0 && measurementPoints.Count > 0)
                    {
                        var baseLastPoint = basePoints[i].Last();
                        var measurementLastPoint = measurementPoints[i].Last();
                        var p1 = _lineRenderTextureController.targetCamera.WorldToScreenPoint(measurementLastPoint);
                        var p2 = _lineRenderTextureController.targetCamera.WorldToScreenPoint(baseLastPoint);
                        var delta = p1 - p2;

                        // 90°単位の角度に変換.
                        var zDegree = Mathf.RoundToInt(Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg / 90) * 90;

                        // 膝のFTA角度の場合は矢印の向きを反転させる.
                        if (Result.point == PostureVerifyPoint.KneeJointAngle)
                        {
                            zDegree += 180;
                        }
                        diffValue.image.transform.Rotate(0, 0, zDegree);
                        diffValue.image.gameObject.SetActive(true);
                    }

                    diffValue.transform.position = objRenderTextureController.targetCamera.WorldToScreenPoint(Result.scaledFocusPoints[i]);
                    if (angle == Angle.Back && (Result.condition == PostureCondition.LeftTiltPelvis || Result.condition == PostureCondition.RightTiltPelvis))
                    {
                        diffValue.transform.Translate(transform.position + new Vector3(-120, -5, 0));
                    }
                    else
                    {
                        diffValue.transform.Translate(transform.position + new Vector3(10, -5, 0));
                    }
                    diffValue.transform.SetAsLastSibling();
                    _diffValueList.Add(diffValue);
                }
            }
        }

        /// <summary>
        /// 子オブジェクトの削除
        /// </summary>
        private void DestroyDetailObject()
        {
            if (objImage != null)
            {
                Destroy(objImage.texture);
                objImage.texture = null;
            }

            if (objRenderTextureController != null)
            {
                Destroy(objRenderTextureController.gameObject);
                objRenderTextureController = null;
            }

            if (lineImage.texture != null)
            {
                Destroy(lineImage.texture);
                lineImage.texture = null;
            }

            if (_baseLineRendererList != null)
            {
                foreach (var renderer in _baseLineRendererList)
                {
                    Destroy(renderer.gameObject);
                }

                _baseLineRendererList.Clear();
                _baseLineRendererList = null;
            }

            if (_measurementLineRendererList != null)
            {
                foreach (var renderer in _measurementLineRendererList)
                {
                    Destroy(renderer.gameObject);
                }

                _measurementLineRendererList.Clear();
                _measurementLineRendererList = null;
            }

            if (_lineRenderTextureController != null)
            {
                Destroy(_lineRenderTextureController.gameObject);
                _lineRenderTextureController = null;
            }

            if (_diffValueList != null)
            {
                foreach (var renderer in _diffValueList)
                {
                    Destroy(renderer.gameObject);
                }

                _diffValueList.Clear();
                _diffValueList = null;
            }
        }

        /// <summary>
        /// 姿勢検証個所のラインの表示／非表示切替.
        /// </summary>
        /// <param name="enable"></param>
        private void SetLineEnable(bool enable)
        {
            if (_measurementLineRendererList != null)
            {
                foreach (var renderer in _measurementLineRendererList)
                {
                    renderer.gameObject.SetActive(enable);
                }
            }

            if (_baseLineRendererList != null)
            {
                foreach (var renderer in _baseLineRendererList)
                {
                    renderer.gameObject.SetActive(enable);
                }
            }

            if (_lineRenderTextureController != null)
            {
                _lineRenderTextureController.UpdateRenderTexture();
            }
        }
    }
}
