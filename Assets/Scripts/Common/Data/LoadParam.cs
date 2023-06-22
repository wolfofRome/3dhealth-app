using System;

namespace Assets.Scripts.Common.Data
{
    [Serializable]
    public struct LoadParam
    {
        private int _resourceId;
        public int ResourceId {
            get {
                return _resourceId;
            }
            set {
                _resourceId = value;
            }
        }

        private long _createTime;
        public long CreateTime {
            get {
                return _createTime;
            }
            set {
                _createTime = value;
            }
        }

        private long _modifiedTime;
        public long ModifiedTime {
            get {
                return _modifiedTime;
            }
            set {
                _modifiedTime = value;
            }
        }

        private string _path;
        public string Path {
            get {
                return _path;
            }
            set {
                _path = value;
            }
        }

        private object _optionalParam;
        public object OptionalParam {
            get {
                return _optionalParam;
            }
            set {
                _optionalParam = value;
            }
        }

        public LoadParam(int resourceId, long createTime, long modifiedTime, string path)
        {
            _resourceId = resourceId;
            _createTime = createTime;
            _modifiedTime = modifiedTime;
            _path = path;
            _optionalParam = null;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is LoadParam))
            {
                return false;
            }
            
            var param = (LoadParam)obj;
            return (this.ResourceId == param.ResourceId && this.ModifiedTime == param.ModifiedTime);
        }

        public override int GetHashCode()
        {
            return ResourceId ^ ModifiedTime.GetHashCode();
        }
    }
}