using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common.Graphics.LineDrawer
{
    [RequireComponent(typeof(LineRenderer))]
    public class Line : MonoBehaviour
    {
        [SerializeField]
        private float _straightThreshold = 0.3f;
        
        private object _optionalParam = null;
        public object optionalParam {
            get {
                return _optionalParam;
            }
            set {
                _optionalParam = value;
            }
        }

        private float _width = 0.05f;
        public float width {
            get {
                return _width;
            }
            set {
                _width = value;
                lineRenderer.startWidth = value;
                lineRenderer.endWidth = value;
            }
        }

        private LineRenderer _lineRenderer;
        private LineRenderer lineRenderer {
            get {
                return _lineRenderer = _lineRenderer ?? GetComponent<LineRenderer>();
            }
        }

        [SerializeField]
        private List<Vector3> _vertexList = new List<Vector3>();
        public List<Vector3> vertexList {
            get {
                return _vertexList;
            }
        }

        public int pointNum {
            get {
                return _vertexList.Count;
            }
        }

        public Vector3? firstPoint {
            get {
                return pointNum > 0 ? (Vector3?)_vertexList[0] : null;
            }
        }

        public Vector3? lastPoint {
            get {
                return pointNum > 0 ? (Vector3?)_vertexList[pointNum - 1] : null;
            }
        }

        public Vector3? midPoint {
            get {
                Vector3? point = null;
                if (firstPoint != null && lastPoint != null)
                {
                    if (pointNum == 2)
                    {
                        point = (firstPoint + lastPoint) / 2;
                    }
                    else
                    {
                        point = _vertexList[pointNum / 2];
                    }
                }
                return point;
            }
        }

        void OnValidate()
        {
            Apply();
        }
        
        void Start()
        {
            lineRenderer.useWorldSpace = true;
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
        }
        
        void Update()
        {
        }

        public void AddPoint(Vector3 point)
        {
            vertexList.Add(point);
        }

        public void ClearPoint()
        {
            vertexList.Clear();
        }

        public void Apply()
        {
            lineRenderer.positionCount = pointNum;
            lineRenderer.SetPositions(vertexList.ToArray());
        }

        public void ToStraight()
        {
            var first = firstPoint;
            var last = lastPoint;
            if (first != null && last != null)
            {
                vertexList.Clear();
                vertexList.Add((Vector3)first);
                vertexList.Add((Vector3)last);
                Apply();
            }
        }
        
        public bool IsStraight()
        {
            var first = (Vector3)firstPoint;
            var last = (Vector3)lastPoint;
            var num = pointNum;

            for (int i = 1; i < num - 1; i++)
            {
                float distance = LineToPointDistance(first, last, vertexList[i]);
                if (distance > _straightThreshold)
                {
                    return false;
                }
            }
            return true;
        }

        private Vector2 NearestPointOnLine(Vector2 p1, Vector2 p2, Vector2 p, bool isSegment = true)
        {
            var dp2 = p2 - p1;
            var sqrMagnitude = dp2.sqrMagnitude;

            if (sqrMagnitude < Vector2.kEpsilon) return p1;

            var dp = p - p1;

            var t = (dp2.x * dp.x + dp2.y * dp.y) / sqrMagnitude;
            
            if (isSegment)
            {
                if (t <= 0) return p1;
                if (t >= 1) return p2;
            }
            
            return new Vector2((1 - t) * p1.x + t * p2.x, (1 - t) * p1.y + t * p2.y);
        }

        private float LineToPointDistance(Vector2 p1, Vector2 p2, Vector2 p, bool isSegment = true)
        {
            return (p - NearestPointOnLine(p1, p2, p, isSegment)).magnitude;
        }
    }
}
