using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TextUtil;

static class Program
{
    static async Task<int> Main(string[] args)
    {
        if (args.Length == 3 && args[0].ToLower().CompareTo("-makeversion") == 0)
            return PackageInfo.MakePackageInfoScript(args[1], args[2]);

        if (args.Length == 4 && args[0].ToLower().CompareTo("-insertline") == 0)
            return TextFileUtil.FindAndInsert(args[1], args[2], args[3]);

        if (args.Length == 3 && args[0].ToLower().CompareTo("-removeline") == 0)
            return TextFileUtil.FindAndDelete(args[1], args[2]);

        if (args.Length == 3 && args[0].ToLower().CompareTo("-slack") == 0)
            return await WebhookUtil.SendSlack(args[1], args[2]);

        if (args.Length == 3 && args[0].ToLower().CompareTo("-changelog") == 0)
        {
            if (!File.Exists("package.json"))
            {
                Console.WriteLine("package.json not found");
                return Constants.FAIL;
            }
            if (!File.Exists(args[1]))
            {
                Console.WriteLine(args[1] + "can not find.");
                return Constants.FAIL;
            }
            string changelog = File.ReadAllText(args[1]);
            if (NPMPackage.SetChangeLogAndFootprint("package.json", changelog, args[2]))
                return Constants.SUCCESS;
            return Constants.FAIL;
        }

        if (args.Length == 1 && args[0].ToLower().CompareTo("-commitid") == 0)
        {
            if (!File.Exists("package.json"))
            {
                Console.WriteLine("package.json not found");
                return Constants.FAIL;
            }

            string commitId = NPMPackage.GetFootprint("package.json");

            if (string.IsNullOrWhiteSpace(commitId))
                return Constants.FAIL;
            else
            {
                Console.WriteLine(commitId);
                return Constants.SUCCESS;
            }
        }

        Console.WriteLine("Useses(Insert line) : TextUtil.exe -insertline <filename> <existing statement> <to add statement>");
        Console.WriteLine("Useses(Remove line) : TextUtil.exe -removeline <filename> <existing statement>");
        Console.WriteLine("Useses(Slack Message) : TextUtil.exe -slack <webhook url> <message>");
        Console.WriteLine("Useses(Make Package Version) : TextUtile.exe -makeversion <to make version file path> <namespace>");
        Console.WriteLine("Useses(Get commitID) : TextUtile.exe -commitid");
        Console.WriteLine("Useses(Set changelog) : TextUtile.exe -changelog <changelog file> <commitid>");

        return Constants.FAIL;
    }
}

public static class Constants
{
    public const int SUCCESS = 0;
    public const int FAIL = 1;
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

public class NPMPackageInfo
{
    public string name { get; set; }
    public string version { get; set; }
}

public static class PackageInfo
{
    public static int MakePackageInfoScript(string fileName, string namespaceName)
    {
        string packageJsonFile = "package.json";

        if (!File.Exists(packageJsonFile))
        {
            Console.WriteLine($"File not found : {packageJsonFile}");
            return Constants.FAIL;
        }
        var packageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<NPMPackageInfo>(File.ReadAllText(packageJsonFile));
        if (packageInfo == null)
        {
            Console.WriteLine($"Not json file : {packageJsonFile}");
            return Constants.FAIL;
        }

        return MakePackageInfoScript(fileName, namespaceName, packageInfo.name, packageInfo.version);
    }

    public static int MakePackageInfoScript(string filename, string namespaceName, string packageName, string packageVersion)
    {
        if (string.IsNullOrWhiteSpace(filename) || string.IsNullOrWhiteSpace(namespaceName) || string.IsNullOrWhiteSpace(packageName) || string.IsNullOrWhiteSpace(packageVersion))
            return Constants.FAIL;

        StringBuilder build = new StringBuilder();
        build.AppendLine("namespace " + namespaceName);
        build.AppendLine("{");
        build.AppendLine("  public class PackageInfo");
        build.AppendLine("  {");
        build.AppendLine("      public const string Name = \"" + packageName + "\";");
        build.AppendLine("      public const string Version = \"" + packageVersion + "\";");
        build.AppendLine("  }");
        build.AppendLine("}");

        File.WriteAllText(filename, build.ToString());

        return Constants.SUCCESS;
    }
}