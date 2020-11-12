using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ccierants.baseclasses;
using System.Net;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Serialization;

namespace CCIERantsPhoneControl
{
    public enum PhoneConnectionState { Connecting, Connected, Disconnected, authFail, Unreachable, NeverConnected };
    public enum PhoneListenMode { BasicMode, AdvancedMode, NotListening };

    public enum PhoneComponentAPISupport { SecondGen, ThirdGen };

    public enum PhoneScreenshotMethod { OldMethod, NewMethod };

    [Serializable]
    public class CiscoPhoneObject : INotifyPropertyChanged, IDisposable
    {
        [XmlIgnore]
        [NonSerialized]
        private TaskScheduler scheduler = null;
        [XmlIgnore]
        [NonSerialized]
        private Task<String> sendKeyTask = null;


        public delegate void sendKeyAction(string key);
        [XmlIgnore]
        [NonSerialized]
        private SimpleHTTPClient simpleHTTP = new SimpleHTTPClient();
        [XmlIgnore]
        [NonSerialized]
        private RestClientPhoneControl restClientKeyPress = new RestClientPhoneControl();
        public delegate void PhoneHTTPReqResponseEventHandler(object sender);
        public delegate void PhoneHTTPReqResponseErrorEventHandler(object sender);
        [XmlIgnore]
        [NonSerialized]
        private PhoneDeviceInformation _phoneDeviceInformation;
        [XmlIgnore]
        [NonSerialized]
        private PhoneNetworkConfiguration _phoneNetworkConfiguration;
        [XmlIgnore]
        [NonSerialized]
        private PhonePortInformation _phonePortInformation;
        [XmlIgnore]
        [NonSerialized]
        private PhoneStreamingStatistics _phoneStreamingStatistics;
        [XmlIgnore]
        [NonSerialized]
        private PhoneEthernetInformation _phoneEthernetInformation;

        [XmlIgnore]
        [NonSerialized]
        private PhoneConnectionState _connectionState;

        private string _phoneDisplayName = "Adding Entry...";
        [XmlIgnore]
        [NonSerialized]
        private string _phoneConnectivityError = "";
        [XmlIgnore]
        [NonSerialized]
        public string[] DeviceLogEntries;


        public PhoneComponentAPISupport componentAPI = PhoneComponentAPISupport.ThirdGen;

        public PhoneScreenshotMethod ScreenshotMethod = PhoneScreenshotMethod.NewMethod;
        public int NumSoftKeys = 4;

        private string isHttps = @"http://";
        private bool useHttps = false;
        public string username { get; set; }
        public string password { get; set; }
        public string ip { get; set; }

        
        [XmlIgnore]
        public PhoneConnectionState connectionState
        {

            get { return _connectionState; }



            set
            {
                if (value == PhoneConnectionState.Connected)
                {
                    
                    restClientKeyPress = new RestClientPhoneControl(ip, username, password);
                };

                // we are currently connected so need to kill the RestHTTPClient

                _connectionState = value;

            }




        }



        [NonSerialized]
        [XmlIgnore]
        public PhoneListenMode listenMode = PhoneListenMode.NotListening;
        /// <summary>
        /// 
        /// </summary>
        public void updatePhoneInformation(string path)
        {

            updatePhoneInfoAction(path);
        }
        public string updatPhoneInformationAsync(object pathobj)
        {

            object[] arrObjects = (object[])pathobj;

            string path = (string)arrObjects[0];
            this.GetPhoneInformation(path);
            
            return "";
        }
        private async void updatePhoneInfoAction(string path)
        {
            if (path.Equals(@"DeviceLog"))
            {
                using (Task<string> DeviceLogTask = new Task<string>(new Func<string>(GetPhoneLogString)))
                {
                    try
                    {
                        DeviceLogTask.Start();
                        await DeviceLogTask;

                    }
                    catch (Exception E)
                    {

                        System.Diagnostics.Debug.WriteLine("Exception retrieving info.");

                        throw new CiscoPhoneObjectException("Exception Occured Updating phoneInfoAction", this, E);


                    }

                }


            }
            else
            {
                CancellationToken token = new CancellationToken();
                CancellationTokenSource sourceToken;

                object[] arrObjects = new object[] { path };

                using (Task<string> PhoneInfoTask = new Task<string>(new Func<object, string>(GetPhoneInformationString), arrObjects, token))
                {
                    try
                    {
                        PhoneInfoTask.Start();
                        await PhoneInfoTask;

                    }
                    catch (Exception E)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception retrieving info.");
                        throw new CiscoPhoneObjectException("Exception Occured Updating phoneInfoAction", this, E);




                    }

                }


            }

        }
        public CiscoPhoneObject updatePhoneInformationReturnObject()
        {
            try
            {
                updatePhoneInfoAction("DeviceInformationX");
                return this;
            }
            catch (Exception E)
            {
                System.Diagnostics.Debug.WriteLine("Exception caught for updatePhoneInformationReturnObject: " + E.Message + E.ToString() + E.InnerException);
                throw new CiscoPhoneObjectException("UpdatePhoneInformationReturnObject", this, (CiscoPhoneObjectException)E);
            }
        }

