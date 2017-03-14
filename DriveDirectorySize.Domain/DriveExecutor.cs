using DriveDirectorySize.Domain.Contracts;
using DriveDirectorySize.Domain.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDirectorySize.Domain
{
    internal class DriveExecutor
    {
        private string _drive;
        private BlockingCollection<DirectorySizeData> _localSizeDirectories;
        private BlockingCollection<DirectorySizeData> _finalDirectories;
        private BlockingCollection<string> _inputDirectories;
        private List<DirectorySizeWorker> _readers;
        private ILog _log;

        public DriveExecutor(string drive, ILog log)
        {
            _drive = drive;
            _log = log;
            _finalDirectories = new BlockingCollection<DirectorySizeData>();
            _localSizeDirectories = new BlockingCollection<DirectorySizeData>();
            _inputDirectories = new BlockingCollection<string>();
            _readers = new List<DirectorySizeWorker>();
        }

        public IEnumerable<DirectorySizeData> Run()
        {
            SetupReaders();
            Task[] workerTasks = new Task[4];
            int taskId = 0;
            _readers.ForEach(r =>
            {
                Task task = Task.Run(() => r.ProcessLocalSizes());
                workerTasks[taskId++] = task;
            });

            WriteOutput("Processing local sizes.");
            var map = new DirectoryMap(_drive, _inputDirectories, _log);
            map.ReadAll();

            Task.WaitAll(workerTasks);
            _localSizeDirectories.CompleteAdding();
            WriteOutput("Finished processing local sizes.");
            WriteOutput("Processing total sizes.");

            taskId = 0;
            _readers.ForEach(r =>
            {
                Task task = Task.Run(() => r.ProcessTotalSizes());
                workerTasks[taskId++] = task;
            });
            Task.WaitAll(workerTasks);
            WriteOutput("Finished.\n");
            return _finalDirectories.ToList();
        }

        private void SetupReaders()
        {
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                var worker = new DirectorySizeWorker(_inputDirectories, _localSizeDirectories, _finalDirectories);
                _readers.Add(worker);
            }
        }

        private void WriteOutput(string text)
        {
            if (_log != null)
            {
                _log.WriteConcurrent(text);
            }
        }
    }
}
