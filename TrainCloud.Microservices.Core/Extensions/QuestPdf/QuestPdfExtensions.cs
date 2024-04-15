using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace TrainCloud.Microservices.Core.Extensions.QuestPdf;

public static class QuestPdfExtensions
{
    public static IServiceCollection AddTrainCloudQuestPdf(this IServiceCollection services)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = false;
        QuestPDF.Settings.EnableDebugging = true;
        QuestPDF.Settings.UseEnvironmentFonts = false;

        var rootFolder = Directory.GetCurrentDirectory();
        var fontFolder = Path.Combine(rootFolder, "wwwroot", "fonts");

        var fontFile = Path.Combine(fontFolder, "Lato-Black.ttf");
        FontManager.RegisterFont(File.OpenRead(fontFile));

        fontFile = Path.Combine(fontFolder, "Lato-BlackItalic.ttf");
        FontManager.RegisterFont(File.OpenRead(fontFile));

        fontFile = Path.Combine(fontFolder, "Lato-Bold.ttf");
        FontManager.RegisterFont(File.OpenRead(fontFile));

        fontFile = Path.Combine(fontFolder, "Lato-BoldItalic.ttf");
        FontManager.RegisterFont(File.OpenRead(fontFile));

        fontFile = Path.Combine(fontFolder, "Lato-Italic.ttf");
        FontManager.RegisterFont(File.OpenRead(fontFile));

        fontFile = Path.Combine(fontFolder, "Lato-Light.ttf");
        FontManager.RegisterFont(File.OpenRead(fontFile));

        fontFile = Path.Combine(fontFolder, "Lato-LightItalic.ttf");
        FontManager.RegisterFont(File.OpenRead(fontFile));

        fontFile = Path.Combine(fontFolder, "Lato-Regular.ttf");
        FontManager.RegisterFont(File.OpenRead(fontFile));

        fontFile = Path.Combine(fontFolder, "Lato-Thin.ttf");
        FontManager.RegisterFont(File.OpenRead(fontFile));

        fontFile = Path.Combine(fontFolder, "Lato-ThinItalic.ttf");
        FontManager.RegisterFont(File.OpenRead(fontFile));

        return services;
    }
}