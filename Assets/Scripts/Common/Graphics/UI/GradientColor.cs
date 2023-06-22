using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Assets.Scripts.Graphics.UI
{
    [AddComponentMenu("UI/Effects/Gradient Color"), RequireComponent(typeof(Graphic))]
    public class GradientColor : BaseMeshEffect
    {
        public enum DIRECTION
        {
            Vertical,
            Horizontal,
        }

        [SerializeField]
        private DIRECTION _direction = DIRECTION.Horizontal;
        [SerializeField]
        private Color startColor = Color.clear;
        [SerializeField]
        private Color endColor = Color.white;


        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
            {
                return;
            }
            List<UIVertex> vList = new List<UIVertex>();
            vh.GetUIVertexStream(vList);

            ModifyVertices(vList);

            vh.Clear();
            vh.AddUIVertexTriangleStream(vList);
        }

        public void ModifyVertices(List<UIVertex> vList)
        {
            if (!IsActive() || vList == null || vList.Count == 0)
            {
                return;
            }
            float topX = 0f;
            float topY = 0f;
            float bottomX = 0f;
            float bottomY = 0f;
            for (int i = 0; i < vList.Count; i++)
            {
                UIVertex vertex = vList[i];
                topX = Mathf.Max(topX, vertex.position.x);
                topY = Mathf.Max(topY, vertex.position.y);
                bottomX = Mathf.Min(bottomX, vertex.position.x);
                bottomY = Mathf.Min(bottomY, vertex.position.y);
            }

            float width = topX - bottomX;
            float height = topY - bottomY;

            UIVertex tempVertex = vList[0];
            for (int i = 0; i < vList.Count; i++)
            {
                tempVertex = vList[i];
                byte orgAlpha = tempVertex.color.a;
                Color colorOrg = tempVertex.color;
                float time;
                switch (_direction)
                {
                    default:
                    case DIRECTION.Vertical:
                        time = (tempVertex.position.y - bottomY) / height;
                        break;
                    case DIRECTION.Horizontal:
                        time = (tempVertex.position.x - bottomX) / width;
                        break;
                }
                Color colorGr = Color.Lerp(startColor, endColor, time);
                tempVertex.color = BlendColor(colorOrg, colorGr);
                tempVertex.color.a = orgAlpha;
                vList[i] = tempVertex;
            }
        }

        private Color BlendColor(Color src1, Color src2)
        {
            return src1 * (1 - src2.a) + src2 * src2.a;
        }
    }
}