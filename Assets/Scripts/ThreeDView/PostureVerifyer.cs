using UnityEngine;
using Assets.Scripts.Common.Config;
using Assets.Scripts.Common;
using System;
using UnityEngine.Events;
using System.Collections;
using Assets.Scripts.Database.DataRow;
using System.Collections.Generic;

namespace Assets.Scripts.ThreeDView
{
    public class PostureVerifyer : MonoBehaviour
    {
        [Serializable]
        public class PostureVerifyerEvent : UnityEvent<PostureVerifyer> { }

        [SerializeField]
        private PostureVerifyerEvent _onPostureVerifyCompleted = default;
        public PostureVerifyerEvent onPostureVerifyCompleted {
            get {
                return _onPostureVerifyCompleted;
            }
        }

        private Measurement _measurement;
        private AvatarController _avatar;

        private Dictionary<PostureVerifyPoint, Result> _results;
        public Dictionary<PostureVerifyPoint, Result> results {
            get {
                return _results;
            }
            private set {
                _results = value;
                onPostureVerifyCompleted.Invoke(this);
            }
        }

        private const float MaxPercentage = 100f;

        private bool _isRunning = false;

        public void OnAvatarUpdated(AvatarController avatar)
        {
            _avatar = avatar;
             StartCoroutine(WaitDataLoadProcess());
        }

        public void OnUpdateMeasurementData(MeasurementDataLoader loader)
        {
            _measurement = loader.Measurement;
            StartCoroutine(WaitDataLoadProcess());
        }

        IEnumerator WaitDataLoadProcess()
        {
            if (_isRunning)
            {
                yield break;
            }
            _isRunning = true;

            // 必要なデータのロード完了まで待機.
            while (_measurement == null || _avatar == null)
            {
                yield return null;
            }

            Verify(_avatar, _measurement);

            _isRunning = false;
        }

        /// <summary>
        /// 全アングルの姿勢検証.
        /// </summary>
        /// <param name="avatar"></param>
        /// <param name="measurement"></param>
        protected void Verify(AvatarController avatar, Measurement measurement)
        {
            var resultMap = new Dictionary<PostureVerifyPoint, Result>();

            VerifyPostureOfSideAngle(avatar, measurement, ref resultMap);
            VerifyPostureOfFrontAngle(avatar, measurement, ref resultMap);
            VerifyPostureOfTopAngle(avatar, measurement, ref resultMap);
            VerifyPostureOfUnderAngle(avatar, measurement, ref resultMap);

            results = resultMap;
        }

        /// <summary>
        /// 横アングルにおける姿勢検証.
        /// </summary>
        /// <param name="avatar"></param>
        /// <param name="measurement"></param>
        /// <param name="resultMap"></param>
        private void VerifyPostureOfSideAngle(AvatarController avatar, Measurement measurement, ref Dictionary<PostureVerifyPoint, Result> resultMap)
        {
            // 1.重心Y軸ラインと耳の乖離.
            try
            {
                var point = PostureVerifyPoint.UpperInclinationRatio;
                var threshold = point.ToRangeThreshold();

                var leftEarlobe = (Vector3)avatar.GetPoint(AvatarBones.LeftEarlobe);
                var rightEarlobe = (Vector3)avatar.GetPoint(AvatarBones.RightEarlobe);
                var jushin = (Vector3)avatar.GetPoint(AvatarBones.Jushin);
                var leftDiff = (leftEarlobe.z - jushin.z);
                var rightDiff = (rightEarlobe.z - jushin.z);

                var diffLength = (leftDiff + rightDiff) / 2;
                var diffPerHeight = MaxPercentage * diffLength / measurement.correctedHeight;

                var ret = threshold.CompareTo(diffPerHeight);
                var basePoint = leftDiff > rightDiff ? leftEarlobe : rightEarlobe;

                var result = new Result
                {
                    point = point,
                    condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.ForwardTiltHead : PostureCondition.BackwardTiltHead),
                    focusPoints = new Vector3[] { basePoint },
                    rawValues = new float[] { diffPerHeight },
                    dispValues = new float[] { diffLength * AppConst.MeasurementDataScale }
                };
                result.AddBaseLinePoints(new Vector3[] { jushin, new Vector3(basePoint.x, basePoint.y, jushin.z) });
                result.AddMeasurementLinePoints(new Vector3[] { jushin, basePoint });

                resultMap[point] = result;
            }
            catch (Exception e)
            {
                DebugUtil.Log(e.Message);
            }

