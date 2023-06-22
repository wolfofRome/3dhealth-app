using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Common;
using Assets.Scripts.Database.DataRow;
using Assets.Scripts.Common.Config;

namespace Assets.Scripts.ThreeDView
{
    public enum MeasurementPartArrow
    {
        Hieght = 0,
        InseamLength = 1,
        NeckCircumference = 2,
        ChestCircumference = 3,
        WaistCircumference = 4,
        HipsCircumference = 5,
        LeftThighCircumference = 6,
        RightThighCircumference = 7,
        LeftShoulderLength = 8,
        RightShoulderLength = 9,
        LeftSleeveLength = 10,
        RightSleeveLength = 11,
        LeftArmCircumference = 12,
        RightArmCircumference = 13,
        LeftCalfCircumference = 14,
        RightCalfCircumference = 15,
    }

    [Serializable]
    public struct Arrow
    {
        public MeasurementPartArrow part;
        public SpriteRenderer arrowImage;
    }

    public class ArrowController : MonoBehaviour
    {
        [SerializeField]
        private List<Arrow> _arrows = default;

        private Dictionary<MeasurementPartArrow, SpriteRenderer> _arrowsDic = new Dictionary<MeasurementPartArrow, SpriteRenderer>();

        AvatarController _avatar;
        Measurement _measurement;
        Dictionary<RowData, Vector3> _neck2Src;
        Vector3 _neck2SrcRight = Vector3.zero;
        Vector3 _neck2SrcLeft = Vector3.zero;
        Vector3 _armRightCorrection = Vector2.zero;
        Vector3 _armLeftCorrection = Vector3.zero;
        float _armRightCorrectionValue = 0;
        float _armLeftCorrectionValue = 0;
        bool isUpdated = false;

        private enum ArrowType
        {
            Line = 0,
            Circle = 1
        }

        private struct ArrowRenderInfo
        {
            AvatarBones[] startPoints;
            AvatarBones[] endPoints;
            public ArrowType type { get; set; }

            public ArrowRenderInfo(AvatarBones[] start, AvatarBones[] end, ArrowType type)
            {
                startPoints = start;
                endPoints = end;
                this.type = type;
            }

            /// <summary>
            /// 矢印の開始点取得.
            /// </summary>
            /// <param name="avatar"></param>
            /// <returns></returns>
            public Vector3? GetStartPoint(AvatarController avatar)
            {
                return GetMidPoint(startPoints, avatar);
            }

            /// <summary>
            /// 矢印の終了点の取得.
            /// </summary>
            /// <param name="avatar"></param>
            /// <returns></returns>
            public Vector3? GetEndPoint(AvatarController avatar)
            {
                return GetMidPoint(endPoints, avatar);
            }

            /// <summary>
            /// 開始点と終了点の中点のZ座標取得.
            /// </summary>
            /// <param name="avatar"></param>
            /// <returns></returns>
            public float GetZValue(AvatarController avatar)
            {
                float MaxZ = float.MinValue;
                float MinZ = float.MaxValue;
                if (startPoints != null)
                {
                    foreach (AvatarBones b in startPoints)
                    {
                        var p = avatar.GetPoint(b);
                        if (p != null)
                        {
                            MaxZ = Mathf.Max(MaxZ, ((Vector3)p).z);
                            MinZ = Mathf.Min(MinZ, ((Vector3)p).z);
                        }
                    }
                }
                if (endPoints != null)
                {
                    foreach (AvatarBones b in endPoints)
                    {
                        var p = avatar.GetPoint(b);
                        if (p != null)
                        {
                            MaxZ = Mathf.Max(MaxZ, ((Vector3)p).z);
                            MinZ = Mathf.Min(MinZ, ((Vector3)p).z);
                        }
                    }
                }
                return (MinZ + MaxZ) / 2;
            }

            /// <summary>
            /// 中点の取得.
            /// </summary>
            /// <param name="bones"></param>
            /// <param name="avatar"></param>
            /// <returns></returns>
            private Vector3? GetMidPoint(AvatarBones[] bones, AvatarController avatar)
            {
                if (bones == null) return null;
                Vector3 point = Vector3.zero;
                foreach (AvatarBones b in bones)
                {
                    point += avatar.GetPoint(b) ?? Vector3.zero;
                }
                return point / bones.Length;
            }
        }

