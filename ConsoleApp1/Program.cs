using System.Reflection;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            var iconPath = Path.Combine(outPutDirectory, "Images\\Img.jpg");
            string icon_path = new Uri(iconPath).LocalPath;
            Console.WriteLine(outPutDirectory.ToString());
            Console.WriteLine(icon_path);
        }
    }
}