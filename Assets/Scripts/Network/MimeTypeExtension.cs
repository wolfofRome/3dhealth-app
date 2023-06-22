using System.Linq;
using System.Collections.Generic;

namespace Assets.Scripts.Network
{
    public enum MimeType
    {
        Unknown = -1,
        Json = 0,
        Csv = 1,
        Zip = 2,
        Xml = 3,
    }

    public static class MimeTypeExtension
    {
        private static readonly Dictionary<MimeType, string> MimeTypeMap = new Dictionary<MimeType, string>()
        {
            { MimeType.Json,"application/json"},
            { MimeType.Csv, "application/csv"},
            { MimeType.Zip, "application/zip"},
            { MimeType.Xml, "text/xml"}
        };

        public static string ToString(this MimeType type)
        {
            return MimeTypeMap[type];
        }

        public static bool TryParse(string value, out MimeType type)
        {
            type = MimeType.Unknown;

            var lowerValue = value.ToLower();
            foreach (var item in MimeTypeMap)
            {
                if (item.Value == lowerValue)
                {
                    type = item.Key;
                    return true;
                }
            }
            return false;
        }
    }
}
