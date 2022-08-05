namespace IssueTracker.Helpers
{
    public static class DateTimeHelpers
    {
        public static string FormatElapsedTime(DateTime? UtcDateTime)
        {
            if (UtcDateTime == null)
            {
                return "";
            }

            TimeSpan elapsed = DateTime.UtcNow.Subtract((DateTime)UtcDateTime);
            int totalMinutes = (int)Math.Floor(elapsed.TotalMinutes);
            int days = (int)totalMinutes / 1440;
            int hours = (int)totalMinutes % 1440 / 60;
            int minutes = (int)totalMinutes % 60;

            string daysString = days > 0 ? days.ToString() + "d " : "";
            string hoursString = hours > 0 ? hours.ToString() + "h " : "";
            string minutesString = minutes > 0 ? minutes.ToString() + "m " : "1m";

            return daysString + hoursString + minutesString + " ago";
        }
    }
}