            // 2.重心Y軸ラインとヒザの乖離
            try
            {
                var point = PostureVerifyPoint.LowerInclinationRatio;
                var threshold = point.ToRangeThreshold();

                var leftPatella = (Vector3)avatar.GetPoint(AvatarBones.LeftPatella);
                var rightPatella = (Vector3)avatar.GetPoint(AvatarBones.RightPatella);
                var jushin = (Vector3)avatar.GetPoint(AvatarBones.Jushin);
                var leftDiff = (leftPatella.z - jushin.z);
                var rightDiff = (rightPatella.z - jushin.z);

                var diffLength = (leftDiff + rightDiff) / 2;
                var diffPerHeight = MaxPercentage * diffLength / measurement.correctedHeight;

                var ret = threshold.CompareTo(diffPerHeight);
                var basePoint = leftDiff > rightDiff ? leftPatella : rightPatella;

                var result = new Result
                {
                    point = point,
                    condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.ForwardTiltKnee : PostureCondition.BackwardTiltKnee),
                    focusPoints = new Vector3[] { basePoint },
                    rawValues = new float[] { diffPerHeight },
                    dispValues = new float[] { diffLength * AppConst.MeasurementDataScale }
                };
                result.AddBaseLinePoints(new Vector3[] { jushin, new Vector3(basePoint.x, basePoint.y, jushin.z) });
                result.AddMeasurementLinePoints(new Vector3[] { jushin, basePoint });
                resultMap[point] = result;
            }
            catch (Exception e)
            {
                DebugUtil.Log(e.Message);
            }

            // 3.骨盤の前後傾斜角度.
            try
            {
                var point = PostureVerifyPoint.PelvisInclinationAngle;
                var threshold = point.ToRangeThreshold();

                var hips = (Vector3)avatar.GetPoint(AvatarBones.Hips);
                var hipsRotation = ((Quaternion)avatar.GetRotation(AvatarBones.Hips)).eulerAngles.x;

                var ret = threshold.CompareTo(hipsRotation);
                var diff = threshold.Diff(hipsRotation);

                var result = new Result
                {
                    point = point,
                    condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.ForwardTiltPelvis : PostureCondition.BackwardTiltPelvis),
                    focusPoints = new Vector3[] { (Vector3)avatar.GetPoint(AvatarBones.Hips) },
                    rawValues = new float[] { hipsRotation },
                    dispValues = new float[] { diff }
                };

                var lineEndPoint = hips + new Vector3(0, 1 / AppConst.ObjLoadScale, 0);
                var measurementPoint = RotateAround(hipsRotation, hips, lineEndPoint, Vector3.right);
                var normalPoint = RotateAround(-diff, hips, measurementPoint, Vector3.right);
                
                result.AddBaseLinePoints(new Vector3[] { hips, normalPoint });
                result.AddMeasurementLinePoints(new Vector3[] { hips, measurementPoint });

