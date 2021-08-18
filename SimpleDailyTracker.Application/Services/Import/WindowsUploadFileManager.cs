using System;
using Microsoft.Win32;
using SimpleDailyTracker.Application.Interfaces;
using SimpleDailyTracker.Application.Settings;

namespace SimpleDailyTracker.Application.Services.Import
{
    public class WindowsUploadFileManager : IUploadFileManager
    {
        private readonly OpenFileDialog _internalDialog;

        public WindowsUploadFileManager(FileSearch fileSearchSettings) =>
            _internalDialog = new OpenFileDialog()
                              {
                                  Multiselect = true,
                                  ValidateNames = true,
                                  Filter = $"Daily files ({fileSearchSettings.SearchPattern})|{fileSearchSettings.SearchPattern}"
                              };

        public string[] SelectFiles() =>
            _internalDialog.ShowDialog()
                           .GetValueOrDefault()
                ? _internalDialog.FileNames
                : Array.Empty<string>();
    }
}