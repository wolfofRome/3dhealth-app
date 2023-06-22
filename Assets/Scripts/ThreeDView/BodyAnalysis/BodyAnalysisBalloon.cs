using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Common;

namespace Assets.Scripts.ThreeDView.BodyAnalysis {
    public class BodyAnalysisBalloon : MonoBehaviour {
        [SerializeField]
        private Image _background = default;
        [SerializeField]
        private Text _percentageValue = default;
        [SerializeField]
        private Text _diffValue = default;
        [SerializeField]
        private Image _fixedLine = default;
        [SerializeField]
        private Image _joint = default;
        [SerializeField]
        private Image _variableLine = default;
        [SerializeField]
        private Image _circle = default;
        [SerializeField]
        private Vector3 _lineAngleOffset = default;

        public void SetBalloonValue(double percentageValue, double diffValue, Color color) {
            float percentageValueClamp = Mathf.Clamp((float)percentageValue, -100, 100);
            float percentage = (float)Math.Round(percentageValueClamp, 1, MidpointRounding.AwayFromZero);
            float percentageAbs = Mathf.Abs(percentage);
            string percentageValueText = "";
            switch (PlayerPrefs.GetString("Lang")) {
                case "Japanese":
                    if (percentageAbs < float.Epsilon) {
                        percentageValueText = "±0";
                    } else if (percentage < 0) {
                        percentageValueText = string.Format("{0:0.0}% 細め", percentageAbs);
                    } else { percentageValueText = string.Format("{0:0.0}% 太め", percentageAbs); }
                    break;
                case "English":
                    if (percentageAbs < float.Epsilon) {
                        percentageValueText = "±0";
                    } else if (percentage < 0) {
                        percentageValueText = string.Format("{0:0.0}% Slender", percentageAbs);
                    } else { percentageValueText = string.Format("{0:0.0}% Thick", percentageAbs); }
                    break;
                case "Chinese":
                    if (percentageAbs < float.Epsilon) {
                        percentageValueText = "±零";
                    } else if (percentage < 0) {
                        percentageValueText = string.Format("{0:0.0}% 瘦的", percentageAbs);
                    } else { percentageValueText = string.Format("{0:0.0}% 肥胖", percentageAbs); }
                    break;
            }

            float diffValueClamp = Mathf.Clamp((float)diffValue, -100, 100);
            float diff = (float)Math.Round(diffValueClamp, 1, MidpointRounding.AwayFromZero);
            float diffAbs = Mathf.Abs(diff);
            string diffValueText = "";
            switch (PlayerPrefs.GetString("Lang")) {
                case "Japanese":
                    if (diffAbs < float.Epsilon) {
                        diffValueText = "±0";
                    } else if (diff < 0) {
                        diffValueText = string.Format("{0:0.0}cm アンダー", diffAbs);
                    } else { diffValueText = string.Format("{0:0.0}cm オーバー", diffAbs); }
                    break;
                case "English":
                    if (diffAbs < float.Epsilon) {
                        diffValueText = "±0";
                    } else if (diff < 0) {
                        diffValueText = string.Format("{0:0.0}cm UNDER", diffAbs);
                    } else { diffValueText = string.Format("{0:0.0}cm OVER", diffAbs); }
                    break;
                case "Chinese":
                    if (diffAbs < float.Epsilon) {
                        diffValueText = "±零";
                    } else if (diff < 0) {
                        diffValueText = string.Format("{0:0.0}厘米 下側", diffAbs);
                    } else { diffValueText = string.Format("{0:0.0}厘米 超过", diffAbs); }
                    break;
            }

            _background.color = color;
            _fixedLine.color = color;
            _joint.color = color;
            _variableLine.color = color;
            _circle.color = color;
            _percentageValue.text = percentageValueText;
            _percentageValue.color = color;
            _diffValue.text = diffValueText;
            _diffValue.color = color;
        }

        public void UpdateLine(Vector3 position) {
            UpdateLine(new List<Vector3>() { position });
        }

        public void UpdateLine(List<Vector3> positions) {
            if (positions.Count == 0) {
                return;
            }

            // 線の開始位置(吹き出し側、jointの位置)
            Vector2 startPosition = _joint.rectTransform.localPosition;

            // 線の終了位置(マシュマロマン側)
            Vector2 endPosition = Vector2.zero;

            // 2点の距離
            float distance = float.MaxValue;

            // 引数で渡された位置の中で一番近い位置とその距離を取得
            foreach (Vector3 position in positions) {
                Vector2 point = transform.InverseTransformPoint(Camera.main.WorldToScreenPoint(position));
                float magnitude = (startPosition - point).magnitude;
                if (magnitude < distance) {
                    endPosition = point;
                    distance = magnitude;
                }
            }

            // 2点の角度
            Quaternion angle = Quaternion.Euler(new Vector3(0, 0, GetAim(startPosition, endPosition)) + _lineAngleOffset);

            // 2点を結ぶ線を調整
            _variableLine.rectTransform.localPosition = startPosition;
            _variableLine.rectTransform.localRotation = angle;
            _variableLine.rectTransform.localScale = _fixedLine.rectTransform.localScale;
            _variableLine.rectTransform.sizeDelta = new Vector2(distance, _fixedLine.rectTransform.sizeDelta.y);

            // 線の終了位置(マシュマロマン側)に円オブジェクトを移動
            _circle.rectTransform.localPosition = endPosition;
        }

        private float GetAim(Vector2 p1, Vector2 p2) {
            float dx = p2.x - p1.x;
            float dy = p2.y - p1.y;
            float rad = Mathf.Atan2(dy, dx);
            return rad * Mathf.Rad2Deg;
        }
    }
}
