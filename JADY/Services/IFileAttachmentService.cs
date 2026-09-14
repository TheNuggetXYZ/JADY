using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using JADY.Core.Models;
using JADY.ViewModels;

namespace JADY.Services;

public interface IFileAttachmentService
{
    public Task<ObservableCollection<FileAttachmentViewModel>> SelectAttachments(IStorageProvider storageProvider);
    public void SaveAttachments(ObservableCollection<FileAttachmentViewModel> fileAttachments);
}