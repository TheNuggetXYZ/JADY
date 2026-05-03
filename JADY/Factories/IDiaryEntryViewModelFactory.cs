using JADY.Core.Models;
using JADY.Services;
using JADY.ViewModels;

namespace JADY.Factories;

public interface IDiaryEntryViewModelFactory
{
    DiaryEntryViewModel Create(DiaryEntry diaryEntry, DiaryViewModel diaryViewModel);
}