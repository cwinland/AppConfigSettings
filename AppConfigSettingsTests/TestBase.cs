using System.IO;
using System.Text;

namespace AppConfigSettingsTests
{
    public abstract class TestBase
    {
        protected const string APP_SETTINGS_NAME = "appsettings";
        protected const string APP_SETTINGS_EXT = "json";
        protected const string ASP_ENVIRONMENT = "ASPNETCORE_ENVIRONMENT";

        protected const string BAD_VAL = "BadVal";
        protected const string GOOD_DATE = "11/11/1998";
        protected const string BAD_DATE = "11/11/1997";
        protected const int GOOD_INT = 22;

        protected static string CreateFile(string name, string content, string path)
        {
            Directory.CreateDirectory(path);
            var fullPath = Path.Combine(Path.Combine(path, name));

            using var file = File.Create(fullPath);

            var text = Encoding.ASCII.GetBytes(content);
            file.Write(text, 0, text.Length);

            return fullPath;
        }

        protected static void DeleteFile(string path) => File.Delete(path);
    }
}
