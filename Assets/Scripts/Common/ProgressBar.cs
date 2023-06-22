using Assets.Scripts.Loading;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common
{
    [RequireComponent(typeof(Image))]
    public class ProgressBar : MonoBehaviour
    {
        private Image _fillImage;

        private Dictionary<ProgressAdapter, int> _progressValues = new Dictionary<ProgressAdapter, int>();


        void Awake()
        {
            _fillImage = GetComponent<Image>();
            
            var adapterList = LoadingSceneManager.Instance.ProgressAdapterList;
            foreach (var adapter in adapterList)
            {
                adapter.OnProgressChanged.AddListener(OnProgressChanged);
                _progressValues[adapter] = 0;
            }
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void OnProgressChanged(int progress)
        {
            _fillImage.fillAmount = progress / 100f;
        }

        public void OnProgressChanged(ProgressAdapter adapter, int progress)
        {
            _progressValues[adapter] = progress;
            _fillImage.fillAmount = TotalProgress / 100f;
        }

        public int TotalProgress {
            get {
                var total = 0.0f;
                var num = _progressValues.Count;
                foreach (var pair in _progressValues)
                {
                    total += (float)pair.Value / num;
                }
                return (int)total;
            }
        }
    }
}