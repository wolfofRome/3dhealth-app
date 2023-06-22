using Assets.Scripts.Common.Config;
using Assets.Scripts.Common.Data;
using Assets.Scripts.Network.Response;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Assets.Scripts.Common
{
    [RequireComponent(typeof(ProgressAdapter))]
    [RequireComponent(typeof(ObjLoader))]
    [RequireComponent(typeof(MeasurementDataLoader))]
    [RequireComponent(typeof(BodyCompositionDataLoader))]
    public abstract class BaseContentsLoader : MonoBehaviour
    {
        [Serializable]
        public class ContentsLoadEvent : UnityEvent<Contents> { }
        [FormerlySerializedAs("OnLoadStart")] public ContentsLoadEvent onLoadStart;
        [FormerlySerializedAs("OnLoadCompleted")] public ContentsLoadEvent onLoadCompleted;

        [FormerlySerializedAs("_objLightList")] [SerializeField]
        private List<Light> objLightList;

        private int _loadTargetCount;
        private int _loadCompleteCount;

        private ObjLoader _objLoader;

        private ObjLoader objLoader {
            get {
                return _objLoader = _objLoader ? _objLoader : GetComponent<ObjLoader>();
            }
        }

        private MeasurementDataLoader _measurementDataLoader;
        protected MeasurementDataLoader measurementDataLoader {
            get {
                return _measurementDataLoader = _measurementDataLoader ? _measurementDataLoader : GetComponent<MeasurementDataLoader>();
            }
        }

        private BodyCompositionDataLoader _bodyCompositionDataLoader;

        private BodyCompositionDataLoader bodyCompositionDataLoader {
            get {
                return _bodyCompositionDataLoader = _bodyCompositionDataLoader ? _bodyCompositionDataLoader : GetComponent<BodyCompositionDataLoader>();
            }
        }

        private ProgressAdapter _progressAdapter;
        protected ProgressAdapter ProgressAdapter {
            get {
                return _progressAdapter = _progressAdapter ? _progressAdapter : GetComponent<ProgressAdapter>();
            }
        }
        
        protected virtual bool useObjMemoryCache => false;

        protected virtual bool useObjDiskCache => true;

        private Contents _contents;

        protected virtual void Awake() {}
        
        protected virtual void Start() {}

        protected virtual void StartLoading(Contents contents)
        {
            _contents = contents;

            // 3D Objectのロード.
            if (objLoader.enabled)
            {
                _loadTargetCount++;
#if UNITY_EDITOR
                    UnityEngine.Debug.Log("Resource_Id : " + _contents.ResourceId);
#endif
                var param = new LoadParam()
                {
                    ResourceId = _contents.ResourceId,
                    CreateTime = _contents.CreateTime,
                    ModifiedTime = _contents.ModifiedTime,
                    Path = _contents.ScanDataZipPath
                };
                objLoader.useMemoryCache = useObjMemoryCache;
                objLoader.useDiskCache = useObjDiskCache;
                objLoader.OnLoadCompleted.AddListener( (loader, obj) => {
                    ProgressAdapter.SecondaryProgress = GetProgress(++_loadCompleteCount, _loadTargetCount);
                });
                objLoader.Load(param);
            }

            // 採寸データのロード.
            if (measurementDataLoader.enabled)
            {
                _loadTargetCount++;
                var param = new LoadParam()
                {
                    ResourceId = _contents.ResourceId,
                    CreateTime = _contents.CreateTime,
                    ModifiedTime = _contents.ModifiedTime,
                    Path = _contents.MeasurementPath
                };
                measurementDataLoader.onUpdateMeasurementData.AddListener( loader => {
                    ProgressAdapter.SecondaryProgress = GetProgress(++_loadCompleteCount, _loadTargetCount);
                });
                measurementDataLoader.LoadFromCacheOrDownload(param);
            }

            // 体組成データのロード.
            if (bodyCompositionDataLoader.enabled)
            {
                _loadTargetCount++;
                var param = new LoadParam()
                {
                    ResourceId = _contents.ResourceId,
                    CreateTime = _contents.CreateTime,
                    ModifiedTime = _contents.ModifiedTime,
                    Path = _contents.BodyCompositionPath
                };
                bodyCompositionDataLoader.OnUpdateBodyCompositionData.AddListener( loader => {
                    ProgressAdapter.SecondaryProgress = GetProgress(++_loadCompleteCount, _loadTargetCount);
                });
                bodyCompositionDataLoader.LoadFromCacheOrDownload(param);
            }

            _loadCompleteCount = 0;

            ProgressAdapter.SecondaryProgress = 0;
            ProgressAdapter.OnProgressUpdateCompleted.AddListener( (adapter, progress) =>
            {
                FinishLoading(_contents);
            });

            onLoadStart.Invoke(_contents);
        }

        protected virtual void FinishLoading(Contents contents)
        {
            var device = contents.Device;
            if (device != null)
            {
                foreach (var objectLight in objLightList)
                {
                    objectLight.intensity *= device.Brightness.ToThreshold();
                }
            }
            onLoadCompleted.Invoke(contents);
        }

        private int GetProgress(int completeNum, int totalNum)
        {
            return Mathf.Clamp(ProgressAdapter.MaxProgress * completeNum / totalNum, ProgressAdapter.MinProgress, ProgressAdapter.MaxProgress);
        }
    }
}