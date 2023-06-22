using UnityEngine.UI;

namespace Assets.Scripts.Graphics.UI
{
    public class GraphicCast : Graphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            vh.Clear();
        }
    }
}