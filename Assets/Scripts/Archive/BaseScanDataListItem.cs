using Assets.Scripts.Common.Graphics.UI.ListView;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Network.Response;
using Assets.Scripts.Common;
using System.IO;
using System;
using Assets.Scripts.Common.Config;
using Assets.Scripts.TalentList;

namespace Assets.Scripts.Archive
{
    public class BindParam
    {
        public Contents contents { get; set; }
        public string description { get; set; }
        public bool isMultiSelectable { get; set; }
        public Action<BaseScanDataListItem> onClick { get; set; }
    }
    
    [RequireComponent(typeof(Button))]
    public abstract class BaseScanDataListItem : BaseListItem<BindParam>
    {
        [SerializeField]
        private Image _image = default;
        public Image image
        {
            get
            {
                return _image;
            }
        }

        [SerializeField]
        private Text _description = default;
        public Text description
        {
            get
            {
                return _description;
            }
        }

        [SerializeField]
        private Text _date = default;
        public Text date
        {
            get
            {
                return _date;
            }
        }

        [SerializeField]
        private Toggle _checkMark = default;
        public Toggle checkMark
        {
            get
            {
                return _checkMark;
            }
        }

        [SerializeField]
        private RectTransform _checkMarkArea = default;
        public RectTransform checkMarkArea
        {
            get
            {
                return _checkMarkArea;
            }
        }

        private Button _button;
        private Button button
        {
            get
            {
                return _button = _button ?? GetComponent<Button>();
            }
        }

        private BindParam _bindParam;

        public Contents contents
        {
            get
            {
                return _bindParam.contents;
            }
        }

        public abstract BaseSceneLoadParam sceneLoadParam { get; }

        private bool _isLoaded = false;

        protected override void Start()
        {
            button.onClick.AddListener(OnClick);
        }

        public override void Bind(BindParam param)
        {
            _bindParam = param;

            var path = DataManager.Instance.CachePath.GetThumbnailFilePath(param.contents.ResourceId);
            if (File.Exists(path) && !_isLoaded)
            {
                var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                var bin = new BinaryReader(fileStream);
                var readBinary = bin.ReadBytes((int)bin.BaseStream.Length);

                bin.Close();

                var texture = new Texture2D(0, 0);
                texture.LoadImage(readBinary);

                var baseTexture = image.sprite.texture;
                TextureScale.Bilinear(texture, baseTexture.width, baseTexture.height);
                
                image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                _isLoaded = true;
            }
            _checkMarkArea.gameObject.SetActive(param.isMultiSelectable);
            description.text = param.description;
            date.text = param.contents.CreateTimeAsDateTime.ToString(AppConst.DateTimeFormat);
        }
        
        public void OnClick()
        {
            _bindParam.onClick(this);
        }
    }
}