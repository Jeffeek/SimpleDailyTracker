using System;
using Microsoft.Win32;

namespace SimpleDailyTracker.Application.Services.Import
{
    public class UploadFileManager
    {
        private readonly OpenFileDialog _internalDialog;

        public UploadFileManager() =>
            _internalDialog = new OpenFileDialog()
                              {
                                  Multiselect = true,
                                  ValidateNames = true,
                                  Filter = "Daily files (day*.json)|day*.json"
                              };

        public string[] SelectFiles() =>
            _internalDialog.ShowDialog()
                           .GetValueOrDefault()
                ? _internalDialog.FileNames
                : Array.Empty<string>();
    }
}