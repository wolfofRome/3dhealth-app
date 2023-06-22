using Assets.Scripts.Network.Response;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Assets.Scripts.Common;

public class ObjThumbnail
{
    public ObjThumbnail(Contents contents)
    {
        var dm = DataManager.Instance;
        _imagePath = dm.CachePath.GetThumbnailFilePath(contents.ResourceId);
        if (File.Exists(_imagePath) )
        {
            FileStream fileStream = new FileStream(_imagePath, FileMode.Open, FileAccess.Read);
            BinaryReader bin = new BinaryReader(fileStream);
            byte[] readBinary = bin.ReadBytes((int)bin.BaseStream.Length);

            bin.Close();

            Texture2D texture = new Texture2D(0, 0);
            texture.LoadImage(readBinary);

            var defaultTexture = DefaultSprite.texture;
            TextureScale.Bilinear(texture, defaultTexture.width, defaultTexture.height);

            _sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            _imagePath = null;
            _sprite = DefaultSprite;
        }

        _contents = contents;
        _dateText = contents.CreateTimeAsDateTime.ToString("yyyy/MM/dd");
        _timeText = contents.CreateTimeAsDateTime.ToString("HH:mm");
    }

    private Contents _contents;
    public Contents Contents {
        get { return _contents; }
    }

    private string _imagePath;
    public string ImagePath {
        get { return _imagePath; }
    }

    private string _dateText;
    public string DateText {
        get { return _dateText; }
    }

    private string _timeText;
    public string TimeText {
        get { return _timeText; }
    }

    private Sprite _sprite;
    public Sprite sprite
    {
        get { return _sprite; }
    }

    private static Sprite _defaultSprite;
    public static Sprite DefaultSprite {
        get {
            return _defaultSprite = _defaultSprite ?? Resources.Load<Sprite>("Sprites/loading");
        }
    }
}

public class ObjThumbnails
{
    private ObjThumbnails()
    {

    }

    private static ObjThumbnails _instance = new ObjThumbnails();
    public static ObjThumbnails Instance {
        get {
            if (_instance == null)
            {
                _instance = new ObjThumbnails();
            }
            return _instance;
        }
    }

    private List<ObjThumbnail> _list = new List<ObjThumbnail>();
    public List<ObjThumbnail> List {
        get { return _list; }
        set { _list = value; }
    }

    private const int _pageMaxItem = 8;

    public int PageCount {
        get { return (_list.Count / _pageMaxItem) + (_list.Count % _pageMaxItem > 0 ? 1 : 0); }
    }

    public List<ObjThumbnail> GetPageItem(int page)
    {
        if (PageCount < page)
        {
            return new List<ObjThumbnail>();
        }

        int index = (page - 1) * _pageMaxItem;
        int length = _pageMaxItem;

        if (PageCount >= page)
        {
            length = _list.Count - index;
        }

        return _list.GetRange(index, length);
    }
}