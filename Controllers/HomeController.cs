using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SSL.Models;

namespace SSL.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            // No url passed - return error message
            if (Request.QueryString.AllKeys.All(p => p != "u"))
                return View("Index",new HomeModel("Parameter 'u' not passed - you must pass u with an encoded url"));

            try
            {
                // Decode URL
                var url = HttpUtility.UrlDecode(Request.QueryString[Request.QueryString.AllKeys.First()]);
                if (string.IsNullOrEmpty(url))
                    return View("Index",new HomeModel($"Error decoding url: {Request.QueryString["u"]}"));

                //var parsedUrl = new Uri(url);

                // Request
                var request = WebRequest.Create(url);

                // Pass headers to the request
                //foreach (var key in System.Web.HttpContext.Current.Request.Headers.AllKeys)
                //{
                //    if (!WebHeaderCollection.IsRestricted(key) && key!="Accept-Encoding")
                //        request.Headers.Add(key,System.Web.HttpContext.Current.Request.Headers[key]);
                //}
                //request.Headers["Accept-Encoding"] = "gzip;q=0,deflate,sdch";

                // Re-pass headers to our Response
                var response = request.GetResponse();
                foreach (var key in response.Headers.AllKeys)
                {
                    switch (key)
                    {
                        case "Content-Type":
                            System.Web.HttpContext.Current.Response.ContentType = response.Headers[key];
                            break;
                        case "Content-Encoding":
                            System.Web.HttpContext.Current.Response.Headers[key] = "identity";
                            //identity
                            break;
                        case "Content-Disposition":
                        case "Date":
                        case "Expires":
                            System.Web.HttpContext.Current.Response.Headers[key] = response.Headers[key];
                            break;
                        case "Set-Cookie":
                        case "Connection":
                        case "Content-Length":
                            break;
                        default:
                            //System.Web.HttpContext.Current.Response.Headers[key] = response.Headers[key];
                            break;
                    }
                }

                // Read request stream
                var enc = Encoding.GetEncoding(1252);  // Windows default Code Page
                var stream = response.GetResponseStream();
                if (stream==null)
                    return View("Index",new HomeModel($"Error reading stream for url: {url}"));
                var streamReader = new StreamReader(stream,enc);

                // Write to output response lines
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    if (string.IsNullOrEmpty(line))
                        continue;
                    System.Web.HttpContext.Current.Response.Write(line);
                }
            }
            catch (Exception e)
            {
                return View("Index",new HomeModel($"[Error processing request] {e.Message}"));
            }
            
            return null;
        }
    }
}