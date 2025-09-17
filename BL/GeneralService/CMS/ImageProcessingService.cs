using BL.Contracts.GeneralService.CMS;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

public class ImageProcessingService : IImageProcessingService
{
    /// <summary>
    /// Resizes an image without converting its format.
    /// </summary>
    /// <param name="imageBytes">The image as a byte array.</param>
    /// <param name="width">The target width.</param>
    /// <param name="height">The target height.</param>
    /// <returns>The resized image as a byte array.</returns>
    public byte[] ResizeImage(byte[] imageBytes, int width, int height)
    {
        using var inputStream = new MemoryStream(imageBytes);
        using var image = Image.Load(inputStream);

        // Resize the image
        image.Mutate(x => x.Resize(width, height));

        // Save the image in its original format
        using var outputStream = new MemoryStream();
        image.Save(outputStream, image.Metadata.DecodedImageFormat); // Preserve the original format
        return outputStream.ToArray();
    }

    /// <summary>
    /// Converts an image to WebP format.
    /// </summary>
    /// <param name="imageBytes">The image as a byte array.</param>
    /// <param name="quality">The quality of the output image (1-100).</param>
    /// <returns>The converted image as a byte array.</returns>
    public byte[] ConvertToWebP(byte[] imageBytes, int quality = 100)
    {
        using var inputStream = new MemoryStream(imageBytes);
        using var image = Image.Load(inputStream);

        // Configure the WebP encoder
        var webpEncoder = new WebpEncoder
        {
            Quality = quality, // Use the provided quality parameter
        };

        // Save the image in WebP format
        using var outputStream = new MemoryStream();
        image.SaveAsWebp(outputStream, webpEncoder);
        return outputStream.ToArray();
    }

    /// <summary>
    /// Resizes an image and converts it to WebP format.
    /// </summary>
    /// <param name="imageBytes">The image as a byte array.</param>
    /// <param name="width">The target width.</param>
    /// <param name="height">The target height.</param>
    /// <param name="quality">The quality of the output image (1-100).</param>
    /// <returns>The resized and converted image as a byte array.</returns>
    public byte[] ResizeAndConvertToWebP(byte[] imageBytes, int width, int height, int quality = 100)
    {
        using var inputStream = new MemoryStream(imageBytes);
        using var image = Image.Load(inputStream);

        // Resize the image
        image.Mutate(x => x.Resize(width, height));

        // Configure the WebP encoder
        var webpEncoder = new WebpEncoder
        {
            Quality = quality
        };

        // Save the image in WebP format
        using var outputStream = new MemoryStream();
        image.SaveAsWebp(outputStream, webpEncoder);
        return outputStream.ToArray();
    }
}