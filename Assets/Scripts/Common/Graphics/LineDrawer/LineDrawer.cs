using Assets.Scripts.Common.InputEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Common.Graphics.LineDrawer
{
    public class LineDrawer : BaseTouchEventListener
    {
        [SerializeField]
        private Camera _camera = default;

        [SerializeField]
        private Line _linePrefab = default;

        [SerializeField]
        private Line _lineEraserPrefab = default;

        [SerializeField]
        private float _lineWidth = 0.05f;

        [SerializeField]
        private float _lineEraserWidth = 0.4f;
        
        private const string _keyLineCount = "keyLineCount";
        private const string _keyIsEraser = "keyIsEraser";
        private const string _keyLinePoints = "keyLinePoints";

        private Line _currentLine;
        private Stack<Line> _lineHistory = new Stack<Line>();

        private bool _isEnableEraser = false;
        public bool isEnableEraser {
            get {
                return _isEnableEraser;
            }
            set {
                _isEnableEraser = value;
            }
        }

        protected override void Start()
        {
            // ライン情報を読込.
            Load();
            gameObject.SetActive(false);
        }

        protected override void Update()
        {
        }

        private Vector3 TransformLineCoordinate(Vector2 pos)
        {
            Vector3 wp = _camera.ScreenToWorldPoint(pos);
            wp.z = _camera.farClipPlane - (0.01f * _lineHistory.Count);
            return wp;
        }

        private Line CreateLine(bool isEraser)
        {
            Line line = null;

            if (isEraser)
            {
                line = Instantiate(_lineEraserPrefab);
                line.width = _lineEraserWidth;
                line.optionalParam = true;
            }
            else
            {
                line = Instantiate(_linePrefab);
                line.width = _lineWidth;
                line.optionalParam = false;
            }

            line.transform.SetParent(transform.parent, false);

            return line;
        }

        public override void OnBeginDrag(int pointerId, TouchEventData data)
        {
            switch (data.pointer.Count)
            {
                case 1:
                    _currentLine = CreateLine(isEnableEraser);
                    _currentLine.AddPoint(TransformLineCoordinate(data.pointer[pointerId].position));
                    _currentLine.Apply();
                    break;
                case 2:
                    if (!isEnableEraser)
                    {
                        var points = data.pointer.Values.ToArray();
                        _currentLine.ClearPoint();
                        _currentLine.AddPoint(TransformLineCoordinate(points[0].position));
                        _currentLine.AddPoint(TransformLineCoordinate(points[1].position));
                        _currentLine.Apply();
                    }
                    break;
            }
        }
        
        public override void OnDrag(int pointerId, TouchEventData data)
        {
            switch (data.pointer.Count)
            {
                case 1:
                    _currentLine.AddPoint(TransformLineCoordinate(data.pointer[pointerId].position));
                    _currentLine.Apply();
                    break;
                case 2:
                    if (!isEnableEraser)
                    {
                        var points = data.pointer.Values.ToArray();
                        _currentLine.ClearPoint();
                        _currentLine.AddPoint(TransformLineCoordinate(points[0].position));
                        _currentLine.AddPoint(TransformLineCoordinate(points[1].position));
                        _currentLine.Apply();
                    }
                    break;
            }
        }

        public override void OnEndDrag(int pointerId, TouchEventData data)
        {
            if (_currentLine != null && _currentLine.pointNum > 1)
            {
                if (!isEnableEraser && _currentLine.IsStraight())
                {
                    _currentLine.ToStraight();
                }
                _lineHistory.Push(_currentLine);
                Save();
            }
        }

        public override void OnPointerSingleClick(PointerEventData data)
        {
        }

        public override void OnPointerDoubleClick(PointerEventData data)
        {
            Undo();
        }

        public void Undo()
        {
            if (_lineHistory.Count > 0)
            {
                Destroy(_lineHistory.Pop().gameObject);
            }
            Save();
        }

        public void RemoveAllLines()
        {
            foreach (var item in _lineHistory)
            {
                Destroy(item.gameObject);
            }
            _lineHistory.Clear();
            ClearLinePrefs();
        }
        
        public void Save()
        {
            PlayerPrefs.SetInt(_keyLineCount, _lineHistory.Count);

            var count = _lineHistory.Count;
            var lineArray = _lineHistory.Reverse().ToArray();
            
            for (var i = 0; i < count; i++)
            {
                PlayerPrefsX.SetBool(_keyIsEraser + i, (bool)lineArray[i].optionalParam);
                PlayerPrefsX.SetVector3Array(_keyLinePoints + i, lineArray[i].vertexList.ToArray());
            }
        }

        public void Load()
        {
            var count = PlayerPrefs.GetInt(_keyLineCount);

            for (var i = 0; i < count; i++)
            {
                _currentLine = CreateLine(PlayerPrefsX.GetBool(_keyIsEraser + i));
                _currentLine.vertexList.AddRange(PlayerPrefsX.GetVector3Array(_keyLinePoints + i));
                _currentLine.transform.SetParent(transform.parent, false);
                _currentLine.Apply();
                _lineHistory.Push(_currentLine);
            }
        }

        public void ClearLinePrefs()
        {
            var count = PlayerPrefs.GetInt(_keyLineCount);

            for (var i = 0; i < count; i++)
            {
                PlayerPrefs.DeleteKey(_keyIsEraser + i);
                PlayerPrefs.DeleteKey(_keyLinePoints + i);
            }
            PlayerPrefs.DeleteKey(_keyLineCount);
        }
    }
}
