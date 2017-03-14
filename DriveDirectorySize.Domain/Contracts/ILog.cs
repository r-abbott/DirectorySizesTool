namespace DriveDirectorySize.Domain.Contracts
{
    public interface ILog
    {
        void Write(string text);
        void WriteLine(string text);
        void WriteConcurrent(string text);
    }
}
