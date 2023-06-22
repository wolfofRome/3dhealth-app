using Assets.Scripts.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ThreeDView
{
    [RequireComponent(typeof(ToggleGroup))]
    public class TrialModeMenuController : ButtonGroupController
    {
        protected override void Start()
        {
            base.Start();
            if (!DataManager.Instance.isTrialAccount)
            {
                buttonList.ForEach(button => {
                    button.gameObject.SetActive(false);
                });
            }
        }

        public override void Show()
        {
            if (!isShown)
            {
                base.Show();
            }
        }

        public override void Hide()
        {
            if (isShown)
            {
                base.Hide();
            }
        }
    }
}