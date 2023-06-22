using UnityEngine;
using Assets.Scripts.ThreeDView;
using Assets.Scripts.Common;

namespace Assets.Scripts.Archive
{
    [RequireComponent(typeof(ThreeDViewLoadParam))]
    public class UserScanDataListItem : BaseScanDataListItem
    {
        private BaseSceneLoadParam _sceneLoadParam;
        public override BaseSceneLoadParam sceneLoadParam
        {
            get
            {
                return _sceneLoadParam = _sceneLoadParam ?? GetComponent<ThreeDViewLoadParam>();
            }
        }

        public override void Bind(BindParam param)
        {
            base.Bind(param);
            ((ThreeDViewLoadParam)sceneLoadParam).argment.resourceId = param.contents.ResourceId;
        }
    }
}