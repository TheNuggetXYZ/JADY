using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JADY.Core.Helpers;
using JADY.Core.Models;
using JADY.ViewModels;

namespace JADY.UI.Views.Controls;

public partial class FileAttachmentControl : UserControl
{
    public FileAttachmentControl()
    {
        InitializeComponent();
        
            
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        if (DataContext is FileAttachmentViewModel context)
            FileSize.Text = context.FileSizeBytes != null ? BytesUnitConverter.BytesToHuman((ulong)context.FileSizeBytes) : "";
    }
}