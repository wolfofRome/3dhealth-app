using Assets.Scripts.Common;
using Assets.Scripts.Database.DataRow;
using Assets.Scripts.Loading;
using Assets.Scripts.Network.Response;
using UnityEngine;

namespace Assets.Scripts.Compare
{
    public class LeftContentsLoader : BaseContentsLoader
    {
        protected override void Awake()
        {
            LoadingSceneManager.Instance.AddProgressAdapter(ProgressAdapter);
            LoadingSceneManager.Instance.Show();

            var args = SceneLoader.Instance.GetArgment<CompareLoadParam.Argment>(gameObject.scene.name);
            StartLoading(args.leftContents);
        }

        protected override void FinishLoading(Contents contents)
        {
            base.FinishLoading(contents);
            LoadingSceneManager.Instance.Hide();

            Light fillLight = GameObject.Find("LeftObjLigths/FillLight").GetComponent<Light>();
            Light mainLight = GameObject.Find("LeftObjLigths/MainLight").GetComponent<Light>();

            switch (measurementDataLoader.Measurement.Type)
            {
                case Measurement.TypeSpaceVision:
                    fillLight.intensity = 1.3f;
                    mainLight.intensity = 1.47f;
                    break;
                case Measurement.Type3DBodyLab:
                    fillLight.intensity = 0.1f;
                    mainLight.intensity = 0.1f;
                    break;
            }
        }
    }
}