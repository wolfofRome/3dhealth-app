using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public class RenderTextureUpdater : MonoBehaviour
    {
        [SerializeField]
        private List<RenderTextureController> _targetList = new List<RenderTextureController>();
        protected List<RenderTextureController> targetList {
            get {
                return _targetList;
            }
        }

        public void AddItem(RenderTextureController item)
        {
            targetList.Add(item);
        }

        public void RemoveItem(RenderTextureController item)
        {
            targetList.Remove(item);
        }

        public void RequestUpdate()
        {
            foreach(var target in targetList)
            {
                target.UpdateRenderTexture();
            }
        }
    }
}
