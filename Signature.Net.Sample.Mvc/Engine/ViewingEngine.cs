using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml.Linq;
using Aspose.Cells;
using Aspose.Cells.Rendering;
using Aspose.Pdf.Devices;
using Aspose.Slides;
using Aspose.Words.Rendering;
using MVCDemo;
using Signature.Net.Sample.Mvc.Models;

namespace Signature.Net.Sample.Mvc.Engine
{
    public interface IViewingEngine
    {
        DocumentType GetDocumentType(string fileNameExtension);
        Image ResizeImage(Image sourceImage, int resultWidth);
        DocumentDescription GetPageDescriptions(
            string storagePath,
            string path,
            int quality, int width);

        byte[] GetDocumentPageImage(string appDataPath, string path, int? width, int? quality, int pageIndex);
    }

    public enum DocumentType
    {
        Pdf,
        Words,
        Slides,
        Cells
    };

    public class ViewingEngine : IViewingEngine
    {
        private const int DefaultQuality = 90;
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
            int resultHeight = (int) (resultWidth/(double) sourceImage.Width*sourceImage.Height);
            Bitmap resultImage = new Bitmap(resultWidth, resultHeight, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(resultImage))
            {
                graphics.Clear(Color.White);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(sourceImage, 0, 0, resultWidth, resultHeight);
            }
            return resultImage;
        }


        public DocumentDescription GetPageDescriptions(
            string storagePath,
            string path,
            int quality, int width)
        {
            string fileNameExtension = Path.GetExtension(path).TrimStart('.');
            fileNameExtension = fileNameExtension.ToLower();
            DocumentDescription documentDescription = new DocumentDescription();
            string fullPathToDocument = Path.Combine(storagePath, path);
            int pageCount = 1;
            PageDescription[] pageDescs = null;
            int maxPageHeight = 0;
            int widthForMaxHeight = 0;
            int pageWidth, pageHeight;
            bool isFirstPass = true;
            DocumentType documentType = GetDocumentType(fileNameExtension);
            switch (documentType)
            {
                case DocumentType.Pdf:
                    Aspose.Pdf.Document document = new Aspose.Pdf.Document(fullPathToDocument);
                    pageCount = document.Pages.Count;
                    const int asposePdfTrialPagesLimit = 4;
                    if (pageCount > asposePdfTrialPagesLimit)
                        pageCount = asposePdfTrialPagesLimit;
                    pageDescs = new PageDescription[pageCount];
                    for (int i = 0; i < pageDescs.Length; i++)
                    {
                        var page = document.Pages[i + 1];
                        pageWidth = (int) page.Rect.Width;
                        pageHeight = (int) page.Rect.Height;
                        pageDescs[i] = new PageDescription()
                        {
                            w = pageWidth,
                            h = pageHeight,
                            pageNumber = i
                        };
                        if (isFirstPass || pageHeight > maxPageHeight)
                        {
                            maxPageHeight = pageHeight;
                            widthForMaxHeight = pageWidth;
                            isFirstPass = false;
                        }
                    }
                    break;

                case DocumentType.Words:
                    Aspose.Words.Document wordsDocument = new Aspose.Words.Document(fullPathToDocument);
                    pageCount = wordsDocument.PageCount;
                    pageDescs = new PageDescription[pageCount];
                    for (int i = 0; i < pageDescs.Length; i++)
                    {
                        PageInfo page = wordsDocument.GetPageInfo(i);
                        SizeF rect = page.SizeInPoints;

                        pageWidth = (int) rect.Width;
                        pageHeight = (int) rect.Height;

                        pageDescs[i] = new PageDescription()
                        {
                            w = pageWidth,
                            h = pageHeight,
                            pageNumber = i
                        };
                        if (isFirstPass || pageHeight > maxPageHeight)
                        {
                            maxPageHeight = pageHeight;
                            widthForMaxHeight = pageWidth;
                            isFirstPass = false;
                        }
                    }
                    break;

                case DocumentType.Cells:
                    Workbook excelDocument = new Workbook(fullPathToDocument);
                    pageCount = excelDocument.Worksheets.Count;

                    pageDescs = new PageDescription[pageCount];
                    List<PageDescription> notEmptyPageList = new List<PageDescription>();
                    for (int i = 0; i < pageCount; i++)
                    {
                        Worksheet sheet = excelDocument.Worksheets[i];
                        ImageOrPrintOptions imgOptions = new ImageOrPrintOptions();
                        imgOptions.ImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                        imgOptions.OnePagePerSheet = true;
                        SheetRender sheetRenderer = new SheetRender(sheet, imgOptions);
                        if (sheetRenderer.PageCount > 0)
                        {
                            Size pageSize = sheetRenderer.GetPageSize(0);
                            pageWidth = pageSize.Width;
                            pageHeight = pageSize.Height;

                            PageDescription pageDesc = new PageDescription()
                            {
                                w = pageWidth,
                                h = pageHeight,
                                pageNumber = i
                            };
                            if (isFirstPass || pageHeight > maxPageHeight)
                            {
                                maxPageHeight = pageHeight;
                                widthForMaxHeight = pageWidth;
                                isFirstPass = false;
                            }

                            notEmptyPageList.Add(pageDesc);
                        }
                    }
                    pageDescs = notEmptyPageList.ToArray();
                    pageCount = pageDescs.Length;
                    break;

                case DocumentType.Slides:
                    Presentation powerPointDocument = new Presentation(fullPathToDocument);
                    pageCount = powerPointDocument.Slides.Count;

                    pageDescs = new PageDescription[pageCount];
                    SizeF slideSize = powerPointDocument.SlideSize.Size;
                    pageWidth = (int) slideSize.Width;
                    pageHeight = (int) slideSize.Height;
                    for (int i = 0; i < pageDescs.Length; i++)
                    {
                        pageDescs[i] = new PageDescription()
                        {
                            w = pageWidth,
                            h = pageHeight,
                            pageNumber = i
                        };
                        if (isFirstPass || pageHeight > maxPageHeight)
                        {
                            maxPageHeight = pageHeight;
                            widthForMaxHeight = pageWidth;
                            isFirstPass = false;
                        }
                    }
                    break;
            }
            documentDescription.pages = pageDescs.ToList();
            documentDescription.maxPageHeight = maxPageHeight;
            documentDescription.widthForMaxHeight = widthForMaxHeight;
            return documentDescription;
        }