        public PhoneDeviceInformation phoneDeviceInformation
        {
            get
            {
                return this._phoneDeviceInformation;
            }
            set
            {
                this._phoneDeviceInformation = value;
                SetPhoneGeneration();
                OnPropertyChanged("PhoneDisplayName");

            }

        }
        public PhoneNetworkConfiguration phoneNetworkConfiguration
        {
            get
            {
                return this._phoneNetworkConfiguration;
            }
            set
            {
                this._phoneNetworkConfiguration = value;
                // if this changes we should say what generation the phone is.

                OnPropertyChanged("PhoneDisplayName");

            }
        }

        private void SetPhoneGeneration()
        {
            try
            {
                if (this._phoneDeviceInformation.modelNumber.Contains("7960") || this._phoneDeviceInformation.modelNumber.Contains("7940") || this._phoneDeviceInformation.modelNumber.Contains("7905") || this._phoneDeviceInformation.modelNumber.Contains("7937") || this._phoneDeviceInformation.modelNumber.Contains("7985G") || this._phoneDeviceInformation.modelNumber.Contains("792") || this._phoneDeviceInformation.modelNumber.Contains("6921") || this._phoneDeviceInformation.modelNumber.Contains("6941") || this._phoneDeviceInformation.modelNumber.Contains("6945") || this._phoneDeviceInformation.modelNumber.Contains("6961"))
                {

                    this.componentAPI = PhoneComponentAPISupport.SecondGen;
                }
                else
                {
                    this.componentAPI = PhoneComponentAPISupport.ThirdGen;
                }
                if (this._phoneDeviceInformation.modelNumber.Contains("7960") || this._phoneDeviceInformation.modelNumber.Contains("7940") || this._phoneDeviceInformation.modelNumber.Contains("7920"))
                {

                    this.ScreenshotMethod = PhoneScreenshotMethod.OldMethod;

                }
                else
                {
                    this.ScreenshotMethod = PhoneScreenshotMethod.NewMethod;
                }

            }
            catch (Exception E)
            {
                // No harm if it isn't caught.

            }


        }
        


        public PhoneEthernetInformation phoneEthernetInformation
        {
            get
            {
                return this._phoneEthernetInformation;
            }
            set
            {
                this._phoneEthernetInformation = value;


            }
        }


        public PhoneStreamingStatistics phoneStreamingStatistics
        {
            get
            {
                return this._phoneStreamingStatistics;
            }
            set
            {
                this._phoneStreamingStatistics = value;


            }
        }


        public PhonePortInformation phonePortInformation
        {
            get
            {

                return this._phonePortInformation;
            }
            set
            {
                this._phonePortInformation = value;


            }
        }




        public int screenshotInterval { get; set; }


        public bool screenshotDisabled { get; set; }
        public bool useHTTPS
        {

            get { return useHttps; }



            set
            {
                useHttps = value;

                if (useHttps)
                {
                    isHttps = @"https://";

                }
                else
                {
                    isHttps = @"http://";
                }


            }
        }

        public string PhoneDisplayName
        {
            get
            {
                return _phoneDisplayName;
            }
        }

