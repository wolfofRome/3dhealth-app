using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Network.Response
{
    [Serializable]
    public class GetNotificationResponse : BaseResponse
    {
        [SerializeField]
        private List<Notification> news_list;
        public List<Notification> NewsList
        {
            get => news_list;
            set => news_list = value;
        }
    }
    
    [Serializable]
    public class Notification
    {
        [SerializeField]
        private string display_from;
        public string DisplayFrom
        {
            get => display_from;
            set => display_from = value;
        }

        [SerializeField]
        private string title;
        public string Title
        {
            get => title;
            set => title = value;
        }

        [SerializeField]
        private string body;
        public string Body
        {
            get => body;
            set => body = value;
        }
    }
}
