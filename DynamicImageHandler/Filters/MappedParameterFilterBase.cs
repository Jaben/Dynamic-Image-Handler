// --------------------------------------------------------------------------------------------------------------------
// Copyright (c) 2009-2010 Esben Carlsen
// Forked Copyright (c) 2011-2017 Jaben Cargman and CaptiveAire Systems
//	
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.

// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA 
// --------------------------------------------------------------------------------------------------------------------

using System.Drawing;
using System.Web;

using DynamicImageHandler.ImageParameters;
using DynamicImageHandler.ImageTools;

namespace DynamicImageHandler.Filters
{
    public abstract class MappedParameterFilterBase<TParams> : IImageFilter
        where TParams : IImageParameterMapping, new()
    {
        protected IImageTool ImageTool => Factory.ImageTool;

        protected int ActualOrder = 10;

        public virtual int Order => this.ActualOrder;

        public virtual bool Process(IImageParameters parameters, ref Bitmap bitmap)
        {
            var mapParameter = parameters.MapParameter<TParams>();

            if (!mapParameter.IsValid())
            {
                return false;
            }

            return ProcessMapped(mapParameter, ref bitmap);
        }

        public abstract bool ProcessMapped(TParams @params, ref Bitmap bitmap);
    }
}