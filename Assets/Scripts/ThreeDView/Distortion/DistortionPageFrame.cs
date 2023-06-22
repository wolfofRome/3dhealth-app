using UnityEngine;

namespace Assets.Scripts.ThreeDView.Distortion
{
    public class DistortionPageFrame : BaseDescriptionPageFrame
    {
        public override GameObject Instantiate(int pagePosition, Vector2 anchorMin, Vector2 anchorMax)
        {
            base.Instantiate(pagePosition, anchorMin, anchorMax);

            // OBJ描画用のカメラを生成.
            if (objRenderTextureController != null)
            {
                return base.Instantiate(pagePosition, anchorMin, anchorMax);
            }
            objRenderTextureController = Instantiate(PrefabObjRenderTextureController, CameraParent, false);
            objRenderTextureController.targetImage = objImage;

            // カメラをフォーカス位置に合わせて調整.
            objRenderTextureController.transform.Translate(0, Result.midScaledFocusPoint.y - CameraParent.transform.position.y - CameraPositionOffsetY, 0);

            return base.Instantiate(pagePosition, anchorMin, anchorMax);
        }

        public override void Bind(int pagePosition, Vector2 anchorMin, Vector2 anchorMax)
        {
            base.Bind(pagePosition, anchorMin, anchorMax);
            objRenderTextureController.UpdateRenderTexture();
        }
    }
}
