using Assets.Scripts.Common;
using Assets.Scripts.TalentList;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Network.Response
{

    [Serializable]
    public class TalentoContents : Contents
    {
        [SerializeField]
        private int talent_resource_id = default;
        public int TalentResourceId
        {
            get
            {
                return talent_resource_id;
            }
        }

        [SerializeField]
        private int talent_id = default;
        public int TalentId
        {
            get
            {
                return talent_id;
            }
        }

        [SerializeField]
        private int point = default;
        public int Point
        {
            get
            {
                return point;
            }
            set
            {
                point = value;
            }
        }


        [SerializeField]
        private string image = default;
        public string ImagePath
        {
            get
            {
                return image;
            }
        }


        [SerializeField]
        private int order = default;
        public int Order
        {
            get
            {
                return order;
            }
        }

        [SerializeField]
        private string comment = default;
        public string Comment
        {
            get
            {
                return comment;
            }
        }

        [SerializeField]
        private string job = default;
        public string Job
        {
            get
            {
                return job;
            }
        }

        [SerializeField]
        private string common_name = default;
        public string CommonName
        {
            get
            {
                return common_name;
            }
        }

        [SerializeField]
        private string gender = default;
        public Gender Gender
        {
            get
            {
                return GenderExtension.Parse(gender);
            }
        }

        [SerializeField]
        private string status = default;
        public PurchaseStatus purchaseStatus
        {
            get
            {
                return PurchaseStatusExtension.Parse(status);
            }
            set
            {
                status = value.ToAppString();
            }
        }
        
        [SerializeField]
        private int categories = default;
        public int Categories
        {
            get
            {
                return categories;
            }
        }
        

        private Sprite _bannerSprite;
        public Sprite bannerSprite
        {
            get
            {
                return _bannerSprite;
            }
        }
        
        public IEnumerator GetBannerImage(Action<Sprite> callback)
        {
            yield return ApiHelper.Instance.GetFileAsBinary(ImagePath, res =>
            {
                if (res.ErrorCode != null)
                {
                    DebugUtil.LogError("GetBannerImage - GetFileAsBinary Error -> " + res.ErrorCode + " : " + res.ErrorMessage);
                    return;
                }

                Texture2D texture = new Texture2D(0, 0);
                texture.LoadImage(res.Data);

                _bannerSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                if (callback != null)
                {
                    callback.Invoke(_bannerSprite);
                }
            });
        }
    }
}
