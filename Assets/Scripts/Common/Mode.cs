using System;
using System.Collections.Generic;
using Sinfonia;

namespace Assets.Scripts.Common {
    [Serializable]
    public enum Mode {
        ThreeDView,
        DataList,
        Posture,
        BodyAnalysis,
        Distortion,
    }

    public static class ModeExt {
        public static readonly Dictionary<Mode, string> _modeNameDictionary = new Dictionary<Mode, string>() {
            /*// Defaults
            {Mode.ThreeDView, "3Dモード"},
            {Mode.DataList, "データ"},
            {Mode.Posture, "姿勢アドバイス"},
            {Mode.BodyAnalysis, "体型分析"},
            {Mode.Distortion, "積み木分析"},//*/
            {Mode.ThreeDView, TitleBridges.GetTitle(0)},
            {Mode.DataList, TitleBridges.GetTitle(1)},
            {Mode.Posture, TitleBridges.GetTitle(2)},
            {Mode.BodyAnalysis, TitleBridges.GetTitle(3)},
            {Mode.Distortion, TitleBridges.GetTitle(4)}
        };

        public static string GetTitle(this Mode mode) {
            return _modeNameDictionary[mode];
        }
    }
}
