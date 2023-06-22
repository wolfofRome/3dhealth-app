using Assets.Scripts.Common.Animators;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ThreeDView.Posture {
    public class AdviceController : MonoBehaviour {
        [SerializeField]
        private RectTransform _contentArea = default;

        [SerializeField]
        private Text _postureConditionTitle = default;

        [SerializeField]
        private RectTransform _recommendExerciseParent = default;

        [SerializeField]
        private Text _adviceMessage = default;

        [SerializeField]
        private RecommendExerciseItem _prefabRecommendExerciseItem = default;

        [SerializeField]
        private Text _prefabRecommendExerciseTitle = default;

        [SerializeField]
        private PostureAdviceAssetConfig _postureAdviceAssetConfig = default;

        private List<GameObject> _recommendItemList = new List<GameObject>();

        private PostureCondition _postureCondition = PostureCondition.Normal;
        public PostureCondition postureCondition {
            get {
                return _postureCondition;
            }
            set {
                if (_postureCondition != value) {
                    _postureConditionTitle.text = value.ToTitle();

                    if (_recommendItemList.Count > 0) {
                        _recommendItemList.ForEach(item => Destroy(item));
                        _recommendItemList.Clear();
                    }

                    var trainingAsset = _postureAdviceAssetConfig.GetTrainingAsset(value);
                    var stretchAsset = _postureAdviceAssetConfig.GetStretchAsset(value);

                    _recommendExerciseParent.gameObject.SetActive(trainingAsset != null || stretchAsset != null);

                    // トレーニングのアドバイス画像追加.
                    if (trainingAsset != null) {
                        // トレーニングのタイトルを追加.
                        _recommendItemList.Add(CreateRecommendExerciseTitle(trainingAsset.id.ToName()).gameObject);

                        // トレーニングの画像を追加.
                        foreach (var image in trainingAsset.images) {
                            // TODO: trainingAsset.id.ToDescription()についてはトレーニングの説明文が未決定次第のため要調整.
                            _recommendItemList.Add(CreateRecommendExerciseItem(image, trainingAsset.id.ToDescription()).gameObject);
                        }
                    }

                    // ストレッチのアドバイス画像追加.
                    if (stretchAsset != null) {
                        // ストレッチのタイトルを追加.
                        _recommendItemList.Add(CreateRecommendExerciseTitle(stretchAsset.id.ToName()).gameObject);

                        // ストレッチの画像を追加.
                        foreach (var image in stretchAsset.images) {
                            // TODO: stretchAsset.id.ToDescription()についてはストレッチの説明文が未決定次第のため要調整.
                            _recommendItemList.Add(CreateRecommendExerciseItem(image, stretchAsset.id.ToDescription()).gameObject);
                        }
                    }
                    _adviceMessage.text = value.ToAdvice();
                }
                _postureCondition = value;
            }
        }

        /// <summary>
        /// オススメエクササイズのタイトル生成.
        /// </summary>
        /// <param name="titleText"></param>
        /// <returns></returns>
        private Text CreateRecommendExerciseTitle(string titleText) {
            var title = Instantiate(_prefabRecommendExerciseTitle);
            title.text = "●" + titleText;
            title.transform.SetParent(_recommendExerciseParent, false);
            return title;
        }

        /// <summary>
        /// オススメエクササイズの画像生成.
        /// </summary>
        /// <param name="titleText"></param>
        /// <returns></returns>
        private RecommendExerciseItem CreateRecommendExerciseItem(Sprite sprite, string description) {
            var item = Instantiate(_prefabRecommendExerciseItem);
            item.image.sprite = sprite;
            item.description.text = description;
            item.transform.SetParent(_recommendExerciseParent, false);
            return item;
        }

        /// <summary>
        /// Collapseアニメーションの完了イベント.
        /// </summary>
        /// <param name="animator"></param>
        public void OnFinishCollapseAnimation(LayoutElementAnimator animator) {
            var position = _contentArea.anchoredPosition;
            position.y = 0;
            _contentArea.anchoredPosition = position;
        }

    }
}
