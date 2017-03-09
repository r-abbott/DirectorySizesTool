namespace DriveDirectorySize.Domain.Contracts
{
    public interface IDriveDirectory
    {
        string Name { get; }
        string ParentName { get; }
        long Size { get; }
        long TotalSize { get; }
    }
}
