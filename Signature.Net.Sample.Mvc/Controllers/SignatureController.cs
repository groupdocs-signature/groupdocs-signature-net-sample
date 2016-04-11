using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.UI;
using Aspose.Pdf;
using Aspose.Pdf.Devices;
using Aspose.Words.Saving;
using GroupDocs.Signature.Config;
using GroupDocs.Signature.Handler;
using GroupDocs.Signature.Options;
using SaveOptions = Aspose.Pdf.SaveOptions;

namespace MVCDemo.Controllers
{
    public class SignatureController : Controller
    {
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
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //string serializedData = serializer.Serialize(result);
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
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //string serializedData = serializer.Serialize(result);
            return Json(result);
        }


        [HttpPost]
        public ActionResult ViewDocument(string path,
            int quality, int width)
        {
            string fileNameExtension = Path.GetExtension(path).TrimStart('.');
            fileNameExtension = fileNameExtension.ToLower();
            string fileType = fileNameExtension.Substring(0, 1).ToUpper() + fileNameExtension.Substring(1);

            string appDataPath = Server.MapPath("~/App_Data/");
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
                    JpegDevice jpegDevice = new JpegDevice(quality);
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
                        pageDescs[i] = new PageDescription() {
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
                path = path,
                docType = "Pdf",
                fileType = fileNameExtension,
                url = "http://localhost:13469/gd-signature/signature2/GetFileHandler?path=sample.pdf\u0026getPdf=false\u0026useHtmlBasedEngine=false\u0026supportPageRotation=false",
                pdfDownloadUrl = "http://localhost:13469/gd-signature/signature2/GetFileHandler?path=sample.pdf\u0026getPdf=true\u0026useHtmlBasedEngine=false\u0026supportPageRotation=false",
                name = path,
                imageUrls= pageImageUrls,
                lic = true,
                pdfPrintUrl = "http://localhost:13469/gd-signature/signature2/GetPdfWithPrintDialogHandler?path=sample.pdf\u0026useHtmlBasedEngine=false\u0026supportPageRotation=false",
                pageHtml = (object)null,
                pageCss = (object)null,
                documentDescription,
                urlForResourcesInHtml = (object)null,
                sharedCss = (object)null,
                success = true
            };
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
            string appDataPath = Server.MapPath("~/App_Data/");
            string fullPathToDocument = Path.Combine(appDataPath, path);

            string fileNameExtension = Path.GetExtension(path).TrimStart('.');
            fileNameExtension = fileNameExtension.ToLower();
            const string mimeType = "image/jpeg";
            switch (fileNameExtension)
            {
                case "pdf":
                    Aspose.Pdf.Document document = new Document(fullPathToDocument);
                    JpegDevice jpegDevice = new JpegDevice(quality);
                    int pageCount = document.Pages.Count;
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

        public class SignatureField
        {
            public int fieldType { get; set; }
            public string data { get; set; }
            public string documentGuid { get; set; }
            public location[] locations { get; set; }

            public class location
            {
                public int page { get; set; }
                public double locationX { get; set; }
                public double locationY { get; set; }
                public int locationWidth { get; set; }
                public int locationHeight { get; set; }
                public string fontName { get; set; }
                public int fontSize { get; set; }
                public int fontColor { get; set; }
                public bool? fontBold { get; set; }
                public bool? fontItalic { get; set; }
                public bool? fontUnderline { get; set; }
                public int alignment { get; set; }
                public string id { get; set; }
            };
        }

        public ActionResult PublicSignDocument(string documentGuid, string documentId, string name, SignatureField[] fields)
        {
            if (fields.Length == 0)
                return new EmptyResult();
            string data = fields[0].data;
            Regex removeUnclosedLinkTagRegex = new Regex(@"<link[^>]*>");
            string svgData = removeUnclosedLinkTagRegex.Replace(data, String.Empty);
            XDocument root = XDocument.Parse(svgData);
            string signatureText = String.Empty;
            IEnumerable<XElement> textElements;
            textElements = root.Descendants("{http://www.w3.org/2000/svg}text");
            foreach (var textElement in textElements)
            {
                signatureText += textElement.Value;
            }

            //string data = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"100%\" height=\"100%\" viewbox=\"0 0 233 82\" preserveaspectratio=\"none\"><text font-family=\"Tangerine\" font-size=\"60px\" fill=\"#0036D9\" y=\"50%\" x=\"50%\" dy=\"0.3em\" text-anchor=\"middle\">Anonymous</text><defs><link href=\"http://fonts.googleapis.com/css?family=Tangerine\" type=\"text/css\" rel=\"stylesheet\" xmlns=\"http://www.w3.org/1999/xhtml\"><style type=\"text/css\">@import url(http://fonts.googleapis.com/css?family=Tangerine)</style></defs></svg>";
            //{ "documentId":"","name":"a b","waterMarkText":"","waterMarkImage":"","fields":[{"fieldType":1,"data":"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"100%\" height=\"100%\" viewbox=\"0 0 233 82\" preserveaspectratio=\"none\"><text font-family=\"Tangerine\" font-size=\"60px\" fill=\"#0036D9\" y=\"50%\" x=\"50%\" dy=\"0.3em\" text-anchor=\"middle\">Anonymous</text><defs><link href=\"http://fonts.googleapis.com/css?family=Tangerine\" type=\"text/css\" rel=\"stylesheet\" xmlns=\"http://www.w3.org/1999/xhtml\"><style type=\"text/css\">@import url(http://fonts.googleapis.com/css?family=Tangerine)</style></defs></svg>","locations":[{"page":1,"locationX":0.4,"locationY":0.3,"locationWidth":150,"locationHeight":50,"fontName":null,"fontSize":null,"fontColor":null,"fontBold":null,"fontItalic":null,"fontUnderline":null,"alignment":0,"id":"ff4dd6a4a44ecd682a4be3a19a801e6f"}],"id":"1c9b463ac3c1e9ebaf51e34ea352de3a"}],"documentGuid":"candy.pdf","recipientGuid":"71d1f3ef88a5d7fe32f4c46588a69887","email":"a@b.com"}
            SignDocument(documentGuid, signatureText);
            return null;
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

        private void SignDocument(string fileName, string signatureText)
        {
            string rootPath = Server.MapPath("~/App_Data");
            string storagePath = Path.Combine(rootPath, @"Storage");
            string outputPath = Path.Combine(rootPath, @"Output");
            string imagesPath = Path.Combine(rootPath, @"Images");

            // setup a configuration
            SignatureConfig config = new SignatureConfig()
            {
                StoragePath = storagePath,
                OutputPath = outputPath,
                ImagesPath = imagesPath
            };

            // instantiating the handler
            SignatureHandler handler = new SignatureHandler(config);

            // Set a license if you have one
            handler.SetLicense(@"GroupDocs.Signature3.lic");

            // setup PDF image signature options
            PDFSignTextOptions signOptions = new PDFSignTextOptions(signatureText);
            signOptions.DocumentPageNumber = 1;
            signOptions.Left = 100;
            signOptions.Top = 0;
            signOptions.SignAllPages = true;

            GroupDocs.Signature.Options.SaveOptions saveOptions = new GroupDocs.Signature.Options.SaveOptions(OutputType.String);
            // sign the document
            handler.Sign<string>(fileName, signOptions, saveOptions);
        }

        #endregion
    }
}
