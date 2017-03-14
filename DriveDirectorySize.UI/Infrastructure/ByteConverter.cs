using DriveDirectorySize.UI.Contracts;

namespace DriveDirectorySize.UI.Infrastructure
{
    public class ByteConversion : ISizeConversion
    {
        public string Convert(long bytes)
        {
            var kb = bytes / 1000;
            if (kb >= 1000)
            {
                var mb = kb / 1000;
                if (mb >= 1000)
                {
                    var gb = mb / 1000;
                    return $"{gb} GB";
                }
                return $"{mb} MB";
            }
            return $"{kb} KB";
        }
    }
}
