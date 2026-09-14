using System;
using CommunityToolkit.Mvvm.ComponentModel;
using JADY.Core.Models;

namespace JADY.ViewModels;

public partial class FileAttachmentViewModel : ViewModelBase
{
    [ObservableProperty] private string? _originalFileName;

    [ObservableProperty] private string? _originalFilePath;

    [ObservableProperty] private ulong? _fileSizeBytes;

    [ObservableProperty] private DateTimeOffset _dateAdded;
    
    [ObservableProperty] private string _guidString;

    public FileAttachmentViewModel(FileAttachment model)
    {
        OriginalFileName = model.OriginalFileName;
        OriginalFilePath = model.OriginalFilePath;
        FileSizeBytes = model.FileSizeBytes;
        DateAdded = model.DateAdded;
        GuidString = model.GuidString;
    }

    public FileAttachment GetModel()
    {
        return new FileAttachment()
        {
            OriginalFileName = OriginalFileName ?? "",
            OriginalFilePath = OriginalFilePath ?? "",
            DateAdded = DateAdded,
            FileSizeBytes = FileSizeBytes,
            GuidString = GuidString
        };
    }
}