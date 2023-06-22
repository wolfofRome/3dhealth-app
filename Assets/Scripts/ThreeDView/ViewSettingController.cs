using Assets.Scripts.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ThreeDView
{
    public class ViewSettingController : MonoBehaviour
    {
        public AlphaControllMenu AlphaControllMenu { get; set; }

        [SerializeField]
        private Toggle _fatWireModeToggle = default;
        [SerializeField]
        private Toggle _flatModeToggle = default;
        [SerializeField]
        private Toggle _gridModeToggle = default;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void LoadSettings()
        {
            if (UserData.HasViewSetting)
            {
                AlphaControllMenu.PhysiqueAlphaButton.SetMeshState((MeshState)UserData.PhysiqueAlpha);
                AlphaControllMenu.SkeletonAlphaButton.SetMeshState((MeshState)UserData.SkeletonAlpha);
                AlphaControllMenu.MuscleAlphaButton.SetMeshState((MeshState)UserData.MuscleAlpha);
                AlphaControllMenu.FatAlphaButton.SetMeshState((MeshState)UserData.FatAlpha);
                _fatWireModeToggle.isOn = UserData.FatWireMode;
                _flatModeToggle.isOn = UserData.FlatMode;
                _gridModeToggle.isOn = UserData.GridMode;
            }
        }

        public void SaveSettings()
        {
            UserData.HasViewSetting = true;
            UserData.PhysiqueAlpha = (int)AlphaControllMenu.PhysiqueAlphaButton.State;
            UserData.SkeletonAlpha = (int)AlphaControllMenu.SkeletonAlphaButton.State;
            UserData.MuscleAlpha = (int)AlphaControllMenu.MuscleAlphaButton.State;
            UserData.FatAlpha = (int)AlphaControllMenu.FatAlphaButton.State;
            UserData.FatWireMode = _fatWireModeToggle.isOn;
            UserData.FlatMode = _flatModeToggle.isOn;
            UserData.GridMode = _gridModeToggle.isOn;
            UserData.Save();
        }
    }
}