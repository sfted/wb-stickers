using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using WbStickers.Source.Helpers;
using WbStickers.Source.Models;

namespace WbStickers.Source.ViewModels;

public class StickersViewModel : ViewModelBase
{
    public StickersViewModel()
    {
        template = Properties.Resources.template;
        Stickers = new ObservableCollection<StickerViewModel>();
        AddStickerCommand = new Command(AddSticker);

        if (File.Exists(STICKERS))
        {
            try
            {
                var result = JsonConvert.DeserializeObject<
                    List<Sticker>>(File.ReadAllText(STICKERS));

                if (result != null)
                {
                    foreach (var sticker in result)
                        AddSticker(sticker);
                }
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
    }

    const string STICKERS = "stickers.json";

    // fields
    readonly string template;
    StickerViewModel? selectedSticker;

    // commands
    public Command AddStickerCommand { get; private set; }

    // properties
    public ObservableCollection<StickerViewModel> Stickers { get; private set; }

    public StickerViewModel? SelectedSticker
    {
        get => selectedSticker;
        set => SetProperty(ref selectedSticker, value);
    }

    // methods
    void AddSticker()
    {
        AddSticker(new Sticker(template));
    }

    void AddSticker(Sticker sticker)
    {
        sticker.MakeSureTemplateIsNotNull(template);
        var vm = new StickerViewModel(sticker);
        vm.Deleting += OnStickerDeleting;
        vm.Saving += OnStickerSaving;
        Stickers.Add(vm);
    }

    void OnStickerDeleting(object? sender, EventArgs e)
    {
        if (sender is StickerViewModel vm)
        {
            vm.Deleting -= OnStickerDeleting;
            vm.Saving -= OnStickerSaving;
            Stickers.Remove(vm);
        }
    }

    void OnStickerSaving(object? sender, EventArgs e)
    {
        try
        {
            var json = JsonConvert.SerializeObject(Stickers.Select(s => s.Model));
            File.WriteAllText(STICKERS, json);
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
}