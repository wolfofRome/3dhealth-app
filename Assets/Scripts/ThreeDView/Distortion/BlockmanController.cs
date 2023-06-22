using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.ThreeDView.Distortion
{
    public class BlockmanController : MonoBehaviour
    {
        [SerializeField]
        private float _rotationRatio = 1f;
        
        [SerializeField]
        private Transform _rootBone = default;

        [SerializeField]
        private SkinnedMeshRenderer _meshRenderer = default;
        public SkinnedMeshRenderer meshRenderer {
            get {
                return _meshRenderer;
            }
        }

        private AvatarController _avatar;
        

        void OnValidate()
        {
            if (_avatar != null)
            {
                ApplyBoneRotation(_avatar);
            }
        }

        void Start()
        {
        }

        void Update()
        {
        }

        public void OnAvatarUpdated(AvatarController avatar)
        {
            _avatar = avatar;
            ApplyBoneRotation(avatar);
        }

        private void ApplyBoneRotation(AvatarController avatar)
        {
            var children = _rootBone.GetComponentsInChildren<Transform>();
            var bones = Enum.GetValues(typeof(AvatarBones));

            foreach (AvatarBones b in bones)
            {
                switch (b)
                {
                    case AvatarBones.SpineWaist:
                        // ブロックマンはboneのSpineをSpineとWaist2つに分割して反映.
                        {
                            var spine = children.Where(x => x.name == b.GetName()).FirstOrDefault();
                            var waist = children.Where(x => x.name == "waist").FirstOrDefault();
                            var rotation = avatar.GetRotation(b);
                            if (spine != null && waist != null && rotation != null)
                            {
                                var r = Quaternion.LerpUnclamped(Quaternion.identity, ((Quaternion)rotation), 0.5f * _rotationRatio);
                                spine.rotation = r;
                                waist.rotation = r;
                            }
                        }
                        break;
                    case AvatarBones.LeftArm:
                    case AvatarBones.RightArm:
                        {
                            var child = children.Where(x => x.name == b.GetName()).FirstOrDefault();
                            var rotation = avatar.GetRotation(b);
                            if (child != null && rotation != null)
                            {
                                child.rotation = ((Quaternion)rotation);
                            }
                        }
                        break;
                    default:
                        {
                            var child = children.Where(x => x.name == b.GetName()).FirstOrDefault();
                            var rotation = avatar.GetRotation(b);
                            if (child != null && rotation != null)
                            {
                                child.rotation = Quaternion.LerpUnclamped(Quaternion.identity, ((Quaternion)rotation), _rotationRatio);
                            }
                        }
                        break;
                }
            }
        }
    }
}
