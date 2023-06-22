using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public class LoadingPanelController : MonoBehaviour
    {
        [SerializeField]
        private ProgressBar _progressBar = default;

        void Start ()
        {
            StartCoroutine(UpdateProgress());
        }

        IEnumerator UpdateProgress()
        {
            while (_progressBar.TotalProgress < 100)
            {
                yield return null;
            }
            gameObject.SetActive(false);
        }
    }
}
