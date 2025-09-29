using QRCoder;

namespace TrainCloud.Microservices.Core.Services.QrCodes;

public sealed class QrCodeGenerator : IQrCodeGenerator
{
    public System.Drawing.Color DarkColor { get; } = System.Drawing.Color.Black;

    public System.Drawing.Color LightColor { get; } = System.Drawing.Color.White;

    public int PixelsPerModule { get; } = 5;

    public bool DrawQuietZones { get; } = true;

    public byte[] GenerateQrCodePng(string codeData)
    {
        QRCodeGenerator.ECCLevel eccLevel = QRCodeGenerator.ECCLevel.H;

        using QRCodeGenerator qrGenerator = new();
        using QRCodeData qrCodeData = qrGenerator.CreateQrCode(codeData, eccLevel);
        using PngByteQRCode qrCode = new(qrCodeData);

        byte[] imageBytes = qrCode.GetGraphic(PixelsPerModule, DarkColor, LightColor, DrawQuietZones);

        return imageBytes;
    }
}
