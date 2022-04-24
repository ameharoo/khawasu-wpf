

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace app;

// Khawasu. GPL license.
// Класс для работы с HTTP драйвером умного дома

public struct Device
{
    public string Name;
    public string Type;
    public string Address;
    public string GroupName;
    // Todo: add implementation attribs
    public List<object> Attribs;

    public Device(string name, string type, string address, string groupName)
    {
        Name = name;
        Type = type;
        Address = address;
        GroupName = groupName;
        Attribs = new List<object>();
    }
}


public class Khawasu
{
    private readonly HttpClient _client;
    private readonly string _hostname;

    public List<Device> Devices => GetDevices().Result;
    

    public Khawasu(string hostname)
    {
        _hostname = hostname;
        _client = new HttpClient();
    }

    protected string GetTypeFromDevClass(int devClass)
    {
        switch (devClass)
        {
            case 1:
                return "Button";
            case 2:
                return "Relay";
            case 3:
                return "TempHumSensor";
            case 4:
                return "TempSensor";
            default:
                return "Unknown";
        }
    }
    
    public static bool HasProperty(dynamic obj, string name)
    {
        return ((JObject)obj).Property(name) != null;
    }
    
    public string Get(string uri)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using(Stream stream = response.GetResponseStream())
        using(StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    private async Task<List<Device>> GetDevices()
    {
        string responseBody = Get($"http://{_hostname}/api/list-devices");

        dynamic responseJson = JsonConvert.DeserializeObject<dynamic>(responseBody);

        if (!HasProperty(responseJson, "data"))
            return new List<Device>();
        
        //if (HasProperty(responseJson["data"], "status"))
        //    return new List<Device>();
        
        var devices = new List<Device>();
        foreach (var deviceRow in (JArray)responseJson["data"])
        {
            var dev = new Device(
                (string)deviceRow["name"], 
                GetTypeFromDevClass((int)deviceRow["dev_class"]), 
                (string)deviceRow["address"], 
                (string)deviceRow["name"]
                );
            devices.Add(dev);
        }
        
        return devices;
    }
    
    

}