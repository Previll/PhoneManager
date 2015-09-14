using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json;
using System.Xml;
namespace ccierants.baseclasses
{
    class PhoneSerializer
    {
        public object SerializeXMLString(string xmlstring, Type returnobject, PhoneDeviceInfo info)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlstring);

            Console.WriteLine("XML Node 0: " + doc.ChildNodes[0].Name);
            Console.WriteLine("XML Node 1: " + doc.ChildNodes[1].Name);

            string json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(doc.ChildNodes[1]);

            json = json.Replace("{\"NetworkConfiguration\":", "");
            json = json.Replace("}}", "}");

            json = json.Replace("{\"DeviceInformation\":", "");
            json = json.Replace("{\"StreamingStatistics\":", "");

            json = json.Replace("{\"PortInformation\":", "");

            json = json.Replace("{\"EthernetInformation\":", "");




            Console.WriteLine("JSON Output: " + json);


            object returned = new object();

            switch (info)
            {
                case PhoneDeviceInfo.Network_Configuration:

                    returned = Newtonsoft.Json.JsonConvert.DeserializeObject<PhoneNetworkConfiguration>(json);

                    break;
                case PhoneDeviceInfo.Device_Information:

                    returned = Newtonsoft.Json.JsonConvert.DeserializeObject<PhoneDeviceInformation>(json);
                    
                    break;

                case PhoneDeviceInfo.Ethernet_Information:
                    returned = Newtonsoft.Json.JsonConvert.DeserializeObject<PhoneEthernetInformation>(json);
                    
                    break;

                case PhoneDeviceInfo.Streaming_Statistics:
                    returned = Newtonsoft.Json.JsonConvert.DeserializeObject<PhoneStreamingStatistics>(json);
                    break;

                case PhoneDeviceInfo.Port_Information:
                    returned  = Newtonsoft.Json.JsonConvert.DeserializeObject<PhonePortInformation>(json);
                    break;

                case PhoneDeviceInfo.Device_Log:
                    returned = Newtonsoft.Json.JsonConvert.DeserializeObject<PhonePortInformation>(json);
                    break;

                 
                default:
                    Console.WriteLine("nothing selected");
                    throw new Exception("PhoneSerializer did not recognize the struct for serializaton");
                    
            }


            return returned;
        }
    }
}
