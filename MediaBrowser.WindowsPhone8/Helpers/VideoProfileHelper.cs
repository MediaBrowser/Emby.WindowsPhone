using System.Collections.Generic;
using MediaBrowser.Model.Dlna;
using Microsoft.Phone.Info;

namespace MediaBrowser.WindowsPhone.Helpers
{
    public static class VideoProfileHelper
    {
        public static DeviceProfile GetWindowsPhoneProfile(bool isHls = false)
        {
            return MediaCapabilities.IsMultiResolutionVideoSupported ? GetWindowsPhoneProfileS4(isHls) : GetWindowsPhoneProfileNonS4(isHls);
        }

        private static DeviceProfile GetWindowsPhoneProfileS4(bool isHls = false)
        {
            var profile = new DeviceProfile();
            var transcodingProfiles = new List<TranscodingProfile>
            {
                new TranscodingProfile
                {
                    Container = "mp3",
                    AudioCodec = "mp3",
                    Type = DlnaProfileType.Audio,
                    Context = EncodingContext.Streaming
                },
                new TranscodingProfile
                {
                    Container = "mp4",
                    VideoCodec = "h264",
                    AudioCodec = "aac",
                    Type = DlnaProfileType.Video,
                    Context = EncodingContext.Static
                },
                new TranscodingProfile
                {
                    Container = "mp3",
                    AudioCodec = "mp3",
                    Type = DlnaProfileType.Audio,
                    Context = EncodingContext.Static
                },
                new TranscodingProfile
                {
                    Container = "jpeg,png,gif,bmp",
                    Type = DlnaProfileType.Photo,
                    Context = EncodingContext.Static
                }
            };

            if (isHls)
            {
                transcodingProfiles.Add(new TranscodingProfile
                {
                    Protocol = "hls",
                    Container = "ts",
                    VideoCodec = "h264",
                    AudioCodec = "aac",
                    Type = DlnaProfileType.Video,
                    Context = EncodingContext.Streaming
                });
            }

            transcodingProfiles.Add(new TranscodingProfile
            {
                Container = "mp4",
                VideoCodec = "h264",
                AudioCodec = "aac",
                Type = DlnaProfileType.Video,
                Context = EncodingContext.Streaming
            });

            profile.TranscodingProfiles = transcodingProfiles.ToArray();

            profile.DirectPlayProfiles = new[]
            {
                new DirectPlayProfile
                {
                    Container = "3gp,mp4,mov,m4v",
                    VideoCodec = "h264",
                    AudioCodec = "aac,mp3",
                    Type = DlnaProfileType.Video
                },

                new DirectPlayProfile
                {
                    Container = "mp4,m4v,avi,3gp",
                    VideoCodec = "mpeg4,msmpeg4",
                    AudioCodec = "aac,mp3",
                    Type = DlnaProfileType.Video
                },

                new DirectPlayProfile
                {
                    Container = "wmv",
                    VideoCodec = "vc1",
                    AudioCodec = "aac,mp3",
                    Type = DlnaProfileType.Video
                },

                new DirectPlayProfile
                {
                    Container = "asf",
                    VideoCodec = "wmv2,wmv3,vc1",
                    AudioCodec = "wmav2,wmapro,wmavoice",
                    Type = DlnaProfileType.Video
                },

                new DirectPlayProfile
                {
                    Container = "asf",
                    AudioCodec = "wmav2,wmapro,wmavoice",
                    Type = DlnaProfileType.Audio
                },

                new DirectPlayProfile
                {
                    Container = "mp3,mp4,aac,wma",
                    Type = DlnaProfileType.Audio
                },

                new DirectPlayProfile
                {
                    Container = "jpeg,png,gif,bmp",
                    Type = DlnaProfileType.Photo
                }
            };

            profile.CodecProfiles = new[]
            {
                new CodecProfile
                {
                    Type = CodecType.Video,
                    Codec="h264",
                    Conditions = new []
                    {
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.Width,
                            Value = "1920"
                        },
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.Height,
                            Value = "1080"
                        },
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.VideoBitrate,
                            Value = "20000000",
                            IsRequired = false
                        },
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.VideoFramerate,
                            Value = "30",
                            IsRequired = false
                        },
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.VideoLevel,
                            Value = "40"
                        },
                        new ProfileCondition(ProfileConditionType.EqualsAny, ProfileConditionValue.VideoProfile, "high|main|baseline|constrained baseline"),
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.NotEquals,
                            Property = ProfileConditionValue.IsAnamorphic,
                            Value = "true"
                        }
                    }
                },

                new CodecProfile
                {
                    Type = CodecType.Video,
                    Codec="mpeg4,msmpeg4,wmv2,wmv3,vc1",
                    Conditions = new []
                    {
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.Width,
                            Value = "1920"
                        },
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.Height,
                            Value = "1080"
                        },
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.VideoBitrate,
                            Value = "20000000",
                            IsRequired = false
                        },
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.VideoFramerate,
                            Value = "30",
                            IsRequired = false
                        },
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.NotEquals,
                            Property = ProfileConditionValue.IsAnamorphic,
                            Value = "true"
                        }
                    }
                },

                new CodecProfile
                {
                    Type = CodecType.VideoAudio,
                    Conditions = new []
                    {
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.AudioBitrate,
                            Value = "320000"
                        },

                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.AudioChannels,
                            Value = "2"
                        }
                    }
                },

                new CodecProfile
                {
                    Type = CodecType.Audio,
                    Conditions = new []
                    {
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.AudioBitrate,
                            Value = "320000"
                        }
                    }
                }
            };

            profile.MaxStreamingBitrate = 20000000;
            profile.MaxStaticBitrate = 20000000;

            return profile;
        }

        private static DeviceProfile GetWindowsPhoneProfileNonS4(bool isHls = false)
        {
            var profile = new DeviceProfile();
            var transcodingProfiles = new List<TranscodingProfile>
            {
                new TranscodingProfile
                {
                    Container = "mp3",
                    AudioCodec = "mp3",
                    Type = DlnaProfileType.Audio,
                    Context = EncodingContext.Streaming
                },
                new TranscodingProfile
                {
                    Container = "mp4",
                    VideoCodec = "h264",
                    AudioCodec = "aac",
                    Type = DlnaProfileType.Video,
                    Context = EncodingContext.Static
                },
                new TranscodingProfile
                {
                    Container = "mp3",
                    AudioCodec = "mp3",
                    Type = DlnaProfileType.Audio,
                    Context = EncodingContext.Static
                },
                new TranscodingProfile
                {
                    Container = "jpeg,png,gif,bmp",
                    Type = DlnaProfileType.Photo,
                    Context = EncodingContext.Static
                }
            };

            if (isHls)
            {
                transcodingProfiles.Add(new TranscodingProfile
                {
                    Protocol = "hls",
                    Container = "ts",
                    VideoCodec = "h264",
                    AudioCodec = "aac",
                    Type = DlnaProfileType.Video,
                    Context = EncodingContext.Streaming
                });
            }

            transcodingProfiles.Add(new TranscodingProfile
            {
                Container = "mp4",
                VideoCodec = "h264",
                AudioCodec = "aac",
                Type = DlnaProfileType.Video,
                Context = EncodingContext.Streaming
            });

            profile.TranscodingProfiles = transcodingProfiles.ToArray();

            profile.DirectPlayProfiles = new[]
            {
                new DirectPlayProfile
                {
                    Container = "3gp,mp4,mov,m4v",
                    VideoCodec = "h264",
                    AudioCodec = "aac,mp3",
                    Type = DlnaProfileType.Video
                },

                new DirectPlayProfile
                {
                    Container = "mp4,m4v,avi,3gp",
                    VideoCodec = "mpeg4,msmpeg4",
                    AudioCodec = "aac,mp3",
                    Type = DlnaProfileType.Video
                },

                new DirectPlayProfile
                {
                    Container = "wmv",
                    VideoCodec = "vc1",
                    AudioCodec = "aac,mp3",
                    Type = DlnaProfileType.Video
                },

                new DirectPlayProfile
                {
                    Container = "asf",
                    VideoCodec = "wmv2,wmv3,vc1",
                    AudioCodec = "wmav2,wmapro,wmavoice",
                    Type = DlnaProfileType.Video
                },

                new DirectPlayProfile
                {
                    Container = "asf",
                    AudioCodec = "wmav2,wmapro,wmavoice",
                    Type = DlnaProfileType.Audio
                },

                new DirectPlayProfile
                {
                    Container = "mp3,mp4,aac,wma",
                    Type = DlnaProfileType.Audio
                },

                new DirectPlayProfile
                {
                    Container = "jpeg,png,gif,bmp",
                    Type = DlnaProfileType.Photo
                }
            };

            profile.CodecProfiles = new[]
            {
                new CodecProfile
                {
                    Type = CodecType.Video,
                    Codec="h264",
                    Conditions = new []
                    {
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.Width,
                            Value = "1280"
                        },
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.Height,
                            Value = "720"
                        },
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.VideoBitrate,
                            Value = "10000000",
                            IsRequired = false
                        },
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.VideoFramerate,
                            Value = "30",
                            IsRequired = false
                        },
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.VideoLevel,
                            Value = "31"
                        },
                        new ProfileCondition(ProfileConditionType.EqualsAny, ProfileConditionValue.VideoProfile, "high|main|baseline|constrained baseline"),
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.NotEquals,
                            Property = ProfileConditionValue.IsAnamorphic,
                            Value = "true"
                        }
                    }
                },

                new CodecProfile
                {
                    Type = CodecType.Video,
                    Codec="mpeg4,msmpeg4,wmv2,wmv3,vc1",
                    Conditions = new []
                    {
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.Width,
                            Value = "800"
                        },
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.Height,
                            Value = "600"
                        },
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.VideoBitrate,
                            Value = "2000000",
                            IsRequired = false
                        },
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.VideoFramerate,
                            Value = "30",
                            IsRequired = false
                        },
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.NotEquals,
                            Property = ProfileConditionValue.IsAnamorphic,
                            Value = "true"
                        }
                    }
                },

                new CodecProfile
                {
                    Type = CodecType.VideoAudio,
                    Conditions = new []
                    {
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.AudioBitrate,
                            Value = "320000"
                        },

                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.AudioChannels,
                            Value = "2"
                        }
                    }
                },

                new CodecProfile
                {
                    Type = CodecType.Audio,
                    Conditions = new []
                    {
                        new ProfileCondition
                        {
                            Condition = ProfileConditionType.LessThanEqual,
                            Property = ProfileConditionValue.AudioBitrate,
                            Value = "320000"
                        }
                    }
                }
            };

            profile.MaxStreamingBitrate = 20000000;
            profile.MaxStaticBitrate = 20000000;

            return profile;
        }
    }
}