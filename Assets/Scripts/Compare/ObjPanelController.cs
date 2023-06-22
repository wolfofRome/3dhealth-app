using Assets.Scripts.Network.Response;
using Assets.Scripts.TalentList;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Assets.Scripts.Common.Config;

namespace Assets.Scripts.Compare
{
    public class ObjPanelController : MonoBehaviour
    {
        [SerializeField]
        private Text _objScanDateTime = default;

        [SerializeField]
        private TalentDataSource _talentDataSource = default;
        private TalentDataSource talentDataSource {
            get {
                return _talentDataSource;
            }
        }

        void Start()
        {
        }

        void Update()
        {
        }

        public void OnLoadStart(Contents contents)
        {
            if (contents.isTalent)
            {
                var talentoInfo = talentDataSource.talentoInfoList.Where(t => t.ResourceId == contents.ResourceId).FirstOrDefault();
                _objScanDateTime.text = talentoInfo.CommonName;
            }
            else
            {
                _objScanDateTime.text = contents.CreateTimeAsDateTime.ToString(AppConst.DateTimeFormat);
            }
        }
    }
}