using Assets.Scripts.Common.Data;
using Assets.Scripts.Network.Response;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Scripts.Database;
using Assets.Scripts.Common.Util;

namespace Assets.Scripts.Common
{
    public class DataManager
    {
        private static DataManager _instance;
        
        private DataManager()
        {
            Cache = new CacheDatabaseProxy("Cache.db", UserData.CacheDatabaseVersion);
            if (Cache != null)
            {
                UserData.CacheDatabaseVersion = Cache.DatabaseVersion;
            }
        }

        public static DataManager Instance
        {
            get
            {
                return _instance = _instance ?? new DataManager();
            }
        }

        /// <summary>
        /// キャッシュのクリア.
        /// </summary>
        public void Clear()
        {
            Cache.Clear();
            if (CachePath != null)
            {
                // キャッシュを削除.
                FileIO.DeleteDirectory(CachePath.UserDataDirPath);
            }
            _instance = null;
        }
        
        public IDatabase Cache { get; set; }

        /// <summary>
        /// トライアルアカウントの設定取得.
        /// </summary>
        private TrialAccountConfig _trialConfig;
        public TrialAccountConfig TrialConfig
        {
            get
            {
                return _trialConfig = _trialConfig ?? Resources.Load<TrialAccountConfig>("Configs/TrialAccountConfig");
            }
            set
            {
                _trialConfig = value;
            }
        }

        /// <summary>
        /// トライアルアカウント判定.
        /// </summary>
        public bool isTrialAccount
        {
            get
            {
                if (Profile != null && Profile.Email == TrialConfig.LoginId)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// プロフィール.
        /// </summary>
        private Profile _profile;
        public Profile Profile
        {
            get
            {
                return _profile;
            }
            set
            {
                _profile = value;
            }
        }

        /// <summary>
        /// デバイス一覧.
        /// </summary>
        private List<Device> _deviceList;
        public List<Device> DeviceList
        {
            get
            {
                if (_deviceList == null)
                {
                    _deviceList = new List<Device>();
                }
                return _deviceList;
            }
            set
            {
                _deviceList = value;
            }
        }

        /// <summary>
        /// ログイン中のユーザーのスキャンデータ一覧.
        /// </summary>
        private List<Contents> _contentsList;
        public List<Contents> ContentsList
        {
            get
            {
                if (_contentsList == null)
                {
                    _contentsList = new List<Contents>();
                }
                return _contentsList;
            }
            set
            {
                _contentsList = value;
            }
        }

        /// <summary>
        /// 体組成データのCSVのパス.
        /// </summary>
        private string _userBodyCompositionPath;
        public string UserBodyCompositionPath
        {
            get
            {
                return _userBodyCompositionPath;
            }
            set
            {
                _userBodyCompositionPath = value;
            }
        }

        /// <summary>
        /// 採寸データのCSVのパス.
        /// </summary>
        private string _userMeasurementPath;
        public string UserMeasurementPath
        {
            get
            {
                return _userMeasurementPath;
            }
            set
            {
                _userMeasurementPath = value;
            }
        }

        /// <summary>
        /// キャッシュ保存ディレクトリのパス.
        /// </summary>
        private CachePath _cachePath;
        public CachePath CachePath
        {
            get
            {
                if (_cachePath == null)
                {
                    _cachePath = new CachePath(Profile.UserId);
                }
                return _cachePath;
            }
        }

        /// <summary>
        /// 3Dモード表示用のスキャンデータ取得.
        /// </summary>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public Contents GetThreeDViewContents(int resourceId)
        {
            Contents contents = null;

            // 指定されていない場合は常に最新を表示.
            if (resourceId <= 0)
            {
                contents = GetLatestContents();
            }
            else
            {
                contents = GetContents(resourceId);
            }
            return contents;
        }

        /// <summary>
        /// 最新のスキャンデータ取得.
        /// </summary>
        /// <returns></returns>
        public Contents GetLatestContents()
        {
            return ContentsList.OrderByDescending(content => content.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 指定されたリソースIDのスキャンデータ取得.
        /// </summary>
        /// <param name="id">リソースID</param>
        /// <returns></returns>
        public Contents GetContents(int id)
        {
            return ContentsList.Where(content => content.ResourceId == id).FirstOrDefault();
        }
        
        /// <summary>
        /// デバイス情報の取得.
        /// </summary>
        /// <param name="id">デバイスID</param>
        /// <returns></returns>
        public Device GetDevice(int id)
        {
            return DeviceList.Where(device => device.DeviceId == id).FirstOrDefault();
        }
    }
}