using Assets.Scripts.Common.Util;
using SQLite4Unity3d;

namespace Assets.Scripts.Database.DataRow
{
    public class User : BaseDataRow
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string LoginId { get; set; }
        public string EncryptPassword { get; set; }
        public int? Height { get; set; }

        [IgnoreAttribute]
        public string Password {
            get {
                string pass = null;
                if (!string.IsNullOrEmpty(EncryptPassword))
                {
                    pass = Security.DecryptString(EncryptPassword, encryptKey, encryptIV);
                }
                return pass;
            }
            set {
                EncryptPassword = Security.EncryptString(value, encryptKey, encryptIV);
            }
        }
        private const string encryptKey = "3DXk7cE9noazQ1o2eV0NMGcEkyuUYpbw";
        private const string encryptIV = "32860880551589123955175234574001";

        public override string ToString()
        {
            return string.Format("[User: Id={0}, Password={1}, LoginId={2}, Height={3}", Id, Password, LoginId, Height);
        }

        public override bool PKEquals(object obj)
        {
            if (obj == null || !(obj is User))
            {
                return false;
            }
            var u = (User)obj;
            return Id == u.Id;
        }
    }
}