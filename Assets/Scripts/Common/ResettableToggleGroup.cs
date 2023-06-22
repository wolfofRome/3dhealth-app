using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace Assets.Scripts.Common
{
    public class ResettableToggleGroup : ToggleGroup
    {
        private List<Toggle> _defaultActiveToggleList;

        protected override void Start()
        {
            base.Start();
            
            _defaultActiveToggleList = ActiveToggles().ToList();
        }

        public void ResetValue()
        {
            SetAllTogglesOff();
            foreach (var t in _defaultActiveToggleList)
            {
                t.isOn = true;
            }
        }
    }
}
