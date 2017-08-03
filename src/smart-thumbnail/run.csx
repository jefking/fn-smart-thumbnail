using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Azure.Data.Wrappers;

private static readonly string storage = ConfigurationManager.AppSettings["Storage"];
private static readonly string subscriptionKey = ConfigurationManager.AppSettings["SubscriptionKey"];
private static readonly string uriBase = ConfigurationManager.AppSettings["Url"];

//EXAMPLE: https://docs.microsoft.com/en-us/azure/cognitive-services/computer-vision/quickstarts/csharp#GetThumbnail
public static async Task Run(Stream input, string name, TraceWriter log)
{
    var uri = uriBase + "?width=200&height=150&smartCropping=true";

    log.Info("Subscription Key:\n" + subscriptionKey);
    log.Info("uri:\n" + uri);

    var client = new HttpClient();
    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

    HttpResponseMessage response;

    var binaryReader = new BinaryReader(input);
    var byteData = binaryReader.ReadBytes((int)input.Length);

    using (var content = new ByteArrayContent(byteData))
    {
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        response = await client.PostAsync(uri, content);

        if (response.IsSuccessStatusCode)
        {
            log.Info("\nResponse:\n" + response);

            var blob = await response.Content.ReadAsByteArrayAsync();

            var container = new Container("destination", storage);
            await container.CreateIfNotExists();
            await container.Save(name, blob);
        }
        else
        {
            var result = await response.Content.ReadAsStringAsync();
            log.Error(result);
        }
    }
}

private static string Env(string name) => System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);