using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public static class MonoBehaviourExtension
    {
        public static IEnumerator DelayFrame(this MonoBehaviour self, int delayFrameCount, Action action)
        {
            for (var i = 0; i < delayFrameCount; i++)
            {
                yield return null;
            }
            action();
        }

        public static IEnumerator DelayTimeSeconds(this MonoBehaviour self, float delayTimeSeconds, Action action)
        {
            yield return new WaitForSeconds(delayTimeSeconds);
            action();
        }
    }
}
