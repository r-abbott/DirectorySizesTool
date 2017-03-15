using DriveDirectorySize.Domain.Contracts;

namespace DriveDirectorySize.UI.Infrastructure
{
    public class ProcessLogger : ILog
    {
        public void Write(string text)
        {
            UIConsole.Write(text);
        }

        public void WriteConcurrent(string text)
        {
            UIConsole.WriteConcurrent(text);
        }

        public void WriteLine(string text)
        {
            UIConsole.WriteLine(text);
        }
    }
}
