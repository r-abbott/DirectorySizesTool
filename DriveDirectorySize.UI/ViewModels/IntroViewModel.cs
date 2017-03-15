using DriveDirectorySize.UI.Infrastructure;

namespace DriveDirectorySize.UI.ViewModels
{
    public class IntroViewModel : IViewModel
    {
        public void Render()
        {
            UIConsole.Write("Drive Directory Sizes Tool.\n");
            UIConsole.WriteHorizontalRule();
        }
    }
}
