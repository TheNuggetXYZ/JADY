using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Data;
using CommunityToolkit.Mvvm.Messaging;
using JADY.Core;
using JADY.Core.Models;
using JADY.Factories;
using JADY.ViewModels;

namespace JADY.Services;

public class DiaryService : IDiaryService
{
    private readonly ISaveService _saveService;
    private readonly IWindowService _windowService;
    private readonly IDiaryViewModelFactory _diaryViewModelFactory;
    
    public ObservableCollection<DiaryViewModel> Diaries { get; } = [];

    public DiaryService(ISaveService saveService, IWindowService windowService, IDiaryViewModelFactory diaryViewModelFactory)
    {
        _saveService = saveService;
        _windowService = windowService;
        _diaryViewModelFactory = diaryViewModelFactory;

        WeakReferenceMessenger.Default.Register<Messages.PerformSave>(this, (_, _) =>
        {
            SaveDiaries();
        });
    }

    public void SaveDiaries()
    {
        _saveService.Save(Diaries.Select(d => d.GetModel()).ToArray());
    }

    public void LoadDiaries(bool loadSave)
    {
        if (loadSave)
            _saveService.LoadSave();

        var loadedDiaries = _saveService.SaveData.Diaries.Select(model => _diaryViewModelFactory.Create(model)).ToList();
        
        RelinkEntries(loadedDiaries);
        
        Diaries.Clear();
        foreach (var vm in loadedDiaries)
            Diaries.Add(vm);
    }

    private static void RelinkEntries(List<DiaryViewModel> diaries)
    {
        foreach (var diary in diaries)
        {
            // Load cache
            var entryCache = diary.Entries.ToDictionary(x => x.EntryGuid);

            // Assign parent Guid and reference
            foreach (var diaryEntry in entryCache.Values)
            {
                if (diaryEntry.ParentEntryGuid is { } parentEntryGuid)
                    diaryEntry.AssignParentEntry(entryCache[parentEntryGuid]);
            }
        }
    }

    public void AddDiary(Diary model)
    {
        Diaries.Add(_diaryViewModelFactory.Create(model));
        
        WeakReferenceMessenger.Default.Send(new Messages.UnsavedChangeCreated());
    }

    public void AddEntry(DiaryEntry model, int diaryIndex)
    {
        Diaries[diaryIndex].AddEntry(model);
    }

    public async Task RemoveDiary(DiaryViewModel item)
    {
        var pickedYes = await _windowService.OpenYesNoMessageBox(_windowService.GetMainWindow(), "Are you sure you want to remove this diary?", "Remove diary?");
        if (!pickedYes.HasValue || pickedYes.Value == false) return;
            
        Diaries.Remove(item);
        WeakReferenceMessenger.Default.Send(new Messages.UnsavedChangeCreated());
    }
}