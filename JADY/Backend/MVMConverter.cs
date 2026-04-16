using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JADY.Models;
using JADY.ViewModels;

namespace JADY.Backend;

/// <summary>
/// Model to view model converter
/// </summary>
public static class MVMConverter
{
    public static ObservableCollection<DiaryEntryViewModel> ConvertModels(IEnumerable<DiaryEntry> models, DiaryViewModel owner) => 
        new(models.Select(x => new DiaryEntryViewModel(x, owner)));
    
    public static List<DiaryEntry> ConvertViewModels(ObservableCollection<DiaryEntryViewModel> viewModels) => 
        new(viewModels.Select(x => { x.Dispose(); return x.GetModel(); }));
}