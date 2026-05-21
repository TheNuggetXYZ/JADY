using System.Collections.ObjectModel;
using System.Threading.Tasks;
using JADY.Core.Models;
using JADY.ViewModels;

namespace JADY.Services;

public interface IDiaryService
{
    ObservableCollection<DiaryViewModel> Diaries { get; }
    
    void SaveDiaries();
    void LoadDiaries(bool loadSave);
    
    void AddDiary(Diary model);
    void AddEntry(DiaryEntry model, int diaryIndex);
    
    Task RemoveDiary(DiaryViewModel item);
}