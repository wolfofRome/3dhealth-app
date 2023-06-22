using SQLite4Unity3d;
using UnityEngine;
using System;
using System.IO;
#if !UNITY_EDITOR
using System.Collections;
#endif
using System.Collections.Generic;
using Assets.Scripts.Database.DataRow;
using Assets.Scripts.Common;
using Database.DataRow.BodyComposition;
using UnityEngine.Networking;
using Version = System.Version;

namespace Assets.Scripts.Database
{
    public class DatabaseHelper : IDatabase
    {
        private SQLiteConnection _connection;
        private Version _version;
        public Version DatabaseVersion {
            get {
                return _version;
            }
        }

        public DatabaseHelper(string databaseName, Version existingVersion)
        {
#if !UNITY_WEBGL
            var orgDBPath = Path.Combine(Application.streamingAssetsPath, databaseName);
            var loadDbVersionPath = Path.Combine(Application.streamingAssetsPath, Path.GetFileNameWithoutExtension(orgDBPath) + "_ver.txt");

            // バージョン情報を読込
            if (loadDbVersionPath.Contains("://"))
            {
                UnityWebRequest www = UnityWebRequest.Get(loadDbVersionPath);
                www.SendWebRequest();
                while (!www.isDone) { }
                _version = new Version(www.downloadHandler.text);
            }
            else
            {
                _version = new Version(File.ReadAllText(loadDbVersionPath));
            }

            DebugUtil.Log(databaseName + " version current: " + _version.ToString() + "  existing: " + existingVersion.ToString());

           
#if UNITY_EDITOR
            var dbPath = orgDBPath;
#else
            var filepath = Path.Combine(Application.persistentDataPath, databaseName);

            // バージョン情報が異なる場合は既存のキャッシュDBを新しいDBで上書き
            if (!_version.Equals(existingVersion) || !File.Exists(filepath)) {
                if (orgDBPath.Contains("://"))
                {
                    // this is the path to your StreamingAssets in android
                    var loader = UnityWebRequest.Get(orgDBPath);
                    loader.SendWebRequest();
                    while (!loader.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
                                                // then save to Application.persistentDataPath
                    File.WriteAllBytes(filepath, loader.downloadHandler.data);
                }
                else
                {
                    // then save to Application.persistentDataPath
                    File.Copy(orgDBPath, filepath, true);
                }
                DebugUtil.Log("Database written");
            }
            var dbPath = filepath;
#endif
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            DebugUtil.Log("Final PATH: " + dbPath);
#endif
        }

        public bool isConnected()
        {
            return _connection == null ? false : true;
        }

        public User GetUser(int id)
        {
            return (from t in _connection.Table<User>()
                    where t.Id == id
                    select t).FirstOrDefault();
        }

        public IEnumerable<BodyComposition> GetBodyComposition(int id)
        {
            return (from t in _connection.Table<BodyComposition>()
                    where t.UserId == id
                    select t);
        }

        public BodyComposition GetBodyComposition(int id, int resourceId)
        {
            return (from t in _connection.Table<BodyComposition>()
                    where t.UserId == id && t.ResourceId == resourceId
                    select t).FirstOrDefault();
        }

        public IEnumerable<Measurement> GetMeasurement(int id)
        {
            return (from t in _connection.Table<Measurement>()
                    where t.UserId == id
                    select t);
        }

        public Measurement GetMeasurement(int id, int resourceId)
        {
            return (from t in _connection.Table<Measurement>()
                    where t.UserId == id && t.ResourceId == resourceId
                    select t).FirstOrDefault();
        }

        public ScanData GetScanData(int id, int resourceId, FileType type)
        {
            return (from t in _connection.Table<ScanData>()
                    where t.UserId == id && t.ResourceId == resourceId && t.Type == type
                    select t).FirstOrDefault();
        }

        public IEnumerable<T> SelectAll<T>() where T : new()
        {
            return _connection.Table<T>();
        }


        public int Insert<T>(T item) where T : BaseDataRow
        {
            return _connection.Insert(item);
        }
        
        public int InsertAll<T>(List<T> item) where T : BaseDataRow
        {
            return _connection.InsertAll(item);
        }

        public int InsertOrReplace<T>(T item) where T : BaseDataRow
        {
            return _connection.InsertOrReplace(item);
        }

        public void Clear()
        {
            _connection.DeleteAll<BodyComposition>();
            _connection.DeleteAll<Measurement>();
            _connection.DeleteAll<ScanData>();
            _connection.DeleteAll<User>();
        }

        public void Clear<T>() where T : BaseDataRow
        {
            _connection.DeleteAll<T>();
        }
    }
}