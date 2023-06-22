using System;
using System.Collections.Generic;
using Assets.Scripts.Database.DataRow;
using Database.DataRow.BodyComposition;

namespace Assets.Scripts.Database
{
    public interface IDatabase
    {
        Version DatabaseVersion { get; }

        void Clear<T>() where T : BaseDataRow;
        void Clear();
        User GetUser(int id);

        IEnumerable<BodyComposition> GetBodyComposition(int id);
        BodyComposition GetBodyComposition(int id, int resourceId);

        IEnumerable<Measurement> GetMeasurement(int id);
        Measurement GetMeasurement(int id, int resourceId);

        ScanData GetScanData(int id, int resourceId, FileType type);

        IEnumerable<T> SelectAll<T>() where T : new();
        int Insert<T>(T item) where T : BaseDataRow;
        int InsertAll<T>(List<T> item) where T : BaseDataRow;
        int InsertOrReplace<T>(T item) where T : BaseDataRow;
    }
}