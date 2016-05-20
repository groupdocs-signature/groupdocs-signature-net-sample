using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Signature.Net.Sample.Mvc.Engine
{
    internal interface IViewingEngine
    {
        DocumentType GetDocumentType(string fileNameExtension);
        Image ResizeImage(Image sourceImage, int resultWidth);
    }

    public enum DocumentType
    {
        Pdf,
        Words,
        Slides,
        Cells
    };

    internal class ViewingEngine : IViewingEngine
    {
        public DocumentType GetDocumentType(string fileNameExtension)
        {
            switch (fileNameExtension)
            {
                case "pdf":
                    return DocumentType.Pdf;

                case "doc":
                case "docx":
                case "rtf":
                case "docm":
                case "dotm":
                case "dotx":
                    return DocumentType.Words;

                case "xls":
                case "csv":
                case "xlsx":
                case "xlsm":
                case "xlsb":
                    return DocumentType.Cells;

                case "ppt":
                case "pps":
                case "pptx":
                    return DocumentType.Slides;

                default:
                    throw new ArgumentException("Unknown document type");
            }
        }

        public Image ResizeImage(Image sourceImage, int resultWidth)
        {
            int resultHeight = (int)(resultWidth / (double)sourceImage.Width * sourceImage.Height);
            Bitmap resultImage = new Bitmap(resultWidth, resultHeight, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(resultImage))
            {
                graphics.Clear(Color.White);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(sourceImage, 0, 0, resultWidth, resultHeight);
            }
            return resultImage;
        }
    }
}