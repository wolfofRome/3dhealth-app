using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Assets.Scripts.Common;
using Assets.Scripts.Common.Config;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// ReSharper disable PossibleInvalidOperationException
// ReSharper disable UnusedMember.Local
// ReSharper disable ConvertToConstant.Local
// ReSharper disable RedundantAssignment

namespace Assets.Scripts.ThreeDView
{
    public enum RowData
    {
        Left = 0,
        Right = 1,
        Forward = 2,
        Back = 3,
        Up = 4,
        Down = 5
    }

    public enum AvatarBones
    {
        Hips = 0,
        Jushin = 1,
        LeftUpLeg = 2,
        LeftDownScale1 = 3,
        LeftDownScale2 = 4,
        LeftGreaterTrochanter = 5,
        LeftLeg = 6,
        LeftPatella = 7,
        LeftFoot = 8,
        LeftAnkle = 9,
        LeftToe = 10,
        RightUpLeg = 11,
        RightDownScale1 = 12,
        RightDownScale2 = 13,
        RightGreaterTrochanter = 14,
        RightLeg = 15,
        RightPatella = 16,
        RightFoot = 17,
        RightAnkle = 18,
        RightToe = 19,
        SpineWaist = 20,
        Spine1 = 21,
        Spine2 = 22,
        Neck1 = 23,
        Neck2 = 24,
        Neck3 = 25,
        LeftEarlobe = 26,
        RightEarlobe = 27,
        Head = 28,
        LeftShoulder = 29,
        LeftAcromion = 30,
        LeftArm = 31,
        LeftUpScale1 = 32,
        LeftForeArm = 33,
        LeftHand = 34,
        LeftHand2 = 35,
        RightShoulder = 36,
        RightAcromion = 37,
        RightArm = 38,
        RightUpScale1 = 39,
        RightForeArm = 40,
        RightHand = 41,
        RightHand2 = 42,
        Root = 43
    }

    public static class AvatarBonesExtension
    {
        private static readonly Dictionary<AvatarBones, string> AvatarBonesNameMap = new Dictionary<AvatarBones, string>
        {
            {AvatarBones.Hips, "hips"},
            {AvatarBones.Jushin, "Jushin"},
            {AvatarBones.LeftUpLeg, "LeftUpLeg"},
            {AvatarBones.LeftDownScale1, "LeftDownScale1"},
            {AvatarBones.LeftDownScale2, "LeftDownScale2"},
            {AvatarBones.LeftGreaterTrochanter, "Daitenshi_L"},
            {AvatarBones.LeftLeg, "LeftLeg"},
            {AvatarBones.LeftPatella, "Hizagashira_L"},
            {AvatarBones.LeftFoot, "LeftFoot"},
            {AvatarBones.LeftAnkle, "Ashikubi_L"},
            {AvatarBones.LeftToe, "LeftToeBase_(1)"},
            {AvatarBones.RightUpLeg, "RightUpLeg"},
            {AvatarBones.RightDownScale1, "RightDownScale1"},
            {AvatarBones.RightDownScale2, "RightDownScale2"},
            {AvatarBones.RightGreaterTrochanter, "Daitenshi_R"},
            {AvatarBones.RightLeg, "RightLeg"},
            {AvatarBones.RightPatella, "Hizagashira_R"},
            {AvatarBones.RightFoot, "RightFoot"},
            {AvatarBones.RightAnkle, "Ashikubi_R"},
            {AvatarBones.RightToe, "RightToeBase_(1)"},
            {AvatarBones.SpineWaist, "spine"},
            {AvatarBones.Spine1, "spine1"},
            {AvatarBones.Spine2, "spine2"},
            {AvatarBones.Neck1, "neck_(1)"},
            {AvatarBones.Neck2, "neck_(2)"},
            {AvatarBones.Neck3, "neck_(3)"},
            {AvatarBones.LeftEarlobe, null},
            {AvatarBones.RightEarlobe, null},
            {AvatarBones.Head, "head"},
            {AvatarBones.LeftShoulder, "LeftShoulder"},
            {AvatarBones.LeftAcromion, null},
            {AvatarBones.LeftArm, "LeftArm"},
            {AvatarBones.LeftUpScale1, "LeftUpScale1"},
            {AvatarBones.LeftForeArm, "LeftForeArm"},
            {AvatarBones.LeftHand, "LeftHand_(1)"},
            {AvatarBones.LeftHand2, "LeftHand_(2)"},
            {AvatarBones.RightShoulder, "RightShoulder"},
            {AvatarBones.RightAcromion, null},
            {AvatarBones.RightArm, "RightArm"},
            {AvatarBones.RightUpScale1, "RightUpScale1"},
            {AvatarBones.RightForeArm, "RightForeArm"},
            {AvatarBones.RightHand, "RightHand_(1)"},
            {AvatarBones.RightHand2, "RightHand_(2)"},
            {AvatarBones.Root, "bone" }
        };

        public static string GetName(this AvatarBones bone)
        {
            return AvatarBonesNameMap.ContainsKey(bone) ? AvatarBonesNameMap[bone] : null;
        }
    }

    public class AvatarController : MonoBehaviour
    {
        [Serializable]
        public class AvatarControllerEvent : UnityEvent<AvatarController> { }

        [FormerlySerializedAs("OnAvatarUpdated")]
        public AvatarControllerEvent onAvatarUpdated;

        private bool _newType;

        private Transform[] _bones;

        private Vector3 _initialPosition;

        private float _xOffset, _yOffset, _zOffset;

        private Vector3[] _objPoints;
        private Dictionary<RowData, Vector3>[] _objSrcPoints;
        private Vector3 _objOffset = Vector3.zero;

        private const float BodyScale = AppConst.ObjLoadScale;
        private int _rowsOffset = -1;

        public void Awake()
        {
            if (_bones != null) return;
            
            ResetManekinObject();
        }

        private void ResetManekinObject()
        {
            _bones = new Transform[Enum.GetNames(typeof(AvatarBones)).Length];
            _boneRootTransform = null;
            _transformListCache.Clear();

            MapBones();
        }

