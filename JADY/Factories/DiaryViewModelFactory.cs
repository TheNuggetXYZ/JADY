using JADY.Core.Models;
using JADY.ViewModels;

namespace JADY.Factories;

public class DiaryViewModelFactory(IDiaryEntryViewModelFactory diaryEntryViewModelFactory) : IDiaryViewModelFactory
{
    public DiaryViewModel Create(Diary diary, MainWindowViewModel mainWindowViewModel)
    {
        return new DiaryViewModel(diary, mainWindowViewModel, diaryEntryViewModelFactory);
    }
}