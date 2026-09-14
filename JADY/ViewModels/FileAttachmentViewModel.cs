using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JADY.Core.Models;

namespace JADY.ViewModels;

public partial class FileAttachmentViewModel : ViewModelBase
{
    [ObservableProperty] private string? _originalFileName;

    [ObservableProperty] private string? _originalFilePath;

    [ObservableProperty] private ulong? _fileSizeBytes;

    [ObservableProperty] private DateTimeOffset _dateAdded;
    
    [ObservableProperty] private string _guidString;
    
    private Action<FileAttachmentViewModel>? _editWindowOnRemove;

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

    public void SetEditWindowOnRemove(Action<FileAttachmentViewModel> action)
    {
        _editWindowOnRemove = action;
    }

    [RelayCommand]
    private void RemoveFromEditWindow()
    {
        _editWindowOnRemove?.Invoke(this);
    }
}