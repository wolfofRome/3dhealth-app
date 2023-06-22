using Assets.Scripts.Calendar;
using Assets.Scripts.Common;
using Assets.Scripts.Common.Config;
using Assets.Scripts.Common.Graphics.UI.ListView;
using Assets.Scripts.Compare;
using Assets.Scripts.Network.Response;
using Assets.Scripts.TalentList;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Archive
{
    [RequireComponent(typeof(CompareLoadParam))]
    public class ArchiveController : MonoBehaviour
    {
        [SerializeField]
        private SlideMenuController _menuController = default;
        private SlideMenuController menuController
        {
            get
            {
                return _menuController;
            }
        }

        [SerializeField]
        private GridViewController _userDataGridView = default;
        private GridViewController userDataGridView
        {
            get
            {
                return _userDataGridView;
            }
        }

        [SerializeField]
        private ScanDataListAdapter _userDataListAdapter = default;
        private ScanDataListAdapter userDataListAdapter
        {
            get
            {
                return _userDataListAdapter;
            }
        }
        [SerializeField]
        private GridViewController _purchasedDataGridView = default;
        private GridViewController purchasedDataGridView
        {
            get
            {
                return _purchasedDataGridView;
            }
        }

        [SerializeField]
        private ScanDataListAdapter _purchasedDataListAdapter = default;
        private ScanDataListAdapter purchasedDataListAdapter
        {
            get
            {
                return _purchasedDataListAdapter;
            }
        }


        private CompareLoadParam _compareLoadParam;
        private CompareLoadParam compareLoadParam
        {
            get
            {
                return _compareLoadParam = _compareLoadParam ?? GetComponent<CompareLoadParam>();
            }
        }

        private ArchiveLoadParam.Argment _loadParam;
        private ArchiveLoadParam.Argment loadParam
        {
            get
            {
                return _loadParam = _loadParam ?? SceneLoader.Instance.GetArgment<ArchiveLoadParam.Argment>(gameObject.scene.name);
            }
        }
        
        private const int SelectNum = 2;
        private List<BaseScanDataListItem> _selectedItems = new List<BaseScanDataListItem>();
        private List<BaseScanDataListItem> selectedItems
        {
            get
            {
                return _selectedItems;
            }
        }

        [SerializeField]
        private TalentDataSource _talentDataSource = default;
        private TalentDataSource talentDataSource
        {
            get
            {
                return _talentDataSource;
            }
        }

        [SerializeField]
        private CalendarDataSource _calendarDataSource = default;
        private CalendarDataSource calendarDataSource
        {
            get
            {
                return _calendarDataSource;
            }
        }

        [SerializeField]
        private RectTransform _buttonGroup = default;
        private RectTransform buttonGroup
        {
            get
            {
                return _buttonGroup;
            }
        }

        private bool _isMultiSelectable;
        public bool isMultiSelectable {
            get {
                return _isMultiSelectable;
            }
            set {
                _isMultiSelectable = value;

                // 購入済データの表示更新.
                foreach (var item in purchasedDataListAdapter.list)
                {
                    item.isMultiSelectable = value;
                }
                purchasedDataGridView.UpdateListItem();


                // ユーザーデータの表示更新.
                foreach (var item in userDataListAdapter.list)
                {
                    item.isMultiSelectable = value;
                }
                userDataGridView.UpdateListItem();
            }
        }

        void Start()
        {
            // 購入済スキャンデータ一覧の初期化.
            var purchasedList = talentDataSource.talentoInfoList.Where(t => t.purchaseStatus == PurchaseStatus.Purchased);
            foreach (var item in purchasedList)
            {
                purchasedDataListAdapter.list.Add(CreateBindParam(item, item.CommonName));
            }
            purchasedDataListAdapter.NotifyDataSetChanged();


            // ユーザーのスキャンデータ一覧の初期化.
            var contentsList = DataManager.Instance.ContentsList.OrderByDescending(v => v.CreateTimeAsDateTime).ToList();
            foreach (var c in contentsList)
            {
                CalendarInfo info;
                calendarDataSource.infoMap.TryGetValue(c.CreateTimeAsDateTime.Date.ToString(CalendarDataSource.KeyFormat), out info);

                userDataListAdapter.list.Add(CreateBindParam(c, (info == null ? string.Empty : info.Memo)));
            }
            userDataListAdapter.NotifyDataSetChanged();

            // スキャン画像が2枚以上存在する場合のみ比較画面への遷移ボタンを表示.
            buttonGroup.gameObject.SetActive(contentsList.Count > 1);

            // 1フレーム後にアニメーションを開始.
            StartCoroutine(this.DelayFrame(1, () =>
            {
                menuController.Show();
            }));
        }

        public void CloseArchive()
        {
            menuController.slideAnimator.onFinishAnimation.RemoveAllListeners();
            menuController.slideAnimator.onFinishAnimation.AddListener(go =>
            {
                SceneLoader.Instance.UnloadScene(go.scene.name);
            });
            menuController.Hide();
        }

        private BindParam CreateBindParam(Contents contents, string description)
        {
            return new BindParam()
            {
                contents = contents,
                description = description,
                isMultiSelectable = loadParam.isMultiSelectable,
                onClick = (item) =>
                {
                    if (isMultiSelectable)
                    {
                        item.checkMark.isOn = !item.checkMark.isOn;

                        if (item.checkMark.isOn && !selectedItems.Contains(item))
                        {
                            selectedItems.Add(item);
                            if (selectedItems.Count > SelectNum)
                            {
                                selectedItems[0].checkMark.isOn = false;
                                selectedItems.RemoveAt(0);
                            }
                        }
                        else
                        {
                            selectedItems.Remove(item);
                        }
                        if (selectedItems.Count >= SelectNum)
                        {
                            // 指定枚数以上選択したら比較画面へ遷移.
                            compareLoadParam.argment.leftContents = selectedItems[0].contents;
                            compareLoadParam.argment.rightContents = selectedItems[1].contents;
                            SceneLoader.Instance.LoadSceneWithParams(compareLoadParam);
                        }
                    }
                    else
                    {
                        SceneLoader.Instance.LoadSceneWithParams(item.sceneLoadParam);
                    }
                }
            };
        }
    }
}