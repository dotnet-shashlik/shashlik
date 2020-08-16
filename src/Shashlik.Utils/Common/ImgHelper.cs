using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Shashlik.Utils.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;
using System.Numerics;
using SixLabors.Shapes;
using System.Text.RegularExpressions;

namespace Shashlik.Utils.Common
{
    /// <summary>
    /// 图片处理帮助类
    /// </summary>
    public class ImgHelper
    {
        /// <summary>
        /// 是否为图片
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static bool IsImage(Stream stream)
        {
            try
            {
                using (Image.Load(stream)) { }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 是否为图片
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <returns></returns>
        public static bool IsImageFromFile(string filePath)
        {
            using (var fs = File.Open(filePath, FileMode.Open))
            {
                return IsImage(fs);
            }
        }

        /// <summary>
        /// 是否为图片
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsImage(string url)
        {
            if (url.IsNullOrWhiteSpace())
                return false;
            if (!url.IsUrl())
                return false;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var res = client.GetStreamAsync(url).GetAwaiter().GetResult();
                    return IsImage(res);
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 生成6位图形验证码(gif格式)
        /// </summary>
        /// <param name="saveStream">保存到流</param>
        /// <param name="codeLength">随机码长度 最小值4,最大值10</param>
        /// <param name="width">图片宽度 最低124</param>
        /// <param name="height">图片高度 最低20</param>
        /// <returns>验证码</returns>
        public static string BuildVerifyCode(Stream saveStream, int codeLength = 4, int width = 136, int height = 36)
        {
            if (codeLength < 4)
                codeLength = 4;
            if (codeLength > 10)
                codeLength = 10;
            if (width < 124)
                width = 124;
            if (height < 20)
                height = 20;

            var rnd = new Random();
            var code = RandomCode(rnd, codeLength);

            var maxX = width / codeLength; // 每个文字最大x宽度
            var prevTextX = 10; // 前面一个文字的x坐标
            var size = 20;// 字体大小

            Font font = null;
            using (var fontSm = typeof(ImgHelper).Assembly.GetManifestResourceStream("Shashlik.Utils.Common.ARLRDBD.TTF"))
            {
                // 字体
                var install_Family = new FontCollection().Install(fontSm);
                font = new Font(install_Family, size, FontStyle.Bold);  //字体
            }

            // 点坐标
            var listPath = new List<IPath>();

            // 噪点
            for (int i = 0; i < 300; i++)
            {
                var position1 = new Vector2(rnd.Next(1, width), rnd.Next(1, height));
                var linerLine = new LinearLineSegment(position1, position1);
                var shapesPath = new SixLabors.Shapes.Path(linerLine);
                listPath.Add(shapesPath);
            }

            // 画图
            using (Image<Rgba32> image = new Image<Rgba32>(width, height))   // 画布大小
            {
                image.Mutate(imgProc =>
                {
                    // 画点 
                    var draw = imgProc.BackgroundColor(Rgba32.WhiteSmoke);

                    // 噪点
                    foreach (var item in listPath)
                    {
                        var r = rnd.Next(0, 255);
                        var g = rnd.Next(0, 255);
                        var b = rnd.Next(0, 255);
                        // 随机颜色
                        var color = new Rgba32((byte)r, (byte)g, (byte)b);

                        draw
                            .Draw(
                                 Pens.Dot(color, rnd.Next(1, 3)),   // 大小
                                 new PathCollection(item)  // 坐标集合
                             );
                    }

                    // 噪线
                    for (int i = 0; i < 10; i++)
                    {
                        var r = rnd.Next(0, 255);
                        var g = rnd.Next(0, 255);
                        var b = rnd.Next(0, 255);
                        // 随机颜色
                        var color = new Rgba32((byte)r, (byte)g, (byte)b);

                        var p1 = new Vector2(rnd.Next(0, width), rnd.Next(0, height));
                        var p2 = new Vector2(rnd.Next(0, width), rnd.Next(0, height));
                        var p3 = new Vector2(rnd.Next(0, width), rnd.Next(0, height));
                        var p4 = new Vector2(rnd.Next(0, width), rnd.Next(0, height));
                        draw.DrawBeziers(color, 1, p1, p2, p3, p4);
                    }

                    // 逐个画字
                    for (int i = 0; i < codeLength; i++)
                    {
                        // 当前的要输出的字
                        var currentChar = code.Substring(i, 1);

                        // 文字坐标
                        var textXY = new Vector2();
                        var maxXX = prevTextX + (maxX - size);
                        textXY.X = new Random().Next(prevTextX, maxXX);
                        textXY.Y = new Random().Next(0, height - size);

                        prevTextX = Convert.ToInt32(Math.Floor(textXY.X)) + size;

                        // 画字
                        imgProc.DrawText(
                           currentChar,   // 文字内容
                           font,
                           i % 2 > 0 ? Rgba32.Blue : Rgba32.Black,
                           textXY);
                    }
                });
                image.SaveAsGif(saveStream);
            }

            return code;
        }

        static string RandomCode(Random rnd, int length)
        {
            StringBuilder sb = new StringBuilder();
            //验证码的字符集，去掉了一些容易混淆的字符 
            char[] character = { '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'd', 'e', 'f', 'g', 'h', 'k', 'm', 'n', 'r', 'x', 'y', 'A', 'B', 'D', 'E', 'F', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y' };
            //生成验证码字符串 
            for (int i = 0; i < length; i++)
            {
                sb.Append(character[rnd.Next(character.Length)]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// base64转为字节数组
        /// </summary>
        /// <param name="base64ImgString"></param>
        /// <returns></returns>
        public static byte[] FromBase64(string base64ImgString)
        {
            if (base64ImgString.IsNullOrWhiteSpace())
                return null;
            base64ImgString = Regex.Replace(base64ImgString, "^data:image/\\w+;base64,", "");
            return Convert.FromBase64String(base64ImgString);
        }

        /// <summary>
        /// 转换为base64字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToBase64(byte[] data)
        {
            return Convert.ToBase64String(data);
        }
        /// <summary>
        /// 图片压缩
        /// </summary>
        /// <param name="inputStream">输入图片</param>
        /// <param name="outputStream">输出图片</param>
        /// <param name="width">压缩宽度</param>
        /// <param name="height">压缩高度</param>
        /// <param name="imgFormat">图片格式</param>
        /// <param name="resizeMode">压缩模式</param>
        public static void Resize(Stream inputStream, Stream outputStream, int width, int height, ImgFormat imgFormat, ResizeMode resizeMode = ResizeMode.Max)
        {
            using (var image = Image.Load(inputStream))
            {
                if (width > image.Width)
                    width = image.Width;
                if (height > image.Height)
                    height = image.Height;
                image.Mutate(imgProc =>
                {
                    imgProc.Resize(new ResizeOptions
                    {
                        Size = new SixLabors.Primitives.Size { Width = width, Height = height },
                        Mode = resizeMode
                    });
                });

                switch (imgFormat)
                {
                    case ImgFormat.Bpm: image.SaveAsBmp(outputStream); break;
                    case ImgFormat.Gif: image.SaveAsGif(outputStream); break;
                    case ImgFormat.Jpeg: image.SaveAsJpeg(outputStream); break;
                    case ImgFormat.Png: image.SaveAsPng(outputStream); break;
                    default: throw new ArgumentException("error image format.");
                }
            }
        }
    }

    public enum ImgFormat
    {
        Jpeg,
        Bpm,
        Gif,
        Png
    }
}