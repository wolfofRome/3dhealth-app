using Assets.Scripts.Common.Config;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.ThreeDView.Distortion
{
    [RequireComponent(typeof(BoxCollider))]
    public class BlockmanPartController : MonoBehaviour
    {
        [SerializeField]
        private BlockmanPart _part = default;
        public BlockmanPart part
        {
            get
            {
                return _part;
            }
        }

        private BoxCollider _boxCollider;
        public BoxCollider boxCollider
        {
            get
            {
                return _boxCollider = _boxCollider ?? GetComponent<BoxCollider>();
            }
        }
    }
}
