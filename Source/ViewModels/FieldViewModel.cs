using System;
using WbStickers.Source.Helpers;
using WbStickers.Source.Models;

namespace WbStickers.Source.ViewModels;

public class FieldViewModel : ViewModelBase
{
    public FieldViewModel(Field field)
    {
        Model = field;
        DeleteCommand = new Command(Delete);
    }

    public FieldViewModel() : this(new Field()) { }

    // fields
    // ...

    // events
    public event EventHandler? Deleting;

    // commands
    public Command DeleteCommand { get; private set; }

    // properties
    public Field Model { get; private set; }

    public string Key
    {
        get => Model.Key;
        set
        {
            Model.Key = value;
            NotifyPropertyChanged();
        }
    }

    public string Value
    {
        get => Model.Value;
        set
        {
            Model.Value = value;
            NotifyPropertyChanged();
        }
    }

    public bool IsEditable
    {
        get => Model.IsEditable;
        set
        {
            Model.IsEditable = value;
            NotifyPropertyChanged();
        }
    }

    // methods
    void Delete() => Deleting?.Invoke(this, EventArgs.Empty);
}