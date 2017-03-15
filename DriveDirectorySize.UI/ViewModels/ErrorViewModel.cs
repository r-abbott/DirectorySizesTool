using DriveDirectorySize.UI.Infrastructure;

namespace DriveDirectorySize.UI.ViewModels
{
    public class ErrorViewModel :IViewModel
    {
        private string _message;

        public ErrorViewModel(string message)
        {
            _message = message;
        }

        public void Render()
        {
            UIConsole.WriteLine(_message);
        }
    }
}
