using System;
using Assets.Scripts.Common;
using Assets.Scripts.Database.DataRow;
using SQLite4Unity3d;

namespace Database.DataRow.BodyComposition
{
    /// 体組成
    /// SQLiteのローカルDB(Cache.db)のテーブルを変更する場合は「Assets/StreamingAssets/Cache_ver.txt」のバージョンをあげる必要がある。
    public class BodyComposition : BaseDataRow
    {
        public const string TypeMc780 = "mc-780";
        public const string TypeIScale10 = "i-scale1.0";

        [PrimaryKey]
        public int UserId { get; set; }
        [PrimaryKey]
        public int ResourceId { get; set; }
        public long CreateTime { get; set; }
        public long ModifiedTime { get; set; }
        public string Type { get; set; }
        public double Weight { get; set; }
        public double FatPercentage { get; set; }
        public double BasalMetabolism { get; set; }
        public double TrunkMuscle { get; set; }
        public int TrunkMuscleScore { get; set; }
        public double TrunkFat { get; set; }
        public double TrunkFatPercentage { get; set; }
        public int TrunkFatScore { get; set; }
        public double LeftArmMuscle { get; set; }
        public int LeftArmMuscleScore { get; set; }
        public double LeftArmFat { get; set; }
        public double LeftArmFatPercentage { get; set; }
        public int LeftArmFatScore { get; set; }
        public double RightArmMuscle { get; set; }
        public int RightArmMuscleScore { get; set; }
        public double RightArmFat { get; set; }
        public double RightArmFatPercentage { get; set; }
        public int RightArmFatScore { get; set; }
        public double LeftLegMuscle { get; set; }
        public int LeftLegMuscleScore { get; set; }
        public double LeftLegFat { get; set; }
        public double LeftLegFatPercentage { get; set; }
        public int LeftLegFatScore { get; set; }
        public double RightLegMuscle { get; set; }
        public int RightLegMuscleScore { get; set; }
        public double RightLegFat { get; set; }
        public double RightLegFatPercentage { get; set; }
        public int RightLegFatScore { get; set; }
        public double VisceralFatLevel { get; set; }
        public double Fat { get; set; }
        public double Muscle { get; set; }
        public double Skeletal { get; set; }
        public double Bmi { get; set; }

        [Ignore]
        public DateTime CreateTimeAsDateTime => DateTimeExtension.UnixTimestamp2LocalTime(CreateTime);

        [Ignore]
        public DateTime ModifiedTimeAsDateTime => DateTimeExtension.UnixTimestamp2LocalTime(ModifiedTime);

        public override string ToString()
        {
            return string.Format("[BodyComposition:" + "UserId={0}, ResourceId={1}, CreateTime={2}, ModifiedTime={3}, Weight={4}, FatPercentage={5}, BasalMetabolism={6}" +
                                                     "TrunkMuscle={7}, TrunkMuscleScore={8}, TrunkFat={9}, TrunkFatPercentage={10}, TrunkFatScore={11}," +
                                                     "LeftArmMuscle={12}, LeftArmMuscleScore={13}, LeftArmFat={14}, LeftArmFatPercentage={15}, LeftArmFatScore={16}, " +
                                                     "RightArmMuscle={17}, RightArmMuscleScore={18, RightArmFat={19}, RightArmFatPercentage={20}, RightArmFatScore={21}, " +
                                                     "LeftLegMuscle={22}, LeftLegMuscleScore={23}, LeftLegFat={24}, LeftLegFatPercentage={25}, LeftLegFatScore={26}," +
                                                     "RightLegMuscle={27}, RightLegMuscleScore={28}, RightLegFat={29}, RightLegFatPercentage={30}, RightLegFatScore={31}," +
                                                     "VisceralFatLevel={32}, Fat={33}, Muscle={34}]",
                                                     UserId, ResourceId, CreateTime, ModifiedTime, Weight, FatPercentage, BasalMetabolism,
                                                     TrunkMuscle, TrunkMuscleScore, TrunkFat, TrunkFatPercentage, TrunkFatScore,
                                                     LeftArmMuscle, LeftArmMuscleScore, LeftArmFat, LeftArmFatPercentage, LeftArmFatScore,
                                                     RightArmMuscle, RightArmMuscleScore, RightArmFat, RightArmFatPercentage, RightArmFatScore,
                                                     LeftLegMuscle, LeftLegMuscleScore, LeftLegFat, LeftLegFatPercentage, LeftLegFatScore,
                                                     RightLegMuscle, RightLegMuscleScore, RightLegFat, RightLegFatPercentage, RightLegFatScore,
                                                     VisceralFatLevel, Fat, Muscle);
        }

