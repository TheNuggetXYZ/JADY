using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using JADY.Core.Models;

namespace JADY.Services;

public interface IFileAttachmentService
{
    public Task<List<FileAttachment>> SelectAttachments(IStorageProvider storageProvider);
    public void SaveAttachments(List<FileAttachment> fileAttachments);
}