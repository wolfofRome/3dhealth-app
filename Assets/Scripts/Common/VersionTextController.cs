using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common
{
    [RequireComponent(typeof(Text))]
    public class VersionTextController : MonoBehaviour
    {
        private Text _versionText;
        private Text VersionText {
            get {
                return _versionText = _versionText ?? GetComponent<Text>();
            }
        }
        
        void Awake( )
        {
            VersionText.text = Application.version;
        }
    }
}