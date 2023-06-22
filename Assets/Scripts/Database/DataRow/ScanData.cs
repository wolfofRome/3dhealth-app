using Assets.Scripts.Common;
using SQLite4Unity3d;
using System;

namespace Assets.Scripts.Database.DataRow
{
    public enum FileType : int
    {
        Texture = 0,
        Mtl = 1,
        Obj = 2,
        Thumbnail = 3,
    }

    public static class FileTypeExt
    {
        public static string GetExtension(this FileType type)
        {
            string[] names = { ".bmp", ".mtl", ".obj", ".png" };
            return names[(int)type];
        }
    }

    public class ScanData : BaseDataRow
    {
        [PrimaryKey]
        public int UserId { get; set; }
        [PrimaryKey]
        public int ResourceId { get; set; }
        [PrimaryKey]
        public FileType Type { get; set; }
        public long CreateTime { get; set; }
        public long ModifiedTime { get; set; }
        public string Path { get; set; }

        [IgnoreAttribute]
        public DateTime CreateTimeAsDateTime {
            get {
                return DateTimeExtension.UnixTimestamp2LocalTime(CreateTime);
            }
        }

        [IgnoreAttribute]
        public DateTime ModifiedTimeAsDateTime {
            get {
                return DateTimeExtension.UnixTimestamp2LocalTime(ModifiedTime);
            }
        }

        public override string ToString()
        {
            return string.Format("[ScanData: UserId={0}, ResourceId={1}, Type={2}, CreateTime={3}, ModifiedTime={4}, Path={5}]", UserId, ResourceId, Type, CreateTime, ModifiedTime, Path);
        }

        public override bool PKEquals(object obj)
        {
            if (obj == null || !(obj is ScanData))
            {
                return false;
            }
            var sd = (ScanData)obj;
            return UserId == sd.UserId && ResourceId == sd.ResourceId && Type == sd.Type;
        }
    }
}