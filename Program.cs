using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Net.Http;

static class Program
{
    static async Task<int> Main(string[] args)
    {
        if (args.Length == 4 && args[0].ToLower().CompareTo("-insertline") == 0)
            return TextFileUtil.FindAndInsert(args[1], args[2], args[3]);

        if (args.Length == 3 && args[0].ToLower().CompareTo("-slack") == 0)
            return await WebhookUtil.SendSlack(args[1], args[2]);

        Console.WriteLine("Useses(Insert line) : TextUtil.ext -insertline <filename> <existing statement> <to add statement>");
        Console.WriteLine("Useses(Slack Message) : TextUtil.ext -slack <webhook url> <message>");

        return Constants.FAIL;
    }
}

public static class Constants
{
    public const int SUCCESS = 0;
    public const int FAIL = 1;
}

public class TextFileUtil
{
    public static int FindAndInsert(string filename, string findWord, string toAddState)
    {

        if (!File.Exists(filename))
        {
            Console.WriteLine($"File not found : {filename}");
            return Constants.FAIL;
        }

        List<string> lines = File.ReadAllLines(filename).ToList();
        if (lines == null || lines.Count < 1)
            return Constants.FAIL;

        int count = lines.Count;
        int insertLine = -1;
        for (int i = 0; i < count; i++)
        {
            var line = lines[i];
            if (line.Contains(findWord))
            {
                insertLine = i + 1;
                lines.Insert(insertLine, toAddState);
                break;
            }
        }

        if (insertLine > 0)
        {
            File.WriteAllLines(filename, lines);
            return Constants.SUCCESS;
        }

        return Constants.FAIL;
    }
}

public static class WebhookUtil
{
    class SlackMessage
    {
        public string text { get; set; }
        public SlackMessage(string text) { this.text = text; }
    }
    public static async Task<int> SendSlack(string url, string text)
    {
        if (url.StartsWith("https://hooks.slack.com/services/") == false)
        {
            Console.WriteLine("Invalid webhook url : " + url);
            return Constants.FAIL;
        }

        SlackMessage message = new SlackMessage(text);

        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(url);
        client.DefaultRequestHeaders
              .Accept
              .Add(new MediaTypeWithQualityHeaderValue("application/json"));

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");
        request.Content = new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.SendAsync(request);
        return response.StatusCode == HttpStatusCode.OK ? Constants.SUCCESS : Constants.FAIL;
    }
}