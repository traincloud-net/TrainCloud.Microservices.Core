namespace TrainCloud.Microservices.Core.Services.QrCodes;

internal interface IQrCodeGenerator
{
    byte[] GenerateQrCodePng(string codeData);
}
