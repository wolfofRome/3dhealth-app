using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Common
{
    public abstract class BaseLoader : MonoBehaviour
    {
        private string _logMsgs = "";

        protected IEnumerator DownloadAsString(string url, System.Action<string> result)
        {
            AddToLog("Downloading " + url);

            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.error != null)
            {
                AddToLog(www.error);
            }
            else {
                AddToLog("Downloaded " + www.downloadHandler.data.Length + " bytes");
            }
            result(www.downloadHandler.text);
        }

        protected IEnumerator DownloadAsBinary(string url, System.Action<byte[]> result)
        {
            AddToLog("Downloading " + url);

            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.error != null)
            {
                AddToLog(www.error);
            }
            else {
                AddToLog("Downloaded " + www.downloadHandler.data.Length + " bytes");
            }
            result(www.downloadHandler.data);
        }

        /* ------------------------------- Logging functions  ---------------------------------- */
        protected void AddToLog(string msg)
        {
            DebugUtil.Log(msg + "\n" + DateTime.Now.ToString("yyy/MM/dd hh:mm:ss.fff"));

            // for some silly reason the Editor will generate errors if the string is too long
            int lenNeeded = msg.Length + 1;
            if (_logMsgs.Length + lenNeeded > 4096) _logMsgs = _logMsgs.Substring(0, 4096 - lenNeeded);

            _logMsgs = _logMsgs + "\n" + msg;
        }

        private string TruncateStringForEditor(string str)
        {
            // for some silly reason the Editor will generate errors if the string is too long
            if (str.Length > 4096) str = str.Substring(0, 4000) + "\n .... display truncated ....\n";
            return str;
        }
    }
}

