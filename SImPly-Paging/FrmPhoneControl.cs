using JulMar.Tapi3;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using oldtapi = JulMar.Atapi;
using ccierants.baseclasses;
using System.Net;
using System.Web;
using NAudio.Wave;
using NAudio.FileFormats.Wav;

using System.IO;

namespace SImPly_Paging
{

    public partial class frmMainScreen : Form
    {
        private TTapi TapiMgr = new TTapi();

        private PhoneDeviceInfo selectedphoneinfo;

        delegate void updateScreenshot();

        private RTPClient RTPReceive;

        private BinaryReader reader;
        private TCall currCall;
        private TCall ansCall;
        private bool onCall = true;
        private answeredCall answeredCall = new answeredCall();
        private bool autoanswer = true;
        private bool callOnHold = false;
        private WaveFormat waveFormat = WaveFormat.CreateMuLawFormat(8000, 1);
        private TTerminal playbackTerminal;
        private TAddress ciscoIPEndpoint;
        private TStream callStream;
        private oldtapi.TapiManager tapi2 = new oldtapi.TapiManager("SImPly-Paging.exe");
        private bool btnAnswerRed;
        private WaveOut wavOut = new WaveOut();
        private RestClientPhoneControl RestClient;

        private string restuser; // this is the user who has access to ALL phones (controls all phones)
        private string restpass; // same as above obviously but password


        // this is what device we are currently controlling.
        private string devicename;


        private RISPhoneInfo phoneinfo;
        // this is the IP address of the phone we are controlling, we might want a whole bunch of other info soon.

        private UserSettings currentSettings;

        private PhoneNetworkConfiguration phoneNetConfig;
        private Image screenshot;
        private CustomPlayer wavPlayer = new CustomPlayer();


        public frmMainScreen()
        {
            InitializeComponent();



        }
        private void RTPReceive_packetReceived(object sender, RTPPacketReceievedEventArgs e)
        {
            wavPlayer.AddSamples(e.packetpayload);

            Console.WriteLine("Packet event fired");

        }
        private void notifyOfCall(object sender, TapiCallNotificationEventArgs notificationOfCall)
        {
            if (answeredCall.isOnCall)
            {
                // The call was already answered;
                Debug.WriteLine("Call already exists");
                return;


            }

            if (notificationOfCall.Call.CallState == CALL_STATE.CS_OFFERING)
            {
                Debug.WriteLine("A call came in and was offered. ");

                // The current call is now the call we are about to answer;
                ansCall = notificationOfCall.Call;


                if (autoanswer)
                {
                    Debug.WriteLine("Auto Answer");






                }

                else
                {
                    tmrFlash.Enabled = true;
                    btnAnswer.Enabled = true;

                }

                return;
            }

            Debug.WriteLine("Call state was: " + notificationOfCall.Call.CallState.ToString());




        }
        private void callInfoChange(object sender, TapiCallInfoChangeEventArgs incomingCallInfo)
        {

            Debug.WriteLine("Call Info Changed.");

            Debug.WriteLine("call info: " + incomingCallInfo.Cause.ToString());
            if (incomingCallInfo.Call.CallState == CALL_STATE.CS_CONNECTED)
            {
                Debug.WriteLine("Call Connected");
                answeredCall.isOnCall = true;
                currCall = incomingCallInfo.Call;
                btnHold.Enabled = true;
            }


            else if (incomingCallInfo.Call.CallState == CALL_STATE.CS_DISCONNECTED && answeredCall.isOnCall)
            {

                Debug.WriteLine("Call was hungup");
                currCall = null;
                answeredCall.isOnCall = false;
                btnHold.Enabled = false;

            }
            else
            {
                Debug.WriteLine("Call not connected at this time");
                answeredCall.isOnCall = false;
                btnHold.Enabled = false;
            }

        }

        private void mediaChange(object sender, TapiCallMediaEventArgs mediaEvent)
        {

        }
        private void frmMainScreen_Load(object sender, EventArgs e)
        {


            lblValue.Text = FingerPrint.Value();


            // #if DEBUG
            txtCallNumber.Text = "9101";
            restuser = "peter";
            restpass = "peter";
            // #endif


            currentSettings = Properties.Settings.Default.MainSettingClass;
            try
            {

                if (currentSettings.settingsstarted)
                {


                }
                else
                {
                    // User hasn't even entered any settings yet
                    Debug.WriteLine("No settings Entered");
                    currentSettings = new UserSettings();

                }


            }


            catch (Exception E)
            {
                // User hasn't even entered any settings yet
                Debug.WriteLine("No settings Entered");
                currentSettings = new UserSettings();

            }





        }

