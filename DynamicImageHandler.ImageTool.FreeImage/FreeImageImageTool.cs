using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using FreeImageAPI;

using DynamicImageHandler.ImageTools;

namespace DynamicImageHandler.ImageTool.FreeImage
{
	public class FreeImageImageTool : NativeImageTool
	{
		#region Instance Methods

		public override byte[] Encode(Bitmap source, ImageFormat imageFormat)
		{
			FIBITMAP dib = FreeImageAPI.FreeImage.CreateFromBitmap(source);

			if (dib.IsNull)
			{
				return base.Encode(source, imageFormat);
			}

			using (MemoryStream ms = new MemoryStream())
			{
				if (!FreeImageAPI.FreeImage.SaveToStream(
					ref dib, ms,
					ConvertToFreeImageFormat(imageFormat),
					ConvertToFreeImageSaveFlags(imageFormat),
					FREE_IMAGE_COLOR_DEPTH.FICD_AUTO,
					true))
				{
					return base.Encode(source, imageFormat);
				}

				return ms.ToArray();
			}
		}

		#endregion

		#region Class Methods

		private static FREE_IMAGE_FORMAT ConvertToFreeImageFormat(ImageFormat imageFormat)
		{
			if (imageFormat == ImageFormat.Png)
			{
				return FREE_IMAGE_FORMAT.FIF_PNG;
			}

			if (imageFormat == ImageFormat.Gif)
			{
				return FREE_IMAGE_FORMAT.FIF_GIF;
			}

			if (imageFormat == ImageFormat.Jpeg)
			{
				return FREE_IMAGE_FORMAT.FIF_JPEG;
			}

			if (imageFormat == ImageFormat.Tiff)
			{
				return FREE_IMAGE_FORMAT.FIF_TIFF;
			}

			return FREE_IMAGE_FORMAT.FIF_PNG;
		}

		private static FREE_IMAGE_SAVE_FLAGS ConvertToFreeImageSaveFlags(ImageFormat imageFormat)
		{
			if (imageFormat == ImageFormat.Png)
			{
				return FREE_IMAGE_SAVE_FLAGS.PNG_Z_BEST_COMPRESSION;
			}

			if (imageFormat == ImageFormat.Gif)
			{
				return FREE_IMAGE_SAVE_FLAGS.DEFAULT;
			}

			if (imageFormat == ImageFormat.Jpeg)
			{
				return FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYGOOD;
			}

			if (imageFormat == ImageFormat.Tiff)
			{
				return FREE_IMAGE_SAVE_FLAGS.TIFF_LZW;
			}

			return FREE_IMAGE_SAVE_FLAGS.PNG_Z_BEST_COMPRESSION;
		}

		#endregion
	}
}