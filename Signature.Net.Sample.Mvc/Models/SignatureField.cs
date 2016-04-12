namespace Signature.Net.Sample.Mvc.Models
{
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
}