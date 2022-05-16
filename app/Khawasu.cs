

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
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
    public ConcurrentQueue<object> IncomingUpdates;
    public ConcurrentQueue<string> OutcomingUpdates;
    public Khawasu instance;

    public Device(string name, string type, string address, string groupName, Khawasu inst)
    {
        Name = name;
        Type = type;
        Address = address;
        GroupName = groupName;
        Attribs = new List<object>();
        IncomingUpdates = new ConcurrentQueue<object>();
        OutcomingUpdates = new ConcurrentQueue<string>();
        instance = inst;
    }
}


public class Khawasu
{
    private readonly HttpClient _client;
    private readonly string _hostname;

    public List<Device> Devices;
    public ConcurrentQueue<Device> Subscribes;
    private Thread backgroundThread;
    public Semaphore ThreadState = new Semaphore(0, 1);

    public Khawasu(string hostname)
    {
        _hostname = hostname;
        _client = new HttpClient();

        Subscribes = new ConcurrentQueue<Device>();

        UpdateDevices();
        
        // Run webhook thread
        backgroundThread = new Thread(WebhookTask);
        backgroundThread.Start();  
    }

    ~Khawasu()
    {
        ThreadState.Release();
    }

    public void UpdateDevices()
    {
        Devices = GetDevices().Result;
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
                return "Temp-Sensor";
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

    // Address: "<mac>/<port>"
    public Device? GetDeviceByAddress(string address)
    {
        return Devices.Find((e) => e.Address == address);
    }

    private async Task<List<Device>> GetDevices()
    {
        var devices = new List<Device>();
        try
        {
            string responseBody = Get($"http://{_hostname}/api/list-devices");

            dynamic responseJson = JsonConvert.DeserializeObject<dynamic>(responseBody);

            if (!HasProperty(responseJson, "data"))
                return new List<Device>();

            //if (HasProperty(responseJson["data"], "status"))
            //    return new List<Device>();


            foreach (var deviceRow in (JArray)responseJson["data"])
            {
                var dev = new Device(
                    (string)deviceRow["name"],
                    GetTypeFromDevClass((int)deviceRow["dev_class"]),
                    (string)deviceRow["address"],
                    (string)deviceRow["name"],
                    this
                );
                devices.Add(dev);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        

        return devices;
        
    }
    
    private void WebhookTask()
    {
        var devices = new List<Device>();
        try
        {
            string responseBody = Get($"http://{_hostname}/api/webhook/new");
            dynamic responseJson = JsonConvert.DeserializeObject<dynamic>(responseBody);

            if (!HasProperty(responseJson, "data"))
                return;

            if ((string)responseJson["data"]["status"] != "ok")
            {
                Console.WriteLine($"Webhook create error: {responseJson["data"]["status"]}");
                return;
            }

            var webhookUUID = (string)responseJson["data"]["uuid"];

            while (true)
            {
                try
                {
                    if(ThreadState.WaitOne(1))
                        break;
                    
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    while (stopWatch.Elapsed < TimeSpan.FromMilliseconds(100))
                    {
                        Device subDev;
                        if (Subscribes.TryDequeue(out subDev))
                        {
                            // Make subscribe
                            Get($"http://{_hostname}/api/webhook/{webhookUUID}/add/{subDev.Address}");
                            
                            // Get initial status
                            string initResponseBody = Get($"http://{_hostname}/api/device/{subDev.Address}/{subDev.Type.ToLower()}/state");
                            dynamic initResponseJson = JsonConvert.DeserializeObject<dynamic>(initResponseBody);
                            if (!HasProperty(initResponseJson, "data"))
                                continue;
                            
                            var dev = GetDeviceByAddress(subDev.Address);
                            if(dev == null) // Not exists in list
                                continue;
                            
                            dev.Value.IncomingUpdates.Enqueue((object)initResponseJson["data"]);
                        }

                        foreach (var dev in Devices)
                        {
                            string command;
                            if (dev.OutcomingUpdates.TryDequeue(out command))
                            {
                                // Send command
                                Get($"http://{_hostname}/api/device/{dev.Address}/{dev.Type.ToLower()}/{command}");
                            }
                        }
                        
                    }
                    stopWatch.Stop();
                    
                    string webhookResponseBody = Get($"http://{_hostname}/api/webhook/{webhookUUID}");
                    dynamic webhookResponseJson = JsonConvert.DeserializeObject<dynamic>(webhookResponseBody);

                    foreach (var (address, value) in (JObject)webhookResponseJson["data"])
                    {
                        var dev = GetDeviceByAddress(address);
                        if(dev == null) // Not exists in list
                            continue;
                        
                        dev.Value.IncomingUpdates.Enqueue((object)value["data"]);
                    }
                    
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }
            }


            
        }
        catch (Exception e)
        {
            Trace.WriteLine(e);
        }
        
    }
    
    

}