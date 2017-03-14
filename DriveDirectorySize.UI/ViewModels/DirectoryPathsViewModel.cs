using DriveDirectorySize.Domain.Models;
using DriveDirectorySize.UI.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveDirectorySize.UI.ViewModels
{
    public class DirectoryPathsViewModel
    {
        private IEnumerable<DirectorySizeData> _directories;
        private ISizeConversion _sizeConversion;
        private Dictionary<string, long> _formattedDirectories;


        private const int PATH_COLUMN_WIDTH = -139;
        private const int SIZE_COLUMN_WIDTH = 10;

        private static string _header = $"{"Path", PATH_COLUMN_WIDTH}{"Total Size",SIZE_COLUMN_WIDTH}";

        public DirectoryPathsViewModel(IEnumerable<DirectorySizeData> directories, ISizeConversion sizeConversion)
        {
            _directories = directories;
            _sizeConversion = sizeConversion;
            Format();
        }

        public void Render()
        {
            RenderHeader();
            foreach (var directory in _formattedDirectories)
            {
                Render(directory);
            }
        }

        private void RenderHeader()
        {
            Console.WriteLine(_header);
            Console.WriteLine(new string('-', Console.BufferWidth - 1));
        }

        private void Render(KeyValuePair<string,long> directory)
        {
            var size = _sizeConversion.Convert(directory.Value);
            var output = $"{directory.Key,PATH_COLUMN_WIDTH}{size,SIZE_COLUMN_WIDTH}";
            if (size.Contains("GB"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (size.Contains("MB"))
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

        private void Format()
        {
            _formattedDirectories = new Dictionary<string, long>();

            foreach (var dir in _directories)
            {
                string formattedPath = dir.Path.FullPath;

                var ancestors = _directories.Where(d => dir.Path.IsDescendentOf(d.Path));

                if (ancestors.Any())
                {
                    var youngest = ancestors.OrderByDescending(a => a.Path.Depth).First();

                    formattedPath = "";

                    for (int i = 0; i <= dir.Path.Depth; i++)
                    {
                        if (i <= youngest.Path.Depth)
                        {
                            formattedPath += ".\\";
                        }
                        else
                        {
                            formattedPath += $"{dir.Path.IdentityAtDepth(i)}\\";
                        }
                    }
                }

                _formattedDirectories.Add(formattedPath, dir.TotalSize);
            }
        }
    }
}