                resultMap[point] = result;
            }
            catch (Exception e)
            {
                DebugUtil.Log(e.Message);
            }

            // 4.背骨(胸椎)角度の正常値からの誤差.
            try
            {
                var point = PostureVerifyPoint.ThoracicVertebraAngle;
                var threshold = point.ToRangeThreshold();

                var spine1 = (Vector3)avatar.GetPoint(AvatarBones.Spine1);
                var spine1Rotation = ((Quaternion)avatar.GetRotation(AvatarBones.Spine1)).eulerAngles.x;

                var ret = threshold.CompareTo(spine1Rotation);
                var diff = threshold.Diff(spine1Rotation);

                var result = new Result
                {
                    point = point,
                    condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.Stoop : PostureCondition.FlatBack),
                    focusPoints = new Vector3[] { (Vector3)avatar.GetPoint(AvatarBones.Spine1) },
                    rawValues = new float[] { spine1Rotation },
                    dispValues = new float[] { diff }
                };
                
                var lineEndPoint = spine1 + new Vector3(0, 1 / AppConst.ObjLoadScale, 0);
                var measurementPoint = RotateAround(spine1Rotation, spine1, lineEndPoint, Vector3.right);
                var normalPoint = RotateAround(-diff, spine1, measurementPoint, Vector3.right);

                result.AddBaseLinePoints(new Vector3[] { spine1, normalPoint });
                result.AddMeasurementLinePoints(new Vector3[] { spine1, measurementPoint });

                resultMap[point] = result;
            }
            catch (Exception e)
            {
                DebugUtil.Log(e.Message);
            }

            // 5.背骨(腰椎上部)角度の正常値からの誤差.
            try
            {
                var point = PostureVerifyPoint.LumbarVertebraAngle;
                var threshold = point.ToRangeThreshold();

                var spineWaist = (Vector3)avatar.GetPoint(AvatarBones.SpineWaist);
                var spineWaistRotation = ((Quaternion)avatar.GetRotation(AvatarBones.SpineWaist)).eulerAngles.x;

                var ret = threshold.CompareTo(spineWaistRotation);
                var diff = threshold.Diff(spineWaistRotation);

                var result = new Result
                {
                    point = point,
                    condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.LumbarFlat : PostureCondition.StomachProtruding),
                    focusPoints = new Vector3[] { (Vector3)avatar.GetPoint(AvatarBones.SpineWaist) },
                    rawValues = new float[] { spineWaistRotation },
                    dispValues = new float[] { diff }
                };

                var lineEndPoint = spineWaist + new Vector3(0, 1 / AppConst.ObjLoadScale, 0);
                var measurementPoint = RotateAround(spineWaistRotation, spineWaist, lineEndPoint, Vector3.right);
                var normalPoint = RotateAround(-diff, spineWaist, measurementPoint, Vector3.right);

                result.AddBaseLinePoints(new Vector3[] { spineWaist, normalPoint });
                result.AddMeasurementLinePoints(new Vector3[] { spineWaist, measurementPoint });

                resultMap[point] = result;
            }
            catch (Exception e)
            {
                DebugUtil.Log(e.Message);
            }

            // 6.首の湾曲角度の正常値からの誤差.
            try
            {
                var point = PostureVerifyPoint.NeckCurveAngle;
                var threshold = point.ToRangeThreshold();

                var neck2 = (Vector3)avatar.GetPoint(AvatarBones.Neck2);
                var neck2Rotation = ((Quaternion)avatar.GetRotation(AvatarBones.Neck2)).eulerAngles.x;

                var ret = threshold.CompareTo(neck2Rotation);
                var diff = threshold.Diff(neck2Rotation);

                var result = new Result
                {
                    point = point,
                    condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.StraightNeck : PostureCondition.NeckBackFlexion),
                    focusPoints = new Vector3[] { (Vector3)avatar.GetPoint(AvatarBones.Neck2) },
                    rawValues = new float[] { neck2Rotation },
                    dispValues = new float[] { diff }
                };

                var lineEndPoint = neck2 + new Vector3(0, 1 / AppConst.ObjLoadScale, 0);
                var measurementPoint = RotateAround(neck2Rotation, neck2, lineEndPoint, Vector3.right);
                var normalPoint = RotateAround(-diff, neck2, measurementPoint, Vector3.right);

                result.AddBaseLinePoints(new Vector3[] { neck2, normalPoint });
                result.AddMeasurementLinePoints(new Vector3[] { neck2, measurementPoint });

                resultMap[point] = result;
            }
            catch (Exception e)
            {
                DebugUtil.Log(e.Message);
            }
        }
        
        /// <summary>
        /// 前面アングルにおける姿勢検証.
        /// </summary>
        /// <param name="avatar"></param>
        /// <param name="measurement"></param>
        /// <param name="resultMap"></param>
        private void VerifyPostureOfFrontAngle(AvatarController avatar, Measurement measurement, ref Dictionary<PostureVerifyPoint, Result> resultMap)
        {
            // 1.重心Y軸ラインと頭頂との乖離.
            try
            {
                var point = PostureVerifyPoint.BodyInclinationRatio;
                var threshold = point.ToRangeThreshold();

                var head = (Vector3)avatar.GetPoint(AvatarBones.Head);
                var jushin = (Vector3)avatar.GetPoint(AvatarBones.Jushin);

                var diffLength = head.x - jushin.x;
                var diffPerHeight = MaxPercentage * diffLength / measurement.correctedHeight;

                var ret = threshold.CompareTo(diffPerHeight);
                
                var result = new Result
                {
                    point = point,
                    condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.LeftTiltBody : PostureCondition.RightTiltBody),
                    focusPoints = new Vector3[] { (head + jushin) / 2 },
                    rawValues = new float[] { diffPerHeight },
                    dispValues = new float[] { diffLength * AppConst.MeasurementDataScale }
                };
                result.AddBaseLinePoints(new Vector3[] { new Vector3(head.x, 0, 0), new Vector3(head.x, head.y, 0) });
                result.AddMeasurementLinePoints(new Vector3[] { new Vector3(head.x, jushin.y, 0), new Vector3(jushin.x, head.y, 0) });
                resultMap[point] = result;
            }
            catch (Exception e)
            {
                DebugUtil.Log(e.Message);
            }

            // 2.大腿骨大転子から膝頭までの両足間の差 （右ー左）.
            try
            {
                var point = PostureVerifyPoint.UpperLegLengthRatio;
                var threshold = point.ToRangeThreshold();
                
                var leftGreaterTrochanter = (Vector3)avatar.GetPoint(AvatarBones.LeftGreaterTrochanter);
                var leftPatella = (Vector3)avatar.GetPoint(AvatarBones.LeftPatella);
                var leftLength = Vector3.Distance(leftGreaterTrochanter, leftPatella);

                var rightGreaterTrochanter = (Vector3)avatar.GetPoint(AvatarBones.RightGreaterTrochanter);
                var rightPatella = (Vector3)avatar.GetPoint(AvatarBones.RightPatella);
                var rightLength = Vector3.Distance(rightGreaterTrochanter, rightPatella);

                var diffLength = rightLength - leftLength;
                var diffPerHeight = MaxPercentage * diffLength / measurement.correctedHeight;

                var ret = threshold.CompareTo(diffPerHeight);

                var result = new Result
                {
                    point = point,
                    condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.LeftThighShort : PostureCondition.RightThighShort),
                    focusPoints = new Vector3[] { (leftGreaterTrochanter + rightGreaterTrochanter) / 2 },
                    rawValues = new float[] { diffPerHeight },
                    dispValues = new float[] { diffLength * AppConst.MeasurementDataScale }
                };
                result.AddBaseLinePoints(new Vector3[] { leftGreaterTrochanter, leftPatella });
                result.AddBaseLinePoints(new Vector3[] { rightGreaterTrochanter, rightPatella });
                resultMap[point] = result;
            }
            catch (Exception e)
            {
                DebugUtil.Log(e.Message);
            }

            // 3.膝頭から足首までの高さの両足間の差.
            try
            {
                var point = PostureVerifyPoint.LowerLegLengthRatio;
                var threshold = point.ToRangeThreshold();

                var leftPatella = (Vector3)avatar.GetPoint(AvatarBones.LeftPatella);
                var leftAnkle = (Vector3)avatar.GetPoint(AvatarBones.LeftAnkle);
                var leftLength = Vector3.Distance(leftPatella, leftAnkle);

                var rightPatella = (Vector3)avatar.GetPoint(AvatarBones.RightPatella);
                var rightAnkle = (Vector3)avatar.GetPoint(AvatarBones.RightAnkle);
                var rightLength = Vector3.Distance(rightPatella, rightAnkle);

                var diffLength = rightLength - leftLength;
                var diffPerHeight = MaxPercentage * diffLength / measurement.correctedHeight;

                var ret = threshold.CompareTo(diffPerHeight);

                var result = new Result
                {
                    point = point,
                    condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.LeftLowerLegLengthShort : PostureCondition.RightLowerLegLengthShort),
                    focusPoints = new Vector3[] { ((leftPatella + rightPatella) / 2 + (leftAnkle + rightAnkle) / 2) / 2 },
                    rawValues = new float[] { diffPerHeight },
                    dispValues = new float[] { diffLength * AppConst.MeasurementDataScale }
                };
                result.AddBaseLinePoints(new Vector3[] { leftPatella, leftAnkle });
                result.AddBaseLinePoints(new Vector3[] { rightPatella, rightAnkle });
                resultMap[point] = result;
            }
            catch (Exception e)
            {
                DebugUtil.Log(e.Message);
            }

            // 4.肩峰の左右の高さの差（右ー左）.
            try
            {
                var point = PostureVerifyPoint.ShoulderInclinationRatio;
                var threshold = point.ToRangeThreshold();

                var left = (Vector3)avatar.GetPoint(AvatarBones.LeftAcromion);
                var right = (Vector3)avatar.GetPoint(AvatarBones.RightAcromion);

                var diffLength = right.y - left.y;
                var diffPerHeight = MaxPercentage * diffLength / measurement.correctedHeight;

                var ret = threshold.CompareTo(diffPerHeight);

                var result = new Result
                {
                    point = point,
                    condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.LeftTiltAcromion : PostureCondition.RightTiltAcromion),
                    focusPoints = new Vector3[] { (right + left) / 2 },
                    rawValues = new float[] { diffPerHeight },
                    dispValues = new float[] { diffLength * AppConst.MeasurementDataScale }
                };
                result.AddBaseLinePoints(new Vector3[] { right, new Vector3(left.x, right.y, right.z) });
                result.AddMeasurementLinePoints(new Vector3[] { right, left });
                resultMap[point] = result;
            }
            catch (Exception e)
            {
                DebugUtil.Log(e.Message);
            }

            // 5.左右の上前腸骨棘の高さの差（右ー左）.
            try
            {
                var point = PostureVerifyPoint.PelvisInclinationRatio;
                var threshold = point.ToRangeThreshold();

                var right = (Vector3)avatar.GetPoint(AvatarBones.RightDownScale2);
                var left = (Vector3)avatar.GetPoint(AvatarBones.LeftDownScale2);

                var diffLength = right.y - left.y;
                var diffPerHeight = MaxPercentage * diffLength / measurement.correctedHeight;

                var ret = threshold.CompareTo(diffPerHeight);

                var result = new Result
                {
                    point = point,
                    condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.LeftTiltPelvis : PostureCondition.RightTiltPelvis),
                    focusPoints = new Vector3[] { (right + left) / 2 },
                    rawValues = new float[] { diffPerHeight },
                    dispValues = new float[] { diffLength * AppConst.MeasurementDataScale }
                };
                result.AddBaseLinePoints(new Vector3[] { right, new Vector3(left.x, right.y, right.z) });
                result.AddMeasurementLinePoints(new Vector3[] { right, left });
                resultMap[point] = result;
            }
            catch (Exception e)
            {
                DebugUtil.Log(e.Message);
            }

            // 6.ヒザ関節の外反角度（FTA）.
            try
            {
                var point = PostureVerifyPoint.KneeJointAngle;
                var threshold = point.ToRangeThreshold();

                var leftGreaterTrochanter = (Vector3)avatar.GetPoint(AvatarBones.LeftGreaterTrochanter);
                var leftPatella = (Vector3)avatar.GetPoint(AvatarBones.LeftPatella);
                var leftAnkle = (Vector3)avatar.GetPoint(AvatarBones.LeftAnkle);
                var leftFTA = GetFTAAngle(leftGreaterTrochanter, leftPatella, leftAnkle);
                var leftResult = threshold.CompareTo(leftFTA);
                var leftDiff = threshold.Diff(leftFTA);

                var rightGreaterTrochanter = (Vector3)avatar.GetPoint(AvatarBones.RightGreaterTrochanter);
                var rightPatella = (Vector3)avatar.GetPoint(AvatarBones.RightPatella);
                var rightAnkle = (Vector3)avatar.GetPoint(AvatarBones.RightAnkle);
                var rightFTA = GetFTAAngle(rightGreaterTrochanter, rightPatella, rightAnkle);
                var rightResult = threshold.CompareTo(rightFTA);
                var rightDiff = threshold.Diff(rightFTA);
                
                var result = new Result
                {
                    point = point,
                    condition = (leftResult > 0 || rightResult > 0) ? PostureCondition.BowLegs :
                                                                      ((leftResult < 0 || rightResult < 0) ? PostureCondition.KnockKnees : PostureCondition.Normal),
                };

                var focusPointList = new List<Vector3>();
                var rawValueList = new List<float>();
                var dispValueList = new List<float>();
                if (leftResult != 0)
                {
                    focusPointList.Add(leftPatella);
                    rawValueList.Add(leftFTA);
                    dispValueList.Add(leftDiff);

                    result.AddBaseLinePoints(new Vector3[] { RotateAround(leftDiff / 2, leftPatella, leftGreaterTrochanter, Vector3.forward),
                                                             leftPatella,
                                                             RotateAround(-leftDiff / 2, leftPatella, leftAnkle, Vector3.forward)});
                    result.AddMeasurementLinePoints(new Vector3[] { leftGreaterTrochanter, leftPatella, leftAnkle });
                }

                if (rightResult != 0)
                {
                    focusPointList.Add(rightPatella);
                    rawValueList.Add(rightFTA);
                    dispValueList.Add(rightDiff);
                    result.AddBaseLinePoints(new Vector3[] { RotateAround(-rightDiff / 2, rightPatella, rightGreaterTrochanter, Vector3.forward),
                                                             rightPatella,
                                                             RotateAround(rightDiff / 2, rightPatella, rightAnkle, Vector3.forward) });

                    result.AddMeasurementLinePoints(new Vector3[] { rightGreaterTrochanter, rightPatella, rightAnkle });
                }

                result.focusPoints = focusPointList.ToArray();
                result.rawValues = rawValueList.ToArray();
                result.dispValues = dispValueList.ToArray();
                
                resultMap[point] = result;
            }
            catch (Exception e)
            {
                DebugUtil.Log(e.Message);
            }
        }
        
        /// <summary>
        /// 上アングルにおける姿勢検証.
        /// </summary>
        /// <param name="avatar"></param>
        /// <param name="measurement"></param>
        /// <param name="resultMap"></param>
        private void VerifyPostureOfTopAngle(AvatarController avatar, Measurement measurement, ref Dictionary<PostureVerifyPoint, Result> resultMap)
        {
            // 1.左右の鎖骨角度の正常角度からの差.
            try
            {
                var point = PostureVerifyPoint.ShoulderDistortionAngle;
                var threshold = point.ToRangeThreshold();

                var leftArm = (Vector3)avatar.GetPoint(AvatarBones.LeftArm);
                var leftShoulder = (Vector3)avatar.GetPoint(AvatarBones.LeftShoulder);
                var leftRotation = ((Quaternion)avatar.GetRotation(AvatarBones.LeftShoulder)).eulerAngles.y;
                var leftResult = threshold.CompareTo(leftRotation);

                var rightArm = (Vector3)avatar.GetPoint(AvatarBones.RightArm);
                var rightShoulder  = (Vector3)avatar.GetPoint(AvatarBones.RightShoulder);
                var rightRotation = ((Quaternion)avatar.GetRotation(AvatarBones.RightShoulder)).eulerAngles.y;
                var rightResult = threshold.CompareTo(rightRotation);
                
                var leftDiff = threshold.Diff(leftRotation);
                var rightDiff = threshold.Diff(rightRotation);
                
                var result = new Result
                {
                    point = point,
                    condition = ((leftResult > 0 && rightResult > 0) ? PostureCondition.RightTwistShoulder :
                                                                       (leftResult < 0 && rightResult < 0) ? PostureCondition.LeftTwistShoulder : PostureCondition.Normal),
                    focusPoints = new Vector3[] { (leftShoulder + leftArm) / 2,  (rightShoulder + rightArm) / 2 },
                    rawValues = new float[] { leftRotation, rightRotation },
                    dispValues = new float[] { leftDiff, rightDiff }
                };

                var leftArmNormal = RotateAround(-leftDiff, leftShoulder, leftArm, Vector3.up);
                var rightArmNormal = RotateAround(-rightDiff, rightShoulder, rightArm, Vector3.up);
                
                result.AddBaseLinePoints(new Vector3[] { leftShoulder, new Vector3(leftArmNormal.x, leftShoulder.y, leftArmNormal.z) });
                result.AddBaseLinePoints(new Vector3[] { rightShoulder, new Vector3(rightArmNormal.x, rightShoulder.y, rightArmNormal.z) });
                
                result.AddMeasurementLinePoints(new Vector3[] { leftShoulder, leftArm });
                result.AddMeasurementLinePoints(new Vector3[] { rightShoulder, rightArm });
                
                resultMap[point] = result;
            }
            catch (Exception e)
            {
                DebugUtil.Log(e.Message);
            }

            // 2.首の左右回転角度.
            try {
                var point = PostureVerifyPoint.NeckDistortionAngle;
                var threshold = point.ToRangeThreshold();

                var neck3 = (Vector3)avatar.GetPoint(AvatarBones.Neck3);
                var neck3Rotation = ((Quaternion)avatar.GetRotation(AvatarBones.Neck3)).eulerAngles.y;

                var ret = threshold.CompareTo(neck3Rotation);
                var diff = threshold.Diff(neck3Rotation);

                var result = new Result
                {
                    point = point,
                    condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.RightTwistNeck : PostureCondition.LeftTwistNeck),
                    focusPoints = new Vector3[] { (Vector3)avatar.GetPoint(AvatarBones.Neck3) },
                    rawValues = new float[] { neck3Rotation },
                    dispValues = new float[] { diff }
                };
                
                var linePoint = new Vector3(neck3.x, neck3.y, neck3.x + 200);
                result.AddBaseLinePoints(new Vector3[] { neck3, linePoint });
                result.AddMeasurementLinePoints(new Vector3[] { neck3, RotateAround(diff, neck3, linePoint, Vector3.up) });

                resultMap[PostureVerifyPoint.NeckDistortionAngle] = result;
            }
            catch (Exception e)
            {
                DebugUtil.Log(e.Message);
            }
        }

        /// <summary>
        /// 下アングルにおける姿勢検証.
        /// </summary>
        /// <param name="avatar"></param>
        /// <param name="measurement"></param>
        /// <param name="resultMap"></param>
        private void VerifyPostureOfUnderAngle(AvatarController avatar, Measurement measurement, ref Dictionary<PostureVerifyPoint, Result> resultMap)
        {
            // 1.腰の左右を結ぶ線と、肩の左右を結ぶ線の角度差（腰に対する肩）.
            try
            {
                var point = PostureVerifyPoint.UpperBodyDistortionAngle;
                var threshold = point.ToRangeThreshold();

                var jushinRotationY = ((Quaternion)avatar.GetRotation(AvatarBones.Jushin)).eulerAngles.y;

                var leftAcromion = (Vector3)avatar.GetPoint(AvatarBones.LeftAcromion);
                var rightAcromion = (Vector3)avatar.GetPoint(AvatarBones.RightAcromion);
                var leftRightDiff = leftAcromion - rightAcromion;
                var acromionAngle = Mathf.Atan2(leftRightDiff.z, leftRightDiff.x);

                var acromionAngleDiff = acromionAngle - jushinRotationY;
                
                var ret = threshold.CompareTo(acromionAngleDiff);

                var result = new Result
                {
                    point = point,
                    condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.RightTwistWaist : PostureCondition.LeftTwistWaist),
                    focusPoints = new Vector3[] { (leftAcromion + rightAcromion) / 2 },
                    rawValues = new float[] { acromionAngleDiff },
                    dispValues = new float[] { threshold.Diff(acromionAngleDiff) }
                };
                var halfLength = Vector3.Distance(leftAcromion, rightAcromion) / 2;
                var z = halfLength * Mathf.Sin(jushinRotationY * Mathf.Deg2Rad);
                var baseStartPoint = new Vector3(-halfLength, rightAcromion.y, -z);
                var baseEndPoint = new Vector3(halfLength, rightAcromion.y, z);
                result.AddBaseLinePoints(new Vector3[] { baseStartPoint, baseEndPoint });
                result.AddMeasurementLinePoints(new Vector3[] { leftAcromion, rightAcromion });
                resultMap[point] = result;
            }
            catch (Exception e)
            {
                DebugUtil.Log(e.Message);
            }

            // 2.膝頭の前後差（右ー左）.
            try {
                var point = PostureVerifyPoint.KneeAnteroposteriorDiffRatio;
                var threshold = point.ToRangeThreshold();

                var left = (Vector3)avatar.GetPoint(AvatarBones.LeftPatella);
                var right = (Vector3)avatar.GetPoint(AvatarBones.RightPatella);

                var diffLength = right.z - left.z;
                var diffPerHeight = MaxPercentage * diffLength / measurement.correctedHeight;

                var ret = threshold.CompareTo(diffPerHeight);

                var result = new Result
                {
                    point = point,
                    condition = ret == 0 ? PostureCondition.Normal : (ret > 0 ? PostureCondition.RightKneeProtruding : PostureCondition.LeftKneeProtruding),
                    focusPoints = new Vector3[] { (right + left) / 2 },
                    rawValues = new float[] { diffPerHeight },
                    dispValues = new float[] { diffLength * AppConst.MeasurementDataScale }
                };
                result.AddBaseLinePoints(new Vector3[] { right, new Vector3(left.x, right.y, right.z) });
                result.AddMeasurementLinePoints(new Vector3[] { right, left });
                resultMap[point] = result;
            }
            catch (Exception e)
            {
                DebugUtil.Log(e.Message);
            }
        }

        /// <summary>
        /// 膝関節のFTA角度の取得.
        /// </summary>
        /// <param name="greaterTrochanter"></param>
        /// <param name="patella"></param>
        /// <param name="ankle"></param>
        /// <returns></returns>
        private float GetFTAAngle(Vector3 greaterTrochanter, Vector3 patella, Vector3 ankle)
        {
            greaterTrochanter.z = 0;
            patella.z = 0;
            ankle.z = 0;
            return Vector3.Angle(greaterTrochanter - patella, ankle - patella);
        }

        /// <summary>
        /// 任意のポイント(target)をpivot周りにangle分回転した際の座標取得.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="pivot"></param>
        /// <param name="target"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        private Vector3 RotateAround(float angle, Vector3 pivot, Vector3 target, Vector3 axis)
        {
            return pivot + Quaternion.AngleAxis(angle, axis) * (target - pivot);
        }

        public class Result
        {
            public PostureVerifyPoint _point;
            public PostureVerifyPoint point {
                get {
                    return _point;
                }
                set {
                    _point = value;
                }
            }

            public PostureCondition _condition = PostureCondition.Normal;
            public PostureCondition condition {
                get {
                    return _condition;
                }
                set {
                    _condition = value;
                }
            }

            public RangeThreshold threshold {
                get {
                    return point.ToRangeThreshold();
                }
            }

            public string description {
                get {
                    return condition.ToDescription(dispValues.Length > 0 ? Mathf.Abs(dispValues[0]) : (float?)null,
                                                   dispValues.Length > 1 ? Mathf.Abs(dispValues[1]) : (float?)null);
                }
            }

            public Vector3[] _focusPoints;
            public Vector3[] focusPoints {
                get {
                    return _focusPoints;
                }
                set {
                    _focusPoints = value;

                    var length = value.Length;
                    var total = Vector3.zero;

                    _scaledFocusPoints = new Vector3[length];
                    for (var i = 0; i < length; i++)
                    {
                        _scaledFocusPoints[i] = _focusPoints[i] * AppConst.ObjLoadScale;
                        total += _focusPoints[i];
                    }

                    _midFocusPoint = total / length;
                }
            }

            private Vector3 _midFocusPoint;
            public Vector3 midFocusPoint {
                get {
                    return _midFocusPoint;
                }
            }
            
            public Vector3 midScaledFocusPoint {
                get {
                    return _midFocusPoint * AppConst.ObjLoadScale;
                }
            }
            
            private float[] _rawValues = new float[] {};
            public float[] rawValues {
                get {
                    return _rawValues;
                }
                set {
                    _rawValues = value;
                }
            }

            private float[] _dispValues = new float[] {};
            public float[] dispValues {
                get {
                    return _dispValues;
                }
                set {
                    _dispValues = value;
                }
            }

            private List<Vector3[]> _scaledBaseLinePoints = new List<Vector3[]>();
            public List<Vector3[]> scaledBaseLinePoints {
                get {
                    return _scaledBaseLinePoints;
                }
            }

            private List<Vector3[]> _scaledMeasurementLinePoints = new List<Vector3[]>();
            public List<Vector3[]> scaledMeasurementLinePoints {
                get {
                    return _scaledMeasurementLinePoints;
                }
            }

            private Vector3[] _scaledFocusPoints;
            public Vector3[] scaledFocusPoints {
                get {
                    return _scaledFocusPoints;
                }
            }

            public void AddBaseLinePoints(Vector3[] linePoints)
            {
                _scaledBaseLinePoints.Add(GetScaledPoints(linePoints));
            }

            public void AddMeasurementLinePoints(Vector3[] linePoints)
            {
                _scaledMeasurementLinePoints.Add(GetScaledPoints(linePoints));
            }

            private static Vector3[] GetScaledPoints(Vector3[] srcPoints)
            {
                if (srcPoints == null)
                {
                    return null;
                }
                var count = srcPoints.Length;
                var scaledPoints = new Vector3[count];

                for (int i = 0; i < count; i++)
                {
                    scaledPoints[i] = srcPoints[i] * AppConst.ObjLoadScale;
                }
                return scaledPoints;
            }

            public override string ToString()
            {
                var dispValueText = "";
                if (dispValues != null)
                {
                    foreach (var value in dispValues)
                    {
                        dispValueText = string.Concat(dispValueText, value.ToString() + ",");
                    }
                }

                var rawValueText = "";
                if (rawValues != null)
                {
                    foreach (var value in rawValues)
                    {
                        rawValueText = string.Concat(rawValueText, value.ToString() + ",");
                    }
                }
                return string.Format("{0}, {1}, {2}, {3}", point.ToRangeThreshold(), dispValueText, point.ToDescription(), rawValueText);
            }
        }
    }
}
