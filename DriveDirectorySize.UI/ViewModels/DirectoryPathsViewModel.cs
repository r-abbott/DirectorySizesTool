using DriveDirectorySize.Domain.Models;
using DriveDirectorySize.Domain.ValueObjects;
using DriveDirectorySize.UI.Contracts;
using DriveDirectorySize.UI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveDirectorySize.UI.ViewModels
{
    public class DirectoryPathsViewModel : IViewModel
    {
        private IEnumerable<DirectorySizeData> _directories;
        private ISizeConversion _sizeConversion;
        private Dictionary<string, Size> _formattedDirectories;


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
            UIConsole.WriteLine(_header);
            UIConsole.WriteHorizontalRule();
        }

        private void Render(KeyValuePair<string,Size> directory)
        {
            var output = $"{directory.Key,PATH_COLUMN_WIDTH}{directory.Value,SIZE_COLUMN_WIDTH}";
            if(directory.Value.SizeType == SizeType.Gigabytes)
            {
                UIConsole.WriteColorLine(ConsoleColor.Red, output);
            }
            else if (directory.Value.SizeType == SizeType.Megabytes)
            {
                UIConsole.WriteColorLine(ConsoleColor.Yellow, output);
            }
            else
            {
                UIConsole.WriteLine(output);
            }
        }

        private void Format()
        {
            _formattedDirectories = new Dictionary<string, Size>();

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
                            var identity = youngest.Path.IdentityAtDepth(i);
                            if (identity.Length < 6)
                            {
                                identity += "\\";
                            }
                            else
                            {
                                if(identity.Length > 5)
                                {
                                    identity = $"{identity.Substring(0, 5)}~\\";
                                }
                            }
                            formattedPath += $"{identity}";
                        }
                        else
                        {
                            formattedPath += $"{dir.Path.IdentityAtDepth(i)}\\";
                        }
                    }
                }

                _formattedDirectories.Add(formattedPath, Size.BestFit(dir.TotalSize));
            }
        }
    }
}
