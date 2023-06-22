using System;
using System.Reflection;

namespace Assets.Scripts.Database.DataRow
{
    public abstract class BaseDataRow
    {
        public T GetValue<T>(string property_name)
        {
            T value;
            PropertyInfo pi = GetType().GetProperty(property_name);
            if (pi != null)
            {
                value = (T)pi.GetValue(this, null);
            }
            else {
                throw new Exception("Not found property : " + property_name);
            }
            return value;
        }

        public object SetValues<T>(T obj)
        {
            PropertyInfo[] pi_array = GetType().GetProperties();
            if (pi_array != null)
            {
                foreach (var pi in pi_array)
                {
                    var value = pi.GetValue(obj, null);
                    if (pi.GetSetMethod() != null )
                    {
                        pi.SetValue(this, value, null);
                    }
                }
            }
            else
            {
                throw new Exception("Not found property : ");
            }
            return this;
        }

        public abstract bool PKEquals(object obj);
    }
}