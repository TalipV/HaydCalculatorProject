using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCalendar.Core.Enums;
using XCalendar.Core.Models;

namespace MauiTestApp
{
    public class CustomDay : CalendarDay
    {
        private Color _mainColor = Colors.White;

        public Color MainColor
        {
            get
            {
                return _mainColor;
            }
            set
            {
                if (_mainColor != value)
                {
                    _mainColor = value;
                    OnPropertyChanged(nameof(MainColor));
                }
            }
        }

        private Color _mainTextColor = Colors.Black;

        public Color MainTextColor
        {
            get
            {
                return _mainTextColor;
            }
            set
            {
                if (_mainTextColor != value)
                {
                    _mainTextColor = value;
                    OnPropertyChanged(nameof(MainTextColor));
                }
            }
        }
    }
}
