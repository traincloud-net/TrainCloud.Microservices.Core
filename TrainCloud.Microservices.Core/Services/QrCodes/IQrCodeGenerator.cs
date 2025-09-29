namespace TrainCloud.Microservices.Core.Services.QrCodes;

public interface IQrCodeGenerator
{
    byte[] GenerateQrCodePng(string codeData);
}