        public void UpdateAvatar(string[] objInfo, Vector3 objOffset)
        {
            _objOffset = objOffset;

            if (_rowsOffset < 0)
            {
                Regex r = new Regex(@"v -?\d+.\d+ -?\d+.\d+ -?\d+.\d+");
                int offset = 0;
                foreach (string objLineStr in objInfo)
                {
                    if (r.IsMatch(objLineStr)) break;
                    offset++;
                }
                _rowsOffset = offset - 1;
            }
            _objSrcPoints = LoadSrcPoints(objInfo);

            int count = Enum.GetNames(typeof(AvatarBones)).Length;

            _objPoints = new Vector3[Enum.GetNames(typeof(AvatarBones)).Length];

            for (int i = 0; i < count; i++)
            {
                AvatarBones bone = (AvatarBones)i;
                int?[] row = AvatarBonesObjRows[bone];

                switch (bone)
                {
                    case AvatarBones.Head:
                    case AvatarBones.LeftEarlobe:
                    case AvatarBones.RightEarlobe:
                    case AvatarBones.LeftAcromion:
                    case AvatarBones.RightAcromion:
                        {
                            Vector3 v = ParseVertexData((int)row[(int)RowData.Up], objInfo);
                            _objPoints[i] = v;
                        }
                        break;
                    case AvatarBones.Neck1:
                        _objPoints[i] = CalcSpineLineVertex(row, objInfo, 0.5f, 0.5f);
                        break;
                    case AvatarBones.LeftShoulder:
                    case AvatarBones.RightShoulder:
                        {
                            float sumZ = 0.0f;

                            Vector3 v = ParseVertexData((int)row[(int)RowData.Forward], objInfo);
                            float forwardX = v.x;
                            float forwardY = v.y;

                            sumZ += v.z;

                            v = ParseVertexData((int)row[(int)RowData.Up], objInfo);
                            sumZ += v.z;

                            _objPoints[i] = new Vector3(forwardX, forwardY, sumZ / 2.0f);
                        }
                        break;
                    case AvatarBones.LeftArm:
                    case AvatarBones.LeftUpScale1:
                    case AvatarBones.RightArm:
                    case AvatarBones.RightUpScale1:
                        {
                            float sumX = 0.0f;
                            float sumY = 0.0f;
                            float sumZ = 0.0f;

                            Vector3 v = ParseVertexData((int)row[(int)RowData.Forward], objInfo);
                            sumX += v.x;
                            sumY += v.y;
                            sumZ += v.z;

                            v = ParseVertexData((int)row[(int)RowData.Back], objInfo);
                            sumX += v.x;
                            sumY += v.y;
                            sumZ += v.z;

                            _objPoints[i] = new Vector3(sumX / 2.0f, sumY / 2.0f, sumZ / 2.0f);
                        }
                        break;
                    case AvatarBones.LeftForeArm:
                    case AvatarBones.RightForeArm:
                        _objPoints[i] = CalcLimbVertex(row, objInfo, 0.6f);
                        break;
                    case AvatarBones.LeftHand2:
                    case AvatarBones.RightHand2:
                        {
                            Vector3 v = ParseVertexData((int)row[(int)RowData.Forward], objInfo);
                            _objPoints[i] = v;
                        }
                        break;
                    case AvatarBones.Spine2:
                        _objPoints[i] = CalcSpineLineVertex(row, objInfo, 0.5f, 0.6f);
                        break;
                    case AvatarBones.Spine1:
                        _objPoints[i] = CalcSpineLineVertex(row, objInfo, 0.5f, 0.7f);
                        break;
                    case AvatarBones.SpineWaist:
                        _objPoints[i] = CalcSpineLineVertex(row, objInfo, 0.5f, 0.6f);
                        break;
                    case AvatarBones.LeftUpLeg:
                    case AvatarBones.LeftDownScale1:
                        {
                            Vector3 v = ParseVertexData((int)row[(int)RowData.Forward], objInfo);
                            float forwardX = v.x;
                            float forwardY = v.y;

                            v = ParseVertexData((int)row[(int)RowData.Left], objInfo);
                            float leftZ = v.z;

                            _objPoints[i] = new Vector3(forwardX, forwardY, leftZ);
                        }
                        break;
                    case AvatarBones.RightUpLeg:
                    case AvatarBones.RightDownScale1:
                        {
                            Vector3 v = ParseVertexData((int)row[(int)RowData.Forward], objInfo);
                            float forwardX = v.x;
                            float forwardY = v.y;

                            v = ParseVertexData((int)row[(int)RowData.Right], objInfo);
                            float rightZ = v.z;

                            _objPoints[i] = new Vector3(forwardX, forwardY, rightZ);
                        }
                        break;
                    case AvatarBones.LeftLeg:
                    case AvatarBones.RightLeg:
                        {
                            Vector3 v = ParseVertexData((int)row[(int)RowData.Forward], objInfo);
                            float forwardX = v.x;
                            float forwardY = v.y;
                            float forwardZ = v.z;

                            v = ParseVertexData((int)row[(int)RowData.Back], objInfo);
                            float backZ = v.z;

                            _objPoints[i] = new Vector3(forwardX, forwardY, Mathf.Lerp(forwardZ, backZ, 0.4f));
                        }
                        break;
                    case AvatarBones.LeftFoot:
                    case AvatarBones.RightFoot:
                        _objPoints[i] = CalcLimbVertex(row, objInfo, 0.45f);
                        break;
                    case AvatarBones.LeftToe:
                    case AvatarBones.RightToe:
                        {
                            Vector3 v = ParseVertexData((int)row[(int)RowData.Up], objInfo);
                            float upX = v.x;
                            float upY = v.y;

                            v = ParseVertexData((int)row[(int)RowData.Forward], objInfo);
                            float forwardZ = v.z;

                            _objPoints[i] = new Vector3(upX, upY, forwardZ);
                        }
                        break;
                    case AvatarBones.LeftDownScale2:
                    case AvatarBones.RightDownScale2:
                    case AvatarBones.Neck2:
                    case AvatarBones.Neck3:
                    case AvatarBones.LeftHand:
                    case AvatarBones.RightHand:
                    case AvatarBones.Hips:
                        {
                            int counter = 0;

                            float sumX = 0.0f;
                            float sumY = 0.0f;
                            float sumZ = 0.0f;

                            if (row[(int)RowData.Left] != null)
                            {
                                Vector3 v = ParseVertexData((int)row[(int)RowData.Left], objInfo);

                                sumX += v.x;
                                sumY += v.y;
                                sumZ += v.z;

                                counter++;
                            }
                            if (row[(int)RowData.Right] != null)
                            {
                                Vector3 v = ParseVertexData((int)row[(int)RowData.Right], objInfo);

                                sumX += v.x;
                                sumY += v.y;
                                sumZ += v.z;

                                counter++;
                            }
                            if (row[(int)RowData.Forward] != null)
                            {
                                Vector3 v = ParseVertexData((int)row[(int)RowData.Forward], objInfo);

                                sumX += v.x;
                                sumY += v.y;
                                sumZ += v.z;

                                counter++;
                            }
                            if (row[(int)RowData.Back] != null)
                            {
                                Vector3 v = ParseVertexData((int)row[(int)RowData.Back], objInfo);

                                sumX += v.x;
                                sumY += v.y;
                                sumZ += v.z;

                                counter++;
                            }
                            if (row[(int)RowData.Up] != null)
                            {
                                Vector3 v = ParseVertexData((int)row[(int)RowData.Up], objInfo);

                                sumX += v.x;
                                sumY += v.y;
                                sumZ += v.z;

                                counter++;
                            }
                            if (row[(int)RowData.Down] != null)
                            {
                                Vector3 v = ParseVertexData((int)row[(int)RowData.Down], objInfo);

                                sumX += v.x;
                                sumY += v.y;
                                sumZ += v.z;

                                counter++;
                            }

                            if (counter > 0)
                            {
                                _objPoints[i] = new Vector3(sumX / counter, sumY / counter, sumZ / counter);
                            }
                        }
                        break;
                    case AvatarBones.Root:
                        break;
                }

            }

#if DISPLAY_BONE_LINES
            for (int i = 0; i < count; i++)
            {
                AvatarBones bone = (AvatarBones)i;

                if (AvatarBonesObjRows[bone] != null)
                {
                    String name = Enum.GetNames(typeof(AvatarBones))[i];
                    DrawSphere(bone, _objPoints[i], Color.green, name);

                    int?[] row = AvatarBonesObjRows[bone];

                    if (row[(int)RowData.Left] != null)
                    {
                        DrawCube(bone, ParseVertexData((int)row[(int)RowData.Left], objInfo), Color.red, name + "_left");
                    }
                    if (row[(int)RowData.Right] != null)
                    {
                        DrawCube(bone, ParseVertexData((int)row[(int)RowData.Right], objInfo), Color.red, name + "_right");
                    }
                    if (row[(int)RowData.Up] != null)
                    {
                        DrawCube(bone, ParseVertexData((int)row[(int)RowData.Up], objInfo), Color.red, name + "_up");
                    }
                    if (row[(int)RowData.Down] != null)
                    {
                        DrawCube(bone, ParseVertexData((int)row[(int)RowData.Down], objInfo), Color.red, name + "_down");
                    }
                    if (row[(int)RowData.Forward] != null)
                    {
                        DrawCube(bone, ParseVertexData((int)row[(int)RowData.Forward], objInfo), Color.red, name + "_forward");
                    }
                    if (row[(int)RowData.Back] != null)
                    {
                        DrawCube(bone, ParseVertexData((int)row[(int)RowData.Back], objInfo), Color.red, name + "_back");
                    }
                }
            }
#endif
            for (int i = 0; i < count; i++)
            {
                if (_bones[i] != null)
                {
                    _bones[i].localRotation = Quaternion.identity;
                }
            }

            {
                float scale = ((_objPoints[(int)AvatarBones.Neck1].y * BodyScale) - _objOffset.y) / _bones[(int)AvatarBones.Neck1].position.y;
                gameObject.transform.localScale = new Vector3(scale, scale, scale);
            }

            {
                bool isMale = DataManager.Instance.Profile.Gender == Gender.Male;

                Vector3 ribRight = ParseVertexData(436, objInfo);
                Vector3 ribLeft = ParseVertexData(1398, objInfo);
                Vector3 ribForward = ParseVertexData(921, objInfo);
                Vector3 ribBack = ParseVertexData(920, objInfo);

                float ribXDistance = (ribRight - ribLeft).magnitude;
                float ribXScale = ribXDistance / (isMale ? 280.0f : 220.0f);

                float ribZDistance = (ribForward - ribBack).magnitude;
                float ribZScale = ribZDistance / (isMale ? 210.0f : 180.0f);

                int index = (int)AvatarBones.SpineWaist;
                _bones[index].localScale =
                    new Vector3(_bones[index].localScale.x * ribXScale, _bones[index].localScale.y, _bones[index].localScale.z * ribZScale);

                index = (int)AvatarBones.Neck2;
                _bones[index].localScale =
                    new Vector3(_bones[index].localScale.x / ribXScale, _bones[index].localScale.y, _bones[index].localScale.z / ribZScale);
            }

            float headDistanceBefore = (_bones[(int)AvatarBones.Head].position - _bones[(int)AvatarBones.Neck2].position).magnitude;

            Quaternion inverseQ = Quaternion.identity;
            Quaternion hipsInverseQ = Quaternion.identity;

            {
                int index = (int)AvatarBones.Hips;
                _bones[index].position = _objPoints[index] * BodyScale;

                Vector3 leftUpLegPos = _objPoints[(int)AvatarBones.LeftUpLeg] * BodyScale;
                Vector3 rightUpLegPos = _objPoints[(int)AvatarBones.RightUpLeg] * BodyScale;

                Vector3 to = CalcDirection(AvatarBones.Hips, AvatarBones.SpineWaist);
                to = new Vector3(0.0f, to.y, to.z);

                Quaternion qX = Quaternion.FromToRotation(Vector3.up, to);

                Quaternion qY;
                {
                    float radian = Mathf.Atan2(rightUpLegPos.z - leftUpLegPos.z, rightUpLegPos.x - leftUpLegPos.x);
                    qY = Quaternion.AngleAxis(radian * Mathf.Rad2Deg, Vector3.up);
                }

                Quaternion qZ;
                {
                    float radian = Mathf.Atan2(rightUpLegPos.y - leftUpLegPos.y, rightUpLegPos.x - leftUpLegPos.x);
                    qZ = Quaternion.AngleAxis(radian * Mathf.Rad2Deg, Vector3.forward);
                }

                _bones[index].localRotation *= (qX * qY * qZ);

                inverseQ = Quaternion.Inverse(_bones[index].rotation);
                hipsInverseQ = inverseQ;
            }
            {
                int index = (int)AvatarBones.SpineWaist;
                _bones[index].position = _objPoints[index] * BodyScale;

                Vector3 to = CalcDirection(AvatarBones.SpineWaist, AvatarBones.Spine1);

                Quaternion q = Quaternion.FromToRotation(Vector3.up, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(19.4f, 0.0f, 0.0f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.Spine1;
                _bones[index].position = _objPoints[index] * BodyScale;

                Vector3 to = CalcDirection(AvatarBones.Spine1, AvatarBones.Spine2);

                Quaternion q = Quaternion.FromToRotation(Vector3.up, to);
                _bones[index].localRotation *= (inverseQ * q);
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.Spine2;
                _bones[index].position = _objPoints[index] * BodyScale;

                Vector3 to = CalcDirection(AvatarBones.Spine2, AvatarBones.Neck1);

                Quaternion q = Quaternion.FromToRotation(Vector3.up, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(-15.7f, 0.0f, 0.0f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.Neck1;
                _bones[index].position = _objPoints[index] * BodyScale;

                Vector3 to = CalcDirection(AvatarBones.Neck1, AvatarBones.Neck2);

                Quaternion q = Quaternion.FromToRotation(Vector3.up, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(-25.6f, 0.0f, 0.0f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.Neck2;
                _bones[index].position = _objPoints[index] * BodyScale;

                Vector3 to = CalcDirection(AvatarBones.Neck2, AvatarBones.Neck3);

                Quaternion q = Quaternion.FromToRotation(Vector3.up, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(-1.3f, 0.0f, 0.0f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            
            {
                int index = (int)AvatarBones.Neck3;
                _bones[index].position = _objPoints[index] * BodyScale;
                
                Vector3 to = CalcDirection(AvatarBones.Neck2, AvatarBones.Head);
                
                Quaternion q = Quaternion.FromToRotation(Vector3.up, to);

                Quaternion qY;
                {
                    Vector3 leftEarlobePos = _objPoints[(int)AvatarBones.LeftEarlobe] * BodyScale;
                    Vector3 rightEarlobePos = _objPoints[(int)AvatarBones.RightEarlobe] * BodyScale;

                    float radian = -Mathf.Atan2(rightEarlobePos.z - leftEarlobePos.z, rightEarlobePos.x - leftEarlobePos.x);
                    qY = Quaternion.AngleAxis(radian * Mathf.Rad2Deg, Vector3.up);
                }

                _bones[index].localRotation *= (inverseQ * q * qY);
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }

            {
                int index = (int)AvatarBones.Head;
                _bones[index].position = _objPoints[index] * BodyScale;
            }
            {
                int index = (int)AvatarBones.LeftShoulder;
                _bones[index].position = _objPoints[index] * BodyScale;

                Vector3 to = CalcDirection(AvatarBones.LeftShoulder, AvatarBones.LeftArm);

                Quaternion q = Quaternion.FromToRotation(Vector3.left, to);
                _bones[index].localRotation *= (q * Quaternion.Euler(0.0f, 17.5f, -3.3f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.LeftArm;
                _bones[index].position = _objPoints[index] * BodyScale;

                Vector3 to = CalcDirection(AvatarBones.LeftArm, AvatarBones.LeftForeArm);

                Quaternion q = Quaternion.FromToRotation(Vector3.left, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(0.0f, 4.3f, 4.7f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.LeftUpScale1;
                _bones[index].position = _objPoints[index] * BodyScale;
            }
            {
                int index = (int)AvatarBones.LeftForeArm;
                _bones[index].position = _objPoints[index] * BodyScale;

                Vector3 to = CalcDirection(AvatarBones.LeftForeArm, AvatarBones.LeftHand);

                Quaternion q = Quaternion.FromToRotation(Vector3.left, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(0.0f, -6.3f, 1.6f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.LeftHand;
                _bones[index].position = _objPoints[index] * BodyScale;

                Vector3 to = CalcDirection(AvatarBones.LeftHand, AvatarBones.LeftHand2);

                Quaternion q = Quaternion.FromToRotation(Vector3.left, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(0.0f, -1.6f, 2.0f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.LeftHand2;
                _bones[index].position = _objPoints[index] * BodyScale;
            }
            {
                int index = (int)AvatarBones.RightShoulder;
                _bones[index].position = _objPoints[index] * BodyScale;

                Vector3 to = CalcDirection(AvatarBones.RightShoulder, AvatarBones.RightArm);

                Quaternion q = Quaternion.FromToRotation(Vector3.right, to);
                _bones[index].localRotation *= (q * Quaternion.Euler(0.0f, -17.5f, 3.3f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.RightArm;
                _bones[index].position = _objPoints[index] * BodyScale;

                Vector3 to = CalcDirection(AvatarBones.RightArm, AvatarBones.RightForeArm);

                Quaternion q = Quaternion.FromToRotation(Vector3.right, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(0.0f, -4.3f, -4.7f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.RightUpScale1;
                _bones[index].position = _objPoints[index] * BodyScale;
            }
            {
                int index = (int)AvatarBones.RightForeArm;
                _bones[index].position = _objPoints[index] * BodyScale;

                Vector3 to = CalcDirection(AvatarBones.RightForeArm, AvatarBones.RightHand);

                Quaternion q = Quaternion.FromToRotation(Vector3.right, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(0.0f, 6.3f, -1.6f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.RightHand;
                _bones[index].position = _objPoints[index] * BodyScale;

                Vector3 to = CalcDirection(AvatarBones.RightHand, AvatarBones.RightHand2);

                Quaternion q = Quaternion.FromToRotation(Vector3.right, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(0.0f, 1.6f, -2.0f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.RightHand2;
                _bones[index].position = _objPoints[index] * BodyScale;
            }
            {
                int index = (int)AvatarBones.LeftUpLeg;
                _bones[index].position = _objPoints[index] * BodyScale;

                Vector3 to = CalcDirection(AvatarBones.LeftUpLeg, AvatarBones.LeftLeg);

                Quaternion q = Quaternion.FromToRotation(Vector3.down, to);
                _bones[index].localRotation *= (q * Quaternion.Euler(-5.4f, 0.0f, -1.0f) * hipsInverseQ);
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.LeftDownScale1;
                _bones[index].position = _objPoints[index] * BodyScale;
            }
            {
                int index = (int)AvatarBones.LeftDownScale2;
                _bones[index].position = _objPoints[index] * BodyScale;
            }
            {
                int index = (int)AvatarBones.LeftLeg;
                _bones[index].position = _objPoints[index] * BodyScale;

                Vector3 to = CalcDirection(AvatarBones.LeftLeg, AvatarBones.LeftFoot);

                Quaternion q = Quaternion.FromToRotation(Vector3.down, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(-7.1f, 0.0f, -0.4f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.LeftFoot;
                _bones[index].position = _objPoints[index] * BodyScale;

                Vector3 to = CalcDirection(AvatarBones.LeftFoot, AvatarBones.LeftToe);

                Quaternion q = Quaternion.FromToRotation(Vector3.forward, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(-19.5f, 0.0f, 0.0f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.LeftToe;
                _bones[index].position = _objPoints[index] * BodyScale;
            }
            {
                int index = (int)AvatarBones.RightUpLeg;
                _bones[index].position = _objPoints[index] * BodyScale;

                Vector3 to = CalcDirection(AvatarBones.RightUpLeg, AvatarBones.RightLeg);

                Quaternion q = Quaternion.FromToRotation(Vector3.down, to);
                _bones[index].localRotation *= (q * Quaternion.Euler(-5.4f, 0.0f, 1.0f) * hipsInverseQ);
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.RightDownScale1;
                _bones[index].position = _objPoints[index] * BodyScale;
            }
            {
                int index = (int)AvatarBones.RightDownScale2;
                _bones[index].position = _objPoints[index] * BodyScale;
            }
            {
                int index = (int)AvatarBones.RightLeg;
                _bones[index].position = _objPoints[index] * BodyScale;

                Vector3 to = CalcDirection(AvatarBones.RightLeg, AvatarBones.RightFoot);

                Quaternion q = Quaternion.FromToRotation(Vector3.down, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(-7.1f, 0.0f, 0.4f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.RightFoot;
                _bones[index].position = _objPoints[index] * BodyScale;

                Vector3 to = CalcDirection(AvatarBones.RightFoot, AvatarBones.RightToe);

                Quaternion q = Quaternion.FromToRotation(Vector3.forward, to);
                _bones[index].localRotation *= (inverseQ * q * Quaternion.Euler(-19.5f, 0.0f, 0.0f));
                inverseQ = Quaternion.Inverse(_bones[index].rotation);
            }
            {
                int index = (int)AvatarBones.RightToe;
                _bones[index].position = _objPoints[index] * BodyScale;
            }

            {
                float headDistanceAfter = (_bones[(int)AvatarBones.Head].position - _bones[(int)AvatarBones.Neck2].position).magnitude;

                if ((headDistanceAfter * 0.9f) < headDistanceBefore)
                {
                    float coefficient = (headDistanceAfter / headDistanceBefore) * 0.9f;
                    int index = (int)AvatarBones.Head;
                    _bones[index].localScale =
                        new Vector3(_bones[index].localScale.x * coefficient, _bones[index].localScale.y * coefficient, _bones[index].localScale.z * coefficient);
                }
            }

#if DISPLAY_FBX_BONE_LINES
            AddPrimitiveGameObjectForFBX(_bones[(int)AvatarBones.SpineWaist], new Vector3(0.005f, 0.2f, 0.005f), "SpineWaist");
            AddPrimitiveGameObjectForFBX(_bones[(int)AvatarBones.Spine1], new Vector3(0.005f, 0.2f, 0.005f), "Spine1");
            AddPrimitiveGameObjectForFBX(_bones[(int)AvatarBones.Spine2], new Vector3(0.005f, 0.2f, 0.005f), "Spine2");
            AddPrimitiveGameObjectForFBX(_bones[(int)AvatarBones.Neck1], new Vector3(0.005f, 0.2f, 0.005f), "Neck1");
            AddPrimitiveGameObjectForFBX(_bones[(int)AvatarBones.Neck2], new Vector3(0.005f, 0.2f, 0.005f), "Neck2");
            AddPrimitiveGameObjectForFBX(_bones[(int)AvatarBones.Neck3], new Vector3(0.005f, 0.2f, 0.005f), "Neck3");
            AddPrimitiveGameObjectForFBX(_bones[(int)AvatarBones.LeftShoulder], new Vector3(0.15f, 0.005f, 0.005f), "LeftShoulder");
            AddPrimitiveGameObjectForFBX(_bones[(int)AvatarBones.LeftArm], new Vector3(0.3f, 0.005f, 0.005f), "LeftArm");
            AddPrimitiveGameObjectForFBX(_bones[(int)AvatarBones.LeftForeArm], new Vector3(0.3f, 0.005f, 0.005f), "LeftForeArm");
            AddPrimitiveGameObjectForFBX(_bones[(int)AvatarBones.LeftHand], new Vector3(0.3f, 0.005f, 0.005f), "LeftHand");
            AddPrimitiveGameObjectForFBX(_bones[(int)AvatarBones.RightShoulder], new Vector3(0.15f, 0.005f, 0.005f), "RightShoulder");
            AddPrimitiveGameObjectForFBX(_bones[(int)AvatarBones.RightArm], new Vector3(0.3f, 0.005f, 0.005f), "RightArm");
            AddPrimitiveGameObjectForFBX(_bones[(int)AvatarBones.RightForeArm], new Vector3(0.3f, 0.005f, 0.005f), "RightForeArm");
            AddPrimitiveGameObjectForFBX(_bones[(int)AvatarBones.RightHand], new Vector3(0.3f, 0.005f, 0.005f), "RightHand");
            AddPrimitiveGameObjectForFBX(_bones[(int)AvatarBones.LeftUpLeg], new Vector3(0.005f, 0.3f, 0.005f), "LeftUpLeg");
            AddPrimitiveGameObjectForFBX(_bones[(int)AvatarBones.LeftLeg], new Vector3(0.005f, 0.3f, 0.005f), "LeftLeg");
            AddPrimitiveGameObjectForFBX(_bones[(int)AvatarBones.LeftFoot], new Vector3(0.005f, 0.005f, 0.3f), "LeftFoot");
            AddPrimitiveGameObjectForFBX(_bones[(int)AvatarBones.RightUpLeg], new Vector3(0.005f, 0.3f, 0.005f), "RightUpLeg");
            AddPrimitiveGameObjectForFBX(_bones[(int)AvatarBones.RightLeg], new Vector3(0.005f, 0.3f, 0.005f), "RightLeg");
            AddPrimitiveGameObjectForFBX(_bones[(int)AvatarBones.RightFoot], new Vector3(0.005f, 0.005f, 0.3f), "RightFoot");
#endif

            onAvatarUpdated.Invoke(this);
        }

        private Dictionary<RowData, Vector3>[] LoadSrcPoints(IReadOnlyList<string> objInfo)
        {
            var result = new Dictionary<RowData, Vector3>[Enum.GetNames(typeof(AvatarBones)).Length];
            int count = Enum.GetNames(typeof(AvatarBones)).Length;

            for (int i = 0; i < count; i++)
            {
                int?[] row = AvatarBonesObjRows[(AvatarBones)i];
                foreach (RowData data in Enum.GetValues(typeof(RowData)))
                {
                    if (row?[(int) data] == null) continue;
                    Vector3 v = ParseVertexData((int)row[(int)data], objInfo);
                    if (result[i] == null) result[i] = new Dictionary<RowData, Vector3>();
                    result[i][data] = v;
                }
            }

            return result;
        }

        private Vector3 ParseVertexData(int rowNum, IReadOnlyList<string> objInfo)
        {
            string rowString = objInfo[rowNum + _rowsOffset];
            string[] elements = rowString.Split(' ');
            return new Vector3(-float.Parse(elements[1]), float.Parse(elements[2]), float.Parse(elements[3])) + (_objOffset / BodyScale);
        }

        private Vector3 CalcSpineLineVertex(IReadOnlyList<int?> row, IReadOnlyList<string> objInfo, float yRate, float zRate)
        {
            float sumX = 0.0f;

            Vector3 v = ParseVertexData((int)row[(int)RowData.Forward], objInfo);
            sumX += v.x;
            float forwardY = v.y;
            float forwardZ = v.z;

            v = ParseVertexData((int)row[(int)RowData.Back], objInfo);
            sumX += v.x;
            float backY = v.y;
            float backZ = v.z;

            return new Vector3(sumX / 2.0f, Mathf.Lerp(forwardY, backY, yRate), Mathf.Lerp(forwardZ, backZ, zRate));
        }

        private Vector3 CalcLimbVertex([NotNull] int?[] row, IReadOnlyList<string> objInfo, float zRate)
        {
            if (row == null) throw new ArgumentNullException(nameof(row));
            float sumX = 0.0f;
            float sumY = 0.0f;

            Vector3 v = ParseVertexData((int)row[(int)RowData.Left], objInfo);
            sumX += v.x;
            sumY += v.y;

            v = ParseVertexData((int)row[(int)RowData.Right], objInfo);
            sumX += v.x;
            sumY += v.y;

            v = ParseVertexData((int)row[(int)RowData.Forward], objInfo);
            float forwardZ = v.z;

            v = ParseVertexData((int)row[(int)RowData.Back], objInfo);
            float backZ = v.z;

            return new Vector3(sumX / 2.0f, sumY / 2.0f, Mathf.Lerp(forwardZ, backZ, zRate));
        }

        private Vector3 CalcDirection(AvatarBones from, AvatarBones to)
        {
            Vector3 fromV = _objPoints[(int)from];
            Vector3 toV = _objPoints[(int)to];

            Vector3 direction = toV - fromV;
            direction = direction.normalized;


            return direction;
        }

        private void ApplyShouldersDirection(Vector3 shouldersDirection, AvatarBones bone)
        {
            Vector3 jointDir = Vector3.Lerp(shouldersDirection, -shouldersDirection, 0.5f);
            jointDir.z = -jointDir.z;
            _bones[(int)bone].rotation *= Quaternion.FromToRotation(Vector3.right, jointDir);
        }

        private GameObject _parentPrimitives;

        private void DrawCube(Vector3 pos, Color c, string objectName)
        {
            if (_parentPrimitives == null)
            {
                GenerateParentPrimitives();
            }

            AddPrimitiveGameObject(pos * BodyScale, new Vector3(0.02f, 0.02f, 0.02f), c, PrimitiveType.Cube, objectName, _parentPrimitives);
        }

        private void DrawSphere(AvatarBones bone, Vector3 pos, Color c, string objectName)
        {
            if (_parentPrimitives == null)
            {
                GenerateParentPrimitives();
            }

            GameObject go = AddPrimitiveGameObject(pos * BodyScale, new Vector3(0.02f, 0.02f, 0.02f), c, PrimitiveType.Sphere, objectName, _parentPrimitives);
            DrawLines(bone, go);
        }

        private void GenerateParentPrimitives()
        {
            _parentPrimitives = new GameObject {layer = LayerMask.NameToLayer("Human"), name = "PrimitiveObjs"};
            _parentPrimitives.SetActive(true);
        }

        private static GameObject AddPrimitiveGameObject(Vector3 pos, Vector3 scale, Color c, PrimitiveType type, string name, GameObject parent)
        {

            GameObject go = GameObject.CreatePrimitive(type);
            go.layer = LayerMask.NameToLayer("Human");
            go.name = name;
            go.transform.parent = parent.transform;
            go.transform.position = pos;
            go.transform.localScale = scale;
            go.SetActive(true);

            Renderer r = go.GetComponent<Renderer>();
            r.material.color = c;

            return go;
        }

        private GameObject AddPrimitiveGameObjectForFbx(Transform bone, Vector3 scale, string objectName)
        {
            GameObject go = AddPrimitiveGameObject(bone.position, scale, Color.blue, PrimitiveType.Cube, "FBX_" + objectName, bone.gameObject);
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = scale;

            return go;
        }

        private void DrawLines(AvatarBones bone, GameObject go)
        {
            switch (bone)
            {
                case AvatarBones.Hips:
                    DrawLinesSub(go, new[] { _objPoints[(int)AvatarBones.Hips], _objPoints[(int)AvatarBones.SpineWaist] });
                    break;
                case AvatarBones.LeftUpLeg:
                    DrawLinesSub(go, new[] { _objPoints[(int)AvatarBones.Hips], _objPoints[(int)AvatarBones.LeftUpLeg], _objPoints[(int)AvatarBones.LeftLeg] });
                    break;
                case AvatarBones.LeftLeg:
                    DrawLinesSub(go, new[] { _objPoints[(int)AvatarBones.LeftLeg], _objPoints[(int)AvatarBones.LeftFoot] });
                    break;
                case AvatarBones.LeftFoot:
                    DrawLinesSub(go, new[] { _objPoints[(int)AvatarBones.LeftFoot], _objPoints[(int)AvatarBones.LeftToe] });
                    break;
                case AvatarBones.RightUpLeg:
                    DrawLinesSub(go, new[] { _objPoints[(int)AvatarBones.Hips], _objPoints[(int)AvatarBones.RightUpLeg], _objPoints[(int)AvatarBones.RightLeg] });
                    break;
                case AvatarBones.RightLeg:
                    DrawLinesSub(go, new[] { _objPoints[(int)AvatarBones.RightLeg], _objPoints[(int)AvatarBones.RightFoot] });
                    break;
                case AvatarBones.RightFoot:
                    DrawLinesSub(go, new[] { _objPoints[(int)AvatarBones.RightFoot], _objPoints[(int)AvatarBones.RightToe] });
                    break;
                case AvatarBones.SpineWaist:
                    DrawLinesSub(go, new[] { _objPoints[(int)AvatarBones.SpineWaist], _objPoints[(int)AvatarBones.Spine1] });
                    break;
                case AvatarBones.Spine1:
                    DrawLinesSub(go, new[] { _objPoints[(int)AvatarBones.Spine1], _objPoints[(int)AvatarBones.Spine2] });
                    break;
                case AvatarBones.Spine2:
                    DrawLinesSub(go, new[] { _objPoints[(int)AvatarBones.Spine2], _objPoints[(int)AvatarBones.Neck1] });
                    break;
                case AvatarBones.Neck1:
                    DrawLinesSub(go, new[] { _objPoints[(int)AvatarBones.Neck1], _objPoints[(int)AvatarBones.Neck2] });
                    break;
                case AvatarBones.Neck2:
                    DrawLinesSub(go, new[] { _objPoints[(int)AvatarBones.Neck2], _objPoints[(int)AvatarBones.Neck3] });
                    break;
                case AvatarBones.Neck3:
                    DrawLinesSub(go, new[] { _objPoints[(int)AvatarBones.Neck3], _objPoints[(int)AvatarBones.Head] });
                    break;
                case AvatarBones.LeftShoulder:
                    DrawLinesSub(go, new[] { _objPoints[(int)AvatarBones.Spine2], _objPoints[(int)AvatarBones.LeftShoulder], _objPoints[(int)AvatarBones.LeftArm] });
                    break;
                case AvatarBones.LeftArm:
                    DrawLinesSub(go, new[] { _objPoints[(int)AvatarBones.LeftArm], _objPoints[(int)AvatarBones.LeftForeArm] });
                    break;
                case AvatarBones.LeftForeArm:
                    DrawLinesSub(go, new[] { _objPoints[(int)AvatarBones.LeftForeArm], _objPoints[(int)AvatarBones.LeftHand] });
                    break;
                case AvatarBones.LeftHand:
                    DrawLinesSub(go, new[] { _objPoints[(int)AvatarBones.LeftHand], _objPoints[(int)AvatarBones.LeftHand2] });
                    break;
                case AvatarBones.RightShoulder:
                    DrawLinesSub(go, new[] { _objPoints[(int)AvatarBones.Spine2], _objPoints[(int)AvatarBones.RightShoulder], _objPoints[(int)AvatarBones.RightArm] });
                    break;
                case AvatarBones.RightArm:
                    DrawLinesSub(go, new[] { _objPoints[(int)AvatarBones.RightArm], _objPoints[(int)AvatarBones.RightForeArm] });
                    break;
                case AvatarBones.RightForeArm:
                    DrawLinesSub(go, new[] { _objPoints[(int)AvatarBones.RightForeArm], _objPoints[(int)AvatarBones.RightHand] });
                    break;
                case AvatarBones.RightHand:
                    DrawLinesSub(go, new[] { _objPoints[(int)AvatarBones.RightHand], _objPoints[(int)AvatarBones.RightHand2] });
                    break;
            }
        }

        private static void DrawLinesSub(GameObject go, Vector3[] points)
        {
            LineRenderer lineRenderer = go.AddComponent(typeof(LineRenderer)) as LineRenderer;
            if (lineRenderer == null) return;
            lineRenderer.material = new Material(Shader.Find("Diffuse")) {color = Color.green};
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            lineRenderer.positionCount = points.Length;

            int count = points.Length;
            for (int i = 0; i < count; i++)
            {
                lineRenderer.SetPosition(i, points[i] * BodyScale);
            }
        }

        private void MapBones()
        {
            int count = _bones.Length;
            for (int i = 0; i < count; i++)
            {
                var bone = GetFirstChildByName(((AvatarBones)i).GetName());
                if (bone != null)
                {
                    _bones[i] = bone;
                }
            }
        }

        private Transform _boneRootTransform;

        private readonly Dictionary<string, Transform> _transformListCache = new Dictionary<string, Transform>();

        private Transform GetFirstChildByName(string childName)
        {
            if (string.IsNullOrEmpty(childName))
            {
                return null;
            }

            if (_boneRootTransform != null) return _transformListCache.ContainsKey(childName) ? _transformListCache[childName] : GetFirstChildByNameSub(_boneRootTransform, childName);
            _boneRootTransform = GetFirstChildByNameSub(transform, AvatarBones.Root.GetName());

            if (_boneRootTransform == null)
            {
                throw new UnityException("Root bone is not found!");
            }

            return _transformListCache.ContainsKey(childName) ? _transformListCache[childName] : GetFirstChildByNameSub(_boneRootTransform, childName);
        }
        
        private Transform GetFirstChildByNameSub(Transform parent, string childName)
        {
            foreach (Transform child in parent)
            {
                _transformListCache[child.name] = child;

                if (child.name == childName)
                {
                    return child;
                }

                Transform target = GetFirstChildByNameSub(child, childName);
                if (target != null) return target;
            }

            return null;
        }

        public Dictionary<RowData, Vector3> GetSrcPoint(AvatarBones bone)
        {
            return _objSrcPoints[(int)bone];
        }

        public Vector3? GetPoint(AvatarBones bone)
        {
            var bonePoint = _bones[(int)bone] != null ? (Vector3?)_bones[(int)bone].position / BodyScale : null;
            var objPoint = _objPoints[(int)bone] != Vector3.zero ? (Vector3?)_objPoints[(int)bone] : null;
            return bonePoint ?? objPoint; 
        }
        
        public Quaternion? GetRotation(AvatarBones bone)
        {
            return _bones[(int)bone] != null ? (Quaternion?)_bones[(int)bone].rotation : null;
        }

        private Dictionary<AvatarBones, int?[]> AvatarBonesObjRows => DataManager.Instance.Profile.Gender == Gender.Male ? _maleAvatarBonesObjRows : _femaleAvatarBonesObjRows;

        private readonly Dictionary<AvatarBones, int?[]> _maleAvatarBonesObjRows = new Dictionary<AvatarBones, int?[]>
        {
            {AvatarBones.Hips, new int?[] {null, null, 6602, 26148, null, null } },
            {AvatarBones.Jushin, null },
            {AvatarBones.LeftUpLeg, new int?[] { 4988, null, 22954, null, null, null } },
            {AvatarBones.LeftDownScale1, new int?[] { 4988, null, 22954, 4505, null, null } },
            {AvatarBones.LeftDownScale2, new int?[] { null, null, 22798, null, null, null } },
            {AvatarBones.LeftGreaterTrochanter, null },
            {AvatarBones.LeftLeg, new int?[] { null, null, 15881, 23121, null, null } },
            {AvatarBones.LeftPatella, null },
            {AvatarBones.LeftFoot, new int?[] { 5379, 5377, 16740, 22864, null, null } },
            {AvatarBones.LeftAnkle, null },
            {AvatarBones.LeftToe, new int?[] { null, null, 18199, null, 18204, null } },
            {AvatarBones.RightUpLeg, new int?[] { null, 10729, 14546, null, null, null } },
            {AvatarBones.RightDownScale1, new int?[] { null, 10729, 14546, null, null, null } },
            {AvatarBones.RightDownScale2, new int?[] { null, null, 14386, null, null, null } },
            {AvatarBones.RightGreaterTrochanter, null },
            {AvatarBones.RightLeg, new int?[] { null, null, 7470, 14712, null, null } },
            {AvatarBones.RightPatella, null },
            {AvatarBones.RightFoot, new int?[] { 3250, 3252, 8342, 14457, null, null } },
            {AvatarBones.RightAnkle, null },
            {AvatarBones.RightToe, new int?[] { null, null, 9789, null, 9791, null } },
            {AvatarBones.SpineWaist, new int?[] { null, null, 26259, 11860, null, null } },
            {AvatarBones.Spine1, new int?[] { null, null, 26141, 26358, null, null } },
            {AvatarBones.Spine2, new int?[] { null, null, 3202, 8010, null, null } },
            {AvatarBones.Neck1, new int?[] { null, null, 3570, 924, null, null } },
            {AvatarBones.Neck2, new int?[] { 19717, 11300, null, null, null, null } },
            {AvatarBones.Neck3, new int?[] { 4692, 2560, null, null, null, null } },
            {AvatarBones.LeftEarlobe, new int?[] { null, null, null, null, 24044, null } },
            {AvatarBones.RightEarlobe, new int?[] { null, null, null, null, 15645, null } },
            {AvatarBones.Head, new int?[] { null, null, null, null, 3225, null } },
            {AvatarBones.LeftShoulder, new int?[] { null, null, 5124, null, 1058, null } },
            {AvatarBones.LeftAcromion, new int?[] { null, null, null, null, 19645, null } },
            {AvatarBones.LeftArm, new int?[] { null, null, 16419, 5575, null, null } },
            {AvatarBones.LeftUpScale1, new int?[] {  null, null, 16419, 5575, null, null } },
            {AvatarBones.LeftForeArm, new int?[] { 25805, 1662, 26016, 25443, null, null } },
            {AvatarBones.LeftHand, new int?[] { null, null, 6244, 6213, null, null } },
            {AvatarBones.LeftHand2, new int?[] { null, null, 24296, null, null, null } },
            {AvatarBones.RightShoulder, new int?[] { null, null, 2988, null, 776, null } },
            {AvatarBones.RightAcromion, new int?[] { null, null, null, null, 11232, null } },
            {AvatarBones.RightArm, new int?[] { null, null, 8017, 3448, null, null } },
            {AvatarBones.RightUpScale1, new int?[] { null, null, 8017, 3448, null, null } },
            {AvatarBones.RightForeArm, new int?[] { 7084, 28157, 28609, 7028, null, null } },
            {AvatarBones.RightHand, new int?[] { null, null, 6853, 27220, null, null } },
            {AvatarBones.RightHand2, new int?[] { null, null, 26898, null, null, null } },
            {AvatarBones.Root, null }
        };
        
        private readonly Dictionary<AvatarBones, int?[]> _femaleAvatarBonesObjRows = new Dictionary<AvatarBones, int?[]>
        {
            {AvatarBones.Hips, new int?[] {null, null, 6602, 26148, null, null } },
            {AvatarBones.Jushin, null },
            {AvatarBones.LeftUpLeg, new int?[] { 4988, null, 22954, null, null, null } },
            {AvatarBones.LeftDownScale1, new int?[] { 4988, null, 22954, 4505, null, null } },
            {AvatarBones.LeftDownScale2, new int?[] { null, null, 22798, null, null, null } },
            {AvatarBones.LeftGreaterTrochanter, null },
            {AvatarBones.LeftLeg, new int?[] { null, null, 15881, 23121, null, null } },
            {AvatarBones.LeftPatella, null },
            {AvatarBones.LeftFoot, new int?[] { 5379, 5377, 16740, 22864, null, null } },
            {AvatarBones.LeftAnkle, null },
            {AvatarBones.LeftToe, new int?[] { null, null, 18199, null, 18204, null } },
            {AvatarBones.RightUpLeg, new int?[] { null, 10729, 14546, null, null, null } },
            {AvatarBones.RightDownScale1, new int?[] { null, 10729, 14546, null, null, null } },
            {AvatarBones.RightDownScale2, new int?[] { null, null, 14386, null, null, null } },
            {AvatarBones.RightGreaterTrochanter, null },
            {AvatarBones.RightLeg, new int?[] { null, null, 7470, 14712, null, null } },
            {AvatarBones.RightPatella, null },
            {AvatarBones.RightFoot, new int?[] { 3250, 3252, 8342, 14457, null, null } },
            {AvatarBones.RightAnkle, null },
            {AvatarBones.RightToe, new int?[] { null, null, 9789, null, 9791, null } },
            {AvatarBones.SpineWaist, new int?[] { null, null, 26259, 11860, null, null } },
            {AvatarBones.Spine1, new int?[] { null, null, 26141, 26358, null, null } },
            {AvatarBones.Spine2, new int?[] { null, null, 3202, 8010, null, null } },
            {AvatarBones.Neck1, new int?[] { null, null, 3570, 924, null, null } },
            {AvatarBones.Neck2, new int?[] { 19717, 11300, null, null, null, null } },
            {AvatarBones.Neck3, new int?[] { 4692, 2560, null, null, null, null } },
            {AvatarBones.LeftEarlobe, new int?[] { null, null, null, null, 24044, null } },
            {AvatarBones.RightEarlobe, new int?[] { null, null, null, null, 15645, null } },
            {AvatarBones.Head, new int?[] { null, null, null, null, 3225, null } },
            {AvatarBones.LeftShoulder, new int?[] { null, null, 5124, null, 1058, null } },
            {AvatarBones.LeftAcromion, new int?[] { null, null, null, null, 19645, null } },
            {AvatarBones.LeftArm, new int?[] { null, null, 4189, 23237, null, null } },
            {AvatarBones.LeftUpScale1, new int?[] { null, null, 4189, 23237, null, null } },
            {AvatarBones.LeftForeArm, new int?[] { 25805, 1662, 26016, 25443, null, null } },
            {AvatarBones.LeftHand, new int?[] { null, null, 6244, 6213, null, null } },
            {AvatarBones.LeftHand2, new int?[] { null, null, 24296, null, null, null } },
            {AvatarBones.RightShoulder, new int?[] { null, null, 2988, null, 776, null } },
            {AvatarBones.RightAcromion, new int?[] { null, null, null, null, 11232, null } },
            {AvatarBones.RightArm, new int?[] { null, null, 2055, 14829, null, null } },
            {AvatarBones.RightUpScale1, new int?[] { null, null, 2055, 14829, null, null } },
            {AvatarBones.RightForeArm, new int?[] { 7084, 28157, 28609, 7028, null, null } },
            {AvatarBones.RightHand, new int?[] { null, null, 6853, 27220, null, null } },
            {AvatarBones.RightHand2, new int?[] { null, null, 26898, null, null, null } },
            {AvatarBones.Root, null }
        };
    }
}