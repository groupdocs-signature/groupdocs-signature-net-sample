using System.IO;

using GroupDocs.Signature.Config;
using GroupDocs.Signature.Handler;
using GroupDocs.Signature.Options;
using GroupDocs.Signature;

namespace Signature.Net.Sample.Mvc.Engine
{
    internal class SigningEngine : ViewingEngine
    {
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
            SetLicense();

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
            SetLicense();

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

        private void SetLicense()
        {
            License license = new License();
            license.SetLicense(@"d:\temp\SignatureLicense\GroupDocs.Signature3.lic");
        }
    }
}