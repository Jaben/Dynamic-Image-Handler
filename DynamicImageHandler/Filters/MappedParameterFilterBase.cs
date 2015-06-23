// DynamicImageHandler - Copyright (c) 2015 CaptiveAire

using System.Drawing;
using System.Web;

using DynamicImageHandler.ImageParameters;
using DynamicImageHandler.ImageTools;

namespace DynamicImageHandler.Filters
{
    public abstract class MappedParameterFilterBase<TParams> : IImageFilter
        where TParams : IImageParameterMapping, new()
    {
        protected IImageTool ImageTool
        {
            get
            {
                return Factory.GetImageTool();
            }
        }

        protected int ActualOrder = 10;

        public virtual int Order
        {
            get
            {
                return this.ActualOrder;
            }
        }

        public virtual bool Process(IImageParameters parameters, HttpContext context, ref Bitmap bitmap)
        {
            var mapParameter = parameters.MapParameter<TParams>();

            if (!mapParameter.IsValid())
            {
                return false;
            }

            return ProcessMapped(mapParameter, context, ref bitmap);
        }

        public abstract bool ProcessMapped(TParams @params, HttpContext context, ref Bitmap bitmap);
    }
}