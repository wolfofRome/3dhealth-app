using Assets.Scripts.Common.Data;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using Assets.Scripts.Common;

namespace Assets.Scripts.ThreeDView
{
    [RequireComponent(typeof(AvatarController))]
    public class Human : BaseHuman
    {
        [SerializeField]
        private Gender _gender = Gender.Male;
        public override Gender Gender {
            get {
                return _gender;
            }
        }

        [SerializeField]
        private BodyType _bodyType = BodyType.Normal;
        public override BodyType BodyType {
            get {
                return _bodyType;
            }
        }

        [SerializeField]
        private FatMaterialConfig _fatMaterialConfig = default;

        [SerializeField]
        private RendererController _fatManController = default;
        [SerializeField]
        private RendererController _honeManController = default;
        [SerializeField]
        private RendererController _muscleManController = default;

        private HumanInitParam _initParams = new HumanInitParam();

        private AvatarController _avatar;
        public override AvatarController AvatarController {
            get {
                return _avatar = _avatar ?? gameObject.GetComponent<AvatarController>();
            }
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        private RendererController GetRendererController(HumanLayer layer)
        {
            switch (layer)
            {
                case HumanLayer.Fat: return _fatManController;
                case HumanLayer.Bone: return _honeManController;
                case HumanLayer.Muscle: return _muscleManController;
            }
            return null;
        }

        public override void SetActive(HumanLayer layer, bool active)
        {
            RendererController rc = GetRendererController(layer);
            if (rc != null)
            {
                rc.gameObject.SetActive(active);
            }
        }

        public override void SetBlendShape(string name, int value)
        {
            foreach (HumanLayer layer in Enum.GetValues(typeof(HumanLayer)))
            {
                RendererController rc = GetRendererController(layer);
                if (rc != null && rc.Renderer is SkinnedMeshRenderer)
                {
                    SkinnedMeshRenderer smr = (SkinnedMeshRenderer)rc.Renderer;
                    int idx = smr.sharedMesh.GetBlendShapeIndex(name);
                    if (idx > 0)
                    {
                        smr.SetBlendShapeWeight(idx, value);
                    }
                }
            }
        }

        public override void SetMaterialColor(HumanLayer layer, string materialName, Color albedoColor, Color specularColor)
        {
            var renderer = GetRendererController(layer);
            renderer.SetColor(materialName, albedoColor.r, albedoColor.g, albedoColor.b);
            renderer.SetSpecularColor(materialName, specularColor.r, specularColor.g, specularColor.b);
        }

        public override HumanInitParam HumanInitParam {
            get {
                return _initParams;
            }
        }

        public override void SetHumanInitParam(HumanInitParam param)
        {
            _fatManController.SetAlpha(param.GetAlpha(HumanLayer.Fat));
            _honeManController.SetAlpha(param.GetAlpha(HumanLayer.Bone));
            _muscleManController.SetAlpha(param.GetAlpha(HumanLayer.Muscle));
            _initParams = param;
        }

        public override void MulFatAlpha(FatPart part, float ratio)
        {
            Assert.IsNotNull(_fatMaterialConfig);
            Material[] mat_arr = _fatManController.Renderer.materials;
            string targetMatName = _fatMaterialConfig.GetMaterial(part).mainTexture.name;
            foreach (Material mat in mat_arr)
            {
                if (mat.mainTexture.name == targetMatName)
                {
                    Color tmpColor = mat.color;
                    tmpColor.a *= ratio;
                    mat.color = tmpColor;
                    MaterialUtil.SetBlendMode(mat, MaterialUtil.BlendMode.Fade);
                }
            }
        }
    }
}