        public byte[] GetDocumentPageImage(string appDataPath, string path, int? width, int? quality, int pageIndex)
        {
            string fullPathToDocument = Path.Combine(appDataPath, path);

            string fileNameExtension = Path.GetExtension(path).TrimStart('.');
            fileNameExtension = fileNameExtension.ToLower();
            int pageCount;
            const string mimeType = "image/jpeg";
            DocumentType documentType = GetDocumentType(fileNameExtension);
            switch (documentType)
            {
                case DocumentType.Pdf:
                    Aspose.Pdf.Document document = new Aspose.Pdf.Document(fullPathToDocument);
                    JpegDevice jpegDevice = new JpegDevice(quality ?? DefaultQuality);
                    pageCount = document.Pages.Count;
                    if (pageIndex < pageCount)
                    {
                        using (MemoryStream outputStream = new MemoryStream())
                        {
                            jpegDevice.Process(document.Pages[pageIndex + 1], outputStream);
                            return outputStream.ToArray();
                        }
                    }
                    else
                        return null;

                case DocumentType.Words:
                    Aspose.Words.Document wordsDocument = new Aspose.Words.Document(fullPathToDocument);
                    pageCount = wordsDocument.PageCount;
                    if (pageIndex < pageCount)
                    {
                        using (MemoryStream outputStream = new MemoryStream())
                        {
                            Aspose.Words.Saving.ImageSaveOptions saveOptions =
                                new Aspose.Words.Saving.ImageSaveOptions(Aspose.Words.SaveFormat.Jpeg)
                                {
                                    PageIndex = pageIndex,
                                    PageCount = 1,
                                    JpegQuality = quality ?? DefaultQuality
                                };

                            wordsDocument.Save(outputStream, saveOptions);
                            return outputStream.ToArray();
                        }
                    }
                    else
                        return null;

                case DocumentType.Cells:
                    Workbook excelDocument = new Workbook(fullPathToDocument);
                    pageCount = excelDocument.Worksheets.Count;
                    if (pageIndex >= pageCount)
                        return null;

                    Worksheet sheet = excelDocument.Worksheets[pageIndex];
                    ImageOrPrintOptions imgOptions = new ImageOrPrintOptions();
                    imgOptions.ImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                    imgOptions.OnePagePerSheet = true;
                    SheetRender sr = new SheetRender(sheet, imgOptions);
                    using (Bitmap bitmap = sr.ToImage(0))
                    {
                        using (MemoryStream outputStream = new MemoryStream())
                        {
                            bitmap.Save(outputStream, ImageFormat.Jpeg);
                            return outputStream.ToArray();
                        }
                    }

                case DocumentType.Slides:
                    Presentation powerPointDocument = new Presentation(fullPathToDocument);
                    pageCount = powerPointDocument.Slides.Count;
                    if (pageIndex >= pageCount)
                        return null;

                    ISlide slide = powerPointDocument.Slides[pageIndex];
                    SizeF slideSize = powerPointDocument.SlideSize.Size;

                    using (System.Drawing.Image image = slide.GetThumbnail(new Size((int)slideSize.Width, (int)slideSize.Height)))
                    {
                        using (System.Drawing.Image resizedImage = ResizeImage(image, width ?? image.Width))
                        {
                            using (MemoryStream outputStream = new MemoryStream())
                            {
                                resizedImage.Save(outputStream, ImageFormat.Jpeg);
                                return outputStream.ToArray();
                            }
                        }
                    }
            }
            string fileModificationDateTime = System.IO.File.GetLastWriteTimeUtc(fullPathToDocument).ToString("s").Replace(":", "_");

            string qualityWidth = String.Format("{0}@{1}x", quality, width);
            string pageFileName = String.Format("page_{0}.jpg", pageIndex);
            string pageImageFullPath = Path.Combine(appDataPath, "temp", "Cache", path, fileModificationDateTime, qualityWidth, pageFileName);
            if (System.IO.File.Exists(pageImageFullPath))
                return File.ReadAllBytes(pageImageFullPath);
            else
                return null;
        }
    }
}
