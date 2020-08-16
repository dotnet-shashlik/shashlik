using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.AliVideo
{
    public class VodCallback
    {
        #region Common
        public DateTime EventTime { get; set; }
        public string EventType { get; set; }
        public string VideoId { get; set; }
        public string Status { get; set; }
        public string Extend { get; set; }

        #endregion


        #region SnapshotComplete
        public string CoverUrl { get; set; }
        #endregion

        #region TranscodeComplete

        public StreamInfo[] StreamInfos { get; set; }

        #endregion
    }

    public class StreamInfo
    {
        public string Status { get; set; }
        public int Bitrate { get; set; }
        public string Definition { get; set; }
        public decimal Duration { get; set; }
        public bool Encrypt { get; set; }
        public string FileUrl { get; set; }
        public string Format { get; set; }
        public int Fps { get; set; }
        public int Height { get; set; }
        public int Size { get; set; }
        public int Width { get; set; }
        public string JobId { get; set; }
    }
}
