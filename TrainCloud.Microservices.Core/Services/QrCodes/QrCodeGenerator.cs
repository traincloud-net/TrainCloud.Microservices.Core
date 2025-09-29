using QRCoder;
using SkiaSharp;

namespace TrainCloud.Microservices.Core.Services.QrCodes;

public sealed class QrCodeGenerator : IQrCodeGenerator
{
    public System.Drawing.Color DarkColor { get; } = System.Drawing.Color.Black;

    public System.Drawing.Color LightColor { get; } = System.Drawing.Color.White;

    public int PixelsPerModule { get; } = 25;

    public int IconSizePercent { get; } = 20;

    public bool DrawQuietZones { get; } = true;

    public byte[] GenerateQrCodePng(string codeData)
    {
        QRCodeGenerator.ECCLevel eccLevel = QRCodeGenerator.ECCLevel.H;

        using QRCodeGenerator qrGenerator = new();
        using QRCodeData qrCodeData = qrGenerator.CreateQrCode(codeData, eccLevel);
        using PngByteQRCode qrCode = new(qrCodeData);

        byte[] qrImageBytes = qrCode.GetGraphic(PixelsPerModule, DarkColor, LightColor, DrawQuietZones);
        using SKBitmap qrBitmap = SKBitmap.Decode(qrImageBytes);

        byte[] qrCodeIconBytes = TrainCloud.Microservices.Core.Properties.Resources.QrCodeIcon;
        using SKBitmap iconBitmap = SKBitmap.Decode(qrCodeIconBytes);
        // Calculate icon size
        int iconSize = (int)(qrBitmap.Width * IconSizePercent / 100.0);

        // Calculate position to center the icon
        int x = (qrBitmap.Width - iconSize) / 2;
        int y = (qrBitmap.Height - iconSize) / 2;

        // Create canvas for drawing
        using SKCanvas canvas = new(qrBitmap);

        // Optional: Draw white background behind icon for better contrast
        int padding = 5;
        using SKPaint whitePaint = new()
        {
            Color = SKColors.White,
            Style = SKPaintStyle.Fill
        };
        canvas.DrawRect(x - padding, y - padding,
            iconSize + (padding * 2), iconSize + (padding * 2), whitePaint);

        // Draw the icon with high quality
        using SKPaint iconPaint = new()
        {
            FilterQuality = SKFilterQuality.High,
            IsAntialias = true
        };

        SKRect destRect = new(x, y, x + iconSize, y + iconSize);
        canvas.DrawBitmap(iconBitmap, destRect, iconPaint);

        // Convert back to byte array
        using SKImage image = SKImage.FromBitmap(qrBitmap);
        using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }
}
