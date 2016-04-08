using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.UI;
using Aspose.Pdf;
using Aspose.Pdf.Devices;
using Aspose.Words.Saving;
using Groupdocs.Data.Signature;
using Groupdocs.Web.UI.Signature.Services;
using MVCDemo.Models;
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
            string fileModificationDateTime =System.IO.File.GetLastWriteTimeUtc(fullPathToDocument).ToString("s").Replace(":", "_");

            string qualityWidth = String.Format("{0}@{1}x", quality, width);
            string pageFileName = String.Format("page_{0}.jpg", pageIndex);
            string pageImageFullPath = Path.Combine(appDataPath, "temp", "Cache", path, fileModificationDateTime, qualityWidth, pageFileName);
            if (System.IO.File.Exists(pageImageFullPath))
                return File(pageImageFullPath, "image/jpeg");
            else
                return new EmptyResult();
        }


        public ActionResult PublicSignDocument(string documentGuid, string documentId)
        {
            return null;
        }

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
    }
}
