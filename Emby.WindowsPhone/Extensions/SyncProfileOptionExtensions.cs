using Emby.WindowsPhone.Localisation;
using MediaBrowser.Model.Sync;

namespace Emby.WindowsPhone.Extensions
{
    public static class SyncProfileOptionExtensions
    {
        public static string GetName(this SyncProfileOption option)
        {
            var id = string.Format("Profile{0}", option.Name);
            var name = AppResources.ResourceManager.GetString(id);
            return string.IsNullOrEmpty(name) ? option.Name : name;
        }

        public static string GetDescription(this SyncProfileOption option)
        {
            var id = string.Format("Profile{0}Description", option.Name);
            var description = AppResources.ResourceManager.GetString(id);
            return string.IsNullOrEmpty(description) ? option.Name : description;
        }

        public static SyncProfileOption Localise(this SyncProfileOption option)
        {
            option.Name = option.GetName();
            option.Description = option.GetDescription();
            return option;
        }
    }
}