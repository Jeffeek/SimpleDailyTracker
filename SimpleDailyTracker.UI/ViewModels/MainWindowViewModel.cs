using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using OxyPlot;
using OxyPlot.Series;
using Prism.Commands;
using Prism.Mvvm;
using SimpleDailyTracker.Application.Enums;
using SimpleDailyTracker.Application.Interfaces;
using SimpleDailyTracker.Application.Models;
using SimpleDailyTracker.Application.Services.Export;
using SimpleDailyTracker.Application.Services.Import;
using SimpleDailyTracker.Application.Settings;
using SimpleDailyTracker.UI.Models;

namespace SimpleDailyTracker.UI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly UserInformationCollector _userInformationCollector;
        private readonly DirectorySettings _directorySettings;
        private readonly FileSearch _fileSearch;
        private readonly IUploadFileManager _uploadFileManager;
        private readonly ExportManager _exportManager;
        private readonly IMapper _mapper;

        private ObservableCollection<UserViewModel> _users;
        private UserViewModel _selectedUser;

        private bool _isUpdating = false;
        private bool _isExporting = false;
        private DelegateCommand _updateCommand;
        private DelegateCommand _uploadCommand;
        private DelegateCommand _exportCommand;

        private ExportType _exportType;

        private Series _highestSeries;

        public MainWindowViewModel(UserInformationCollector userInformationCollector,
                                   Configuration configuration,
                                   WindowsUploadFileManager windowsUploadFileManager,
                                   ExportManager exportManager,
                                   IMapper mapper)
        {
            _userInformationCollector = userInformationCollector ?? throw new ArgumentNullException(nameof(userInformationCollector));
            _directorySettings = configuration.DirectorySettings ?? throw new ArgumentNullException(nameof(configuration.DirectorySettings));
            _fileSearch = configuration.FileSearch ?? throw new ArgumentNullException(nameof(configuration.FileSearch));
            _uploadFileManager = windowsUploadFileManager ?? throw new ArgumentNullException(nameof(windowsUploadFileManager));
            _exportManager = exportManager ?? throw new ArgumentNullException(nameof(exportManager));
            _mapper = mapper;
        }

        public Series HighestSeries
        {
            get => _highestSeries;
            set => SetProperty(ref _highestSeries, value);
        }

        public ObservableCollection<UserViewModel> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public UserViewModel SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (!SetProperty(ref _selectedUser, value))
                    return;

                ExportCommand.RaiseCanExecuteChanged();
                UpdateCommand.RaiseCanExecuteChanged();
                UploadCommand.RaiseCanExecuteChanged();
                UpdateHighestPoint();
            }
        }

        private bool IsExporting
        {
            get => _isExporting;
            set
            {
                if (!SetProperty(ref _isExporting, value))
                    return;

                ExportCommand.RaiseCanExecuteChanged();
            }
        }

        private bool IsUpdating
        {
            get => _isUpdating;
            set
            {
                if (!SetProperty(ref _isUpdating, value))
                    return;

                ExportCommand.RaiseCanExecuteChanged();
                UpdateCommand.RaiseCanExecuteChanged();
            }
        }

        public ExportType ExportType
        {
            get => _exportType;
            set => SetProperty(ref _exportType, value);
        }

        public DelegateCommand UpdateCommand =>
            _updateCommand ??= new DelegateCommand(async () => await UpdateUsersAsync(GetImportFiles()),
                                                   CanUpdateExecute).ObservesProperty(() => IsUpdating);

        private bool CanUpdateExecute() => !IsUpdating && !IsExporting;

        private async Task UpdateUsersAsync(IEnumerable<string> files)
        {
            IsUpdating = true;
            var parseResult = await _userInformationCollector.CollectDataAsync(files);

            Users = new ObservableCollection<UserViewModel>(_mapper.Map<IEnumerable<UserViewModel>>(parseResult));
            SelectedUser = Users.FirstOrDefault();
            IsUpdating = false;
        }

        public DelegateCommand UploadCommand =>
            _uploadCommand ??= new DelegateCommand(async () => await UploadFilesAsync(),
                                                   CanUploadExecute).ObservesProperty(() => IsUpdating);

        private bool CanUploadExecute() => !IsUpdating;

        private async Task UploadFilesAsync()
        {
            var files = _uploadFileManager.SelectFiles();

            if (files.Length == 0)
                return;

            await UpdateUsersAsync(files);
        }

        public DelegateCommand ExportCommand =>
            _exportCommand ??= new DelegateCommand(async () => await ExportUserInformationAsync(),
                                                   CanExportExecute).ObservesProperty(() => IsExporting);

        private bool CanExportExecute() => SelectedUser != null && !IsExporting && !IsUpdating;

        private async Task ExportUserInformationAsync()
        {
            IsExporting = true;
            await _exportManager.ExportAsync(_mapper.Map<FullUserInformation>(SelectedUser), ExportType);
            IsExporting = false;
        }

        private void UpdateHighestPoint()
        {
            if (SelectedUser == null)
                return;

            HighestSeries = null;

            var innerMaxPoint = SelectedUser.StepsByDay.Max(z => z.Value);

            var maxPoints = SelectedUser.StepsByDay
                                        .Where(x => x.Value == innerMaxPoint)
                                        .ToArray();

            if (!maxPoints.Any())
                return;

            if (maxPoints.Length > 1)
                return;

            HighestSeries = new LineSeries()
                            {
                                Points =
                                {
                                    new DataPoint(maxPoints[0].Key,
                                                  maxPoints[0].Value)
                                }
                            };
        }

        private IEnumerable<string> GetImportFiles() =>
            new DirectoryInfo(_directorySettings.ImportDirectory).GetFiles(_fileSearch.SearchPattern, SearchOption.TopDirectoryOnly)
                                                                 .Where(x => x.Extension == _fileSearch.Extension)
                                                                 .Select(x => x.FullName);
    }
}