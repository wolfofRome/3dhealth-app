using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Graphics.UI
{
    public class TitleBar : MonoBehaviour
    {
        [SerializeField]
        private Text _title = default;
        public Text title {
            get {
                return _title;
            }
        }

        [SerializeField]
        private Text _summary = default;
        public Text summary {
            get {
                return _summary;
            }
        }

        [SerializeField]
        private string _titleText = default;

        [SerializeField]
        private string _summaryText = default;

        private void OnValidate()
        {
            if (title != null) title.text = _titleText;
            if (summary != null) summary.text = _summaryText;
        }
    }
}
