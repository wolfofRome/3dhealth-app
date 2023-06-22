using Assets.Scripts.Common;
using Assets.Scripts.Common.Util;
using Assets.Scripts.ThreeDView;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.Scripts.Common
{
    public class ObjController : BaseObjController
    {
        [FormerlySerializedAs("_alphaControllerList")] [SerializeField]
        private List<MeshAlphaController> alphaControllerList = new List<MeshAlphaController>();

        [FormerlySerializedAs("_colorControllerList")] [SerializeField]
        private List<MeshColorController> colorControllerList = new List<MeshColorController>();

        private AvatarController _avatar;
        
        public override Vector3 ObjectPosition
        {
            get => base.ObjectPosition;

            set
            {
                if (TargetObject == null || _avatar == null) return;
                base.ObjectPosition = value;
                TargetObject.transform.position = value;
                _avatar.transform.position = value;
            }
        }

        protected override void Start()
        {
        }
        
        protected override void Update()
        {
        }

        public override void OnLoadObj(ObjLoader loader, GameObject obj)
        {
            base.OnLoadObj(loader, obj);

            RegisterRendererController(obj);
            
            foreach (var controller in alphaControllerList)
            {
                controller.Apply();
            }

            foreach (var controller in colorControllerList)
            {
                controller.Apply();
            }

            UpdateAvatar();
        }

        public void RegisterRendererController(GameObject obj)
        {
            Renderer[] objRenderers = obj.GetComponentsInChildren<Renderer>();
            foreach (Renderer objectRenderer in objRenderers)
            {
                RendererController rc = objectRenderer.GetComponent<RendererController>();
                if (rc == null)
                {
                    rc = objectRenderer.gameObject.AddComponent<RendererController>();
                }

                foreach (var controller in alphaControllerList)
                {
                    controller.AddRendererController(rc);
                }

                foreach (var controller in colorControllerList)
                {
                    controller.AddRendererController(rc);
                }
            }
        }

        public void OnChangeBodyType(Gender gender, BodyType type, BaseHuman human)
        {
            _avatar = human.AvatarController;
            UpdateAvatar();
        }

        private void UpdateAvatar()
        {
            if (Loader == null || _avatar == null)
            {
                return;
            }
            _avatar.transform.position = TargetObject.transform.position;
            _avatar.UpdateAvatar(Loader.ObjInfo, ObjectPosition);
        }
    }
    
    public abstract class BaseObjController : MonoBehaviour
    {
        [FormerlySerializedAs("_objLayer")] [SerializeField]
        private LayerMask objLayer = default;
        [FormerlySerializedAs("_objPosition")] [SerializeField]
        private Vector3 objectPosition = Vector3.zero;
        public virtual Vector3 ObjectPosition
        {
            get => objectPosition;
            set => objectPosition = value;
        }

        public virtual Bounds objBounds { get; private set; }

        protected ObjLoader Loader;

        protected GameObject TargetObject;
        private string _modeInfo;

        // Use this for initialization
        protected virtual void Start()
        {
        }

        // Update is called once per frame
        protected virtual void Update()
        {
        }

        public virtual void OnLoadObj(ObjLoader loader, GameObject obj)
        {
            TargetObject = obj;
            Loader = loader;
            TargetObject.layer = (int)Mathf.Log(objLayer.value, 2);

            var children = TargetObject.GetComponentsInChildren<Transform>();
            foreach (var child in children)
            {
                child.gameObject.layer = TargetObject.layer;
            }

            // place the bottom on the floor
            if (TargetObject.transform.position == Vector3.zero)
            {
                var bounds = RendererUtil.GetEncapsulateBounds(TargetObject.GetComponentsInChildren<Renderer>());
                objectPosition.y -= bounds.min.y;
                objectPosition.z -= bounds.center.z;
                TargetObject.transform.position = objectPosition;
            }
            else
            {
                objectPosition = TargetObject.transform.position;
            }

            objBounds = RendererUtil.GetEncapsulateBounds(TargetObject.GetComponentsInChildren<Renderer>());
            _modeInfo = GetModelInfo(TargetObject, objBounds);
        }

        private static string GetModelInfo(GameObject go, Bounds bounds)
        {
            string infoString = "";
            int meshCount = 0;
            int subMeshCount = 0;
            int vertexCount = 0;
            int triangleCount = 0;

            MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>();
            if (meshFilters != null) meshCount = meshFilters.Length;
            if (meshFilters != null)
                foreach (MeshFilter mf in meshFilters)
                {
                    Mesh mesh = mf.mesh;
                    subMeshCount += mesh.subMeshCount;
                    vertexCount += mesh.vertices.Length;
                    triangleCount += mesh.triangles.Length / 3;
                }

            infoString = infoString + meshCount + " mesh(es)\n";
            infoString = infoString + subMeshCount + " sub meshes\n";
            infoString = infoString + vertexCount + " vertices\n";
            infoString = infoString + triangleCount + " triangles\n";
            infoString = infoString + bounds.size + " meters";
            return infoString;
        }
    }
}
