namespace BrowserInteractLabeler.Common
{
    public class ExportData
    {
        public int ClassID { get; set; }
        public Point [] Points { get; set; }

        public TypeMarkup TypeDrawing { get; set; }

        public string FullImgName { get; set; }
    }
}