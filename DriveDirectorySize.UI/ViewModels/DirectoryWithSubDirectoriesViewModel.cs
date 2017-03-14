using DriveDirectorySize.Domain.Models;
using DriveDirectorySize.UI.Contracts;
using System;
using System.Collections.Generic;

namespace DriveDirectorySize.UI.ViewModels
{
    public class DirectoryWithSubDirectoriesViewModel
    {
        private DirectorySizeData _directory;
        private IEnumerable<DirectorySizeData> _subDirectories;
        private ISizeConversion _sizeConversion;

        private const int NAME_COLUMN_WIDTH = -109;
        private const int SIZE_COLUMN_WIDTH = 20;

        private static string _header = $"{"Directory Name", NAME_COLUMN_WIDTH}{"Local Size",SIZE_COLUMN_WIDTH}{"Total Size",SIZE_COLUMN_WIDTH}";

        public DirectoryWithSubDirectoriesViewModel(
            DirectorySizeData directory,
            IEnumerable<DirectorySizeData> subDirectories,
            ISizeConversion sizeConversion
            )
        {
            _directory = directory;
            _subDirectories = subDirectories;
            _sizeConversion = sizeConversion;
        }

        public void Render()
        {
            RenderHeader();
            Render(_directory);
            Console.WriteLine();
            foreach(var directory in _subDirectories)
            {
                Render(directory);
            }
        }

        private void RenderHeader()
        {
            Console.WriteLine(_header);
            Console.WriteLine(new string('-', Console.BufferWidth-1));
        }

        private void Render(DirectorySizeData directory)
        {
            var size = _sizeConversion.Convert(directory.Size);
            var totalSize = _sizeConversion.Convert(directory.TotalSize);
            var output = $"{directory.Name,NAME_COLUMN_WIDTH}{size,SIZE_COLUMN_WIDTH}{totalSize,SIZE_COLUMN_WIDTH}";

            if (totalSize.Contains("GB"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if(totalSize.Contains("MB"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine(output);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
