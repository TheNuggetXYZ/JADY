using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using JADY.Core;
using JADY.Core.Attributes;

namespace JADY.ViewModels;

public abstract class SaveDependentViewModel : ViewModelBase
{
    private static readonly Dictionary<Type, string[]> Cache = new();

    protected SaveDependentViewModel()
    {
        WeakReferenceMessenger.Default.Register<Messages.JadySaveChanged>(this, (r, m) => OnSaveChanged());
    }

    private void OnSaveChanged()
    {
        var type = GetType();
        if (!Cache.TryGetValue(type, out var props))
        {
            props = type.GetProperties()
                .Where(p => p.IsDefined(typeof(SaveDependentAttribute), true))
                .Select(p => p.Name)
                .ToArray();
            Cache[type] = props;
        }

        foreach (var prop in props)
            OnPropertyChanged(prop);
    }
}