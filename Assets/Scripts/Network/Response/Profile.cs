using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts.Common;

namespace Assets.Scripts.Network.Response
{
    [Serializable]
    public class Profile
    {
        [SerializeField]
        private int user_id = default;
        public int UserId {
            get {
                return user_id;
            }
            set {
                user_id = value;
            }
        }

        [SerializeField]
        private string email = default;
        public string Email {
            get {
                return email;
            }
            set {
                email = value;
            }
        }

        [SerializeField]
        private string next_email = default;
        public string NextEmail {
            get {
                return next_email;
            }
            set {
                next_email = value;
            }
        }

        [SerializeField]
        private string lastname = default;
        public string LastName {
            get {
                return lastname;
            }
            set {
                lastname = value;
            }
        }

        [SerializeField]
        private string firstname = default;
        public string FirstName {
            get {
                return firstname;
            }
            set {
                firstname = value;
            }
        }

        [SerializeField]
        private string lastname_kana = default;
        public string LastNameKana {
            get {
                return lastname_kana;
            }
            set {
                LastNameKana = value;
            }
        }

        [SerializeField]
        private string firstname_kana = default;
        public string FirstNameKana {
            get {
                return firstname_kana;
            }
            set {
                firstname_kana = value;
            }
        }

        [SerializeField]
        private string phone = default;
        public string Phone {
            get {
                return phone;
            }
            set {
                phone = value;
            }
        }
        [SerializeField]
        private string mobile = default;
        public string Mobile {
            get {
                return mobile;
            }
            set {
                mobile = value;
            }
        }

        [SerializeField]
        private string next_mobile = default;
        public string NextMobile {
            get {
                return next_mobile;
            }
            set {
                next_mobile = value;
            }
        }

        [SerializeField]
        private string birthday = default;
        public string Birthday {
            get {
                return birthday;
            }
            set {
                birthday = value;
            }
        }
        
        public DateTime BirthdayOfDateTime {
            get {
                var date = DateTime.MinValue;
                if (DateTime.TryParse(Birthday, out date) )
                {
                    return date;
                }
                return DateTime.MinValue;
            }
        }

        public int GetAge(DateTime date)
        {
            var birthDay = BirthdayOfDateTime;
            int age = date.Year - birthDay.Year;
            if (date.Month < birthDay.Month || (date.Month == birthDay.Month && date.Day < birthDay.Day))
            {
                age--;
            }
            return age;
        }

        [SerializeField]
        private string nickname = default;
        public string Nickname {
            get {
                return nickname;
            }
            set {
                nickname = value;
            }
        }

        [SerializeField]
        private string gender = default;
        public Gender Gender {
            get {
                return GenderExtension.Parse(gender);
            }
            set {
                gender = value.ToString();
            }
        }

        [SerializeField]
        private string zip_code = default;
        public string ZipCode {
            get {
                return zip_code;
            }
            set {
                zip_code = value;
            }
        }
        [SerializeField]
        private int prefecture = default;
        public int Prefecture {
            get {
                return prefecture;
            }
            set {
                prefecture = value;
            }
        }

        [SerializeField]
        private string city = default;
        public string City {
            get {
                return city;
            }
            set {
                city = value;
            }
        }

        [SerializeField]
        private string address1 = default;
        public string Address1 {
            get {
                return address1;
            }
            set {
                address1 = value;
            }
        }

        [SerializeField]
        private string address2 = default;
        public string Address2 {
            get {
                return address2;
            }
            set {
                address2 = value;
            }
        }

        [SerializeField]
        private int height = default;
        public int Height {
            get {
                return height;
            }
            set {
                height = value;
            }
        }
    }
}
