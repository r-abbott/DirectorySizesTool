using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DriveDirectorySize.Domain;
using DriveDirectorySize.Domain.Contracts;

namespace DriveDirectorySize.UI
{
    public class Runner
    {
        private IDriveSizeStorage _storage = new DiskStorage();
        private IDrive _drive;

        public Runner()
        {
            _drive = new Drive(_storage);
        }

        public void Run()
        {
            var reader = _drive.ReadFromStorage();
            if(reader == null)
            {
                reader = _drive.Read("c:\\");
            }
            ProcessCommands(reader);
        }

        private void ProcessCommands(IDriveReader reader)
        {
            PrintCurrentDirectory(reader.CurrentDirectory, reader.CurrentSubDirectories);

            var command = GetInput(reader.CurrentDirectory);
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

                        var directory = reader.ChangeDirectory(parts[1]);

                        if (directory == null)
                        {
                            Console.WriteLine("Directory not found.");
                        }
                        else
                        {
                            PrintCurrentDirectory(reader.CurrentDirectory, reader.CurrentSubDirectories);
                        }
                        break;
                    case "largest":
                        var largestDirectory = reader.FindLargestSubDirectory();
                        if (largestDirectory == null)
                        {
                            Console.WriteLine("Invalid command, directory has no subdirectories.");
                        }
                        else
                        {
                            PrintDirectory(largestDirectory);
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid command.");
                        break;
                }
                command = GetInput(reader.CurrentDirectory);
            }
        }

        private string GetInput(IDriveDirectory currentDirectory)
        {
            Console.Write($"\n{currentDirectory.Name}> ");
            return Console.ReadLine();
        }

        private void PrintCurrentDirectory(IDriveDirectory currentDirectory, IEnumerable<IDriveDirectory> subDirectories)
        {
            Console.WriteLine("");
            PrintDirectory(currentDirectory);
            if (subDirectories.Count() > 0)
            {
                Console.WriteLine($"\nSub Directories:");
                foreach (var sd in subDirectories)
                {
                    PrintDirectory(sd);
                }
            }
        }

        private void PrintDirectory(IDriveDirectory directory)
        {
            Console.WriteLine($"{directory.Name,-38}{directory.Size,20}{directory.TotalSize,20}");
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            var runner = new Runner();
            runner.Run();
        }
    }
}
