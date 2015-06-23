// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemImageStore.cs" company="">
// Copyright (c) 2009-2010 Esben Carlsen
// Forked by Jaben Cargman and CaptiveAire Systems
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
// </copyright>
// <summary>
//   The Scheduler
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using DynamicImageHandler.Utils;

namespace DynamicImageHandler.Services
{
    public class AppScheduler
    {
        private static readonly object _lock = new object();

        private static readonly SortedDictionary<DateTime, IScheduleTask> _pendingTasks = new SortedDictionary<DateTime, IScheduleTask>();

        private static readonly Timer _timer = new Timer(ProcessTasks, null, -1, -1);

        public static bool IsScheduledTask(IScheduleTask task)
        {
            return _pendingTasks.ContainsValue(task);
        }

        public static void ScheduleTask(IScheduleTask task)
        {
            lock (_lock)
            {
                DateTime occursAt = task.GetNextReoccurrence();
                while (_pendingTasks.ContainsKey(occursAt))
                {
                    occursAt = occursAt.AddMilliseconds(1);
                }

                _pendingTasks.Add(occursAt, task);
                ProcessTasks(null);
            }
        }

        public static void UnscheduleTask(IScheduleTask task)
        {
            lock (_lock)
            {
                var entry = _pendingTasks.FirstOrDefault(p => p.Value == task);
                if (entry.IsNotDefault())
                {
                    _pendingTasks.Remove(entry.Key);
                    ProcessTasks(null);
                }
            }
        }

        private static void ProcessTasks(object state)
        {
            lock (_lock)
            {
                DateTime now = DateTime.Now;

                foreach (var t in _pendingTasks.Where(p => p.Key < now).ToList())
                {
                    _pendingTasks.Remove(t.Key);
                    ThreadPool.QueueUserWorkItem(TaskThreadProcess, t.Value);
                }

                if (_pendingTasks.Any())
                {
                    // Set the time to fire for the next task
                    var first = _pendingTasks.First();
                    DateTime key = first.Key;
                    TimeSpan next = key - DateTime.Now;

                    // Set the timer to fire just after the task expires
                    _timer.Change(((int)next.TotalMilliseconds) + 10, -1);
                }
                else
                {
                    // Clear the timer we have no more task to run
                    _timer.Change(-1, -1);
                }
            }
        }

        private static void TaskThreadProcess(object state)
        {
            var task = (IScheduleTask)state;

            try
            {
                task.RunTask();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
            finally
            {
                if (task.ReoccurringTask && !IsScheduledTask(task))
                {
                    ScheduleTask(task);
                }
            }
        }
    }
}