        private void btnHold_Click(object sender, System.EventArgs e)
        {


            Console.WriteLine("Current on call variable: " + this.answeredCall);

            if (currCall != null && !callOnHold)
            {
                currCall.Hold(true);
                callOnHold = true;
                btnHold.Text = "Unhold";

            }
            else if (currCall != null && callOnHold)
            {

                currCall.Hold(false);
                callOnHold = false;
                btnHold.Text = "Hold";

            }

            wrkScreenshot.RunWorkerAsync();

        }


        private void btnListView_Click(object sender, EventArgs e)
        {
            try
            {


                TapiMgr.Initialize();



                this.TapiMgr.TE_CALLNOTIFICATION += new System.EventHandler<TapiCallNotificationEventArgs>(this.notifyOfCall);
                this.TapiMgr.TE_CALLINFOCHANGE += new System.EventHandler<TapiCallInfoChangeEventArgs>(this.callInfoChange);
                this.TapiMgr.TE_CALLMEDIA += new System.EventHandler<TapiCallMediaEventArgs>(this.mediaChange);

                foreach (TAddress addr in TapiMgr.Addresses)
                {
                    if ((addr.MediaTypes & TAPIMEDIATYPES.AUDIO) != 0)
                        cmbProviders.Items.Add(addr.AddressName);


                    Debug.WriteLine("TapiMgr.Addresses: " + addr.AddressName);

                }

            }
            catch (Exception error)
            {

                MessageBox.Show("The error for TAPI init was: " + error);
            }

            try
            {

                tapi2.Initialize();

                foreach (oldtapi.TapiLine line in tapi2.Lines)
                {

                    Debug.WriteLine("Line : " + line.Name);
                }
            }
            catch (oldtapi.TapiException errorold)
            {
                MessageBox.Show(errorold.Message);
            }

        }

        private void btnCall_Click(object sender, EventArgs e)
        {



            currCall = ciscoIPEndpoint.CreateCall(txtCallNumber.Text, LINEADDRESSTYPES.PhoneNumber, TAPIMEDIATYPES.AUDIO);

            if (currCall != null)
            {


                try
                {
                    // Connect the call
                    currCall.Connect(false);
                    Debug.WriteLine("Placing call");
                }
                catch (TapiException ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                try
                {




                    if (playbackTerminal != null)
                    {

                        //    currCall.SelectTerminalOnCall(playbackTerminal);
                    }
                    else
                    {
                        MessageBox.Show("Failed to retrieve playback terminal.");
                    }
                }
                catch (TapiException Ex)
                {

                    MessageBox.Show(Ex.Message);

                }


            }
            wrkScreenshot.RunWorkerAsync();

        }

        private void tmrFlash_Tick(object sender, EventArgs e)
        {

            if (btnAnswerRed)
            {

                btnAnswer.BackColor = System.Drawing.SystemColors.Control;
                btnAnswerRed = false;

            }
            else
            {


                btnAnswer.BackColor = System.Drawing.Color.Red;
                btnAnswerRed = true;
            }
        }

        private void cmbProviders_SelectedIndexChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("Selected index of cmb box: " + cmbProviders.SelectedIndex);


            ciscoIPEndpoint = TapiMgr.Addresses[5];



            Debug.WriteLine("The Name of the device: " + ciscoIPEndpoint.AddressName);
            Debug.WriteLine("Is DND on?: " + ciscoIPEndpoint.DoNotDisturb.ToString());
            Debug.WriteLine(ciscoIPEndpoint.DialableAddress);


            Debug.WriteLine("Current State: " + ciscoIPEndpoint.State.ToString());

            devicename = ciscoIPEndpoint.AddressName;

            // get just the name of the device and put it in the devicename variable
            devicename = ExtractDeviceName(devicename);
            // get the IP address of the device (phone.IP will get updated)
            getDeviceInfoViaAXL();

            phoneinfo.DeviceName = devicename;

            //   ciscoIPEndpoint.Open(TAPIMEDIATYPES.AUDIO);

        }

        private void btnAnswer_Click(object sender, EventArgs e)
        {
            btnAnswerRed = false;
            btnAnswer.BackColor = System.Drawing.SystemColors.Control;
            btnAnswer.Enabled = false;

            tmrFlash.Enabled = false;

            ansCall.Answer();

            // MAY BE DANGERUS.

            currCall = ansCall;
            wrkScreenshot.RunWorkerAsync();

        }

        private void chkAutoAnswer_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoAnswer.Checked)
                autoanswer = true;


