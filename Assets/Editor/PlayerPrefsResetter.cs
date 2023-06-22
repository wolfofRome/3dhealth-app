using UnityEngine;
using UnityEditor;

namespace Assets.Editor
{
    public static class PlayerPrefsResetter
    {
        [MenuItem("Tools/Reset PlayerPrefs")]
        public static void ResetPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            Debug.Log("Reset PlayerPrefs");
        }
    }
}
