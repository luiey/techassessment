namespace techassessment
{
    public class Global
    {
        public static string localDBConn { get; set; } = string.Empty;
    }
    public class ConnectionStrings
    {
        public string PlatformWell { get; set; }
    }
    public class LoginModel
    {
        public object username { get; set; } = null;
        public object password { get; set; } = null;
    }
    public class Well
    {
        public object id { get; set; } = null;
        public object platformId { get; set; } = null;
        public object uniqueName { get; set; } = null;
        public object latitude { get; set; } = null;
        public object longitude { get; set; } = null;
        public object createdAt { get; set; } = null;
        public object updatedAt { get; set; } = null;
    }

    public class Platform
    {
        public object id { get; set; } = null;
        public object uniqueName { get; set; } = null;
        public object latitude { get; set; } = null;
        public object longitude { get; set; } = null;
        public object createdAt { get; set; } = null;
        public object updatedAt { get; set; } = null;
        public List<Well> well { get; set; }
    }
}
