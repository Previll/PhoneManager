using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml;
using System.Web;
namespace ccierants.baseclasses
{
  
    class SimpleHTTPClient
    {
        public event EventHandler<DownloadStringCompletedEventArgs> GetResponse;
        

        /// <summary>
        /// GetRequest will send the request, you then subscribe to the getResponse event 
        /// </summary>
        /// <param name="fullurl">the full url including https:// or http://</param>
        public void SendRequest(string fullurl) 
        {

            WebClient wc = new WebClient();
            
            wc.DownloadStringCompleted += HttpCompleted;
            wc.DownloadStringAsync(new Uri(fullurl));

        }
        /// <summary>
        /// GetRequest will send the request, you then subscribe to the getResponse event 
        /// </summary>
        /// <param name="fullurl">the full url including https:// or http://</param>
        
        public void SendRequest(string fullurl, string username, string password)
        {

            WebClient wc = new WebClient();
            wc.Credentials = new System.Net.NetworkCredential(username, password);
            wc.DownloadStringCompleted += HttpCompleted;
            wc.DownloadStringAsync(new Uri(fullurl));

        }

        private void HttpCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
        
            if (e.Error == null)
            {

            System.Diagnostics.Debug.WriteLine(e.Result);

            EventHandler<DownloadStringCompletedEventArgs> RaiseEvent = GetResponse;

            GetResponse(this, e);
            
            

        
            }
            
        }
        

    }
}

