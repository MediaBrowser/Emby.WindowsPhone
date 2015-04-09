namespace Emby.WindowsPhone.Helpers
{
    public static class ResolutionHelper
    {
        public enum Resolutions { WVGA, WXGA, HD };

       
            private static bool IsWvga
            {
                get
                {
                    return App.Current.Host.Content.ScaleFactor == 100;
                }
            }

            private static bool IsWxga
            {
                get
                {
                    return App.Current.Host.Content.ScaleFactor == 160;
                }
            }

            private static bool IsHD
            {
                get
                {
                    return App.Current.Host.Content.ScaleFactor == 150;
                }
            }


            public static int Width
            {
                get
                {
                    if (IsHD)
                        return 1280;
                    if (IsWxga)
                        return 1280;
                    return 800;
                }
            }

            public static int Height
            {
                get
                {
                    if (IsHD)
                        return 720;
                    if (IsWxga)
                        return 768;
                    return 480;
                }
            }

            public static int DefaultVideoBitrate
            {
                get
                {
                    if (IsHD || IsWxga)
                        return 2500000;

                    return 1000000;
                }
            }
        }

    }