        public override bool PKEquals(object obj)
        {
            if (!(obj is BodyComposition))
            {
                return false;
            }
            var bc = (BodyComposition)obj;
            return UserId == bc.UserId && ResourceId == bc.ResourceId;
        }
        
        [Ignore]
        public int MuscleScore
        {
            get
            {
                // 性別
                var gender = DataManager.Instance.Profile.Gender;

                // 筋肉率 = 筋肉量 / 体重
                var musclePercentage = (float)(Muscle / Weight);

                // 筋肉量スコア
                int muscleScore = 0;

                if (gender == Gender.Male)
                {
                    // 男性
                    if (musclePercentage < 16)
                    {
                        muscleScore = -4;
                    }
                    else if (musclePercentage < 21)
                    {
                        muscleScore = -3;
                    }
                    else if (musclePercentage < 26)
                    {
                        muscleScore = -2;
                    }
                    else if (musclePercentage < 31)
                    {
                        muscleScore = -1;
                    }
                    else if (musclePercentage < 35)
                    {
                        muscleScore = 0;
                    }
                    else if (musclePercentage < 40)
                    {
                        muscleScore = 1;
                    }
                    else if (musclePercentage < 50)
                    {
                        muscleScore = 2;
                    }
                    else if (musclePercentage < 60)
                    {
                        muscleScore = 3;
                    }
                    else
                    {
                        muscleScore = 4;
                    }
                }
                else if (gender == Gender.Male)
                {
                    // 女性
                    if (musclePercentage < 15)
                    {
                        muscleScore = -4;
                    }
                    else if (musclePercentage < 20)
                    {
                        muscleScore = -3;
                    }
                    else if (musclePercentage < 23)
                    {
                        muscleScore = -2;
                    }
                    else if (musclePercentage < 26)
                    {
                        muscleScore = -1;
                    }
                    else if (musclePercentage < 28)
                    {
                        muscleScore = 0;
                    }
                    else if (musclePercentage < 30)
                    {
                        muscleScore = 1;
                    }
                    else if (musclePercentage < 40)
                    {
                        muscleScore = 2;
                    }
                    else if (musclePercentage < 50)
                    {
                        muscleScore = 3;
                    }
                    else
                    {
                        muscleScore = 4;
                    }
                }

                return muscleScore;
            }
        }
        
        [Ignore]
        public int FatScore
        {
            get
            {
                // 性別
                var gender = DataManager.Instance.Profile.Gender;

                // 体脂肪率
                var fatPercentage = FatPercentage;

                // 体脂肪率スコア
                int fatScore = 0;

                if (gender == Gender.Male)
                {
                    // 男性
                    if (fatPercentage < 5)
                    {
                        fatScore = -4;
                    }
                    else if (fatPercentage < 8)
                    {
                        fatScore = -3;
                    }
                    else if (fatPercentage < 11)
                    {
                        fatScore = -2;
                    }
                    else if (fatPercentage < 14)
                    {
                        fatScore = -1;
                    }
                    else if (fatPercentage < 18)
                    {
                        fatScore = 0;
                    }
                    else if (fatPercentage < 22)
                    {
                        fatScore = 1;
                    }
                    else if (fatPercentage < 27)
                    {
                        fatScore = 2;
                    }
                    else if (fatPercentage < 35)
                    {
                        fatScore = 3;
                    }
                    else
                    {
                        fatScore = 4;
                    }
                }
                else if (gender == Gender.Male)
                {
                    // 女性
                    if (fatPercentage < 15)
                    {
                        fatScore = -4;
                    }
                    else if (fatPercentage < 18)
                    {
                        fatScore = -3;
                    }
                    else if (fatPercentage < 21)
                    {
                        fatScore = -2;
                    }
                    else if (fatPercentage < 24)
                    {
                        fatScore = -1;
                    }
                    else if (fatPercentage < 31)
                    {
                        fatScore = 0;
                    }
                    else if (fatPercentage < 35)
                    {
                        fatScore = 1;
                    }
                    else if (fatPercentage < 40)
                    {
                        fatScore = 2;
                    }
                    else if (fatPercentage < 45)
                    {
                        fatScore = 3;
                    }
                    else
                    {
                        fatScore = 4;
                    }
                }

                return fatScore;
            }
        }
    }
}