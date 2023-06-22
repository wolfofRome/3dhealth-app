using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Common;

namespace Assets.Scripts.ThreeDView.BodyAnalysis
{
    [Serializable]
    public enum MarshmallowManParts
    {
        Waist,
        Hip,
        LeftThighs,
        RightThighs,
    }
    
    public class MarshmallowManController : MonoBehaviour
    {
        [SerializeField]
        private SkinnedMeshRenderer _partsMeshRenderer = default;

        [SerializeField]
        private Material _headMaterial = default;
        [SerializeField]
        private Material _waistMaterial = default;
        [SerializeField]
        private Material _hipMaterial = default;
        [SerializeField]
        private Material _leftThighsMaterial = default;
        [SerializeField]
        private Material _rightThighsMaterial = default;

        [SerializeField]
        private Texture _headSpriteDefault = default;
        [SerializeField]
        private Texture _headSpritePhase1 = default;
        [SerializeField]
        private Texture _headSpritePhase2 = default;

        [SerializeField]
        private Color _partsColorDefault = new Color(36f / 255f, 36f / 255f, 36f / 255f, 160f / 255f);
        [SerializeField]
        private Color _partsColorPhase1 = new Color(246f / 255f, 246f / 255f, 0f / 255f, 160f / 255f);
        [SerializeField]
        private Color _partsColorPhase2 = new Color(255f / 255f, 82f / 255f, 0f / 255f, 160f / 255f);

        // ブレンドシェイプMAX(100)時の理想値差%
        [SerializeField, Range(0, 1)]
        private float _partsMeshBlendShapeUnit = 0.25f;

        // 位置初期値(ObjImage変換時の位置補正(ViewportPoint指定))
        [SerializeField]
        private Vector2 _imageRectOffset = new Vector2(0.15f, 0);
        public Vector2 imageRectOffset
        {
            get
            {
                return _imageRectOffset;
            }
        }

        // 角度初期値
        [SerializeField]
        private Vector3 _defaultRotation = new Vector3(0, -45, 0);
        public Vector3 defaultRotation
        {
            get
            {
                return _defaultRotation;
            }
        }

        // 拡大/縮小初期値(カメラのfieldOfViewを操作)
        [SerializeField]
        private float _defaultScale = 1.1f;
        public float defaultScale
        {
            get
            {
                return _defaultScale;
            }
        }

        [SerializeField]
        private Camera _mainObjCamera = default;
        [SerializeField]
        private List<Transform> _balloonPointsWaist = default;
        [SerializeField]
        private List<Transform> _balloonPointsHip = default;
        [SerializeField]
        private List<Transform> _balloonPointsLeftThighs = default;
        [SerializeField]
        private List<Transform> _balloonPointsRightThighs = default;

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        private enum MarshmallowManPartsPhase
        {
            // 0%以下
            Default,
            // 10%未満
            Phase1,
            // 10%以上
            Phase2,
        }
        
        private MarshmallowManPartsPhase GetPartsPhase(float percentage)
        {
            if (percentage > 10)
            {
                return MarshmallowManPartsPhase.Phase2;
            }
            else if (percentage > 0)
            {
                return MarshmallowManPartsPhase.Phase1;
            }
            else
            {
                return MarshmallowManPartsPhase.Default;
            }
        }

        public void SetHeadMaterial(float percentage)
        {
            MarshmallowManPartsPhase phase = GetPartsPhase(percentage);

            Texture texture = null;

            switch (phase)
            {
                case MarshmallowManPartsPhase.Default:
                    texture = _headSpriteDefault;
                    break;
                case MarshmallowManPartsPhase.Phase1:
                    texture = _headSpritePhase1;
                    break;
                case MarshmallowManPartsPhase.Phase2:
                    texture = _headSpritePhase2;
                    break;
            }

            if (texture != null)
            {
                _headMaterial.mainTexture = texture;
            }
        }

        public void SetPartsMesh(MarshmallowManParts parts, float percentage)
        {
            SetPartsMeshMaterialColor(parts, percentage);
            SetPartsMeshBlendShapes(parts, percentage);
        }

        private void SetPartsMeshMaterialColor(MarshmallowManParts parts, float percentage)
        {
            MarshmallowManPartsPhase phase = GetPartsPhase(percentage);
            
            Color color = _partsColorDefault;
            
            switch (phase)
            {
                case MarshmallowManPartsPhase.Default:
                    color = _partsColorDefault;
                    break;
                case MarshmallowManPartsPhase.Phase1:
                    color = _partsColorPhase1;
                    break;
                case MarshmallowManPartsPhase.Phase2:
                    color = _partsColorPhase2;
                    break;
            }

            Material material = null;

            switch (parts)
            {
                case MarshmallowManParts.Waist:
                    material = _waistMaterial;
                    break;
                case MarshmallowManParts.Hip:
                    material = _hipMaterial;
                    break;
                case MarshmallowManParts.LeftThighs:
                    material = _leftThighsMaterial;
                    break;
                case MarshmallowManParts.RightThighs:
                    material = _rightThighsMaterial;
                    break;
            }

            if (material != null)
            {
                material.SetColor("_Color", color);
            }
        }

        private void SetPartsMeshBlendShapes(MarshmallowManParts parts, float percentage)
        {
            int blendShapeIndex = -1;

            switch (parts)
            {
                case MarshmallowManParts.Waist:
                    blendShapeIndex = _partsMeshRenderer.sharedMesh.GetBlendShapeIndex("Morph_Waist");
                    break;
                case MarshmallowManParts.Hip:
                    blendShapeIndex = _partsMeshRenderer.sharedMesh.GetBlendShapeIndex("Morph_Hip");
                    break;
                case MarshmallowManParts.LeftThighs:
                    blendShapeIndex = _partsMeshRenderer.sharedMesh.GetBlendShapeIndex("Morph_L_Foot");
                    break;
                case MarshmallowManParts.RightThighs:
                    blendShapeIndex = _partsMeshRenderer.sharedMesh.GetBlendShapeIndex("Morph_R_Foot");
                    break;
            }

            if (blendShapeIndex >= 0)
            {
                float weight = Mathf.Clamp(percentage / _partsMeshBlendShapeUnit, 0, 100);
                _partsMeshRenderer.SetBlendShapeWeight(blendShapeIndex, weight);
            }
        }

        public List<Vector3> GetPartsWorldPoints(MarshmallowManParts parts)
        {
            List<Transform> transforms = null;

            switch (parts)
            {
                case MarshmallowManParts.Waist:
                    transforms = _balloonPointsWaist;
                    break;
                case MarshmallowManParts.Hip:
                    transforms = _balloonPointsHip;
                    break;
                case MarshmallowManParts.LeftThighs:
                    transforms = _balloonPointsLeftThighs;
                    break;
                case MarshmallowManParts.RightThighs:
                    transforms = _balloonPointsRightThighs;
                    break;
            }

            List<Vector3> positions = new List<Vector3>();

            if (transforms != null)
            {
                Vector3 viewportPoint;
                Vector3 position;
                
                // ObjImage変換時の位置補正(ViewportPoint指定)
                Vector3 offset = new Vector2(-imageRectOffset.x, imageRectOffset.y);

                foreach (Transform transform in transforms)
                {
                    if (transform != null)
                    {
                        viewportPoint = _mainObjCamera.WorldToViewportPoint(transform.position);
                        viewportPoint += offset;
                        position = Camera.main.ViewportToWorldPoint(viewportPoint);
                        position.z = 0f;

                        positions.Add(position);
                    }
                }
            }
            
            return positions;
        }
    }
}
