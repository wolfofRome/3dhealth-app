using Assets.Scripts.Common;
using Assets.Scripts.Network.Response;
using UnityEngine;

namespace Assets.Scripts.ThreeDView
{
    public class ThreeDModeDataSource : ScriptableObject
    {
        [SerializeField]
        private Contents _contents = default;
        public Contents contents
        {
            get
            {
                return _contents;
            }
            set
            {
                _contents = value;
            }
        }

        [SerializeField]
        private Mode _mode = Mode.ThreeDView;
        public Mode mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
            }
        }
    }
}
