using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Phone.Controls;
using CustomMessageBox = MediaBrowser.WindowsPhone.Controls.CustomMessageBox;

namespace MediaBrowser.WindowsPhone.Extensions
{
    public static class ToolkitExtensions
    {
        public static Task<CustomMessageBoxResult> ShowAsync(this CustomMessageBox source)
        {
            var completion = new TaskCompletionSource<CustomMessageBoxResult>();

            // wire up the event that will be used as the result of this method
            EventHandler<DismissedEventArgs> dismissed = null;
            dismissed += (sender, args) =>
            {
                completion.SetResult(args.Result);

                // make sure we unsubscribe from this!
                source.Dismissed -= dismissed;
            };

            source.Show();
            return completion.Task;
        }

        private const string ExternalAddress = "app://external/";
        
        /// <summary>
        /// An implementation of the Contains member of string that takes in a 
        /// string comparison. The traditional .NET string Contains member uses 
        /// StringComparison.Ordinal.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <param name="value">The string value to search for.</param>
        /// <param name="comparison">The string comparison type.</param>
        /// <returns>Returns true when the substring is found.</returns>
        public static bool Contains(this string s, string value, StringComparison comparison)
        {
            return s.IndexOf(value, comparison) >= 0;
        }

        /// <summary>
        /// Returns whether the page orientation is in portrait.
        /// </summary>
        /// <param name="orientation">Page orientation</param>
        /// <returns>If the orientation is portrait</returns>
        public static bool IsPortrait(this PageOrientation orientation)
        {
            return (PageOrientation.Portrait == (PageOrientation.Portrait & orientation));
        }

        /// <summary>
        /// Returns whether the dark visual theme is currently active.
        /// </summary>
        /// <param name="resources">Resource Dictionary</param>
        public static bool IsDarkThemeActive(this ResourceDictionary resources)
        {
            return ((Visibility)resources["PhoneDarkThemeVisibility"] == Visibility.Visible);
        }

        /// <summary>
        /// Returns whether the uri is from an external source.
        /// </summary>
        /// <param name="uri">The uri</param>
        public static bool IsExternalNavigation(this Uri uri)
        {
            return (ExternalAddress == uri.ToString());
        }

        public static FlowDirection GetUsefulFlowDirection(this FrameworkElement element)
        {
            if (element.ReadLocalValue(FrameworkElement.FlowDirectionProperty) == DependencyProperty.UnsetValue)
            {
                PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;

                if (frame != null)
                {
                    PhoneApplicationPage page = frame.Content as PhoneApplicationPage;

                    if (page != null)
                    {
                        return page.FlowDirection;
                    }

                    return frame.FlowDirection;
                }
            }

            return element.FlowDirection;
        }
    }
}
