using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Signature.Net.Sample.Mvc.Engine
{
    internal class ViewingEngine
    {
        internal System.Drawing.Image ResizeImage(System.Drawing.Image sourceImage,
            int resultWidth)
        {
            int resultHeight = (int)(resultWidth / (double)sourceImage.Width * sourceImage.Height);
            Bitmap resultImage = new Bitmap(resultWidth, resultHeight, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(resultImage))
            {
                graphics.Clear(System.Drawing.Color.White);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(sourceImage, 0, 0, resultWidth, resultHeight);
            }
            return resultImage;
        }
    }
}