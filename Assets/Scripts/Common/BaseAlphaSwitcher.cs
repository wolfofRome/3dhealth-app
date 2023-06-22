using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Common
{
    public enum MeshState
    {
        AlphaZero,
        AlphaZeroPointFive,
        AlphaOne
    };

    public abstract class BaseAlphaSwitcher : MonoBehaviour
    {
        [Serializable]
        public class AlphaChangeEvent : UnityEvent<float> { }
        public AlphaChangeEvent OnAlphaChanged;

        [SerializeField]
        protected MeshState _mobileDefaultState = MeshState.AlphaOne;
        [SerializeField]
        protected MeshState _webGLDefaultState = MeshState.AlphaOne;

        protected MeshState _state;
        public MeshState State {
            get {
                return _state;
            }
            set {
                _state = value;
            }
        }

        protected readonly float[] _alphaTable = new float[] { 0f, 0.5f, 1f };

        protected virtual void Awake ()
        {
#if UNITY_WEBGL
            _state = _webGLDefaultState;
#else
            _state = _mobileDefaultState;
#endif
        }

        // Use this for initialization
        protected virtual void Start()
        {
            SetMeshState(_state);
        }

        // Update is called once per frame
        protected virtual void Update()
        {
        }

        public void SetMeshState(MeshState state)
        {
            _state = state;
            ChangeButtonState(_state);
            OnAlphaChanged.Invoke(_alphaTable[(int)_state]);
        }

        protected abstract void ChangeButtonState(MeshState state);
    }
}