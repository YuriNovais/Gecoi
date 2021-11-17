namespace Protocolo.Helpers
{
    public class Alert
    {
        public const string TempDataKey = "TempDataAlerts";

        public string AlertStyle { get; set; }
        public string Icon { get; set; }
        public string Message { get; set; }
    }

    public static class AlertStyles
    {
        public const string Success = "success";
        public const string Information = "info";
        public const string Warning = "warning";
        public const string Danger = "danger";
    }

    public static class IconStyles
    {
        public const string Success = "ok";
        public const string Info = "info";
        public const string Warning = "warning";
        public const string Danger = "exclamation";
    }
}