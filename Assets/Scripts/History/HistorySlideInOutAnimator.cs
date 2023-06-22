using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Common;

namespace Assets.Scripts.History
{
    public class HistorySlideInOutAnimator : MonoBehaviour
    {
        [SerializeField]
        private GameObject dataLoader = default;
        [SerializeField]
        private SceneEndButton sceneEndButton = default;

        public void OnFinishSlideInAnimation()
        {
            dataLoader.SetActive(true);
        }

        public void StartSlideOutAnimation()
        {
            GetComponent<Animator>().Play("Hidden");
        }

        public void OnFinishSlideOutAnimation()
        {
            sceneEndButton.UnloadSceneAsync();
        }
    }
}
