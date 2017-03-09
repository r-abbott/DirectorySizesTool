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
        private BlockingCollection<DirectorySizeData> _outputDirectories;
        private BlockingCollection<string> _inputDirectories;
        private List<DirectorySizeWorker> _readers;

        public DriveExecutor(string drive)
        {
            _drive = drive;
            _outputDirectories = new BlockingCollection<DirectorySizeData>();
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
                Task task = Task.Run(() => r.Run());
                workerTasks[taskId++] = task;
            });

            var map = new DirectoryMap(_drive, _inputDirectories);
            map.ReadAll();

            Task.WaitAll(workerTasks);
            return _outputDirectories.ToList();
        }

        private void SetupReaders()
        {
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                var worker = new DirectorySizeWorker(_inputDirectories, _outputDirectories.Add);
                _readers.Add(worker);
            }
        }
    }
}
