using JADY.Core.Models;
using JADY.Services;
using JADY.ViewModels;

namespace JADY.Factories;

public class DiaryViewModelFactory(IDiaryEntryViewModelFactory diaryEntryViewModelFactory, IWindowService windowService, IDiaryService diaryService) : IDiaryViewModelFactory
{
    public DiaryViewModel Create(Diary diary)
    {
        return new DiaryViewModel(diary, diaryEntryViewModelFactory, windowService, diaryService);
    }
}