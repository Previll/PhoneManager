using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;



namespace SImPly_Paging
{
    class AXLConnectorPhoneControl : ccierants.baseclasses.AXLConnector
    {
        
        public AXLConnectorPhoneControl(string defaultip, string defaultuser, string defaultpass) 
        {

            cucmip = defaultip;
            cucmusername = defaultuser;
            cucmpassword = defaultpass;
            cucmhttppath = ":8443/realtimeservice2/services/RISService70";


        }
        public AXLConnectorPhoneControl(string defaultip, string defaultuser, string defaultpass, string contenttype, string header)
        {

            cucmip = defaultip;
            cucmusername = defaultuser;
            cucmpassword = defaultpass;
            cucmhttppath = ":8443/realtimeservice2/services/RISService70";
            cucmheaders = header;
            cucmcontenttype = contenttype;


        }
        public override string getSoapHeader()
        {

            // This returns the standard SOAP header 

            string soapheader;

            soapheader = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:soap=\"http://schemas.cisco.com/ast/soap\">";

            soapheader += "<soapenv:Header/>";
            soapheader += "<soapenv:Body>";



            Debug.WriteLine("Soap Header: " + soapheader);


            return soapheader;
        }



        public string selectCmDeviceReturnIP(string devicename)
        {
//            
      //<soap:selectCmDevice>
      //      <soap:StateInfo></soap:StateInfo>
      //   <soap:CmSelectionCriteria>
      //     <soap:MaxReturnedDevices>1000</soap:MaxReturnedDevices>
      //      <soap:DeviceClass>Any</soap:DeviceClass>
      //      <soap:Model></soap:Model>
      //      <soap:Status>Any</soap:Status>
      //      <soap:NodeName></soap:NodeName>
      //      <soap:SelectBy>Name</soap:SelectBy>
      //      <soap:SelectItems>
      //         <!--Zero or more repetitions:-->
      //         <soap:item>
      //            <soap:Item>SEP*</soap:Item>
      //         </soap:item>
      //      </soap:SelectItems>
      //          <soap:Protocol>Any</soap:Protocol>
      //      <soap:DownloadStatus>Any</soap:DownloadStatus>
      //   </soap:CmSelectionCriteria>
      //</soap:selectCmDevice>

            string soapreq;



            soapreq = this.getSoapHeader();

            soapreq += @"<soap:SelectCmDeviceExt>
            <soap:StateInfo></soap:StateInfo>
         <soap:CmSelectionCriteria>
           <soap:MaxReturnedDevices>1000</soap:MaxReturnedDevices>
            <soap:DeviceClass>Any</soap:DeviceClass>
            <soap:Model></soap:Model>
            <soap:Status>Any</soap:Status>
            <soap:NodeName></soap:NodeName>
            <soap:SelectBy>Name</soap:SelectBy>
            <soap:SelectItems>
               <!--Zero or more repetitions:-->
               <soap:item>
                  <soap:Item>" + devicename + @"</soap:Item>
               </soap:item>
            </soap:SelectItems>
                <soap:Protocol>Any</soap:Protocol>
            <soap:DownloadStatus>Any</soap:DownloadStatus>
         </soap:CmSelectionCriteria>
      </soap:SelectCmDeviceExt>";

            soapreq += base.getSoapFooter();

            System.Diagnostics.Debug.WriteLine("Full SOAP request for SRST is: " + soapreq);

            XDocument AxlResponseXMLDocument = sendAXLCommandReturnXML(soapreq);

            XNamespace ns = "http://schemas.cisco.com/ast/soap";

            string phoneip = "";

            foreach (var element in AxlResponseXMLDocument.Descendants(ns.GetName("IP")))
            {
                Debug.WriteLine("ELement Name: " + element.Name);
                Debug.WriteLine("Namespace: " + element.Name.NamespaceName);
                Debug.WriteLine("Local Namespace: " + element.Name.LocalName);
                Debug.WriteLine("Element Value: " + element.Value);
                phoneip = element.Value;

            }

            return phoneip;




        }
    }
}
