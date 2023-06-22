using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Scripts.Common.Config;
using Assets.Scripts.Graphics.UI;
using Assets.Scripts.Loading;
using Assets.Scripts.Network.Response;
using Assets.Scripts.TalentList;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.Scripts.TalentDetail
{
    public class ContentsLoader : BaseContentsLoader
    {
        [SerializeField, FormerlySerializedAs("_titleBar")]
        private TitleBar titleBar;
        
        [SerializeField, FormerlySerializedAs("_purchasedActivateObjectList")]
        private List<GameObject> purchasedActivateObjectList;

        [SerializeField, FormerlySerializedAs("_unpurchasedActivateObjectList")]
        private List<GameObject> unpurchasedActivateObjectList;

        // 有名人データはディスクキャッシュを無効化.
        protected override bool useObjDiskCache => false;

        protected override void Awake()
        {
            base.Awake();
            LoadingSceneManager.Instance.AddProgressAdapter(ProgressAdapter);
            LoadingSceneManager.Instance.Show();
        }

        protected override void Start()
        {
            base.Start();
            var args = SceneLoader.Instance.GetArgment<TalentDetailLoadParam.Argment>(gameObject.scene.name);
            
            titleBar.title.text = args.talentInfo.CommonName;
            titleBar.summary.text = args.talentInfo.CreateTimeAsDateTime.ToString(AppConst.DateFormat) + " " + args.talentInfo.Comment;
            
            StartLoading(args.talentInfo);

            var purchased = args.talentInfo.purchaseStatus == PurchaseStatus.Purchased;
            purchasedActivateObjectList.ForEach(obj => { obj.SetActive(purchased); });
            unpurchasedActivateObjectList.ForEach(obj => { obj.SetActive(!purchased); });
        }
        
        protected override void FinishLoading(Contents contents)
        {
            base.FinishLoading(contents);            
            LoadingSceneManager.Instance.Hide();
        }
    }
}