using JADY.Core.Models;
using JADY.Services;
using JADY.ViewModels;

namespace JADY.Factories;

public class DiaryEntryViewModelFactory(ISaveService saveService, IWindowService windowService) : IDiaryEntryViewModelFactory
{
    public DiaryEntryViewModel Create(DiaryEntry diaryEntry, DiaryViewModel diaryViewModel)
    {
        return new DiaryEntryViewModel(diaryEntry, diaryViewModel, saveService, windowService);
    }
}