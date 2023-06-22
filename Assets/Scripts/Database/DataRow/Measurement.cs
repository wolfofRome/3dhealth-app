using Assets.Scripts.Common;
using SQLite4Unity3d;
using System;

namespace Assets.Scripts.Database.DataRow
{
    /// 採寸
    /// SQLiteのローカルDB(Cache.db)のテーブルを変更する場合は「Assets/StreamingAssets/Cache_ver.txt」のバージョンをあげる必要がある。
    public class Measurement : BaseDataRow
    {
        public const string TypeSpaceVision = "space_vision";
        public const string Type3DBodyLab = "3d_body_lab";

        [PrimaryKey] public int UserId { get; set; }
        [PrimaryKey] public int ResourceId { get; set; }
        public long CreateTime { get; set; }
        public long ModifiedTime { get; set; }
        public string Type { get; set; }
        public int Height { get; set; }
        public int Inseam { get; set; }
        public int Neck { get; set; }
        public int Chest { get; set; }
        public int MinWaist { get; set; }
        public int MaxWaist { get; set; }
        public int Hip { get; set; }
        public int LeftThigh { get; set; }
        public int RightThigh { get; set; }
        public int LeftShoulder { get; set; }
        public int RightShoulder { get; set; }
        public int LeftSleeve { get; set; }
        public int RightSleeve { get; set; }
        public int LeftArm { get; set; }
        public int RightArm { get; set; }
        public int LeftCalf { get; set; }
        public int RightCalf { get; set; }

        [IgnoreAttribute] public DateTime CreateTimeAsDateTime => DateTimeExtension.UnixTimestamp2LocalTime(CreateTime);

        [IgnoreAttribute] public DateTime ModifiedTimeAsDateTime => DateTimeExtension.UnixTimestamp2LocalTime(ModifiedTime);

        [IgnoreAttribute] public float correctionValue { get; set; }

        [IgnoreAttribute] public float correctedHeight => Height + correctionValue;

        [IgnoreAttribute] public float correctedInseam => Inseam + correctionValue;

        public override string ToString()
        {
            return $"[Measurement:  UserId={UserId}, ResourceId={ResourceId}, CreateTime={CreateTime}, ModifiedTime={ModifiedTime}, " + 
                   $"Height={Height}, Inseam={Inseam}, Neck={Neck}, Chest={Chest}, MinWaist={MinWaist}, MaxWaist={MaxWaist}, Hip={Hip}, " +
                   $"LeftThigh={LeftThigh}, RightThigh={RightThigh}, LeftShoulder={LeftShoulder}, RightShoulder={RightShoulder}, LeftSleeve={LeftSleeve}, " +
                   $"RightSleeve={RightSleeve}, LeftArm={LeftArm}, RightArm={RightArm}, LeftCalf={LeftCalf}, RightCalf={RightCalf}]";
        }

        public override bool PKEquals(object obj)
        {
            if (!(obj is Measurement))
            {
                return false;
            }

            var measurement = (Measurement) obj;
            return UserId == measurement.UserId && ResourceId == measurement.ResourceId;
        }
    }
}