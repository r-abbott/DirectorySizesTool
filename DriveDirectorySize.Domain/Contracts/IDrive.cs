namespace DriveDirectorySize.Domain.Contracts
{
    public interface IDrive
    {
        IDriveReader ReadFromStorage();
        IDriveReader Read(string drive);
    }
}
