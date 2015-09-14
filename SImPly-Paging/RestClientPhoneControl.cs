using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Net;
using System.IO;

namespace SImPly_Paging
{
    public class RestClientPhoneControl : ccierants.baseclasses.RestClient
    {
        public RestClientPhoneControl(string endpoint, string username, string password)
        {
            cucmusername = username;
            cucmpassword = password;

            EndPoint = endpoint;
            Method = HttpVerb.GET;
            ContentType = "text/xml";
            PostData = "";
            
           
        }
        public Image getScreenshot()
        {
            Console.WriteLine("Getting screenshot from this URL: " + EndPoint);
            
            System.Net.ServicePointManager.CertificatePolicy = new BruteForcePolicy();

            
            
            
            var request = (HttpWebRequest)WebRequest.Create(EndPoint);

            request.Method = Method.ToString();
            request.ContentLength = 0;
            request.ContentType = ContentType;

            request.Credentials = new System.Net.NetworkCredential(cucmusername, cucmpassword);
            request.KeepAlive = false;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseValue = string.Empty;

                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
                {
                    var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }



                // grab the response
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        Image bitmapscreenshot = Image.FromStream(responseStream);
                        return bitmapscreenshot;
                    }
                }
                

            }
            throw new  Exception("Couldn't get bitmap stream");
        }

    }
}
