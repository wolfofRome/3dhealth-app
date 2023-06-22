using UnityEngine;

namespace Assets.Scripts.ThreeDView
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class WireFrameObjCreator : MonoBehaviour
    {
        private Color _wireFrameColor = new Color(0.196f, 0.196f, 0.196f);
        public Color WireFrameColor {
            get {
                return _wireFrameColor;
            }
        }

        private SkinnedMeshRenderer _targetRenderer;
        private SkinnedMeshRenderer TargetRenderer {
            get {
                return _targetRenderer = _targetRenderer ?? GetComponent<SkinnedMeshRenderer>();
            }
            set {
                _targetRenderer = value;
            }
        }
        private SkinnedMeshRenderer _wireFrameRenderer;

        private GameObject _wireFrameObject;
        private GameObject WireFrameObject {
            get {
                return _wireFrameObject = _wireFrameObject ?? new GameObject(TargetRenderer.gameObject.name + "_wire");
            }
            set {
                _wireFrameObject = value;
            }
        }


#if true
        void Awake()
        {
            TargetRenderer = GetComponent<SkinnedMeshRenderer>();
        }

        public void CreateWireFrame()
        {
            // WireFrame用のGameObjectを生成
            WireFrameObject.transform.SetParent(TargetRenderer.gameObject.transform);
            WireFrameObject.layer = TargetRenderer.gameObject.layer;
            _wireFrameRenderer = WireFrameObject.AddComponent<SkinnedMeshRenderer>();
            _wireFrameRenderer.sharedMesh = Instantiate(TargetRenderer.sharedMesh);
            _wireFrameRenderer.localBounds = TargetRenderer.localBounds;
            _wireFrameRenderer.rootBone = TargetRenderer.rootBone;
            _wireFrameRenderer.bones = TargetRenderer.bones;
            _wireFrameRenderer.quality = TargetRenderer.quality;

            // ブレンドシェイプの設定をコピー
            for (int i = 0; i < _wireFrameRenderer.sharedMesh.blendShapeCount; i++)
            {
                _wireFrameRenderer.SetBlendShapeWeight(i, TargetRenderer.GetBlendShapeWeight(i));
            }

            Material[] materials = new Material[TargetRenderer.materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = CreateMaterial(_wireFrameColor);
            }

            _wireFrameRenderer.materials = materials;
            for (int i = 0; i < TargetRenderer.sharedMesh.subMeshCount; i++)
            {
                _wireFrameRenderer.sharedMesh.SetIndices(TrianglesToLines(TargetRenderer.sharedMesh.GetIndices(i)), MeshTopology.Lines, i);
            }
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        private Material CreateMaterial(Color color)
        {
            Material material = Resources.Load<Material>("WireFrameMaterial");
            material.SetColor("_Color", color);
            return material;
        }

        /// <summary>
        /// 三角形の頂点情報を線の頂点に変換する
        /// </summary>
        /// <param name="triangles"></param>
        /// <returns></returns>
        private int[] TrianglesToLines(int[] triangles)
        {
            int[] indices = new int[2 * triangles.Length];
            int i = 0;
            // MeshTopology.Trianglesの頂点をMeshTopology.Lines用の頂点に変換する
            for (int t = 0; t < triangles.Length; t += 3)
            {
                indices[i++] = triangles[t];
                indices[i++] = triangles[t + 1];

                indices[i++] = triangles[t + 1];
                indices[i++] = triangles[t + 2];

                indices[i++] = triangles[t + 2];
                indices[i++] = triangles[t];
            }
            return indices;
        }

        public void SetActive(bool value)
        {
            WireFrameObject.SetActive(value);
        }

        public void SetColor(Color color)
        {
            if (_wireFrameRenderer != null)
            {
                foreach (Material material in _wireFrameRenderer.materials)
                {
                    material.SetColor("_Color", color);
                }
            }
            _wireFrameColor = color;
        }

#else
        private Material _mat;
        public Color _color = Color.white;
        private Vector3[] _vertices;

        public void Start() {
            CreateMaterial();
            SetVertice();
        }

        void OnRenderObject() {
            DrawLine();
        }

        private void CreateMaterial() {
            //_mat = new Material
            //(
            //"Shader \"Hidden/Invert\" {" +
            //"SubShader {" +
            //"    Pass {" +
            //"    Blend SrcAlpha OneMinusSrcAlpha" +
            //"    Blend Off ZTest Always Cull Off ZWrite Off" +
            //"    BindChannels {" +
            //"    Bind \"Color\", color" +
            //"    }" +
            //"    }" +
            //"    }" +
            //"}"
            //);

            _mat = new Material(Shader.Find("Unlit/Color"));
            _mat.color = Color.gray;
        }

        private void SetVertice() {
            SkinnedMeshRenderer smr = this.transform.GetComponent<SkinnedMeshRenderer>();
            Vector3[] vertices = smr.sharedMesh.vertices;
            int[] triangles = smr.sharedMesh.triangles;

            _vertices = new Vector3[triangles.Length];
            for (int i = 0; i < triangles.Length / 3; i++) {
                _vertices[i * 3 + 0] = vertices[triangles[i * 3 + 0]];
                _vertices[i * 3 + 1] = vertices[triangles[i * 3 + 1]];
                _vertices[i * 3 + 2] = vertices[triangles[i * 3 + 2]];
            }
        }

        private void DrawLine() {
            Vector3 _vPos = transform.position;
            Quaternion _qRot = transform.localRotation;
            Vector3 _trScl = this.transform.localScale;
            Matrix4x4 _matrix = Matrix4x4.TRS(_vPos, _qRot, _trScl);

            _mat.SetPass(0);

            GL.PushMatrix();
            GL.MultMatrix(_matrix);
            GL.Begin(GL.LINES);
            GL.Color(_color);

            for (int i = 0; i < _vertices.Length / 3; i++) {
                GL.Vertex(_vertices[i * 3]);
                GL.Vertex(_vertices[i * 3 + 1]);

                GL.Vertex(_vertices[i * 3 + 1]);
                GL.Vertex(_vertices[i * 3 + 2]);

                GL.Vertex(_vertices[i * 3 + 2]);
                GL.Vertex(_vertices[i * 3]);
            }
            GL.End();
            GL.PopMatrix();
        }

        public void SetActive(bool value) {
        }

        public void SetColor(Color color) {
        }
#endif
    }
}