// DynamicImageHandler - Copyright (c) 2015 CaptiveAire

using System.Drawing;
using System.Web;

using DynamicImageHandler.ImageParameters;

namespace DynamicImageHandler.Filters
{
    internal class GrayscaleFilter : MappedParameterFilterBase<GrayscaleFilterParameters>
    {
        public override bool ProcessMapped(GrayscaleFilterParameters @params, HttpContext context, ref Bitmap bitmap)
        {
            bitmap = this.ImageTool.ToGreyScale(bitmap);

            return true;
        }
    }

    internal class GrayscaleFilterParameters : IImageParameterMapping
    {
        [ParameterNames("grayscale", "greyscale")]
        public bool Enabled { get; set; }

        public bool IsValid()
        {
            return this.Enabled;
        }
    }
}