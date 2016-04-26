using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Aspose.Cells;
using Aspose.Cells.Rendering;
using Aspose.Pdf;
using Aspose.Pdf.Devices;
using Aspose.Slides;

using MVCDemo;
using Signature.Net.Sample.Mvc.Models;
using PageInfo = Aspose.Words.Rendering.PageInfo;
using Signature.Net.Sample.Mvc.Engine;
using Rectangle = Aspose.Pdf.Rectangle;

namespace Signature.Net.Sample.Mvc.Controllers
{
    public class SignatureController : Controller
    {
        private const string AppDataVirtualPath = "~/App_Data/";

        [HttpPost]
        public ActionResult PublicGetDocument(string documentGuid, string recipientGuid)
        {
            var result = new
            {
                status = "Ok",
                result = new
                {
                    document = new
                    {
                        guid = documentGuid,
                        name = documentGuid,
                        signedName = documentGuid + "_signed1.pdf",
                        signedFromAll = false,
                        recipient = new
                        {
                            id = 0,
                            guid = "71d1f3ef88a5d7fe32f4c46588a69887",
                            documentGuid,
                            firstName = "",
                            lastName = "",
                            email = "",
                            signed = false
                        }
                    }
                }
            };
            return Json(result);
        }

        [HttpPost]
        public ActionResult PublicGetDocumentFields()
        {
            var result = new
            {
                status = "Ok",
                result = new
                {
                    fields = new[]
                    {
                        new
                        {
                            id = "1c9b463ac3c1e9ebaf51e34ea352de3a",
                            name = "Signature1",
                            fieldType = 1,
                            mandatory = true,
                            data = (object) null,
                            minGraphSizeW = 0,
                            minGraphSizeH = 0,
                            recipientGuid = "71d1f3ef88a5d7fe32f4c46588a69887",
                            locations = new[]
                            {
                                new
                                {
                                    page = 1,
                                    locationHeight = 50.0,
                                    locationWidth = 150.0,
                                    locationX = 0.4,
                                    locationY = 0.3,
                                    fontBold = (object) null,
                                    fontColor = (object) null,
                                    fontItalic = (object) null,
                                    fontName = (object) null,
                                    fontSize = (object) null,
                                    fontUnderline = (object) null,
                                    id = "ff4dd6a4a44ecd682a4be3a19a801e6f",
                                    align = 0
                                }
                            },
                            lockDuringSign = false,
                            acceptableValues = (object) null,
                            tooltip = (object) null,
                            settings = (object) null,
                            groupName = (object) null,
                            guidanceText = (object) null,
                            regularExpression = (object) null,
                            defaultValue = ""
                        }
                    }
                }

            };
            return Json(result);
        }


