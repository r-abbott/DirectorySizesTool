using DriveDirectorySize.UI.Infrastructure;

namespace DriveDirectorySize.UI.CommandHandlers
{
    public class DriveLetterCommandHandler : ICommandHandler
    {
        private DirectoryController _controller;

        public DriveLetterCommandHandler(DirectoryController controller)
        {
            _controller = controller;
        }

        public void HandleInput()
        {
            var input = GetInput();
            var driveLetter = input.ToLower()[0];
            _controller.ReadDrive(driveLetter);
        }

        private string GetInput()
        {
            UIConsole.Write("Which drive do you want to read: ");
            return UIConsole.ReadLine();
        }
    }
}
