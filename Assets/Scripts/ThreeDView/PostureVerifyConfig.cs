using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ThreeDView {
    /// <summary>
    /// 姿勢判定個所.
    /// ※詳細は以下のエクセルを参照.
    ///   (Dropbox)\姿勢機能\姿勢判定処理_170728.xlsx
    /// </summary>
    public enum PostureVerifyPoint {
        /// <summary>
        /// 重心Y軸ラインと耳の乖離.
        /// 姿勢の前後の傾斜.
        /// (左右アングル)
        /// </summary>
        UpperInclinationRatio = 0,

        /// <summary>
        /// 重心Y軸ラインとヒザの乖離.
        /// 姿勢の前後の傾斜.
        /// (左右アングル)
        /// </summary>
        LowerInclinationRatio = 1,

        /// <summary>
        /// 骨盤の前後傾斜角度の正常値.
        /// 骨盤の前後の傾斜.
        /// (左右アングル)
        /// </summary>
        PelvisInclinationAngle = 2,

        /// <summary>
        /// 背骨(胸椎)角度の正常値からの誤差.
        /// 後弯の度合い.
        /// (左右アングル)
        /// </summary>
        ThoracicVertebraAngle = 3,

        /// <summary>
        /// 背骨(腰椎上部)角度の正常値からの誤差.
        /// 前弯の度合い.
        /// (左右アングル)
        /// </summary>
        LumbarVertebraAngle = 4,

        /// <summary>
        /// 首の湾曲角度の正常値からの誤差.
        /// 首の湾曲度合い.
        /// (左右アングル)
        /// </summary>
        NeckCurveAngle = 5,

        /// <summary>
        /// 重心Y軸ラインと頭頂との乖離.
        /// 体全体の左右の傾斜.
        /// (前面アングル)
        /// </summary>
        BodyInclinationRatio = 6,

        /// <summary>
        /// 大腿骨大転子から膝頭までの両足間の差（右ー左）.
        /// 長さ.
        /// (前面アングル)
        /// </summary>
        UpperLegLengthRatio = 7,

        /// <summary>
        /// 膝頭から足首までの高さの両足間の差.
        /// 長さ.
        /// (前面アングル)
        /// </summary>
        LowerLegLengthRatio = 8,

        /// <summary>
        /// 肩峰の左右の高さの差（右ー左）.
        /// 肩の傾斜.
        /// (前面アングル)
        /// </summary>
        ShoulderInclinationRatio = 9,

        /// <summary>
        /// 左右の上前腸骨棘の高さの差（右ー左）.
        /// 骨盤の傾斜.
        /// (前面アングル)
        /// </summary>
        PelvisInclinationRatio = 10,

        /// <summary>
        /// ヒザ関節の外反角度（FTA）.
        /// O脚、X脚.
        /// (前面アングル)
        /// </summary>
        KneeJointAngle = 11,

        /// <summary>
        /// 左右の鎖骨角度の正常角度からの差.
        /// 両肩それぞれの前後のねじれ.
        /// (上アングル)
        /// </summary>
        ShoulderDistortionAngle = 12,

        /// <summary>
        /// 首の左右回転角度.
        /// 首の左右のねじれ.
        /// (上アングル)
        /// </summary>
        NeckDistortionAngle = 13,

        /// <summary>
        /// 腰の左右を結ぶ線と、肩の左右を結ぶ線の角度差（腰に対する肩）.
        /// 肩と腰のねじれ.
        /// (下アングル)
        /// </summary>
        UpperBodyDistortionAngle = 14,

        /// <summary>
        /// 膝頭の前後差（右ー左）.
        /// 膝の前後差.
        /// (下アングル)
        /// </summary>
        KneeAnteroposteriorDiffRatio = 15,
    }


    public static class PostureVerifyPointExtension {
        private struct PostureInfo {
            public RangeThreshold threshold { get; set; }
            public string description { get; set; }
            public BonePart[] boneParts { get; set; }

            public PostureInfo(RangeThreshold threshold, string description, BonePart[] boneParts) {
                this.threshold = threshold;
                this.description = description;
                this.boneParts = boneParts;
            }
        }

        private static readonly Dictionary<PostureVerifyPoint, PostureInfo> _postureInfoTable = new Dictionary<PostureVerifyPoint, PostureInfo>()
        {
            // 左右アングル.
            { PostureVerifyPoint.UpperInclinationRatio,
                new PostureInfo(
                    new RangeThreshold(-1.1f, 1.1f, VerifyType.Length),
                    "重心Y軸ラインと耳の乖離",
                    new BonePart[] {
                        BonePart.head_b,
                        BonePart.neck_b,
                        BonePart.kyotsui_b,
                        BonePart.yotsui_b,
                        BonePart.nose_nan,
                        BonePart.ear_nan,
                        BonePart.neck_nan,
                        BonePart.kyotsui_nan,
                        BonePart.yotsui_nan
                    }) },
            { PostureVerifyPoint.LowerInclinationRatio,
                new PostureInfo(
                    new RangeThreshold(-0.5f, 0.5f, VerifyType.Length),
                    "重心Y軸ラインとヒザの乖離",
                    new BonePart[] {
                        BonePart.daitai_l_b,
                        BonePart.daitai_r_b,
                        BonePart.sune_l_b,
                        BonePart.sune_r_b,
                        BonePart.daitai_l_nan1,
                        BonePart.daitai_l_nan2,
                        BonePart.daitai_r_nan1,
                        BonePart.daitai_r_nan2,
                        BonePart.sune_l_nan,
                        BonePart.sune_r_nan
                    }) },
            { PostureVerifyPoint.PelvisInclinationAngle,
                new PostureInfo(
                    new RangeThreshold(-3, 3, VerifyType.Angle),
                    "骨盤の前後傾斜角度",
                    new BonePart[] {
                        BonePart.senkotsu_b,
                        BonePart.kankotsu_b
                    }) },
            { PostureVerifyPoint.ThoracicVertebraAngle,
                new PostureInfo(
                    new RangeThreshold(-3, 3, VerifyType.Angle),
                    "背骨(胸椎)角度の正常値からの誤差",
                    new BonePart[] {
                        BonePart.kyotsui_b,
                        BonePart.kyotsui_nan,
                    }) },
            { PostureVerifyPoint.LumbarVertebraAngle,
                new PostureInfo(
                    new RangeThreshold(-3, 3, VerifyType.Angle),
                    "背骨(腰椎上部)角度の正常値からの誤差",
                    new BonePart[] {
                        BonePart.yotsui_b,
                        BonePart.yotsui_nan,
                    }) },
            { PostureVerifyPoint.NeckCurveAngle,
                new PostureInfo(
                    new RangeThreshold(-3, 3, VerifyType.Angle),
                    "首の湾曲角度の正常値からの誤差",
                    new BonePart[] {
                        BonePart.neck_b,
                        BonePart.neck_nan,
                    }) },
            
            // 前面アングル.
            { PostureVerifyPoint.BodyInclinationRatio,
                new PostureInfo(
                    new RangeThreshold(-0.8f, 0.8f, VerifyType.Length),
                    "重心Y軸ラインと頭頂との乖離",
                    new BonePart[] {
                        BonePart.head_b,
                        BonePart.neck_b,
                        BonePart.kyotsui_b,
                        BonePart.yotsui_b,
                    }) },
            { PostureVerifyPoint.UpperLegLengthRatio,
                new PostureInfo(
                    new RangeThreshold(-0.8f, 0.8f, VerifyType.Length),
                    "大腿骨大転子から膝頭までの両足間の差 （右ー左）",
                    new BonePart[] {
                        BonePart.kankotsu_b
                    }) },
            { PostureVerifyPoint.LowerLegLengthRatio,
                new PostureInfo(
                    new RangeThreshold(-0.5f, 0.5f, VerifyType.Length),
                    "膝頭から足首までの高さの両足間の差",
                    new BonePart[] {
                        BonePart.sune_l_b,
                        BonePart.sune_r_b,
                        BonePart.sune_l_nan,
                        BonePart.sune_r_nan
                    }) },
            { PostureVerifyPoint.ShoulderInclinationRatio,
                new PostureInfo(
                    new RangeThreshold(-0.5f, 0.5f, VerifyType.Length),
                    "肩峰の左右の高さの差（右ー左）",
                    new BonePart[] {
                        BonePart.sakotsu_l_b,
                        BonePart.sakotsu_r_b
                    }) },
            { PostureVerifyPoint.PelvisInclinationRatio,
                new PostureInfo(
                    new RangeThreshold(-0.5f, 0.5f, VerifyType.Length),
                    "左右の上前腸骨棘の高さの差（右ー左）",

                    new BonePart[] {
                        BonePart.sakotsu_l_b,
                        BonePart.sakotsu_r_b
                    }) },
            { PostureVerifyPoint.KneeJointAngle,
                new PostureInfo(
                    new RangeThreshold(174, 178, VerifyType.Angle, true),
                    "ヒザ関節の外反角度（FTA）",
                    new BonePart[] {
                        BonePart.daitai_l_b,
                        BonePart.sune_l_b,
                        BonePart.daitai_r_b,
                        BonePart.sune_r_b,
                        BonePart.daitai_l_nan1,
                        BonePart.daitai_l_nan2,
                        BonePart.sune_l_nan,
                        BonePart.daitai_r_nan1,
                        BonePart.daitai_r_nan2,
                        BonePart.sune_r_nan
                    }) },
            
            // 上アングル.
            { PostureVerifyPoint.ShoulderDistortionAngle,
                new PostureInfo(
                    new RangeThreshold(-2, 2, VerifyType.Angle),
                    "左右の鎖骨角度の正常角度からの差",
                    new BonePart[] {
                        BonePart.sakotsu_l_b,
                        BonePart.sakotsu_r_b,
                        BonePart.kenko_l_b,
                        BonePart.kenko_r_b
                    }) },
            { PostureVerifyPoint.NeckDistortionAngle,
                new PostureInfo(
                    new RangeThreshold(-3, 3, VerifyType.Angle),
                    "首の左右回転角度",
                    new BonePart[] {
                        BonePart.head_b,
                        BonePart.nose_nan,
                        BonePart.ear_nan
                    }) },
            
            // 下アングル.
            { PostureVerifyPoint.UpperBodyDistortionAngle,
                new PostureInfo(
                    new RangeThreshold(-3, 3, VerifyType.Angle),
                    "腰の角度を反映した線（青）と、左右の肩を結ぶ線の角度差",
                    new BonePart[] {
                        BonePart.sakotsu_l_b,
                        BonePart.sakotsu_r_b,
                        BonePart.kenko_l_b,
                        BonePart.kenko_r_b
                    }) },
            { PostureVerifyPoint.KneeAnteroposteriorDiffRatio,
                new PostureInfo(
                    new RangeThreshold(-0.5f, 0.5f, VerifyType.Length),
                    "膝頭の前後差（右ー左）",
                    new BonePart[] {
                        BonePart.daitai_l_b,
                        BonePart.daitai_r_b,
                        BonePart.sune_l_b,
                        BonePart.sune_r_b,
                        BonePart.daitai_l_nan1,
                        BonePart.daitai_l_nan2,
                        BonePart.daitai_r_nan1,
                        BonePart.daitai_r_nan2,
                        BonePart.sune_l_nan,
                        BonePart.sune_r_nan
                    }) },
        };

        public static RangeThreshold ToRangeThreshold(this PostureVerifyPoint point) {
            return _postureInfoTable[point].threshold;
        }

        public static string ToDescription(this PostureVerifyPoint point) {
            return _postureInfoTable[point].description;
        }

        public static BonePart[] GetBoneParts(this PostureVerifyPoint point) {
            return _postureInfoTable[point].boneParts;
        }
    }

    public enum PostureCondition {
        /// <summary>
        /// 正常.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 前傾（頭部）.
        /// </summary>
        ForwardTiltHead = 1,

        /// <summary>
        /// 後傾（頭部）.
        /// </summary>
        BackwardTiltHead = 2,

        /// <summary>
        /// 前傾（膝）.
        /// </summary>
        ForwardTiltKnee = 3,

        /// <summary>
        /// 後傾（膝）.
        /// </summary>
        BackwardTiltKnee = 4,

        /// <summary>
        /// 前傾（骨盤）.
        /// </summary>
        ForwardTiltPelvis = 5,

        /// <summary>
        /// 後傾（骨盤）.
        /// </summary>
        BackwardTiltPelvis = 6,

        /// <summary>
        /// 猫背（胸椎後弯）.
        /// </summary>
        Stoop = 7,

        /// <summary>
        /// 平背.
        /// </summary>
        FlatBack = 8,

        /// <summary>
        /// 腰椎平坦.
        /// </summary>
        LumbarFlat = 9,

        /// <summary>
        /// おなか突出.
        /// </summary>
        StomachProtruding = 10,

        /// <summary>
        /// ストレートネック.
        /// </summary>
        StraightNeck = 11,

        /// <summary>
        /// のけぞり（頸部後屈）.
        /// </summary>
        NeckBackFlexion = 12,

        /// <summary>
        /// 左傾き（体）.
        /// </summary>
        LeftTiltBody = 13,

        /// <summary>
        /// 右傾き（体）.
        /// </summary>
        RightTiltBody = 14,

        /// <summary>
        /// 左寛骨前傾 or 右寛骨後傾 or 両方.
        /// </summary>
        LeftThighShort = 15,

        /// <summary>
        /// 右寛骨前傾 or 左寛骨後傾 or 両方.
        /// </summary>
        RightThighShort = 16,

        /// <summary>
        /// 膝下長（左が短い）.
        /// </summary>
        LeftLowerLegLengthShort = 17,

        /// <summary>
        /// 膝下長（右が短い）.
        /// </summary>
        RightLowerLegLengthShort = 18,

        /// <summary>
        /// 肩峰左傾.
        /// </summary>
        LeftTiltAcromion = 19,

        /// <summary>
        /// 肩峰右傾.
        /// </summary>
        RightTiltAcromion = 20,

        /// <summary>
        /// 骨盤左傾.
        /// </summary>
        LeftTiltPelvis = 21,

        /// <summary>
        /// 骨盤右傾.
        /// </summary>
        RightTiltPelvis = 22,

        /// <summary>
        /// X脚.
        /// </summary>
        KnockKnees = 23,

        /// <summary>
        /// O脚.
        /// </summary>
        BowLegs = 24,

        /// <summary>
        /// 左ねじれ（肩）.
        /// </summary>
        LeftTwistShoulder = 25,

        /// <summary>
        /// 右ねじれ（肩）.
        /// </summary>
        RightTwistShoulder = 26,

        /// <summary>
        /// 左ねじれ（首）.
        /// </summary>
        LeftTwistNeck = 27,

        /// <summary>
        /// 右ねじれ（首）.
        /// </summary>
        RightTwistNeck = 28,

        /// <summary>
        /// 左ねじれ（腰）.
        /// </summary>
        LeftTwistWaist = 29,

        /// <summary>
        /// 右ねじれ（腰）.
        /// </summary>
        RightTwistWaist = 30,

        /// <summary>
        /// 右膝が前に出ている.
        /// </summary>
        RightKneeProtruding = 31,

        /// <summary>
        /// 左膝が前に出ている.
        /// </summary>
        LeftKneeProtruding = 32,
    }

    public static class PostureConditionExtension {
        private class Description {
            public string title { get; set; }
            public string summary { get; set; }
            public string detail { get; set; }
            public string advice { get; set; }
            public Description(string title, string summary, string detail, string advice) {
                this.title = title;
                this.summary = summary;
                this.detail = detail;
                this.advice = advice;
            }
        }

        /// <summary>
        /// 姿勢判定の症状とメッセージのテーブル.
        /// </summary>
        private static readonly Dictionary<PostureCondition, Description>
            _postureConditionTable = new Dictionary<PostureCondition, Description>(){
                {PostureCondition.Normal,                   new Description("", "", "", "")},
                {PostureCondition.ForwardTiltHead,          new Description("前傾", "体が前に傾いています。",  "体が前に傾いています。\n耳が体の重心の軸よりも{0:0.0}cm、前に出ています。\n（縦に重なっているのが正常な状態です）", "")},
                {PostureCondition.BackwardTiltHead,         new Description("後傾", "体が後ろに傾いています。", "体が後ろに傾いています。\n耳が体の重心の軸よりも{0:0.0}cm、後ろに出ています。\n（縦に重なっているのが正常な状態です）", "")},
                {PostureCondition.ForwardTiltKnee,          new Description("前傾", "体が前に傾いています。", "体が前に傾いています。\n膝が体の重心の軸よりも{0:0.0}cm、前に出ています。\n（縦に重なっているのが正常な状態です）", "")},
                {PostureCondition.BackwardTiltKnee,         new Description("後傾", "体が後ろに傾いています。", "体が後ろに傾いています。\n膝が体の重心の軸よりも{0:0.0}cm、後ろに出ています。\n（縦に重なっているのが正常な状態です）", "")},
                {PostureCondition.ForwardTiltPelvis,        new Description("骨盤前傾", "骨盤が前に傾いています。", "骨盤が前に傾いています。\n正常な範囲より{0:0.0}度、前に傾き過ぎています。\n「骨盤前傾」の傾向が見受けられます。", "")},
                {PostureCondition.BackwardTiltPelvis,       new Description("骨盤後傾", "骨盤が後ろに傾いています。", "骨盤が後ろに傾いています。\n正常な範囲より{0:0.0}度、後ろに傾き過ぎています。\n「骨盤後傾」の傾向が見受けられます。", "")},
                {PostureCondition.Stoop,                    new Description("猫背（胸椎前弯）", "背骨が前方に曲がっています。", "背骨が前方に曲がっています。\n正常な範囲より{0:0.0}度、湾曲しすぎています。\n「猫背（胸椎後弯）」の傾向が見受けられます。", "")},
                {PostureCondition.FlatBack,                 new Description("平背（へいはい）", "背骨の前湾が足らない、もしくは後方に反り返っています。", "背骨の前湾が足らない、もしくは後方に反り返っています。\n正常な範囲より{0:0.0}度、湾曲が足らない、もしくは後方へ反り返っています。\n「平背」の傾向が見受けられます。", "")},
                {PostureCondition.LumbarFlat,               new Description("腰椎平坦", "腰椎が前方に曲がっています。", "腰椎が前方に曲がっています。\n本来、なだらかに前方に湾曲しているはずの腰椎が、正常な範囲より{0:0.0}度、後弯しています。「腰椎平坦」の傾向が見受けられます。", "")},
                {PostureCondition.StomachProtruding,        new Description("おなか突出", "腰椎が後方に曲がっています。", "腰椎が後方に曲がっています。\n正常な範囲より{0:0.0}度、前弯し過ぎています。\n「おなか突出」の傾向が見受けられます。", "")},
                {PostureCondition.StraightNeck,             new Description("ストレートネック", "首の湾曲が足りません。", "首の湾曲が足りません。\n本来、なだらかに後ろに湾曲しているはずの首の骨が、正常な範囲より{0:0.0}度、湾曲が足りない、もしくは前弯しています。\n「ストレートネック」の傾向が見受けられます。", "")},
                {PostureCondition.NeckBackFlexion,          new Description("のけぞり（頸部後屈）", "首が後ろに曲がっています。", "首が後ろに曲がっています。\n正常な範囲より{0:0.0}度、後弯し過ぎています。\n「のけぞり（頸部後屈）」の傾向が見受けられます。", "")},
                {PostureCondition.LeftTiltBody,             new Description("左傾き", "体が左に傾いています。", "体が左に傾いています。\n体の重心の軸よりも{0:0.0}cm、左に傾いています。", "")},
                {PostureCondition.RightTiltBody,            new Description("右傾き", "体が右に傾いています。", "体が右に傾いています。\n体の重心の軸よりも{0:0.0}cm、右に傾いています。", "")},
                {PostureCondition.LeftThighShort,           new Description("右寛骨前傾 or 左寛骨後傾 or 両方", "右の大腿よりも左の大腿の長さが短いようです。", "右の大腿よりも左の大腿の長さが{0:0.0}cm、短いようです。\n左寛骨が右寛骨よりも後ろに傾いている可能性があります。", "")},
                {PostureCondition.RightThighShort,          new Description("左寛骨前傾 or 右寛骨後傾 or 両方", "左の大腿よりも右の大腿の長さが短いようです。", "左の大腿よりも右の大腿の長さが{0:0.0}cm、短いようです。\n右寛骨が左寛骨よりも後ろに傾いている可能性があります。", "")},
                {PostureCondition.LeftLowerLegLengthShort,  new Description("膝頭から足首までの高さの両足間の差", "右の膝下よりも左の膝下の長さが短いようです。", "右の膝下よりも左の膝下の長さが{0:0.0}cm、短いようです。", "左右の足の長さに不均衡が見られます。\n歩行や走行の際は十分に休息をとるなど負荷を和らげる日常生活をお勧めします。")},
                {PostureCondition.RightLowerLegLengthShort, new Description("膝頭から足首までの高さの両足間の差", "左の膝下よりも右の膝下の長さが短いようです。", "左の膝下よりも右の膝下の長さが{0:0.0}cm、短いようです。", "左右の足の長さに不均衡が見られます。\n歩行や走行の際は十分に休息をとるなど負荷を和らげる日常生活をお勧めします。")},
                {PostureCondition.LeftTiltAcromion,         new Description("左傾き", "左右の肩の高さに差があります。", "左右の肩の高さに差があります。\n左肩が、右肩より{0:0.0}cm下がっています。", "")},
                {PostureCondition.RightTiltAcromion,        new Description("右傾き", "左右の肩の高さに差があります。", "左右の肩の高さに差があります。\n右肩が、左肩より{0:0.0}cm下がっています。", "")},
                {PostureCondition.LeftTiltPelvis,           new Description("骨盤の左傾き", "骨盤が左右に傾いています。", "骨盤が左右に傾いています。\n骨盤の左側が右側よりも{0:0.0}cm下がっています。", "")},
                {PostureCondition.RightTiltPelvis,          new Description("骨盤の右傾き", "骨盤が左右に傾いています。", "骨盤が左右に傾いています。\n骨盤の右側が左側よりも{0:0.0}cm下がっています。", "")},
                {PostureCondition.KnockKnees,               new Description("X脚", "膝が内側に曲がっています。", "膝が内側に曲がっています。\n膝の外反角度の正常値より{0:0.0}度、内側に曲がっています。\n「X脚（外反膝）」の傾向が見受けられます。", "")},
                {PostureCondition.BowLegs,                  new Description("O脚", "膝が外側に曲がっています。", "膝が外側に曲がっています。\n膝の外反角度の正常値より{0:0.0}度、外側に曲がっています。\n「O脚（内反膝）」の傾向が見受けられます。", "")},
                {PostureCondition.LeftTwistShoulder,        new Description("左ねじれ", "肩が前後方向にねじれています。", "肩が前後方向にねじれています。\n正常な状態よりも左肩が{0:0.0}度後ろに、右肩が{1:0.0}度前にねじれています。", "")},
                {PostureCondition.RightTwistShoulder,       new Description("右ねじれ", "肩が前後方向にねじれています。", "肩が前後方向にねじれています。\n正常な状態よりも左肩が{0:0.0}度前に、右肩が{1:0.0}度後ろにねじれています。", "")},
                {PostureCondition.LeftTwistNeck,            new Description("首の右ねじれ", "首の向きがねじれています。", "首の向きがねじれています。\n正常な値より{0:0.0}度、右にねじれています。", "")},
                {PostureCondition.RightTwistNeck,           new Description("首の左ねじれ", "首の向きがねじれています。", "首の向きがねじれています。\n正常な値より{0:0.0}度、左にねじれています。", "")},
                {PostureCondition.LeftTwistWaist,           new Description("右ねじれ", "肩のラインが右にねじれています。", "腰の角度を反映した線（青）と、両肩を結ぶ線がねじれています。\n肩のラインが{0:0.0}度、右にねじれています。", "")},
                {PostureCondition.RightTwistWaist,          new Description("左ねじれ", "肩のラインが左にねじれています。", "腰の角度を反映した線（青）と、両肩を結ぶ線がねじれています。\n肩のラインが{0:0.0}度、左にねじれています。", "")},
                {PostureCondition.RightKneeProtruding,      new Description("膝の前後差（右が前）", "両膝の位置が前後にずれています。", "両膝の位置が前後にずれています。\n右膝の方が{0:0.0}cm、前に出ています。", "")},
                {PostureCondition.LeftKneeProtruding,       new Description("膝の前後差（左が前）", "両膝の位置が前後にずれています。", "両膝の位置が前後にずれています。\n左膝の方が{0:0.0}cm、前に出ています。", "")}
            },
            _postureConditionTableChi = new Dictionary<PostureCondition, Description>(){
                {PostureCondition.Normal,                   new Description("", "", "", "")},
                {PostureCondition.ForwardTiltHead,          new Description("身体前倾", "体が前に傾いています。",  "身体呈前倾状。\n以身体的重心为轴线，耳朵向前探出{0:0.0}厘米。\n（纵轴上的重叠属于正常状态）", "")},
                {PostureCondition.BackwardTiltHead,         new Description("身体后倾", "体が後ろに傾いています。", "身体呈后仰状。\n以身体的重心为轴线，耳朵向后探出{0:0.0}厘米。\n（纵轴上的重叠属于正常状态）", "")},
                {PostureCondition.ForwardTiltKnee,          new Description("身体前倾", "体が前に傾いています。", "身体呈前倾状。\n以身体的重心为轴线，膝盖向前探出{0:0.0}厘米。\n（纵轴上的重叠属于正常状态）", "")},
                {PostureCondition.BackwardTiltKnee,         new Description("身体后倾", "体が後ろに傾いています。", "身体呈后仰状。\n以身体的重心为轴线，膝盖向后探出{0:0.0}厘米。\n（纵轴上的重叠属于正常状态）", "")},
                {PostureCondition.ForwardTiltPelvis,        new Description("骨盆前倾", "骨盤が前に傾いています。", "骨盆呈前倾状。\n与正常的范围相比，过度前倾{0:0.0}度。\n可以看到“骨盆前傾”的倾向。", "")},
                {PostureCondition.BackwardTiltPelvis,       new Description("骨盆后倾", "骨盤が後ろに傾いています。", "骨盆呈后倾状。\n与正常的范围相比，过度后倾{0:0.0}度。\n可以看到“骨盆后倾”的倾向。", "")},
                {PostureCondition.Stoop,                    new Description("貓背（胸椎后弯）", "背骨が前方に曲がっています。", "脊柱向前彎曲。\n与正常的范围相比，过度弯曲{0:0.0}度。\n可以看到“驼背（胸椎后弯）”的傾向。", "")},
                {PostureCondition.FlatBack,                 new Description("平背", "背骨の前湾が足らない、もしくは後方に反り返っています。", "脊柱后弯不足，或者脊柱呈反向弯曲状。\n与正常的范围相比，弯曲角度{0:0.0}度不足，或者脊柱呈反向弯曲状。\n可以看到“平背”的倾向。", "")},
                {PostureCondition.LumbarFlat,               new Description("腰椎扁平", "腰椎が前方に曲がっています。", "腰椎呈向前弯曲状。\n本来腰椎应该向后呈平缓的弯曲状，与正常的范围相比，向前弯曲{0:0.0}度。\n可以看到“腰椎曲度变直”的傾向。", "")},
                {PostureCondition.StomachProtruding,        new Description("腹部突出", "腰椎が後方に曲がっています。", "腰椎向後彎曲。\n与正常的范围相比，过度后弯{0:0.0}度。\n可以看到“腹部前突”的傾向。", "")},
                {PostureCondition.StraightNeck,             new Description("直颈", "首の湾曲が足りません。", "颈椎弯曲度不够。\n本来颈椎应该向后呈平缓的弯曲状，与正常的范围相比，弯曲角度{0:0.0}度不足，或者向前弯曲。\n可以看到“直頸”的倾向。", "")},
                {PostureCondition.NeckBackFlexion,          new Description("后仰（颈部后曲）", "首が後ろに曲がっています。", "脖子向後彎曲。\n与正常的范围相比，向后过度弯曲{0:0.0}度。\n可以看到“仰面朝天（颈部后曲）”的倾向。", "")},
                {PostureCondition.LeftTiltBody,             new Description("左倾斜", "体が左に傾いています。", "身体呈向左倾斜状。\n以身体的重心为轴线，身体向左倾斜{0:0.0}厘米", "")},
                {PostureCondition.RightTiltBody,            new Description("右倾斜", "体が右に傾いています。", "身体呈向右倾斜状。\n以身体的重心为轴线，身体向右倾斜{0:0.0}厘米", "")},
                {PostureCondition.LeftThighShort,           new Description("单侧或两侧髋骨前倾", "右の大腿よりも左の大腿の長さが短いようです。", "左侧大腿的长度似乎比右侧大腿的长度短了{0:0.0}厘米\n与右侧髋骨相比较，左侧髋骨存在向后错位的可能性", "")},
                {PostureCondition.RightThighShort,          new Description("单侧或两侧髋骨前倾", "左の大腿よりも右の大腿の長さが短いようです。", "右侧大腿的长度似乎比左侧大腿的长度短了{0:0.0}厘米\n与左侧髋骨相比较，右侧髋骨存在向后错位的可能性", "")},
                {PostureCondition.LeftLowerLegLengthShort,  new Description("左右脚膝高不一致", "右の膝下よりも左の膝下の長さが短いようです。", "左侧膝盖以下长度似乎比右侧膝盖以下长度短了{0:0.0}厘米", "左右の足の長さに不均衡が見られます。\n歩行や走行の際は十分に休息をとるなど負荷を和らげる日常生活をお勧めします。")},
                {PostureCondition.RightLowerLegLengthShort, new Description("左右脚膝高不一致", "左の膝下よりも右の膝下の長さが短いようです。", "右侧膝盖以下长度似乎比左侧膝盖以下长度短了{0:0.0}厘米", "左右の足の長さに不均衡が見られます。\n歩行や走行の際は十分に休息をとるなど負荷を和らげる日常生活をお勧めします。")},
                {PostureCondition.LeftTiltAcromion,         new Description("左肩倾斜", "左右の肩の高さに差があります。", "左肩和右肩的高度存在差異。\n左肩比右肩低{0:0.0}厘米", "")},
                {PostureCondition.RightTiltAcromion,        new Description("右肩倾斜", "左右の肩の高さに差があります。", "左肩和右肩的高度存在差異。\n右肩比左肩低{0:0.0}厘米", "")},
                {PostureCondition.LeftTiltPelvis,           new Description("骨盆左倾", "骨盤が左右に傾いています。", "骨盆左右不平。\n骨盆的左側比右側低{0:0.0}厘米", "")},
                {PostureCondition.RightTiltPelvis,          new Description("骨盆右倾", "骨盤が左右に傾いています。", "骨盆左右不平。\n骨盆的右側比左側低{0:0.0}厘米", "")},
                {PostureCondition.KnockKnees,               new Description("X形腿（膝盖外翻）", "膝が内側に曲がっています。", "膝蓋向內彎曲。\n与膝盖正常的外翻角度相比，向内弯曲{0:0.0}度。\n可以看到“X腿（膝盖外翻）”的傾向", "")},
                {PostureCondition.BowLegs,                  new Description("O形腿（膝盖内翻）", "膝が外側に曲がっています。", "膝蓋向外彎曲。\n与膝盖正常的外翻角度相比，向外弯曲{0:0.0}度。\n可以看到“罗圈腿（膝盖内翻）”的傾向", "")},
                {PostureCondition.LeftTwistShoulder,        new Description("肩部左扭曲", "肩が前後方向にねじれています。", "左右两肩呈前后方向错位\n与正常状态相比，左肩向后错位{0:0.0}度、右肩向前错位{1:0.0}度。", "")},
                {PostureCondition.RightTwistShoulder,       new Description("肩部右扭曲", "肩が前後方向にねじれています。", "左右两肩呈前后方向错位\n与正常状态相比，左肩向前错位{0:0.0}度、右肩向后错位{1:0.0}度。", "")},
                {PostureCondition.LeftTwistNeck,            new Description("头部右扭曲", "首の向きがねじれています。", "颈椎水平角度发生偏差\n与正常值相比，颈椎向右偏差{0:0.0}度。", "")},
                {PostureCondition.RightTwistNeck,           new Description("头部左扭曲", "首の向きがねじれています。", "颈椎水平角度发生偏差\n与正常值相比，颈椎向左偏差{0:0.0}度。", "")},
                {PostureCondition.LeftTwistWaist,           new Description("相对腰线肩部左扭曲", "肩のラインが右にねじれています。", "反映腰部角度的線條（藍色）和連接肩部的線條是不平行的。\n肩部的连线向右扭曲{0:0.0}度。", "")},
                {PostureCondition.RightTwistWaist,          new Description("相对腰线肩部右扭曲", "肩のラインが左にねじれています。", "反映腰部角度的線條（藍色）和連接肩部的線條是不平行的。\n肩部的连线向左扭曲{0:0.0}度。", "")},
                {PostureCondition.RightKneeProtruding,      new Description("右膝靠前", "両膝の位置が前後にずれています。", "膝盖的位置前后错开\n右侧的膝盖前突{0:0.0}厘米前面。", "")},
                {PostureCondition.LeftKneeProtruding,       new Description("左膝靠前", "両膝の位置が前後にずれています。", "膝盖的位置前后错开\n左侧的膝盖前突{0:0.0}厘米前面。", "")}
            },
            _postureConditionTableEng = new Dictionary<PostureCondition, Description>(){
                {PostureCondition.Normal,                   new Description("", "", "", "")},
                {PostureCondition.ForwardTiltHead,          new Description("前傾", "体が前に傾いています。",  "体が前に傾いています。\n耳が体の重心の軸よりも{0:0.0}cm、前に出ています。\n（縦に重なっているのが正常な状態です）", "")},
                {PostureCondition.BackwardTiltHead,         new Description("後傾", "体が後ろに傾いています。", "体が後ろに傾いています。\n耳が体の重心の軸よりも{0:0.0}cm、後ろに出ています。\n（縦に重なっているのが正常な状態です）", "")},
                {PostureCondition.ForwardTiltKnee,          new Description("前傾", "体が前に傾いています。", "体が前に傾いています。\n膝が体の重心の軸よりも{0:0.0}cm、前に出ています。\n（縦に重なっているのが正常な状態です）", "")},
                {PostureCondition.BackwardTiltKnee,         new Description("後傾", "体が後ろに傾いています。", "体が後ろに傾いています。\n膝が体の重心の軸よりも{0:0.0}cm、後ろに出ています。\n（縦に重なっているのが正常な状態です）", "")},
                {PostureCondition.ForwardTiltPelvis,        new Description("骨盤前傾", "骨盤が前に傾いています。", "骨盤が前に傾いています。\n正常な範囲より{0:0.0}度、前に傾き過ぎています。\n「骨盤前傾」の傾向が見受けられます。", "")},
                {PostureCondition.BackwardTiltPelvis,       new Description("骨盤後傾", "骨盤が後ろに傾いています。", "骨盤が後ろに傾いています。\n正常な範囲より{0:0.0}度、後ろに傾き過ぎています。\n「骨盤後傾」の傾向が見受けられます。", "")},
                {PostureCondition.Stoop,                    new Description("猫背（胸椎前弯）", "背骨が前方に曲がっています。", "背骨が前方に曲がっています。\n正常な範囲より{0:0.0}度、湾曲しすぎています。\n「猫背（胸椎後弯）」の傾向が見受けられます。", "")},
                {PostureCondition.FlatBack,                 new Description("平背（へいはい）", "背骨の前湾が足らない、もしくは後方に反り返っています。", "背骨の前湾が足らない、もしくは後方に反り返っています。\n正常な範囲より{0:0.0}度、湾曲が足らない、もしくは後方へ反り返っています。\n「平背」の傾向が見受けられます。", "")},
                {PostureCondition.LumbarFlat,               new Description("腰椎平坦", "腰椎が前方に曲がっています。", "腰椎が前方に曲がっています。\n本来、なだらかに後ろに湾曲しているはずの腰椎が、正常な範囲より{0:0.0}度、前弯しています。「腰椎平坦」の傾向が見受けられます。", "")},
                {PostureCondition.StomachProtruding,        new Description("おなか突出", "腰椎が後方に曲がっています。", "腰椎が後方に曲がっています。\n正常な範囲より{0:0.0}度、後弯し過ぎています。\n「おなか突出」の傾向が見受けられます。", "")},
                {PostureCondition.StraightNeck,             new Description("ストレートネック", "首の湾曲が足りません。", "首の湾曲が足りません。\n本来、なだらかに後ろに湾曲しているはずの首の骨が、正常な範囲より{0:0.0}度、湾曲が足りない、もしくは前弯しています。\n「ストレートネック」の傾向が見受けられます。", "")},
                {PostureCondition.NeckBackFlexion,          new Description("のけぞり（頸部後屈）", "首が後ろに曲がっています。", "首が後ろに曲がっています。\n正常な範囲より{0:0.0}度、後弯し過ぎています。\n「のけぞり（頸部後屈）」の傾向が見受けられます。", "")},
                {PostureCondition.LeftTiltBody,             new Description("左傾き", "体が左に傾いています。", "体が左に傾いています。\n体の重心の軸よりも{0:0.0}cm、左に傾いています。", "")},
                {PostureCondition.RightTiltBody,            new Description("右傾き", "体が右に傾いています。", "体が右に傾いています。\n体の重心の軸よりも{0:0.0}cm、右に傾いています。", "")},
                {PostureCondition.LeftThighShort,           new Description("右寛骨前傾 or 左寛骨後傾 or 両方", "右の大腿よりも左の大腿の長さが短いようです。", "右の大腿よりも左の大腿の長さが{0:0.0}cm、短いようです。\n左寛骨が右寛骨よりも後ろに傾いている可能性があります。", "")},
                {PostureCondition.RightThighShort,          new Description("左寛骨前傾 or 右寛骨後傾 or 両方", "左の大腿よりも右の大腿の長さが短いようです。", "左の大腿よりも右の大腿の長さが{0:0.0}cm、短いようです。\n右寛骨が左寛骨よりも後ろに傾いている可能性があります。", "")},
                {PostureCondition.LeftLowerLegLengthShort,  new Description("膝頭から足首までの高さの両足間の差", "右の膝下よりも左の膝下の長さが短いようです。", "右の膝下よりも左の膝下の長さが{0:0.0}cm、短いようです。", "左右の足の長さに不均衡が見られます。\n歩行や走行の際は十分に休息をとるなど負荷を和らげる日常生活をお勧めします。")},
                {PostureCondition.RightLowerLegLengthShort, new Description("膝頭から足首までの高さの両足間の差", "左の膝下よりも右の膝下の長さが短いようです。", "左の膝下よりも右の膝下の長さが{0:0.0}cm、短いようです。", "左右の足の長さに不均衡が見られます。\n歩行や走行の際は十分に休息をとるなど負荷を和らげる日常生活をお勧めします。")},
                {PostureCondition.LeftTiltAcromion,         new Description("左傾き", "左右の肩の高さに差があります。", "左右の肩の高さに差があります。\n左肩が、右肩より{0:0.0}cm下がっています。", "")},
                {PostureCondition.RightTiltAcromion,        new Description("右傾き", "左右の肩の高さに差があります。", "左右の肩の高さに差があります。\n右肩が、左肩より{0:0.0}cm下がっています。", "")},
                {PostureCondition.LeftTiltPelvis,           new Description("骨盤の左傾き", "骨盤が左右に傾いています。", "骨盤が左右に傾いています。\n骨盤の左側が右側よりも{0:0.0}cm下がっています。", "")},
                {PostureCondition.RightTiltPelvis,          new Description("骨盤の右傾き", "骨盤が左右に傾いています。", "骨盤が左右に傾いています。\n骨盤の右側が左側よりも{0:0.0}cm下がっています。", "")},
                {PostureCondition.KnockKnees,               new Description("X脚", "膝が内側に曲がっています。", "膝が内側に曲がっています。\n膝の外反角度の正常値より{0:0.0}度、内側に曲がっています。\n「X脚（外反膝）」の傾向が見受けられます。", "")},
                {PostureCondition.BowLegs,                  new Description("O脚", "膝が外側に曲がっています。", "膝が外側に曲がっています。\n膝の外反角度の正常値より{0:0.0}度、外側に曲がっています。\n「O脚（内反膝）」の傾向が見受けられます。", "")},
                {PostureCondition.LeftTwistShoulder,        new Description("左ねじれ", "肩が前後方向にねじれています。", "肩が前後方向にねじれています。\n正常な状態よりも左肩が{0:0.0}度後ろに、右肩が{1:0.0}度前にねじれています。", "")},
                {PostureCondition.RightTwistShoulder,       new Description("右ねじれ", "肩が前後方向にねじれています。", "肩が前後方向にねじれています。\n正常な状態よりも左肩が{0:0.0}度前に、右肩が{1:0.0}度後ろにねじれています。", "")},
                {PostureCondition.LeftTwistNeck,            new Description("首の右ねじれ", "首の向きがねじれています。", "首の向きがねじれています。\n正常な値より{0:0.0}度、右にねじれています。", "")},
                {PostureCondition.RightTwistNeck,           new Description("首の左ねじれ", "首の向きがねじれています。", "首の向きがねじれています。\n正常な値より{0:0.0}度、左にねじれています。", "")},
                {PostureCondition.LeftTwistWaist,           new Description("右ねじれ", "肩のラインが右にねじれています。", "腰の角度を反映した線（青）と、両肩を結ぶ線がねじれています。\n肩のラインが{0:0.0}度、右にねじれています。", "")},
                {PostureCondition.RightTwistWaist,          new Description("左ねじれ", "肩のラインが左にねじれています。", "腰の角度を反映した線（青）と、両肩を結ぶ線がねじれています。\n肩のラインが{0:0.0}度、左にねじれています。", "")},
                {PostureCondition.RightKneeProtruding,      new Description("膝の前後差（右が前）", "両膝の位置が前後にずれています。", "両膝の位置が前後にずれています。\n右膝の方が{0:0.0}cm、前に出ています。", "")},
                {PostureCondition.LeftKneeProtruding,       new Description("膝の前後差（左が前）", "両膝の位置が前後にずれています。", "両膝の位置が前後にずれています。\n左膝の方が{0:0.0}cm、前に出ています。", "")}
            };

        public static string ToTitle(this PostureCondition point) {
            string s = "";
            switch (PlayerPrefs.GetString("Lang")) {
                case "Japanese":
                    s = _postureConditionTable[point].title;
                    break;
                default:
                case "English":
                    s = _postureConditionTableEng[point].title;
                    break;
                case "Chinese":
                    s = _postureConditionTableChi[point].title;
                    break;
            }
            return s;
        }

        public static string ToSummary(this PostureCondition point) {
            string s = "";
            switch (PlayerPrefs.GetString("Lang")) {
                case "Japanese":
                    s = _postureConditionTable[point].summary;
                    break;
                default:
                case "English":
                    s = _postureConditionTableEng[point].summary;
                    break;
                case "Chinese":
                    s = _postureConditionTableChi[point].summary;
                    break;
            }
            return s;
        }

        public static string ToAdvice(this PostureCondition point) {
            string s = "";
            switch (PlayerPrefs.GetString("Lang")) {
                case "Japanese":
                    s = _postureConditionTable[point].advice;
                    break;
                default:
                case "English":
                    s = _postureConditionTableEng[point].advice;
                    break;
                case "Chinese":
                    s = _postureConditionTableChi[point].advice;
                    break;
            }
            return s;
        }

        public static string ToDescription(this PostureCondition point, float? value1, float? value2) {
            string s = "";
            switch (PlayerPrefs.GetString("Lang")) {
                case "Japanese":
                    s = string.Format(_postureConditionTable[point].detail, value1, value2);
                    break;
                default:
                case "English":
                    s = string.Format(_postureConditionTableEng[point].detail, value1, value2);
                    break;
                case "Chinese":
                    s = string.Format(_postureConditionTableChi[point].detail, value1, value2);
                    break;
            }
            return s;
        }
    }

    public enum VerifyType : int {
        Length = 0,
        Angle = 1
    }

    public static class PostureVerifyTypeExtension {
        private static readonly Dictionary<VerifyType, string>
            _measurementUnitMap = new Dictionary<VerifyType, string>(){
                { VerifyType.Length, "cm"},
                { VerifyType.Angle,  "°"}
            },
            _measurementUnitMapCHN = new Dictionary<VerifyType, string>(){
                { VerifyType.Length, "厘米"},
                { VerifyType.Angle,  "度"}
            };

        public static string
            ToMeasurementUnit(this VerifyType type) {
                switch (PlayerPrefs.GetString("Lang")) {
                    case "Japanese":
                    default:
                        return _measurementUnitMap[type];
                    case "Chinese":
                        return _measurementUnitMapCHN[type];
                }
        }
    }

    [Serializable]
    public class RangeThreshold {
        [SerializeField]
        private float _min = default;
        public float min {
            get {
                return _min;
            }
        }

        [SerializeField]
        private float _max = default;
        public float max {
            get {
                return _max;
            }
        }

        [SerializeField]
        private VerifyType _type = default;
        public VerifyType type {
            get {
                return _type;
            }
        }

        [SerializeField]
        private bool _containsMinMax = false;
        public bool containsMinMax {
            get {
                return _containsMinMax;
            }
        }

        public RangeThreshold(float min, float max, VerifyType type, bool containsMinMax = false) {
            _min = (type == VerifyType.Angle ? Mathf.DeltaAngle(0, min) : min);
            _max = (type == VerifyType.Angle ? Mathf.DeltaAngle(0, max) : max);
            _type = type;
            _containsMinMax = containsMinMax;
        }

        public float Diff(float src) {
            var diff = 0f;

            src = (type == VerifyType.Angle ? Mathf.DeltaAngle(0, src) : src);

            var compareResult = CompareTo(src);
            if (compareResult != 0) {
                diff = compareResult > 0 ? src - max : src - min;
            }
            return diff;
        }

        public int CompareTo(float src) {
            var value = (type == VerifyType.Angle ? Mathf.DeltaAngle(0, src) : src);
            return Contains(value) ? 0 : (value < min || Mathf.Approximately(value, min) ? -1 : 1);
        }

        public bool Contains(float src) {
            var value = (type == VerifyType.Angle ? Mathf.DeltaAngle(0, src) : src);
            return (containsMinMax && Approximately(value)) || (min < value && value < max);
        }

        private bool Approximately(float src) {
            return Mathf.Approximately(src, min) || Mathf.Approximately(src, max);
        }

        public override string ToString() {
            return string.Format("{0,7}, {1,10}, {2,10}", type.ToString(), min, max);
        }
    }
}
