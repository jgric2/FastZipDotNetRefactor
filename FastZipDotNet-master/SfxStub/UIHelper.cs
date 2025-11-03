using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfxStub
{
    public static class UIHelper
    {
        public static string FormatTime(TimeSpan time)
        {
            unchecked
            {
                if (time.Days > 0)
                    return time.Days.ToString() + " Days, " + time.Hours.ToString() + " Hours.";
                else if (time.Hours > 0)
                    return time.Hours.ToString() + " Hours, " + time.Minutes.ToString() + " Minutes.";
                else if (time.Minutes > 0)
                    return time.Minutes.ToString() + " Minutes, " + time.Seconds.ToString() + " Seconds.";
                else if (time.Seconds > 0)
                    return time.Seconds.ToString() + " Seconds, " + time.Milliseconds.ToString() + " Milliseconds.";
                else
                    return time.Milliseconds.ToString() + " Milliseconds.";
            }
        }
    }
}
