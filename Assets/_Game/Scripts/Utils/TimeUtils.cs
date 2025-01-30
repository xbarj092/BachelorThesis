using System;

public static class TimeUtils
{
    public static string GetFormattedTimeFromSeconds(int seconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);

        string timeText = "";

        int totalHours = (int)timeSpan.TotalHours;

        if (totalHours > 0)
        {
            timeText += $"{totalHours}H ";
        }
        if (timeSpan.Minutes > 0 || totalHours > 0)
        {
            timeText += $"{timeSpan.Minutes}M ";
        }

        timeText += $"{timeSpan.Seconds}S";

        return timeText.Trim();
    }
}
