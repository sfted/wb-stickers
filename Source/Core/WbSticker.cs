using BarcodeLib;
using Microsoft.Win32;
using PdfSharp.Pdf;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace WbStickers.Source.Core;

public class WbSticker
{
    public WbSticker(string template)
    {
        html = template;
    }

    private string code = "0";
    private string html;

    public WbSticker AddBarcode(string code, int width, int height)
    {
        var barcode = new Barcode();

        var img = barcode.Encode(
            TYPE.EAN13, code, Color.Black, Color.White, width, height);

        using (var stream = new MemoryStream())
        {
            img.Save(stream, ImageFormat.Jpeg);
            var bytes = stream.ToArray();
            var base64 = Convert.ToBase64String(bytes);
            html = html.Replace("{barcode_image}", base64);
            html = html.Replace("{barcode_numeric}", code);
            this.code = code;
        }

        return this;
    }

    public WbSticker AddCompany(string company)
    {
        html = html.Replace("{company}", company);
        return this;
    }

    public WbSticker AddField(string field)
    {
        if(string.IsNullOrEmpty(field))
            return this;

        html = html.Replace(
            "{additional_field}",
            $"<div>{field}</div>\n{{additional_field}}");

        return this;
    }

    public void SaveAsPdf()
    {
        using var stream = new MemoryStream();
        var pdf = PdfGenerator.GeneratePdf(ToString(), new PdfGenerateConfig
        {
            ManualPageSize = PdfGenerateConfig.MilimitersToUnits(80, 60),
            MarginBottom = 4,
            MarginLeft = 4,
            MarginRight = 4,
            MarginTop = 4
        });

        pdf.Save(stream);

        var dialog = new SaveFileDialog()
        {
            FileName = $"sticker_{code}",
            DefaultExt = ".pdf",
            Filter = "PDF Documents (.pdf)|*.pdf"
        };

        var bytes = stream.ToArray();

        if (dialog.ShowDialog() == true)
            File.WriteAllBytes(dialog.FileName, bytes);
    }

    public override string ToString()
    {
        html = html.Replace("{additional_field}", string.Empty);
        return FixASCIICharacters(html);
    }

    static string FixASCIICharacters(string html)
    {
        var builder = new StringBuilder();
        char[] chars = html.ToCharArray();

        foreach (char ch in chars)
        {
            var chInt = Convert.ToInt32(ch);
            if (chInt > 127)
                builder.Append($"&#{chInt};");
            else
                builder.Append(ch);
        }

        return builder.ToString();
    }
}