using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ThreeDView.BodyAnalysis
{
    public class BodyAnalysisDetailPanel : MonoBehaviour
    {
        [SerializeField]
        private Text _waistValue = default;
        [SerializeField]
        private Text _waistIdealValue = default;
        [SerializeField]
        private Text _hipValue = default;
        [SerializeField]
        private Text _hipIdealValue = default;
        [SerializeField]
        private Text _thighsValue = default;
        [SerializeField]
        private Text _thighsIdealValue = default;

        private double _waist;
        public double waist
        {
            get
            {
                return _waist;
            }
            set
            {
                _waist = value;
                SetValue(_waistValue, value);
            }
        }

        private double _waistIdeal;
        public double waistIdeal
        {
            get
            {
                return _waistIdeal;
            }
            set
            {
                _waistIdeal = value;
                SetValue(_waistIdealValue, value);
            }
        }

        private double _hip;
        public double hip
        {
            get
            {
                return _hip;
            }
            set
            {
                _hip = value;
                SetValue(_hipValue, value);
            }
        }

        private double _hipIdeal;
        public double hipIdeal
        {
            get
            {
                return _hipIdeal;
            }
            set
            {
                _hipIdeal = value;
                SetValue(_hipIdealValue, value);
            }
        }

        private double _thighs;
        public double thighs
        {
            get
            {
                return _thighs;
            }
            set
            {
                _thighs = value;
                SetValue(_thighsValue, value);
            }
        }

        private double _thighsIdeal;
        public double thighsIdeal
        {
            get
            {
                return _thighsIdeal;
            }
            set
            {
                _thighsIdeal = value;
                SetValue(_thighsIdealValue, value);
            }
        }
        
        private void SetValue(Text text, double value)
        {
            if (text != null)
            {
                double valueRound = Math.Round(value, 1, MidpointRounding.AwayFromZero);
                text.text = string.Format("{0:0.0}cm", valueRound);
            }
        }
    }
}
