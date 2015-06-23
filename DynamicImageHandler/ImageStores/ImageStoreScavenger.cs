// --------------------------------------------------------------------------------------------------------------------
// Copyright (c) 2009-2010 Esben Carlsen
// Forked Copyright (c) 2011-2015 Jaben Cargman and CaptiveAire Systems
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

using System;

using DynamicImageHandler.Services;

namespace DynamicImageHandler.ImageStores
{
    public abstract class ImageStoreScavenger : IScheduleTask
    {
        protected ImageStoreScavenger(TimeSpan scavengeInterval, bool startInitialScavenge)
        {
            this._scavengeInterval = scavengeInterval;
            this._firstRun = startInitialScavenge;
        }

        public bool ReoccurringTask
        {
            get
            {
                return true;
            }
        }

        protected abstract void CleanOldImages();

        private readonly TimeSpan _scavengeInterval;

        private bool _firstRun;

        public DateTime GetNextReoccurrence()
        {
            if (this._firstRun)
            {
                this._firstRun = false;
                return DateTime.Now.AddSeconds(10);
            }

            return DateTime.Now.Add(this._scavengeInterval);
        }

        public void RunTask()
        {
            this.CleanOldImages();
        }
    }
}