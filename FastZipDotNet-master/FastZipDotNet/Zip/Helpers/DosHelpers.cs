namespace FastZipDotNet.Zip.Helpers
{
    public static class DosHelpers
    {
        /* DOS Date and time:
    MS-DOS date. The date is a packed value with the following format. Bits Description 
        0-4 Day of the month (1–31) 
        5-8 Month (1 = January, 2 = February, and so on) 
        9-15 Year offset from 1980 (add 1980 to get actual year) 
    MS-DOS time. The time is a packed value with the following format. Bits Description 
        0-4 Second divided by 2 
        5-10 Minute (0–59) 
        11-15 Hour (0–23 on a 24-hour clock) 
*/
        public static DateTime DosTimeToDateTime(uint _dt)
        {
            return new DateTime(
                (int)(_dt >> 25) + 1980,
                (int)(_dt >> 21) & 15,
                (int)(_dt >> 16) & 31,
                (int)(_dt >> 11) & 31,
                (int)(_dt >> 5) & 63,
                (int)(_dt & 31) * 2);
        }

        public static uint DateTimeToDosTime(DateTime _dt)
        {
            return (uint)(
                (_dt.Second / 2) | (_dt.Minute << 5) | (_dt.Hour << 11) |
                (_dt.Day << 16) | (_dt.Month << 21) | ((_dt.Year - 1980) << 25));
        }
    }
}
