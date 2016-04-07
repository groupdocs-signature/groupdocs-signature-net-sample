using System.Linq;
using System.Web.Mvc;
using Groupdocs.Data.Signature;
using Groupdocs.Web.UI.Signature.Services;
using MVCDemo.Models;

namespace MVCDemo.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            //var document1 = FluentSignature.Document()
            //    .FileName("sample.pdf")
            //    .SignedFileName("sample_signed1.pdf")
            //    .AddRecipient(FluentSignature.Recipient()
            //        .AddField(FluentSignature.Field()
            //            .Name("Signature1")
            //            .Mandatory(true)
            //            .LockDuringSign(false)
            //            .Type(SignatureFieldType.Signature)
            //            .AddLocation(FluentSignature.Location()
            //                .Page(1)
            //                .LocationWidth(150)
            //                .LocationHeight(50)
            //                .LocationX((decimal)0.400)
            //                .LocationY((decimal)0.300)
            //            )
            //        )
            //    )
            //    .Create();

            //var document2 = FluentSignature.Document()
            //    .FileName("sample.pdf")
            //    .SignedFileName("sample_signed2.pdf")
            //    .AddRecipient(FluentSignature.Recipient()
            //        .AddField(FluentSignature.Field()
            //            .Name("Signature1")
            //            .Mandatory(true)
            //            .LockDuringSign(false)
            //            .Type(SignatureFieldType.Signature)
            //            .AddLocation(FluentSignature.Location()
            //                .Page(1)
            //                .LocationWidth(150)
            //                .LocationHeight(50)
            //                .LocationX((decimal)0.400)
            //                .LocationY((decimal)0.300)
            //            )
            //        )
            //    )
            //    .Create();

            //var signDocument = new SignDocument
            //{
            //    DocumentGuid1 = document1.Guid,
            //    RecipientGuid1 = document1.SignatureDocumentRecipients.First().Guid,
            //    DocumentGuid2 = document2.Guid,
            //    RecipientGuid2 = document2.SignatureDocumentRecipients.First().Guid
            //};

            //return View(signDocument);
            return View();
        }
        
    }
}
