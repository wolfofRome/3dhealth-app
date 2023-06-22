using System;
using Assets.Scripts.Common;

namespace Database.DataRow.BodyComposition
{
    internal static class BodyFat
    {
        private enum AgeGroup
        {
            EighteenToThirtyNine,
            FortyToFiftyNine,
            SixtyUp,
        }
        
        public static string GetBodyFatDescription(Gender gender, int age, int bodyFatRate)
        {
            if (IsEssentialFatBody(gender, age, bodyFatRate))
                return "痩せ";
            if (IsAthletesBody(gender, age, bodyFatRate))
                return "標準ー";
            if (IsFitnessBody(gender, age, bodyFatRate))
                return "標準＋";
            if (IsAverageBody(gender, age, bodyFatRate))
                return "軽肥満";
            if (IsObeseBody(gender, age, bodyFatRate))
                return "肥満";
            throw new AggregateException();
        }

        private static AgeGroup GetAgeGroup(int age)
        {
            //if (18 <= age && age <= 39)
            if (age <= 39)
                return AgeGroup.EighteenToThirtyNine;
            if (40 <= age && age <= 59)
                return AgeGroup.FortyToFiftyNine;
            if (60 <= age)
                return AgeGroup.SixtyUp;
            throw new ArgumentOutOfRangeException();
        }

        private static bool IsEssentialFatBody(Gender gender, int age, int bodyFatRate)
        {
            AgeGroup ageGroup = GetAgeGroup(age);
            switch (ageGroup)
            {
                case AgeGroup.EighteenToThirtyNine:
                    return Gender.Male == gender ? bodyFatRate <= 10 : bodyFatRate <= 20;
                case AgeGroup.FortyToFiftyNine:
                    return Gender.Male == gender ? bodyFatRate <= 11 : bodyFatRate <= 21;
                case AgeGroup.SixtyUp:
                    return Gender.Male == gender ? bodyFatRate <= 13 : bodyFatRate <= 22;
                default:
                    return false;
            }
        }

        private static bool IsAthletesBody(Gender gender, int age, int bodyFatRate)
        {
            AgeGroup ageGroup = GetAgeGroup(age);
            switch (ageGroup)
            {
                case AgeGroup.EighteenToThirtyNine:
                    return Gender.Male == gender ? (11 <= bodyFatRate && bodyFatRate <= 16) : (21 <= bodyFatRate && bodyFatRate <= 27);
                case AgeGroup.FortyToFiftyNine:
                    return Gender.Male == gender ? (12 <= bodyFatRate && bodyFatRate <= 17) : (22 <= bodyFatRate && bodyFatRate <= 28);
                case AgeGroup.SixtyUp:
                    return Gender.Male == gender ? (14 <= bodyFatRate && bodyFatRate <= 19) : (23 <= bodyFatRate && bodyFatRate <= 29);
                default:
                    return false;
            }
        }

        private static bool IsFitnessBody(Gender gender, int age, int bodyFatRate)
        {
            AgeGroup ageGroup = GetAgeGroup(age);
            switch (ageGroup)
            {
                case AgeGroup.EighteenToThirtyNine:
                    return Gender.Male == gender ? (17 <= bodyFatRate && bodyFatRate <= 21) : (28 <= bodyFatRate && bodyFatRate <= 34);
                case AgeGroup.FortyToFiftyNine:
                    return Gender.Male == gender ? (18 <= bodyFatRate && bodyFatRate <= 22) : (29 <= bodyFatRate && bodyFatRate <= 35);
                case AgeGroup.SixtyUp:
                    return Gender.Male == gender ? (20 <= bodyFatRate && bodyFatRate <= 24) : (30 <= bodyFatRate && bodyFatRate <= 36);
                default:
                    return false;
            }
        }

        private static bool IsAverageBody(Gender gender, int age, int bodyFatRate)
        {
            AgeGroup ageGroup = GetAgeGroup(age);
            switch (ageGroup)
            {
                case AgeGroup.EighteenToThirtyNine:
                    return Gender.Male == gender ? (22 <= bodyFatRate && bodyFatRate <= 26) : (35 <= bodyFatRate && bodyFatRate <= 39);
                case AgeGroup.FortyToFiftyNine:
                    return Gender.Male == gender ? (23 <= bodyFatRate && bodyFatRate <= 27) : (36 <= bodyFatRate && bodyFatRate <= 40);
                case AgeGroup.SixtyUp:
                    return Gender.Male == gender ? (25 <= bodyFatRate && bodyFatRate <= 29) : (37 <= bodyFatRate && bodyFatRate <= 41);
                default:
                    return false;
            }
        }

        private static bool IsObeseBody(Gender gender, int age, int bodyFatRate)
        {
            AgeGroup ageGroup = GetAgeGroup(age);
            switch (ageGroup)
            {
                case AgeGroup.EighteenToThirtyNine:
                    return Gender.Male == gender ? bodyFatRate >= 27 : bodyFatRate >= 40;
                case AgeGroup.FortyToFiftyNine:
                    return Gender.Male == gender ? bodyFatRate >= 28 : bodyFatRate >= 41;
                case AgeGroup.SixtyUp:
                    return Gender.Male == gender ? bodyFatRate >= 29 : bodyFatRate >= 42;
                default:
                    return false;
            }
        }
    }
}