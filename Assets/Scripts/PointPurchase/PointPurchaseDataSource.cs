using Assets.Scripts.Common;
using Assets.Scripts.Network;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.PointPurchase
{
    public class PointPurchaseDataSource : ScriptableObject
    {
        [Serializable]
        public class DataSourceUpdateEvent : UnityEvent<int> { }

        [SerializeField]
        private DataSourceUpdateEvent _onUpdate = new DataSourceUpdateEvent();
        public DataSourceUpdateEvent onUpdate
        {
            get
            {
                return _onUpdate;
            }
        }
        
        private int _pointBalance;
        public int pointBalance
        {
            get
            {
                return _pointBalance;
            }
            set
            {
                _pointBalance = Math.Max(value, 0);
                onUpdate.Invoke(value);
            }
        }

        public void AddPoint(int value)
        {
            pointBalance += value;
        }

        public IEnumerator UpdateDataSource()
        {
            yield return ApiHelper.Instance.GetPoint(response =>
            {
                if (response.ErrorCode != null)
                {
                    DebugUtil.LogError("GetPoint -> " + response.ErrorCode + " : " + response.ErrorMessage);
                    return;
                }
                pointBalance = response.Point;
            });
        }
    }
}
