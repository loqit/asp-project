namespace MovieHub
{
    public class Settings
    {
        public static string DBConnection = "DefaultConnection";
        public static string ResourcesPath = "Resources";
        public static string ExceptionHandler = "/Home/Error";
        public static class Password
        {
            public static bool RequireNonAlphanumeric = false;
            public static bool RequireUppercase = false;
            public static int RequiredLength = 8;

        }

    }
}