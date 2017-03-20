using DriveDirectorySize.UI.Controllers;
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
            var availableDrives = string.Join(",", _controller.AvailableDrives());
            UIConsole.Write($"Which drive do you want to read ({availableDrives}): ");
            return UIConsole.ReadLine();
        }
    }
}
