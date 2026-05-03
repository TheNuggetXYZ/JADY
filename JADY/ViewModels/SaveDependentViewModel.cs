using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using JADY.Core;
using JADY.Core.Attributes;

namespace JADY.ViewModels;

public abstract class SaveDependentViewModel : ViewModelBase
{
    private static readonly Dictionary<Type, string[]> _cache = new();

    protected SaveDependentViewModel()
    {
        WeakReferenceMessenger.Default.Register<Messages.JadySaveChanged>(this, (r, m) => OnSaveChanged());
    }

    private void OnSaveChanged()
    {
        var type = GetType();
        if (!_cache.TryGetValue(type, out var props))
        {
            props = type.GetProperties()
                .Where(p => p.IsDefined(typeof(SaveDependentAttribute), true))
                .Select(p => p.Name)
                .ToArray();
            _cache[type] = props;
        }

        foreach (var prop in props)
            OnPropertyChanged(prop);
    }
}