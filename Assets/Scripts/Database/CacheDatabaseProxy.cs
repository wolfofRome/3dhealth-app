using Assets.Scripts.Database.DataRow;
using System;
using System.Collections.Generic;
using System.Linq;
using Database.DataRow.BodyComposition;

namespace Assets.Scripts.Database
{
    class CacheDatabaseProxy : IDatabase
    {
        private DatabaseHelper _dbHelper;
        private TableMap _tableMap;
        private Version _version = new Version("1.0.0");
        public Version DatabaseVersion {
            get {
                return _version;
            }
        }

        private class TableMap
        {
            private readonly Dictionary<Type, object> _map = new Dictionary<Type, object>();

            public void NewEntry<T>()
            {
                _map.Add(typeof(T), new List<T>());
            }

            public void AddEntry<T>(List<T> value)
            {
                _map[typeof(T)] = value;
            }

            public void ClearEntry<T>()
            {
                _map.Remove(typeof(T));
                _map.Add(typeof(T), new List<T>());
            }

            public List<T> GetEntry<T>()
            {
                return (List<T>)_map[(typeof(T))];
            }

            public void Clear()
            {
                _map.Clear();
            }
        }

        public CacheDatabaseProxy(string databaseName, Version existingVersion)
        {
            _tableMap = new TableMap();
            _tableMap.NewEntry<User>();
            _tableMap.NewEntry<BodyComposition>();
            _tableMap.NewEntry<Measurement>();
            _tableMap.NewEntry<ScanData>();

            _dbHelper = new DatabaseHelper(databaseName, existingVersion);
            if (_dbHelper.isConnected())
            {
                // WebGLとシーケンスを統一するため、初めにメモリに展開しておく
                _version = _dbHelper.DatabaseVersion;
                _tableMap.AddEntry(_dbHelper.SelectAll<User>().ToList());
                _tableMap.AddEntry(_dbHelper.SelectAll<BodyComposition>().ToList());
                _tableMap.AddEntry(_dbHelper.SelectAll<Measurement>().ToList());
                _tableMap.AddEntry(_dbHelper.SelectAll<ScanData>().ToList());
            }
        }

        public User GetUser(int id)
        {
            return (from t in _tableMap.GetEntry<User>()
                    where t.Id == id
                    select t).FirstOrDefault();
        }

        public IEnumerable<BodyComposition> GetBodyComposition(int id)
        {
            return (from t in _tableMap.GetEntry<BodyComposition>()
                    where t.UserId == id
                    select t);
        }

        public BodyComposition GetBodyComposition(int id, int resourceId)
        {
            return (from t in _tableMap.GetEntry<BodyComposition>()
                    where t.UserId == id && t.ResourceId == resourceId
                    select t).FirstOrDefault();
        }

        public IEnumerable<Measurement> GetMeasurement(int id)
        {
            return (from t in _tableMap.GetEntry<Measurement>()
                    where t.UserId == id
                    select t);
        }

        public Measurement GetMeasurement(int id, int resourceId)
        {
            return (from t in _tableMap.GetEntry<Measurement>()
                    where t.UserId == id && t.ResourceId == resourceId
                    select t).FirstOrDefault();
        }

        public ScanData GetScanData(int id, int resourceId, FileType type)
        {
            return (from t in _tableMap.GetEntry<ScanData>()
                    where t.UserId == id && t.ResourceId == resourceId && t.Type == type
                    select t).FirstOrDefault();
        }

        public IEnumerable<BaseDataRow> SelectAll<BaseDataRow>() where BaseDataRow : new()
        {
            return _tableMap.GetEntry<BaseDataRow>();
        }

        public int Insert<T>(T item) where T : BaseDataRow
        {
            if (_dbHelper.isConnected())
            {
                _dbHelper.Insert(item);
            }

            var row = (from t in _tableMap.GetEntry<T>()
                       where t.PKEquals(item)
                       select t).FirstOrDefault();

            if (row == null)
            {
                _tableMap.GetEntry<T>().Add(item);
            }
            return _tableMap.GetEntry<T>().Count;
        }

        public int InsertAll<T>(List<T> item) where T : BaseDataRow
        {
            if (_dbHelper.isConnected())
            {
                _dbHelper.InsertAll(item);
            }

            _tableMap.GetEntry<T>().AddRange(item);

            return _tableMap.GetEntry<T>().Count;
        }

        public int InsertOrReplace<T>(T item) where T : BaseDataRow
        {
            if (_dbHelper.isConnected())
            {
                _dbHelper.InsertOrReplace(item);
            }

            var row = (from t in _tableMap.GetEntry<T>()
                       where t.PKEquals(item)
                       select t).FirstOrDefault();

            // リストに存在する場合は更新、なければ追加
            if (row == null)
            {
                _tableMap.GetEntry<T>().Add(item);
            }
            else
            {
                row.SetValues(item);
            }

            return _tableMap.GetEntry<T>().Count;
        }

        public void Clear()
        {
            if (_dbHelper.isConnected())
            {
                _dbHelper.Clear();
            }
            _tableMap.Clear();
        }

        public void Clear<T>() where T : BaseDataRow
        {
            if (_dbHelper.isConnected())
            {
                _dbHelper.Clear<T>();
            }
            _tableMap.ClearEntry<T>();
        }
    }
}