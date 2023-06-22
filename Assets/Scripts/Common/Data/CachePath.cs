using System.IO;
using UnityEngine;

namespace Assets.Scripts.Common.Data
{
    public class CachePath
    {
        private int _userId;
        public CachePath(int userId) {
            _userId = userId;
        }

        private static string Root {
            get {
                return Application.temporaryCachePath + "/";
            }
        }

        public static string CommonDataRoot {
            get {
                string path = Root + "common/";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }
        
        public string UserDataDirPath {
            get {
                string path = Root + _userId + "/";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        public string GetResourceDirPath(int resourceId)
        {
            string path = UserDataDirPath + resourceId + "/";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public string GetThumbnailFilePath(int resourceId )
        {
            return GetResourceDirPath(resourceId) + "thumbnail.png";
        }
    }
}