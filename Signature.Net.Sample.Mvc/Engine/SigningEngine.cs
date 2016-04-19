using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using GroupDocs.Signature.Config;
using GroupDocs.Signature.Handler;
using GroupDocs.Signature.Options;

namespace Signature.Net.Sample.Mvc.Engine
{
    internal class SigningEngine
    {
        protected internal string SignDocument(string rootPath,
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
            handler.SetLicense(@"d:\temp\SignatureLicense\GroupDocs.Signature3.lic");

            // setup PDF image signature options
            SignTextOptions signOptions = null;
            string fileNameExtension = Path.GetExtension(fileName).TrimStart('.');
            fileNameExtension = fileNameExtension.ToLower();
            int pageWidth = 0, pageHeight = 0;
            switch (fileNameExtension)
            {
                case "pdf":
                    signOptions = new PDFSignTextOptions(signatureText);
                    break;

                case "doc":
                case "docx":
                case "rtf":
                    signOptions = new WordsSignTextOptions(signatureText);
                    break;

                case "xls":
                case "xlsx":
                    signOptions = new CellsSignTextOptions(signatureText)
                    {
                        ColumnNumber = signatureColumnNum,
                        RowNumber = signatureRowNum
                    };
                    break;

                case "ppt":
                case "pptx":
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
    }
}