        public void setPhoneDisplayName(string name)
        {

        }
        public string PhoneConnectivityError
        {

            get
            {
                return _phoneConnectivityError;
            }
            set
            {
                _phoneConnectivityError = value;
            }
        }


        private string GetPhoneConnectivityError()
        {
            if (this.connectionState == PhoneConnectionState.Unreachable || this.connectionState == PhoneConnectionState.authFail)
            {
                return this.connectionState.ToString();
            }
            else
            {
                return "";
            }

        }


        public void updatePhoneDisplayName()
        {

            if (String.IsNullOrEmpty(this.phoneDeviceInformation.HostName))
            {
                // if no hostname, we don't wanna add this guy at all and instead want to show the error message.
                _phoneDisplayName = "(" + GetPhoneConnectivityError() + ")" + " (IP: " + this.ip + ")";

            }

            if (String.IsNullOrEmpty(this.phoneDeviceInformation.phoneDN))
            {
                _phoneDisplayName = this.phoneDeviceInformation.HostName + "  (IP: " + this.ip + ")";

            }
            _phoneDisplayName = this.phoneDeviceInformation.HostName + " (DN: " + this.phoneDeviceInformation.phoneDN + ") (IP: " + this.ip + ")";


        }
        public override string ToString()
        {
            return PhoneDisplayName;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            updatePhoneDisplayName();
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }
        protected void OnPhoneHTTPReqResponse()
        {
            PhoneHTTPReqResponseEventHandler handler = PhoneHTTPReqResponse;
            if (handler != null)
                handler(this);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public event PhoneHTTPReqResponseEventHandler PhoneHTTPReqResponse;
        public event PhoneHTTPReqResponseErrorEventHandler PhoneHTTPReqResponseError;

        protected void OnPhoneHTTPReqResponseError()
        {
            PhoneHTTPReqResponseErrorEventHandler handler = PhoneHTTPReqResponseError;
            if (handler != null)
                handler(this);

        }

        private void GetPhoneInformation(string path)
        {
            simpleHTTP.SendRequest(isHttps + ip + @"/" + path);
            simpleHTTP.GetResponse += GetPhoneInformation_GetResponse;
        }
        private string GetPhoneInformationString(object incomingObject)
        {

            object[] arrObjects = (object[])incomingObject;

            string path = (string)arrObjects[0];
            this.GetPhoneInformation(path);
            return "";

        }
        private void GetPhoneLog()
        {
            simpleHTTP.SendRequest(isHttps + ip + @"/DeviceLogX?0");
            simpleHTTP.GetResponse += GetPhoneLog_GetResponse;
        }
        private string GetPhoneLogString()
        {
            this.GetPhoneLog();
            return "";
        }

        private PhoneDeviceInfo getPhoneDeviceInfoCategory(string returnedData)
        {
            ///TODO: Get this code to check for shit in the response.

            if (Regex.IsMatch(returnedData, "<DeviceInformation>", RegexOptions.IgnoreCase))
                return PhoneDeviceInfo.Device_Information;
            if (Regex.IsMatch(returnedData, "<NetworkConfiguration>", RegexOptions.IgnoreCase))
                return PhoneDeviceInfo.Network_Configuration;
            if (Regex.IsMatch(returnedData, "<PortInformation>", RegexOptions.IgnoreCase))
                return PhoneDeviceInfo.Port_Information;
            if (Regex.IsMatch(returnedData, "<StreamingStatistics>", RegexOptions.IgnoreCase))
                return PhoneDeviceInfo.Streaming_Statistics;
            if (Regex.IsMatch(returnedData, "<EthernetInformation>", RegexOptions.IgnoreCase))
                return PhoneDeviceInfo.Ethernet_Information;

            throw new Exception("Could not DetermineDeviceInfoCategory. Phone may not be supported.");


        }

        private void GetPhoneLog_GetResponse(object sender, DownloadStringCompletedEventArgs e)
        {

            try
            {
                AXLXMLParser DeviceLogXML = new AXLXMLParser();

                DeviceLogEntries = DeviceLogXML.getValueSingleElement(e.Result, "status", 50);




            }
            catch (Exception E)
            {

                Console.WriteLine("Their was an error getting the Device Logs: " + E.Message);
                OnPhoneHTTPReqResponseError();
                return;


            }

            OnPhoneHTTPReqResponse();
            simpleHTTP.GetResponse -= GetPhoneLog_GetResponse;

        }
        private void GetPhoneInformation_GetResponse(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                /// now we have a response time to serialize it.
                // create the serializer object to take the info and serialize it, based on what info we requested:
                PhoneSerializer serialize = new PhoneSerializer();

                object PhoneInformation = new object();
                switch (getPhoneDeviceInfoCategory(e.Result))
                {
                    case PhoneDeviceInfo.Network_Configuration:
                        PhoneInformation = (PhoneNetworkConfiguration)serialize.SerializeXMLString(e.Result, typeof(PhoneNetworkConfiguration), PhoneDeviceInfo.Network_Configuration);
                        this.phoneNetworkConfiguration = (PhoneNetworkConfiguration)PhoneInformation;

                        break;
                    case PhoneDeviceInfo.Device_Information:
                        PhoneInformation = (PhoneDeviceInformation)serialize.SerializeXMLString(e.Result, typeof(PhoneDeviceInformation), PhoneDeviceInfo.Device_Information);
                        this.phoneDeviceInformation = (PhoneDeviceInformation)PhoneInformation;
                        break;

                    case PhoneDeviceInfo.Streaming_Statistics:
                        PhoneInformation = (PhoneStreamingStatistics)serialize.SerializeXMLString(e.Result, typeof(PhoneStreamingStatistics), PhoneDeviceInfo.Streaming_Statistics);
                        this.phoneStreamingStatistics = (PhoneStreamingStatistics)PhoneInformation;
                        break;

                    case PhoneDeviceInfo.Port_Information:
                        PhoneInformation = (PhonePortInformation)serialize.SerializeXMLString(e.Result, typeof(PhonePortInformation), PhoneDeviceInfo.Port_Information);
                        this.phonePortInformation = (PhonePortInformation)PhoneInformation;
                        break;
                    case PhoneDeviceInfo.Ethernet_Information:
                        PhoneInformation = (PhoneEthernetInformation)serialize.SerializeXMLString(e.Result, typeof(PhoneEthernetInformation), PhoneDeviceInfo.Ethernet_Information);
                        this.phoneEthernetInformation = (PhoneEthernetInformation)PhoneInformation;
                        break;


                    default:
                        simpleHTTP.GetResponse -= GetPhoneInformation_GetResponse;
                        break;

                }


                OnPropertyChanged("PhoneDisplayName");
                simpleHTTP.GetResponse -= GetPhoneInformation_GetResponse;
                OnPhoneHTTPReqResponse();


            }
            catch (Exception E)
            {

                OnPropertyChanged("PhoneDisplayName");

                simpleHTTP.GetResponse -= GetPhoneInformation_GetResponse;


                OnPhoneHTTPReqResponseError();


            }
        }


        public async void sendKey(string key)
        {
            if (this._connectionState == PhoneConnectionState.Connected)
            {

                try
                {

                    if(sendKeyTask == null)
                    {
                        sendKeyTask = new Task<String>(new Func<String>(restClientKeyPress.MakeRequest));
                    }

                        
                    
                    while (sendKeyTask.Status == TaskStatus.Running)
                    {
                        // Waiting for the previous task to finish

                    }

                    sendKeyTask = new Task<String>(new Func<String>(restClientKeyPress.MakeRequest));
                    restClientKeyPress.EndPoint = isHttps + ip + @"/CGI/Execute";
                    restClientKeyPress.Method = HttpVerb.POST;
                    string XML = "XML=<CiscoIPPhoneExecute><ExecuteItem URL=\"Key:" + key + "\"/></CiscoIPPhoneExecute>";
                    restClientKeyPress.PostData = XML;




                    sendKeyTask.Start();
                    string responseResult = await sendKeyTask;








                }
                catch (Exception E)
                {

                    OnPhoneHTTPReqResponseError();

                }
            }
        }






        public void Dispose()
        {
            restClientKeyPress.Dispose();

            this.Dispose();

        }
    }

}
