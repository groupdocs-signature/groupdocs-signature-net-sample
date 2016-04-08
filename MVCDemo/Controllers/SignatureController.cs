using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
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
        public ActionResult PublicGetDocument()
        {
            var result = new
            {
                status = "Ok",
                result = new
                {
                    document = new
                    {
                        guid = "cea6784811dc54d7feac5fcb5ef8817a",
                        name = "sample.pdf",
                        signedName = "sample_signed1.pdf",
                        signedFromAll = false,
                        recipient = new
                        {
                            id = 0,
                            guid = "71d1f3ef88a5d7fe32f4c46588a69887",
                            documentGuid = "cea6784811dc54d7feac5fcb5ef8817a",
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
            fileNameExtension = fileNameExtension.Substring(0, 1).ToUpper() + fileNameExtension.Substring(1);
            var result = new
            {
                path = path,
                docType = "Pdf",
                fileType = fileNameExtension,
                url = "http://localhost:13469/gd-signature/signature2/GetFileHandler?path=sample.pdf\u0026getPdf=false\u0026useHtmlBasedEngine=false\u0026supportPageRotation=false",
                pdfDownloadUrl = "http://localhost:13469/gd-signature/signature2/GetFileHandler?path=sample.pdf\u0026getPdf=true\u0026useHtmlBasedEngine=false\u0026supportPageRotation=false",
                name = "sample.pdf",
                imageUrls= new[] { "http://localhost:13469/gd-signature/signature2/GetDocumentPageImageHandler?path=sample.pdf\u0026width=395\u0026quality=80\u0026usePdf=False\u0026useHtmlBasedEngine=False\u0026rotate=False\u0026pageIndex=0",
                "http://localhost:13469/gd-signature/signature2/GetDocumentPageImageHandler?path=sample.pdf\u0026width=395\u0026quality=80\u0026usePdf=False\u0026useHtmlBasedEngine=False\u0026rotate=False\u0026pageIndex=1",
                "http://localhost:13469/gd-signature/signature2/GetDocumentPageImageHandler?path=sample.pdf\u0026width=395\u0026quality=80\u0026usePdf=False\u0026useHtmlBasedEngine=False\u0026rotate=False\u0026pageIndex=2"},
                lic = true,
                pdfPrintUrl = "http://localhost:13469/gd-signature/signature2/GetPdfWithPrintDialogHandler?path=sample.pdf\u0026useHtmlBasedEngine=false\u0026supportPageRotation=false",
                pageHtml = (object)null,
                pageCss = (object)null,
                documentDescription = "{\"pages\":[{\"w\":612,\"h\":936,\"number\":1},{\"w\":612,\"h\":936,\"number\":2},{\"w\":612,\"h\":936,\"number\":3},{\"w\":612,\"h\":936,\"number\":4},{\"w\":612,\"h\":936,\"number\":5},{\"w\":612,\"h\":936,\"number\":6},{\"w\":612,\"h\":936,\"number\":7},{\"w\":612,\"h\":936,\"number\":8},{\"w\":612,\"h\":936,\"number\":9},{\"w\":612,\"h\":936,\"number\":10},{\"w\":612,\"h\":936,\"number\":11},{\"w\":612,\"h\":936,\"number\":12},{\"w\":612,\"h\":936,\"number\":13},{\"w\":612,\"h\":936,\"number\":14},{\"w\":612,\"h\":936,\"number\":15},{\"w\":612,\"h\":936,\"number\":16},{\"w\":612,\"h\":936,\"number\":17},{\"w\":612,\"h\":936,\"number\":18},{\"w\":612,\"h\":936,\"number\":19},{\"w\":612,\"h\":936,\"number\":20},{\"w\":612,\"h\":936,\"number\":21},{\"w\":612,\"h\":936,\"number\":22},{\"w\":612,\"h\":936,\"number\":23},{\"w\":612,\"h\":936,\"number\":24},{\"w\":612,\"h\":936,\"number\":25},{\"w\":612,\"h\":936,\"number\":26},{\"w\":612,\"h\":936,\"number\":27},{\"w\":612,\"h\":936,\"number\":28},{\"w\":612,\"h\":936,\"number\":29},{\"w\":612,\"h\":936,\"number\":30},{\"w\":612,\"h\":936,\"number\":31},{\"w\":612,\"h\":936,\"number\":32},{\"w\":612,\"h\":936,\"number\":33},{\"w\":612,\"h\":936,\"number\":34},{\"w\":612,\"h\":936,\"number\":35},{\"w\":612,\"h\":936,\"number\":36},{\"w\":612,\"h\":936,\"number\":37},{\"w\":612,\"h\":936,\"number\":38},{\"w\":612,\"h\":936,\"number\":39},{\"w\":612,\"h\":936,\"number\":40},{\"w\":612,\"h\":936,\"number\":41},{\"w\":612,\"h\":936,\"number\":42},{\"w\":612,\"h\":936,\"number\":43},{\"w\":612,\"h\":936,\"number\":44},{\"w\":612,\"h\":936,\"number\":45},{\"w\":612,\"h\":936,\"number\":46},{\"w\":612,\"h\":936,\"number\":47},{\"w\":612,\"h\":936,\"number\":48},{\"w\":612,\"h\":936,\"number\":49},{\"w\":612,\"h\":936,\"number\":50},{\"w\":612,\"h\":936,\"number\":51},{\"w\":612,\"h\":936,\"number\":52},{\"w\":612,\"h\":936,\"number\":53},{\"w\":612,\"h\":936,\"number\":54},{\"w\":612,\"h\":936,\"number\":55},{\"w\":612,\"h\":936,\"number\":56},{\"w\":612,\"h\":936,\"number\":57},{\"w\":612,\"h\":936,\"number\":58},{\"w\":612,\"h\":936,\"number\":59},{\"w\":612,\"h\":936,\"number\":60},{\"w\":612,\"h\":936,\"number\":61},{\"w\":612,\"h\":936,\"number\":62},{\"w\":612,\"h\":936,\"number\":63},{\"w\":612,\"h\":936,\"number\":64},{\"w\":612,\"h\":936,\"number\":65},{\"w\":612,\"h\":936,\"number\":66},{\"w\":612,\"h\":936,\"number\":67},{\"w\":612,\"h\":936,\"number\":68},{\"w\":612,\"h\":936,\"number\":69},{\"w\":612,\"h\":936,\"number\":70},{\"w\":612,\"h\":936,\"number\":71},{\"w\":612,\"h\":936,\"number\":72},{\"w\":612,\"h\":936,\"number\":73},{\"w\":612,\"h\":936,\"number\":74},{\"w\":612,\"h\":936,\"number\":75},{\"w\":612,\"h\":936,\"number\":76},{\"w\":612,\"h\":936,\"number\":77},{\"w\":612,\"h\":936,\"number\":78},{\"w\":612,\"h\":936,\"number\":79},{\"w\":612,\"h\":936,\"number\":80},{\"w\":612,\"h\":936,\"number\":81},{\"w\":612,\"h\":936,\"number\":82},{\"w\":612,\"h\":936,\"number\":83},{\"w\":612,\"h\":936,\"number\":84},{\"w\":612,\"h\":936,\"number\":85},{\"w\":612,\"h\":936,\"number\":86},{\"w\":612,\"h\":936,\"number\":87},{\"w\":612,\"h\":936,\"number\":88},{\"w\":612,\"h\":936,\"number\":89},{\"w\":612,\"h\":936,\"number\":90},{\"w\":612,\"h\":936,\"number\":91},{\"w\":612,\"h\":936,\"number\":92},{\"w\":612,\"h\":936,\"number\":93},{\"w\":612,\"h\":936,\"number\":94},{\"w\":612,\"h\":936,\"number\":95},{\"w\":612,\"h\":936,\"number\":96},{\"w\":612,\"h\":936,\"number\":97},{\"w\":612,\"h\":936,\"number\":98},{\"w\":612,\"h\":936,\"number\":99},{\"w\":612,\"h\":936,\"number\":100},{\"w\":612,\"h\":936,\"number\":101},{\"w\":612,\"h\":936,\"number\":102},{\"w\":612,\"h\":936,\"number\":103},{\"w\":612,\"h\":936,\"number\":104},{\"w\":612,\"h\":936,\"number\":105}],\"maxPageHeight\":936,\"widthForMaxHeight\":612}",
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
    }
}
