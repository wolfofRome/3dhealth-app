using System;
using Assets.Scripts.Common.Config;
using Assets.Scripts.Database.DataRow;
using Assets.Scripts.History;
using Assets.Scripts.Network.Response;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Assets.Scripts.Common
{
    [RequireComponent(typeof(Text))]
    public class MeasurementDataPanel : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("Height")]
        protected Text height;
        [SerializeField, FormerlySerializedAs("Inseam")]
        protected Text inseam;
        [SerializeField, FormerlySerializedAs("Neck")]
        protected Text neck;
        [SerializeField, FormerlySerializedAs("Chest")]
        protected Text chest;
        [SerializeField] [FormerlySerializedAs("waist"), FormerlySerializedAs("Waist")]
        protected Text minWaist;
        [SerializeField]
        protected Text maxWaist;
        [SerializeField, FormerlySerializedAs("Hip")]
        protected Text hip;
        [SerializeField, FormerlySerializedAs("LeftThigh")]
        protected Text leftThigh;
        [SerializeField, FormerlySerializedAs("RightThigh")]
        protected Text rightThigh;
        [SerializeField, FormerlySerializedAs("LeftShoulder")]
        protected Text leftShoulder;
        [SerializeField, FormerlySerializedAs("RightShoulder")]
        protected Text rightShoulder;
        [SerializeField, FormerlySerializedAs("LeftSleeve")]
        protected Text leftSleeve;
        [SerializeField, FormerlySerializedAs("RightSleeve")]
        protected Text rightSleeve;
        [SerializeField, FormerlySerializedAs("LeftArm")]
        protected Text leftArm;
        [SerializeField, FormerlySerializedAs("RightArm")]
        protected Text rightArm;
        [SerializeField, FormerlySerializedAs("LeftCalf")]
        protected Text leftCalf;
        [SerializeField, FormerlySerializedAs("RightCalf")]
        protected Text rightCalf;
        
        private Measurement _measurement;

        protected virtual void Start()
        {
        }

        /// <summary>
        /// 採寸データのロード開始イベント.
        /// </summary>
        /// <param name="contents"></param>
        public void OnLoadStart(Contents contents)
        {
            var paramList = GetComponentsInChildren<HistoryLoadParam>(true);
            foreach (var param in paramList)
            {
                param.argment.selectedDate = contents.CreateTimeAsDateTime;
            }
        }

        /// <summary>
        /// 採寸データのロード完了イベント.
        /// </summary>
        /// <param name="loader"></param>
        public void OnUpdateMeasurementData(MeasurementDataLoader loader)
        {
            _measurement = loader.Measurement;
            Apply();
        }

        /// <summary>
        /// 採寸データの表示更新.
        /// </summary>
        protected virtual void Apply()
        {
            SetValue(height, _measurement.correctedHeight * AppConst.MeasurementDataScale);
            SetValue(inseam, _measurement.correctedInseam * AppConst.MeasurementDataScale);
            SetValue(neck, _measurement.Neck * AppConst.MeasurementDataScale);
            //先方の指定で表示するバストサイズに0.93をかけています
            SetValue(chest, _measurement.Chest * AppConst.MeasurementDataScale * 0.93f);
            SetValue(minWaist, _measurement.MinWaist * AppConst.MeasurementDataScale);
            SetValue(hip, _measurement.Hip * AppConst.MeasurementDataScale);
            SetValue(leftThigh, _measurement.LeftThigh * AppConst.MeasurementDataScale);
            SetValue(rightThigh, _measurement.RightThigh * AppConst.MeasurementDataScale);
            SetValue(leftShoulder, _measurement.LeftShoulder * AppConst.MeasurementDataScale);
            SetValue(rightShoulder, _measurement.RightShoulder * AppConst.MeasurementDataScale);
            SetValue(leftSleeve, _measurement.LeftSleeve * AppConst.MeasurementDataScale);
            SetValue(rightSleeve, _measurement.RightSleeve * AppConst.MeasurementDataScale);
            SetValue(leftArm, _measurement.LeftArm * AppConst.MeasurementDataScale);
            SetValue(rightArm, _measurement.RightArm * AppConst.MeasurementDataScale);

            if (Mathf.Abs((float) _measurement.MaxWaist) > 0.1)
                SetValue(maxWaist, _measurement.MaxWaist * AppConst.MeasurementDataScale);
            
            Light fillLight = GameObject.Find("ObjLights/FillLight").GetComponent<Light>();
            Light mainLight = GameObject.Find("ObjLights/MainLight").GetComponent<Light>();

            switch (_measurement.Type)
            {
                case Measurement.TypeSpaceVision:
                    fillLight.intensity = 1.3f;
                    mainLight.intensity = 1.47f;

                    break;
                    
                case Measurement.Type3DBodyLab:
                    SetValue(leftCalf, _measurement.LeftCalf * AppConst.MeasurementDataScale);
                    SetValue(rightCalf, _measurement.RightCalf * AppConst.MeasurementDataScale);

                    fillLight.intensity = 0.1f;
                    mainLight.intensity = 0.1f;

                    break;
            }
        }

        /// <summary>
        /// 体組成データのテキスト設定.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        private static void SetValue(Text text, double value)
        {
            if (text == null) return;
            text.gameObject.transform.parent.gameObject.SetActive(true);
            text.text = FormatValue(value);
        }

        /// <summary>
        /// 採寸データの表示フォーマットの変換.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string FormatValue(double value)
        {
            return $"{value:0.0}cm";
        }
    }
}