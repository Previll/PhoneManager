using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;




namespace ccierants.baseclasses
{
    // inspired by: https://developer.cisco.com/site/sxml/documents/api-reference/risport/#selectcmdevice-status-reason-codes

    public struct RISPhoneInfo
    {
        public string DeviceName;
        public string DirNumber;

        public enum Status{ Registered, Unregistered, Unknown, Any, PartiallyRegistered, Rejected };

        public int RegistrationAttempts;

        public string LoginUserId; // extension mobility

        public string Description;

        public string TimeStamp;

        public enum DownloadStatus { Any, Upgrading, Successful, Failed, Unknown };

        public string DownloadFailureReason;

        public string DownloadServer;

        public string IP;

        /// TODO:  Create an enum for teh STatusReason 
        /// 
    }

    public struct PhonePortInformation {


        public string RxtotalPkt;
        public string RxcrcErr;
        public string RxalignErr;
        public string Rxmulticast;
        public string Rxbroadcast;
        public string Rxunicast;
        public string RxshortErr;
        public string RxshortGood;
        public string RxlongGood;
        public string RxlongErr;
        public string Rxsize64;
        public string Rxsize65to127;
        public string Rxsize128to255;
        public string Rxsize256to511;
        public string Rxsize512to1023;
        public string Rxsize1024to1518;
        public string RxtokenDrop;
        public string TxexcessDefer;
        public string TxlateCollision;
        public string TxtotalGoodPkt;
        public string Txcollisions;
        public string TxexcessLength;
        public string Txbroadcast;
        public string Txmulticast;
        public string lldpFramesOutTotal;
        public string lldpAgeoutsTotal;
        public string lldpFramesDiscardedTotal;
        public string lldpFramesInErrorsTotal;
        public string lldpFramesInTotal;
        public string lldpTLVDiscardedTotal;
        public string lldpTLVUnrecognizedTotal;
        public string CDPNeighborDeviceId;
        public string CDPNeighborIP;
        public string CDPNeighborIPv6;
        public string CDPNeighborPort;
        public string LLDPNeighborDeviceId;
        public string LLDPNeighborIP;
        public string LLDPNeighborIPv6;
        public string LLDPNeighborPort;
        public string PortSpeed;
        public string PortInformation;
    
    }
    public struct PhoneDeviceInformation
    {


        public string DeviceInformation;
        public string MACAddress;
        public string HostName;
        public string phoneDN;
        public string appLoadID;
        public string bootLoadID;
        public string versionID;
        public string addonModule1;
        public string addonModule2;
        public string hardwareRevision;
        public string serialNumber;
        public string modelNumber;
        public string MessageWaiting;
        public string udi;
        public string time;
        public string timezone;
        public string date;
        public string systemFreeMemory;
        public string javaHeapFreeMemory;
        public string javaPoolFreeMemory;
        public string fipsModeEnabled;
        

    }
    public struct PhoneDeviceLog
    {
        // Only appears to contain one
        public string status;
    }

    public struct PhoneEthernetInformation
    {


        public string TxFrames;
        public string TxBroadcasts;
        public string TxMulticasts;
        public string TxUnicasts;
        public string RxFrames;
        public string RxMulticasts;
        public string RxBroadcasts;
        public string RxUnicasts;
        public string RxPacketNoDes;                      


    }
    public struct PhoneStreamingStatistics
    {


