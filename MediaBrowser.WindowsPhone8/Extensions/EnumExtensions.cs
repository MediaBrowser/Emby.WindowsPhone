using System;
using System.Globalization;
using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Model.Streaming;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.ViewModel;
using MediaBrowser.WindowsPhone.ViewModel.Settings;

namespace MediaBrowser.WindowsPhone.Extensions
{
    public static class EnumExtensions
    {
        public static string GetLocalisedName(this DayOfWeek dayOfWeek)
        {
            return DateTimeFormatInfo.CurrentInfo == null ? dayOfWeek.ToString() : DateTimeFormatInfo.CurrentInfo.GetDayName(dayOfWeek);
        }

        public static string GetLocalisedName(this GroupBy groupBy)
        {
            switch (groupBy)
            {
                case GroupBy.Genre:
                    return AppResources.Genre;
                case GroupBy.Name:
                    return AppResources.NameLabel;
                case GroupBy.ProductionYear:
                    return AppResources.ProductionYear;
            }

            return groupBy.ToString();
        }

        public static string GetLocalisedName(this RecordedGroupBy recordedGroupBy)
        {
            switch (recordedGroupBy)
            {
                case RecordedGroupBy.Channel:
                    return AppResources.LabelChannel;
                case RecordedGroupBy.RecordedDate:
                    return AppResources.LabelRecordedDate;
                case RecordedGroupBy.ShowName:
                    return AppResources.LabelShowName;
            }

            return recordedGroupBy.ToString();
        }

        public static string GetBitrateLabel(this StreamingLMH lmh, bool isWifi)
        {
            var vm = SimpleIoc.Default.GetInstance<SettingsViewModel>();
            if (vm == null)
            {
                return string.Empty;
            }

            var res = isWifi ? vm.WifiStreamingResolution : vm.StreamingResolution;

            switch (res)
            {
                case StreamingResolution.ThreeSixty:
                    return "1Mbps";
                case StreamingResolution.FourEighty:
                    switch (lmh)
                    {
                        case StreamingLMH.Low:
                            return "720Kbps";
                        case StreamingLMH.Medium:
                            return "1.5Mbps";
                        case StreamingLMH.High:
                            return "4.5Mpbs";
                    }
                    break;
                case StreamingResolution.SevenTwenty:
                    switch (lmh)
                    {
                        case StreamingLMH.Low:
                            return "5Mpbs";
                        case StreamingLMH.Medium:
                            return "7.5Mbps";
                        case StreamingLMH.High:
                            return "10Mbps";
                    }
                    break;
                case StreamingResolution.TenEighty:
                    switch (lmh)
                    {
                        case StreamingLMH.Low:
                            return "7.5Mbps";
                        case StreamingLMH.Medium:
                            return "15Mbps";
                        case StreamingLMH.High:
                            return "25Mbps";
                    }
                    break;
            }

            return string.Empty;
        }

        public static string GetProperResolutionName(this StreamingResolution res)
        {
            switch (res)
            {
                case StreamingResolution.ThreeSixty:
                    return "360p";
                case StreamingResolution.FourEighty:
                    return "480p";
                case StreamingResolution.SevenTwenty:
                    return "720p";
                case StreamingResolution.TenEighty:
                    return "1080p";
            }
            return string.Empty;
        }

        public static string GetLocalisedName<T>(this Enum<T> item)
        {
            return item.ToString();
        }
    }
}
