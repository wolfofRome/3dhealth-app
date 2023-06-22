using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Common
{
    public abstract class BaseSingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance {
            get {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));
                    Assert.IsNotNull(instance, typeof(T) + "is nothing");
                }
                return instance;
            }
        }
    }
}
