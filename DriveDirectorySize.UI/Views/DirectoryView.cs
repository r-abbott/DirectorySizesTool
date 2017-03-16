using DriveDirectorySize.UI.CommandHandlers;
using DriveDirectorySize.UI.Controllers;
using DriveDirectorySize.UI.ViewModels;

namespace DriveDirectorySize.UI.Views
{
    public class DirectoryView
    {
        public void Display(IViewModel viewModel, ICommandHandler commandHandler)
        {
            viewModel.Render();
            commandHandler.HandleInput();
        }
    }
}
