using JADY.Core.Models;
using JADY.ViewModels;

namespace JADY.Factories;

public interface IDiaryViewModelFactory
{
    DiaryViewModel Create(Diary diary, MainWindowViewModel mainWindowViewModel);
}