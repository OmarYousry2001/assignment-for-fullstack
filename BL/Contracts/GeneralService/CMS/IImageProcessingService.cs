namespace BL.Contracts.GeneralService.CMS
{
    public interface IImageProcessingService
    {
        byte[] ResizeImage(byte[] imageBytes, int width, int height);
        byte[] ConvertToWebP(byte[] imageBytes, int quality = 100);
        byte[] ResizeAndConvertToWebP(byte[] imageBytes, int width, int height, int quality = 100);
    }
}
