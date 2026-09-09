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
    public async Task<IReadOnlyList<IStorageFile>> SelectAttachments(IStorageProvider storageProvider)
    {
        return await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions() { AllowMultiple = true });
    }

    public async Task<List<FileAttachment>> CreateAttachments(IReadOnlyList<IStorageFile> rawAttachments)
    {
        var attachments = await ConvertAttachments(rawAttachments);
        CopyAttachments(attachments, rawAttachments);
        return attachments;
    }

    private void CopyAttachments(List<FileAttachment> fileAttachments, IReadOnlyList<IStorageFile> rawAttachments)
    {
        if (fileAttachments.Count != rawAttachments.Count)
            throw new InvalidOperationException("Raw and converted attachments count does not match");

        if (!Path.Exists(AppDirectories.FileAttachmentDirectory))
            Directory.CreateDirectory(AppDirectories.FileAttachmentDirectory);

        for (int i = 0; i < rawAttachments.Count; i++)
        {
            File.Copy(rawAttachments[i].Path.AbsolutePath, Path.Combine(AppDirectories.FileAttachmentDirectory, fileAttachments[i].GuidString));
        }
    }

    private async Task<List<FileAttachment>> ConvertAttachments(IReadOnlyList<IStorageFile> rawAttachments)
    {
        List<FileAttachment> fileAttachments = new();
        
        foreach (var rawAttachment in rawAttachments)
        {
            var rawProperties = await rawAttachment.GetBasicPropertiesAsync();

            var attachment = new FileAttachment()
            {
                DateAdded = DateTimeOffset.Now,
                FileSizeBytes = rawProperties.Size,
                OriginalFileName = rawAttachment.Name,
            };
            
            fileAttachments.Add(attachment);
        }
        
        return fileAttachments;
    }
}