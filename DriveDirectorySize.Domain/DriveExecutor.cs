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
        private BlockingCollection<string> _inputDirectories;
        private ConcurrentDictionary<string, DirectorySizeData> _directories;
        private List<DirectorySizeWorker> _readers;
        private ILog _log;

        private int _workerThreads = Environment.ProcessorCount - 1;

        public DriveExecutor(string drive, ILog log)
        {
            _drive = drive;
            _log = log;
            _inputDirectories = new BlockingCollection<string>();
            _readers = new List<DirectorySizeWorker>();

            _directories = new ConcurrentDictionary<string, DirectorySizeData>();
            
        }

        public IEnumerable<DirectorySizeData> Run()
        {
            SetupReaders();
            Task[] workerTasks = new Task[_readers.Count];
            int taskId = 0;
            _readers.ForEach(r =>
            {
                Task task = Task.Run(() => r.ProcessSizes());
                workerTasks[taskId++] = task;
            });

            WriteOutput("Processing local sizes.");
            var map = new DirectoryMap(_drive, _inputDirectories, _log);
            map.ReadAll();

            Task.WaitAll(workerTasks);
            WriteOutput("Finished.\n");
            return _directories.Select(x => x.Value).ToList();

        }

        private void SetupReaders()
        {
            for (int i = 0; i < _workerThreads; i++)
            {
                var worker = new DirectorySizeWorker(_inputDirectories, _directories);
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
