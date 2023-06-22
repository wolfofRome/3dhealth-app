using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Common
{
    public class ProgressAdapter : MonoBehaviour
    {
        private int _minProgress = 0;
        public int MinProgress {
            get {
                return _minProgress;
            }
            set {
                _minProgress = value;
            }
        }

        private int _maxProgress = 100;
        public int MaxProgress {
            get {
                return _maxProgress;
            }
            set {
                _maxProgress = value;
            }
        }

        public class ProgressAdapterEvent : UnityEvent<ProgressAdapter, int> { }
        public ProgressAdapterEvent OnProgressChanged = new ProgressAdapterEvent();
        public ProgressAdapterEvent OnProgressUpdateCompleted = new ProgressAdapterEvent();

        private int _progress;
        protected int Progress {
            get {
                return _progress;
            }
            set {
                _progress = value;
                OnProgressChanged.Invoke(this, _progress);
            }
        }

        private int _secondaryProgress = 0;
        public int SecondaryProgress {
            get {
                return _secondaryProgress;
            }
            set {
                _secondaryProgress = value;
            }
        }

        private int _updateFrameNum = 0;
        public int UpdateFrameNum {
            get {
                return _updateFrameNum;
            }
            set {
                _updateFrameNum = value;
            }
        }
        
        void Awake()
        {
            StartCoroutine(UpdateProgress());
        }

        IEnumerator UpdateProgress()
        {
            while (SecondaryProgress == MinProgress)
            {
                yield return null;
            }

            while (Progress < MaxProgress)
            {
                var updateUnit = 1;
                if (UpdateFrameNum != 0)
                {
                    updateUnit = (int)((float)(SecondaryProgress - Progress) / UpdateFrameNum + 0.5f);
                }
                while (Progress < SecondaryProgress)
                {
                    Progress += updateUnit;
                    yield return null;
                }
                yield return null;
            }

            OnProgressUpdateCompleted.Invoke(this, _progress);
        }
    }
}