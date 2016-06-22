using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Aspose.Cells;
using Aspose.Cells.Rendering;
using Aspose.Slides;
using Aspose.Words.Rendering;
using GroupDocs.Signature.Config;
using GroupDocs.Signature.Handler;
using GroupDocs.Signature.Options;
using Signature.Net.Sample.Mvc.Models;

namespace Signature.Net.Sample.Mvc.Engine
{
    public interface ISigningEngine : IViewingEngine
    {
        object SignDocument(
            string appDataPath,
            string documentGuid,
            string documentId,
            string name,
            SignatureField[] fields,
            Func<string, string> urlCreator);
    }

    public class SigningEngine : ViewingEngine, ISigningEngine
    {
        private readonly ISvgRenderer _svgRenderer;
        private readonly ILicensing _licensing;

        public SigningEngine(ISvgRenderer svgRenderer,
            ILicensing licensing)
        {
            _svgRenderer = svgRenderer;
            _licensing = licensing;
        }

        public object SignDocument(
            string appDataPath,
            string documentGuid,
            string documentId,
            string name,
            SignatureField[] fields,
            Func<string, string> urlCreator)
        {
            if (fields == null || fields.Length == 0)
                return null;
            SignatureField field = fields[0];
            string data = field.Data;
            string signatureText = String.Empty;
            SignatureField.Location location = field.Locations[0];
            const double scaleForSizes = 2.083;
            int signatureWidth = (int)(location.LocationWidth / scaleForSizes);
            int signatureHeight = (int)(location.LocationHeight / scaleForSizes);

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
                IEnumerable<XElement> textElements;

                imageBytes = _svgRenderer.DrawSvgImage(svgData, signatureWidth, signatureHeight);

                XDocument root = XDocument.Parse(svgData);
                textElements = root.Descendants("{http://www.w3.org/2000/svg}text");
                if (textElements.Count() > 0)
                {
                    foreach (XElement textElement in textElements)
                    {
                        signatureText += textElement.Value;
                    }
                }
            }
            // request structure:
            //{ "documentId":"","name":"a b","waterMarkText":"","waterMarkImage":"","fields":[{"fieldType":1,"data":"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"100%\" height=\"100%\" viewbox=\"0 0 233 82\" preserveaspectratio=\"none\"><text font-family=\"Tangerine\" font-size=\"60px\" fill=\"#0036D9\" y=\"50%\" x=\"50%\" dy=\"0.3em\" text-anchor=\"middle\">Anonymous</text><defs><link href=\"http://fonts.googleapis.com/css?family=Tangerine\" type=\"text/css\" rel=\"stylesheet\" xmlns=\"http://www.w3.org/1999/xhtml\"><style type=\"text/css\">@import url(http://fonts.googleapis.com/css?family=Tangerine)</style></defs></svg>","locations":[{"page":1,"locationX":0.4,"locationY":0.3,"locationWidth":150,"locationHeight":50,"fontName":null,"fontSize":null,"fontColor":null,"fontBold":null,"fontItalic":null,"fontUnderline":null,"alignment":0,"id":"ff4dd6a4a44ecd682a4be3a19a801e6f"}],"id":"1c9b463ac3c1e9ebaf51e34ea352de3a"}],"documentGuid":"candy.pdf","recipientGuid":"71d1f3ef88a5d7fe32f4c46588a69887","email":"a@b.com"}

            string path = documentGuid;
            string fullPathToDocument = Path.Combine(appDataPath, path);

            string fileNameExtension = Path.GetExtension(path).TrimStart('.');
            fileNameExtension = fileNameExtension.ToLower();
            int pageWidth = 0, pageHeight = 0;
            int signatureColumnNum = 0, signatureRowNum = 0;

            int pageNumber = location.Page;
            DocumentType documentType = GetDocumentType(fileNameExtension);
            switch (documentType)
            {
                case DocumentType.Pdf:
                    using (Aspose.Pdf.Document document = new Aspose.Pdf.Document(fullPathToDocument))
                    {
                        Aspose.Pdf.Rectangle pageRect = document.Pages[pageNumber].Rect;
                        pageWidth = (int)pageRect.Width;
                        pageHeight = (int)pageRect.Height;
                    }
                    break;

                case DocumentType.Words:
                    Aspose.Words.Document wordsDocument = new Aspose.Words.Document(fullPathToDocument);
                    pageNumber = location.Page - 1;
                    PageInfo page = wordsDocument.GetPageInfo(pageNumber);
                    SizeF rect = page.SizeInPoints;
                    pageWidth = (int)rect.Width;
                    pageHeight = (int)rect.Height;
                    pageNumber = location.Page;
                    break;

                case DocumentType.Cells:
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

                case DocumentType.Slides:
                    Presentation powerPointDocument = new Presentation(fullPathToDocument);
                    SizeF slideSize = powerPointDocument.SlideSize.Size;
                    pageWidth = (int)slideSize.Width;
                    pageHeight = (int)slideSize.Height;
                    pageNumber = location.Page;
                    break;
            }
            
            string outputFilePath;

