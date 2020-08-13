using Guc.Utils.Common;
using Senparc.Weixin;
using Senparc.Weixin.MP.Containers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Guc.Utils.Extensions;

namespace Guc.Wx
{
    class DefaultWxMedia : IWxMedia, Guc.Kernel.Dependency.ITransient
    {
        public DefaultWxMedia(IWxSettings wxSettings)
        {
            this.wxSettings = wxSettings;
        }

        IWxSettings wxSettings { get; }

        public async Task<WxMediaFile> GetFile(string mediaId)
        {
            // api doc:https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1444738727

            var token = wxSettings.Token;
            string url = $"https://api.weixin.qq.com/cgi-bin/media/get?access_token={token}&media_id={mediaId}";

            var res = await HttpHelper.GetForOriginResponse(url);

            if (!res.IsSuccessful)
                throw new Exception($"获取微信媒体文件发生错误,url:{url},method:get,httpcode:{res.StatusCode},result:{res.Content}", res.ErrorException);

            var dis = res.Headers.FirstOrDefault(r => r.Name.EqualsIgnoreCase("Content-disposition"))?.Value?.ToString();
            if (dis.IsNullOrWhiteSpace())
                throw new Exception($"获取微信媒体文件结果错误:{res.Content}");

            // 这才是文件
            if (dis.Contains("attachment", StringComparison.OrdinalIgnoreCase))
            {
                string fileName = null;
                var items = dis.Split(new[] { ';', '=' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].Trim().EqualsIgnoreCase("filename"))
                    {
                        if (i + 1 >= items.Length)
                        {
                            fileName = null;
                            break;
                        }
                        fileName = items[i + 1];
                        break;
                    }
                }

                string httpContentType = res.Headers.FirstOrDefault(r => r.Name.EqualsIgnoreCase("Content-Type"))?.Value?.ToString();
                return new WxMediaFile
                {
                    Data = res.RawBytes,
                    FileName = fileName?.Trim('"'),
                    HttpContentType = httpContentType
                };
            }
            else
                throw new Exception($"获取微信媒体文件结果错误:{res.Content}");
        }
    }
}
