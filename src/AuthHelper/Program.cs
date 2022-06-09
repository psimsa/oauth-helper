using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using Xamarin.Auth;

namespace AuthHelper
{
    class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            IConfigurationRoot config = CreateConfig();
            var appConfig = config.GetSection("remoteAppSettings");
            var auth = new OAuth2Authenticator(
                appConfig["appId"],
                appConfig["appSecret"],
                appConfig["scopes"],
                new Uri(appConfig["authUrl"]),
                new Uri(appConfig["redirectUrl"]),
                new Uri(appConfig["tokenUrl"]))
                ;

            var url = auth.GetInitialUrlAsync().Result;
            Console.WriteLine($"Authorization url: {url}\r\nThe URL will now open in a new browser. " +
                              $"The browser will automatically close once\r\nauthorization code is retrieved.");

            Application.EnableVisualStyles();
            var f = new WebFormControl(url);
            var w = f.Controls.OfType<WebBrowser>().FirstOrDefault();
            w.Navigated += (sender, eventArgs) =>
            {
                if (eventArgs.Url.ToString().StartsWith(appConfig["redirectUrl"]))
                {
                    url = eventArgs.Url;
                    f.Close();
                }
            };
            w.Navigate(url);
            Application.Run(f);
            var query = url.Query.TrimStart('?');

            var queryItems = query.Split('&');
            var codeItem = DecodeUrlEncoded(queryItems.FirstOrDefault(x => x.StartsWith("code=")));

            if (codeItem != null)
            {
                var code = codeItem.Split('=').Last();
                Console.WriteLine(code);

                var result = auth.RequestAccessTokenAsync(code).Result;

                Console.WriteLine("Code retrieved. Here are your details:\r\n");
                foreach (var item in result)
                {
                    Console.WriteLine($"{item.Key} - {item.Value}");
                }

                Console.WriteLine(
                    "\r\nYou can now use the access_token value to authenticate requests against the web api.\r\n" +
                    "The refresh_token serves for requesting new access tokens once they expire (by default 1 hour).\r\n\r\n" +
                    "Do you want to save the results into a file? Y/N");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    using (var tw = new StreamWriter("tokens.json"))
                    {

                        JsonObject ob =
                            new JsonObject(result.Select(x =>
                            {
                                var item = new KeyValuePair<string, JsonValue>(x.Key, x.Value);
                                return item;
                            }));

                        var s = ob.ToString();
                        tw.WriteLine(s);
                    }
                    Console.WriteLine("Tokens written to tokens.json.");
                }
            }
            else
            {
                Console.WriteLine("Navigation not successfull.");
            }
            Console.WriteLine("Exiting...");
        }

        private static IConfigurationRoot CreateConfig()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                
                .Build();
        }

        private static string DecodeUrlEncoded(string input)
        {
            return HttpUtility.UrlDecode(input);
        }
    }
}
