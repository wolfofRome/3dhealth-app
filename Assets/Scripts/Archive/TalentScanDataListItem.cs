using UnityEngine;
using Assets.Scripts.TalentDetail;
using Assets.Scripts.Common;
using Assets.Scripts.Network.Response;

namespace Assets.Scripts.Archive
{
    [RequireComponent(typeof(TalentDetailLoadParam))]
    public class TalentScanDataListItem : BaseScanDataListItem
    {
        private BaseSceneLoadParam _sceneLoadParam;
        public override BaseSceneLoadParam sceneLoadParam
        {
            get
            {
                return _sceneLoadParam = _sceneLoadParam ?? GetComponent<TalentDetailLoadParam>();
            }
        }

        public override void Bind(BindParam param)
        {
            base.Bind(param);
            ((TalentDetailLoadParam)sceneLoadParam).argment.talentInfo = (TalentoContents)param.contents;
        }
    }
}