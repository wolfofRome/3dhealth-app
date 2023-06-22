using System;
using Assets.Scripts.Common;
using Assets.Scripts.Common.Config;
using Assets.Scripts.Database.DataRow;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.Scripts.ThreeDView.BodyAnalysis
{
    public class BodyAnalysisController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("_marshmallowManController")]
        private MarshmallowManController marshmallowManController;

        public MarshmallowManController marshmallowMan => marshmallowManController;


        [SerializeField, FormerlySerializedAs("_balloonColorDefault")]
        private Color balloonColorDefault = new Color(230f / 255f, 230f / 255f, 230f / 255f, 255f / 255f);

        [SerializeField, FormerlySerializedAs("_balloonColorPhase1")]
        private Color balloonColorPhase1 = new Color(0f / 255f, 203f / 255f, 255f / 255f, 255f / 255f);

        [SerializeField, FormerlySerializedAs("_balloonColorPhase2")]
        private Color balloonColorPhase2 = new Color(255f / 255f, 255f / 255f, 0f / 255f, 255f / 255f);

        [SerializeField, FormerlySerializedAs("_balloonWaist")]
        private BodyAnalysisBalloon balloonWaist;

        [SerializeField, FormerlySerializedAs("_balloonHip")]
        private BodyAnalysisBalloon balloonHip;

        [SerializeField, FormerlySerializedAs("_balloonThighs")]
        private BodyAnalysisBalloon balloonThighs;

        [SerializeField, FormerlySerializedAs("_detailPanel")]
        private BodyAnalysisDetailPanel detailPanel;

        public void OnUpdateMeasurementData(MeasurementDataLoader loader)
        {
            Measurement measurement = loader.Measurement;

            // 性別
            Gender gender = DataManager.Instance.Profile.Gender;

            // 身長
            double height = measurement.correctedHeight * AppConst.MeasurementDataScale;

            // 理想値との比較パーセント最大値
            float maxPercentage = float.MinValue;

            // ウエスト
            detailPanel.waist = measurement.MinWaist * AppConst.MeasurementDataScale;
            detailPanel.waistIdeal = height * (gender == Gender.Male ? 0.43f : 0.38f);
            float waistPercentage = (float) (detailPanel.waist / detailPanel.waistIdeal * 100f) - 100f;
            float waistDiff = (float) (detailPanel.waist - detailPanel.waistIdeal);
            if (waistPercentage > 10)
            {
                balloonWaist.SetBalloonValue(waistPercentage, waistDiff, balloonColorPhase2);
            }
            else if (waistPercentage > 0)
            {
                balloonWaist.SetBalloonValue(waistPercentage, waistDiff, balloonColorPhase1);
            }
            else
            {
                balloonWaist.SetBalloonValue(waistPercentage, waistDiff, balloonColorDefault);
            }

            maxPercentage = Math.Max(maxPercentage, waistPercentage);

            // ヒップ
            detailPanel.hip = measurement.Hip * AppConst.MeasurementDataScale;
            detailPanel.hipIdeal = height * (gender == Gender.Male ? 0.51f : 0.535f);
            float hipPercentage = (float) (detailPanel.hip / detailPanel.hipIdeal * 100f) - 100f;
            float hipDiff = (float) (detailPanel.hip - detailPanel.hipIdeal);
            if (hipPercentage > 10)
            {
                balloonHip.SetBalloonValue(hipPercentage, hipDiff, balloonColorPhase2);
            }
            else if (hipPercentage > 0)
            {
                balloonHip.SetBalloonValue(hipPercentage, hipDiff, balloonColorPhase1);
            }
            else
            {
                balloonHip.SetBalloonValue(hipPercentage, hipDiff, balloonColorDefault);
            }

            maxPercentage = Math.Max(maxPercentage, hipPercentage);

            // 太もも
            detailPanel.thighsIdeal = height * (gender == Gender.Male ? 0.32f : 0.3f);
            detailPanel.thighs = (measurement.LeftThigh + measurement.RightThigh) / 2.0 * AppConst.MeasurementDataScale;
            float thighsPercentage = (float) (detailPanel.thighs / detailPanel.thighsIdeal * 100f) - 100f;
            float thighsDiff = (float) (detailPanel.thighs - detailPanel.thighsIdeal);
            if (thighsPercentage > 10)
            {
                balloonThighs.SetBalloonValue(thighsPercentage, thighsDiff, balloonColorPhase2);
            }
            else if (thighsPercentage > 0)
            {
                balloonThighs.SetBalloonValue(thighsPercentage, thighsDiff, balloonColorPhase1);
            }
            else
            {
                balloonThighs.SetBalloonValue(thighsPercentage, thighsDiff, balloonColorDefault);
            }

            maxPercentage = Math.Max(maxPercentage, thighsPercentage);

            // マシュマロマンのフェイスマテリアル変更
            marshmallowManController.SetHeadMaterial(maxPercentage);

            // マシュマロマンの各部位マテリアル色、ブレンドシェイプス変更
            marshmallowManController.SetPartsMesh(MarshmallowManParts.Waist, waistPercentage);
            marshmallowManController.SetPartsMesh(MarshmallowManParts.Hip, hipPercentage);
            marshmallowManController.SetPartsMesh(MarshmallowManParts.LeftThighs, thighsPercentage);
            marshmallowManController.SetPartsMesh(MarshmallowManParts.RightThighs, thighsPercentage);

            // マシュマロマンの各部位と吹き出しを繋ぐ線を更新
            OnUpdateBalloonLine();
        }

        public void OnUpdateBalloonLine()
        {
            var positions = marshmallowManController.GetPartsWorldPoints(MarshmallowManParts.Waist);
            balloonWaist.UpdateLine(positions);

            positions = marshmallowManController.GetPartsWorldPoints(MarshmallowManParts.Hip);
            balloonHip.UpdateLine(positions);

            positions = marshmallowManController.GetPartsWorldPoints(MarshmallowManParts.LeftThighs);
            positions.AddRange(marshmallowManController.GetPartsWorldPoints(MarshmallowManParts.RightThighs));
            balloonThighs.UpdateLine(positions);
        }
    }
}