using System;
using System.Collections.Generic;
using System.Linq;
using DriveDirectorySize.Domain;
using DriveDirectorySize.Domain.Contracts;
using DriveDirectorySize.Domain.Models;

namespace DriveDirectorySize.UI
{
    public class Runner
    {
        private IDrive _drive;

        public Runner()
        {
            _drive = new Drive();
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
                    case "large":
                        var result = reader.FindLargestDirectories();
                        foreach(var r in result)
                        {
                            PrintFullPathDirectory(r);
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid command.");
                        break;
                }
                command = GetInput(reader.CurrentDirectory);
            }
        }

        private string GetInput(DirectorySizeData currentDirectory)
        {
            Console.Write($"\n{string.Join(@"\",currentDirectory.Path)}> ");
            return Console.ReadLine();
        }

        private void PrintCurrentDirectory(DirectorySizeData currentDirectory, IEnumerable<DirectorySizeData> subDirectories)
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

        private void PrintDirectory(DirectorySizeData directory)
        {
            var name = directory.Name;
            if (name.Length > 40)
            {
                name = $"{name.Substring(0, 40)}~";
            }
            Console.WriteLine($"{name,-41}{ByteConversion.Convert(directory.Size),19}{ByteConversion.Convert(directory.TotalSize),19}");
        }

        private void PrintFullPathDirectory(DirectorySizeData directory)
        {
            Console.WriteLine($"{directory.Path,-60}{ByteConversion.Convert(directory.TotalSize),19}");
        }

    }

    public class ByteConversion
    {
        public long Raw { get; }
        public string Value { get; private set; }

        public ByteConversion(long bytes)
        {
            Raw = bytes;
        }

        public static string Convert(long bytes)
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

        private void ConvertToLargestSignificance()
        {
            var kb = Raw / 1000;
            if(kb >= 1000)
            {
                var mb = kb / 1000;
                if(mb >= 1000)
                {
                    var gb = mb / 1000;
                    Value = $"{gb} GB";
                    return;
                }
                Value = $"{mb} MB";
                return;
            }
            Value = $"{kb} KB";
        }

        public override string ToString()
        {
            return base.ToString();
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
