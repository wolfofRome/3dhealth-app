using UnityEngine;
using System.Collections;
using Assets.Scripts.Calendar;
using Assets.Scripts.Common.Graphics.UI;

namespace Assets.Scripts.Calender
{ 
    public class ToDayButton : MonoBehaviour
    {
        [SerializeField]
        CalendarViewPager calenderViewPager = default;

        void Start()
        {

        }
        public void OnClick()
        {
            calenderViewPager.SetToDay();
            calenderViewPager.Invalidate();
        }
    }
}
