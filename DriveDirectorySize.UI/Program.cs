namespace DriveDirectorySize.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            var controller = new DirectoryController();
            controller.Handle();
        }
    }
}
