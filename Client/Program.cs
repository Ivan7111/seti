using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(tokent());

            Console.WriteLine("Hello World!");
            SendTCP("howdy");
            Task.Run(Listen);
            Console.Read();

            Console.WriteLine("Обработка завершена");
        }

      /*  public static void Response(HttpListenerResponse response, string content)
        {
            string responseString = HTMLManager.GetMemeHTMLPage(content);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }*/




        private static async Task Listen()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8888/");
            listener.Start();
            Console.WriteLine("Ожидание подключений...");

            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;

                HttpListenerResponse response = context.Response;

                string values = "";
                int v1 = 0;
                for (int i = 0; i < request.Headers.Count; i++)
                {

                    values += String.Join("\n", request.Headers.GetValues(v1++)) + "\n";
                }
                   Console.WriteLine(values);
                var str = "";


                if (request.HasEntityBody)
                {

                    byte[] b = new byte[request.ContentLength64];
                    request.InputStream.Read(b, 0, (int)request.ContentLength64);
                    request.InputStream.Flush();
                    request.InputStream.Close();
                    str = System.Text.Encoding.Default.GetString(b);
                    // Console.WriteLine($"connected  {request.HasEntityBody}");
                    Console.WriteLine(str + "\n::" + str.Length);
                    Response(response, SendTCP(str));

                   // response.
                   // RequestResolver.ResolveRequest(str, response);
                }
                else {
                    Console.WriteLine("i recieved: nothing");
                    Response(response, SendTCP("")); 
                }

                /*
                Console.WriteLine($"connected  {request.HasEntityBody} : {str} " +
                    // $"||\n{response.}"+
                    $"\n||\n{values}");//+
                                                                          // $"||\n{String.Join("\n" , response.QueryString.AllKeys)}");
                                                                          // request.
                                                                          //$"||\n{String.Join("\n" ,request.Headers.AllKeys)}");
                                                                          // $"||\n{String.Join("\n" ,request.Headers.AllKeys)}");
                                                                          */


            }
        }

        public static void Response(HttpListenerResponse response, string content)
        {

            string responseString = content;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        static string SendTCP(string request)
        {
            TcpClient client = new TcpClient();
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12000));

            StreamWriter sw = new StreamWriter(client.GetStream());
            sw.AutoFlush = true;
            sw.WriteLine(request);

            StreamReader sr = new StreamReader(client.GetStream());
            string result = sr.ReadToEnd();

            Console.WriteLine("-From server: -st---");
            Console.WriteLine(result);
            Console.WriteLine("--end---");

            //if(result.StartsWith())

            sr.Close();
            sw.Close();
            client.Close();
            return result;
        }
        static string tokent()
        {
            var client = new RestClient("https://service.endpoint.com/api/oauth2/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", "grant_type=client_credentials&client_id=abc&client_secret=123", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.Content ;
        }
    }
    /*
    public string getAccessToken()
    {
        string uri = "http://192.168.1.89:8080/oauth/token";
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(
            string.Format("{0}:{1}", client_id, client_secret))));

        client.BaseAddress = new Uri(uri);

        var parameters = new Dictionary<string, string>();
        parameters["password"] = password;
        parameters["username"] = username;
        parameters["grant_type"] = grant_type;
        parameters["scope"] = scope;
        parameters["client_id"] = client_id;
        parameters["client_secret"] = client_secret;

        var response = client.PostAsync(uri, new FormUrlEncodedContent(parameters)).Result;
        Console.WriteLine((response.StatusCode.ToString()));
        string resultContent = response.Content.ReadAsStringAsync().Result;
        Console.WriteLine(resultContent);

        string s = "access_token\":\"";
        string token = resultContent.Substring(resultContent.IndexOf(s) + s.Length, tokenLength);
        return token;
    }*/



    /*

    
    class RequestResolver
    {
        public enum RequestType
        {
            Create, Edit, Delete,
        }

        static string prevRequest = "";
        public static void ResolveRequest(string request, HttpListenerResponse response)
        {
            if (request == prevRequest) { Console.WriteLine("Same request"); return; }

            request = WebUtility.UrlDecode(request);
            Console.WriteLine(request);

            if (request.StartsWith("memeName"))
            {

                string[] args = request.Split('&');
                List<string> genres = new List<string>();
                string link = "";
                string lyrics = "";
                string name = "";


                foreach (var item in args)
                {
                    if (item.Contains("memeLink=")) { link = item.Replace("memeLink=", ""); }
                    if (item.Contains("memeName=")) { name = item.Replace("memeName=", ""); }
                    if (item.Contains("lyrics=")) { lyrics = item.Replace("lyrics=", ""); }
                    if (item.Contains("genre=")) { genres.Add(item.Replace("genre=", "").Replace("+", " ")); }
                }
                Create(name, link, genres.ToArray(), lyrics);
            }
            else if (request.EndsWith("Button="))
            {
                string s = "";
                s = request.Replace("Button=", "");
                s = s.Replace("+", " ");
                GoToGenre(s, response);
            }
            else if (request.StartsWith("deleteMemeName="))
            {
                string s = "";
                s = request.Replace("deleteMemeName=", "");
                Delete(s);
            }
            //request.
        }

        public static void Create(string name, string link, string[] genres, string lyrics = "")
        {
            Console.WriteLine("Adding new meme " + name);
            BaseManager.AddMeme(genres, name, link, lyrics);
        }

        public static void GoToGenre(string genre, HttpListenerResponse response)
        {
            Console.WriteLine("GOTO " + genre);
            //Program.Response(response, genre);
            // HTMLManager.GetMemeHTMLPage(genre);
        }

        public static void Delete(string name)
        {
            Console.WriteLine("Delete " + name);
            BaseManager.DeleteMeme(name);
        }

    }*/
}




