using Newtonsoft.Json;
using PropertyChanged;

namespace Emby.WindowsPhone.Model
{
    [ImplementPropertyChanged]
    public class UdpResponse
    {
        // {"Address":"http://192.168.0.2:8096","Id":"33f19ad268ed4ea28e77101dba238002","Name":"Home"}
        [JsonProperty("Address")]
        public string ServerAddress { get; set; }
        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
    }
}
