using System;
using System.Globalization;
using Assets.Scripts.Common;

namespace Database.DataRow.BodyComposition
{
    internal static class MuscleMass
    {
        private enum AgeGroup
        {
            EighteenToTwentyFour,
            TwentyFiveToFortyFour,
            FortyFiveToSixtyFour,
            SixtyFiveToSevenFour,
            SeventyFiveUp,
        }
        
        private static AgeGroup GetAgeGroup(int age)
        {
            if (18 <= age && age <= 24)
                return AgeGroup.EighteenToTwentyFour;
            if (25 <= age && age <= 44)
                return AgeGroup.TwentyFiveToFortyFour;
            if (45 <= age && age <= 64)
                return AgeGroup.FortyFiveToSixtyFour;
            if (65 <= age && age <= 74)
                return AgeGroup.SixtyFiveToSevenFour;
            if (age >= 75)
                return AgeGroup.SeventyFiveUp;
            throw new ArgumentOutOfRangeException();
        }
        
        public static string GetMuscleMassDescription(Gender gender, int age, StandardBodyCompositionData.BodyPart bodyPart)
        {
            AgeGroup ageGroup = GetAgeGroup(age);
            string description;
            switch (bodyPart)
            {
                case StandardBodyCompositionData.BodyPart.Trunk:
                    description = GetTrunkMuscleMass(gender, ageGroup).ToString(CultureInfo.InvariantCulture);
                    break;
                case StandardBodyCompositionData.BodyPart.RightArm:
                    description = GetRightUpperLimbMuscleMass(gender, ageGroup).ToString(CultureInfo.InvariantCulture);
                    break;
                case StandardBodyCompositionData.BodyPart.LeftArm:
                    description = GetLeftUpperLimbMuscleMass(gender, ageGroup).ToString(CultureInfo.InvariantCulture);
                    break;
                case StandardBodyCompositionData.BodyPart.RightLeg:
                    description = GetRightLowerLimbMuscleMass(gender, ageGroup).ToString(CultureInfo.InvariantCulture);
                    break;
                case StandardBodyCompositionData.BodyPart.LeftLeg:
                    description = GetLeftLowerLimbMuscleMass(gender, ageGroup).ToString(CultureInfo.InvariantCulture);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bodyPart), bodyPart, null);
            }
            return description + "kg";
        }
        
        private static double GetTrunkMuscleMass(Gender gender, AgeGroup ageGroup)
        {
            switch (ageGroup)
            {
                case AgeGroup.EighteenToTwentyFour:
                    return Gender.Male == gender ? 25.9 : 17.5; 
                case AgeGroup.TwentyFiveToFortyFour:
                    return Gender.Male == gender ? 28.3 : 19.2;
                case AgeGroup.FortyFiveToSixtyFour:
                    return Gender.Male == gender ? 28.4 : 20.1;
                case AgeGroup.SixtyFiveToSevenFour:
                    return Gender.Male == gender ? 26.4 : 19.4;
                case AgeGroup.SeventyFiveUp:
                    return Gender.Male == gender ? 25.2 : 18.8;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private static double GetRightUpperLimbMuscleMass(Gender gender, AgeGroup ageGroup)
        {
            switch (ageGroup)
            {
                case AgeGroup.EighteenToTwentyFour:
                    return Gender.Male == gender ? 2.5 : 1.5; 
                case AgeGroup.TwentyFiveToFortyFour:
                    return Gender.Male == gender ? 2.7 : 1.8;
                case AgeGroup.FortyFiveToSixtyFour:
                    return Gender.Male == gender ? 2.8 : 1.7;
                case AgeGroup.SixtyFiveToSevenFour:
                    return Gender.Male == gender ? 2.5 : 1.5;
                case AgeGroup.SeventyFiveUp:
                    return Gender.Male == gender ? 2.3 : 1.5;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private static double GetLeftUpperLimbMuscleMass(Gender gender, AgeGroup ageGroup)
        {
            switch (ageGroup)
            {
                case AgeGroup.EighteenToTwentyFour:
                    return Gender.Male == gender ? 2.4 : 1.5; 
                case AgeGroup.TwentyFiveToFortyFour:
                    return Gender.Male == gender ? 2.6 : 1.7;
                case AgeGroup.FortyFiveToSixtyFour:
                    return Gender.Male == gender ? 2.6 : 1.6;
                case AgeGroup.SixtyFiveToSevenFour:
                    return Gender.Male == gender ? 2.4 : 1.5;
                case AgeGroup.SeventyFiveUp:
                    return Gender.Male == gender ? 2.1 : 1.5;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private static double GetRightLowerLimbMuscleMass(Gender gender, AgeGroup ageGroup)
        {                    
            switch (ageGroup)
            {
                case AgeGroup.EighteenToTwentyFour:
                    return Gender.Male == gender ? 9.8 : 6.8; 
                case AgeGroup.TwentyFiveToFortyFour:
                    return Gender.Male == gender ? 10.2 : 7.0;
                case AgeGroup.FortyFiveToSixtyFour:
                    return Gender.Male == gender ? 9.9 : 6.3;
                case AgeGroup.SixtyFiveToSevenFour:
                    return Gender.Male == gender ? 8.9 : 5.6;
                case AgeGroup.SeventyFiveUp:
                    return Gender.Male == gender ? 7.6 : 5.4;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private static double GetLeftLowerLimbMuscleMass(Gender gender, AgeGroup ageGroup)
        {    
            switch (ageGroup)
            {
                case AgeGroup.EighteenToTwentyFour:
                    return Gender.Male == gender ? 9.8 : 6.8; 
                case AgeGroup.TwentyFiveToFortyFour:
                    return Gender.Male == gender ? 9.9 : 6.9;
                case AgeGroup.FortyFiveToSixtyFour:
                    return Gender.Male == gender ? 9.7 : 6.1;
                case AgeGroup.SixtyFiveToSevenFour:
                       return Gender.Male == gender ? 8.6 : 5.5;
                case AgeGroup.SeventyFiveUp:
                    return Gender.Male == gender ? 7.5 : 5.2;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}