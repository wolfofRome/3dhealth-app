using Assets.Scripts.PopUpConfirm;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Common
{
    [RequireComponent(typeof(PopUpConfirmLoadParam))]
    [RequireComponent(typeof(Button))]
    public class PointConsumptionButton : MonoBehaviour
    {
        [Serializable]
        public class FinishPointConsumptionEvent : UnityEvent {}
        [SerializeField]
        private FinishPointConsumptionEvent _onClickPointConsumption = new FinishPointConsumptionEvent();
        public FinishPointConsumptionEvent onClickPointConsumption
        {
            get
            {
                return _onClickPointConsumption;
            }
        }

        [Serializable]
        public class CancelPointConsumptionEvent : UnityEvent {}
        [SerializeField]
        private CancelPointConsumptionEvent _onCancelPointConsumption = new CancelPointConsumptionEvent();
        public CancelPointConsumptionEvent onCancelPointConsumption
        {
            get
            {
                return _onCancelPointConsumption;
            }
        }

        private Button _button;
        private Button button
        {
            get
            {
                return _button = _button ?? GetComponent<Button>();
            }
        }

        private PopUpConfirmLoadParam _popUpConfirmLoadParam;
        public PopUpConfirmLoadParam popUpConfirmLoadParam
        {
            get
            {
                return _popUpConfirmLoadParam = _popUpConfirmLoadParam ?? GetComponent<PopUpConfirmLoadParam>();
            }
        }

        protected virtual void Start()
        {
            popUpConfirmLoadParam.argment.positiveClickAction.AddListener(() =>
            {
                onClickPointConsumption.Invoke();
            });

            popUpConfirmLoadParam.argment.negativeClickAction.AddListener(() =>
            {
                onCancelPointConsumption.Invoke();
            });

            button.onClick.AddListener(() =>
            {
                SceneLoader.Instance.LoadSceneWithParams(popUpConfirmLoadParam);
            });
        }
    }
}