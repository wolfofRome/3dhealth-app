using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.Common.Config;
using Assets.Scripts.Common.Graphics.UI.ViewPager;
using Assets.Scripts.ThreeDView.Posture;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Assets.Scripts.ThreeDView.Distortion
{
    public class DistortionController : MonoBehaviour
    {
        [FormerlySerializedAs("_config")] [SerializeField]
        private DistortionConfig config;
        
        [FormerlySerializedAs("_blockman")] [SerializeField]
        private BlockmanController blockman;

        [FormerlySerializedAs("_touchHandler")] [SerializeField]
        private BlockmanTouchHandler touchHandler;

        [FormerlySerializedAs("_distortionViewPager")] [SerializeField]
        private ViewPager distortionViewPager;

        [FormerlySerializedAs("_descriptionPanel")] [SerializeField]
        private SlideMenuController descriptionPanel;

        [FormerlySerializedAs("_prefabPageFrame")] [SerializeField]
        private DistortionPageFrame prefabPageFrame;

        [FormerlySerializedAs("_blockmanCameraParent")] [SerializeField]
        private Transform blockmanCameraParent;

        [FormerlySerializedAs("_description")] [SerializeField]
        private Text description;

        [FormerlySerializedAs("_adviceController")] [SerializeField]
        private AdviceController adviceController;

        private readonly Dictionary<BlockmanPart, int> _descriptionMap = new Dictionary<BlockmanPart, int>();

        private PostureVerifyer _verifier;

        private bool _isRunning;

        private void Start()
        {
            touchHandler.onClick.AddListener(OnClick);
            distortionViewPager.onPageChanged.AddListener(OnPageChanged);
            StartCoroutine(WaitForPostureVerifyProcess(ApplyPostureVerifyResult));
        }

        private void OnPageChanged(int pagePosition)
        {
            var frame = (DistortionPageFrame)distortionViewPager.adapter.GetItem(pagePosition);
            if (frame == null) return;
            description.text = string.Format(frame.Result.description);
            adviceController.postureCondition = frame.Result.condition;
        }

        private IEnumerator WaitForPostureVerifyProcess(Action action)
        {
            if (_isRunning)
            {
                yield break;
            }
            _isRunning = true;

            while (_verifier == null)
            {
                yield return null;
            }
            _isRunning = false;

            action();
        }

        public void OnClick(BlockmanPart part, RaycastHit hitInfo)
        {
            if (!_descriptionMap.ContainsKey(part)) return;
            distortionViewPager.SetPagePosition(_descriptionMap[part], false);
            descriptionPanel.Show();
        }

        public void OnPostureVerifyCompleted(PostureVerifyer verifier)
        {
            _verifier = verifier;
        }

        private void ApplyPostureVerifyResult()
        {
            var textureMap = config.BlockmanTextureList.ToDictionary(x => x.Part);

            // テクスチャの初期化.
            foreach (var item in textureMap)
            {
                blockman.meshRenderer.materials[(int)item.Key].mainTexture = item.Value.GoodTexture;
            }

            var warningCount = 0;
            
            foreach (var result in _verifier.results)
            {
                if (result.Value.condition == PostureCondition.Normal || !WarningPartMap.ContainsKey(result.Key)) continue;
                // 異常個所のテクスチャの差し替え.
                var parts = WarningPartMap[result.Key];
                foreach (var p in parts)
                {
                    blockman.meshRenderer.materials[(int)p].mainTexture = textureMap[p].BadTexture;
                    if (!_descriptionMap.ContainsKey(p))
                    {
                        _descriptionMap[p] = distortionViewPager.adapter.GetCount();
                    }
                }

                // 症状説明画面の追加.
                var pageItem = Instantiate(prefabPageFrame, distortionViewPager.adapter.transform, false);
                pageItem.CameraParent = blockmanCameraParent;
                pageItem.Result = result.Value;
                distortionViewPager.adapter.AddItem(pageItem);
                warningCount++;
            }
            distortionViewPager.adapter.NotifyDataSetChanged();

            // 1箇所でも異常がある場合は顔のテクスチャを変更.
            if (warningCount > 0)
            {
                blockman.meshRenderer.materials[(int)BlockmanPart.Face].mainTexture = textureMap[BlockmanPart.Face].BadTexture;
            }
        }

        private static readonly Dictionary<PostureVerifyPoint, BlockmanPart[]> WarningPartMap = new Dictionary<PostureVerifyPoint, BlockmanPart[]>
        {
            {PostureVerifyPoint.PelvisInclinationAngle, new[] { BlockmanPart.Senkotsu, BlockmanPart.UpLegL, BlockmanPart.UpLegR } },
            {PostureVerifyPoint.ThoracicVertebraAngle,  new[] { BlockmanPart.MuneL, BlockmanPart.MuneR, BlockmanPart.HaraUp } },
            {PostureVerifyPoint.LumbarVertebraAngle,  new[] { BlockmanPart.HaraUp, BlockmanPart.HaraDown } },
            {PostureVerifyPoint.NeckCurveAngle,  new[] { BlockmanPart.NeckUp, BlockmanPart.NeckDown } },
            {PostureVerifyPoint.KneeJointAngle,  new[] { BlockmanPart.Leg1L, BlockmanPart.Leg2L, BlockmanPart.Leg1R, BlockmanPart.Leg2R } },
            {PostureVerifyPoint.ShoulderDistortionAngle,  new[] { BlockmanPart.KataL, BlockmanPart.KataR } },
            {PostureVerifyPoint.NeckDistortionAngle,  new[] { BlockmanPart.NeckUp } }
        };
    }
}
