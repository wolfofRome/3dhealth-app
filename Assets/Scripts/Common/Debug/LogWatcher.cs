using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common.Debug
{
    [RequireComponent(typeof(Text))]
    public class LogWatcher : MonoBehaviour
    {
#if DEBUG
        private const int LogMaxLength = 14000;

        private Text _logText;
        private Text logText {
            get {
                return _logText = _logText ?? GetComponent<Text>();
            }
        }

        private StringBuilder _sb = new StringBuilder();

        void Awake()
        {
            Application.logMessageReceived += LogCallBackHandler;
        }
        
        void LogCallBackHandler(string condition, string stackTrace, LogType type)
        {
            System.Diagnostics.StackTrace systemStackTrace = new System.Diagnostics.StackTrace(true);
            string systemStackTraceStr = systemStackTrace.ToString();

            SetLogData(condition, stackTrace, systemStackTraceStr, type);
        }

        void SetLogData(string condition, string stackTrace, string systemStackTrace, LogType type)
        {
            _sb.Append(type.ToString()).Append(":").Append(condition).Append("\n");

            var start = _sb.Length > LogMaxLength ? (_sb.Length - LogMaxLength) : 0;
            logText.text = _sb.ToString().Substring(start);
        }

        void OnDestroy()
        {
            Application.logMessageReceived -= LogCallBackHandler;
        }
#endif
    }
}
