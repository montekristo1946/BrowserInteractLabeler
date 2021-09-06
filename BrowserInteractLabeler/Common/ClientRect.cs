using Newtonsoft.Json;

namespace BrowserInteractLabeler.Common
{
    public class ClientRect
    {
        [JsonProperty("x")]
        public double X { get; set; } = -1;
        
        [JsonProperty("y")]
        public double Y { get; set; }= -1;
        
        [JsonProperty("width")]
        public double Width { get; set; }= -1;
        
        [JsonProperty("height")]
        public double Height { get; set; }= -1;
        
        [JsonProperty("top")]
        public double Top { get; set; }= -1;
        
        [JsonProperty("right")]
        public double Right { get; set; }= -1;
        
        [JsonProperty("bottom")]
        public double Bottom { get; set; }= -1;
        
        [JsonProperty("left")]
        public double Left { get; set; }= -1;
    }
}