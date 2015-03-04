using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MediaBrowser.WindowsPhone.Extensions
{
    public static class SerialisationExtensions
    {
        public static Task<TReturnType> DeserialiseAsync<TReturnType>(this string json)
        {
            return Task.Factory.StartNew(() => JsonConvert.DeserializeObject<TReturnType>(json));
        }

        public static Task<string> SerialiseAsync(this object item)
        {
            return Task.Factory.StartNew(() => JsonConvert.SerializeObject(item, new JsonSerializerSettings{NullValueHandling = NullValueHandling.Ignore}));
        }

        public static Task<string> SerialiseAsync(this object item, JsonConverter converter)
        {
            return Task.Factory.StartNew(() => JsonConvert.SerializeObject(item, converter));
        }
    }
}