        /// <summary>
        /// 採寸個所矢印のサイズ調整用のポイント情報.
        /// AvatarControllerのポーズ調整完了後、矢印のサイズと向きを調整する.
        /// </summary>
        private readonly Dictionary<MeasurementPartArrow, ArrowRenderInfo> _avatarBonesPontsDic = new Dictionary<MeasurementPartArrow, ArrowRenderInfo>() {
        { MeasurementPartArrow.Hieght,                  new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.LeftToe, AvatarBones.RightToe },
                                                                             new AvatarBones[]{ AvatarBones.Head },
                                                                             ArrowType.Line) },
        { MeasurementPartArrow.InseamLength,            new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.LeftToe, AvatarBones.RightToe },
                                                                             new AvatarBones[]{ AvatarBones.Hips },
                                                                             ArrowType.Line) },
        { MeasurementPartArrow.NeckCircumference,       new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.Neck2 },
                                                                             null,
                                                                             ArrowType.Circle) },
        { MeasurementPartArrow.ChestCircumference,      new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.Spine1 },
                                                                             null,
                                                                             ArrowType.Circle) },
        { MeasurementPartArrow.WaistCircumference,      new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.SpineWaist },
                                                                             null,
                                                                             ArrowType.Circle) },
        { MeasurementPartArrow.HipsCircumference,       new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.Hips },
                                                                             null,
                                                                             ArrowType.Circle) },
        { MeasurementPartArrow.LeftThighCircumference,  new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.LeftLeg },
                                                                             new AvatarBones[]{ AvatarBones.LeftUpLeg },
                                                                             ArrowType.Circle) },
        { MeasurementPartArrow.RightThighCircumference, new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.RightLeg },
                                                                             new AvatarBones[]{ AvatarBones.RightUpLeg },
                                                                             ArrowType.Circle) },
        { MeasurementPartArrow.LeftShoulderLength,      new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.Neck1, AvatarBones.Neck1, AvatarBones.Neck2},
                                                                             new AvatarBones[]{ AvatarBones.LeftArm },
                                                                             ArrowType.Line) },
        { MeasurementPartArrow.RightShoulderLength,     new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.Neck1, AvatarBones.Neck1, AvatarBones.Neck2 },
                                                                             new AvatarBones[]{ AvatarBones.RightArm },
                                                                             ArrowType.Line) },
        { MeasurementPartArrow.LeftSleeveLength,        new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.LeftHand },
                                                                             new AvatarBones[]{ AvatarBones.LeftArm },
                                                                             ArrowType.Line) },
        { MeasurementPartArrow.RightSleeveLength,       new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.RightHand },
                                                                             new AvatarBones[]{ AvatarBones.RightArm },
                                                                             ArrowType.Line) },
        { MeasurementPartArrow.LeftArmCircumference,    new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.LeftForeArm },
                                                                             new AvatarBones[]{ AvatarBones.LeftArm },
                                                                             ArrowType.Circle ) },
        { MeasurementPartArrow.RightArmCircumference,   new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.RightForeArm },
                                                                             new AvatarBones[]{ AvatarBones.RightArm },
                                                                             ArrowType.Circle ) },
        { MeasurementPartArrow.LeftCalfCircumference,  new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.LeftFoot },
                                                                             new AvatarBones[]{ AvatarBones.LeftLeg },
                                                                             ArrowType.Circle) },
        { MeasurementPartArrow.RightCalfCircumference, new ArrowRenderInfo( new AvatarBones[]{ AvatarBones.RightFoot },
                                                                             new AvatarBones[]{ AvatarBones.RightLeg },
                                                                             ArrowType.Circle) },
    };

        void Awake()
        {
            foreach (Arrow a in _arrows)
            {
                _arrowsDic.Add(a.part, a.arrowImage);
            }
        }

        // Use this for initialization.
        void Start()
        {

        }

        // Update is called once per frame.
        void Update()
        {
            if (isUpdated && _measurement != null && _avatar != null)
            {
                UpdateArrays(_avatar, _measurement);
                isUpdated = false;
            }
        }

        /// <summary>
        /// 採寸データロード完了イベント.
        /// </summary>
        /// <param name="loader"></param>
        public void OnUpdateMeasurementData(MeasurementDataLoader loader)
        {
            _measurement = loader.Measurement;
            isUpdated = true;
        }

        /// <summary>
        /// 3Dモデルの更新イベント.
        /// </summary>
        /// <param name="avatar"></param>
        public void OnAvatarUpdated(AvatarController avatar)
        {
            _avatar = avatar;

            var neck2Src = avatar.GetSrcPoint(AvatarBones.Neck2);
            if (neck2Src != null)
            {
                if (neck2Src.ContainsKey(RowData.Right)) _neck2SrcRight = neck2Src[RowData.Right];
                if (neck2Src.ContainsKey(RowData.Left)) _neck2SrcLeft = neck2Src[RowData.Left];
            }

            var armRightSrc = avatar.GetSrcPoint(AvatarBones.RightArm);
            if (armRightSrc != null)
            {
                // 矢印の表示調整用の値を算出.
                _armRightCorrectionValue = GetMinDistance(avatar.GetPoint(AvatarBones.RightArm), armRightSrc) / 2;
                _armRightCorrection = Vector3.zero;
                _armRightCorrection.y = _armRightCorrectionValue;
            }

            var armLeftSrc = avatar.GetSrcPoint(AvatarBones.LeftArm);
            if (armLeftSrc != null)
            {
                // 矢印の表示調整用の値を算出.
                _armLeftCorrectionValue = GetMinDistance(avatar.GetPoint(AvatarBones.LeftArm), armLeftSrc) / 2;
                _armLeftCorrection = Vector3.zero;
                _armLeftCorrection.y = _armLeftCorrectionValue;
            }

            isUpdated = true;
        }

        /// <summary>
        /// 矢印のサイズ、及び角度の更新.
        /// </summary>
        /// <param name="avatar"></param>
        /// <param name="_measurement"></param>
        private void UpdateArrays(AvatarController avatar, Measurement _measurement)
        {
            float objScale = AppConst.ObjLoadScale;
            foreach (KeyValuePair<MeasurementPartArrow, SpriteRenderer> pair in _arrowsDic)
            {
                var arrowImg = pair.Value;
                var ari = _avatarBonesPontsDic[pair.Key];
                var start = ari.GetStartPoint(avatar);
                var end = ari.GetEndPoint(avatar);
                var zPos = ari.GetZValue(avatar);
                if (start != null && end != null)
                {
                    // 2点指定時は位置、角度、スケールを調整.
                    Vector3 sp = (Vector3)start;
                    Vector3 ep = (Vector3)end;

                    float distance;
                    switch (pair.Key)
                    {
                        case MeasurementPartArrow.LeftSleeveLength:
                            ep += _armLeftCorrection;
                            distance = Vector2.Distance(ep, sp);
                            break;
                        case MeasurementPartArrow.RightSleeveLength:
                            ep += _armRightCorrection;
                            distance = Vector2.Distance(ep, sp);
                            break;
                        case MeasurementPartArrow.LeftShoulderLength:
                            sp.x = _neck2SrcLeft.x;
                            ep.x -= _armRightCorrectionValue;
                            ep += _armLeftCorrection;
                            sp += _armLeftCorrection;
                            distance = Vector2.Distance(ep, sp);
                            break;
                        case MeasurementPartArrow.RightShoulderLength:
                            sp.x = _neck2SrcRight.x;
                            ep.x += _armLeftCorrectionValue;
                            ep += _armRightCorrection;
                            sp += _armRightCorrection;
                            distance = Vector2.Distance(ep, sp);
                            break;
                        case MeasurementPartArrow.InseamLength:
                            distance = _measurement.Inseam;
                            break;
                        default:
                            distance = Vector2.Distance(ep, sp);
                            break;
                    }

                    sp.z = zPos;
                    if (ari.type == ArrowType.Line)
                    {
                        // 2点間の距離と矢印のサイズからスケールを算出.
                        var scale = arrowImg.transform.lossyScale;
                        scale.y *= (distance * objScale) / arrowImg.bounds.size.y;
                        arrowImg.transform.localScale = scale;
                        arrowImg.transform.position = sp * objScale;
                        arrowImg.transform.rotation = Quaternion.Euler(0, 0, GetAngle(sp, ep) - 90);
                    }
                    else
                    {
                        arrowImg.transform.position = (sp + ep) / 2 * objScale;
                        arrowImg.transform.rotation = Quaternion.Euler(0, 0, GetAngle(sp, ep) - 90);
                    }

                }
                else if (start != null)
                {
                    // 1点指定時は位置のみ調整.
                    arrowImg.transform.position = (Vector3)start * objScale;
                }
            }
        }

        /// <summary>
        /// 2点間の角度取得.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private float GetAngle(Vector2 p1, Vector2 p2)
        {
            float dx = p2.x - p1.x;
            float dy = p2.y - p1.y;
            return Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// 基準点からの最短距離の取得.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="src"></param>
        /// <returns></returns>
        public float GetMinDistance(Vector3? center, Dictionary<RowData, Vector3> src)
        {
            float average = 0;
            if (src != null && src.Count > 0 && center != null)
            {
                average = float.MaxValue;
                foreach (var v in src.Values)
                {
                    average = Mathf.Min(Vector3.Distance((Vector3)center, v), average);
                }
            }
            return average;
        }
    }
}