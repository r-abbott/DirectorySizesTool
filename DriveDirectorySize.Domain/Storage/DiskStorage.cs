using DriveDirectorySize.Domain.Contracts;
using DriveDirectorySize.Domain.Models;
using DriveDirectorySize.Domain.Storage.Contracts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace DriveDirectorySize.Domain.Storage
{
    internal class DiskStorage : IDriveSizeStorage
    {
        private readonly ILog _output;

        public DiskStorage(ILog output)
        {
            _output = output;
        }

        public bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }

        public IEnumerable<DirectorySizeData> Retrieve(string filePath)
        {
            WriteOutput("Looking for save file.\n");
            if (Exists(filePath))
            {
                WriteOutput("Save file found, reading... ");
                var data = File.ReadAllText(filePath);
                WriteOutput("Done!\n");
                WriteOutput("Converting... ");
                var converted = JsonConvert.DeserializeObject<IEnumerable<DirectorySizeData>>(data);
                WriteOutput("Done!\n");
                return converted;
            }
            WriteOutput("No save file found.\n");
            return null;
        }

        public void Save(string filePath, IEnumerable<DirectorySizeData> data)
        {
            WriteOutput($"Saving data to {filePath}... ");
            var json = JsonConvert.SerializeObject(data);
            File.WriteAllText(filePath, json);
            WriteOutput("File saved!\n");
        }

        private void WriteOutput(string text)
        {
            if (_output != null)
            {
                _output.Write(text);
            }
        }
    }
}
