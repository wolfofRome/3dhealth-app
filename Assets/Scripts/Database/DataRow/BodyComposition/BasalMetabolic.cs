using System;
using Assets.Scripts.Common;

namespace Database.DataRow.BodyComposition
{
    internal static class BasalMetabolic
    {
        private enum AgeGroup
        {
            EighteenToTwentyNine,
            ThirtyToFortyNine,
            FiftyToSixtyNine,
            SeventyUp,
        }
        
        private static AgeGroup GetAgeGroup(int age)
        {
            //if (18 <= age && age <= 29)
            if (age <= 29)
                return AgeGroup.EighteenToTwentyNine;
            if (30 <= age && age <= 49)
                return AgeGroup.ThirtyToFortyNine;
            if (50 <= age && age <= 69)
                return AgeGroup.FiftyToSixtyNine;
            if (70 <= age)
                return AgeGroup.SeventyUp;
            throw new ArgumentOutOfRangeException();
        }
        
        public static string GetBasalMetabolicDescription(Gender gender, int age)
        {
            AgeGroup ageGroup = GetAgeGroup(age);
            switch (ageGroup)
            {
                case AgeGroup.EighteenToTwentyNine:
                    return gender == Gender.Male ? "1550kcal" : "1210kcal";
                case AgeGroup.ThirtyToFortyNine:
                    return gender == Gender.Male ? "1550kcal" : "1170kcal";
                case AgeGroup.FiftyToSixtyNine:
                    return gender == Gender.Male ? "1350kcal" : "1110kcal";
                case AgeGroup.SeventyUp:
                    return gender == Gender.Male ? "1220kcal" : "1010kcal";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}