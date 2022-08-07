namespace IssueTracker.Helpers
{
    public static class DateTimeHelpers
    {
        public static string GetSimpleElapsedTime(DateTime? UtcDateTime)
        {
            if (UtcDateTime == null)
            {
                return "";
            }

            TimeSpan elapsed = DateTime.UtcNow.Subtract((DateTime)UtcDateTime);
            int totalMinutes = (int)Math.Floor(elapsed.TotalMinutes);
            int days = (int)totalMinutes / 1440;
            if(days > 0)
            {
                return days + "d ago";
            }
            int hours = (int)totalMinutes % 1440 / 60;
            if(hours > 0)
            {
                return hours + "h ago";
            }
            int minutes = (int)totalMinutes % 60;
            if(minutes > 0)
            {
                return minutes + "m ago";
            }

            return "1m ago";
        }

        public static string GetElapsedTime(DateTime? UtcDateTime)
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

            string daysString = days > 0 ? days + "d " : "";
            string hoursString = hours > 0 ? hours + "h " : "";
            string minutesString = minutes > 0 ? minutes + "m " : "1m";

            return daysString + hoursString + minutesString + " ago";
        }
    }

    
}
