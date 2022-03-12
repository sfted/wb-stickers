using System;
using System.Collections.Generic;
using WbStickers.Source.Core;
using WbStickers.Source.Helpers;

namespace WbStickers.Source.Models;

public class Sticker : ViewModelBase
{
    public Sticker(string template)
    {
        this.template = template;

        Fields = new List<Field>
        {
            new Field() { IsEditable = false, Key = "Штрихкод" },
            new Field() { IsEditable = false, Key = "Продавец" }
        };
    }

    // fields
    string template;
    string preview = string.Empty;

    // properties
    public List<Field> Fields { get; set; }

    public string HtmlPreview
    {
        get => preview;
        set => SetProperty(ref preview, value);
    }

    // methods
    public void MakeSureTemplateIsNotNull(string template)
    {
        this.template = template;
    }

    public void RefreshPreview()
    {
        HtmlPreview = GenerateWbSticker().ToString();
    }

    public WbSticker GenerateWbSticker()
    {
        var code = Fields[0].Value;

        if (code.Length != 13)
            throw new ArgumentException(
                "Длина кода должна быть 13 символов.", nameof(code));

        //if (string.IsNullOrEmpty(template))
        //    return;

        var sticker = new WbSticker(template)
            .AddBarcode(Fields[0].Value, 300, 50)
            .AddCompany(Fields[1].Value);

        var neededFields = Fields.GetRange(2, Fields.Count - 2);
        foreach (var field in neededFields)
            sticker.AddField($"{field.Key}: {field.Value}");

        return sticker;
    }

    public override string ToString()
    {
        return Fields[0].Value;
    }
}