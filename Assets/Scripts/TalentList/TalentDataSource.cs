using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Assets.Scripts.Network.Response;
using System.Collections;
using Assets.Scripts.Network;
using Assets.Scripts.Common;

namespace Assets.Scripts.TalentList
{
    public enum PurchaseStatus : int
    {
        Unpurchased = 0,
        Purchased = 1,
    }

    public static class PurchaseStatusExtension
    {
        private static readonly Dictionary<PurchaseStatus, string> _purchaseStatusTable = new Dictionary<PurchaseStatus, string>()
        {
            {PurchaseStatus.Unpurchased, "non-purchased" },
            {PurchaseStatus.Purchased, "purchased" },
        };

        public static string ToAppString(this PurchaseStatus status)
        {
            return _purchaseStatusTable[status];
        }

        public static PurchaseStatus Parse(string status)
        {
            return _purchaseStatusTable.First(x => x.Value == status).Key;
        }
    }

    public class TalentDataSource : ScriptableObject
    {
        [Serializable]
        public class DataSourceUpdateEvent : UnityEvent<TalentDataSource> { }

        [SerializeField]
        private DataSourceUpdateEvent _onUpdate = new DataSourceUpdateEvent();
        public DataSourceUpdateEvent onUpdate
        {
            get
            {
                return _onUpdate;
            }
        }
        
        private List<TalentoContents> _talentoInfoList;
        public List<TalentoContents> talentoInfoList
        {
            get
            {
                return _talentoInfoList;
            }
            private set
            {
                _talentoInfoList = value;
                onUpdate.Invoke(this);
            }
        }

        public Dictionary<int, TalentoContents> talentoInfoMap
        {
            get
            {
                return talentoInfoList.ToDictionary(info => info.TalentResourceId);
            }
        }

        public IEnumerator UpdateDataSource()
        {
            yield return ApiHelper.Instance.GetTalentList(null, res =>
            {
                if (res.ErrorCode != null)
                {
                    DebugUtil.LogError("GetTalentList Error -> " + res.ErrorCode + " : " + res.ErrorMessage);
                    return;
                }
                talentoInfoList = res.ContentsList.OrderBy(c => c.Order).ToList();
            });
        }
    }
}
