using ccierants.baseclasses;

using LumiSoft.Net;
using LumiSoft.Net.RTP;
using LumiSoft.Net.SDP;
using LumiSoft.Net.SIP.Stack;
using NAudio.Wave;
using NAudio;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace CCIERantsPhoneControl
{
    public partial class frmMain : Form
    {

        private string runningHWID;
        
        private RoundButton btnKeyPad1;
        private RoundButton btnKeyPad2;
        private RoundButton btnKeyPad3;
        private RoundButton btnKeyPad4;
        private RoundButton btnKeyPad5;
        private RoundButton btnKeyPad6;
        private RoundButton btnKeyPad7;
        private RoundButton btnKeyPad8;
        private RoundButton btnKeyPad9;
        private RoundButton btnKeyPad0;
        private RoundButton btnKeyPadStar;
        private RoundButton btnKeyPadPound;
        private FileStream writerOutput;
        private StreamWriter writer;
        private BindingList<CiscoPhoneObject> phoneConList = new BindingList<CiscoPhoneObject>();
        private ScreenshotHolderClass ScreenshotHolder = new ScreenshotHolderClass();
        private CiscoPhoneObject tempPhoneCon;
        private bool globalUserPass = false;
        private bool userHasBeenWarned = false;
        // this is the selected index of the phoneconnectivity

        // this is a global variable if 

        private RTPClientAsync RTPReceiveStream1;
        private RTPClientAsync RTPReceiveStream2;
        private RTPClientAsync RTPReceiveXMLMethod;
        // this is how many entries there are in the listbox.

        private SimpleHTTPClient simpleHTTP = new SimpleHTTPClient();
        private SimpleHTTPClient screenHTTP = new SimpleHTTPClient();

        private RestClientPhoneControl restClientScreenshot;


        private WebServer WS;
        private WebServer WS2;

        private bool changingDeviceInfo = false; // used in cases where we are changing the phone info we want to display.


        private int SamplesPerSecond = 8000;
        private short BitsPerSample = 16;
        private short Channels = 2;
        private Int32 BufferCount = 8;

        private int RTPPortCountStream1 = 24670;
        private int RTPPortCountStream2 = 24671;
        private int RTPPortCountXML = 20490;
        private int RTPBasePortCountXML = 0;
        private int RTPResponsePortCount = 0;
        private int RTPBasePortCount = 0;
        private CustomPlayer wavPlayer; //= new CustomPlayer();
        private CustomPlayer wavPlayer2; //= new CustomPlayer();
        private CustomPlayer wavPlayer3 = new CustomPlayer();

        private int wavPlayerSsrcID = 0;
        private int wavPlayer2SsrcID = 0;
        private int ConnectedIndex = -1;
        private SIP_Stack sipStack;
        private SIP_Stack sipStackUDP;

        private string sipListenerIP;
        private bool sendingBulk = false;
        private bool sendingMsg = false;




        delegate void updateBtnTest();
        delegate void updateBtnDisconnect();
        delegate void updateDataPhoneInfoLog(string[] ParamsPassed);
        delegate void updateDataPhoneInfoFull(string[] ParamsPassed, string[] ValuesPassed);
        delegate void updateScreenshot();
        delegate void updateProgressBar();
        delegate void closeMainFrm();
        delegate void DisconnectInvoke();
        delegate void ConnectInvoke();
        delegate void GenericInvoker();


        private CancellationTokenSource cancellationTokenSourceSendMsg = new CancellationTokenSource(); //Declare a cancellation token source

        private CancellationToken cancellationTokenSendMsg; //Declare a cancellation token object,

        private CancellationTokenSource cancellationTokenSourceBulkEdit = new CancellationTokenSource(); //Declare a cancellation token source

        private CancellationToken cancellationTokenBulkEdit; //Declare a cancellation token object,


        private CancellationTokenSource cancellationTokenSourceScreenGrab = new CancellationTokenSource(); //Declare a cancellation token source

        private CancellationToken cancellationTokenScreenGrab; //Declare a cancellation token object,

        private CancellationTokenSource cancellationTokenSourceUpdateInfo = new CancellationTokenSource();

        private CancellationToken cancellationTokenUpdateInfo;
        private Bitmap newBackground;
        private Bitmap newBackGroundPreview;

        public frmMain()
        {
            InitializeComponent();
        }

        private void GetHWFingerPrint()
        {

            runningHWID = FingerPrint.BaseValue();
            lblBaseValue.Text += " " + runningHWID;

        }

        private void isLicensed()
        {
            Console.WriteLine("Unlocked License Mode only");
            btnSendMsg.Enabled = true;
            btnStartListening.Enabled = true;
            btnStartListening.Visible = true;
            btnSendMsg.Visible = true;
            lblUpgrade.Visible = false;
            lblUpgrade2.Visible = false;
            lblFeature.Visible = false;
            lblFeature2.Visible = false;

            // Placeholder to do stuff if they are indeed definitely licensed.
        }
        private bool isTemplicenseDateValid()
        {
            return true;
        }
        private bool checkTempHWIDfromLicense()
        {


            return true;
        }
        private bool checkPermHWIDfromLicense()
        {
            Console.WriteLine("HW ID in perm license Setting: " + curLicenseData.hwid1);
            if (curLicenseData.hwid1 == runningHWID)
            {
                // Correct HWID for license file.
                Console.WriteLine("Correct HW ID in license file.");
                isLicensed();
                return true;

            }
            else
            {
                // No mathcing hardware ID I am afraid.
                Console.WriteLine("HW ID Doesn't match License File.");
                enableBasicLicense();
            }

            return false;

        }
        private bool checkAndLoadLicense()
        {
            try
            {
                if (!isNull(Properties.Settings.Default.permLicense))
                {
                    if (String.IsNullOrEmpty(Properties.Settings.Default.permLicense.hwid1))
                    {
                        Console.WriteLine("No value for perm license hwid, license not installed.");
                        return false;
                    }
                    Console.WriteLine("License loaded from settings...");
                    curLicenseData = Properties.Settings.Default.permLicense;

                    if (LicenseLibrary.ValidatePermLicense(curLicenseData))
                    {
                        Console.WriteLine("Permanent License Validated. ");

                        checkPermHWIDfromLicense();

                        /// TIME TO CHECK THE HARWDARE ID
                        /// 
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Permanent License did not show as valid.");
                        return false;
                    }





                }
                else
                {
                    Console.WriteLine("No Perm License in settings");
                    return false;
                }


            }
            catch (Exception E)
            {
                return false;
            }
        }

        private bool checkAndLoadTempLicense()
        {
            try
            {

                if (!isNull(Properties.Settings.Default.tempLicense))
                {
                    curTempLicense = Properties.Settings.Default.tempLicense;
                    checkTempHWIDfromLicense();
                    if (isTemplicenseDateValid())
                    {
                        isLicensed();
                    }


                    Console.WriteLine("Temp License Loaded from settings...");
                    return true;
                }
                Console.WriteLine("No Temp License in settings");
                return false;
            }
            catch (Exception E)
            {

                return false;
            }

        }

        public void LicenseCheckProcess()
        {
            //LicenseLibrary.PrivateKeyPubKeyGeneratorAllBase64();
            // First thing we do is get the hardware ID and update the label
            GetHWFingerPrint();


            //LicenseData newLicense = new LicenseData();
            //newLicense.hwid1 = runningHWID;
            //newLicense.Signature = LicenseLibrary.SignPermLicense(newLicense);
            //Properties.Settings.Default.permLicense = newLicense;


            //Properties.Settings.Default.Save();




            if (checkAndLoadLicense())
            {
                // License is sitting in memory now.


                return;
            }


            if (checkAndLoadTempLicense())
            {

                return;
            }

            enableBasicLicense();

        }

        private void enableBasicLicense()
        {
            Console.WriteLine("Basic License Mode only");
            btnSendMsg.Enabled = false;
            btnStartListening.Enabled = false;


            btnCancelSend.Enabled = false;
            btnStop.Enabled = false;
            btnSendMsg.Enabled = false;
            lblUpgrade.Visible = true;
            lblUpgrade2.Visible = true;
            lblFeature.Visible = true;
            lblFeature2.Visible = true;
            btnStartListening.Visible = true;


        }
        private void frmMain_Load(object sender, EventArgs e)
        {


            try
            {

                writerOutput = new System.IO.FileStream("LoggingFile-Peter.txt", System.IO.FileMode.Append, System.IO.FileAccess.Write);
                writer = new System.IO.StreamWriter(writerOutput);
                writer.AutoFlush = true;
                Console.SetOut(writer);


            }
            catch (Exception E)
            {

            }




            btnKeyPad1 = new RoundButton();
            btnKeyPad1.Location = new System.Drawing.Point(8, 295);
            btnKeyPad1.Size = new System.Drawing.Size(80, 80);
            btnKeyPad1.Font = new System.Drawing.Font("VAGRounded BT", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnKeyPad1.Visible = true;
            btnKeyPad1.Text = "1";
            btnKeyPad1.Click += new System.EventHandler(this.btnPhoneInteraction_Click);
            btnKeyPad1.Refresh();
            btnKeyPad1.Name = "btnKeyPad1";

            btnKeyPad2 = new RoundButton();
            btnKeyPad2.Location = new System.Drawing.Point(btnKeyPad1.Location.X + btnKeyPad1.Width, 295);
            btnKeyPad2.Size = new System.Drawing.Size(80, 80);
            btnKeyPad2.Font = new System.Drawing.Font("VAGRounded BT", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnKeyPad2.Text = "2";
            btnKeyPad2.Visible = true;
            btnKeyPad2.Click += new System.EventHandler(this.btnPhoneInteraction_Click);
            btnKeyPad2.Refresh();
            btnKeyPad2.Name = "btnKeyPad2";

            btnKeyPad3 = new RoundButton();
            btnKeyPad3.Location = new System.Drawing.Point(btnKeyPad2.Location.X + btnKeyPad1.Width, 295);
            btnKeyPad3.Size = new System.Drawing.Size(80, 80);
            btnKeyPad3.Font = new System.Drawing.Font("VAGRounded BT", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnKeyPad3.Text = "3";
            btnKeyPad3.Visible = true;
            btnKeyPad3.Click += new System.EventHandler(this.btnPhoneInteraction_Click);
            btnKeyPad3.Refresh();
            btnKeyPad3.Name = "btnKeyPad3";

            btnKeyPad4 = new RoundButton();
            btnKeyPad4.Location = new System.Drawing.Point(8, btnKeyPad1.Location.Y + btnKeyPad1.Height);
            btnKeyPad4.Size = new System.Drawing.Size(80, 80);
            btnKeyPad4.Font = new System.Drawing.Font("VAGRounded BT", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnKeyPad4.Text = "4";
            btnKeyPad4.Visible = true;
            btnKeyPad4.Click += new System.EventHandler(this.btnPhoneInteraction_Click);
            btnKeyPad4.Refresh();
            btnKeyPad4.Name = "btnKeyPad4";

            btnKeyPad5 = new RoundButton();
            btnKeyPad5.Location = new System.Drawing.Point(btnKeyPad4.Location.X + btnKeyPad1.Width, btnKeyPad4.Location.Y);
            btnKeyPad5.Size = new System.Drawing.Size(80, 80);
            btnKeyPad5.Visible = true;
            btnKeyPad5.Font = new System.Drawing.Font("VAGRounded BT", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnKeyPad5.Text = "5";
            btnKeyPad5.Click += new System.EventHandler(this.btnPhoneInteraction_Click);
            btnKeyPad5.Refresh();
            btnKeyPad5.Name = "btnKeyPad5";

            btnKeyPad6 = new RoundButton();
            btnKeyPad6.Location = new System.Drawing.Point(btnKeyPad5.Location.X + btnKeyPad1.Width, btnKeyPad4.Location.Y);
            btnKeyPad6.Size = new System.Drawing.Size(80, 80);
            btnKeyPad6.Font = new System.Drawing.Font("VAGRounded BT", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnKeyPad6.Text = "6";
            btnKeyPad6.Visible = true;
            btnKeyPad6.Click += new System.EventHandler(this.btnPhoneInteraction_Click);
            btnKeyPad6.Refresh();
            btnKeyPad6.Name = "btnKeyPad6";


            btnKeyPad7 = new RoundButton();
            btnKeyPad7.Location = new System.Drawing.Point(8, btnKeyPad4.Location.Y + btnKeyPad1.Height);
            btnKeyPad7.Size = new System.Drawing.Size(80, 80);
            btnKeyPad7.Font = new System.Drawing.Font("VAGRounded BT", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnKeyPad7.Text = "7";
            btnKeyPad7.Visible = true;
            btnKeyPad7.Click += new System.EventHandler(this.btnPhoneInteraction_Click);
            btnKeyPad7.Refresh();
            btnKeyPad7.Name = "btnKeyPad7";

            btnKeyPad8 = new RoundButton();
            btnKeyPad8.Location = new System.Drawing.Point(btnKeyPad7.Location.X + btnKeyPad1.Width, btnKeyPad7.Location.Y);
            btnKeyPad8.Size = new System.Drawing.Size(80, 80);
            btnKeyPad8.Font = new System.Drawing.Font("VAGRounded BT", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnKeyPad8.Text = "8";
            btnKeyPad8.Visible = true;
            btnKeyPad8.Click += new System.EventHandler(this.btnPhoneInteraction_Click);
            btnKeyPad8.Refresh();
            btnKeyPad8.Name = "btnKeyPad8";

            btnKeyPad9 = new RoundButton();
            btnKeyPad9.Location = new System.Drawing.Point(btnKeyPad8.Location.X + btnKeyPad1.Width, btnKeyPad8.Location.Y);
            btnKeyPad9.Size = new System.Drawing.Size(80, 80);
            btnKeyPad9.Font = new System.Drawing.Font("VAGRounded BT", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnKeyPad9.Text = "9";
            btnKeyPad9.Visible = true;
            btnKeyPad9.Click += new System.EventHandler(this.btnPhoneInteraction_Click);
            btnKeyPad9.Refresh();
            btnKeyPad9.Name = "btnKeyPad9";

            btnKeyPadStar = new RoundButton();
            btnKeyPadStar.Location = new System.Drawing.Point(8, btnKeyPad7.Location.Y + btnKeyPad1.Height);
            btnKeyPadStar.Size = new System.Drawing.Size(80, 80);
            btnKeyPadStar.Font = new System.Drawing.Font("VAGRounded BT", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnKeyPadStar.Text = "*";
            btnKeyPadStar.Visible = true;
            btnKeyPadStar.Click += new System.EventHandler(this.btnPhoneInteraction_Click);
            btnKeyPadStar.Refresh();
            btnKeyPadStar.Name = "btnKeyPadStar";


            btnKeyPad0 = new RoundButton();
            btnKeyPad0.Location = new System.Drawing.Point(btnKeyPadStar.Location.X + btnKeyPad1.Width, btnKeyPadStar.Location.Y);
            btnKeyPad0.Size = new System.Drawing.Size(80, 80);
            btnKeyPad0.Font = new System.Drawing.Font("VAGRounded BT", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnKeyPad0.Text = "0";
            btnKeyPad0.Visible = true;
            btnKeyPad0.Click += new System.EventHandler(this.btnPhoneInteraction_Click);
            btnKeyPad0.Refresh();
            btnKeyPad0.Name = "btnKeyPad0";


            btnKeyPadPound = new RoundButton();
            btnKeyPadPound.Location = new System.Drawing.Point(btnKeyPad0.Location.X + btnKeyPad1.Width, btnKeyPad0.Location.Y);
            btnKeyPadPound.Size = new System.Drawing.Size(80, 80);
            btnKeyPadPound.Font = new System.Drawing.Font("VAGRounded BT", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnKeyPadPound.Text = "#";
            btnKeyPadPound.Visible = true;
            btnKeyPadPound.Click += new System.EventHandler(this.btnPhoneInteraction_Click);
            btnKeyPadPound.Refresh();
            btnKeyPadPound.Name = "btnKeyPadPound";


            this.tabControl1.TabPages[0].Controls.Add(btnKeyPad1);
            this.tabControl1.TabPages[0].Controls.Add(btnKeyPad2);
            this.tabControl1.TabPages[0].Controls.Add(btnKeyPad3);
            this.tabControl1.TabPages[0].Controls.Add(btnKeyPad4);
            this.tabControl1.TabPages[0].Controls.Add(btnKeyPad5);
            this.tabControl1.TabPages[0].Controls.Add(btnKeyPad6);
            this.tabControl1.TabPages[0].Controls.Add(btnKeyPad7);
            this.tabControl1.TabPages[0].Controls.Add(btnKeyPad8);
            this.tabControl1.TabPages[0].Controls.Add(btnKeyPad9);
            this.tabControl1.TabPages[0].Controls.Add(btnKeyPadStar);
            this.tabControl1.TabPages[0].Controls.Add(btnKeyPad0);
            this.tabControl1.TabPages[0].Controls.Add(btnKeyPadPound);





            this.Refresh();


            getNetworkAdapters();
            loadSettings();

            //LicenseCheckProcess();
            FillInToolTips();
            bindGUIelementsToDataSource();

            cmbNum.SelectedIndex = 0;
#if RELEASE
           btnResetSettings.Visible = false;
            btnResetSettings.Enabled = false;
#endif
#if DEBUG
            chkPrintHeader.Visible = true;
#endif
            lstViewCommands.Alignment = ListViewAlignment.Left;
            lstViewCommands.AutoArrange = false;



        }
        private void bindGUIelementsToDataSource()
        {
            lstDataBindPhones.DataSource = phoneConList;
            phoneConList.ListChanged += phoneConList_ListChanged;
            cmbPhone.DisplayMember = "PhoneDisplayName";

            cmbPhone.DataSource = phoneConList;
            pctCurScreen.DataBindings.Add("Image", this.ScreenshotHolder, "CurrentScreenshot", true);



            lstDataBindPhones.DisplayMember = "PhoneDisplayName";





        }

        void phoneConList_ListChanged(object sender, ListChangedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                lstDataBindPhones.DataSource = null;
                lstDataBindPhones.DataSource = phoneConList;

                cmbPhone.DataSource = null;
                cmbPhone.DataSource = phoneConList;



            });
        }
        private void getNetworkAdapters()
        {

            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {

                IPInterfaceProperties properties = adapter.GetIPProperties();

                foreach (UnicastIPAddressInformation address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        cmbListenIP.Items.Add(address.Address.ToString());
                        cmbHTTP.Items.Add(address.Address.ToString());
                    }

                }

            }

        }

        private void editPhoneDetails(int index)
        {
            try
            {
                if (index == -1)
                {
                    return;

                }
                else
                {

                    updateGUIFromPhoneConEntry(index);
                    btnSave.Enabled = true;
                }
            }
            catch (IndexOutOfRangeException e)
            {

            }


        }

        private void updateGUIFromPhoneConEntry(int index)
        {

            if (index.Equals(-1))
            {
                return;
                // don't do this stuff if the index is equal to negative 1.
            }

            txtPassword.Text = phoneConList[index].password;

            txtUsername.Text = phoneConList[index].username;
            txtPhoneIP.Text = phoneConList[index].ip;
            chkUseHTTPS.Checked = phoneConList[index].useHTTPS;
            trkTimer.Value = Int32.Parse(phoneConList[index].screenshotInterval.ToString().Replace("000", ""));
            chkDisableScreenshot.Checked = phoneConList[index].screenshotDisabled;
            cmbNum.Text = phoneConList[index].NumSoftKeys.ToString();

        }


        private void testConnectivity()
        {


            btnTest.Enabled = false;
            btnTest.Text = "Testing...";
            try
            {
                CiscoPhoneObject TestPhone = this.createPhoneObject();

                TestPhone.PhoneHTTPReqResponse += OnPhoneHTTPReqResponseTestConnectivity;

                TestPhone.updatePhoneInformation("DeviceLog?0");




            }

            catch (Exception E)
            {

                btnTest.Enabled = true;
                btnTest.Text = "Test Connection";
                MessageBox.Show("Could not connect to phone, even Unauthenticated, Please double check IP Address.", "Unauthenticated Test Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);


                System.Diagnostics.Debug.WriteLine("Error Occured: " + E.Message);

            }



        }

        private void OnPhoneHTTPReqResponseTestConnectivity(object sender)
        {
            MessageBox.Show("Connection Succesful.", "Connected", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Invoke(new updateBtnTest(updateTestButton));


        }
        private void updateTestButton()
        {
            btnTest.Enabled = true;
            btnTest.Text = "Test Connection";
        }


        public static bool isNull(object aRef)
        {
            return aRef != null && aRef.Equals(null);
        }
        private bool checkIPRemoveHTTP(string ipstring)
        {


            int elementcount = ipstring.Split('.').Length;

            if (ipstring.Contains(@"http://") || ipstring.Contains(@"HTTP://") || ipstring.Contains(@"Http://"))
            {
                ipstring = Regex.Replace(ipstring, @"http://", "", RegexOptions.IgnoreCase);
                txtPhoneIP.Text = ipstring;
                chkUseHTTPS.Checked = false;



            }
            if (ipstring.Contains("https://") || ipstring.Contains(@"HTTPS://") || ipstring.Contains(@"Https://"))
            {

                ipstring = Regex.Replace(ipstring, @"https://", "", RegexOptions.IgnoreCase);
                txtPhoneIP.Text = ipstring;
                chkUseHTTPS.Checked = true;
            }


            Console.WriteLine("count number of dots: " + elementcount.ToString());

            if (elementcount != 4)
            {

                MessageBox.Show("IP Address does not contain 4 .'s are you sure it's formatted correctly?: " + ipstring);
                return false;
            }
            else
            {

            }
            char[] dot = new char[] { '.' };
            string[] Octects = ipstring.Split(dot, 4);

            int count = 0;
            foreach (string octet in Octects)
            {
                try
                {
                    int num = Int32.Parse(octet);

                    if (num <= 255)
                    {
                        Console.WriteLine("Octet: " + num.ToString());

                    }
                    else
                    {
                        // One of the octets was wrong
                        Console.WriteLine("Contained a number higher than 255");
                        MessageBox.Show("IP Address Appears to be invalid: " + ipstring);
                        return false;



                    }


                    if (count == 3)
                    {  // last octet

                        if (num == 255 || num == 0)
                        {
                            // Last octet is 255 or 0
                            MessageBox.Show("last octet cannot be 0 or 255");
                            return false;
                        }

                    }

                    count++;

                }
                catch (Exception e)
                {
                    MessageBox.Show("Your IP address contains letters: " + ipstring);
                    return false;

                }
            }
            return true;









        }
        private void btnTest_Click(object sender, EventArgs e)
        {
            if (checkIPRemoveHTTP(txtPhoneIP.Text))
            {

                testConnectivity();
            }


        }


        private void updatePhoneInfoGUI(string[] Parameters, string[] Values)
        {
            dataPhoneInfo.Invoke(new Action(() => dataPhoneInfo.Rows.Clear()));



            for (int i = 0; i < Parameters.Length; i++)
            {
                dataPhoneInfo.Invoke(new Action(() => dataPhoneInfo.Rows.Add(Parameters[i], Values[i])));



            }

        }

        private void updatePhoneInfoGUI(string[] StatusEntries)
        {



            dataPhoneInfo.Invoke(new Action(() => dataPhoneInfo.Rows.Clear()));

            foreach (string statusentry in StatusEntries)
            {

                if (isNull(statusentry))
                {


                    // if it's null don't bother returning a value.


                }
                else
                {


                    dataPhoneInfo.Invoke(new Action(() => dataPhoneInfo.Rows.Add("Log Message: ", statusentry)));
                }

            }
        }



        private CiscoPhoneObject createPhoneObject()
        {
            CiscoPhoneObject tempPhoneCon = new CiscoPhoneObject();
            tempPhoneCon.ip = txtPhoneIP.Text;
            tempPhoneCon.username = txtUsername.Text;
            tempPhoneCon.password = txtPassword.Text;
            tempPhoneCon.screenshotInterval = getTrkValue();
            tempPhoneCon.useHTTPS = chkUseHTTPS.Checked;
            tempPhoneCon.screenshotDisabled = chkDisableScreenshot.Checked;

            return tempPhoneCon;
        }




        private void GetPhoneHTTP_InitialConnectStep2(object sender, DownloadStringCompletedEventArgs e)
        {

            try
            {

                /// now we have a response time to serialize it.
                // create the serializer object to take the info and serialize it, based on what info we requested:
                PhoneSerializer serialize = new PhoneSerializer();
                object PhoneInformation = new object();
                PhoneInformation = (PhoneNetworkConfiguration)serialize.SerializeXMLString(e.Result, typeof(PhoneNetworkConfiguration), PhoneDeviceInfo.Network_Configuration);


                CiscoPhoneObject tempPhoneCon = new CiscoPhoneObject();


                tempPhoneCon.phoneNetworkConfiguration = (PhoneNetworkConfiguration)PhoneInformation;


                //                saveSettings();


            }
            catch (Exception E)
            {
                Console.WriteLine("In InitialConnect2 had something weird happen: " + E.Message);


            }





            simpleHTTP.GetResponse -= GetPhoneHTTP_InitialConnectStep2;


        }


        private void AddPhone()
        {

            tempPhoneCon = createPhoneObject();
            tempPhoneCon.PhoneHTTPReqResponse += frmMain_PhoneFirstConnection;
            tempPhoneCon.NumSoftKeys = Int32.Parse(cmbNum.Text);
            phoneConList.Add(tempPhoneCon.updatePhoneInformationReturnObject());
            lstDataBindPhones.SelectedIndexChanged -= lstDataBindPhones_SelectedIndexChanged;

            if (lstDataBindPhones.SelectedIndex == -1)
            {
            }
            else
            {

                lstDataBindPhones.SetSelected(lstDataBindPhones.SelectedIndex, false);

            }
            clearInputs();



        }
        private void rebindlstDataBindPhones()
        {
            if (lstDataBindPhones.SelectedIndex == -1)
            {
                lstDataBindPhones.SelectedIndexChanged += lstDataBindPhones_SelectedIndexChanged;

                return;
            }
            lstDataBindPhones.SetSelected(lstDataBindPhones.SelectedIndex, false);
            lstDataBindPhones.SelectedIndexChanged += lstDataBindPhones_SelectedIndexChanged;

        }
        void frmMain_PhoneFirstConnection(object sender)
        {
            try
            {
                phoneConList[phoneConList.Count - 1].updatePhoneDisplayName();
                this.Invoke(new GenericInvoker(rebindlstDataBindPhones));

            }
            catch (Exception E)
            {

            }
        }

        private void clearInputs()
        {
            if (chkGlobalUserPass.Checked)
            {


                txtPhoneIP.Text = "";
            }
            else
            {

                txtUsername.Text = "";
                txtPassword.Text = "";
                txtPhoneIP.Text = "";
            }
        }
        private void StartScreenshotMonitor()
        {
            // some phone objects don't have ScreenshotMonitor turned on!

            if (phoneConList[ConnectedIndex].screenshotDisabled)
            {

            }
            else
            {
                cancellationTokenSourceScreenGrab = new CancellationTokenSource();

                cancellationTokenScreenGrab = cancellationTokenSourceScreenGrab.Token;
                getScreenshot();
            };




        }


        private void btnAdd_Click(object sender, EventArgs e)
        {


            if (checkIPRemoveHTTP(txtPhoneIP.Text))
            {

                AddPhone();


            }





        }
        // Global method to use HTTPS instead of HTTP
        private string isHTTPSConnection()
        {
            if (phoneConList[ConnectedIndex].useHTTPS)
            {

                return @"s://";
            }

            return @"://";

        }
        private string isHTTPSConnection(CiscoPhoneObject phoneStruct)
        {
            if (phoneStruct.useHTTPS)
            {

                return @"s://";
            }

            return @"://";

        }


        private void saveSettings()
        {


            Console.WriteLine("In save settings");

            Properties.Settings.Default.advancedModeLast = chkAdvancedMode.Checked;
            Properties.Settings.Default.basicModeLast = chkBasicListenMethod.Checked;
            Properties.Settings.Default.toolTips = chkEnableToolTips.Checked;
            if (!String.IsNullOrEmpty(cmbListenIP.Text))
                Properties.Settings.Default.lastIP = cmbListenIP.Text;

            Properties.Settings.Default.Save();
            savePhones();


        }

        private void savePhones()
        {
            try
            {
                if (phoneConList.Count != -1)
                {
                    Console.WriteLine("Entry found! Writing phoneConSettings.xml");
                    Properties.Settings.Default.firstUse = false;
                    //   if (ScreenshotIsInitSetup)
                    //   {
                    ////       pctCurScreen.Image = Properties.Resources.PhoneScreenshot;
                    //       ScreenshotIsInitSetup = false;

                    //   }


                    XmlSerializer serializeSettings = new XmlSerializer(typeof(BindingList<CiscoPhoneObject>));

                    System.IO.StreamWriter writePhoneConFile = new System.IO.StreamWriter("phoneConSettings.xml");
                    serializeSettings.Serialize(writePhoneConFile, phoneConList);
                    writePhoneConFile.Close();


                }
            }
            catch (System.IO.IOException E)
            {
                MessageBox.Show("There was an error writing to the phoneConSettings.xml file, please check directory permissions and try again.");

            }
        }

        private void loadSettings()
        {

            Console.WriteLine("In Load Settings");

            chkAdvancedMode.Checked = Properties.Settings.Default.advancedModeLast;
            chkBasicListenMethod.Checked = Properties.Settings.Default.basicModeLast;
            chkEnableToolTips.Checked = Properties.Settings.Default.toolTips;
            ToolTipEnableUpdate();
            if (!String.IsNullOrEmpty(Properties.Settings.Default.lastIP))
            {

                if (cmbListenIP.Items.Contains(Properties.Settings.Default.lastIP))
                {
                    cmbListenIP.SelectedIndex = cmbListenIP.FindStringExact(Properties.Settings.Default.lastIP);

                }
            }

            if (!Properties.Settings.Default.firstUse)
            {
                loadPhones();

            }
            else
            {



            }

        }

        private void loadPhones()
        {

            try
            {


                XmlSerializer serializeSettings = new XmlSerializer(typeof(BindingList<CiscoPhoneObject>));
                System.IO.FileStream openPhoneConFile = new System.IO.FileStream("phoneConSettings.xml", System.IO.FileMode.Open);



                phoneConList = (BindingList<CiscoPhoneObject>)serializeSettings.Deserialize(openPhoneConFile);

                openPhoneConFile.Close();


            }
            catch (Exception E)
            {

                Console.WriteLine("There was an error loading the phoneConSettings.XML File, set firstuse back to true so we can reset the settings");
                Properties.Settings.Default.firstUse = true;
            }



        }

        private void cmbInformation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ConnectedIndex == -1)
            {
                MessageBox.Show("Please Connect to a phone first.", "Connect to a Phone", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                System.Diagnostics.Debug.WriteLine("Must be connected to a phone first.");
                return;
            }
            if (phoneConList[ConnectedIndex].connectionState == PhoneConnectionState.Connected)
            {
                phoneConList[ConnectedIndex].PhoneHTTPReqResponse += OnPhoneHTTPReqResponseUpdateInfo;
                cmbInformationTextString = cmbInformation.Text;
                switch (cmbInformation.Text)
                {
                    case "Network Configuration":
                        phoneConList[ConnectedIndex].updatePhoneInformation("NetworkConfigurationX");
                        break;
                    case "Device Information":
                        phoneConList[ConnectedIndex].updatePhoneInformation("DeviceInformationX");
                        break;
                    case "Ethernet Information":
                        phoneConList[ConnectedIndex].updatePhoneInformation("EthernetInformationX");

                        break;
                    case "Port Information":
                        phoneConList[ConnectedIndex].updatePhoneInformation("PortInformationX");

                        break;
                    case "Device Log":
                        phoneConList[ConnectedIndex].updatePhoneInformation("DeviceLog");
                        phoneConList[ConnectedIndex].PhoneHTTPReqResponse -= OnPhoneHTTPReqResponseUpdateInfo;
                        phoneConList[ConnectedIndex].PhoneHTTPReqResponse += OnPhoneHTTPReqResponseUpdateLog;
                        return;


                    case "Streaming Statistics":
                        phoneConList[ConnectedIndex].updatePhoneInformation("StreamingStatisticsX");

                        break;


                }




            }

        }

        private void updatePhoneInfoDeviceLogOnly(string[] StatusEntries)
        {



            dataPhoneInfo.Invoke(new Action(() => dataPhoneInfo.Rows.Clear()));

            foreach (var statusentry in StatusEntries)
            {

                if (isNull(statusentry))
                {


                    // if it's null don't bother returning a value.


                }
                else
                {


                    dataPhoneInfo.Invoke(new Action(() => dataPhoneInfo.Rows.Add("Log Message: ", statusentry)));
                }

            }
            phoneConList[ConnectedIndex].PhoneHTTPReqResponse -= OnPhoneHTTPReqResponseUpdateLog;
        }
        private void OnPhoneHTTPReqResponseUpdateLog(object sender)
        {

            updatePhoneInfoDeviceLogOnly(phoneConList[ConnectedIndex].DeviceLogEntries);

        }
        private void OnPhoneHTTPReqResponseUpdateInfo(object sender)
        {
            phoneConList[ConnectedIndex].PhoneHTTPReqResponse -= OnPhoneHTTPReqResponseUpdateInfo;

            object PhoneInformation = new object();
            switch (cmbInformationTextString)
            {

                case "Network Configuration":
                    PhoneInformation = phoneConList[ConnectedIndex].phoneNetworkConfiguration;
                    break;
                case "Device Information":
                    PhoneInformation = phoneConList[ConnectedIndex].phoneDeviceInformation;
                    break;
                case "Ethernet Information":
                    PhoneInformation = phoneConList[ConnectedIndex].phoneEthernetInformation;

                    break;
                case "Port Information":
                    PhoneInformation = phoneConList[ConnectedIndex].phonePortInformation;

                    break;

                case "Streaming Statistics":

                    PhoneInformation = phoneConList[ConnectedIndex].phoneStreamingStatistics;
                    break;
            }


            System.Reflection.FieldInfo[] fieldarray = PhoneInformation.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);

            String[] values = new string[fieldarray.Length];
            String[] fieldnames = new string[fieldarray.Length];
            //dataPhoneInfo.Rows.Clear();

            int i = 0;
            foreach (System.Reflection.FieldInfo Field in fieldarray)
            {

                // Console.WriteLine("Field Name: " + Field.Name);
                //  Console.WriteLine("Value: " + Field.GetValue(PhoneInformation));
                if (isNull(Field.GetValue(PhoneInformation)))
                {
                    //   Console.WriteLine("Field: " + Field.Name + " Has no value, not added to grid");

                    fieldnames[i] = Field.Name;
                    // if it's null don't bother returning a value.

                    values[i] = "";
                }
                else
                {
                    fieldnames[i] = Field.Name;
                    values[i] = (string)Field.GetValue(PhoneInformation);

                }
                i++;
            }

            updatePhoneInfoGUI(fieldnames, values);



        }

        private void dataPhoneInfo_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!String.IsNullOrEmpty(dataPhoneInfo.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
            {
                frmMsgView frmMsg = new frmMsgView(dataPhoneInfo.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                frmMsg.Show();
            }


        }


        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {


            CancelAllTasks();
            saveSettings();


        }


        private void CancelAllTasks()
        {
            if (ConnectedIndex == -1)
            {
                return;
            }

            if (phoneConList[ConnectedIndex].connectionState == PhoneConnectionState.Connected)
            {

                Disconnect();
            }

        }


        private void picRight_MouseEnter(object sender, EventArgs e)
        {
            picNavRight.Image = Properties.Resources.ArrowRightHover;
        }

        private void picRight_MouseLeave(object sender, EventArgs e)
        {
            picNavRight.Image = Properties.Resources.ArrowRight;
        }

        private void picSelect_MouseHover(object sender, EventArgs e)
        {
            picNavSelect.Image = Properties.Resources.SelectPressed;
        }

        private void picSelect_MouseLeave(object sender, EventArgs e)
        {
            picNavSelect.Image = Properties.Resources.Select;
        }

        private void picLeft_MouseHover(object sender, EventArgs e)
        {
            picNavLeft.Image = Properties.Resources.ArrowLeftHover;
        }

        private void picLeft_MouseLeave(object sender, EventArgs e)
        {
            picNavLeft.Image = Properties.Resources.ArrowLeft;
        }

        private void picSelect_MouseEnter(object sender, EventArgs e)
        {

            picNavSelect.Image = Properties.Resources.SelectPressed;
        }

        private void picLeft_MouseEnter(object sender, EventArgs e)
        {

            picNavLeft.Image = Properties.Resources.ArrowLeftHover;
        }

        private void picDown_MouseEnter(object sender, EventArgs e)
        {
            picNavDwn.Image = Properties.Resources.ArrowDownHover;
        }

        private void picDown_MouseLeave(object sender, EventArgs e)
        {
            picNavDwn.Image = Properties.Resources.ArrowDown;
        }

        private void picUp_MouseEnter(object sender, EventArgs e)
        {
            picNavUp.Image = Properties.Resources.ArrowUpHover;
        }

        private void picUp_MouseLeave(object sender, EventArgs e)
        {
            picNavUp.Image = Properties.Resources.ArrowUp;

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            StartConnecting();
        }

        private void StartConnecting()
        {
            if (!String.IsNullOrEmpty(cmbPhone.Text))
            {
                if (ConnectedIndex != -1)
                {
                    if (phoneConList[ConnectedIndex].connectionState == PhoneConnectionState.Connected)
                    {
                        Disconnect();

                        return;
                    }
                }

                Connecting();


            }

            else
            {

                MessageBox.Show("No phone selected to connect.", "No phone Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Connecting()
        {

            ConnectedIndex = cmbPhone.SelectedIndex;
            cmbPhone.Enabled = false;
            btnConnect.Text = "Connecting...";

            btnConnect.Enabled = false;
            picLED.Image = Properties.Resources.led_green_off;

            try
            {
                phoneConList[ConnectedIndex].connectionState = PhoneConnectionState.Connecting;
                phoneConList[ConnectedIndex].PhoneHTTPReqResponse += OnPhoneHTTPReqResponseConnected;
                phoneConList[ConnectedIndex].PhoneHTTPReqResponseError += OnPhoneHTTPReqResponseError;
                phoneConList[ConnectedIndex].updatePhoneInformation("DeviceInformationX");



            }
            catch (CiscoPhoneObjectException E)
            {
                CiscoPhoneObjectErrorHandler(PhoneConnectionState.Unreachable, E);
            }

        }

        private void OnPhoneHTTPReqResponseError(object sender)
        {
            if (ConnectedIndex == -1)
            {
                return;
            }

            phoneConList[ConnectedIndex].PhoneHTTPReqResponseError -= OnPhoneHTTPReqResponseError;
            phoneConList[ConnectedIndex].connectionState = PhoneConnectionState.Unreachable;
            CiscoPhoneObjectErrorHandler(PhoneConnectionState.Unreachable);

        }
        private void CiscoPhoneObjectErrorHandler(PhoneConnectionState curPhoneState, CiscoPhoneObjectException CiscoException)
        {
            if (ConnectedIndex == -1)
            {

            }
            else
            {

                switch (curPhoneState)
                {

                    case PhoneConnectionState.Unreachable:
                        //phoneConList[ConnectedIndex].connectionState = PhoneConnectionState.Unreachable;

                        DisconnectGUIMethods();
                        Disconnect();

                        break;

                    case PhoneConnectionState.authFail:
                        MessageBox.Show("The screenshot for this phone could not be fetched. The most common cause of this is an authentication issue. Please see the help file for more details.", "Authentication failed.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        if (!cancellationTokenSourceScreenGrab.IsCancellationRequested)
                            cancellationTokenSourceScreenGrab.Cancel();
                        phoneConList[ConnectedIndex].PhoneConnectivityError = "Screenshotfail";
                        phoneConList[ConnectedIndex].screenshotDisabled = true;

                        break;

                    default:

                        break;


                }
            }
            Console.WriteLine("Error Handling Complete.");

        }
        private void CiscoPhoneObjectErrorHandler(PhoneConnectionState curPhoneState)
        {
            if (ConnectedIndex == -1)
            {

            }
            else
            {

                switch (curPhoneState)
                {

                    case PhoneConnectionState.Unreachable:
                        //phoneConList[ConnectedIndex].connectionState = PhoneConnectionState.Unreachable;
                        Disconnect();
                        DisconnectGUIMethods();
                        break;

                    case PhoneConnectionState.authFail:
                        MessageBox.Show("The screenshot for this phone could not be fetched. The most common cause of this is an authentication issue. Please see the help file for more details.", "Authentication failed.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        if (!cancellationTokenSourceScreenGrab.IsCancellationRequested)
                            cancellationTokenSourceScreenGrab.Cancel();
                        phoneConList[ConnectedIndex].PhoneConnectivityError = "Screenshotfail";
                        phoneConList[ConnectedIndex].screenshotDisabled = true;

                        break;

                    default:

                        break;


                }
            }
            Console.WriteLine("Error Handling Complete.");

        }
        private void CiscoPhoneObjectErrorHandler(PhoneConnectionState curPhoneState, string ErrorMessage)
        {
            if (ConnectedIndex == -1)
            {

            }
            else
            {

                switch (curPhoneState)
                {

                    case PhoneConnectionState.Unreachable:
                        //phoneConList[ConnectedIndex].connectionState = PhoneConnectionState.Unreachable;

                        DisconnectGUIMethods();
                        break;

                    case PhoneConnectionState.authFail:
                        MessageBox.Show("The screenshot for this phone could not be fetched. The most common cause of this is an authentication issue. Please see the help file for more details.", "Authentication failed.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        if (!cancellationTokenSourceScreenGrab.IsCancellationRequested)
                            cancellationTokenSourceScreenGrab.Cancel();
                        phoneConList[ConnectedIndex].PhoneConnectivityError = "Screenshotfail";
                        phoneConList[ConnectedIndex].screenshotDisabled = true;

                        break;

                    default:

                        break;


                }
            }
            Console.WriteLine("Error Handling Complete.");

        }
        private void OnPhoneHTTPReqResponseConnected(object sender)
        {
            Connected();



        }




        private void Disconnect()
        {

            if (ConnectedIndex == -1)
            {
                Console.WriteLine("Disconnect called when nothing connected somehow.");
                return;

            }
            phoneConList[ConnectedIndex].connectionState = PhoneConnectionState.Disconnected;

            CancelAllListeningTasks();


            this.Invoke(new updateBtnDisconnect(DisconnectGUIMethods));

            // cancel screenshot if its still running.
            if (!cancellationTokenSourceScreenGrab.IsCancellationRequested)
                cancellationTokenSourceScreenGrab.Cancel();


            try
            {
                WS.Stop();
                WS2.Stop();
            }
            catch (Exception E)
            {

            }

            ConnectedIndex = -1;

        }

        private void CancelAllListeningTasks()
        {
            if (ConnectedIndex == -1)
            {
                return;
            }
            if (phoneConList[ConnectedIndex].listenMode == PhoneListenMode.BasicMode)
            {
                stopBasicListeningMode();

            }
            if (phoneConList[ConnectedIndex].listenMode == PhoneListenMode.AdvancedMode)
            {
                stopAdvancedListenMode();
            }
        }

        private void DisconnectGUIMethods()
        {

            btnConnect.Enabled = true;
            btnConnect.Text = "Connect";

            ScreenshotHolder.CurrentScreenshot = ScreenshotHolder.ChangeOpacity(ScreenshotHolder.CurrentScreenshot, .30F);

            picLED.Image = Properties.Resources.led_red_black;
            dataPhoneInfo.Rows.Clear();
            cmbPhone.Enabled = true;



        }
        private void ConnectGUIMethods()
        {
            picLED.Image = Properties.Resources.led_green_on;
            btnConnect.Text = "Disconnect";
            btnConnect.Enabled = true;
            ConnectedIndex = cmbPhone.SelectedIndex;
            ResizeSoftkeys(phoneConList[ConnectedIndex].NumSoftKeys);
            saveSettings();

        }
        private void Connected()
        {

            phoneConList[ConnectedIndex].connectionState = PhoneConnectionState.Connected;
            phoneConList[ConnectedIndex].PhoneHTTPReqResponse -= OnPhoneHTTPReqResponseConnected;
            this.Invoke(new ConnectInvoke(ConnectGUIMethods));
            StartScreenshotMonitor();








        }
        private async void getScreenshot()
        {

            try
            {


                CiscoPhoneObject curPhoneObject = phoneConList[ConnectedIndex];

                restClientScreenshot = new RestClientPhoneControl(@"http" + isHTTPSConnection(curPhoneObject) + curPhoneObject.ip + @"/CGI/Screenshot", curPhoneObject.username, curPhoneObject.password);
                if (phoneConList[ConnectedIndex].ScreenshotMethod == PhoneScreenshotMethod.OldMethod)
                {
                    MessageBox.Show("Screenshots not currently supported for Older Generation phones :(");

                    return;
                }


                while (!cancellationTokenScreenGrab.IsCancellationRequested)
                {


                    Image screenshot;
                    using (Task<Image> getScreenshotTask = new Task<Image>(new Func<Image>(restClientScreenshot.getScreenshot), cancellationTokenScreenGrab))
                    {
                        getScreenshotTask.Start();
                        try
                        {
                            screenshot = await getScreenshotTask;
                        }
                        catch (Exception E)
                        {
                            // The actual task exception needs to be caught cause we have to cancel the task.
                            cancellationTokenSourceScreenGrab.Cancel();
                            throw E;
                        }


                    }


                    if (!isNull(screenshot))
                    {

                        ScreenshotHolder.CurrentScreenshot = screenshot;







                    }
                    else
                    {
                        // Connection succeeded but screenshot not returned

                        throw new Exception("Disconnected due to screenshot returning null");

                    }
                    await Task.Delay(curPhoneObject.screenshotInterval, cancellationTokenScreenGrab);


                }
            }
            catch (TaskCanceledException E)
            {
                // No need to do anything here, this is a perfectly OK exception.
                System.Diagnostics.Debug.WriteLine("Screenshot retrieval Cancelled.");

            }
            catch (ArgumentException bigError)
            {

                System.Diagnostics.Debug.WriteLine("Get Screenshot Caught an AggregateException: " + bigError.Message);
                CiscoPhoneObjectErrorHandler(PhoneConnectionState.Unreachable);

            }
            catch (AggregateException E)
            {

                System.Diagnostics.Debug.WriteLine("Get Screenshot Caught an AggregateException: " + E.Message);
                CiscoPhoneObjectErrorHandler(PhoneConnectionState.authFail);


            }
            catch (Exception E)
            {
                System.Diagnostics.Debug.WriteLine("Get Screenshot Caught an Exception: " + E.Message);
                CiscoPhoneObjectErrorHandler(PhoneConnectionState.authFail);

            }

            //TODO: Fade the image.



            Disconnect();
        }




        private void DisplayAuthError()
        {
            MessageBox.Show("Error authenticating to phone, please check user has the phone listed as a controlled device in CUCM.", "Auth connection failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            try
            {

                if (!String.IsNullOrEmpty(phoneConList[ConnectedIndex].ip))
                {
                    if (phoneConList[ConnectedIndex].phoneNetworkConfiguration.AuthenticationURL.Contains("informacast"))
                    {
                        MessageBox.Show("The issue may also be your enterprise service param for the auth URL, which is set to: " + phoneConList[ConnectedIndex].phoneNetworkConfiguration.AuthenticationURL + " I noticed this was an informacast URL. Try setting your Authentication URL on the phone to the following address..");
                        MessageBox.Show(@"http://X.X.X.X/ccmcip/authenticate.jsp or https://X.X.X.X/ccmip/authenticate.jsp where X.X.X.X is the IP of your CUCM.");

                    }
                    else
                    {

                        MessageBox.Show("The issue may also be your Enterprise Service Parameter for the authentication URL, which is currently: " + phoneConList[ConnectedIndex].phoneNetworkConfiguration.AuthenticationURL + " is this the correct auth URL or do you need to update your service URL's?", "Check Service Parameter", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                MessageBox.Show("Try Disabling screenshot retrieval for this phone to use the phone information tab.");
            }
            catch (Exception E)
            {

            }
            Disconnect();
        }

        private void btnPhoneInteraction_Click(object sender, EventArgs e)
        {
            if (ConnectedIndex == -1)
            {
                MessageBox.Show("Not connected to a phone. Please select a phone from the dropdown list above, or add one via the settings tab before pressing a key.", "Please connect to a phone", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (phoneConList[ConnectedIndex].connectionState == PhoneConnectionState.Connected)
            {
                System.Windows.Forms.Button pressedButton = (System.Windows.Forms.Button)sender;
                Console.WriteLine(pressedButton.Name + " was pressed");




                sendKey(pressedButton.Name.Replace("btn", ""));
            }
            else
            {
                MessageBox.Show("Not connected to a phone. Please select a phone from the dropdown list above, or add one via the settings tab before pressing a key.", "Please connect to a phone", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


        }


        private void pctPhoneInteraction_Clicked(object sender, EventArgs e)
        {
            if (ConnectedIndex == -1)
            {
                MessageBox.Show("Not connected to a phone. Please select a phone from the dropdown list above, or add one via the settings tab before pressing a key.", "Please connect to a phone", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (phoneConList[ConnectedIndex].connectionState == PhoneConnectionState.Connected)
            {
                System.Windows.Forms.PictureBox pressedPicture = (System.Windows.Forms.PictureBox)sender;

                System.Diagnostics.Debug.WriteLine(pressedPicture.Name + " was pressed");
                sendKey(pressedPicture.Name.Replace("pic", ""));
            }
            else
            {
                MessageBox.Show("Not connected to a phone. Please select a phone from the dropdown list above, or add one via the settings tab before pressing a key.", "Please connect to a phone", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
        private void playKeySound(string key)
        {
            System.Media.SoundPlayer audio;
            switch (key)
            {
                // convert all audio to PCM unsigned 8 bit and it will work great :).

                case "KeyPad0":
                    audio = new System.Media.SoundPlayer(Properties.Resources.Dtmf_0); // here WindowsFormsApplication1 is the namespace and AddPhone is the audio file name
                    audio.Play();
                    break;


                case "KeyPad1":
                    audio = new System.Media.SoundPlayer(Properties.Resources.Dtmf_1); // here WindowsFormsApplication1 is the namespace and AddPhone is the audio file name
                    audio.Play();
                    break;



                case "KeyPad2":
                    audio = new System.Media.SoundPlayer(Properties.Resources.Dtmf_2); // here WindowsFormsApplication1 is the namespace and AddPhone is the audio file name
                    audio.Play();
                    break;



                case "KeyPad3":
                    audio = new System.Media.SoundPlayer(Properties.Resources.Dtmf_3); // here WindowsFormsApplication1 is the namespace and AddPhone is the audio file name
                    audio.Play();
                    break;



                case "KeyPad4":
                    audio = new System.Media.SoundPlayer(Properties.Resources.Dtmf_4); // here WindowsFormsApplication1 is the namespace and AddPhone is the audio file name
                    audio.Play();
                    break;



                case "KeyPad5":
                    audio = new System.Media.SoundPlayer(Properties.Resources.Dtmf_5); // here WindowsFormsApplication1 is the namespace and AddPhone is the audio file name
                    audio.Play();
                    break;



                case "KeyPad6":
                    audio = new System.Media.SoundPlayer(Properties.Resources.Dtmf_6); // here WindowsFormsApplication1 is the namespace and AddPhone is the audio file name
                    audio.Play();
                    break;



                case "KeyPad7":
                    audio = new System.Media.SoundPlayer(Properties.Resources.Dtmf_7); // here WindowsFormsApplication1 is the namespace and AddPhone is the audio file name
                    audio.Play();
                    break;



                case "KeyPad8":
                    audio = new System.Media.SoundPlayer(Properties.Resources.Dtmf_8); // here WindowsFormsApplication1 is the namespace and AddPhone is the audio file name
                    audio.Play();
                    break;



                case "KeyPad9":
                    audio = new System.Media.SoundPlayer(Properties.Resources.Dtmf_9); // here WindowsFormsApplication1 is the namespace and AddPhone is the audio file name
                    audio.Play();
                    break;



                case "KeyPadPound":
                    audio = new System.Media.SoundPlayer(Properties.Resources.Dtmf_pound); // here WindowsFormsApplication1 is the namespace and AddPhone is the audio file name
                    audio.Play();
                    break;



                case "KeyPadStar":
                    audio = new System.Media.SoundPlayer(Properties.Resources.Dtmf_star); // here WindowsFormsApplication1 is the namespace and AddPhone is the audio file name
                    audio.Play();
                    break;




                default:
                    break;

            }


        }
        private void sendKey(string key)
        {
            playKeySound(key);
            try
            {
                if (ConnectedIndex == -1)
                {
                    return;
                }
                if (phoneConList[ConnectedIndex].connectionState == PhoneConnectionState.Connected)
                {
                    phoneConList[ConnectedIndex].sendKey(key);

                }


            }
            catch (CiscoPhoneObjectException CiscoE)
            {
                Console.WriteLine(CiscoE.LastErrorMessage);
                CiscoPhoneObjectErrorLogging(CiscoE);
            }
            catch (Exception E)
            {
                Console.WriteLine("General Exception Detected:");
                Console.WriteLine("Unable to make a keypress request, failed: " + E.Message);


            }
        }

        private void sendKeyBulk(string key, int Index)
        {

            try
            {
                phoneConList[Index].sendKey(key);




            }
            catch (CiscoPhoneObjectException CiscoE)
            {
                Console.WriteLine(CiscoE.LastErrorMessage);
                CiscoPhoneObjectErrorLogging(CiscoE);
            }
            catch (Exception E)
            {
                Console.WriteLine("General Exception Detected:");
                Console.WriteLine("Unable to make a keypress request, failed: " + E.Message);


            }
        }
        private int getTrkValue()
        {

            string timervalue = trkTimer.Value.ToString() + "000";

            return Int32.Parse(timervalue);

        }





        private void stopMsgSend()
        {
            sendingMsg = false;
            prgSendMsg.Value = 0;
            lblMsgStatus.Visible = true;
            prgSendMsg.Visible = false;
        }
        private void btnSendMsg_Click(object sender, EventArgs e)
        {
            sendMsgStart();

        }
        private void sendMsgStart()
        {
            if (sendingMsg)
            {

                MessageBox.Show("Please cancel existing message send before attempting to send another", "Currently Sending Msg", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txtMessage.Text.Length == 0)
            {


                DialogResult dialogResult = MessageBox.Show("Are you sure you want to send a blank message?", "Blank Message Warning.", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {

                }
                else if (dialogResult == DialogResult.No)
                {
                    return;

                }
            }


            if (lstMsgReceivers.SelectedItems.Count > 0)
            {
                prgSendMsg.Visible = true;
                lblMsgStatus.Visible = true;
                sendingMsg = true;
                sendChatMsgFactory();
                btnCancelSend.Enabled = true;
            }
            else
            {
                MessageBox.Show("No Phones to recieve message selected. Please select phones to send the message to.", "Select phones to receive message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

            }
        }
        private async void sendChatMsgFactory()
        {
            lblMsgStatus.Text = "Sending...";
            cancellationTokenSourceSendMsg = new CancellationTokenSource();

            cancellationTokenSendMsg = cancellationTokenSourceSendMsg.Token;

            //             CiscoPhoneObject[] msgReceipientPhone =  new CiscoPhoneObject[lstMsgReceivers.SelectedItems.Count];

            CiscoPhoneObject[] msgReceipientPhone = getBulkMethodCiscoPhoneObjects(lstMsgReceivers);


            // calculate how much to extend the progress bar by.
            int increasevalueby = 10000 / msgReceipientPhone.Count();

            foreach (CiscoPhoneObject msgReceiver in msgReceipientPhone)
            {



                if (cancellationTokenSendMsg.IsCancellationRequested)
                {

                    break;
                }



                object[] arrObjects = new object[] { msgReceiver, txtMessage.Text };
                using (Task<string> sendChatTask = new Task<string>(new Func<object, string>(sendChatMsgAction), arrObjects, cancellationTokenSendMsg))
                {
                    try
                    {
                        sendChatTask.Start();
                        await sendChatTask;

                    }
                    catch (CiscoPhoneObjectException E)
                    {
                        System.Diagnostics.Debug.WriteLine("Couldn't send msg to phone " + msgReceiver.PhoneDisplayName);
                        lstMsgReceivers.Items[lstMsgReceivers.FindString(msgReceiver.PhoneDisplayName)] = msgReceiver.PhoneDisplayName + " (Sending msg Failed)";
                        CiscoPhoneObjectErrorLogging(E);

                    }
                    prgSendMsg.Value += increasevalueby;

                }



            }

            if (cancellationTokenSendMsg.IsCancellationRequested)
            {
                lblMsgStatus.Text = "Last Msg Send Cancelled";
                stopMsgSend();
            }
            else
            {
                lblMsgStatus.Text = "Last Msg Send Complete";
                stopMsgSend();
            }


        }

        private void CiscoPhoneObjectErrorLogging(CiscoPhoneObjectException E)
        {
            ///TODO: make this log error messages etc. that are sent as exceptions.
            ///
        }

        private CiscoPhoneObject[] getBulkMethodCiscoPhoneObjects(ListBox ListoBoxBulkCiscoPhones)
        {
            CiscoPhoneObject[] bulkPhones = new CiscoPhoneObject[ListoBoxBulkCiscoPhones.SelectedIndices.Count];

            int bulkPhoneCount = 0;

            for (int i = 0; i < phoneConList.Count; i++)
            {
                if (ListoBoxBulkCiscoPhones.SelectedIndices.Contains(i))
                {
                    bulkPhones[bulkPhoneCount] = phoneConList[i];

                    bulkPhoneCount++;


                }
            }

            return bulkPhones;
        }

        private string sendBulkEditAction(object state)
        {

            object[] arrObjects = (object[])state;
            CiscoPhoneObject msgPhone = (CiscoPhoneObject)arrObjects[0];
            int Index = phoneConList.IndexOf(msgPhone);

            string cmd = (string)arrObjects[1];

            if (cmd.Contains("Delay"))
            {
                cmd = cmd.Replace("Delay ", "");
                System.Diagnostics.Debug.WriteLine("Thread is sleeping for: " + cmd + "000 milliseconds");
                Thread.Sleep(Int32.Parse(cmd + "000"));


            }
            else
            {
                try
                {
                    sendKeyBulk(cmd, Index);

                }
                catch (Exception E)
                {
                    return "Error Sending";
                }
            }
            return "Sent Command.";

        }
        private string dialNumber(object state)
        {

            /*<CiscoIPPhoneExecute>
<ExecuteItem URL="Dial:918005551212:1:Cisco/Dialer"/>
</CiscoIPPhoneExecute>
*/
            object[] arrObjects = (object[])state;

            CiscoPhoneObject phoneDial = (CiscoPhoneObject)arrObjects[0];
            string number = (string)arrObjects[1];
            Console.WriteLine("sending dialing number to: " + phoneDial.password);
            try
            {

                RestClientPhoneControl restClientChatMsg = new RestClientPhoneControl(phoneDial.ip, phoneDial.username, phoneDial.password);
                restClientChatMsg.EndPoint = @"http" + isHTTPSConnection(phoneDial) + phoneDial.ip + @"/CGI/Execute";
                restClientChatMsg.Method = HttpVerb.POST;


                restClientChatMsg.PostData = "XML=<CiscoIPPhoneExecute><ExecuteItem URL=\"Dial:" + number + "\"/></CiscoIPPhoneExecute>";

                    
                    Console.WriteLine("chat send sent the following XML:" + "XML=<CiscoIPPhoneExecute><ExecuteItem URL = \"Dial:" + number + "\"/></CiscoIPPhoneExecute> to the ip address: " + phoneDial.ip);


                restClientChatMsg.MakeRequest();
            }
            catch (WebException WebE)
            {

                throw new CiscoPhoneObjectException("HTTPConnectFailed", phoneDial, WebE);

            }
            catch (Exception E)
            {

                throw new CiscoPhoneObjectException("GeneralException", phoneDial, E);



            }


            return "complete";


        }
        private string sendChatMsgAction(object state)
        {
            object[] arrObjects = (object[])state;

            CiscoPhoneObject msgPhone = (CiscoPhoneObject)arrObjects[0];
            string chatTextMsg = (string)arrObjects[1];
            Console.WriteLine("Sending Msg to: " + msgPhone.password);


            /*
             * should be: XML=<CiscoIPPhoneText>
<Title>Title text goes here</Title>
<Prompt>The prompt text goes here</Prompt>
<Text>The text to be displayed as the message body goes here</Text>
</CiscoIPPhoneText>*/

            try
            {

                RestClientPhoneControl restClientChatMsg = new RestClientPhoneControl(msgPhone.ip, msgPhone.username, msgPhone.password);
                restClientChatMsg.EndPoint = @"http" + isHTTPSConnection(msgPhone) + msgPhone.ip + @"/CGI/Execute";
                restClientChatMsg.Method = HttpVerb.POST;


                restClientChatMsg.PostData = "XML=<CiscoIPPhoneText><Text>" + chatTextMsg + "</Text></CiscoIPPhoneText>";
                //  RestClient.PutData = "XML=<CiscoIPPhoneExecute><ExecuteItem URL=\"Key:" + key + "\"/></CiscoIPPhoneExecute>";

                Console.WriteLine("chat send sent the following XML:" + "XML=<CiscoIPPhoneText><Text>" + chatTextMsg + "</Text></CiscoIPPhoneText> to the ip address: " + msgPhone.ip);


                restClientChatMsg.MakeRequest();
            }
            catch (WebException WebE)
            {

                throw new CiscoPhoneObjectException("HTTPConnectFailed", msgPhone, WebE);

            }
            catch (Exception E)
            {

                throw new CiscoPhoneObjectException("GeneralException", msgPhone, E);



            }


            return "complete";
        }

        //public static DateTime GetNetworkTime()
        //{
        //    const string ntpServer = "pool.ntp.org";
        //    var ntpData = new byte[48];
        //    ntpData[0] = 0x1B; //LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)

        //    var addresses = Dns.GetHostEntry(ntpServer).AddressList;
        //    var ipEndPoint = new IPEndPoint(addresses[0], 123);
        //    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //    socket.ReceiveTimeout = 1000;
        //    socket.SendTimeout = 1000;
        //    socket.Connect(ipEndPoint);
        //    socket.Send(ntpData);
        //    socket.Receive(ntpData);
        //    socket.Close();

        //    ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
        //    ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];

        //    var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
        //    var networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);

        //    return networkDateTime;
        //}







        private void tabPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (tabControl1.SelectedTab == tabPhone)
            {

            }
            else
            {
                return;
            }
            if (ConnectedIndex == -1)
            {
                return;
            }
            if (phoneConList[ConnectedIndex].connectionState == PhoneConnectionState.Connected)
            {
                if (e.KeyChar == '0' || e.KeyChar == '1' || e.KeyChar == '2' || e.KeyChar == '3' || e.KeyChar == '4' || e.KeyChar == '5' || e.KeyChar == '6' || e.KeyChar == '7' || e.KeyChar == '8' || e.KeyChar == '9')
                {

                    sendKey("KeyPad" + e.KeyChar);
                }
                if (e.KeyChar == '#')
                {
                    sendKey("KeyPadPound");
                }
                if (e.KeyChar == '*')
                {
                    sendKey("KeyPadStar");

                }
            }

        }

        private void tabChat_Enter(object sender, EventArgs e)
        {
            lstMsgReceivers.Items.Clear();
            lstMsgReceivers.Items.AddRange(lstDataBindPhones.Items);

        }


        private void startSIPStack()
        {
            try
            {

                sipStack = null;
                sipStack = new SIP_Stack();


                sipStack.UserAgent = "CCIERants Phone Control";
                sipStack.BindInfo = new IPBindInfo[] { new IPBindInfo("", BindInfoProtocol.TCP, IPAddress.Any, 5060) };

                //m_pStack.Allow
                sipStack.Error += new EventHandler<ExceptionEventArgs>(sipStackErrorHandler);
                sipStack.RequestReceived += new EventHandler<SIP_RequestReceivedEventArgs>(sipStackRequestReceiveHandler);

                sipStack.Start();

                sipStackUDP = null;
                sipStackUDP = new SIP_Stack();
                sipStackUDP.UserAgent = "CCIERants Phone Control";
                sipStackUDP.BindInfo = new IPBindInfo[] { new IPBindInfo("", BindInfoProtocol.UDP, IPAddress.Any, 5060) };
                sipStackUDP.Error += new EventHandler<ExceptionEventArgs>(sipStackErrorHandler);
                sipStackUDP.RequestReceived += new EventHandler<SIP_RequestReceivedEventArgs>(sipStackRequestReceiveHandler);

                sipStackUDP.Start();
            }
            catch (Exception E)
            {
                System.Diagnostics.Debug.WriteLine("SIP Stack Error: " + E.Message);
                Console.WriteLine("SIP Stack Error: " + E.Message);

            }

        }
        private void ProcessMediaOffer(SIP_Dialog dialog, SIP_ServerTransaction transaction, RTP_MultimediaSession rtpMultimediaSession, SDP_Message offer, SDP_Message localSDP)
        {


            //                SDP_Message sdpOffer = SDP_Message.Parse(Encoding.UTF8.GetString(e.Request.Data));


            // Build local SDP template
            SDP_Message sdpLocal = new SDP_Message();
            sdpLocal.Version = "0";
            sdpLocal.Origin = new SDP_Origin("-", sdpLocal.GetHashCode(), 1, "IN", "IP4", sipListenerIP);
            sdpLocal.SessionName = "SIP Call";
            sdpLocal.Times.Add(new SDP_Time(0, 0));

            string[] mediaFormats = new string[] { "0" };

            SDP_MediaDescription answerMedia = new SDP_MediaDescription(SDP_MediaTypes.audio, RTPPortCountStream1, 0, "RTP/AVP", mediaFormats);





            //                            Dictionary<int, LumiSoft.Net.Media.Codec.Audio.AudioCodec> audioCodecs = GetOurSupportedAudioCodecs(offerMedia);
            answerMedia.MediaFormats.Clear();
            answerMedia.Attributes.Clear();
            LumiSoft.Net.Media.Codec.Audio.PCMU g711codec = new LumiSoft.Net.Media.Codec.Audio.PCMU();

            int codeckey = 0;
            answerMedia.Attributes.Add(new SDP_Attribute("rtpmap", 0 + " " + g711codec.Name + "/" + g711codec.CompressedAudioFormat.SamplesPerSecond));
            answerMedia.MediaFormats.Add(codeckey.ToString());
            answerMedia.Attributes.Add(new SDP_Attribute("ptime", "20"));
            int port = 24670 + RTPResponsePortCount;
            answerMedia.Attributes.Add(new SDP_Attribute("rport", port.ToString()));
            answerMedia.Connection = new SDP_Connection("IN", "IP4", sipListenerIP);


            sdpLocal.MediaDescriptions.Add(answerMedia);


            SIP_Response response = sipStack.CreateResponse(SIP_ResponseCodes.x200_Ok, transaction.Request, transaction.Flow);
            //response.Contact = SIP stack will allocate it as needed;
            response.ContentType = "application/sdp";
            response.Data = sdpLocal.ToByte();

            transaction.SendResponse(response);
            RTPResponsePortCount++;

            sipStack.RequestReceived -= sipStackRequestReceiveHandler;
            sipStackUDP.RequestReceived -= sipStackRequestReceiveHandler;
            // ProcessMediaOffer(dialog,e.ServerTransaction,rtpMultimediaSession,sdpOffer,sdpLocal);

            // Create call.
            //    m_pCall = new SIP_Call(m_pStack,dialog,rtpMultimediaSession,sdpLocal);
            //     m_pCall.StateChanged += new EventHandler(m_pCall_State
            // Call ready to be accepted!


        }
        private void sipStackRequestReceiveHandler(object sender, SIP_RequestReceivedEventArgs e)
        {

            if (e.Request.RequestLine.Method == SIP_Methods.INVITE)
            {

                SIP_Response responseRinging = sipStack.CreateResponse(SIP_ResponseCodes.x180_Ringing, e.Request, e.Flow);
                responseRinging.To.Tag = LumiSoft.Net.SIP.SIP_Utils.CreateTag();
                e.ServerTransaction.SendResponse(responseRinging);


                SIP_Dialog_Invite dialog = (SIP_Dialog_Invite)sipStack.TransactionLayer.GetOrCreateDialog(e.ServerTransaction, responseRinging);

                this.BeginInvoke(new MethodInvoker(delegate ()
                {
                    try
                    {

                        SDP_Message sdpLocal = new SDP_Message();
                        sdpLocal.Version = "0";
                        sdpLocal.Origin = new SDP_Origin("-", sdpLocal.GetHashCode(), 1, "IN", "IP4", sipListenerIP);
                        sdpLocal.SessionName = "SIP Call";
                        sdpLocal.Times.Add(new SDP_Time(0, 0));
                        string[] mediaFormats = new string[] { "0" };


                        ProcessMediaOffer(dialog, e.ServerTransaction, null, null, sdpLocal);





                    }
                    catch (Exception x1)
                    {

                    }
                }));

            }
            else
            {

            }


        }



        private void sipStackErrorHandler(object sender, ExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Error Occured: " + e.Exception.Message);

        }
        private void startBasicListeningMode()
        {


            if (phoneConList[ConnectedIndex].connectionState == PhoneConnectionState.Connected && checkIPRemoveHTTP(cmbListenIP.Text))
            {


                string listenIP = cmbListenIP.Text;

                RTPPortCountXML += RTPBasePortCountXML;

                startRTPListenerXMLMethod(RTPPortCountXML.ToString());

                Thread sendListenCmdThread = new System.Threading.Thread(() => this.sendListenerCMDtoPhone(listenIP));
                sendListenCmdThread.Start();



                RTPBasePortCountXML += 10;

            }
            else
            {
                phoneConList[ConnectedIndex].listenMode = PhoneListenMode.NotListening;
                MessageBox.Show("Either listener IP selection is invalid or you are not connected to a phone.", "AddPhone to phone first and check Listener IP.", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }

        private void startAdvancedListeningMode()
        {
            if (phoneConList[ConnectedIndex].connectionState == PhoneConnectionState.Connected && checkIPRemoveHTTP(cmbListenIP.Text))
            {
                try
                {

                }
                catch (Exception E)
                {
                    Console.WriteLine("Major error starting SIP stack," + E.Message);
                    CiscoPhoneObjectErrorHandler(phoneConList[ConnectedIndex].connectionState, "SIP Stack Error: " + E.Message);

                }
            }
            else
            {
                phoneConList[ConnectedIndex].listenMode = PhoneListenMode.NotListening;
                MessageBox.Show("Either listener IP selection is invalid or you are not connected to a phone.", "AddPhone to phone first and check Listener IP.", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }


            RTPPortCountStream1 += RTPBasePortCount;
            RTPPortCountStream2 += RTPBasePortCount;
             RTPReceiveStream1 = new ccierants.baseclasses.RTPClientAsync(RTPPortCountStream1);

            var            rtpListenerStream1 = new System.Threading.Thread(() => this.startRTPListenerStream1(RTPPortCountStream1.ToString()));
            rtpListenerStream1.Start();
            RTPReceiveStream2 = new ccierants.baseclasses.RTPClientAsync(RTPPortCountStream2);
            var rtpListenerStream2 = new System.Threading.Thread(() => this.startRTPListenerStream2(RTPPortCountStream2.ToString()));

            rtpListenerStream2.Start();

            btnStop.Enabled = true;
            RTPBasePortCount += 10;




        }

        private void btnStartListening_Click(object sender, EventArgs e)
        {



            if (chkBasicListenMethod.Checked && chkAdvancedMode.Checked)
            {
                MessageBox.Show("Cannot have both listen methods selected at the same time.");
                return;
            }
            if (String.IsNullOrEmpty(cmbListenIP.Text))
            {
                MessageBox.Show("You must select a listening IP Address from the dropdown.");
                return;

            }
            if (ConnectedIndex == -1)
            {
                MessageBox.Show("Not connected to a phone. Please select a phone from the dropdown list above, or add one via the settings tab before pressing a key.", "Please connect to a phone", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (phoneConList[ConnectedIndex].connectionState == PhoneConnectionState.Connected)
            {
                if (phoneConList[ConnectedIndex].listenMode == PhoneListenMode.NotListening)
                {
                    StartListening();
                    return;
                }
                else
                {
                    MessageBox.Show("A listen session is currently in progress or in the process of being cancelled. Please try again.");
                    return;

                }



            }
            else
            {
                MessageBox.Show("Not connected to a phone. Please select a phone from the dropdown list above, or add one via the settings tab before pressing a key.", "Please connect to a phone", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }



        }

        private void StartListening()
        {

            if (chkBasicListenMethod.Checked)
            {
                phoneConList[ConnectedIndex].listenMode = PhoneListenMode.BasicMode;
                startBasicListeningMode();
                return;
            }
            if (chkAdvancedMode.Checked)
            {
                phoneConList[ConnectedIndex].listenMode = PhoneListenMode.AdvancedMode;
                startAdvancedListeningMode();
                return;
            }
            MessageBox.Show("Please select a Listen method Basic or Advanced.", "Select listen method", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void sendListenerCMDtoPhone(string listenip)
        {
            try
            {


                RestClientPhoneControl RestClientStartListen = new RestClientPhoneControl(phoneConList[ConnectedIndex].ip, phoneConList[ConnectedIndex].username, phoneConList[ConnectedIndex].password);
                RestClientStartListen.EndPoint = @"http" + isHTTPSConnection() + phoneConList[ConnectedIndex].ip + @"/CGI/Execute";
                RestClientStartListen.Method = HttpVerb.POST;

                string xmlinput = @"XML=<startMedia>
<mediaStream>
<type>audio</type>
<codec>G.711</codec>
<mode>send</mode>
<address>" + listenip + @"</address>
<port>" + RTPPortCountXML.ToString() + @"</port>
</mediaStream>
</startMedia>";
                RestClientStartListen.PostData = xmlinput;

                Console.WriteLine("XML to the ip address: " + xmlinput + " to this phone: " + phoneConList[ConnectedIndex].ip + " with this username: " + phoneConList[ConnectedIndex].username);



                RestClientStartListen.MakeRequest();

            }
            catch (Exception e)
            {

                Console.WriteLine("Phone did not accept the startmedia method");
                phoneConList[ConnectedIndex].listenMode = PhoneListenMode.NotListening;

            }

        }
        private void stopSIPandRTPListen()
        {
            try
            {
                if (sipStackUDP != null)
                {



                    sipStackUDP.Stop();
                    sipStack.Stop();
                    sipStack.Dispose();
                    sipStackUDP.Dispose();
                    sipStack.RequestReceived -= sipStackRequestReceiveHandler;
                    sipStackUDP.RequestReceived -= sipStackRequestReceiveHandler;
                    sipStack.Error -= sipStackErrorHandler;
                    sipStackUDP.Error -= sipStackErrorHandler;
                    RTPReceiveStream1.Dispose();
                    RTPReceiveStream2.Dispose();
                    wavPlayer2SsrcID = 0;
                    wavPlayerSsrcID = 0;
                }
                else
                {

                }
            }
            catch(NullReferenceException E)
            {


            }


            //try
            //{
            //    while (rtpListenerStream1.IsAlive)
            //    {
            //        rtpListenerStream1.Abort();
            //    }
            //    while (rtpListenerStream2.IsAlive)
            //    {

            //        rtpListenerStream2.Abort();
            //    }
            ////}
            //catch (Exception E)
            //{
            //    MessageBox.Show("Error aborting the stream threads!!");


            //}

            RTPReceiveStream2.packetReceived -= RTPReceive_packetReceivedStream2;
            RTPReceiveStream1.packetReceived -= RTPReceive_packetReceivedStream1;

            Console.WriteLine("succesfully stopped the RTP Listen thread and set Currently Listen to False");

        }
        private void stopAdvancedListenMode()
        {
            phoneConList[ConnectedIndex].listenMode = PhoneListenMode.NotListening;
            Thread stopSIPandRTPListenThread = new System.Threading.Thread(() => this.stopSIPandRTPListen());
            stopSIPandRTPListenThread.Start();

        }
        private void startRTPListenerStream2(string port)
        {

            try
            {





                RTPReceiveStream2 = new ccierants.baseclasses.RTPClientAsync(int.Parse(port));
                RTPReceiveStream2.printHeader = chkPrintHeader.Checked;

                RTPReceiveStream2.packetReceived += RTPReceive_packetReceivedStream2;
                RTPReceiveStream2.StartClient();
            }
            catch (Exception E)
            {

                MessageBox.Show("Could not start RTP Listener, ensure you can bind port " + RTPPortCountStream2.ToString() + " to the selected IP Address and try again.");

            }
        }
        private void startRTPListenerStream1(string port)
        {
            try
            {


                RTPReceiveStream1 = new ccierants.baseclasses.RTPClientAsync(int.Parse(port));
                RTPReceiveStream1.printHeader = chkPrintHeader.Checked;

                RTPReceiveStream1.StartClient();
                RTPReceiveStream1.packetReceived += RTPReceive_packetReceivedStream1;
            }
            catch (Exception E)
            {

                MessageBox.Show("Could not start RTP Listener, ensure you can bind port " + RTPPortCountStream1.ToString() + " to the selected IP Address and try again.");
                CiscoPhoneObjectErrorHandler(phoneConList[ConnectedIndex].connectionState);

            }
        }
        private void startRTPListenerXMLMethod(string port)
        {
            System.Diagnostics.Debug.WriteLine("RTPClientReceivePacketStarted.");
            RTPReceiveXMLMethod = new ccierants.baseclasses.RTPClientAsync(int.Parse(port));
            RTPReceiveXMLMethod.printHeader = chkPrintHeader.Checked;
            RTPReceiveXMLMethod.packetReceived += RTPReceive_packetReceivedXMLMethod;
            RTPReceiveXMLMethod.StartClient();

        }


        private void RTPReceive_packetReceivedStream2(object sender, RTPPacketReceievedEventArgs e)
        {

            if (wavPlayer2SsrcID == 0)
            {
                Console.WriteLine("Received first Packet for Stream 2");
                wavPlayer2.AddSamples(e.packetpayload);
                wavPlayer2SsrcID = e.ssrcID;
            }
            else if (wavPlayer2SsrcID == e.ssrcID)
            {
                wavPlayer2.AddSamples(e.packetpayload);
            }





        }
        private void RTPReceive_packetReceivedXMLMethod(object sender, RTPPacketReceievedEventArgs e)
        {
            if (chkStartPlaying.Checked)
            {
                wavPlayer3.AddSamples(e.packetpayload);
            }


            packetReceiveFlashLed();




        }
        private void RTPReceive_packetReceivedStream1(object sender, RTPPacketReceievedEventArgs e)
        {
            if (wavPlayerSsrcID == 0)
            {
                Console.WriteLine("Received first Packet for Stream 1, SSRC Id: " + e.ssrcID);
                wavPlayer.AddSamples(e.packetpayload);
                wavPlayerSsrcID = e.ssrcID;

            }
            else if (wavPlayerSsrcID == e.ssrcID)
            {

                wavPlayer.AddSamples(e.packetpayload);
            }
            else if (wavPlayer2SsrcID == 0)
            {
                Console.WriteLine("Received Packet for Stream 1 with new SSRC Id: " + e.ssrcID + " mark this call to see if they received the audio for this one");
                wavPlayer2.AddSamples(e.packetpayload);
                wavPlayer2SsrcID = e.ssrcID;
            }
            else if (wavPlayer2SsrcID == e.ssrcID)
            {
                wavPlayer2.AddSamples(e.packetpayload);

            }



        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopListening();

            if (chkRecord.Checked)
            {
                CreateRecordingFile();
            }
            else
            {




            }


        }

        private void CreateRecordingFile()
        {
            SaveFileDialog RecordFile = new SaveFileDialog();
            RecordFile.Filter = "Wav File (*.Wav)|*.wav|All Files (*.*)|*.*";



            if (RecordFile.ShowDialog() == DialogResult.OK)
            {






                var waveFormat = WaveFormat.CreateMuLawFormat(8000, 1);


                // memory stream contains the data in raw format (PCM Format)
                MemoryStream memStream = RTPReceiveXMLMethod.GetMemoryStream();
                memStream.Position = 0;
                var rawSource = new RawSourceWaveStream(memStream, waveFormat);





                using (WaveStream convertedStream = WaveFormatConversionStream.CreatePcmStream(rawSource))
                {




                    WaveFileWriter.CreateWaveFile(RecordFile.FileName, convertedStream);

                }

            }




        }
        private void StopListening()
        {
            if (ConnectedIndex == -1)
            {
                MessageBox.Show("Not connected to a phone. Please select a phone from the dropdown list above, or add one via the settings tab before pressing a key.", "Please connect to a phone", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (phoneConList[ConnectedIndex].listenMode == PhoneListenMode.BasicMode)
            {
                stopBasicListeningMode();

            }


            if (phoneConList[ConnectedIndex].listenMode == PhoneListenMode.AdvancedMode)
            {
                stopAdvancedListenMode();

            }




        }

        private void stopBasicListeningMode()
        {

            phoneConList[ConnectedIndex].listenMode = PhoneListenMode.NotListening;


            try
            {

                RestClientPhoneControl RestClientStopListen = new RestClientPhoneControl(phoneConList[ConnectedIndex].ip, phoneConList[ConnectedIndex].username, phoneConList[ConnectedIndex].password);
                RestClientStopListen.EndPoint = @"http" + isHTTPSConnection() + phoneConList[ConnectedIndex].ip + @"/CGI/Execute";
                RestClientStopListen.Method = HttpVerb.POST;

                string xmlinput = @"XML=<stopMedia>
<mediaStream/>
</stopMedia>";
                RestClientStopListen.PostData = xmlinput;

                Console.WriteLine("XML to the ip address: " + xmlinput + " to this phone: " + phoneConList[ConnectedIndex].ip + " with this username: " + phoneConList[ConnectedIndex].username);



                RestClientStopListen.MakeRequest();

                RTPReceiveXMLMethod.StopClient();




            }
            catch (Exception e)
            {
                // Catastrophic
                Console.WriteLine("Catastrophic Error, client ignored StopClient method.");

            }




        }



        private void cmbListenIP_SelectedIndexChanged(object sender, EventArgs e)
        {
            sipListenerIP = cmbListenIP.Text;
        }

        private void chkAdvancedMode_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkBasicListenMethod_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void chkUseHTTPS_CheckedChanged(object sender, EventArgs e)
        {

        }

        private bool readyToSaveOrRemove()
        {
            if (lstDataBindPhones.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select a Phone before editing/removing it.", "Select a phone from the list below", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            if (phoneConList[lstDataBindPhones.SelectedIndex].connectionState == PhoneConnectionState.Connected)
            {

                MessageBox.Show("Please Disconnect from this phone before editing or removing it. ", "Disconnect before removing phone", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }


            return true;

        }
        private void btnSave_Click(object sender, EventArgs e)
        {

            if (readyToSaveOrRemove())
            {


                int index = lstDataBindPhones.SelectedIndex;
                updatePhoneConListEntry(index);




                saveSettings();

            }
            else
            {
                updateGUIFromPhoneConEntry(ConnectedIndex);
                return;
            }





        }

        private void updatePhoneConListEntry(int index)
        {
            phoneConList[index].password = txtPassword.Text;
            phoneConList[index].username = txtUsername.Text;
            phoneConList[index].ip = txtPhoneIP.Text;
            phoneConList[index].useHTTPS = chkUseHTTPS.Checked;
            phoneConList[index].screenshotDisabled = chkDisableScreenshot.Checked;
            phoneConList[index].screenshotInterval = getTrkValue();
            phoneConList[index].NumSoftKeys = Int32.Parse(cmbNum.Text);
        }



        private void btnResetSettings_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();

        }

        private void btnCancelSend_Click(object sender, EventArgs e)
        {
            if (sendingMsg)
            {

                cancellationTokenSourceSendMsg.Cancel();
            }
            else
            {

            }

        }

        private void lblUpgrade_Click(object sender, EventArgs e)
        {
            InstallTempLicense();
            isLicensed();
        }

        private void lblFeature2_Click(object sender, EventArgs e)
        {

        }

        private void InstallTempLicense()
        {

        }




        private void btnRefresh_Click(object sender, EventArgs e)
        {
            cmbListenIP.Items.Clear();
            getNetworkAdapters();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {

            if (ConnectedIndex == -1)
            {

            }
            else
            {
                MessageBox.Show("Please disconnect before attempting to remove a phone entry.", "Disconnect First", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;



            }
            if (phoneConList.Count != 0)
            {
                phoneConList.RemoveAt(lstDataBindPhones.SelectedIndex);
            };

        }

        private void chkEnableToolTips_CheckedChanged(object sender, EventArgs e)
        {
            ToolTipEnableUpdate();
        }

        private void ToolTipEnableUpdate()
        {
            if (chkEnableToolTips.Checked)
            {
                toolTip1.Active = true;
            }
            else
            {
                toolTip1.Active = false;
            }


        }


        private void FillInToolTips()
        {
            toolTip1.SetToolTip(btnSave, "Select an entry you wish to edit then click save when finished.");
            toolTip1.SetToolTip(btnAdd, "Add a new phone.");
            toolTip1.SetToolTip(trkTimer, "Screenshot refresh (1 to 10 seconds.) 5 second & below recommended for LAN Monochrome phones only, use caution with color screens: BW usage is high!");
            toolTip1.SetToolTip(chkUseHTTPS, "Use HTTPS for connectivity to phone, some phones require this.");
            toolTip1.SetToolTip(txtUsername, "A user configured in CUCM who has the phone listed as a controlled device and is assigned CCM End User Privileges");
            toolTip1.SetToolTip(txtPassword, "The Password of a user configured in CUCM for controlled device of the phone and is assigned CCM End User Privileges");
            toolTip1.SetToolTip(txtPhoneIP, "The IP Address of the phone, be sure you can reach this address when trying to connect");
            toolTip1.SetToolTip(lblIP, "Remember, the phone should be enabled for Web Access in CUCM.");
            toolTip1.SetToolTip(btnSpeaker, "Speakerphone mode");
            toolTip1.SetToolTip(btnMute, "Mute the Line");
            toolTip1.SetToolTip(btnHeadset, "Headset Mode");
            toolTip1.SetToolTip(btnDirectories, "Directory");
            toolTip1.SetToolTip(btnServices, "Services");
            toolTip1.SetToolTip(btnMessages, "Voicemail");
            toolTip1.SetToolTip(btnSettings, "Settings");


        }

        private void lstDataBindPhones_SelectedIndexChanged(object sender, EventArgs e)
        {

            editPhoneDetails(lstDataBindPhones.SelectedIndex);

        }

        private void tabPhone_Click(object sender, EventArgs e)
        {

        }
        private void ResizeSoftkeys(int softkeynum)
        {
            // need this to account for the edges.
            int formLength = this.Width - 26;

            switch (softkeynum)
            {

                case 4:
                    //                    btnKeyPad3.Location = new System.Drawing.Point(btnKeyPad2.Location.X + btnKeyPad1.Width, 295);

                    btnSoft1.Width = formLength / softkeynum;
                    btnSoft1.Location = new System.Drawing.Point(0, 254);
                    btnSoft2.Width = formLength / softkeynum;
                    btnSoft2.Location = new System.Drawing.Point(btnSoft1.Location.X + (formLength / softkeynum), 254);
                    btnSoft3.Location = new System.Drawing.Point(btnSoft2.Location.X + (formLength / softkeynum), 254);
                    btnSoft3.Width = formLength / softkeynum;
                    btnSoft4.Location = new System.Drawing.Point(btnSoft3.Location.X + (formLength / softkeynum), 254);
                    btnSoft4.Width = formLength / softkeynum;
                    btnSoft5.Visible = false;
                    break;

                case 5:
                    btnSoft1.Width = formLength / softkeynum;
                    //      btnSoft1.Location = new System.Drawing.Point(0, 254);
                    btnSoft2.Location = new System.Drawing.Point(btnSoft1.Location.X + (formLength / softkeynum), 254);

                    btnSoft2.Width = formLength / softkeynum;

                    btnSoft3.Location = new System.Drawing.Point(btnSoft2.Location.X + (formLength / softkeynum), 254);
                    btnSoft3.Width = formLength / softkeynum;
                    btnSoft4.Location = new System.Drawing.Point(btnSoft3.Location.X + (formLength / softkeynum), 254);
                    btnSoft4.Width = formLength / softkeynum;
                    btnSoft4.Location = new System.Drawing.Point(btnSoft3.Location.X + (formLength / softkeynum), 254);
                    btnSoft4.Width = formLength / softkeynum;
                    btnSoft5.Visible = true;
                    btnSoft5.Location = new System.Drawing.Point(btnSoft4.Location.X + (formLength / softkeynum), 254);
                    btnSoft5.Width = formLength / softkeynum;

                    break;


            }

        }


        private void chkPrintHeader_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkRecord_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRecord.Checked)
            {
                chkStartPlaying.Checked = false;
            }
            else
            {
                chkStartPlaying.Checked = true;
            }

        }

        private void frmMain_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (tabPhone.Focused)
            {
                tabPhone_KeyPress(sender, e);
            }
        }



        public string cmbInformationTextString { get; set; }

        private void cmbPhone_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private void chkStartPlaying_CheckedChanged(object sender, EventArgs e)
        {
            if (chkStartPlaying.Checked)
            {
                chkRecord.Checked = false;
            }
            else
            {
                chkRecord.Checked = true;
            }

        }

        private void tabSettings_Click_1(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (lstViewCommands.SelectedIndices.Equals(-1))
            {
                return;
            }
            foreach (ListViewItem cmdToAdd in lstViewCommands.SelectedItems)
            {
                lstCommands.Items.Add(cmdToAdd.Tag);
                lstCommands.Items.Add("Delay " + trkBulkDelay.Value);

            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (lstCommands.SelectedIndices.Equals(-1))
            {
                return;

            }
            using (var ListBox2 = new ListBox())
            {

                for (int x = lstCommands.SelectedIndices.Count - 1; x >= 0; x--)
                {
                    int idx = lstCommands.SelectedIndices[x];
                    ListBox2.Items.Add(lstCommands.Items[idx]);
                    lstCommands.Items.RemoveAt(idx);

                }

            };





        }


        private void btnBulkSendCommand_Click(object sender, EventArgs e)
        {
            sendBulkStart();
        }

        private void btnCancelBulk_Click(object sender, EventArgs e)
        {
            if (sendingBulk)
            {
                cancellationTokenSourceBulkEdit.Cancel();

            }
            else
            {

            }


        }


        private void sendBulkStart()
        {
            if (sendingBulk)
            {

                MessageBox.Show("Please cancel existing Bulk Config before attempting to start another", "Bulk in progress.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (lstCommands.Items.Count.Equals(-1))
            {
                MessageBox.Show("Please Select commands you want to send in bulk first.", "Select Bulk Commands", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (lstMsgReceivers.SelectedItems.Count.Equals(-1))
            {

                MessageBox.Show("No Phones to recieve Bulk Commands selected. Please select phones to recieve the bulk commands.", "Select phones to receive commands.", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;


            }
            else
            {
                prgBulkSend.Visible = true;

                lblBulkStatus.Visible = true;
                sendingBulk = true;
                sendBulkCmdFactory();
                btnCancelBulk.Enabled = true;
            }


        }
        private void stopBulkSend()
        {
            sendingBulk = false;
            prgBulkSend.Value = 0;
            prgBulkSend.Visible = false;

        }
        private async void sendBulkCmdFactory()
        {
            lblBulkStatus.Text = "Sending...";
            prgBulkSend.Visible = true;

            cancellationTokenSourceBulkEdit = new CancellationTokenSource();

            cancellationTokenBulkEdit = cancellationTokenSourceBulkEdit.Token;

            //             CiscoPhoneObject[] msgReceipientPhone =  new CiscoPhoneObject[lstMsgReceivers.SelectedItems.Count];

            CiscoPhoneObject[] bulkReceiptientPhone = getBulkMethodCiscoPhoneObjects(lstBulkPhones);


            // calculate how much to extend the progress bar by.
            int increasevalueby = 10000 / bulkReceiptientPhone.Count();

            foreach (CiscoPhoneObject cmdReceiver in bulkReceiptientPhone)
            {

                foreach (string cmd in lstCommands.Items)
                {
                    if (String.IsNullOrEmpty(cmd))
                    {
                        break;
                    }

                    if (cancellationTokenBulkEdit.IsCancellationRequested)
                    {
                        stopBulkSend();
                        break;
                    }



                    object[] arrObjects = new object[] { cmdReceiver, cmd };
                    using (Task<string> sendBulkTask = new Task<string>(new Func<object, string>(sendBulkEditAction), arrObjects, cancellationTokenBulkEdit))
                    {
                        try
                        {
                            lblBulkStatus.Text = "Sending Command: " + cmd + " to phone: " + cmdReceiver.PhoneDisplayName;
                            sendBulkTask.Start();
                            await sendBulkTask;

                        }
                        catch (CiscoPhoneObjectException E)
                        {
                            System.Diagnostics.Debug.WriteLine("Couldn't Send bulk command to phone " + cmdReceiver.PhoneDisplayName);

                            lstBulkPhones.Items[lstBulkPhones.FindString(cmdReceiver.PhoneDisplayName)] = cmdReceiver.PhoneDisplayName + " (Sending msg Failed)";
                            CiscoPhoneObjectErrorLogging(E);

                        }


                    }


                }
                prgBulkSend.Value += increasevalueby;


            }

            if (cancellationTokenBulkEdit.IsCancellationRequested)
            {
                lblBulkStatus.Text = "Last Bulk Commands Cancelled";
                stopBulkSend();
            }
            else
            {
                lblBulkStatus.Text = "Last Bulk Commands Complete.";
                stopBulkSend();
            }


        }

        private void tabBulk_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void tabAudio_Click(object sender, EventArgs e)
        {

        }
        private void packetReceiveFlashLed()
        {

            this.Invoke((MethodInvoker)delegate
            {
                pctPacketReceived.Image = Properties.Resources.led_green_on;
                tmrRTPIndicatorTracker.Interval = 10;

                if (!tmrRTPIndicatorTracker.Enabled)
                {

                    tmrRTPIndicatorTracker.Enabled = true;
                }



            });





        }

        private void tmrRTPIndicatorTracker_Tick(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {

                pctPacketReceived.Image = Properties.Resources.led_green_off;

            });

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void chkGlobalUserPass_CheckedChanged(object sender, EventArgs e)
        {

            globalUserPass = chkGlobalUserPass.Checked;

            if (globalUserPass)
            {

                txtUsername.Enabled = false;
                txtPassword.Enabled = false;
            }
            else
            {

                txtUsername.Enabled = true;
                txtPassword.Enabled = true;

            }


        }

        private void picNavDwn_DoubleClick(object sender, EventArgs e)
        {

        }

        private void tmrCheckVM_Tick(object sender, EventArgs e)
        {
            try
            {

                if (ConnectedIndex == -1)
                {
                    turnOffMWI();
                    return;
                }

                if (isNull(phoneConList[ConnectedIndex]))
                {
                    turnOffMWI();
                    return;
                }

                if (phoneConList[ConnectedIndex].phoneDeviceInformation.MessageWaiting.Equals("Yes"))
                {
                    turnOnMWI();

                }
                else
                {
                    turnOffMWI();
                }
            }
            catch (Exception E)
            {

                return;
            }
        }

        private void turnOnMWI()
        {
            this.Icon = Properties.Resources.Mwi_Icon;
        }
        private void turnOffMWI()
        {
            this.Icon = Properties.Resources.MainIcon;

        }

        private void checkMWIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ConnectedIndex == -1)
            {
                MessageBox.Show("Please Connect to a phone first.", "Connect to a Phone", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                System.Diagnostics.Debug.WriteLine("Must be connected to a phone first.");
                return;
            }
            if (phoneConList[ConnectedIndex].connectionState == PhoneConnectionState.Connected)
            {
                phoneConList[ConnectedIndex].updatePhoneInformation("DeviceInformationX");
            }

        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Close();

        }

        private void setupWizardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Coming soon!...");
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (ConnectedIndex.Equals(-1))
            {
                MessageBox.Show("Not Currently connected!");
                return;
            }
            {
                if (phoneConList[ConnectedIndex].connectionState == PhoneConnectionState.Connected)
                {
                    Disconnect();

                    return;
                }
            }

        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ConnectedIndex.Equals(-1))
            {


            }
            else
            {
                if (phoneConList[ConnectedIndex].connectionState == PhoneConnectionState.Connected)
                {
                    MessageBox.Show("Please disconnect from the phone first.");
                    return;
                }
            }

            if (!String.IsNullOrEmpty(cmbPhone.Text))
            {
                Connecting();
            }
            else
            {
                MessageBox.Show("Please select a phone before attempting to connect.");

            }

        }
        private Byte[] ToRTPData(Byte[] data, int bitsPerSample, int channels)
        {
            //Neues RTP Packet erstellen
            RTPPacket rtp = ToRTPPacket(data, bitsPerSample, channels);
            //RTPHeader in Bytes erstellen
            Byte[] rtpBytes = rtp.ToBytes();
            //Fertig
            return rtpBytes;
        }
        private RTPPacket ToRTPPacket(Byte[] linearData, int bitsPerSample, int channels)
        {

            Byte[] mulaws = Utils.LinearToMulaw(linearData, bitsPerSample, channels);


            RTPPacket rtp = new RTPPacket();


            //rtp.Data = mulaws;
            //rtp.SourceId = m_SourceId;
            //rtp.CSRCCount = m_CSRCCount;
            //rtp.Extension = m_Extension;
            //rtp.HeaderLength = RTPPacket.MinHeaderLength;
            //rtp.Marker = m_Marker;
            //rtp.Padding = m_Padding;
            //rtp.PayloadType = m_PayloadType;
            //rtp.Version = m_Version;


            //try
            //{
            //    rtp.SequenceNumber = Convert.ToUInt16(m_SequenceNumber);
            //    m_SequenceNumber++;
            //}
            //catch (Exception)
            //{
            //    m_SequenceNumber = 0;
            //}
            //try
            //{
            //    rtp.Timestamp = Convert.ToUInt32(m_TimeStamp);
            //    m_TimeStamp += mulaws.Length;
            //}
            //catch (Exception)
            //{
            //    m_TimeStamp = 0;
            //}


            return rtp;
        }
        private void StartImageWebservers()
        {
            WS = new WebServer(getNewbackground, "http://" + cmbHTTP.Text + ":23124/mainImage/");
            WS2 = new WebServer(getNewbackgroundSmall, "http://" + cmbHTTP.Text + ":23124/previewImage/");

            WS.Run();
            WS2.Run();
        }

        private void StopImageWebservers()
        {
            WS.Stop();
            WS2.Stop();
        }
        private void btnStartReceiving_Click(object sender, EventArgs e)
        {
            

        }
        private Bitmap getNewbackground(HttpListenerRequest request)
        {
            // FIRST CHECK THE MAX RESOLUTION THIS PHONE IS ALLOWED.
            return newBackground;

        }

        private void setRingTone()
        {
            //            < setringtone >
            //< ringtone >
            //http://url.to.file/file.raw
            //</ ringtone >
            //</ setringtone >
            ///
        }
        private void changeBackgroundImageResolution(int width, int height)
        {
            //            Stream ImageMem = new MemoryStream();
            //            Bitmap origImage = new Bitmap(txtImageFileName.Text);
            //            origImage.Save(ImageMem, System.Drawing.Imaging.ImageFormat.Tiff);

            ////            Image clone = new System.Drawing.ImageFormatConverter.StandardValuesCollection

            //            using (Graphics gr = Graphics.FromImage(clone))
            //            {

            //                gr.DrawImage(clone, new Rectangle(0, 0, clone.Width, clone.Height));

            //            }

            //            newBackground = new Bitmap(clone, new Size(width, height));







            //            newBackGroundPreview = ScaleImage.ScaleByPercent(newBackground, 25);





        }

        private void checkResolution()
        {
            /* You send: getDeviceCaps and the resposne will look like:
            < getDeviceCapsResponse >
< physical >
< modelNumber > CP - 8841 </ modelNumber >
< display width = "800" height = "480" bitDepth = "24" isColor = "true" />
       </ physical >
       < services sdkVersion = "8.5.1" >
        < browser >
        
        < acceptLanguage > en_US </ acceptLanguage >
        < acceptCharset > utf - 8,iso - 8859 - 1; q = 0.8 </ acceptCharset >
                  </ browser >
                  </ services >
                  </ getDeviceCapsResponse > */


        }
        public static bool IsUserAdministrator()
        {
            //bool value to hold our return value
            bool isAdmin;
            try
            {
                //get the currently logged in user
                System.Security.Principal.WindowsIdentity user = System.Security.Principal.WindowsIdentity.GetCurrent();
                System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }

        private Bitmap getNewbackgroundSmall(HttpListenerRequest request)
        {
            return newBackGroundPreview;

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click_1(object sender, EventArgs e)
        {
            if (userHasBeenWarned)
            {

            }
            else
            {
                MessageBox.Show("Please make sure you enabled everything required for phone personalization.  Check help menu -> Enable Phone Personlization.", "Confirm Phone Personalization Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
                userHasBeenWarned = true;
            }
            OpenFileDialog BackgroundFile = new OpenFileDialog();
            BackgroundFile.Filter = "BMP File (*.BMP), PNG File (*.PNG)|*.bmp,*.png|All Files (*.*)|*.*";




            if (BackgroundFile.ShowDialog() == DialogResult.OK)
            {

                newBackground = new System.Drawing.Bitmap(BackgroundFile.OpenFile());
                pctBackgroundPreview.Image = newBackground;


            }
        }


        class RoundButton : Button
        {
            protected override void OnResize(EventArgs e)
            {
                using (var path = new GraphicsPath())
                {
                    path.AddEllipse(new Rectangle(2, 2, this.Width - 20, this.Height - 20));
                    this.Region = new Region(path);
                }
                base.OnResize(e);
            }
        }
        public class myButtonObject : UserControl
        {


            public class ShapedButton : Button
            {
                protected override void OnResize(EventArgs e)
                {
                    base.OnResize(e);
                    GraphicsPath gp = new GraphicsPath();
                    gp.AddEllipse(new Rectangle(Point.Empty, this.Size));
                    this.Region = new Region(gp);
                }
            }



            // Draw the new button. 
            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics graphics = e.Graphics;
                Pen myPen = new Pen(Color.Black);
                // Draw the button in the form of a circle
                graphics.DrawEllipse(myPen, 0, 0, 100, 100);
                myPen.Dispose();
            }
        }

        private void tabCustomize_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel3_Click(object sender, EventArgs e)
        {
            
        }

        private void sendBackgroundSetCommand()
        {

            string response = "";
            try
            {


                RestClientPhoneControl RestClientSetBG = new RestClientPhoneControl(phoneConList[ConnectedIndex].ip, phoneConList[ConnectedIndex].username, phoneConList[ConnectedIndex].password);
                RestClientSetBG.EndPoint = @"http" + isHTTPSConnection() + phoneConList[ConnectedIndex].ip + @"/CGI/Execute";
                RestClientSetBG.Method = HttpVerb.POST;

                string xmlinput = @"XML=<setBackground><background><image>http://" + cmbHTTP.Text + @":23124/mainImage/backMain.png</image><icon>http://" + cmbHTTP.Text + @":23124/previewImage/prev2222.png</icon></background></setBackground>";
                
                RestClientSetBG.PostData = xmlinput;

                Console.WriteLine("XML to the ip address: " + xmlinput + " to this phone: " + phoneConList[ConnectedIndex].ip + " with this username: " + phoneConList[ConnectedIndex].username);



               response =  RestClientSetBG.MakeRequest();



            }
            
            catch (WebException WebE)
            {
                MessageBox.Show("The phone background method returned the following error: " + WebE.Message);

                throw new CiscoPhoneObjectException("HTTPConnectFailed", phoneConList[ConnectedIndex], WebE);

                


            }
            catch (Exception E)
            {
                MessageBox.Show("The phone background method returned the following error: " + E.Message);


                throw new CiscoPhoneObjectException("HTTPConnectFailed", phoneConList[ConnectedIndex], E);



            }


        }
        private void btnChooseBackground_Click(object sender, EventArgs e)
        {
            if (pctBackgroundPreview.Image.Size == Properties.Resources.clickherebackground.Size || pctBackPreviewSmall.Image.Size == Properties.Resources.clickherebackgroundPreview.Size)
            {
                MessageBox.Show("Please Select a background and a background preview image first.", "Select images", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            if (String.IsNullOrEmpty(cmbHTTP.Text))
            {
                MessageBox.Show("You must select a listening IP Address from the dropdown.");
                return;

            }
            if (ConnectedIndex == -1)
            {
                MessageBox.Show("Not connected to a phone. Please select a phone from the dropdown list above, or add one via the settings tab before pressing a key.", "Please connect to a phone", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (phoneConList[ConnectedIndex].connectionState == PhoneConnectionState.Connected)
            {

                try
                {
                    if (IsUserAdministrator())
                    {
                        try
                        {

                            StartImageWebservers();
                            sendBackgroundSetCommand();
                        }
                        catch (Exception E)
                        {
                            System.Diagnostics.Debug.WriteLine("Webserver could not start: " + E.Message);
                            Console.WriteLine("Webserver could not start: " + E.Message);
                        }
                        finally {
                            sendBackgroundSetCommand();
                        }
                        


                    }
                    else
                    {
                        MessageBox.Show("Unfortunately setting the background requires this program to run with admin privileges (HTTP Server has to be started so the images can be downloaded to the phone.). Please restart and run as administrator.", "Privileges required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception E)
                {


                }
            } else
            {

                MessageBox.Show("Not connected to a phone. Please select a phone from the dropdown list above, or add one via the settings tab before pressing a key.", "Please connect to a phone", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;

            }
        }

        private void pctBackPreview_Click(object sender, EventArgs e)
        {

            OpenFileDialog BackgroundFile = new OpenFileDialog();
            BackgroundFile.Filter = "BMP File (*.BMP)|*.bmp|PNG File (*.PNG)|*.png|All Files (*.*)|*.*";



            if (BackgroundFile.ShowDialog() == DialogResult.OK)
            {

                newBackGroundPreview = new System.Drawing.Bitmap(BackgroundFile.OpenFile());
                pctBackPreviewSmall.Image = newBackGroundPreview;


            }
        }

        private void cmbHTTP_SelectedIndexChanged(object sender, EventArgs e)
        {
         
        }

        private void cmbHTTP_MouseEnter(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void dataPhoneInfo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            frmMsgView frmMsg = new frmMsgView(@"
7906/7911  - 95x34 large, 23x8 preview
7941/7961  - 320x196 large, 80x49 preview
7942/7962 - 320x196 large, 80x49 preview
7945/7965 - 320x212 large, 80x53 preview
7975 - 320x216 large, 80x54 preview
7985 - 800x600 large 800x600 preview
8941/8945/8961/9951/9971  640x480 large 123x111 preview                
8841/8845/8851/8865 - 800x480 large 159x109 preview
");
            frmMsg.Show();
        }

        private void btnRetrieve_Click(object sender, EventArgs e)
        {
        //    phoneConList[ConnectedIndex].phoneDeviceInformation.
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private async void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string cucmTftpIP = phoneConList[ConnectedIndex].phoneNetworkConfiguration.TFTPServer1;

            string phoneHostname = phoneConList[ConnectedIndex].phoneNetworkConfiguration.HostName;

            if (String.IsNullOrEmpty(cucmTftpIP) || String.IsNullOrEmpty(phoneHostname))
            {

                object[] arrObjects = new object[] { "NetworkConfigurationX" };

                using (Task<string> RetrieveURLInfo = new Task<string>(new Func<object, string>(phoneConList[ConnectedIndex].updatPhoneInformationAsync), arrObjects))
                {
                    try
                    {
                        RetrieveURLInfo.Start();
                        await RetrieveURLInfo;
                         while (String.IsNullOrEmpty(phoneConList[ConnectedIndex].phoneNetworkConfiguration.HostName))
                        {
                            await Task.Delay(200);
                        }

                         cucmTftpIP = phoneConList[ConnectedIndex].phoneNetworkConfiguration.TFTPServer1;

                        phoneHostname = phoneConList[ConnectedIndex].phoneNetworkConfiguration.HostName;

                        string URL = "http://" + cucmTftpIP + ":6970/" + phoneHostname + ".cnf.xml";

                        System.Diagnostics.Process.Start(URL);

                    }
                    catch (Exception E)
                    {

                        System.Diagnostics.Debug.WriteLine("Exception retrieving info.");

                        


                    }


                }
            }
            else
            {

                string URL = "http://" + cucmTftpIP + ":6970/" + phoneHostname + ".cnf.xml";

                System.Diagnostics.Process.Start(URL);
            }

        }

        private void cmbInformation_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void dialCalleridNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void dialInternal7DigitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (ConnectedIndex == -1)
                {
                    return;
                }
                if (phoneConList[ConnectedIndex].connectionState == PhoneConnectionState.Connected)
                {
                    object[] arrObjects = new object[] { phoneConList[ConnectedIndex], txtMessage.Text };

                    dialNumber(phoneConList[ConnectedIndex]);


                }


            }
            catch (CiscoPhoneObjectException CiscoE)
            {
                Console.WriteLine(CiscoE.LastErrorMessage);
                CiscoPhoneObjectErrorLogging(CiscoE);
            }
            catch (Exception E)
            {
                Console.WriteLine("General Exception Detected:");
                Console.WriteLine("Unable to make a keypress request, failed: " + E.Message);


            }

            
        }

        private void mnuMain_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}



    //public static class ScaleImage
    //{


//    //public static Bitmap ScaleByPercent(this Bitmap imgPhoto, int Percent)
//    //{
//    //    float nPercent = ((float)Percent / 100);

//    //    int sourceWidth = imgPhoto.Width;
//    //    int sourceHeight = imgPhoto.Height;
//    //    var destWidth = (int)(sourceWidth * nPercent);
//    //    var destHeight = (int)(sourceHeight * nPercent);

//    //    var bmPhoto = new Bitmap(destWidth, destHeight,
//    //                             System.Drawing.Imaging.PixelFormat.Format24bppRgb);
//    //    bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
//    //                          imgPhoto.VerticalResolution);

//    //    Graphics grPhoto = Graphics.FromImage(bmPhoto);
//    //    grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

//    //    grPhoto.DrawImage(imgPhoto,
//    //                      new Rectangle(0, 0, destWidth, destHeight),
//    //                      new Rectangle(0, 0, sourceWidth, sourceHeight),
//    //                      GraphicsUnit.Pixel);

//    //    grPhoto.Dispose();
//    //    return bmPhoto;
//    //}
//    //}
//}
