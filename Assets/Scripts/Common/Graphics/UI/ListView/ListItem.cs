using UnityEngine;

namespace Assets.Scripts.Common.Graphics.UI.ListView
{
    public abstract class BaseListItem<T> : MonoBehaviour
    {
        protected virtual void Awake() {}
        protected virtual void Start() {}
        protected virtual void Update() {}

        public abstract void Bind(T param);
    }
}
