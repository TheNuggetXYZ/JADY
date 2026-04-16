using JADY.ViewModels;

namespace JADY.Backend;

public class Messages
{
    public record RemoveDiaryMessage(DiaryViewModel Diary);
    
    public record RemoveDiaryEntryMessage(DiaryEntryViewModel Entry, DiaryViewModel Owner);
    public record EditDiaryEntryMessage(DiaryEntryViewModel Entry, DiaryViewModel Owner);
    
    public record RemoveSubEntryMessage(DiarySubEntryViewModel SubEntry, DiaryEntryViewModel Owner);
}