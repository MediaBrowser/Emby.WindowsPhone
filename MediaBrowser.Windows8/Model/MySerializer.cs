using System.IO;

namespace MediaBrowser.Windows8.Model
{
    //class MySerializer : IDataSerializer
    //{
    //    public T DeserializeFromStream<T>(Stream stream)
    //    {
    //        return JsonSerializer.DeserializeFromStream<T>(stream);
    //    }

    //    public T DeserializeJsonFromStream<T>(Stream stream)
    //    {
    //        return JsonSerializer.DeserializeFromStream<T>(stream);
    //    }

    //    public T DeserializeJsvFromStream<T>(Stream stream)
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public T DeserializeProtobufFromStream<T>(Stream stream)
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public bool CanDeserializeJsv
    //    {
    //        get { return false; }
    //    }

    //    public bool CanDeserializeProtobuf
    //    {
    //        get { return false; }
    //    }

    //    public object DeserializeJsonFromStream(Stream stream, System.Type type)
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public object DeserializeJsvFromStream(Stream stream, System.Type type)
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public object DeserializeProtobufFromStream(Stream stream, System.Type type)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}

    //public static class JsonSerializer
    //{
    //    public static T DeserializeFromStream<T>(Stream stream)
    //    {
    //        string json = "";
    //        using (var reader = new StreamReader(stream))
    //        {
    //            json = reader.ReadToEnd();
    //        }
    //        try
    //        {
    //            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
    //        }
    //        catch
    //        {
    //            return default(T);
    //        }
    //    }

    //}
}