            MemoryStream imageStream = null;
            try
            {
                if (imageBytes == null)
                {
                    outputFilePath = SignDocumentWithText(appDataPath,
                        documentGuid,
                        signatureText,
                        pageNumber,
                        (int)(pageWidth * location.LocationX),
                        (int)(pageHeight * location.LocationY),
                        signatureWidth,
                        signatureHeight,
                        signatureColumnNum, signatureRowNum);
                }
                else
                {
                    imageStream = new MemoryStream(imageBytes);
                    outputFilePath = SignDocumentWithImage(appDataPath,
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
                        signedDocumentUrl = urlCreator(relativeOutputFileName),
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

            return resultData;
        }


        protected internal string SignDocumentWithText(string rootPath,
                                  string fileName,
                                  string signatureText,
                                  int pageNumber,
                                  int left,
                                  int top,
                                  int width,
                                  int height,
                                  int signatureColumnNum, int signatureRowNum)
        {
            string storagePath = rootPath;
            string outputPath = Path.Combine(rootPath, @"Output");
            string imagesPath = Path.Combine(rootPath, @"Images");

            // set up a configuration
            SignatureConfig config = new SignatureConfig()
            {
                StoragePath = storagePath,
                OutputPath = outputPath,
                ImagesPath = imagesPath
            };

            // instantiating the handler
            SignatureHandler handler = new SignatureHandler(config);

            // Set a license if you have one
            _licensing.ApplyLicense();

            // setup PDF image signature options
            SignTextOptions signOptions = null;
            string fileNameExtension = Path.GetExtension(fileName).TrimStart('.');
            fileNameExtension = fileNameExtension.ToLower();
            int pageWidth = 0, pageHeight = 0;
            DocumentType fileType = GetDocumentType(fileNameExtension);
            switch (fileType)
            {
                case DocumentType.Pdf:
                    signOptions = new PdfSignTextOptions(signatureText);
                    break;

                case DocumentType.Words:
                    signOptions = new WordsSignTextOptions(signatureText);
                    break;

                case DocumentType.Cells:
                    signOptions = new CellsSignTextOptions(signatureText)
                    {
                        ColumnNumber = signatureColumnNum,
                        RowNumber = signatureRowNum
                    };
                    break;

                case DocumentType.Slides:
                    signOptions = new SlidesSignTextOptions(signatureText);
                    break;
            }
            signOptions.DocumentPageNumber = pageNumber;
            signOptions.Left = left;
            signOptions.Top = top;
            signOptions.Width = width;
            signOptions.Height = height;
            signOptions.SignAllPages = false;

            GroupDocs.Signature.Options.SaveOptions saveOptions = new GroupDocs.Signature.Options.SaveOptions(OutputType.String);
            // sign the document
            string outputFilePath = handler.Sign<string>(fileName, signOptions, saveOptions);
            return outputFilePath;

        }


        protected internal string SignDocumentWithImage(string rootPath,
                                  string fileName,
                                  Stream imageStream,
                                  int pageNumber,
                                  int left,
                                  int top,
                                  int width,
                                  int height,
                                  int signatureColumnNum, int signatureRowNum)
        {
            string storagePath = rootPath;
            string outputPath = Path.Combine(rootPath, @"Output");
            string imagesPath = Path.Combine(rootPath, @"Images");

            // set up a configuration
            SignatureConfig config = new SignatureConfig()
            {
                StoragePath = storagePath,
                OutputPath = outputPath,
                ImagesPath = imagesPath
            };

            // instantiating the handler
            SignatureHandler handler = new SignatureHandler(config);

            // Set a license if you have one
            _licensing.ApplyLicense();

            // setup PDF image signature options
            SignImageOptions signOptions = null;
            string fileNameExtension = Path.GetExtension(fileName).TrimStart('.');
            fileNameExtension = fileNameExtension.ToLower();
            int pageWidth = 0, pageHeight = 0;
            DocumentType fileType = GetDocumentType(fileNameExtension);
            switch (fileType)
            {
                case DocumentType.Pdf:
                    signOptions = new PdfSignImageOptions(imageStream);
                    break;

                case DocumentType.Words:
                    signOptions = new WordsSignImageOptions(imageStream);
                    break;

                case DocumentType.Cells:
                    signOptions = new CellsSignImageOptions(imageStream)
                    {
                        ColumnNumber = signatureColumnNum,
                        RowNumber = signatureRowNum
                    };
                    break;

                case DocumentType.Slides:
                    signOptions = new SlidesSignImageOptions(imageStream);
                    break;
            }
            signOptions.DocumentPageNumber = pageNumber;
            signOptions.Left = left;
            signOptions.Top = top;
            signOptions.Width = width;
            signOptions.Height = height;
            signOptions.SignAllPages = false;

            GroupDocs.Signature.Options.SaveOptions saveOptions = new GroupDocs.Signature.Options.SaveOptions(OutputType.String);
            // sign the document
            string outputFilePath = handler.Sign<string>(fileName, signOptions, saveOptions);
            return outputFilePath;
        }
    }
}