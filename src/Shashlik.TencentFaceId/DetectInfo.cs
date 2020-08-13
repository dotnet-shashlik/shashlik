using System;
using System.Collections.Generic;
using System.Text;

namespace TencentFaceId.Sdk
{
    public class DetectInfo
    {
        // ReSharper disable All 
#pragma warning disable IDE1006 // 命名样式

        /// <summary>
        /// 原始内容
        /// </summary>
        public string DetectInfoContent { get; set; }
        public _Text Text { get; set; }

        public _IdCardData IdCardData { get; set; }

        public _BestFrame BestFrame { get; set; }

        public _VideoData VideoData { get; set; }

        public class _Text
        {
            public string ErrCode { get; set; }
            public string ErrMsg { get; set; }
            public string IdCard { get; set; }
            public string Name { get; set; }
            public string OcrNation { get; set; }
            public string OcrAddress { get; set; }
            public string OcrBirth { get; set; }
            public string OcrAuthority { get; set; }
            public string OcrValidDate { get; set; }
            public string OcrName { get; set; }
            public string OcrIdCard { get; set; }
            public string OcrGender { get; set; }
            public string LiveStatus { get; set; }
            public string LiveMsg { get; set; }
            public string Comparestatus { get; set; }
            public string Comparemsg { get; set; }
            public string Extra { get; set; }
            public object Detail { get; set; }
        }

        public class _IdCardData
        {
            public string OcrFront { get; set; }
            public string OcrBack { get; set; }
        }

        public class _BestFrame
        {
            public string BestFrame { get; set; }
        }

        public class _VideoData
        {
            public string LivenessVideo { get; set; }
        }

#pragma warning restore IDE1006 // 命名样式
        // ReSharper restore All 
    }
}
