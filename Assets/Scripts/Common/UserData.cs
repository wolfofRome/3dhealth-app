using System;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public static class UserData
    {
        private const string keyCacheDataBaseVersionMajor = "CacheDataBaseVersionMajor";
        private const string keyCacheDataBaseVersionMinor = "CacheDataBaseVersionMinor";
        private const string keyCacheDataBaseVersionBuild = "CacheDataBaseVersionBuild";
        private const string keyHasViewSetting = "HasViewSetting";
        private const string keyPhysiqueAlpha = "PhysiqueAlphaState";
        private const string keySkeletonAlpha = "SkeletonAlphaState";
        private const string keyMuscleAlpha = "MuscleAlphaState";
        private const string keyFatAlpha = "FatAlphaState";
        private const string keyFatWireMode = "FatWireMode";
        private const string keyFlatMode = "FlatMode";
        private const string keyGridMode = "GridMode";
		private const string keyAutoLoginUserId = "AutoLoginUserId";
		private const string keyHistoryGraphTerm = "HistoryGraphTerm";
        private const string keyRegistrationTokenId = "RegistrationTokenId";
        private const string keyShouldShowPosturePurchaseConfirm = "ShouldShowPosturePurchaseConfirm";

        public static void Save()
        {
            PlayerPrefs.Save();
        }
        
        public static Version CacheDatabaseVersion {
            get {
                var major = PlayerPrefs.GetInt(keyCacheDataBaseVersionMajor, 0);
                var minor = PlayerPrefs.GetInt(keyCacheDataBaseVersionMinor, 0);
                var build = PlayerPrefs.GetInt(keyCacheDataBaseVersionBuild, 0);
                return new Version(major, minor, build);
            }
            set {
                PlayerPrefs.SetInt(keyCacheDataBaseVersionMajor, value.Major);
                PlayerPrefs.SetInt(keyCacheDataBaseVersionMinor, value.Minor);
                PlayerPrefs.SetInt(keyCacheDataBaseVersionBuild, value.Build);
            }
        }
        
        public static bool HasViewSetting {
            get {
                return bool.Parse(PlayerPrefs.GetString(keyHasViewSetting, "false"));
            }
            set {
                PlayerPrefs.SetString(keyHasViewSetting, value.ToString());
            }
        }

        public static int PhysiqueAlpha {
            get {
                return PlayerPrefs.GetInt(keyPhysiqueAlpha, 0);
            }
            set {
                PlayerPrefs.SetInt(keyPhysiqueAlpha, value);
            }
        }
        
        public static int SkeletonAlpha {
            get {
                return PlayerPrefs.GetInt(keySkeletonAlpha, 0);
            }
            set {
                PlayerPrefs.SetInt(keySkeletonAlpha, value);
            }
        }

        public static int MuscleAlpha {
            get {
                return PlayerPrefs.GetInt(keyMuscleAlpha, 0);
            }
            set {
                PlayerPrefs.SetInt(keyMuscleAlpha, value);
            }
        }

        public static int FatAlpha {
            get {
                return PlayerPrefs.GetInt(keyFatAlpha, 0);
            }
            set {
                PlayerPrefs.SetInt(keyFatAlpha, value);
            }
        }

        public static bool FatWireMode {
            get {
                return bool.Parse(PlayerPrefs.GetString(keyFatWireMode, "false"));
            }
            set {
                PlayerPrefs.SetString(keyFatWireMode, value.ToString());
            }
        }
        
        public static bool FlatMode {
            get {
                return bool.Parse(PlayerPrefs.GetString(keyFlatMode, "false"));
            }
            set {
                PlayerPrefs.SetString(keyFlatMode, value.ToString());
            }
        }

        public static bool GridMode {
            get {
                return bool.Parse(PlayerPrefs.GetString(keyGridMode, "false"));
            }
            set {
                PlayerPrefs.SetString(keyGridMode, value.ToString());
            }
        }

        public static int AutoLoginUserId {
            get {
                return PlayerPrefs.GetInt(keyAutoLoginUserId, -1);
            }
            set {
                PlayerPrefs.SetInt(keyAutoLoginUserId, value);
            }
		}

		public static int HistoryGraphTerm {
			get {
				return PlayerPrefs.GetInt(keyHistoryGraphTerm, 0);
			}
			set {
				PlayerPrefs.SetInt(keyHistoryGraphTerm, value);
			}
		}

        public static int RegistrationTokenId {
            get {
                return PlayerPrefs.GetInt(keyRegistrationTokenId, -1);
            }
            set {
                PlayerPrefs.SetInt(keyRegistrationTokenId, value);
            }
        }

        public static bool ShouldShowPosturePurchaseConfirm
        {
            get
            {
                return bool.Parse(PlayerPrefs.GetString(keyShouldShowPosturePurchaseConfirm, "true"));
            }
            set
            {
                PlayerPrefs.SetString(keyShouldShowPosturePurchaseConfirm, value.ToString());
            }
        }
    }
}
