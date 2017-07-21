using DynamicImageHandler.ImageParameters;

namespace DynamicImageHandler.ImageProcessors
{
    public interface IImageProcessor
    {
        /// <summary>
        /// Gets the image from cache or processes the image fresh and stores it.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">parameters</exception>
        byte[] GetProcessedImage(IImageParameters parameters);
    }
}