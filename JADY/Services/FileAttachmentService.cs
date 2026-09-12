using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using JADY.Core.Data;
using JADY.Core.Models;
using Microsoft.Extensions.Logging;

namespace JADY.Services;

public class FileAttachmentService(ILogger<FileAttachmentService> logger) : IFileAttachmentService
{
    public void SaveAttachments(List<FileAttachment> fileAttachments)
    {
        if (!Path.Exists(AppDirectories.FileAttachmentDirectory))
            Directory.CreateDirectory(AppDirectories.FileAttachmentDirectory);

        foreach (var attach in fileAttachments)
        {
            File.Copy(attach.OriginalFilePath, Path.Combine(AppDirectories.FileAttachmentDirectory, attach.GuidString));
        }
    }

    public async Task<List<FileAttachment>> SelectAttachments(IStorageProvider storageProvider)
    {
        IReadOnlyList<IStorageFile> rawAttachments = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions() { AllowMultiple = true });
        List<FileAttachment> fileAttachments = new();
        
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
            
            fileAttachments.Add(attachment);
        }
        
        return fileAttachments;
    }
}