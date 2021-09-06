using Newtonsoft.Json;

namespace BrowserInteractLabeler.Common
{
    public class MousePositionData
    {
        [JsonProperty("offsetLeft")]
        public  double OffsetLeft { get; set; }
        
        [JsonProperty("offsetTop")]
        public double OffsetTop { get; set; }
        
        [JsonProperty("clientRect")]
        public ClientRect ActiveRectangle { get; set; }
    }
    
}