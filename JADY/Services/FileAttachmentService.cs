using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using JADY.Core.Data;
using JADY.Core.Models;
using JADY.ViewModels;
using Microsoft.Extensions.Logging;

namespace JADY.Services;

public class FileAttachmentService(ILogger<FileAttachmentService> logger) : IFileAttachmentService
{
    public void SaveAttachments(ObservableCollection<FileAttachmentViewModel> fileAttachments)
    {
        if (!Path.Exists(AppDirectories.FileAttachmentDirectory))
            Directory.CreateDirectory(AppDirectories.FileAttachmentDirectory);

        foreach (var attach in fileAttachments)
        {
            File.Copy(attach.OriginalFilePath, Path.Combine(AppDirectories.FileAttachmentDirectory, attach.GuidString));
        }
    }

    public async Task<ObservableCollection<FileAttachmentViewModel>> SelectAttachments(IStorageProvider storageProvider)
    {
        IReadOnlyList<IStorageFile> rawAttachments = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions() { AllowMultiple = true });
        ObservableCollection<FileAttachmentViewModel> fileAttachments = new();
        
        foreach (var rawAttachment in rawAttachments)
        {
            var rawProperties = await rawAttachment.GetBasicPropertiesAsync();

            var attachment = new FileAttachment()
            {
                DateAdded = DateTimeOffset.Now,
                FileSizeBytes = rawProperties.Size,
                OriginalFileName = rawAttachment.Name,
                OriginalFilePath = rawAttachment.Path.LocalPath,
            };
            
            fileAttachments.Add(new FileAttachmentViewModel(attachment));
        }
        
        return fileAttachments;
    }
}