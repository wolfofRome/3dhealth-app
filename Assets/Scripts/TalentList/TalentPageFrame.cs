using UnityEngine;
using Assets.Scripts.Common.Graphics.UI.ViewPager;
using System.Collections.Generic;
using Assets.Scripts.Network.Response;

namespace Assets.Scripts.TalentList
{
    public class TalentPageFrame : PageFrame
    {
        [SerializeField]
        private TalentBanner _prefabTalentBanner = default;
        
        private List<TalentoContents> _contentsList;
        public List<TalentoContents> contentsList
        {
            get
            {
                return _contentsList;
            }
            set
            {
                foreach (var contents in value)
                {
                    var banner = Instantiate(_prefabTalentBanner);
                    banner.contents = contents;
                    banner.transform.SetParent(transform, false);
                    _bannerList.Add(banner);
                }
                _contentsList = value;
            }
        }
        
        private List<TalentBanner> _bannerList = new List<TalentBanner>();

        public override GameObject Instantiate(int pagePosition, Vector2 anchorMin, Vector2 anchorMax)
        {
            return base.Instantiate(pagePosition, anchorMin, anchorMax);
        }
    }
}
