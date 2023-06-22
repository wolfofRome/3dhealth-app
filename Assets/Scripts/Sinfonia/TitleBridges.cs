using UnityEngine;
using UnityEngine.UI;

namespace Sinfonia {
    public class TitleBridges : MonoBehaviour {
        public Text[]
            titleTexts = new Text[5];

        public static string[]
            _titleTexts = new string[5];

        private void Update() {
            for (int i = 0; i < titleTexts.Length; i++) {
                _titleTexts[i] = titleTexts[i].text;
            }
        }

        public static string GetTitle(int i) {
            return _titleTexts[i];
        }
    }
}
