namespace DynamicImageHandler.ImageTool.Wpf
{
  #region Using

  using System;
  using System.Drawing;
  using System.Drawing.Imaging;
  using System.IO;
  using System.Windows;
  using System.Windows.Interop;
  using System.Windows.Media.Imaging;

  using DynamicImageHandler.ImageTools;

  #endregion

  /// <summary>
  /// The wpf image tool.
  /// </summary>
  public class WpfImageTool : NativeImageTool
  {
    #region Public Methods

    /// <summary>
    /// The encode.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="imageFormat">
    /// The image format.
    /// </param>
    /// <returns>
    /// </returns>
    public override byte[] Encode(Bitmap source, ImageFormat imageFormat)
    {
      if (imageFormat == ImageFormat.Png)
      {
        return EncodeBitmap(new PngBitmapEncoder(), source);
      }

      if (imageFormat == ImageFormat.Gif)
      {
        return EncodeBitmap(new GifBitmapEncoder(), source);
      }

      if (imageFormat == ImageFormat.Jpeg)
      {
        return EncodeBitmap(new JpegBitmapEncoder() { QualityLevel = 90 }, source);
      }

      if (imageFormat == ImageFormat.Tiff)
      {
        return EncodeBitmap(new TiffBitmapEncoder(), source);
      }

      return base.Encode(source, imageFormat);
    }

    #endregion

    #region Methods

    /// <summary>
    /// The encode bitmap.
    /// </summary>
    /// <param name="bitmapEncoder">
    /// The bitmap encoder.
    /// </param>
    /// <param name="bitmap">
    /// The bitmap.
    /// </param>
    /// <returns>
    /// </returns>
    private static byte[] EncodeBitmap(BitmapEncoder bitmapEncoder, Bitmap bitmap)
    {
      BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
        bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

      using (var ms = new MemoryStream())
      {
        bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));
        bitmapEncoder.Save(ms);
        return ms.ToArray();
      }
    }

    #endregion
  }
}