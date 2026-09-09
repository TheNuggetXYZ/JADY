using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using JADY.Core.Models;

namespace JADY.Services;

public interface IFileAttachmentService
{
    public Task<IReadOnlyList<IStorageFile>> SelectAttachments(IStorageProvider storageProvider);
    public Task<List<FileAttachment>> CreateAttachments(IReadOnlyList<IStorageFile> rawAttachments);
}