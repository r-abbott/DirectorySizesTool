using DriveDirectorySize.Domain;
using DriveDirectorySize.Domain.Contracts;
using DriveDirectorySize.Domain.ValueObjects;
using DriveDirectorySize.UI.CommandHandlers;
using DriveDirectorySize.UI.Contracts;
using DriveDirectorySize.UI.Infrastructure;
using DriveDirectorySize.UI.ViewModels;
using DriveDirectorySize.UI.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DriveDirectorySize.UI.Controllers
{
    public class DirectoryController
    {
        private readonly ISizeConversion _sizeConversion;
        private readonly ILog _log;
        private readonly DirectoryView _view;
        private readonly ICommandHandler _driveReaderCommandHandler;

        private IDrive _drive;
        private IDriveReader _reader;
        private ICommandHandler _currentCommandHandler;

        public DirectoryController()
            : this(new ProcessLogger(), new ByteConversion())
        {
        }

        public DirectoryController(ILog log, ISizeConversion sizeConversion)
        {
            _log = log;
            _sizeConversion = sizeConversion;
            // TODO - have one handler and just fill it with necessary commands
            _driveReaderCommandHandler = new DriveReaderCommandHandler(this);
            _currentCommandHandler = new DriveLetterCommandHandler(this);
            _view = new DirectoryView();
            UIConsole.SetTitle("Directory Size Tool");
        }

        public void Handle()
        {
            _view.Display(new IntroViewModel(), _currentCommandHandler);
        }

        public IEnumerable<string> AvailableDrives()
        {
            return Drive.AvailableDrives();
        }

        public void ReadDrive(char driveLetter)
        {
            try
            {
                _drive = new Drive(driveLetter, _log);
                _reader = _drive.ReadFromStorage();
                if (_reader == null)
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    _reader = _drive.Read();
                    stopwatch.Stop();
                }
                _currentCommandHandler = _driveReaderCommandHandler;
                DisplayCurrentDirectory();
            }
            catch(InvalidOperationException ex)
            {
                DisplayError(ex.Message);
            }
        }

        public void ChangeDirectory(string directoryName)
        {
            if(_reader == null)
            {
                DisplayError("Read a drive first!");
                return;
            }

            var directory = _reader.ChangeDirectory(directoryName);

            if (directory == null)
            {
                DisplayError("Directory not found.");
            }
            else
            {
                DisplayCurrentDirectory();
            }
        }

        public void DiscoverLargestDirectories(double percentThreshold)
        {
            var result = _reader.FindLargestDirectories(percentThreshold);
            var viewModel = new DirectoryPathsViewModel(result, _sizeConversion);
            View(viewModel);
        }

        public void DirectoriesLargerThan(Size size)
        {
            var result = _reader.FindDirectoriesLargerThan(size);
            var viewModel = new DirectoryPathsViewModel(result, _sizeConversion);
            View(viewModel);
        }

        public string CurrentDirectoryPath
        {
            get
            {
                if(_reader == null)
                {
                    DisplayError("Read a drive first!");
                }
                return $"{string.Join(@"\", _reader.CurrentDirectory.Path)}";
            }
        }

        public void Quit()
        {
            Environment.Exit(0);
        }

        private void DisplayCurrentDirectory()
        {
            View(new DirectoryWithSubDirectoriesViewModel(_reader.CurrentDirectory, _reader.CurrentSubDirectories, _sizeConversion));
        }

        private void DisplayError(string text)
        {
            _view.Display(new ErrorViewModel(text), _currentCommandHandler);
        }

        private void View(IViewModel viewModel)
        {
            _view.Display(viewModel, _currentCommandHandler);
        }
    }
}
