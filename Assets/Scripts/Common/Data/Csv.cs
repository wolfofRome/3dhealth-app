using System.IO;
using System.Collections.Generic;

namespace Assets.Scripts.Common.Data
{
    public class Csv
    {
        private List<Dictionary<string, string>> _csvData;

        public int Length {
            get {
                return _csvData.Count;
            }
        }

        public Csv()
        {
            _csvData = new List<Dictionary<string, string>>();
        }

        public void SetSourceString(string srcString, bool hasHeader)
        {
            StringReader reader = new StringReader(srcString);

            _csvData.Clear();

            string[] header = null;
            while (reader.Peek() > -1)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(',');
                if (hasHeader && header == null)
                {
                    header = values;
                }
                else {
                    Dictionary<string, string> csvElement = new Dictionary<string, string>();
                    foreach (string value in values)
                    {
                        string key = (header == null ? csvElement.Count.ToString() : header[csvElement.Count]);
                        //DebugUtil.Log("key : " + key + "value : " + value);
                        csvElement.Add(key, value);
                    }
                    _csvData.Add(csvElement);
                }
            }
        }

        public void Clear()
        {
            _csvData.Clear();
        }

        public Dictionary<string, string>.KeyCollection GetKeys()
        {
            return _csvData[0].Keys;
        }

        public Dictionary<string, string> GetRowValues(int rowIdx)
        {
            return _csvData[rowIdx];
        }

        public string GetValue(int rowIdx, string key)
        {
            return _csvData[rowIdx][key];
        }
    }
}