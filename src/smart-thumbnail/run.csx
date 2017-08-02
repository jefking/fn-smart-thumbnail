using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

private static readonly string subscriptionKey = Env("SubscriptionKey");
private const string uriBase = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/generateThumbnail";

//EXAMPLE: https://docs.microsoft.com/en-us/azure/cognitive-services/computer-vision/quickstarts/csharp#GetThumbnail
public static async Task Run(Stream input, Stream resized, TraceWriter log)
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

        // Execute the REST API call.
        response = await client.PostAsync(uri, content);

        if (response.IsSuccessStatusCode)
        {
            log.Info("\nResponse:\n" + response);

            var blob = await response.Content.ReadAsByteArrayAsync();
            resized = new MemoryStream(blob);
        }
        else
        {
            var result = await response.Content.ReadAsStringAsync();
            log.Error(result);
        }
    }
}

private static string Env(string name) => System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);