        [HttpPost]
        public ActionResult ViewDocument(string path,
            int quality, int width)
        {
            string fileNameExtension = Path.GetExtension(path).TrimStart('.');
            fileNameExtension = fileNameExtension.ToLower();
            string fileType = fileNameExtension.Substring(0, 1).ToUpper() + fileNameExtension.Substring(1);

            string appDataPath = Server.MapPath(AppDataVirtualPath);
            string fullPathToDocument = Path.Combine(appDataPath, path);
            int pageCount= 1;
            PageDescription[] pageDescs = null;
            int maxPageHeight = 0, widthForMaxHeight = 0;
            int pageWidth, pageHeight;
            bool isFirstPass = true;
            switch (fileNameExtension)
            {
                case "pdf":
                    Aspose.Pdf.Document document = new Document(fullPathToDocument);
                    pageCount = document.Pages.Count;
                    const int asposePdfTrialPagesLimit = 4;
                    if (pageCount > asposePdfTrialPagesLimit)
                        pageCount = asposePdfTrialPagesLimit;
                    pageDescs = new PageDescription[pageCount];
                    for (int i = 0; i < pageDescs.Length; i++)
                    {
                        var page = document.Pages[i + 1];
                        pageWidth = (int)page.Rect.Width;
                        pageHeight = (int)page.Rect.Height;
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

                case "doc":
                case "docx":
                case "rtf":
                    Aspose.Words.Document wordsDocument = new Aspose.Words.Document(fullPathToDocument);
                    pageCount = wordsDocument.PageCount;
                    pageDescs = new PageDescription[pageCount];
                    for (int i = 0; i < pageDescs.Length; i++)
                    {
                        PageInfo page = wordsDocument.GetPageInfo(i);
                        SizeF rect = page.SizeInPoints;

                        pageWidth = (int)rect.Width;
                        pageHeight = (int)rect.Height;

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

                case "xls":
                case "xlsx":
                    Workbook excelDocument = new Workbook(fullPathToDocument);
                    pageCount = excelDocument.Worksheets.Count;

                    pageDescs = new PageDescription[pageCount];
                    List<PageDescription> notEmptyPageList =  new List<PageDescription>();
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

                case "ppt":
                case "pptx":
                    Presentation powerPointDocument = new Presentation(fullPathToDocument);
                    pageCount = powerPointDocument.Slides.Count;

                    pageDescs = new PageDescription[pageCount];
                    SizeF slideSize = powerPointDocument.SlideSize.Size;
                    pageWidth = (int)slideSize.Width;
                    pageHeight = (int)slideSize.Height;
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

            string[] pageImageUrls = GetImageUrls(path, 0, pageCount, width, quality);
            string documentDescription = new JavaScriptSerializer().Serialize(new
            {
                pages = pageDescs,
                maxPageHeight,
                widthForMaxHeight
            });
            var result = new
            {
                path,
                docType = "Pdf",
                fileType = fileNameExtension,
                url = (string)null,
                pdfDownloadUrl = (string)null,
                name = path,
                imageUrls= pageImageUrls,
                lic = true,
                pdfPrintUrl = (string)null,
                pageHtml = (object)null,
                pageCss = (object)null,
                documentDescription,
                urlForResourcesInHtml = (object)null,
                sharedCss = (object)null,
                success = true
            };
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetImageUrls(string path,
                                         string dimension,
                                         int firstPage,
                                         int pageCount,
                                         int quality)
        {
            int width = Int32.Parse(dimension.Substring(0, dimension.Length - 1));
            string[] pageImageUrls = GetImageUrls(path, 0, pageCount, width, quality);
            var result = new {imageUrls = pageImageUrls, success = true };
            return Json(result);
        }


        public ActionResult GetSignatureBackgroundSvg()
        {
            return File(Server.MapPath("~/gd-signature/signature2/resource/signature.svg"), "image/svg+xml");
        }

        public ActionResult GetStampBackgroundSvg()
        {
            return File(Server.MapPath("~/gd-signature/signature2/resource/stamp.png"), "image/png");
        }


        public ActionResult GetDocumentPageImage(string path, int width, int quality, int pageIndex)
        {
            string appDataPath = Server.MapPath(AppDataVirtualPath);
            string fullPathToDocument = Path.Combine(appDataPath, path);

            string fileNameExtension = Path.GetExtension(path).TrimStart('.');
            fileNameExtension = fileNameExtension.ToLower();
            int pageCount;
            const string mimeType = "image/jpeg";
            switch (fileNameExtension)
            {
                case "pdf":
                    Aspose.Pdf.Document document = new Document(fullPathToDocument);
                    JpegDevice jpegDevice = new JpegDevice(quality);
                    pageCount = document.Pages.Count;
                    if (pageIndex < pageCount)
                    {
                        using (MemoryStream outputStream = new MemoryStream())
                        {
                            jpegDevice.Process(document.Pages[pageIndex + 1], outputStream);
                            return File(outputStream.ToArray(), mimeType);
                        }
                    }
                    else
                        return new EmptyResult();

                case "doc":
                case "docx":
                case "rtf":
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
                                    JpegQuality = quality
                                };

                            wordsDocument.Save(outputStream, saveOptions);
                            return File(outputStream.ToArray(), mimeType);
                        }
                    }
                    else
                        return new EmptyResult();

                case "xls":
                    Workbook excelDocument = new Workbook(fullPathToDocument);
                    pageCount = excelDocument.Worksheets.Count;
                    if (pageIndex >= pageCount)
                        return new EmptyResult();

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
                            return File(outputStream.ToArray(), mimeType);
                        }
                    }

                case "ppt":
                case "pptx":
                    Presentation powerPointDocument = new Presentation(fullPathToDocument);
                    pageCount = powerPointDocument.Slides.Count;
                    if (pageIndex >= pageCount)
                        return new EmptyResult();

                    ISlide slide = powerPointDocument.Slides[pageIndex];
                    SizeF slideSize = powerPointDocument.SlideSize.Size;
                    ViewingEngine viewingEngine = new ViewingEngine();
                    using (System.Drawing.Image image = slide.GetThumbnail(new Size((int)slideSize.Width, (int)slideSize.Height)))
                    {
                        using (System.Drawing.Image resizedImage = viewingEngine.ResizeImage(image, width))
                        {
                            using (MemoryStream outputStream = new MemoryStream())
                            {
                                resizedImage.Save(outputStream, ImageFormat.Jpeg);
                                return File(outputStream.ToArray(), mimeType);
                            }
                        }
                    }
            }
            string fileModificationDateTime = System.IO.File.GetLastWriteTimeUtc(fullPathToDocument).ToString("s").Replace(":", "_");

