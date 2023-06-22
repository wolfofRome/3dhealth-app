using UnityEngine;

namespace Assets.Scripts.Common.Graphics.UI.ViewPager
{
    public class PageFrame : BasePageFrame
    {
        public override GameObject Instantiate(int pagePosition, Vector2 anchorMin, Vector2 anchorMax)
        {
            var rt = GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = new Vector2(0, 0);
            rt.offsetMax = new Vector2(0, 0);

            return gameObject;
        }

        public override void Bind(int pagePosition, Vector2 anchorMin, Vector2 anchorMax)
        {
        }

        public override void UnBind(int pagePosition)
        {
        }

        public override void OnGotFocus()
        {
        }

        public override void OnLostFocus()
        {
        }
    }
    
    public abstract class BasePageFrame : MonoBehaviour
    {
        /// <summary>
        /// ページ生成イベント.
        /// </summary>
        /// <param name="pagePosition"></param>
        /// <param name="anchorMin"></param>
        /// <param name="anchorMax"></param>
        /// <returns></returns>
        public abstract GameObject Instantiate(int pagePosition, Vector2 anchorMin, Vector2 anchorMax);

        /// <summary>
        /// ページバインドイベント.
        /// </summary>
        /// <param name="pagePosition"></param>
        /// <param name="anchorMin"></param>
        /// <param name="anchorMax"></param>
        public abstract void Bind(int pagePosition, Vector2 anchorMin, Vector2 anchorMax);

        /// <summary>
        /// ページアンバインドイベント.
        /// </summary>
        public abstract void UnBind(int pagePosition);

        /// <summary>
        /// フォーカス取得イベント.
        /// </summary>
        public abstract void OnGotFocus();

        /// <summary>
        /// フォーカス消失イベント.
        /// </summary>
        public abstract void OnLostFocus();
    }
}
