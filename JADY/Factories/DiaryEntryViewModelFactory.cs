using JADY.Core.Models;
using JADY.Services;
using JADY.ViewModels;

namespace JADY.Factories;

public class DiaryEntryViewModelFactory(ISaveService saveService, IWindowService windowService) : IDiaryEntryViewModelFactory
{
    public DiaryEntryViewModel Create(DiaryEntry diaryEntry, DiaryViewModel parentDiary, DiaryEntryViewModel? parentEntry = null)
    {
        return new DiaryEntryViewModel(diaryEntry, parentDiary, parentEntry, saveService, windowService);
    }
}