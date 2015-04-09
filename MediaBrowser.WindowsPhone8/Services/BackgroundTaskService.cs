using System;
using System.Threading.Tasks;
using Emby.WindowsPhone.Localisation;
using Microsoft.Phone.Scheduler;

namespace Emby.WindowsPhone.Services
{
    public class BackgroundTaskService
    {
        private static BackgroundTaskService _current;

        public static BackgroundTaskService Current { get { return _current ?? (_current = new BackgroundTaskService()); } }

        public ScheduledAction GetTask()
        {
            return GetTask(Constants.PhotoUploadBackgroundTaskName);
        }

        private static ScheduledAction GetTask(string name)
        {
            return ScheduledActionService.Find(name);
        }

        public bool TaskExists
        {
            get { return CheckTaskExists(); }
        }

        private bool CheckTaskExists()
        {
            return GetTask() != null;
        }

        public void RemoveTask()
        {
            if (TaskExists)
            {
                ScheduledActionService.Remove(Constants.PhotoUploadBackgroundTaskName);
            }
        }

        public bool CreateTask()
        {
            try
            {
                RemoveTask();

                var resourceTask = new ResourceIntensiveTask(Constants.PhotoUploadBackgroundTaskName)
                {
                    Description = AppResources.BackgroundTaskDescription
                };
                ScheduledActionService.Add(resourceTask);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void LaunchTask()
        {
            ScheduledActionService.LaunchForTest(Constants.PhotoUploadBackgroundTaskName, TimeSpan.FromSeconds(10));
        }
    }
}
