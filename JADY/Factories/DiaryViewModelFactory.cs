using System;
using JADY.Core.Models;
using JADY.Services;
using JADY.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace JADY.Factories;

public class DiaryViewModelFactory(IServiceProvider serviceProvider, IDiaryEntryViewModelFactory diaryEntryViewModelFactory, IWindowService windowService) : IDiaryViewModelFactory
{
    public DiaryViewModel Create(Diary diary)
    {
        // resolve circular dependency
        var diaryService = serviceProvider.GetRequiredService<IDiaryService>();
        
        return new DiaryViewModel(diary, diaryEntryViewModelFactory, windowService, diaryService);
    }
}