            string qualityWidth = String.Format("{0}@{1}x", quality, width);
            string pageFileName = String.Format("page_{0}.jpg", pageIndex);
            string pageImageFullPath = Path.Combine(appDataPath, "temp", "Cache", path, fileModificationDateTime, qualityWidth, pageFileName);
            if (System.IO.File.Exists(pageImageFullPath))
                return File(pageImageFullPath, "image/jpeg");
            else
                return new EmptyResult();
        }


        public ActionResult PublicSignDocument(string documentGuid,
            string documentId,
            string name,
            SignatureField[] fields)
        {
            if (fields == null || fields.Length == 0)
                return new EmptyResult();
            SignatureField field = fields[0];
            string data = field.Data;
            string signatureText = String.Empty;
            byte[] imageBytes = null;
            const string dataUrlPrefix = "data:image/png;base64,";
            if (data.StartsWith(dataUrlPrefix))
            {
                string base64Data = data.Substring(dataUrlPrefix.Length);
                imageBytes = Convert.FromBase64String(base64Data);
            }
            else
            {
                Regex removeUnclosedLinkTagRegex = new Regex(@"<link[^>]*>");
                string svgData = removeUnclosedLinkTagRegex.Replace(data, String.Empty);
                XDocument root = XDocument.Parse(svgData);
                IEnumerable<XElement> textElements;
                textElements = root.Descendants("{http://www.w3.org/2000/svg}text");
                foreach (var textElement in textElements)
                {
                    signatureText += textElement.Value;
                }

            }
            // request structure:
            //{ "documentId":"","name":"a b","waterMarkText":"","waterMarkImage":"","fields":[{"fieldType":1,"data":"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"100%\" height=\"100%\" viewbox=\"0 0 233 82\" preserveaspectratio=\"none\"><text font-family=\"Tangerine\" font-size=\"60px\" fill=\"#0036D9\" y=\"50%\" x=\"50%\" dy=\"0.3em\" text-anchor=\"middle\">Anonymous</text><defs><link href=\"http://fonts.googleapis.com/css?family=Tangerine\" type=\"text/css\" rel=\"stylesheet\" xmlns=\"http://www.w3.org/1999/xhtml\"><style type=\"text/css\">@import url(http://fonts.googleapis.com/css?family=Tangerine)</style></defs></svg>","locations":[{"page":1,"locationX":0.4,"locationY":0.3,"locationWidth":150,"locationHeight":50,"fontName":null,"fontSize":null,"fontColor":null,"fontBold":null,"fontItalic":null,"fontUnderline":null,"alignment":0,"id":"ff4dd6a4a44ecd682a4be3a19a801e6f"}],"id":"1c9b463ac3c1e9ebaf51e34ea352de3a"}],"documentGuid":"candy.pdf","recipientGuid":"71d1f3ef88a5d7fe32f4c46588a69887","email":"a@b.com"}

            string path = documentGuid;
            string appDataPath = Server.MapPath(AppDataVirtualPath);
            string fullPathToDocument = Path.Combine(appDataPath, path);

            string fileNameExtension = Path.GetExtension(path).TrimStart('.');
            fileNameExtension = fileNameExtension.ToLower();
            int pageWidth = 0, pageHeight = 0;
            int signatureColumnNum = 0, signatureRowNum = 0;
            SignatureField.Location location = field.Locations[0];
            const double scaleForSizes = 2.083;
            int signatureWidth = (int)(location.LocationWidth / scaleForSizes);
            int signatureHeight = (int)(location.LocationHeight / scaleForSizes);
            int pageNumber = location.Page;
            switch (fileNameExtension)
            {
                case "pdf":
                    using (Aspose.Pdf.Document document = new Document(fullPathToDocument))
                    {
                        Rectangle pageRect = document.Pages[pageNumber].Rect;
                        pageWidth = (int)pageRect.Width;
                        pageHeight = (int)pageRect.Height;
                    }
                    break;

                case "doc":
                case "docx":
                case "rtf":
                    Aspose.Words.Document wordsDocument = new Aspose.Words.Document(fullPathToDocument);
                    pageNumber = location.Page - 1;
                    PageInfo page = wordsDocument.GetPageInfo(pageNumber);
                    SizeF rect = page.SizeInPoints;
                    pageWidth = (int)rect.Width;
                    pageHeight = (int)rect.Height;
                    pageNumber = location.Page;
                    break;

                case "xls":
                case "xlsx":
                    Workbook excelDocument = new Workbook(fullPathToDocument);
                    pageNumber = location.Page - 1;
                    Worksheet sheet = excelDocument.Worksheets[pageNumber];
                    ImageOrPrintOptions imgOptions = new ImageOrPrintOptions();
                    imgOptions.ImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                    imgOptions.OnePagePerSheet = true;
                    pageNumber = location.Page;

                    CellDrawFindingNearest cellsDrawFindingNearest = new CellDrawFindingNearest();
                    imgOptions.DrawObjectEventHandler = cellsDrawFindingNearest;
                    SheetRender sheetRenderer = new SheetRender(sheet, imgOptions);

                    if (sheetRenderer.PageCount > 0)
                    {
                        Size pageSize = sheetRenderer.GetPageSize(0);
                        pageWidth = pageSize.Width;
                        pageHeight = pageSize.Height;
                        int absoluteLocationLeft = (int)(pageWidth * location.LocationX);
                        int absoluteLocationTop = (int)(pageHeight * location.LocationY);
                        cellsDrawFindingNearest.SetPositions(absoluteLocationLeft, absoluteLocationTop);
                        using (MemoryStream tempStream = new MemoryStream())
                        {
                            sheetRenderer.ToImage(0, tempStream);
                        }
                        signatureColumnNum = cellsDrawFindingNearest.Column;
                        signatureRowNum = cellsDrawFindingNearest.Row;
                    }
                    break;

                case "ppt":
                case "pptx":
                    Presentation powerPointDocument = new Presentation(fullPathToDocument);
                    pageNumber = location.Page - 1;
                    SizeF slideSize = powerPointDocument.SlideSize.Size;
                    pageWidth = (int)slideSize.Width;
                    pageHeight = (int)slideSize.Height;
                    break;
            }

            string rootPath = Server.MapPath("~/App_Data");

            SigningEngine signingEngine = new SigningEngine();
            string outputFilePath;

            MemoryStream imageStream = null;
            try
            {
                if (imageBytes == null)
                {
                    outputFilePath = signingEngine.SignDocumentWithText(rootPath,
                        documentGuid,
                        signatureText,
                        pageNumber,
                        (int) (pageWidth*location.LocationX),
                        (int) (pageHeight*location.LocationY),
                        signatureWidth,
                        signatureHeight,
                        signatureColumnNum, signatureRowNum);
                }
                else
                {
                    imageStream = new MemoryStream(imageBytes);
                    outputFilePath = signingEngine.SignDocumentWithImage(rootPath,
                        documentGuid,
                        imageStream,
                        pageNumber,
                        (int)(pageWidth * location.LocationX),
                        (int)(pageHeight * location.LocationY),
                        signatureWidth,
                        signatureHeight,
                        signatureColumnNum, signatureRowNum);
                }
            }
            finally
            {
                if (imageStream != null)
                    imageStream.Dispose();
            }
            string relativeOutputFileName = Path.Combine("Output", Path.GetFileName(outputFilePath));
            var resultData = new
            {
                status = "Ok",
                result = new
                {
                    document = new
                    {
                        guid = relativeOutputFileName,
                        name = relativeOutputFileName,
                        signedName = relativeOutputFileName,
                        signedFromAll = true,
                        recipients = new[]
                        {
                            new
                            {
                                id = 0,
                                guid = "71d1f3ef88a5d7fe32f4c46588a69887",
                                documentGuid = "cea6784811dc54d7feac5fcb5ef8817a",
                                firstName = "dummy",
                                lastName = "dummy",
                                email = "dummy@dummy.pp",
                                signed = true
                            }
                        }
                    }
                }
            };
  
            return Json(resultData);
        }

        #region Private methods

        private string[] GetImageUrls(string path, int startingPageNumber, int pageCount, int? pageWidth, int? quality)
        {
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary()
            {
                {"path", path},
                {"width", pageWidth},
                {"quality", quality},
            };

            string[] pageUrls = new string[pageCount];
            for (int i = 0; i < pageCount; i++)
            {
                routeValueDictionary["pageIndex"] = startingPageNumber + i;
                pageUrls[i] = ConvertUrlToAbsolute(new UrlHelper(Request.RequestContext).Action("GetDocumentPageImage", routeValueDictionary));
            }
            return pageUrls;
        }


        private string ConvertUrlToAbsolute(string inputUrl)
        {
            HttpRequestBase request = Request;
            string applicationHost = String.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Host,
                                                    request.Url.Port == 80 ? String.Empty : ":" + request.Url.Port);

            if (applicationHost.EndsWith("/"))
            {
                applicationHost = applicationHost.Substring(0, applicationHost.Length - 1);
            }
            string result = string.Format("{0}{1}", applicationHost, inputUrl);
            return result;
        }
        
        #endregion
    }
}
