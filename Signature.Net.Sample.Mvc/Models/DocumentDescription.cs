using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCDemo;

namespace Signature.Net.Sample.Mvc.Models
{
    public class DocumentDescription
    {
        public DocumentDescription()
        {
            pages = new List<PageDescription>();
        }

        public IList<PageDescription> pages { get; set; }
        public int pageCount { get; set; }
        public int widthForMaxHeight { get; set; }
        public int maxPageHeight { get; set; }
    }
}