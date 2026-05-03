using JADY.Core.Models;
using JADY.Services;
using JADY.ViewModels;

namespace JADY.Factories;

public class DiaryViewModelFactory(IDiaryEntryViewModelFactory diaryEntryViewModelFactory, IWindowService windowService) : IDiaryViewModelFactory
{
    public DiaryViewModel Create(Diary diary, MainWindowViewModel mainWindowViewModel)
    {
        return new DiaryViewModel(diary, mainWindowViewModel, diaryEntryViewModelFactory, windowService);
    }
}