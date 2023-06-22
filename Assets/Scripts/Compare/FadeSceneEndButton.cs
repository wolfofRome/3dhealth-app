using UnityEngine;
using Assets.Scripts.Common;
using Assets.Scripts.Common.Animators;

namespace Assets.Scripts.Compare
{
    public class FadeSceneEndButton : SceneEndButton
    {
        public override void OnClick()
        {
            var async = BackToPreviousSceneAsync();
            ScreenFadeAnimator.Instance.FadeOut(() =>
            {
                Screen.orientation = ScreenOrientation.Portrait;
                async.allowSceneActivation = true;
                ScreenFadeAnimator.Instance.FadeIn(() => {});
            });
        }
    }
}