using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace neo4j_2nd_try
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // BaseManager.StartBase();
            //Task.Run(Listen);
            Task.Run(() => StartTCPServer());
            Console.Read();

            Console.WriteLine("Обработка завершена");
        }

        private static void StartTCPServer()
        {
            TcpListener listner = new TcpListener(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12000));
            listner.Start();
            while (true)
            {
                TcpClient client = listner.AcceptTcpClient();

                StreamReader sr = new StreamReader(client.GetStream());

                string request = sr.ReadLine();
                Console.WriteLine("(\n" + request + "\n)");

                string htmlPage = RequestResolver.ResolveRequest(request);

                StreamWriter sw = new StreamWriter(client.GetStream());
                sw.AutoFlush = true;
                sw.WriteLine(htmlPage);

                client.Close();
            }
        }
    }

    internal class HTMLManager
    {
        public static string GetMemeHTMLPage(string memeClass = "wolf memes")
        {
            Console.WriteLine("send page with " + memeClass);

            string js = "\n" +
               "<script type=\"text/javascript\">" +

"let buttonCountMinus = document.getElementById(\"buttonCountMinus\");" +
"let count = document.getElementById(\"buttonCountNumber\");" +
"let count2 = document.getElementById(\"num\");let number = 1;let price = 350;" +
 "function httpGet1(data){ var http = new XMLHttpRequest();var url = 'http://localhost:8888/';http.open('GET', url, true);http.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');" +
"http.onreadystatechange = function() { if (http.readyState == 4 && http.status == 200) { alert('ready',http.responseText) } }\n" +
  "http.send(data); alert(http.responseText);}; let buttonCountPlus = document.getElementById(\"buttonCountPlus\");" +
"function httpGet(data){ var xmlHttp = new XMLHttpRequest(); xmlHttp.onreadystatechange = function () { if (xmlHttp.readyState == 4 && xmlHttp.status == 200) { document.getElementById(\"output\").innerHTML=xmlHttp.responseText; } }; xmlHttp.open('POST', 'http://localhost:8888/', true);  xmlHttp.send(data);  alert(xmlHttp.responseText);} " +
"buttonCountPlus.onclick = function() { if (number <= 3) { number++; count.innerHTML = number; count2.value = number * price;  httpGet1(number)  } };" +
           " buttonCountMinus.onclick = function() { if (number >= 2) { number--; count.innerHTML = number; count2.value = number * price; httpGet(); } };" +
       "\n</script>\n";

            string responseStart = "<html><head><meta charset='utf8'><title>Meme page</title></head><body>Привет мир!";

            string inputField = "<p><input type=\"text\" id=\"mainMemeInput\">  <button id=\"find\" onclick=\"find()\">find</button></p>";

            string headerField = "<input type=\"button\" id=\"buttonCountPlus\" value=\" + \">" +
"<input type = \"button\" id = \"buttonCountMinus\" value = \"-\" >" +
     "<div id = \"buttonCountNumber\" > 1 </div >" +
      "<input type = \"hidden\" value = \"1\" id = \"num\" name = \"num\" > ";

            string inputField2 = "<form method=\"POST\" action=\"http://localhost:8888/connection?post=gggkkk/\">" +
         " <p> Текст:<br><textarea name = \"message\"></textarea></p>" +

               " <p><button type = \"submit\"> Отправить </button></p>" +
                " </form> ";

            string ing3
                =

                "<form method=\"POST\" name=\"authorization\" action=\"http://localhost:8888/connection\"> " +

                "<form action=\"\" method=\"POST\">" +
            CreateInputField("Удалить", "deleteMemeName") + "<br><br><br><br>" +
                "</form>" +

            CreateInputField("Название", "memeName") + "<br>" +
           CreateInputField("Ссылка", "memeLink") + "<br>" +
           CreateInputField("Текст", "lyrics") + "<br>" +
           CreateSelector("selectorOFGenres", "genre", BaseManager.getGenresNames(), true) +

                 " <input type = \"submit\" value = \"Cоздать\">" +
               "  </form> ";

            string responseEnd = "</body></html>";

            string memeClasses = "meme classes:: ";
            memeClasses += "\n";//"<form action=\"\" method=\"POST\">";
            foreach (var g in BaseManager.RequestNodes("match (n:memeClass) return n"))
            {
                memeClasses +=
                        //"<form action=\"\" method=\"POST\">"+
                        $"<button name = \"{g.Properties["name"] + "Button"}\" getThisMemeGenre=\"{g.Properties["name"]}\" onclick=\"httpGet('{g.Properties["name"]}')\"> {g.Properties["name"]} </button>";

                // Console.WriteLine(g.Properties["name"]);
            }
            memeClasses += "\n";// "</form>";

            string responseStr = responseStart + headerField + ing3 + memeClasses + "<br>" + "<div id='output'>" + GetMemesHTML(memeClass) + "</div>" + js + responseEnd;

            return responseStr;
        }

        public static string GetMemesHTML(string memeClass)
        {
            string responseMemes = "";
            string memeFormat = "<img src=\"{0}\" alt=\"{1}\">";
            foreach (var m in BaseManager.RequestNodes($"MATCH (n:memeClass{{name:\"{memeClass}\"}})<-[]-(m:meme) RETURN m LIMIT 25"))
            {
                responseMemes += string.Format(memeFormat, m.Properties["link"], "theres no meme"/* m.Properties["lyrics"]*/);
            }
            return responseMemes;
        }

        private static string CreateInputField(string description, string name, string type = "text")
        {
            string format = $"{description} <input type = \"{type }\" name = \"{name}\">";
            return format;
        }

        private static string CreateSelector(string name, string argName, string[] options, bool multiplie = false)
        {
            string s = "";
            s += $"<form method=\"POST\" name=\"{name}\" action=\"http://localhost:8888/connection\"> " +
                 $"<p><select {(multiplie ? "multiple" : "")} name = \"{argName}\">";

            // " <option disabled > Выберите классы </ option>"+
            for (int i = 0; i < options.Length; i++)
            {
                s += CreateOption(options[i], options[i]);
            }

            s +=
                      " </select ></p >" +
                      " <p><input type = \"submit\" value = \"Отправить\"></p>" +
                           "</form> ";

            return s;
        }

        private static string CreateOption(string value, string description, bool selected = false)
        {
            return $"<option {(selected ? "selected" : "")} value = \"{value}\"> {description} </option >";
        }
    }

    internal class BaseManager
    {
        public static void StartBase()
        {
            Console.WriteLine("Try to init base");

            foreach (var g in RequestNodes("match (n:memeClass) return n"))
            {
                //  memeClasses += $"<button> {g.Properties["name"]} </button>";
                Console.WriteLine(g.Properties["name"]);
            }

            foreach (var m in RequestNodes("MATCH (n:memeClass{name:\"wolf memes\"})<-[]-(m:meme) RETURN m LIMIT 25"))
            {
                Console.WriteLine(m.Properties["link"]/*+" "+ m.Properties["lyrics"]*/);
            }

            Console.WriteLine("Base is ready");
        }

        public static string[] getGenresNames()
        {
            List<string> genres = new List<string>();
            foreach (var g in RequestNodes("match (n:memeClass) return n"))
            {
                genres.Add(g.Properties["name"] + "");
            }
            return genres.ToArray();
        }

        public static IResult getResult(string req)
        {
            var driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "123"));
            var session = driver.Session();
            return session.Run(req);
        }

        public static List<INode> RequestNodes(string req)
        {
            var result = getResult(req);

            List<INode> reqNodes = new List<INode>();

            foreach (var r in result)
            {
                foreach (var k in r.Values)
                {
                    INode node = (INode)k.Value;
                    reqNodes.Add(node);
                }
            }

            return reqNodes;
        }

        public static void AddMeme(string[] memeClass, string memeName, string memeLink, string memeLyrics = "")
        {
            if (getAllMemeNames().Contains(memeName)) { Console.WriteLine($"meme {memeName} already exists"); return; }
            Console.WriteLine();
            string req = $"match (n:memeClass{{name:\"{memeClass[0]}\"}}) create (m :meme{{memeId:\"{memeName}\",link:\"{memeLink}\",lyrics:\"{memeLyrics}\"}})-[:is]->(n)";

            for (int i = 1; i < memeClass.Length; i++)
            {
                getResult($"match (n:memeClass{{name:\"{memeClass[i]}\"}}),(m:meme{{memeId:\"{memeName}\"}}) MERGE (m)-[:is]->(n)");
            }

            Console.WriteLine(req);
            getResult(req);
        }

        public static List<string> getAllMemeNames()
        {
            List<string> memeNames = new List<string>();
            foreach (var m in RequestNodes("MATCH (m:meme) return (m)"))
            {
                memeNames.Add(m["memeId"] + "");
            }

            return memeNames;
        }

        public static void EditMeme(string idtf, string changingProp, string newPropVal)
        {
            string req = $"match (n{{{idtf}}}) SET n.{changingProp}=\"{ newPropVal }\"";
            //string req= $"match (n{{link:"121"}}) SET n.memeId="2228"";
        }

        public static void DeleteMeme(string idtf)
        {
            string req = $"match (n{{{idtf}}}) delete n";
            getResult(req);
            //string req= $"match (n{{link:"121"}}) SET n.memeId="2228"";
        }
    }

    internal class RequestResolver
    {
        public enum RequestType
        {
            Create, Edit, Delete,
        }

        private static string prevRequest = "";

        public static string ResolveRequest(string request)
        {
            if (request == prevRequest) { Console.WriteLine("Same request"); return HTMLManager.GetMemeHTMLPage("memes"); ; }
            int m = 0;

            if (int.TryParse(request, out m))
            {
                //return "memeID="+m;
            }
            request = WebUtility.UrlDecode(request);
            Console.WriteLine(request);

            if (new List<string>(BaseManager.getGenresNames()).Contains(request)) { return HTMLManager.GetMemesHTML(request); }

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
                return GoToGenre(s);
            }
            else if (request.StartsWith("deleteMemeName="))
            {
                string s = "";
                s = request.Replace("deleteMemeName=", "");
                Delete(s);
            }

            return HTMLManager.GetMemeHTMLPage("memes");
            //request.
        }

        public static void Create(string name, string link, string[] genres, string lyrics = "")
        {
            Console.WriteLine("Adding new meme " + name);
            BaseManager.AddMeme(genres, name, link, lyrics);
        }

        public static string GoToGenre(string genre)
        {
            Console.WriteLine("GOTO " + genre);
            //Program.Response(response, genre);
            return HTMLManager.GetMemeHTMLPage(genre);
        }

        public static void Delete(string name)
        {
            Console.WriteLine("Delete " + name);
            BaseManager.DeleteMeme(name);
        }
    }
}