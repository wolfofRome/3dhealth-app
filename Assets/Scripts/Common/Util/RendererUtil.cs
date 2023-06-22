using UnityEngine;

namespace Assets.Scripts.Common.Util
{
    public static class RendererUtil
    {
        public static Bounds GetEncapsulateBounds(UnityEngine.Renderer[] renderers)
        {
            var bounds = new Bounds(Vector3.zero, Vector3.zero);
            foreach (var r in renderers)
            {
                if (bounds.size == Vector3.zero)
                {
                    bounds = new Bounds(r.bounds.center, r.bounds.size);
                }
                else
                {
                    bounds.Encapsulate(r.bounds);
                }
            }
            return bounds;
        }
    }
}