            if (!chkAutoAnswer.Checked)
                autoanswer = false;

        }

        private void btnGetScreen_Click(object sender, EventArgs e)
        {

            wrkScreenshot.RunWorkerAsync();



        }

        private void RefreshScreenshot()
        {
            RestClient = new RestClientPhoneControl(phoneinfo.IP, restuser, restpass);

            RestClient.EndPoint = @"http://" + phoneinfo.IP + @"/CGI/Screenshot";

            screenshot = RestClient.getScreenshot();


        }

        // The TAPI endpointaddress isn't in a format that we can use with AXL to get the device, so we use this method to get it in the format we want.
        private string ExtractDeviceName(string tapiAddress)
        {
            char[] seperator = new char[] { '[', ']' };
            Debug.WriteLine(tapiAddress.Split(seperator)[1]);
            return tapiAddress.Split(seperator)[1];







        }
        // This method should return a struct once we know what we want in the struct so we can just access that :).
        private void getDeviceInfoViaAXL()
        {
            AXLConnectorPhoneControl AxlConnector = new AXLConnectorPhoneControl("172.21.1.55", "admin", "sub2dana", "Content-Type: text/xml;charset=UTF-8", "SOAPAction: \"selectCmDevice\"");


            phoneinfo.IP = AxlConnector.selectCmDeviceReturnIP(devicename);


        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            Console.WriteLine("In the sending area: " + phoneinfo.IP.ToString());

            RestClient = new RestClientPhoneControl(phoneinfo.IP, restuser, restpass);

            RestClient.EndPoint = @"https://" + phoneinfo.IP + @"/CGI/Execute";
            RestClient.Method = HttpVerb.POST;
            RestClient.PostData = @"XML=<CiscoIPPhoneText><Text>" + txtChat.Text + "</Text></CiscoIPPhoneText>";

            RestClient.MakeRequest();







        }

        private void dataPhoneInfo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {


        }

        private void SettingsChanged_EventHandler(object sender, settingschanged E)
        {




            Properties.Settings.Default.MainSettingClass = E.changedSettings;
            currentSettings = E.changedSettings;

            Properties.Settings.Default.Save();

            Debug.WriteLine("The password is now: " + currentSettings.currentAXLSettings.cucmpassword);
            Debug.WriteLine("The current IP is now: " + currentSettings.currentAXLSettings.ipaddress);




        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //ccierants.baseclasses.FrmSettings SettingsForm = new FrmSettings(ref currentSettings);
            //SettingsForm.AXLSettingsEventHandler += new EventHandler<settingschanged>(SettingsChanged_EventHandler);
            //Application.DoEvents();

            //SettingsForm.Show();

        }

        private void cmbInformation_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Selected value is: " + cmbInformation.Text);



            switch (cmbInformation.Text)
            {
                case "Network Configuration":
                    selectedphoneinfo = PhoneDeviceInfo.Network_Configuration;
                    break;
                case "Device Information":
                    selectedphoneinfo = PhoneDeviceInfo.Device_Information;
                    break;
                case "Ethernet Information":
                    selectedphoneinfo = PhoneDeviceInfo.Ethernet_Information;
                    break;
                case "Port Information":
                    selectedphoneinfo = PhoneDeviceInfo.Port_Information;
                    break;
                case "Device Log":
                    selectedphoneinfo = PhoneDeviceInfo.Device_Log;
                    break;
                case "Streaming Statistics":
                    selectedphoneinfo = PhoneDeviceInfo.Streaming_Statistics;
                    break;


            }
            getPhoneInformation(cmbInformation.Text);

        }
        /// <summary>
        /// This method pulls down the phone information based on what you selected in the drop down box.
        /// </summary>
        /// <param name="category"></param>
        private void getPhoneInformation(string category)
        {

            if (String.IsNullOrEmpty(phoneinfo.IP))
            {
                // THe 7965 at home
#if DEBUG
                phoneinfo.IP = "192.168.1.41";
#endif

                phoneinfo.IP = "10.42.4.231";


            }

            SimpleHTTPClient GetPhoneHTTP = new SimpleHTTPClient();
            switch (selectedphoneinfo)
            {
                case PhoneDeviceInfo.Network_Configuration:

                    GetPhoneHTTP.SendRequest("http://" + phoneinfo.IP + "/NetworkConfigurationX");
                    GetPhoneHTTP.GetResponse += GetPhoneHTTP_GetResponse;
                    break;
                case PhoneDeviceInfo.Device_Information:

                    GetPhoneHTTP.SendRequest("http://" + phoneinfo.IP + "/DeviceInformationX");
                    GetPhoneHTTP.GetResponse += GetPhoneHTTP_GetResponse;
                    break;

                case PhoneDeviceInfo.Ethernet_Information:
                    GetPhoneHTTP.SendRequest("http://" + phoneinfo.IP + "/EthernetInformationX");
                    GetPhoneHTTP.GetResponse += GetPhoneHTTP_GetResponse;
                    break;

                case PhoneDeviceInfo.Streaming_Statistics:
                    GetPhoneHTTP.SendRequest("http://" + phoneinfo.IP + "/StreamingStatisticsX");
                    GetPhoneHTTP.GetResponse += GetPhoneHTTP_GetResponse;
                    break;

                case PhoneDeviceInfo.Port_Information:
                    GetPhoneHTTP.SendRequest("http://" + phoneinfo.IP + "/PortInformationX");
                    GetPhoneHTTP.GetResponse += GetPhoneHTTP_GetResponse;

                    break;



                default:
                    Console.WriteLine("nothing selected");
                    break;

            }




        }

        void GetPhoneHTTP_GetResponse(object sender, DownloadStringCompletedEventArgs e)
        {

            /// now we have a response time to serialize it.
            // create the serializer object to take the info and serialize it, based on what info we requested:
            PhoneSerializer serialize = new PhoneSerializer();
            object PhoneInformation = new object();
            switch (selectedphoneinfo)
            {
                case PhoneDeviceInfo.Network_Configuration:
                    PhoneInformation = (PhoneNetworkConfiguration)serialize.SerializeXMLString(e.Result, typeof(PhoneNetworkConfiguration), PhoneDeviceInfo.Network_Configuration);

                    break;
                case PhoneDeviceInfo.Device_Information:
                    PhoneInformation = (PhoneDeviceInformation)serialize.SerializeXMLString(e.Result, typeof(PhoneDeviceInformation), PhoneDeviceInfo.Device_Information);

                    break;

                case PhoneDeviceInfo.Streaming_Statistics:
                    PhoneInformation = (PhoneStreamingStatistics)serialize.SerializeXMLString(e.Result, typeof(PhoneStreamingStatistics), PhoneDeviceInfo.Streaming_Statistics);
                    break;

                case PhoneDeviceInfo.Port_Information:
                    PhoneInformation = (PhonePortInformation)serialize.SerializeXMLString(e.Result, typeof(PhonePortInformation), PhoneDeviceInfo.Port_Information);
                    break;
                case PhoneDeviceInfo.Ethernet_Information:
                    PhoneInformation = (PhoneEthernetInformation)serialize.SerializeXMLString(e.Result, typeof(PhoneEthernetInformation), PhoneDeviceInfo.Ethernet_Information);
                    break;

                case PhoneDeviceInfo.Device_Log:
                    PhoneInformation = (PhoneDeviceLog)serialize.SerializeXMLString(e.Result, typeof(PhoneDeviceLog), PhoneDeviceInfo.Device_Log);
                    break;
                default:
                    throw new Exception("No phone device information selected from the combo box?");

            }





            System.Reflection.FieldInfo[] fieldarray = PhoneInformation.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);

            dataPhoneInfo.Rows.Clear();

            foreach (System.Reflection.FieldInfo Field in fieldarray)
            {

                Console.WriteLine("Field Name: " + Field.Name);
                Console.WriteLine("Value: " + Field.GetValue(PhoneInformation));
                if (isNull(Field.GetValue(PhoneInformation)))
                {
                    Console.WriteLine("Field: " + Field.Name + " Has no value, not added to grid");

                    // if it's null don't bother returning a value.


                }
                else
                {


                    dataPhoneInfo.Rows.Add(Field.Name, Field.GetValue(PhoneInformation));
                }

            }

        }
        public static bool isNull(object aRef)
        {
            return aRef != null && aRef.Equals(null);
        }

        private void dataPhoneInfo_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void chkContinousRefresh_CheckedChanged(object sender, EventArgs e)
        {
            if (chkContinousRefresh.Checked)
            {

                tmrScreenshot.Enabled = true;
            }
        }

        private void tmrScreenshot_Tick(object sender, EventArgs e)
        {
            wrkScreenshot.RunWorkerAsync();


        }

        private void trkBar_Scroll(object sender, EventArgs e)
        {
            int timer = Int32.Parse(trkBar.Value.ToString() + "000");
            tmrScreenshot.Interval = timer;


        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            wrkScreenshot.RunWorkerAsync();
        }

        private void btnKeyUp_Click(object sender, EventArgs e)
        {
            Button curbutton = (System.Windows.Forms.Button)sender;
            string btnPressed = curbutton.Name.Replace("btn", "");
            Console.WriteLine("button: " + btnPressed + " was pressed");
            pushButton(btnPressed);
            wrkScreenshot.RunWorkerAsync();



        }


        private void pushButton(string key)
        {

            //RestClient = new RestClientPhoneControl(phoneinfo.IP, restuser, restpass);

            //RestClient.EndPoint = @"https://" + phoneinfo.IP + @"/CGI/Execute";
            //RestClient.Method = HttpVerb.POST;
            //RestClient.PostData = @"XML=<CiscoIPPhoneText><Text>" + txtChat.Text + "</Text></CiscoIPPhoneText>";

            //RestClient.MakeRequest();



            RestClient = new RestClientPhoneControl(phoneinfo.IP, restuser, restpass);

            RestClient.EndPoint = @"http://" + phoneinfo.IP + @"/CGI/Execute";
            RestClient.Method = HttpVerb.POST;

            RestClient.PostData = "XML=<CiscoIPPhoneExecute><ExecuteItem URL=\"Key:" + key + "\"/></CiscoIPPhoneExecute>";
            //  RestClient.PutData = "XML=<CiscoIPPhoneExecute><ExecuteItem URL=\"Key:" + key + "\"/></CiscoIPPhoneExecute>";

            Console.WriteLine("Push Button sent the following XML:" + "XML=<CiscoIPPhoneExecute><ExecuteItem URL=\"Key:" + key + "\"/></CiscoIPPhoneExecute>");


            RestClient.MakeRequest();



        }
        private void btnSettings_Click(object sender, EventArgs e)
        {

        }

        private void btnSelect_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {




            phoneinfo.IP = "10.42.4.231";

        }

        private void btnSoft1_Click(object sender, EventArgs e)
        {
            Button curbutton = (System.Windows.Forms.Button)sender;
            string btnPressed = curbutton.Name.Replace("btn", "");
            Console.WriteLine("button: " + btnPressed + " was pressed");
            pushButton(btnPressed);
            wrkScreenshot.RunWorkerAsync();

        }

        private void wrkScreenshot_DoWork(object sender, DoWorkEventArgs e)
        {
            RefreshScreenshot();
        }


        private void updatePictureScreenshot()
        {
            pctCurScreen.Width = screenshot.Width;
            pctCurScreen.Height = screenshot.Height;
            pctCurScreen.Image = screenshot;

        }

        private void wrkScreenshot_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Invoke(new updateScreenshot(this.updatePictureScreenshot));

        }

        private void btnStop_Click(object sender, EventArgs e)
        {


            var waveFormat = WaveFormat.CreateMuLawFormat(8000, 1);
            RTPReceive.StopClient();

            // memory stream contains the data in raw format (PCM Format)
            MemoryStream memStream = RTPReceive.GetMemoryStream();
            memStream.Position = 0;
            var rawSource = new RawSourceWaveStream(memStream, waveFormat);








            using (WaveStream convertedStream = WaveFormatConversionStream.CreatePcmStream(rawSource))
            {




                WaveFileWriter.CreateWaveFile(@"f:\temp crap\test3.wav", convertedStream);

            }















        }

        private void btnResend_Click(object sender, EventArgs e)
        {
            RTPReceive.SendReplay();

        }

    }
    public class CustomPlayer
    {
        private WaveFormat waveOutFormat = new WaveFormat(8000, 1);


        private BufferedWaveProvider bufferedWaveProvider;
        private WaveOut waveout;
        public CustomPlayer()
        {

            bufferedWaveProvider = new BufferedWaveProvider(waveOutFormat);

            waveout = new WaveOut();
            waveout.Init(bufferedWaveProvider);

        }

        public void AddSamples(byte[] array)
        {


            var waveFormat = WaveFormat.CreateMuLawFormat(8000, 1);


            // memory stream contains the data in raw format (PCM Format)
            MemoryStream memStream = new MemoryStream(array);


            var rawSource = new RawSourceWaveStream(memStream, waveFormat);








            WaveStream convertedStream = WaveFormatConversionStream.CreatePcmStream(rawSource);

            byte[] buffer = new byte[convertedStream.Length];

            convertedStream.Read(buffer, 0, (int)convertedStream.Length);
            bufferedWaveProvider.AddSamples(buffer, 0, buffer.Length);




            waveout.Play();
        }
    }



    public static class extensionMethods
    {

        public static byte[] ReadAllBytes(this BinaryReader reader)
        {
            const int bufferSize = 4096;
            using (var ms = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                    ms.Write(buffer, 0, count);
                return ms.ToArray();
            }

        }
    }


}




