using System;
using UnityEngine;

namespace Assets.Scripts.Network.Response
{
    public enum ExerciseId : int
    {
        MuscleTrainingChest = 1,
        MuscleTrainingArms = 2,
        MuscleTrainingBack = 3,
        MuscleTrainingAbdominal = 4,
        MuscleTrainingLowerBody = 5,
        Swimming = 6,
        RunningAndWalking = 7,
        Bicycle = 8,
        StretchAndYoga = 9,
        Other = 10,
        EMSArms = 11,
        EMSAbdominal = 12,
        EMSBack = 13,
        EMSLeg = 14,
        EMSAss = 15,
        EMSFace = 16,
        EMSSensorStretch = 17,
    }

    [Serializable]
    public class Exercise
    {
        [SerializeField]
        private int calendar_exercise_id = default;
        public ExerciseId ExerciseId {
            get {
                return (ExerciseId)calendar_exercise_id;
            }
            set {
                calendar_exercise_id = (int)value;
            }
        }
    }
}
