using DriveDirectorySize.Domain.ValueObjects;
using DriveDirectorySize.UI.Controllers;
using DriveDirectorySize.UI.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace DriveDirectorySize.UI.CommandHandlers
{
    public class DriveReaderCommandHandler : ICommandHandler
    {
        private DirectoryController _controller;

        public DriveReaderCommandHandler(DirectoryController controller)
        {
            _controller = controller;
        }

        public void HandleInput()
        {
            var input = GetInput();

            while (true)
            {
                var parts = input.Split(' ');
                switch (parts[0])
                {
                    case "quit":
                        _controller.Quit();
                        break;
                    case "cd":
                        ChangeDirectory(parts.Skip(1));
                        break;
                    case "discover":
                        DiscoverLargestDirectories(parts);
                        break;
                    case "greater":
                        DisplayDirectoriesLargerThan(parts);
                        break;
                    case "read":
                        ReadDrive(parts);
                        break;
                    case "open":
                        Open(parts);
                        break;
                    default:
                        UIConsole.WriteLine("Invalid command.\nCommands are\n  cd [subdirectory name]\n  discover <[percent threshold]>\n  greater [size] <[b/kb/mb/gb]>\n  quit \n  read [drive letter]");
                        break;
                }
                input = GetInput();
            }
        }

        private string GetInput()
        {
            UIConsole.Write($"\n{string.Join(@"\", _controller.CurrentDirectoryPath)}\\> ");
            return UIConsole.ReadLine();
        }

        private void ChangeDirectory(IEnumerable<string> name)
        {
            if (!name.Any())
            {
                UIConsole.WriteLine("Invalid parameters for 'cd'. (cd [subdirectory name])");
                return;
            }

            var directoryName = string.Join(" ", name);
            _controller.ChangeDirectory(directoryName);
        }

        private void DiscoverLargestDirectories(string[] parts)
        {
            double percentThreshold = .05d;
            if (parts.Length > 1)
            {
                double.TryParse(parts[1], out percentThreshold);
                if(percentThreshold > 1d)
                {
                    percentThreshold = percentThreshold / 100d;
                }
            }
            _controller.DiscoverLargestDirectories(percentThreshold);
        }

        private void DisplayDirectoriesLargerThan(string[] parts)
        {
            if (parts.Length < 2)
            {
                UIConsole.WriteLine("Invalid parameters for 'greater'. (greater [size] [b/kb/mb/gb])");
                return;
            }

            long value;
            if (!long.TryParse(parts[1], out value))
            {
                UIConsole.WriteLine("Invalid input. Should be \"greater [size] [b/kb/mb/gb]\"");
                return;
            }

            Size size;
            if (parts.Length < 4)
            {
                // Default to GB
                size = Size.GigaBytes(value);
            }
            else
            {
                var inputParts = string.Join(" ", parts.Skip(1).Take(2));
                if (!Size.TryParse(inputParts, out size))
                {
                    UIConsole.WriteLine("Invalid input. Should be \"greater [size] [b/kb/mb/gb]\"");
                    return;
                }
            }
            _controller.DirectoriesLargerThan(size);
        }

        private void ReadDrive(string[] parts)
        {
            if (parts.Length < 2)
            {
                UIConsole.WriteLine("Invalid parameters for 'read'. (read [drive letter])");
                return;
            }
            _controller.ReadDrive(parts[1].ToLower()[0]);
        }

        private void Open(string[] parts)
        {
            string path = _controller.CurrentDirectoryPath;
            if (parts.Length > 1)
            {
                path = parts[1];
            }
            try
            {
                Process.Start(path);
            }
            catch(Win32Exception win32Exception)
            {
                UIConsole.WriteLine($"Couldn't open {path}. {win32Exception.Message}");
            }
        }
    }
}
