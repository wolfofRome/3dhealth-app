using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Common.Graphics.UI.ListView
{
    public class ListAdapter<PrefabType, DataType> : BaseAdapter where PrefabType : BaseListItem<DataType>
    {
        [SerializeField]
        private PrefabType _prefab = default;

        private List<DataType> _list = new List<DataType>();
        public List<DataType> list {
            get {
                return _list;
            }
        }

        public override int GetCount()
        {
            return _list.Count;
        }

        public override object GetItem(int position)
        {
            return _list[position];
        }

        public override GameObject InstantiateItem(int position)
        {
            return Instantiate(_prefab).gameObject;
        }

        public override void Bind(GameObject item, int position)
        {
            item.GetComponent<PrefabType>().Bind(_list[position]);
        }
    }

    public abstract class BaseAdapter : MonoBehaviour
    {
        private UnityEvent _onNotifyDataSetChanged = new UnityEvent();
        public UnityEvent OnNotifyDataSetChanged {
            get {
                return _onNotifyDataSetChanged;
            }
        }

        protected virtual void Awake() { }
        protected virtual void Start() { }
        protected virtual void Update() { }
        
        public abstract int GetCount();
        public abstract object GetItem(int position);
        public abstract GameObject InstantiateItem(int position);
        public abstract void Bind(GameObject item, int position);

        public virtual void NotifyDataSetChanged()
        {
            OnNotifyDataSetChanged.Invoke();
        }
    }
}