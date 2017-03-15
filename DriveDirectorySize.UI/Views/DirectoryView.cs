using DriveDirectorySize.UI.CommandHandlers;
using DriveDirectorySize.UI.ViewModels;

namespace DriveDirectorySize.UI.Views
{
    public class DirectoryView
    {
        private readonly DirectoryController _controller;

        public DirectoryView(DirectoryController controller)
        {
            _controller = controller;
        }

        public void Display(IViewModel viewModel, ICommandHandler commandHandler)
        {
            viewModel.Render();
            commandHandler.HandleInput();
        }
    }
}
