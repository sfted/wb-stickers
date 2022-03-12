using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using WbStickers.Source.Helpers;
using WbStickers.Source.Models;

namespace WbStickers.Source.ViewModels;

public class StickerViewModel : ViewModelBase
{
    public StickerViewModel(Sticker sticker)
    {
        Model = sticker;
        Fields = new ObservableCollection<FieldViewModel>();

        foreach (var field in sticker.Fields)
            AddField(field);

        DeleteCommand = new Command(Delete);
        SaveAndRefreshCommand = new Command(SaveAndRefresh);
        AddFieldCommand = new Command(AddField);
        SaveAsPdfCommand = new Command(SaveAsPdf);

        UpdateFieldsString();

        Model.PropertyChanged += OnModelPropertyChanged;
    }

    // fields
    string fieldsString = string.Empty;

    // events
    public event EventHandler? Deleting;
    public event EventHandler? Saving;

    // commands
    public Command DeleteCommand { get; private set; }
    public Command SaveAndRefreshCommand { get; private set; }
    public Command AddFieldCommand { get; private set; }
    public Command SaveAsPdfCommand { get; private set; }

    // properties
    public Sticker Model { get; private set; }

    public string FieldsString
    {
        get => fieldsString;
        set => SetProperty(ref fieldsString, value);
    }

    public ObservableCollection<FieldViewModel> Fields { get; private set; }

    public string HtmlPreview
    {
        get => Model.HtmlPreview;
        set => Model.HtmlPreview = value;
    }

    // methods
    void Delete() => Deleting?.Invoke(this, EventArgs.Empty);

    void SaveAndRefresh()
    {
        try
        {
            var fieldModels = new List<Field>();
            foreach (var vm in Fields)
                fieldModels.Add(vm.Model);
            Model.Fields = fieldModels;
            Model.RefreshPreview();

            UpdateFieldsString();
            Saving?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    void UpdateFieldsString()
    {
        var neededFields = Fields
            .Where(f => !string.IsNullOrEmpty(f.Value))
            .Select(f => f.Value);

        FieldsString = string.Join(", ", neededFields);
    }

    void AddField() => AddField(new Field());

    void AddField(Field field)
    {
        var vm = new FieldViewModel(field);
        vm.Deleting += OnFieldDeleting;
        vm.PropertyChanged += OnModelPropertyChanged;
        Fields.Add(vm);
    }

    void SaveAsPdf()
    {
        SaveAndRefresh();
        Model.GenerateWbSticker().SaveAsPdf();
    }

    void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != null)
            NotifyPropertyChanged(e.PropertyName);
    }

    void OnFieldDeleting(object? sender, EventArgs e)
    {
        if (sender is FieldViewModel vm)
        {
            vm.Deleting -= OnFieldDeleting;
            Fields.Remove(vm);
        }
    }
}