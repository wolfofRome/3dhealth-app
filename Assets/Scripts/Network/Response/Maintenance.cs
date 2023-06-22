using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.Network.Response
{
    [Serializable]
    public class Maintenance
    {
        [SerializeField]
        private bool is_maintenance;
        public bool isMaintenance
        {
            get
            {
                return is_maintenance;
            }
            set
            {
                is_maintenance = value;
            }
        }

        [SerializeField]
        private bool is_android;
        public bool _isAndroid
        {
            get
            {
                return is_android;
            }
            set
            {
                is_android = value;
            }
        }

        [SerializeField]
        private bool is_ios;
        public bool _isIos
        {
            get
            {
                return is_ios;
            }
            set
            {
                is_ios = value;
            }
        }

        [SerializeField]
        private string to;
        public string _to
        {
            get
            {
                return to;
            }
            set
            {
                to = value;
            }
        }

        [SerializeField]
        private string from;
        public string _from
        {
            get
            {
                return from;
            }
            set
            {
                from = value;
            }
        }

        [SerializeField]
        private string html;
        public string _html
        {
            get
            {
                return html;
            }
            set
            {
                html = value;
            }
        }
    }
}