        public string StreamingStatistics;
        public string RemoteAddr;
        public string LocalAddr;
        public string StartTime;
        public string StreamStatus;
        public string Name;
        public string SenderPackets;
        public string SenderOctets;
        public string SenderCodec;
        public string SenderReportsSent;
        public string SenderReportTimeSent;
        public string RcvrLostPackets;
        public string AvgJitter;
        public string RcvrCodec;
        public string RcvrReportsSent;
        public string RcvrReportTimeSent;
        public string RcvrPackets;
        public string RcvrOctets;
        public string MOSLQK;
        public string AvgMOSLQK;
        public string MinMOSLQK;
        public string MaxMOSLQK;
        public string MOSLQKVersion;
        public string CmltveConcealRatio;
        public string IntervalConcealRatio;
        public string MaxConcealRatio;
        public string ConcealSecs;
        public string SeverelyConcealSecs;
        public string Latency;
        public string MaxJitter;
        public string SenderSize;
        public string SenderReportsReceived;
        public string SenderReportTimeReceived;
        public string RcvrSize;
        public string RcvrDiscarded;
        public string RcvrReportsReceived;
        public string RcvrReportTimeReceived;
        public string Domain;
        public string SenderJoins;
        public string ReceiverJoins;
        public string Byes;
        public string RowStatus;
        public string SenderTool;
        public string SenderReports;
        public string SenderReportTime;
        public string SenderStartTime;
        public string RcvrJitter;
        public string ReceiverTool;
        public string RcvrReports;
        public string RcvrReportTime;
        public string RcvrStartTime;             

    }
    public struct PhoneNetworkConfiguration
    {
     
        public string NetworkConfiguration;
        public string DHCPServer;
        public string BOOTPServer;
        public string MACAddress;
        public string HostName;
        public string DomainName;
        public string IPAddress;
        public string SubNetMask;
        public string TFTPServer1;
        public string DefaultRouter1;
        public string DefaultRouter2;
        public string DefaultRouter3;
        public string DefaultRouter4;
        public string DefaultRouter5;
        public string DNSServer1;
        public string DNSServer2;
        public string DNSServer3;
        public string DNSServer4;
        public string DNSServer5;
        public string VLANId;
        public string AdminVLANId;
        public string CallManager1;
        public string CallManager2;
        public string CallManager3;
        public string CallManager4;
        public string CallManager5;
        public string InformationURL;
        public string DirectoriesURL;
        public string MessagesURL;
        public string ServicesURL;
        public string DHCPEnabled;
        public string DHCPAddressReleased;
        public string AltTFTP;
        public string ForwardingDelay;
        public string IdleURL;
        public string IdleURLTime;
        public string ProxyServerURL;
        public string AuthenticationURL;
        public string SWPortCfg;
        public string PCPortCfg;
        public string TFTPServer2;
        public string UserLocale;
        public string NetworkLocale;
        public string HeadsetEnable;
        public string UserLocaleVersion;
        public string NetworkLocaleVersion;
        public string PCPortDisable;
        public string SpeakerEnable;
        public string GARP;
        public string VideoCapability;
        public string VoiceVlanAccess;
        public string AutoSelectLine;
        public string DscpForCallControl;
        public string DscpForConfiguration;
        public string DscpForServices;
        public string SecurityMode;
        public string WebAccess;
        public string SpanPcPort;
        public string PCVlan;
        public string CDPPCPort;
        public string CDPSWPort;
        public string LLDPSWPort;
        public string LLDPPCPort;
        public string lldpPowerPriority;
        public string lldpAssetId;
        public string portAutoLinkSync;
        public string swPortRemoteConfig;
        public string pcPortRemoteConfig;
        public string IPAddressingMode;
        public string IPPreferenceMode;
        public string AutoIPConfig;
        public string IPv6LoadServer;
        public string IPv6LogServer;
        public string IPv6CAPFServer;
        public string IPv6DHCPEnabled;
        public string IPv6HostIPAddress;
        public string IPv6PrefixLength;
        public string IPv6DefaultGateway1;
        public string IPv6DNSServer1;
        public string IPv6DNSServer2;
        public string IPv6DHCPAddressReleased;
        public string IPv6AlternateTFTP;
        public string IPv6TFTPAddress1;
        public string IPv6TFTPAddress2;
        public string SSHAccess;
        public string EnergywiseDomain;
        public string EnergywisePowerLevel;



    }
    public struct AXLSettings
    {
        public bool settingsentered;

        public bool settingsverified;

        public string cucmusername;

        public string cucmpassword;

        public string httpaddress;

        public string version;

        public string ipaddress;
    
    }

    public struct RESTSettings
    {

        public bool settingsentered;

        public bool settingsverified;

        public string username;

        public string password;


        public string httpaddress;

        public string version;

    }

}
