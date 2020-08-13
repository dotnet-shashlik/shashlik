using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.vod.Model.V20170321;
using Guc.Kernel.Dependency;
using Microsoft.Extensions.Options;

namespace Guc.AliVideo
{
    public class AliyunVod : ITransient
    {
        private AliVideoOptions Options { get; }
        private DefaultAcsClient Client { get; }
        private string RegionId { get; }

        public AliyunVod(IOptions<AliVideoOptions> options)
        {
            Options = options.Value;
            RegionId = options.Value.RegionId;
            Client = InitVodClient(Options.AccessId, Options.AccessKey);
        }

        private DefaultAcsClient InitVodClient(string accessKeyId, string accessKeySecret)
        {
            // 点播服务接入区域
            var profile = DefaultProfile.GetProfile(RegionId, accessKeyId, accessKeySecret);
            profile.AddEndpoint(RegionId, RegionId, "vod", "vod." + RegionId + ".aliyuncs.com");

            return new DefaultAcsClient(profile);
        }

        /// <summary>
        /// 获取视频上传凭证
        /// </summary>
        /// <param name="title"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public VideoUploadDto CreateUploadVideo(string title, string name)
        {
            var request = new CreateUploadVideoRequest { Title = title, FileName = name, RegionId = RegionId };
            var response = Client.GetAcsResponse(request);
            return new VideoUploadDto()
            {
                AccountId = Options.AccountId,
                UploadAddress = response.UploadAddress,
                UploadAuth = response.UploadAuth,
                VideoId = response.VideoId
            };
        }

        /// <summary>
        /// 删除视频
        /// </summary>
        /// <param name="videoId"></param>
        public void DeleteVideo(string videoId)
        {
            var request = new DeleteVideoRequest();
            request.VideoIds = videoId;
            request.RegionId = RegionId;
            Client.GetAcsResponse(request);
        }

        /// <summary>
        /// 刷新视频上传凭证
        /// </summary>
        /// <param name="videoId"></param>
        /// <returns></returns>
        public VideoUploadDto RefreshUpload(string videoId)
        {
            var request = new RefreshUploadVideoRequest { VideoId = videoId, RegionId = RegionId };
            var response = Client.GetAcsResponse(request);
            return new VideoUploadDto()
            {
                AccountId = Options.AccountId,
                UploadAddress = response.UploadAddress,
                UploadAuth = response.UploadAuth,
                VideoId = response.VideoId
            };
        }

        /// <summary>
        /// 获取视频播放凭证
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="authTimeout"></param>
        /// <returns></returns>
        public GetVideoPlayAuthResponse GetVideoPlayAuth(string videoId, int authTimeout = 3000)
        {
            var request = new GetVideoPlayAuthRequest();
            request.VideoId = videoId;
            request.AuthInfoTimeout = authTimeout;
            request.RegionId = RegionId;
            return Client.GetAcsResponse(request);
        }

        /// <summary>
        /// 获取视频播放地址
        /// </summary>
        /// <param name="videoId"></param>
        /// <returns></returns>
        public string GetVideoPlayUrl(string videoId, int authTimeout = 7200)
        {
            var request = new GetPlayInfoRequest();
            request.VideoId = videoId;
            request.AuthTimeout = authTimeout;
            request.Formats = "m3u8";
            request.RegionId = RegionId;
            var response = Client.GetAcsResponse(request);
            return response?.PlayInfoList?.FirstOrDefault()?.PlayURL;
        }
    }
}
