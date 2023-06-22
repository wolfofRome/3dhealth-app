using Assets.Scripts.Network.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Calendar
{
    public class CalendarConfig : ScriptableObject
    {
        [Serializable]
        public class ExerciseImage
        {
            [SerializeField]
            private ExerciseId _id = default;
            public ExerciseId id {
                get {
                    return _id;
                }
            }

            [SerializeField]
            private Sprite _sprite = default;
            public Sprite sprite {
                get {
                    return _sprite;
                }
            }
        }

        [SerializeField]
        private List<ExerciseImage> _exerciseImagelist = default;
        
        private Dictionary<ExerciseId, ExerciseImage> _exerciseImageMap;
        public Dictionary<ExerciseId, ExerciseImage> exerciseImageMap {
            get {
                return _exerciseImageMap = _exerciseImageMap ?? _exerciseImagelist.ToDictionary(x => x.id);
            }
        }
        
        public const int MaxWeekOfMonth = 6;

        public const int OnceLoadNum = 12;

        void OnEnable()
        {
            _exerciseImageMap = _exerciseImagelist.ToDictionary(x => x.id);
        }
    }
}
