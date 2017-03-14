using System;
using DriveDirectorySize.Domain;
using DriveDirectorySize.Domain.Contracts;
using DriveDirectorySize.UI.Contracts;
using DriveDirectorySize.UI.ViewModels;
using DriveDirectorySize.UI.Infrastructure;
using System.Linq;
using System.Collections.Generic;

namespace DriveDirectorySize.UI
{
    public class Runner
    {
        private IDrive _drive;
        private ISizeConversion _sizeConversion = new ByteConversion();
        private IDriveReader _reader;

        public void Run()
        {
            var driveLetter = GetDriveLetter();
            _drive = new Drive(driveLetter, new ProcessLogger());

            _reader = _drive.ReadFromStorage();
            if(_reader == null)
            {
                _reader = _drive.Read();
            }
            ProcessCommands();
        }

        private char GetDriveLetter()
        {
            char drive = 'c';
            bool isValidDrive = false;
            IEnumerable<char> drives = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

            while (!isValidDrive)
            {
                Console.Write("Which drive do you want to read: ");
                var response = Console.ReadLine().ToLower();
                if (response.Length > 0)
                {
                    drive = response.First();
                    isValidDrive = drives.Contains(drive);
                }
            }
            return drive;
        }

        private void ProcessCommands()
        {
            DisplayCurrentDirectory();

            var command = GetInput();
            while (command != "quit")
            {
                var parts = command.Split(' ');
                switch (parts[0])
                {
                    case "cd":
                        if (parts.Length < 2)
                        {
                            Console.WriteLine("Invalid parameters for 'open'");
                            break;
                        }
                        ChangeDirectory(parts.Skip(1));
                        break;
                    case "largest":
                    case "large":
                        double percentThreshold = .05d;
                        if (parts.Length > 1)
                        {
                            double.TryParse(parts[1], out percentThreshold);
                        }
                        DisplayLargestDirectories(percentThreshold);
                        break;
                    default:
                        Console.WriteLine("Invalid command.");
                        break;
                }
                command = GetInput();
            }
        }

        private void ChangeDirectory(IEnumerable<string> name)
        {
            var directory = _reader.ChangeDirectory(string.Join(" ",name));

            if (directory == null)
            {
                Console.WriteLine("Directory not found.");
            }
            else
            {
                DisplayCurrentDirectory();
            }
        }

        private void DisplayCurrentDirectory()
        {
            var viewModel = new DirectoryWithSubDirectoriesViewModel(_reader.CurrentDirectory, _reader.CurrentSubDirectories, _sizeConversion);
            viewModel.Render();
        }

        private void DisplayLargestDirectories(double percentThreshold)
        {
            var result = _reader.FindLargestDirectories(percentThreshold);
            var viewModel = new DirectoryPathsViewModel(result, _sizeConversion);
            viewModel.Render();
        }

        private string GetInput()
        {
            Console.Write($"\n{string.Join(@"\", _reader.CurrentDirectory.Path)}\\> ");
            return Console.ReadLine();
        }
    }

    

    class Program
    {
        static void Main(string[] args)
        {
            Console.WindowWidth = 150;
            Console.WindowHeight = 40;

            var runner = new Runner();
            runner.Run();
        }
    }
}
