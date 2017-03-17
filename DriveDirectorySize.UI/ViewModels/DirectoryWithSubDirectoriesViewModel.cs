using DriveDirectorySize.Domain.Models;
using DriveDirectorySize.UI.Contracts;
using DriveDirectorySize.UI.Infrastructure;
using System;
using System.Collections.Generic;

namespace DriveDirectorySize.UI.ViewModels
{
    public class DirectoryWithSubDirectoriesViewModel : IViewModel
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
            ISizeConversion sizeConversion)
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
                RenderSubDirectory(directory);
            }
        }

        private void RenderHeader()
        {
            Console.WriteLine(_header);
            Console.WriteLine(new string('-', Console.BufferWidth-1));
        }

        private void Render(DirectorySizeData directory)
        {
            var size = _sizeConversion.Convert(directory.LocalSize);
            var totalSize = _sizeConversion.Convert(directory.TotalSize);
            var output = $"{directory.Name,NAME_COLUMN_WIDTH}{size,SIZE_COLUMN_WIDTH}{totalSize,SIZE_COLUMN_WIDTH}";
            RenderColorLine(totalSize, output);
        }

        private void RenderSubDirectory(DirectorySizeData directory)
        {
            var size = _sizeConversion.Convert(directory.LocalSize);
            var totalSize = _sizeConversion.Convert(directory.TotalSize);
            var output = $"  {directory.Name,NAME_COLUMN_WIDTH+2}{size,SIZE_COLUMN_WIDTH}{totalSize,SIZE_COLUMN_WIDTH}";
            RenderColorLine(totalSize, output);            
        }

        private void RenderColorLine(string totalSize, string output)
        {
            if (totalSize.Contains("GB"))
            {
                UIConsole.WriteColorLine(ConsoleColor.Red, output);
            }
            else if (totalSize.Contains("MB"))
            {
                UIConsole.WriteColorLine(ConsoleColor.Yellow, output);
            }
            else
            {
                UIConsole.WriteLine(output);
            }
        }
    }
}
