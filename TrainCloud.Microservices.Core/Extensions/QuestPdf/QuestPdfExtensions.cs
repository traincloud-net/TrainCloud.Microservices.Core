using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;

namespace TrainCloud.Microservices.Core.Extensions.QuestPdf;

/// <summary>
/// Global settings for all QuestPdf using services
/// </summary>
public static class QuestPdfExtensions
{
    public static IServiceCollection AddTrainCloudQuestPdf(this IServiceCollection services)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = false;
        QuestPDF.Settings.EnableDebugging = true;
        QuestPDF.Settings.UseEnvironmentFonts = false;

        return services;
    }
}