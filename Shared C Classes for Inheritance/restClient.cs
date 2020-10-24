using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;

public enum HttpVerb
{
    GET,
    POST,
    PUT,
    DELETE
}

namespace QuickAndSimpleSIPTrunkConfigGenerator
{
    public class RestClient
    {
        private string cucmusername;
        private string cucmpassword;
        public string EndPoint { get; set; }
        public HttpVerb Method { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }

        public string PutData { get; set; }

        public FileStream PutDataFileStream { get; set; }
        

        public RestClient()
        {

            cucmusername = "axluser";
            cucmpassword = "axlpassword";

            EndPoint = "";
            Method = HttpVerb.GET;
            ContentType = "text/xml";
            PostData = "";
        }
        public RestClient(string endpoint)
        {
            cucmusername = "axluser";
            cucmpassword = "axlpassword";

            EndPoint = endpoint;
            Method = HttpVerb.GET;
            ContentType = "text/xml";
            PostData = "";
        }
        public RestClient(string endpoint, HttpVerb method)
        {
            cucmusername = "axluser";
            cucmpassword = "axlpassword";

            EndPoint = endpoint;
            Method = method;
            ContentType = "text/xml";
            PostData = "";
        }

        public RestClient(string endpoint, HttpVerb method, string postData)
        {
            cucmusername = "axluser";
            cucmpassword = "axlpassword";

            EndPoint = endpoint;
            Method = method;
            ContentType = "text/xml";
            PostData = postData;
        }


   
        public string MakeRequest()
        {

            System.Net.ServicePointManager.CertificatePolicy = new BruteForcePolicy();

            var request = (HttpWebRequest)WebRequest.Create(EndPoint);

            request.Method = Method.ToString();
            request.ContentLength = 0;
            request.ContentType = ContentType;

            request.Credentials = new System.Net.NetworkCredential(cucmusername, cucmpassword);

            
            if (!string.IsNullOrEmpty(PostData) && Method == HttpVerb.POST)
            {
                var encoding = new UTF8Encoding();
                var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(PostData);
                request.ContentLength = bytes.Length;

                using (var writeStream = request.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }
            }

            if (!string.IsNullOrEmpty(PutData) && Method == HttpVerb.PUT)
            {
                

                var encoding = new UTF8Encoding();
                var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(PutData);
                request.ContentLength = bytes.Length;
                
                using (var writeStream = request.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }


            }
            if (!Stream.Equals(null,PutDataFileStream)  && Method == HttpVerb.PUT)
            {
                System.Diagnostics.Debug.WriteLine("Length of PUT data is: " + PutDataFileStream.Length);
                request.ContentLength = PutDataFileStream.Length;
            
                // so writeStream is the stream we are going to write to.



                byte[] filecontent = new byte[PutDataFileStream.Length];


                PutDataFileStream.Read(filecontent, 0, (int)PutDataFileStream.Length);


                System.Diagnostics.Debug.WriteLine("Length of filecontent is: " + filecontent.Length);



                using (var writeStream = request.GetRequestStream())
                {

                    writeStream.WriteAsync(filecontent, 0, filecontent.Length);

                    writeStream.Close();

                    // request.


                }


            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseValue = string.Empty;

                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
                {
                    var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }


                if (response.StatusCode == HttpStatusCode.NoContent)
                {

                    return "Upload Succesful using Put Data, returned message was 204 NoContent";

                }


                // grab the response
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                        }
                }


                
                return responseValue;
            }
        }

        public CallHandlerData[] GetCallHandlers()
        {

            CallHandlerData[] returnedcallhandle = new CallHandlerData[1000];
            System.Net.ServicePointManager.CertificatePolicy = new BruteForcePolicy();

            var request = (HttpWebRequest)WebRequest.Create(EndPoint);

            request.Method = Method.ToString();
            request.ContentLength = 0;
            request.ContentType = ContentType;

            request.Credentials = new System.Net.NetworkCredential(cucmusername, cucmpassword);


            if (!string.IsNullOrEmpty(PostData) && Method == HttpVerb.POST)
            {
                var encoding = new UTF8Encoding();
                var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(PostData);
                request.ContentLength = bytes.Length;

                using (var writeStream = request.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }


                // grab the response
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {



                        using (var reader = new StreamReader(responseStream))
                        {

                            XDocument xmlDoc = new XDocument();
                            try
                            {
                                xmlDoc = XDocument.Parse(reader.ReadToEnd());
                                string RootElement = xmlDoc.Root.Element("Callhandler").Name.ToString();

                                System.Diagnostics.Debug.WriteLine("Root Element Value: " + RootElement);
                                int i = 0;

                                foreach (XElement xcallhandler in xmlDoc.Root.Elements("Callhandler"))
                                {
                                    returnedcallhandle[i].chname = xcallhandler.Element("DisplayName").Value;
                                    returnedcallhandle[i].chlangid = xcallhandler.Element("Language").Value;
                                    returnedcallhandle[i].chURI = xcallhandler.Element("URI").Value;
                                    returnedcallhandle[i].chGreetingURI = xcallhandler.Element("GreetingsURI").Value;
                                    returnedcallhandle[i].chObjectID = xcallhandler.Element("ObjectId").Value;
                                    System.Diagnostics.Debug.WriteLine("");

                                    System.Diagnostics.Debug.WriteLine("Call Handler Display name: " + returnedcallhandle[i].chname);
                                    System.Diagnostics.Debug.WriteLine("Call Handler Language ID: " + returnedcallhandle[i].chlangid);
                                    System.Diagnostics.Debug.WriteLine("Call Handler URI: " + returnedcallhandle[i].chURI);
                                    System.Diagnostics.Debug.WriteLine("Call Handler Greetings URI: " + returnedcallhandle[i].chGreetingURI);
                                    System.Diagnostics.Debug.WriteLine("Call handler object id: " + returnedcallhandle[i].chObjectID);
                                    i++;


                                }

                                return returnedcallhandle;



                            }
                            catch (Exception)
                            {
                                System.Diagnostics.Debug.WriteLine("An error occured in the call handler retrival");
                                // handle if necessary
                            }
                        }
                    }


                }

                return returnedcallhandle;
            }
        }


    } // class

}
