using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace QuickAndSimpleSIPTrunkConfigGenerator
{
    class AXLConnector
    {
        private string cucmip;
        private string cucmusername;
        private string cucmpassword;


        public AXLConnector(string defaultip, string defaultuser, string defaultpass)
        {

            cucmip = defaultip;
            cucmusername = defaultuser;
            cucmpassword = defaultpass;


        }
        private string getSoapHeader()
        {

            // This returns the standard SOAP header 

            string soapheader;
            
            soapheader = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ns=\"http://www.cisco.com/AXL/API/10.5\">";

            soapheader += "<soapenv:Header/>";
            soapheader += "<soapenv:Body>";



            Debug.WriteLine("Soap Header: " + soapheader);


            return soapheader;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="NamedProperlyRegion">Region name should be formatted exactly as you require. </param>
        public void addRegion(string NamedProperlyRegion)
        {


            /*
             * <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ns="http://www.cisco.com/AXL/API/10.5">
       <soapenv:Header/>
       <soapenv:Body>
          <ns:addRegion sequence="?">
             <region>
                <name>TEST-REG</name>
            
            
                <defaultCodec>G.711</defaultCodec>
             </region>
          </ns:addRegion>
       </soapenv:Body>
    </soapenv:Envelope>
             * */

            

            string soapreq;



            soapreq = this.getSoapHeader();

            soapreq += "<ns:addRegion sequence=\"?\"><region><name>" + NamedProperlyRegion + @"</name>
<defaultCodec>G.711</defaultCodec>
      </region>
          </ns:addRegion>
";

            soapreq += this.getSoapFooter();


            this.sendAXLCommand(soapreq);

                                            

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Region name should be formatted exactly as you require. </param>
        public void addLocation(string name)
        {


        

            string soapreq;



            soapreq = this.getSoapHeader();

            soapreq += " <ns:addLocation><location><name>" + name + @"</name>
</location>
</ns:addLocation>";

            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP request = " + soapreq);

            this.sendAXLCommand(soapreq);

                                            

        }


        public void addPartition(string name)
        {



            /*
    <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ns="http://www.cisco.com/AXL/API/10.5">
   <soapenv:Header/>
   <soapenv:Body>
      <ns:addRoutePartition sequence="?">
         <routePartition>
            <name>?</name>
            <!--Optional:-->
            <description>?</description>
           
         </routePartition>
      </ns:addRoutePartition>
   </soapenv:Body>
</soapenv:Envelope>
             * */



            string soapreq;



            soapreq = this.getSoapHeader();

            soapreq += " <ns:addRoutePartition  sequence=\"?\"><routePartition><name>" + name + "</name>";

            soapreq += "<description>" + name + @" Local PT</description>
           
         </routePartition>
      </ns:addRoutePartition>";


            

            soapreq += this.getSoapFooter();
      

            Debug.WriteLine("Full SOAP request for partition = " + soapreq);
            this.sendAXLCommand(soapreq);



           





        }



        public void addDevicePool(string name)
        {

            /* <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ns="http://www.cisco.com/AXL/API/10.5">
   <soapenv:Header/>
   <soapenv:Body>
      <ns:addDevicePool sequence="?">
         <devicePool>
            <name>SAKER-NJ-Store622-DP</name>
               <dateTimeSettingName>EDT</dateTimeSettingName>
               <callManagerGroupName>CMGroup-StoreGroup1</callManagerGroupName>
               <mediaResourceListName>TEST-Store333-MRGL</mediaResourceListName>
               <regionName>SAKER-NJ-654-REG</regionName>
               <networkLocale/>
               <srstName>SA-SRST</srstName>
               <locationName>test-111-LOC</locationName>
               <localRouteGroup>
                  <name>Standard Local Route Group</name>
                  <value>SAKER-NJ-Store613-RG</value>
               </localRouteGroup>
         </devicePool>
      </ns:addDevicePool>
   </soapenv:Body>
</soapenv:Envelope>
</soapenv:Envelope>
             * */


            string soapreq;



            soapreq = this.getSoapHeader();

            soapreq += " <ns:addDevicePool  sequence=\"?\"><devicePool><name>" + name + @"-DP</name>
<dateTimeSettingName>EDT</dateTimeSettingName>
               <callManagerGroupName>CMGroup-StoreGroup1</callManagerGroupName>
               <mediaResourceListName>" + name + @"-MRGL</mediaResourceListName>
               <regionName>" + name + @"-R</regionName>
               <networkLocale/>
               <localRouteGroupName>" + name + @"-RG</localRouteGroupName>
               <srstName>" + name + @"-SRST</srstName>
               <locationName>" + name + @"-L</locationName>
                 <localRouteGroup>
                   <name>Standard Local Route Group</name>
                   <value>" + name + @"-RG</value>
                 </localRouteGroup>
               </devicePool>
               </ns:addDevicePool>
                


";



            soapreq += this.getSoapFooter();


            Debug.WriteLine("Full SOAP request for DP = " + soapreq);
            this.sendAXLCommand(soapreq);


        }

        public void updateRouteGroup(string name)
        {


            /*
             * <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ns="http://www.cisco.com/AXL/API/10.5">
   <soapenv:Header/>
   <soapenv:Body>
      <ns:updateRouteGroup>
         <!--You have a CHOICE of the next 2 items at this level-->
         <name>?</name>
         <members>
            <!--1 or more repetitions:-->
            <member>
               <deviceSelectionOrder>1</deviceSelectionOrder>
               <deviceName>" + na</deviceName>
               <port></port>
            </member>
             *      <member>
                     <deviceSelectionOrder>2</deviceSelectionOrder>
                     
                     <deviceName>WFC-NJ-NAC-PSTN</deviceName>
                     <port>0</port>
                  </member>
         </members>
        </ns:updateRouteGroup>
   </soapenv:Body>
</soapenv:Envelope>*/


            string soapreq;



            soapreq = this.getSoapHeader();

            soapreq += @"<ns:updateRouteGroup>
        <name>" + name + @"-RG</name>

 <members>
                <member>
                    <deviceSelectionOrder>1</deviceSelectionOrder>
                    <deviceName>" + name + @"-SIP</deviceName>
                    <port>0</port>
                </member>
               
               <member>
                    <deviceSelectionOrder>2</deviceSelectionOrder>
                    <deviceName>WFC-NJ-NAC-PSTN</deviceName>
                    <port>0</port>
                </member>
</members>
      </ns:updateRouteGroup>
";

            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP request for Route Group Update = " + soapreq);

            this.sendAXLCommand(soapreq);


        }


        public void updateMRGL(string name, string MRGLname)
        {


      /*      <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ns="http://www.cisco.com/AXL/API/10.5">
   <soapenv:Header/>
   <soapenv:Body>
      <ns:updateMediaResourceList sequence="?">
   
         <addMembers>
            <!--1 or more repetitions:-->
            <member>
               <mediaResourceGroupName uuid="?">?</mediaResourceGroupName>
               <order>?</order>
            </member>
         </addMembers>
      
      </ns:updateMediaResourceList>
   </soapenv:Body>
</soapenv:Envelope>*/


            string soapreq;



            soapreq = this.getSoapHeader();

            soapreq += @"<ns:updateMediaResourceList>
         <name>" + MRGLname + @"</name>
         <members>

               <member>
                 <mediaResourceGroupName>" + name + @"-SW-GW</mediaResourceGroupName>
               <order>1</order>
                </member>
               <member>
                 <mediaResourceGroupName>" + name + @"-HW-GW-MRG</mediaResourceGroupName>
               <order>2</order>
                </member>
               <member>
                 <mediaResourceGroupName>uccmsrv02-MRG</mediaResourceGroupName>
               <order>3</order>
                </member>
               <member>
                 <mediaResourceGroupName>uccmsrv04-MRG</mediaResourceGroupName>
               <order>4</order>
                </member>
                <member>
                 <mediaResourceGroupName>uccmsrv03-MRG</mediaResourceGroupName>
               <order>5</order>
                </member>
                <member>
                 <mediaResourceGroupName>uccmsrv01-MRG</mediaResourceGroupName>
               <order>6</order>
                </member>
               
               

</members>

      </ns:updateMediaResourceList>
";

            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP request for MRGL Update = " + soapreq);


            this.sendAXLCommand(soapreq);





        }
        public void addRouteGroup(string name)
        {

            /*
             * <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/">
   <soapenv:Body>
      <ns:getRouteGroupResponse xmlns:ns="http://www.cisco.com/AXL/API/10.5">
         <return>
            <routeGroup uuid="{A7D4DDE7-CF24-CAFC-8143-E72B8030F11A}">
               <dialPlanWizardGenld/>
               <distributionAlgorithm>Top Down</distributionAlgorithm>
               <members>
                  <member uuid="{0E272FDF-5AA7-6C5A-BF3C-2C790B573AE0}">
                     <deviceSelectionOrder>1</deviceSelectionOrder>
                     <dialPlanWizardGenId/>
                     <deviceName uuid="{0D3C3D0B-314D-E284-F9C7-9EA7BC06E485}">WFC-NJ-NAC-PSTN</deviceName>
                     <port>0</port>
                  </member>
               </members>
               <name>SAKER-NJ-Store613-RG</name>
            </routeGroup>
         </return>
      </ns:getRouteGroupResponse>
   </soapenv:Body>
</soapenv:Envelope> 
             * 
             * 
             * 
             * 
             * <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ns="http://www.cisco.com/AXL/API/10.5">
   <soapenv:Header/>
   <soapenv:Body>
      <ns:addRouteGroup sequence="?">
         <routeGroup>
            <distributionAlgorithm>Circular</distributionAlgorithm>
            <members>
               <!--1 or more repetitions:-->
               <member>
                  <deviceSelectionOrder>?</deviceSelectionOrder>
                  <deviceName uuid="?">?</deviceName>
                  <port>?</port>
               </member>
            </members>
            <name>?</name>
         </routeGroup>
      </ns:addRouteGroup>
   </soapenv:Body>
</soapenv:Envelope>
             * 
             * 
             */




            string soapreq;



            soapreq = this.getSoapHeader();

            soapreq += @"<ns:addRouteGroup>
         <routeGroup><distributionAlgorithm>Top Down</distributionAlgorithm>

 <members>
               
               <member>
                    <deviceSelectionOrder>1</deviceSelectionOrder>
                    <deviceName>WFC-NJ-NAC-PSTN</deviceName>
                    <port>0</port>
                </member>
</members>
<name>" + name + @"</name>
  </routeGroup>
      </ns:addRouteGroup>
";

            soapreq += this.getSoapFooter();


            this.sendAXLCommand(soapreq);


            // don't forget to add the NAC backup.
        }

        public void addMRGL(string name)
        {

            /*
             * 
             * <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/">
   <soapenv:Body>
      <ns:getMediaResourceListResponse xmlns:ns="http://www.cisco.com/AXL/API/10.5">
         <return>
            <mediaResourceList uuid="{E8514F3E-CA0A-584D-2552-74D5BF1C70E8}">
               <name>Software</name>
               <clause>uccmsrv02-MRG:uccmsrv04-MRG:uccmsrv03-MRG:uccmsrv01-MRG</clause>
               <members>
                  <member uuid="{77FB91AE-1CA2-DCAC-C55E-236D70EC7608}">
                     <mediaResourceGroupName uuid="{CFA1307A-2EDA-A91C-80B5-326D005E3A91}">uccmsrv02-MRG</mediaResourceGroupName>
                     <order>0</order>
                  </member>
                  <member uuid="{3E42DED6-7733-8602-BB10-0E7D738F85EA}">
                     <mediaResourceGroupName uuid="{529B9EED-02C1-2612-1F0A-41DE7C0A8A92}">uccmsrv04-MRG</mediaResourceGroupName>
                     <order>1</order>
                  </member>
                  <member uuid="{13E310F6-5C8C-7485-5632-1F1FFBB36F56}">
                     <mediaResourceGroupName uuid="{4CB97A7E-660C-3194-0DB1-65FCE3BA4830}">uccmsrv03-MRG</mediaResourceGroupName>
                     <order>2</order>
                  </member>
                  <member uuid="{89167CD6-7618-8235-6B0B-225F088582AC}">
                     <mediaResourceGroupName uuid="{2F3E4333-6D1C-9B00-F7EA-E05638DAAC56}">uccmsrv01-MRG</mediaResourceGroupName>
                     <order>3</order>
                  </member>
               </members>
            </mediaResourceList>
         </return>
      </ns:getMediaResourceListResponse>
   </soapenv:Body>
</soapenv:Envelope>
             * /
             * <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ns="http://www.cisco.com/AXL/API/10.5">
   <soapenv:Header/>
   <soapenv:Body>
      <ns:addMediaResourceList sequence="?">
         <mediaResourceList>
            <name>?</name>
            <members>
               <!--1 or more repetitions:-->
               <member>
                  <mediaResourceGroupName uuid="?">?</mediaResourceGroupName>
                  <order>?</order>
               </member>
            </members>
         </mediaResourceList>
      </ns:addMediaResourceList>
   </soapenv:Body>
</soapenv:Envelope>
             * 
             * 
             */



            string soapreq;



            soapreq = this.getSoapHeader();

            soapreq += "  <ns:addMediaResourceList sequence=\"?\"><mediaResourceList><name>" + name + @"</name>
<members>
 
                  <member>
                     <mediaResourceGroupName>uccmsrv02-MRG</mediaResourceGroupName>
                     <order>0</order>
                  </member>
                  <member>
                     <mediaResourceGroupName>uccmsrv04-MRG</mediaResourceGroupName>
                     <order>1</order>
                  </member>
                  <member>
                     <mediaResourceGroupName>uccmsrv03-MRG</mediaResourceGroupName>
                     <order>2</order>
                  </member>
                  <member>
                     <mediaResourceGroupName>uccmsrv01-MRG</mediaResourceGroupName>
                     <order>3</order>
                  </member>
         


      </members>
         </mediaResourceList>
      </ns:addMediaResourceList>
";

            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP request for MRGL is: " + soapreq);

            this.sendAXLCommand(soapreq);






        }


        private void addMediaResources(string name)
        {


        }
        public void resetSIPTrunk(string name)
        {
            /*
                 <ns:resetSipTrunk sequence="?">
         <!--You have a CHOICE of the next 2 items at this level-->
         <name>?</name>
         <uuid>?</uuid>
      </ns:resetSipTrunk>


             */




            string soapreq;



            soapreq = this.getSoapHeader();

            soapreq += @" <ns:resetSipTrunk>
             <name>" + name + @"</name>
            </ns:resetSipTrunk>";

            soapreq += this.getSoapFooter();

            Debug.WriteLine("SIP reset SOAP call: " + soapreq);


            this.sendAXLCommand(soapreq);

        }
        public void addSipTrunk(string name, string gateway1, string gatewayhsrp, string callerid)
        {
            /*
               <ns:addSipTrunk>
       
     <sipTrunk>
               <name>def-Store456-SIP1</name>
               <description/>
               <product>SIP Trunk</product>
               <model>SIP Trunk</model>
               <class>Trunk</class>
               <protocol>SIP</protocol>
               <protocolSide>Network</protocolSide>
               <callingSearchSpaceName>def-Store456-CSS-Internal</callingSearchSpaceName>
               <devicePoolName>def-Store456-DP</devicePoolName>
               <locationName>def-Store456-L</locationName>
               <mediaResourceListName>def-Store456-MRGL</mediaResourceListName>
               <retryVideoCallAsAudio>true</retryVideoCallAsAudio>
               <securityProfileName>Non Secure SIP Trunk Profile</securityProfileName>
               <sipProfileName>Standard SIP Profile</sipProfileName>
               <subscribeCallingSearchSpaceName>def-Store456-CSS-Internal</subscribeCallingSearchSpaceName>
               <rerouteCallingSearchSpaceName>def-Store456-CSS-Internal</rerouteCallingSearchSpaceName>
               <referCallingSearchSpaceName>def-Store456-CSS-Internal</referCallingSearchSpaceName>
               <mtpRequired>true</mtpRequired>
               <destAddrIsSrv>false</destAddrIsSrv>
               <tkSipCodec>711ulaw</tkSipCodec>
               
               <acceptInboundRdnis>true</acceptInboundRdnis>
               <acceptOutboundRdnis>true</acceptOutboundRdnis>
               <isPaiEnabled>true</isPaiEnabled>
               <sipPrivacy>Default</sipPrivacy>
               <isRpidEnabled>true</isRpidEnabled>
               <sipAssertedType>Default</sipAssertedType>
               <dtmfSignalingMethod>No Preference</dtmfSignalingMethod>
               <routeClassSignalling>Default</routeClassSignalling>
               <sipTrunkType>None(Default)</sipTrunkType>
               <pstnAccess>true</pstnAccess>
               <runOnEveryNode>true</runOnEveryNode>
               <destinations>
                  <destination>
                     <addressIpv4>10.221.183.21</addressIpv4>
                     <addressIpv6/>
                     <port>5060</port>
                     <sortOrder>1</sortOrder>
                  </destination>
                  <destination>
                     <addressIpv4>10.221.183.252</addressIpv4>
                     <addressIpv6/>
                     <port>5060</port>
                     <sortOrder>2</sortOrder>
                  </destination>
               </destinations>
               
            </sipTrunk>
           
         
      </ns:addSipTrunk>

        */




            string soapreq;



            soapreq = this.getSoapHeader();

            soapreq += @"

<ns:addSipTrunk>
       
     <sipTrunk>
               <name>" + name + @"-SIP</name>
               <description>" + name + @" SIP Trunk via AVPN</description>
               <product>SIP Trunk</product>
               <model>SIP Trunk</model>
               <class>Trunk</class>
               <protocol>SIP</protocol>
               <protocolSide>Network</protocolSide>
               <callingSearchSpaceName>" + name + @"-CSS-Internal</callingSearchSpaceName>
               <devicePoolName>" + name + @"-DP</devicePoolName>
               <locationName>" + name + @"-L</locationName>
               <mediaResourceListName>" + name + @"-MRGL</mediaResourceListName>
               <retryVideoCallAsAudio>true</retryVideoCallAsAudio>
               <securityProfileName>Non Secure SIP Trunk Profile</securityProfileName>
               <sipProfileName>Standard SIP Profile</sipProfileName>
               <subscribeCallingSearchSpaceName>" + name + @"-CSS-Internal</subscribeCallingSearchSpaceName>
               <rerouteCallingSearchSpaceName>" + name + @"-CSS-Internal</rerouteCallingSearchSpaceName>
               <referCallingSearchSpaceName>" + name + @"-CSS-Internal</referCallingSearchSpaceName>
               <mtpRequired>true</mtpRequired>
               <destAddrIsSrv>false</destAddrIsSrv>
               <tkSipCodec>711ulaw</tkSipCodec>";

            if (callerid.Equals("")) 
            {

                  

            }
            else
            {
                soapreq += "<callerIdDn>" + callerid + "</callerIdDn>";
            }
               
            soapreq += @"   <acceptInboundRdnis>true</acceptInboundRdnis>
               <acceptOutboundRdnis>true</acceptOutboundRdnis>
               <isPaiEnabled>true</isPaiEnabled>
               <sipPrivacy>Default</sipPrivacy>
               <isRpidEnabled>true</isRpidEnabled>
               <sipAssertedType>Default</sipAssertedType>
               <dtmfSignalingMethod>No Preference</dtmfSignalingMethod>
               <routeClassSignalling>Default</routeClassSignalling>
               <sipTrunkType>None(Default)</sipTrunkType>
               <pstnAccess>true</pstnAccess>
               <runOnEveryNode>true</runOnEveryNode>
               <destinations>
                  <destination>
                     <addressIpv4>" + gateway1 + @"</addressIpv4>
                     <addressIpv6/>
                     <port>5060</port>
                     <sortOrder>1</sortOrder>
                  </destination>
                  <destination>
                     <addressIpv4>" + gatewayhsrp + @"</addressIpv4>
                     <addressIpv6/>
                     <port>5060</port>
                     <sortOrder>2</sortOrder>
                  </destination>
               </destinations>
               
            </sipTrunk>
           
         
      </ns:addSipTrunk>";

            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP request for SIP Trunk is: " + soapreq);

            this.sendAXLCommand(soapreq);
            



            // Don't forget to make sure it matches the settings of a "good sip trunk"
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameprefix">Name Format should be: CHAIN-STATE-Store$num, for example: SAKER-NJ-Store613</param>
        public void addInternationalLDCSS(string nameprefix)
        {
            #region howtocodetheCSSMsgs
            /*
             * <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ns="http://www.cisco.com/AXL/API/10.5">
   <soapenv:Header/>
   <soapenv:Body>
      <ns:getCss sequence="?">
         <!--You have a CHOICE of the next 2 items at this level-->
         <name>SAKER-NJ-Store613-CSS-Intl</name>
         <returnedTags uuid="?">
            <!--Optional:-->
            <description></description>
            <!--Optional:--> 
            <clause></clause>
            <!--Optional:-->
            <dialPlanWizardGenId></dialPlanWizardGenId>
            <!--Optional:-->
            <members>
               <!--Zero or more repetitions:-->
               <member uuid="?">
                  <!--Optional:-->
                  <routePartitionName uuid="?"></routePartitionName>
                  <!--Optional:-->
                  <index></index>
               </member>
            </members>
            <!--Optional:-->
            <partitionUsage></partitionUsage>
            <!--Optional:-->
            <name></name>
         </returnedTags>
      </ns:getCss>
   </soapenv:Body>
</soapenv:Envelope>
             * 
             * 
             * 
             * Here is an international one:
             * 
             * description>SAKER-NJ-Store613-CSS-Intl</description>
               <clause>WF-LD:WF-E911:WF-Intl:SAKER-NJ-Store613-Local:WF-Local:WF-Service:WF-TollFree:LINE</clause>
               <dialPlanWizardGenId/>
               <members>
                  <member uuid="{04878253-9D93-4B8F-F21C-B23167D4470F}">
                     <routePartitionName uuid="{47A5937B-9E8E-E316-8AE8-5B888F050C3C}">WF-LD</routePartitionName>
                     <index>1</index>
                  </member>
                  <member uuid="{68BD9301-F389-E022-DB79-3BD924F16AD6}">
                     <routePartitionName uuid="{4A9B0ABF-666D-24E3-2599-073B4E685EB2}">WF-E911</routePartitionName>
                     <index>2</index>
                  </member>
                  <member uuid="{31AEB13E-94F4-A0C0-752C-1682F5CC35D6}">
                     <routePartitionName uuid="{6A5449BE-AE4E-227E-E101-E12BDB8B8475}">WF-Intl</routePartitionName>
                     <index>3</index>
                  </member>
                  <member uuid="{1B0031D3-1412-71A0-21FE-24558D5DE08B}">
                     <routePartitionName uuid="{63FEDA2C-3F0C-4B07-D775-34950C1BDA84}">SAKER-NJ-Store613-Local</routePartitionName>
                     <index>4</index>
                  </member>
                  <member uuid="{20ECF168-4F40-9E13-1573-B11151B8F3A2}">
                     <routePartitionName uuid="{C982C948-2401-0167-B6F5-824D1EC14F61}">WF-Local</routePartitionName>
                     <index>5</index>
                  </member>
                  <member uuid="{B3460AB7-5F4D-00D9-3393-CB3CE9F6588E}">
                     <routePartitionName uuid="{BB1CAF55-315C-C5A9-4937-E8261C615C78}">WF-Service</routePartitionName>
                     <index>6</index>
                  </member>
                  <member uuid="{B91B59BF-ACEA-DA25-A5E6-A0A44ED8EADF}">
                     <routePartitionName uuid="{2E6807D4-F5CF-37B7-F574-BE10206EBA99}">WF-TollFree</routePartitionName>
                     <index>7</index>
                  </member>
                  <member uuid="{ED61B8ED-C03C-E610-1B97-794DE48FC358}">
                     <routePartitionName uuid="{822DEDE9-7D38-FCF5-D1BB-46EA201B79BC}">LINE</routePartitionName>
                     <index>8</index>
                  </member>
               </members>
               <partitionUsage>General</partitionUsage>
               <name>SAKER-NJ-Store613-CSS-Intl</name>
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * */
            #endregion

            
            string soapreq;

            // add the International CSS first....

            soapreq = this.getSoapHeader();

            soapreq += " <ns:addCss><css>";
            soapreq += "<description>" + nameprefix + @" International  CSS</description>";
            soapreq += "<members>";
            soapreq += @"<member>

                     <routePartitionName>WF-LD</routePartitionName>
                     <index>1</index>
                  </member>
                  <member>
                     <routePartitionName>WF-E911</routePartitionName>
                     <index>2</index>
                  </member>
                  <member>
                     <routePartitionName>WF-Intl</routePartitionName>
                     <index>3</index>
                  </member>
                  <member>
                     <routePartitionName>" + nameprefix + @"-Local</routePartitionName>
                     <index>4</index>
                  </member>
                  <member>
                     <routePartitionName>" + nameprefix + @"-Internal</routePartitionName>
                     <index>5</index>
                  </member>
                  <member>
                     <routePartitionName>WF-Local</routePartitionName>
                     <index>6</index>
                  </member>
                  <member>
                     <routePartitionName>WF-Service</routePartitionName>
                     <index>7</index>
                  </member>
                  <member>
                     <routePartitionName>WF-TollFree</routePartitionName>
                     <index>8</index>
                  </member>
                  <member>
                     <routePartitionName>LINE</routePartitionName>
                     <index>9</index>
                  </member>";


            soapreq += "</members><partitionUsage>General</partitionUsage>";
            soapreq += "<name>" + nameprefix + "-CSS-Intl</name>";

            soapreq += "</css></ns:addCss>";


            soapreq += this.getSoapFooter();


            Debug.WriteLine("Full SOAP request for CSS International = " + soapreq);

            this.sendAXLCommand(soapreq);

            System.Threading.Thread.Sleep(2000);


            // Then long distance...

            soapreq = this.getSoapHeader();
            soapreq += " <ns:addCss><css>";
            soapreq += "<description>" + nameprefix + @" Long Distance  CSS</description>";
            soapreq += "<members>";
            soapreq += @"<member>

                     <routePartitionName>WF-LD</routePartitionName>
                     <index>1</index>
                  </member>
                  <member>
                     <routePartitionName>WF-E911</routePartitionName>
                     <index>2</index>
                  </member>
                  <member>
                     <routePartitionName>" + nameprefix + @"-Local</routePartitionName>
                     <index>3</index>
                  </member>
                  <member>
                     <routePartitionName>WF-Local</routePartitionName>
                     <index>4</index>
                  </member>
                  <member>
                     <routePartitionName>WF-Service</routePartitionName>
                     <index>5</index>
                  </member>
                  <member>
                     <routePartitionName>WF-TollFree</routePartitionName>
                     <index>6</index>
                  </member>
                  <member>
                     <routePartitionName>LINE</routePartitionName>
                     <index>7</index>
                  </member>
                 <member>
                     <routePartitionName>" + nameprefix + @"-Internal</routePartitionName>
                     <index>8</index>
                  </member>
";


            soapreq += "</members><partitionUsage>General</partitionUsage>";
            soapreq += "<name>" + nameprefix + "-CSS-LD</name>";

            soapreq += "</css></ns:addCss>";


            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP request for CSS LD = " + soapreq);

            this.sendAXLCommand(soapreq);
            System.Threading.Thread.Sleep(2000);


        }

        public void addMTP(string name, string DPName)
        {

    /*           <ns:addMtp sequence="?">
         <mtp>
            <mtpType>?</mtpType>
            <name>?</name>
            <!--Optional:-->
            <description>?</description>
            <devicePoolName uuid="?">?</devicePoolName>
            <!--Optional:-->
            <trustedRelayPoint>?</trustedRelayPoint>
         </mtp>
      </ns:addMtp>*/



            string soapreq;



            soapreq = this.getSoapHeader();

            soapreq += @" <ns:addMtp>
               <mtp>
                <mtpType>Cisco IOS Enhanced Software Media Termination Point</mtpType>
                <name>" + name + @"</name>
                <description>" + name + @" SW MTP</description>
                <devicePoolName>" + DPName + @"</devicePoolName>
                <trustedRelayPoint>false</trustedRelayPoint>
              </mtp>
            </ns:addMtp>";

            soapreq += this.getSoapFooter();

            Debug.WriteLine("add MTP SOAP call: " + soapreq);


            this.sendAXLCommand(soapreq);



        }

        public void addTranscoder(string name, string DPName)
        {
            /*     <ns:getTranscoderResponse xmlns:ns="http://www.cisco.com/AXL/API/10.5">
         <return>
            <transcoder uuid="{8D005A00-E870-1252-D19F-684DBBB82E1F}">
               <description>Transcoder</description>
               <product>Cisco IOS Enhanced Media Termination Point</product>
               <subUnit>0</subUnit>
               <devicePoolName uuid="{2322E69B-339A-8320-EDA7-B418CCF58359}">def-Store456-DP</devicePoolName>
               <isTrustedRelayPoint>false</isTrustedRelayPoint>
               <maximumCapacity>0</maximumCapacity>
            </transcoder>
         </return>
      </ns:getTranscoderResponse>
*/

            string soapreq;



            soapreq = this.getSoapHeader();

            soapreq += @"<ns:addTranscoder>
         <transcoder>
            <name>" + name + @"</name>
         
            <description>" + name + @" HW XCode</description>
            <product>Cisco IOS Enhanced Media Termination Point</product>
            
            <subUnit>0</subUnit>
            <devicePoolName>" + DPName + @"</devicePoolName>
            
            
            <isTrustedRelayPoint>false</isTrustedRelayPoint>
            
         </transcoder>
      </ns:addTranscoder>
          ";


            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP call for adding transcoder: " + soapreq);


            this.sendAXLCommand(soapreq);





        }


        public void addUser(string firstname, string lastname, string userid, string telephone, string password, string pin)
        {


            /*
             *   <ns:addUser sequence="?">
         <user>
            <!--Optional:-->
            <firstName>?</firstName>
            <!--Optional:-->
            
            <lastName>?</lastName>
            <userid>?</userid>
            <!--Optional:-->
            <password>?</password>
            <!--Optional:-->
            <pin>?</pin>
            <telephoneNumber>?</telephoneNumber>
            
         </user>
      </ns:addUser>
             */


            string soapreq;



            soapreq = this.getSoapHeader();

            soapreq += @"<ns:addUser>
           <user>
            <firstName>" + firstname +  @"</firstName>
            <lastName>" + lastname + @"</lastName>
            <userid>" + userid + @"</userid>
            <password>" + password + @"</password>
            <pin>" + pin + @"</pin>
            <telephoneNumber>" + telephone + @"</telephoneNumber>
            </user>
      </ns:addUser>
          ";


            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP call for adding conference: " + soapreq);


            this.sendAXLCommand(soapreq);



        }
        public void addUser(string firstname, string lastname, string userid, string telephone, string password, string pin, string digestcredentials, string primaryextensionpartition, string primaryext)
        {


            /*
             *   <ns:addUser sequence="?">
         <user>
            <!--Optional:-->
            <firstName>?</firstName>
            <!--Optional:-->
            
            <lastName>?</lastName>
            <userid>?</userid>
            <!--Optional:-->
            <password>?</password>
            <!--Optional:-->
            <pin>?</pin>
            <telephoneNumber>?</telephoneNumber>
            
         </user>
      </ns:addUser>
             */


            string soapreq;



            soapreq = this.getSoapHeader();

            soapreq += @"<ns:addUser>
           <user>
            <firstName>" + firstname + @"</firstName>
            <lastName>" + lastname + @"</lastName>
            <userid>" + userid + @"</userid>
            <password>" + password + @"</password>
            <pin>" + pin + @"</pin>
            <telephoneNumber>" + telephone + @"</telephoneNumber>
            </user>
      </ns:addUser>
          ";


            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP call for adding conference: " + soapreq);


            this.sendAXLCommand(soapreq);



        }
        public void addATEBPhone(string devicename, string userid, string extension, string storenum, string storeprefix)
        {
            #region

            /*<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/">
   <soapenv:Body>
     <ns:addPhone sequence="?">
         <return>
            <phone>
               <name>SEP111122223631</name>
               <description>STORE 631 ATEB Saker</description>
               <product>Third-party SIP Device (Advanced)</product>
               <model>Third-party SIP Device (Advanced)</model>
               <class>Phone</class>
               <protocol>SIP</protocol>
               <protocolSide>User</protocolSide>
               <callingSearchSpaceName>SAKER-NJ-Store631-CSS-LD</callingSearchSpaceName>
               <devicePoolName>SAKER-NJ-Store631-DP</devicePoolName>
               <commonDeviceConfigName>Store-CDC-1</commonDeviceConfigName>
               <commonPhoneConfigName>Standard Common Phone Profile</commonPhoneConfigName>
               <networkLocation>Use System Default</networkLocation>
               <locationName>SAKER-NJ-Store631-L</locationName>
               <mediaResourceListName>SAKER-NJ-Store631-MRGL</mediaResourceListName>
               <networkHoldMohAudioSourceId/>
               <userHoldMohAudioSourceId/>
               <automatedAlternateRoutingCssName/>
               <aarNeighborhoodName/>
               <loadInformation special="false"/>
               <vendorConfig/>
               <versionStamp>{1428608960-01A45ABD-88DE-4765-ADDB-72CD3C3E887C}</versionStamp>
               <traceFlag>false</traceFlag>
               <mlppDomainId/>
               <mlppIndicationStatus>Off</mlppIndicationStatus>
               <preemption>Disabled</preemption>
               <useTrustedRelayPoint>Default</useTrustedRelayPoint>
               <retryVideoCallAsAudio>true</retryVideoCallAsAudio>
               <securityProfileName>Third-party SIP Device Advanced - Standard SIP Non-Secure Profile</securityProfileName>
               <sipProfileName>Standard SIP Profile</sipProfileName>
               <cgpnTransformationCssName/>
               <useDevicePoolCgpnTransformCss>true</useDevicePoolCgpnTransformCss>
               <geoLocationName/>
               <geoLocationFilterName/>
               <sendGeoLocation>false</sendGeoLocation>
               <lines>
                  <line>
                     <index>1</index>
                     <label/>
                     <display/>
                     <dirn>
                        <pattern>106310010</pattern>
                        <routePartitionName>LINE</routePartitionName>
                     </dirn>
                     <ringSetting>Ring</ringSetting>
                     <consecutiveRingSetting>Use System Default</consecutiveRingSetting>
                     <ringSettingIdlePickupAlert/>
                     <ringSettingActivePickupAlert/>
                     <displayAscii/>
                     <e164Mask/>
                     <dialPlanWizardId/>
                     <mwlPolicy>Use System Policy</mwlPolicy>
                     <maxNumCalls>2</maxNumCalls>
                     <busyTrigger>2</busyTrigger>
                     <callInfoDisplay>
                        <callerName>true</callerName>
                        <callerNumber>false</callerNumber>
                        <redirectedNumber>false</redirectedNumber>
                        <dialedNumber>true</dialedNumber>
                     </callInfoDisplay>
                     <recordingProfileName/>
                     <monitoringCssName/>
                     <recordingFlag>Call Recording Disabled</recordingFlag>
                     <audibleMwi>Default</audibleMwi>
                     <speedDial/>
                     <partitionUsage>General</partitionUsage>
                     <associatedEndusers/>
                     <missedCallLogging>true</missedCallLogging>
                     <recordingMediaSource>Gateway Preferred</recordingMediaSource>
                  </line>
               </lines>
               <numberOfButtons>8</numberOfButtons>
               <phoneTemplateName>Third-party SIP Device (Advanced)</phoneTemplateName>
               <speeddials/>
               <busyLampFields/>
               <primaryPhoneName/>
               <ringSettingIdleBlfAudibleAlert>Default</ringSettingIdleBlfAudibleAlert>
               <ringSettingBusyBlfAudibleAlert>Default</ringSettingBusyBlfAudibleAlert>
               <blfDirectedCallParks/>
               <addOnModules/>
               <userLocale/>
               <networkLocale/>
               <idleTimeout/>
               <authenticationUrl/>
               <directoryUrl/>
               <idleUrl/>
               <informationUrl/>
               <messagesUrl/>
               <proxyServerUrl/>
               <servicesUrl/>
               <services/>
               <softkeyTemplateName/>
               <loginUserId/>
               <defaultProfileName/>
               <enableExtensionMobility>false</enableExtensionMobility>
               <currentProfileName uuid=""/>
               <loginTime/>
               <loginDuration/>
               <currentConfig>
                  <userHoldMohAudioSourceId/>
                  <phoneTemplateName>Third-party SIP Device (Advanced)</phoneTemplateName>
                  <mlppDomainId/>
                  <mlppIndicationStatus>Off</mlppIndicationStatus>
                  <preemption>Disabled</preemption>
                  <softkeyTemplateName/>
                  <ignorePresentationIndicators>false</ignorePresentationIndicators>
                  <singleButtonBarge>Off</singleButtonBarge>
                  <joinAcrossLines>Off</joinAcrossLines>
                  <callInfoPrivacyStatus>Default</callInfoPrivacyStatus>
                  <dndStatus/>
                  <dndRingSetting/>
                  <dndOption>Ringer Off</dndOption>
                  <alwaysUsePrimeLine>Default</alwaysUsePrimeLine>
                  <alwaysUsePrimeLineForVoiceMessage>Default</alwaysUsePrimeLineForVoiceMessage>
                  <emccCallingSearchSpaceName xsi:nil="true" uuid="" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"/>
               </currentConfig>
               <singleButtonBarge>Off</singleButtonBarge>
               <joinAcrossLines>Off</joinAcrossLines>
               <builtInBridgeStatus>Off</builtInBridgeStatus>
               <callInfoPrivacyStatus>Default</callInfoPrivacyStatus>
               <hlogStatus>On</hlogStatus>
               <ownerUserName>106310010</ownerUserName>
               <ignorePresentationIndicators>false</ignorePresentationIndicators>
               <packetCaptureMode>None</packetCaptureMode>
               <packetCaptureDuration>0</packetCaptureDuration>
               <subscribeCallingSearchSpaceName>WF-Internal-CSS</subscribeCallingSearchSpaceName>
               <rerouteCallingSearchSpaceName>WF-Internal-CSS</rerouteCallingSearchSpaceName>
               <allowCtiControlFlag>false</allowCtiControlFlag>
               <presenceGroupName>Standard Presence group</presenceGroupName>
               <unattendedPort>false</unattendedPort>
               <requireDtmfReception>false</requireDtmfReception>
               <rfc2833Disabled>false</rfc2833Disabled>
               <certificateOperation>No Pending Operation</certificateOperation>
               <authenticationMode>By Null String</authenticationMode>
               <keySize>1024</keySize>
               <authenticationString/>
               <certificateStatus>None</certificateStatus>
               <upgradeFinishTime/>
               <deviceMobilityMode>Default</deviceMobilityMode>
               <remoteDevice>false</remoteDevice>
               <dndOption>Ringer Off</dndOption>
               <dndRingSetting/>
               <dndStatus>false</dndStatus>
               <isActive>true</isActive>
               <isDualMode>false</isDualMode>
               <mobilityUserIdName/>
               <phoneSuite>Default</phoneSuite>
               <phoneServiceDisplay>Default</phoneServiceDisplay>
               <isProtected>false</isProtected>
               <mtpRequired>false</mtpRequired>
               <mtpPreferedCodec>711ulaw</mtpPreferedCodec>
               <dialRulesName/>
               <sshUserId/>
               <sshPwd/>
               <digestUser>106310010</digestUser>
               <outboundCallRollover>No Rollover</outboundCallRollover>
               <hotlineDevice>false</hotlineDevice>
               <secureInformationUrl/>
               <secureDirectoryUrl/>
               <secureMessageUrl/>
               <secureServicesUrl/>
               <secureAuthenticationUrl/>
               <secureIdleUrl/>
               <alwaysUsePrimeLine>Default</alwaysUsePrimeLine>
               <alwaysUsePrimeLineForVoiceMessage>Default</alwaysUsePrimeLineForVoiceMessage>
               <featureControlPolicy/>
               <deviceTrustMode>Not Trusted</deviceTrustMode>
               <AllowPresentationSharingUsingBfcp>false</AllowPresentationSharingUsingBfcp>
               <confidentialAccess>
                  <confidentialAccessMode/>
                  <confidentialAccessLevel>-1</confidentialAccessLevel>
               </confidentialAccess>
               <allowiXApplicableMedia>false</allowiXApplicableMedia>
               <cgpnIngressDN/>
               <useDevicePoolCgpnIngressDN>true</useDevicePoolCgpnIngressDN>
               <msisdn/>
               <enableCallRoutingToRdWhenNoneIsActive>f</enableCallRoutingToRdWhenNoneIsActive>
               <wifiHotspotProfile/>
               <wirelessLanProfileGroup/>
            </phone>
         </return>
      </ns:addPhone sequence="?">
   </soapenv:Body>
</soapenv:Envelope>*/
            #endregion



            string soapreq;



            soapreq = this.getSoapHeader();

            soapreq += @" <ns:addPhone>
        
            <phone>
               <name>" + devicename + @"</name>
    
            <description>STORE " + storenum + @" ATEB</description>
               <product>Third-party SIP Device (Advanced)</product>
               
               <class>Phone</class>
               <protocol>SIP</protocol>
               <protocolSide>User</protocolSide>
               <callingSearchSpaceName>" + storeprefix + @"-CSS-LD</callingSearchSpaceName>
               <devicePoolName>" + storeprefix + @"-DP</devicePoolName>
               <commonDeviceConfigName>Store-CDC-1</commonDeviceConfigName>
               <commonPhoneConfigName>Standard Common Phone Profile</commonPhoneConfigName>
               <networkLocation>Use System Default</networkLocation>
               <locationName>" + storeprefix + @"-L</locationName>
               <mediaResourceListName>" + storeprefix + @"-MRGL</mediaResourceListName>
               <networkHoldMohAudioSourceId/>
               <userHoldMohAudioSourceId/>
               <automatedAlternateRoutingCssName/>
               <aarNeighborhoodName/>";
               soapreq += "<loadInformation special=\"false\"/>";
               soapreq += @"
               <vendorConfig/>
               <traceFlag>false</traceFlag>
               <mlppDomainId/>
<mlppIndicationStatus>Off</mlppIndicationStatus>
               <preemption>Disabled</preemption>
               <useTrustedRelayPoint>Default</useTrustedRelayPoint>
               <retryVideoCallAsAudio>true</retryVideoCallAsAudio>
               <securityProfileName>Third-party SIP Device Advanced - Standard SIP Non-Secure Profile</securityProfileName>
               <sipProfileName>Standard SIP Profile</sipProfileName>
               <cgpnTransformationCssName/>
               <useDevicePoolCgpnTransformCss>true</useDevicePoolCgpnTransformCss>
               <geoLocationName/>
               <geoLocationFilterName/>
               <sendGeoLocation>false</sendGeoLocation>
               <lines>
                  <line>
                     <index>1</index>
                     <label/>
                     <display/>
                     <dirn>
                        <pattern>" + extension + @"</pattern>
                        <routePartitionName>LINE</routePartitionName>
                     </dirn>
                     <ringSetting>Ring</ringSetting>
                     <consecutiveRingSetting>Use System Default</consecutiveRingSetting>
                     <ringSettingIdlePickupAlert/>
                     <ringSettingActivePickupAlert/>
                     <displayAscii/>
                     <e164Mask/>
                     <dialPlanWizardId/>
                     <mwlPolicy>Use System Policy</mwlPolicy>
                     <maxNumCalls>4</maxNumCalls>
                     <busyTrigger>4</busyTrigger>
                     <callInfoDisplay>
                        <callerName>true</callerName>
                        <callerNumber>false</callerNumber>
                        <redirectedNumber>false</redirectedNumber>
                        <dialedNumber>true</dialedNumber>
                     </callInfoDisplay>
                     <recordingProfileName/>
                     <monitoringCssName/>
                     <recordingFlag>Call Recording Disabled</recordingFlag>
                     <audibleMwi>Default</audibleMwi>
                     <speedDial/>
                     <partitionUsage>General</partitionUsage>
                     <associatedEndusers/>
                     <missedCallLogging>true</missedCallLogging>
                     <recordingMediaSource>Gateway Preferred</recordingMediaSource>
                  </line>
               </lines>
               <numberOfButtons>8</numberOfButtons>
               <phoneTemplateName>Third-party SIP Device (Advanced)</phoneTemplateName>
               <speeddials/>
               <busyLampFields/>
               <primaryPhoneName/>
               <ringSettingIdleBlfAudibleAlert>Default</ringSettingIdleBlfAudibleAlert>
               <ringSettingBusyBlfAudibleAlert>Default</ringSettingBusyBlfAudibleAlert>
               <blfDirectedCallParks/>
               <addOnModules/>
               <userLocale/>
               <networkLocale/>
               <idleTimeout/>
               <authenticationUrl/>
               <directoryUrl/>
               <idleUrl/>
               <informationUrl/>
               <messagesUrl/>
               <proxyServerUrl/>
               <servicesUrl/>
               <services/>
               <softkeyTemplateName/>
               <loginUserId/>
               <defaultProfileName/>
               <enableExtensionMobility>false</enableExtensionMobility>
              
               <loginTime/>
               <loginDuration/>
               <currentConfig>
                  <userHoldMohAudioSourceId/>
                  <phoneTemplateName>Third-party SIP Device (Advanced)</phoneTemplateName>
                  <mlppDomainId/>
                  <mlppIndicationStatus>Off</mlppIndicationStatus>
                  <preemption>Disabled</preemption>
                  <softkeyTemplateName/>
                  <ignorePresentationIndicators>false</ignorePresentationIndicators>
                  <singleButtonBarge>Off</singleButtonBarge>
                  <joinAcrossLines>Off</joinAcrossLines>
                  <callInfoPrivacyStatus>Default</callInfoPrivacyStatus>
                  <dndStatus/>
                  <dndRingSetting/>
                  <dndOption>Ringer Off</dndOption>
                  <alwaysUsePrimeLine>Default</alwaysUsePrimeLine>
                  <alwaysUsePrimeLineForVoiceMessage>Default</alwaysUsePrimeLineForVoiceMessage>
                  
               </currentConfig>
               <singleButtonBarge>Off</singleButtonBarge>
               <joinAcrossLines>Off</joinAcrossLines>
               <builtInBridgeStatus>Off</builtInBridgeStatus>
               <callInfoPrivacyStatus>Default</callInfoPrivacyStatus>
               <hlogStatus>On</hlogStatus>
               <ownerUserName>" + userid + @"</ownerUserName>
               <ignorePresentationIndicators>false</ignorePresentationIndicators>
               <packetCaptureMode>None</packetCaptureMode>
               <packetCaptureDuration>0</packetCaptureDuration>
               <subscribeCallingSearchSpaceName>WF-Internal-CSS</subscribeCallingSearchSpaceName>
               <rerouteCallingSearchSpaceName>WF-Internal-CSS</rerouteCallingSearchSpaceName>
               <allowCtiControlFlag>false</allowCtiControlFlag>
               <presenceGroupName>Standard Presence group</presenceGroupName>
               <unattendedPort>false</unattendedPort>
               <requireDtmfReception>false</requireDtmfReception>
               <rfc2833Disabled>false</rfc2833Disabled>
               <certificateOperation>No Pending Operation</certificateOperation>
               <authenticationMode>By Null String</authenticationMode>
               <keySize>1024</keySize>
               <authenticationString/>
               <certificateStatus>None</certificateStatus>
               <upgradeFinishTime/>
               <deviceMobilityMode>Default</deviceMobilityMode>
               <remoteDevice>false</remoteDevice>
               <dndOption>Ringer Off</dndOption>
               <dndRingSetting/>
               <dndStatus>false</dndStatus>
               <isActive>true</isActive>
               <isDualMode>false</isDualMode>
               <mobilityUserIdName/>
               <phoneSuite>Default</phoneSuite>
               <phoneServiceDisplay>Default</phoneServiceDisplay>
               <isProtected>false</isProtected>
               <mtpRequired>false</mtpRequired>
               <mtpPreferedCodec>711ulaw</mtpPreferedCodec>
               <dialRulesName/>
               <sshUserId/>
               <sshPwd/>
               <digestUser>" + userid + @"</digestUser>
               <outboundCallRollover>No Rollover</outboundCallRollover>
               <hotlineDevice>false</hotlineDevice>
               <secureInformationUrl/>
               <secureDirectoryUrl/>
               <secureMessageUrl/>
               <secureServicesUrl/>
               <secureAuthenticationUrl/>
               <secureIdleUrl/>
               <alwaysUsePrimeLine>Default</alwaysUsePrimeLine>
               <alwaysUsePrimeLineForVoiceMessage>Default</alwaysUsePrimeLineForVoiceMessage>
               <featureControlPolicy/>
               <deviceTrustMode>Not Trusted</deviceTrustMode>
               <AllowPresentationSharingUsingBfcp>false</AllowPresentationSharingUsingBfcp>
               <allowiXApplicableMedia>false</allowiXApplicableMedia>
               <cgpnIngressDN/>
               <useDevicePoolCgpnIngressDN>true</useDevicePoolCgpnIngressDN>
               <msisdn/>
               <enableCallRoutingToRdWhenNoneIsActive>f</enableCallRoutingToRdWhenNoneIsActive>
               <wifiHotspotProfile/>
               <wirelessLanProfileGroup/>
            </phone>
         
      </ns:addPhone>";





            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP call for adding conference: " + soapreq);


            this.sendAXLCommand(soapreq);




        }
        public void addDirectoryNumberATEB(string storenum, string display, string storeprefix)
        {
            /// TODO: add a struct that carries all of the call forward information.
            #region
            /*
             *   <line uuid="{7946E5C7-2FBB-9D4A-FB0F-22F2EE362B01}">
               <pattern>105790010</pattern>
               <description>Store 579 Pharmacy</description>
               <usage>Device</usage>
               <routePartitionName uuid="{822DEDE9-7D38-FCF5-D1BB-46EA201B79BC}">LINE</routePartitionName>
               <aarNeighborhoodName/>
               <aarDestinationMask/>
               <aarKeepCallHistory>true</aarKeepCallHistory>
               <aarVoiceMailEnabled>false</aarVoiceMailEnabled>
               <callForwardAll>
                  <forwardToVoiceMail>false</forwardToVoiceMail>
                  <callingSearchSpaceName xsi:nil="true" uuid="" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"/>
                  <secondaryCallingSearchSpaceName xsi:nil="true" uuid="" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"/>
                  <destination/>
               </callForwardAll>
               <callForwardBusy>
                  <forwardToVoiceMail>false</forwardToVoiceMail>
                  <callingSearchSpaceName xsi:nil="true" uuid="" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"/>
                  <destination/>
               </callForwardBusy>
               <callForwardBusyInt>
                  <forwardToVoiceMail>false</forwardToVoiceMail>
                  <callingSearchSpaceName xsi:nil="true" uuid="" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"/>
                  <destination/>
               </callForwardBusyInt>
               <callForwardNoAnswer>
                  <forwardToVoiceMail>false</forwardToVoiceMail>
                  <callingSearchSpaceName xsi:nil="true" uuid="" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"/>
                  <destination/>
                  <duration>300</duration>
               </callForwardNoAnswer>
               <callForwardNoAnswerInt>
                  <forwardToVoiceMail>false</forwardToVoiceMail>
                  <callingSearchSpaceName xsi:nil="true" uuid="" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"/>
                  <destination/>
                  <duration>300</duration>
               </callForwardNoAnswerInt>
               <callForwardNoCoverage>
                  <forwardToVoiceMail>false</forwardToVoiceMail>
                  <callingSearchSpaceName xsi:nil="true" uuid="" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"/>
                  <destination/>
               </callForwardNoCoverage>
               <callForwardNoCoverageInt>
                  <forwardToVoiceMail>false</forwardToVoiceMail>
                  <callingSearchSpaceName xsi:nil="true" uuid="" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"/>
                  <destination/>
               </callForwardNoCoverageInt>
               <callForwardOnFailure>
                  <forwardToVoiceMail>false</forwardToVoiceMail>
                  <callingSearchSpaceName xsi:nil="true" uuid="" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"/>
                  <destination/>
               </callForwardOnFailure>
               <callForwardAlternateParty>
                  <callingSearchSpaceName xsi:nil="true" uuid="" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"/>
                  <destination/>
                  <duration/>
               </callForwardAlternateParty>
               <callForwardNotRegistered>
                  <forwardToVoiceMail>false</forwardToVoiceMail>
                  <callingSearchSpaceName uuid="{353F48AC-557B-13B1-FD45-AB006CDF6758}">EICKHOFF-NJ-Store579-CSS-Internal</callingSearchSpaceName>
                  <destination>105793500</destination>
               </callForwardNotRegistered>
               <callForwardNotRegisteredInt>
                  <forwardToVoiceMail>false</forwardToVoiceMail>
                  <callingSearchSpaceName uuid="{353F48AC-557B-13B1-FD45-AB006CDF6758}">EICKHOFF-NJ-Store579-CSS-Internal</callingSearchSpaceName>
                  <destination>105793500</destination>
               </callForwardNotRegisteredInt>
               <callPickupGroupName xsi:nil="true" uuid="" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"/>
               <autoAnswer>Auto Answer Off</autoAnswer>
               <networkHoldMohAudioSourceId/>
               <userHoldMohAudioSourceId/>
               <alertingName>Store 579 Pharmacy</alertingName>
               <asciiAlertingName>Store 579 Pharmacy</asciiAlertingName>
               <presenceGroupName uuid="{AD243D17-98B4-4118-8FEB-5FF2E1B781AC}">Standard Presence group</presenceGroupName>
               <shareLineAppearanceCssName/>
               <voiceMailProfileName/>
               <patternPrecedence>Default</patternPrecedence>
               <releaseClause>No Error</releaseClause>
               <hrDuration/>
               <hrInterval/>
               <cfaCssPolicy>Use System Default</cfaCssPolicy>
               <defaultActivatedDeviceName/>
               <parkMonForwardNoRetrieveDn/>
               <parkMonForwardNoRetrieveIntDn/>
               <parkMonForwardNoRetrieveVmEnabled>false</parkMonForwardNoRetrieveVmEnabled>
               <parkMonForwardNoRetrieveIntVmEnabled>false</parkMonForwardNoRetrieveIntVmEnabled>
               <parkMonForwardNoRetrieveCssName/>
               <parkMonForwardNoRetrieveIntCssName/>
               <parkMonReversionTimer/>
               <partyEntranceTone>Default</partyEntranceTone>
               <directoryURIs/>
               <allowCtiControlFlag>true</allowCtiControlFlag>
               <rejectAnonymousCall>false</rejectAnonymousCall>
               <patternUrgency>false</patternUrgency>
               <confidentialAccess>
                  <confidentialAccessMode/>
                  <confidentialAccessLevel>-1</confidentialAccessLevel>
               </confidentialAccess>
               <externalCallControlProfile/>
               <enterpriseAltNum>
                  <numMask/>
                  <isUrgent>f</isUrgent>
                  <addLocalRoutePartition>f</addLocalRoutePartition>
                  <routePartition/>
                  <advertiseGloballyIls>f</advertiseGloballyIls>
               </enterpriseAltNum>
               <e164AltNum>
                  <numMask/>
                  <isUrgent>f</isUrgent>
                  <addLocalRoutePartition>f</addLocalRoutePartition>
                  <routePartition/>
                  <advertiseGloballyIls>f</advertiseGloballyIls>
               </e164AltNum>
               <pstnFailover/>
               <callControlAgentProfile/>
               <associatedDevices>
                  <device>SEP111111110579</device>
               </associatedDevices>
               <useEnterpriseAltNum>false</useEnterpriseAltNum>
               <useE164AltNum>false</useE164AltNum>
               <active>true</active>
            </line>*/
            #endregion

            
            string soapreq;



            soapreq = this.getSoapHeader();

            soapreq += @" <ns:addLine>
            <line>
               <pattern>10" + storenum + @"0010</pattern>
               <description>" + display + @"</description>
               <usage>Device</usage>
               <routePartitionName>LINE</routePartitionName>
               <aarNeighborhoodName/>
               <aarDestinationMask/>
               <aarKeepCallHistory>true</aarKeepCallHistory>
               <aarVoiceMailEnabled>false</aarVoiceMailEnabled>
               <callForwardNotRegistered>
                  <forwardToVoiceMail>false</forwardToVoiceMail>
                  <callingSearchSpaceName>" + storeprefix + @"-CSS-Internal</callingSearchSpaceName>
                  <destination>10" + storenum + @"3500</destination>
               </callForwardNotRegistered>
               <callForwardNotRegisteredInt>
                  <forwardToVoiceMail>false</forwardToVoiceMail>
                  <callingSearchSpaceName>" + storeprefix + @"-CSS-Internal</callingSearchSpaceName>
                  <destination>10" + storenum + @"3500</destination>
               </callForwardNotRegisteredInt>
               <autoAnswer>Auto Answer Off</autoAnswer>
               <networkHoldMohAudioSourceId/>
               <userHoldMohAudioSourceId/>
               <alertingName>" + display + @"</alertingName>
               <asciiAlertingName>" + display + @"</asciiAlertingName>
               <presenceGroupName>Standard Presence group</presenceGroupName>
               <shareLineAppearanceCssName/>
               <voiceMailProfileName/>
               <patternPrecedence>Default</patternPrecedence>
               <releaseClause>No Error</releaseClause>
               <hrDuration/>
               <hrInterval/>
               <cfaCssPolicy>Use System Default</cfaCssPolicy>
               <defaultActivatedDeviceName/>
               <parkMonForwardNoRetrieveDn/>
               <parkMonForwardNoRetrieveIntDn/>
               <parkMonForwardNoRetrieveVmEnabled>false</parkMonForwardNoRetrieveVmEnabled>
               <parkMonForwardNoRetrieveIntVmEnabled>false</parkMonForwardNoRetrieveIntVmEnabled>
               <parkMonForwardNoRetrieveCssName/>
               <parkMonForwardNoRetrieveIntCssName/>
               <parkMonReversionTimer/>
               <partyEntranceTone>Default</partyEntranceTone>
               <directoryURIs/>
               <allowCtiControlFlag>true</allowCtiControlFlag>
               <rejectAnonymousCall>false</rejectAnonymousCall>
               <patternUrgency>false</patternUrgency>
               <externalCallControlProfile/>
               <enterpriseAltNum>
                  <numMask/>
                  <isUrgent>f</isUrgent>
                  <addLocalRoutePartition>f</addLocalRoutePartition>
                  <routePartition/>
                  <advertiseGloballyIls>f</advertiseGloballyIls>
               </enterpriseAltNum>
               <e164AltNum>
                  <numMask/>
                  <isUrgent>f</isUrgent>
                  <addLocalRoutePartition>f</addLocalRoutePartition>
                  <routePartition/>
                  <advertiseGloballyIls>f</advertiseGloballyIls>
               </e164AltNum>
               <pstnFailover/>
               <callControlAgentProfile/>
               <useEnterpriseAltNum>false</useEnterpriseAltNum>
               <useE164AltNum>false</useE164AltNum>
               <active>true</active>
            </line>
</ns:addLine>
";


            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP call for adding conference: " + soapreq);


            this.sendAXLCommand(soapreq);




        }
        public void addConference(string name, string DPName, string LocationName)
        {
            /*          <ns:addConferenceBridge sequence="?">
         <conferenceBridge>
               <name>SR666-CONF</name>
               <description>SR666-CONF</description>
               <product>Cisco IOS Enhanced Conference Bridge</product>
               <devicePoolName>def-Store456-DP</devicePoolName>
               <locationName>test-Store451-L</locationName>
               <subUnit>0</subUnit>
               <loadInformation special="false"/>
               <useTrustedRelayPoint>Default</useTrustedRelayPoint>
               <securityProfileName>Non Secure Conference Bridge</securityProfileName>
               <destinationAddress/>
            </conferenceBridge>
      </ns:addConferenceBridge>
*/

            string soapreq;



            soapreq = this.getSoapHeader();

            soapreq += @"<ns:addConferenceBridge>
         <conferenceBridge>
            <name>" + name + @"</name>
         
            <description>" + name + @" HW Conference Bridge</description>
            <product>Cisco IOS Enhanced Conference Bridge</product>
            
            <devicePoolName>" + DPName + @"</devicePoolName>
            <locationName>" + LocationName + @"</locationName>
            <subUnit>0</subUnit>
            <useTrustedRelayPoint>Default</useTrustedRelayPoint>
            <securityProfileName>Non Secure Conference Bridge</securityProfileName>
            
            </conferenceBridge>
      </ns:addConferenceBridge>
          ";


            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP call for adding conference: " + soapreq);


            this.sendAXLCommand(soapreq);





        }

        public void addPhoneService(string storenum)
        {


            string soapreq;
            soapreq = this.getSoapHeader();
            soapreq += @"
   <ns:addIpPhoneServices>
         <ipPhoneServices>
            <serviceName>Store" + storenum + @"-Directory</serviceName>
            <asciiServiceName>Store" + storenum + @"-Directory</asciiServiceName>
            
            <serviceDescription>Store " + storenum + @" specific directory</serviceDescription>
            <serviceUrl>http://uccmsrv1.wakefern.com:8080/ccmcip/xmldirectorylist.jsp</serviceUrl>
            <serviceCategory>XML Service</serviceCategory>
            <serviceType>Standard IP Phone Service</serviceType>
            
            <enabled>true</enabled>
            <enterpriseSubscription>false</enterpriseSubscription>
            <parameters>
               <parameter>
                  <name>f</name>
                  <displayName>FirstName</displayName>
                  
                  <default>store" + storenum + @"</default>
                  <description>store" + storenum + @"</description>
                  <paramRequired>false</paramRequired>
                  
                  <paramPassword>false</paramPassword>
               </parameter>
            </parameters>
         </ipPhoneServices>
      </ns:addIpPhoneServices>
            

            ";


            soapreq += this.getSoapFooter();


            Debug.WriteLine("Full SOAP request for Phone Service = " + soapreq);
            this.sendAXLCommand(soapreq);

        }
        public void add7821Phone(string nameprefix ) {


            /*<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/">
   <soapenv:Body>
      <ns:getPhoneResponse xmlns:ns="http://www.cisco.com/AXL/API/10.5">
         <return>
            <phone ctiid="452" uuid="{A0E97D66-3F2F-D14B-81E8-B8D62E618AA5}">
               <name>7821_TP_Store613</name>
               <description/>
               <product>Cisco 7821</product>
               <model>Cisco 7821</model>
               <class>Phone Template</class>
               <protocol>SIP</protocol>
               <protocolSide>User</protocolSide>
               <callingSearchSpaceName uuid="{95B851E2-9ECD-D158-F0C4-ABEE7DD29987}">SAKER-NJ-Store613-CSS-Intl</callingSearchSpaceName>
               <devicePoolName uuid="{53A37B9E-0289-1DD2-0DCD-3177477B0092}">SAKER-NJ-Store613-DP</devicePoolName>
               <commonDeviceConfigName/>
               <commonPhoneConfigName uuid="{AC243D17-98B4-4118-8FEB-5FF2E1B781AC}">Standard Common Phone Profile</commonPhoneConfigName>
               <networkLocation>Use System Default</networkLocation>
               <locationName uuid="{C9B79D06-FCA9-238F-BD35-33F58BD3AFCC}">SAKER-NJ-Store613-L</locationName>
               <mediaResourceListName uuid="{29F36790-A1A4-A639-2FB3-910EEACD835C}">SAKER-NJ-Store613-MRGL</mediaResourceListName>
               <networkHoldMohAudioSourceId>1</networkHoldMohAudioSourceId>
               <userHoldMohAudioSourceId>1</userHoldMohAudioSourceId>
               <automatedAlternateRoutingCssName/>
               <aarNeighborhoodName/>
               <loadInformation special="false">sip78xx.10-2-1-12</loadInformation>
               <vendorConfig>
                  <disableSpeaker>false</disableSpeaker>
                  <disableSpeakerAndHeadset>false</disableSpeakerAndHeadset>
                  <pcPort>0</pcPort>
                  <voiceVlanAccess>0</voiceVlanAccess>
                  <spanToPCPort>1</spanToPCPort>
                  <loggingDisplay>1</loggingDisplay>
                  <recordingTone>0</recordingTone>
                  <recordingToneLocalVolume>100</recordingToneLocalVolume>
                  <recordingToneRemoteVolume>50</recordingToneRemoteVolume>
                  <moreKeyReversionTimer>5</moreKeyReversionTimer>
                  <powerPriority>0</powerPriority>
                  <LineKeyBarge>0</LineKeyBarge>
                  <minimumRingVolume>0</minimumRingVolume>
                  <ehookEnable>0</ehookEnable>
                  <headsetWidebandUIControl>0</headsetWidebandUIControl>
                  <headsetWidebandEnable>0</headsetWidebandEnable>
               </vendorConfig>
               <versionStamp>{1423592658-3CDA382F-9925-4B98-8CC6-824ED9580676}</versionStamp>
               <traceFlag>false</traceFlag>
               <mlppDomainId/>
               <mlppIndicationStatus>Default</mlppIndicationStatus>
               <preemption>Default</preemption>
               <useTrustedRelayPoint>Default</useTrustedRelayPoint>
               <retryVideoCallAsAudio>true</retryVideoCallAsAudio>
               <securityProfileName uuid="{1CD2CB25-C9F6-4786-A38F-1CF02DE290EE}">Universal Device Template - Model-independent Security Profile</securityProfileName>
               <sipProfileName uuid="{FCBC7581-4D8D-48F3-917E-00B09FB39213}">Standard SIP Profile</sipProfileName>
               <cgpnTransformationCssName/>
               <useDevicePoolCgpnTransformCss>true</useDevicePoolCgpnTransformCss>
               <geoLocationName/>
               <geoLocationFilterName/>
               <sendGeoLocation>false</sendGeoLocation>
               <lines>
                  <line uuid="{8B824C91-618F-A37A-E699-62ED8E80320B}">
                     <index>1</index>
                     <label/>
                     <display/>
                     <dirn uuid="{BA023C78-5A96-0F65-5CD1-566F9970D0F7}">
                        <pattern>SAKER_NJ_LINE_TEMPLATE</pattern>
                        <routePartitionName/>
                     </dirn>
                     <ringSetting>Use System Default</ringSetting>
                     <consecutiveRingSetting>Use System Default</consecutiveRingSetting>
                     <ringSettingIdlePickupAlert>Use System Default</ringSettingIdlePickupAlert>
                     <ringSettingActivePickupAlert>Use System Default</ringSettingActivePickupAlert>
                     <displayAscii/>
                     <e164Mask/>
                     <dialPlanWizardId/>
                     <mwlPolicy>Use System Policy</mwlPolicy>
                     <maxNumCalls>4</maxNumCalls>
                     <busyTrigger>2</busyTrigger>
                     <callInfoDisplay>
                        <callerName>true</callerName>
                        <callerNumber>false</callerNumber>
                        <redirectedNumber>false</redirectedNumber>
                        <dialedNumber>true</dialedNumber>
                     </callInfoDisplay>
                     <recordingProfileName/>
                     <monitoringCssName/>
                     <recordingFlag>Call Recording Disabled</recordingFlag>
                     <audibleMwi>Default</audibleMwi>
                     <speedDial/>
                     <partitionUsage>General</partitionUsage>
                     <associatedEndusers/>
                     <missedCallLogging>true</missedCallLogging>
                     <recordingMediaSource>Gateway Preferred</recordingMediaSource>
                  </line>
               </lines>
               <numberOfButtons>8</numberOfButtons>
               <phoneTemplateName uuid="{16F15A8C-63F6-44BA-B240-82C24714EC12}">Standard 7821 SIP</phoneTemplateName>
               <speeddials/>
               <busyLampFields/>
               <primaryPhoneName/>
               <ringSettingIdleBlfAudibleAlert>Default</ringSettingIdleBlfAudibleAlert>
               <ringSettingBusyBlfAudibleAlert>Default</ringSettingBusyBlfAudibleAlert>
               <blfDirectedCallParks/>
               <addOnModules/>
               <userLocale/>
               <networkLocale/>
               <idleTimeout/>
               <authenticationUrl/>
               <directoryUrl/>
               <idleUrl/>
               <informationUrl/>
               <messagesUrl/>
               <proxyServerUrl/>
               <servicesUrl/>
               <services>
                  <service>
                     <telecasterServiceName uuid="{F56A074C-EA26-6FAB-EC89-786B407AA3E9}">Store613-Directory</telecasterServiceName>
                     <name>Store613-Directory</name>
                     <url>http://uccmsrv1.wakefern.com:8080/ccmcip/xmldirectorylist.jsp?f=store%20613</url>
                     <urlButtonIndex>0</urlButtonIndex>
                     <urlLabel/>
                     <serviceNameAscii/>
                     <phoneService>Standard IP Phone Service</phoneService>
                     <phoneServiceCategory>XML Service</phoneServiceCategory>
                     <vendor/>
                     <version/>
                     <priority>50</priority>
                  </service>
               </services>
               <softkeyTemplateName uuid="{6ED4F904-08AD-32DC-BD57-4F96AC152D1F}">WF-Store-Standard User-Park</softkeyTemplateName>
               <loginUserId/>
               <defaultProfileName/>
               <enableExtensionMobility>false</enableExtensionMobility>
               <currentProfileName uuid=""/>
               <loginTime/>
               <loginDuration/>
               <currentConfig/>
               <singleButtonBarge>Off</singleButtonBarge>
               <joinAcrossLines>Off</joinAcrossLines>
               <builtInBridgeStatus>Default</builtInBridgeStatus>
               <callInfoPrivacyStatus>Default</callInfoPrivacyStatus>
               <hlogStatus>On</hlogStatus>
               <ownerUserName/>
               <ignorePresentationIndicators>false</ignorePresentationIndicators>
               <packetCaptureMode>None</packetCaptureMode>
               <packetCaptureDuration>0</packetCaptureDuration>
               <subscribeCallingSearchSpaceName/>
               <rerouteCallingSearchSpaceName/>
               <allowCtiControlFlag>true</allowCtiControlFlag>
               <presenceGroupName uuid="{AD243D17-98B4-4118-8FEB-5FF2E1B781AC}">Standard Presence group</presenceGroupName>
               <unattendedPort>false</unattendedPort>
               <requireDtmfReception>false</requireDtmfReception>
               <rfc2833Disabled>false</rfc2833Disabled>
               <certificateOperation>No Pending Operation</certificateOperation>
               <authenticationMode>By Null String</authenticationMode>
               <keySize>1024</keySize>
               <authenticationString/>
               <certificateStatus>None</certificateStatus>
               <upgradeFinishTime/>
               <deviceMobilityMode>Default</deviceMobilityMode>
               <remoteDevice>false</remoteDevice>
               <dndOption>Use Common Phone Profile Setting</dndOption>
               <dndRingSetting/>
               <dndStatus>false</dndStatus>
               <isActive>true</isActive>
               <isDualMode>false</isDualMode>
               <mobilityUserIdName/>
               <phoneSuite>Default</phoneSuite>
               <phoneServiceDisplay>Default</phoneServiceDisplay>
               <isProtected>false</isProtected>
               <mtpRequired>false</mtpRequired>
               <mtpPreferedCodec>711ulaw</mtpPreferedCodec>
               <dialRulesName/>
               <sshUserId/>
               <sshPwd/>
               <digestUser/>
               <outboundCallRollover>No Rollover</outboundCallRollover>
               <hotlineDevice>false</hotlineDevice>
               <secureInformationUrl/>
               <secureDirectoryUrl/>
               <secureMessageUrl/>
               <secureServicesUrl/>
               <secureAuthenticationUrl/>
               <secureIdleUrl/>
               <alwaysUsePrimeLine>Default</alwaysUsePrimeLine>
               <alwaysUsePrimeLineForVoiceMessage>Default</alwaysUsePrimeLineForVoiceMessage>
               <featureControlPolicy/>
               <deviceTrustMode>Not Trusted</deviceTrustMode>
               <confidentialAccess>
                  <confidentialAccessMode/>
                  <confidentialAccessLevel>-1</confidentialAccessLevel>
               </confidentialAccess>
               <cgpnIngressDN/>
               <useDevicePoolCgpnIngressDN>true</useDevicePoolCgpnIngressDN>
               <msisdn/>
               <enableCallRoutingToRdWhenNoneIsActive>f</enableCallRoutingToRdWhenNoneIsActive>
               <wifiHotspotProfile/>
               <wirelessLanProfileGroup/>
            </phone>
         </return>
      </ns:getPhoneResponse>
   </soapenv:Body>
</soapenv:Envelope>*/


        }
        public void addLocalInternalCSS(string nameprefix)
        {



            string soapreq;
            soapreq = this.getSoapHeader();
            soapreq += " <ns:addCss><css>";
            soapreq += "<description>" + nameprefix + @" Local  CSS</description>";
            soapreq += "<members>";
            soapreq += @"
                  <member>
                     <routePartitionName>WF-E911</routePartitionName>
                     <index>1</index>
                  </member>
                  <member>
                     <routePartitionName>" + nameprefix + @"-Local</routePartitionName>
                     <index>2</index>
                  </member>
                  <member>
                     <routePartitionName>WF-Local</routePartitionName>
                     <index>3</index>
                  </member>
                  <member>
                     <routePartitionName>WF-Service</routePartitionName>
                     <index>4</index>
                  </member>
                  <member>
                     <routePartitionName>WF-TollFree</routePartitionName>
                     <index>5</index>
                  </member>
                  <member>
                     <routePartitionName>LINE</routePartitionName>
                     <index>6</index>
                  </member>
<member>
                     <routePartitionName>" + nameprefix + @"-Internal</routePartitionName>
                     <index>7</index>
                  </member>


";


            soapreq += "</members><partitionUsage>General</partitionUsage>";
            soapreq += "<name>" + nameprefix + "-CSS-Local</name>";

            soapreq += "</css></ns:addCss>";


            soapreq += this.getSoapFooter();


            Debug.WriteLine("Full SOAP request for CSS Local = " + soapreq);
            this.sendAXLCommand(soapreq);
            System.Threading.Thread.Sleep(2000);




            // Then internal only


            soapreq = this.getSoapHeader();
            soapreq += " <ns:addCss><css>";
            soapreq += "<description>" + nameprefix + @" Internal Only CSS</description>";
            soapreq += "<members>";
            soapreq += @"
                  <member>
                     <routePartitionName>WF-E911</routePartitionName>
                     <index>1</index>
                  </member>
                  <member>
                     <routePartitionName>WF-Service</routePartitionName>
                     <index>2</index>
                  </member>
                  <member>
                     <routePartitionName>LINE</routePartitionName>
                     <index>3</index>
                  </member>
<member>
                     <routePartitionName>" + nameprefix + @"-Internal</routePartitionName>
                     <index>4</index>
                  </member>";


            soapreq += "</members><partitionUsage>General</partitionUsage>";
            soapreq += "<name>" + nameprefix + "-CSS-Internal</name>";

            soapreq += "</css></ns:addCss>";


            soapreq += this.getSoapFooter();


            Debug.WriteLine("Full SOAP request for CSS Internal = " + soapreq);

            this.sendAXLCommand(soapreq);



        }


        public void addMRG(string name, string storenum)
        {
            /*
             *     <ns:addMediaResourceGroup sequence="1">
         <mediaResourceGroup>
            <name>test</name>
            <!--Optional:-->
            <description></description>
       	  <multicast>f</multicast>
            <members>
               <!--1 or more repetitions:-->
               <member>
                  <deviceName>SR666-CONF</deviceName>
               </member>
            </members>
         </mediaResourceGroup>
      </ns:addMediaResourceGroup>
           <ns:addMediaResourceGroup sequence="2">
         <mediaResourceGroup>
            <name>test2</name>
            <!--Optional:-->
            <description></description>
            <multicast>f</multicast>
            <members>
               <!--1 or more repetitions:-->
               <member>
                  <deviceName>SR666-CONF</deviceName>
               </member>
            </members>
         </mediaResourceGroup>
      </ns:addMediaResourceGroup>*/


            string soapreq;



            soapreq = this.getSoapHeader();

            soapreq += "<ns:addMediaResourceGroup sequence=\"1\">";
            soapreq += @"    
 <mediaResourceGroup>
            <name>" + name + @"-HW-GW-MRG</name>
            <description>Hardware Media Resource group list for " + name + @"</description>
       	  <multicast>f</multicast>
            <members>
               
               <member>
                  <deviceName>SR" + storenum + @"-CONF</deviceName>
                  <deviceName>SR" + storenum + @"-XCODE</deviceName>
               </member>
            </members>
         </mediaResourceGroup>
 </ns:addMediaResourceGroup>";
       




            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP request for add MRG is: " + soapreq);

            this.sendAXLCommand(soapreq);


            soapreq = this.getSoapHeader();


            soapreq += "<ns:addMediaResourceGroup sequence=\"1\">";
            soapreq += @"
         <mediaResourceGroup>
            <name>" + name + @"-SW-GW</name>
            <description>Software Media Resource group list for " + name + @"</description>
            <multicast>f</multicast>
            <members>
               <!--1 or more repetitions:-->
               <member>
                  <deviceName>SR" + storenum + @"-MTP</deviceName>
               </member>
            </members>
         </mediaResourceGroup>
      </ns:addMediaResourceGroup>";

            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP request for add MRG is: " + soapreq);

            this.sendAXLCommand(soapreq);








        }




        public void addsrst(string name, string IPofGW)
        {


            /*
             * <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ns="http://www.cisco.com/AXL/API/10.5">
   <soapenv:Header/>
   <soapenv:Body>
      <ns:addSrst sequence="?">
         <srst>
            <name>?</name>
            <port>2000</port>
            <ipAddress>?</ipAddress>
   
         </srst>
      </ns:addSrst>
   </soapenv:Body>
</soapenv:Envelope>
             * /
             */


            string soapreq;



            soapreq = this.getSoapHeader();

            soapreq += "  <ns:addSrst sequence=\"?\"><srst><name>" + name + "</name> <port>2000</port><ipAddress>" + IPofGW + @"</ipAddress>      </srst>
      </ns:addSrst>";

            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP request for SRST is: " + soapreq);

            this.sendAXLCommand(soapreq);




        }


       

        private void sendAXLCommand(string soapreq)
        {

            System.Net.ServicePointManager.CertificatePolicy = new BruteForcePolicy();

            try
            {



                System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)WebRequest.Create("https://" + cucmip + ":8443/axl/");
                req.ProtocolVersion = System.Net.HttpVersion.Version10;
                req.ContentType = "text/xml;";
                req.Method = "POST";
                req.Headers.Add("SOAPAction: CUCM:DB ver=8.5");

                req.Credentials = new System.Net.NetworkCredential(cucmusername, cucmpassword);



                StreamWriter sw = new StreamWriter(req.GetRequestStream());
                System.Text.StringBuilder soapRequest = new System.Text.StringBuilder();
                soapRequest.Append(soapreq);
                sw.Write(soapRequest.ToString());
                sw.Close();
               

              
                //Get response and display
                using (System.Net.WebResponse webresp = (System.Net.WebResponse)req.GetResponse())
                {


                    DataSet resultXML = new DataSet();

                    Stream returnedData = webresp.GetResponseStream();


                    resultXML.ReadXml(returnedData);



#if DEBUG
                    StreamReader reader = new StreamReader(returnedData);
                    Debug.WriteLine("Resulting SOAP Response is: " + reader.ReadToEnd());
#endif





                

                    Debug.WriteLine("Complete");
                }
            }


            catch (WebException ConnectFailed)
            {
                Debug.WriteLine("Failed to connect for some reason...");
                

            }
            catch (UriFormatException UriInvalid)
            {
                Debug.WriteLine("invalid URI");
                

            }
            catch (Exception AllOtherErrors)
            {
                Debug.WriteLine(false, "Error received: " + AllOtherErrors.Message);
                
            }

            

        }


        public void add4DigitDialling(string storeprefix, string storenum)
        {
            /*
             * 
             * 
             * 
          </ns:addTransPattern>
                 <pattern>[1-8]XXX</pattern>
                   <description>SR631 4Digit Dialing</description>
                   <usage>Translation</usage>
                   <routePartitionName uuid="{AB213C19-7C30-6CD8-D163-CB3477AC753A}">SAKER-NJ-Store631-Local</routePartitionName>
                   <callingSearchSpaceName></callingSearchSpaceName>
             *     <blockEnable>false</blockEnable>
                   <useCallingPartyPhoneMask>Off</useCallingPartyPhoneMask>
                   <patternUrgency>false</patternUrgency>
                   <prefixDigitsOut>10631</prefixDigitsOut>
                   <provideOutsideDialtone>false</provideOutsideDialtone>
                   <routeNextHopByCgpn>false</routeNextHopByCgpn>
                   <routeClass>Default</routeClass>
                   <callInterceptProfileName/>
                   <releaseClause>No Error</releaseClause>
                   <useOriginatorCss>false</useOriginatorCss>
                   <dontWaitForIDTOnSubsequentHops>false</dontWaitForIDTOnSubsequentHops>
            */



            string soapreq;



            soapreq = this.getSoapHeader();

            soapreq += @"<ns:addTransPattern>
            <transPattern>
            <pattern>[1234568]XXX</pattern>
             <description>SR" + storenum + " 4Digit Dialling</description>";
            
            soapreq += @" <usage>Translation</usage>
                <routePartitionName>" + storeprefix + "-Store" + storenum + @"-Internal</routePartitionName>
            <blockEnable>false</blockEnable>
                   <useCallingPartyPhoneMask>Off</useCallingPartyPhoneMask>
                   <patternUrgency>false</patternUrgency>
 <prefixDigitsOut>10" + storenum + @"</prefixDigitsOut>
 <provideOutsideDialtone>false</provideOutsideDialtone>
                   <routeNextHopByCgpn>false</routeNextHopByCgpn>
                   <routeClass>Default</routeClass>
                   <callInterceptProfileName/>
                   <releaseClause>No Error</releaseClause>
                   <useOriginatorCss>false</useOriginatorCss>
                   <dontWaitForIDTOnSubsequentHops>false</dontWaitForIDTOnSubsequentHops>
                    <callingSearchSpaceName>" + storeprefix + "-Store" + storenum  + @"-CSS-Internal</callingSearchSpaceName>
                </transPattern>
          </ns:addTransPattern>
            
            ";





            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP request for Translation patterns is: " + soapreq);

            this.sendAXLCommand(soapreq);












        }


        public void addGateway(string macaddress, string storeprefix, string storenum)
        {

            /*
               <ns:getGatewayResponse xmlns:ns="http://www.cisco.com/AXL/API/10.5">
         <return>
            <gateway uuid="{1EE0B73A-8C9A-A847-EA9A-FD90E5BB6C8C}">
               <domainName>SKIGW05CA2D0B99</domainName>
               <description>Store 263 Voice GW</description>
               <product>Cisco 2901</product>
               <protocol>SCCP</protocol>
               <callManagerGroupName uuid="{5F56691F-57EB-C4DE-145E-F87718AC7A8F}">CMGroup-StoreGroup1</callManagerGroupName>
               <units>
                  <unit>
                     <index>0</index>
                     <product>NM-4VWIC-MBRD</product>
                     <subunits>
                        <subunit>
                           <index>2</index>
                           <product>VIC3-2FXS/DID-SCCP</product>
                           <beginPort>0</beginPort>
                        </subunit>
                     </subunits>
                  </unit>
               </units>
               <scratch/>
               <vendorConfig>
                  <globalISDNSwitchType>4ESS</globalISDNSwitchType>
                  <switchBack>Graceful</switchBack>
                  <switchBackDelay>10</switchBackDelay>
                  <switchBackSchedule>12:00</switchBackSchedule>
                  <DtmfRelay>NoChange</DtmfRelay>
                  <ModemPassthrough>Enable</ModemPassthrough>
                  <CiscoFaxRelay>Disable</CiscoFaxRelay>
                  <T38FaxRelay>Disable</T38FaxRelay>
                  <rtpPackageCapability>Enable</rtpPackageCapability>
                  <mtPackageCapability>Disable</mtPackageCapability>
                  <resPackageCapability>Disable</resPackageCapability>
                  <prePackageCapability>Enable</prePackageCapability>
                  <sstPackageCapability>Enable</sstPackageCapability>
                  <rtpUnreachableOnOff>Enable</rtpUnreachableOnOff>
                  <rtpUnreachableTimeout>1000</rtpUnreachableTimeout>
                  <rtcpReportInterval>0</rtcpReportInterval>
                  <simpleSdpEnable>Enable</simpleSdpEnable>
               </vendorConfig>
               <versionStamp>1430266326-b6e94c21-3574-4ab9-9699-9fb0316d6267</versionStamp>
               <loadInformation/>
            </gateway>
         </return>
      </ns:getGatewayResponse>


          */

        }

        public void addGateWay(string macaddress, string prefix, string storenum)
        {
            /*<ns:addGateway>
             *    <return>
            <gateway>
               <domainName>SKIGW05CA2D0B99</domainName>
               <description>Store 263 Voice GW</description>
               <product>Cisco 2901</product>
               <protocol>SCCP</protocol>
               <callManagerGroupName>CMGroup-StoreGroup1</callManagerGroupName>
               <units>
                  <unit>
                     <index>0</index>
                     <product>NM-4VWIC-MBRD</product>
                     <subunits>
                        <subunit>
                           <index>2</index>
                           <product>VIC3-2FXS/DID-SCCP</product>
                           <beginPort>0</beginPort>
                        </subunit>
                     </subunits>
                  </unit>
               </units>
               <scratch/>
               <vendorConfig>
                  <globalISDNSwitchType>4ESS</globalISDNSwitchType>
                  <switchBack>Graceful</switchBack>
                  <switchBackDelay>10</switchBackDelay>
                  <switchBackSchedule>12:00</switchBackSchedule>
                  <DtmfRelay>NoChange</DtmfRelay>
                  <ModemPassthrough>Enable</ModemPassthrough>
                  <CiscoFaxRelay>Disable</CiscoFaxRelay>
                  <T38FaxRelay>Disable</T38FaxRelay>
                  <rtpPackageCapability>Enable</rtpPackageCapability>
                  <mtPackageCapability>Disable</mtPackageCapability>
                  <resPackageCapability>Disable</resPackageCapability>
                  <prePackageCapability>Enable</prePackageCapability>
                  <sstPackageCapability>Enable</sstPackageCapability>
                  <rtpUnreachableOnOff>Enable</rtpUnreachableOnOff>
                  <rtpUnreachableTimeout>1000</rtpUnreachableTimeout>
                  <rtcpReportInterval>0</rtcpReportInterval>
                  <simpleSdpEnable>Enable</simpleSdpEnable>
               </vendorConfig>
               <versionStamp>1430266326-b6e94c21-3574-4ab9-9699-9fb0316d6267</versionStamp>
               <loadInformation/>
            </gateway>
      
      </ns:addGateway>
*/






//            string soapreq;



//            soapreq = this.getSoapHeader();

//            soapreq += "<ns:addCallPark sequence=\"1\">";
//            soapreq += @"    
// <callPark>
//            <pattern>700[0-9]</pattern>
//            <description>" + storeprefix + @" Call Park Slots</description>
//       	    <routePartitionName>" + storeprefix + @"-Local</routePartitionName>
//            <callManagerName>10.250.40.32</callManagerName>
//         </callPark>
//      </ns:addCallPark>";





//            soapreq += this.getSoapFooter();

//            Debug.WriteLine("Full SOAP request for add Park is: " + soapreq);

//            this.sendAXLCommand(soapreq);



        }

        public void addCallPark(string storeprefix)
        {

            /*
             *       <ns:addCallPark sequence="1">
         <callPark>
            <pattern>700[0-9]</pattern>
            <!--Optional:-->
            <description>test</description>
            <!--Optional:-->
            <routePartitionName>?</routePartitionName>
            <callManagerName>?</callManagerName>
         </callPark>
      </ns:addCallPark>*/


            string soapreq;



            soapreq = this.getSoapHeader();

            soapreq += "<ns:addCallPark sequence=\"1\">";
            soapreq += @"    
 <callPark>
            <pattern>700[0-9]</pattern>
            <description>" + storeprefix + @" Call Park Slots</description>
       	    <routePartitionName>" + storeprefix + @"-Internal</routePartitionName>
            <callManagerName>10.250.40.32</callManagerName>
         </callPark>
      </ns:addCallPark>";





            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP request for add Park is: " + soapreq);

            this.sendAXLCommand(soapreq);



            soapreq = this.getSoapHeader();

            soapreq += "<ns:addCallPark sequence=\"1\">";
            soapreq += @"    
 <callPark>
            <pattern>701[0-9]</pattern>
            <description>" + storeprefix + @" Call Park Slots</description>
       	    <routePartitionName>" + storeprefix + @"-Internal</routePartitionName>
            <callManagerName>10.120.40.31</callManagerName>
         </callPark>
      </ns:addCallPark>";





            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP request for add Park is: " + soapreq);

            this.sendAXLCommand(soapreq);


            

            soapreq = this.getSoapHeader();

            soapreq += "<ns:addCallPark sequence=\"1\">";
            soapreq += @"    
 <callPark>
            <pattern>702[0-9]</pattern>
            <description>" + storeprefix + @" Call Park Slots</description>
       	    <routePartitionName>" + storeprefix + @"-Internal</routePartitionName>
            <callManagerName>10.120.40.32</callManagerName>
         </callPark>
      </ns:addCallPark>";





            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP request for add Park is: " + soapreq);

            this.sendAXLCommand(soapreq);




            soapreq = this.getSoapHeader();

            soapreq += "<ns:addCallPark sequence=\"1\">";
            soapreq += @"    
 <callPark>
            <pattern>703[0-9]</pattern>
            <description>" + storeprefix + @" Call Park Slots</description>
       	    <routePartitionName>" + storeprefix + @"-Internal</routePartitionName>
            <callManagerName>10.120.40.31</callManagerName>
         </callPark>
      </ns:addCallPark>";





            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP request for add Park is: " + soapreq);

            this.sendAXLCommand(soapreq);




            soapreq = this.getSoapHeader();

            soapreq += "<ns:addCallPark sequence=\"1\">";
            soapreq += @"    
 <callPark>
            <pattern>704[0-9]</pattern>
            <description>" + storeprefix + @" Call Park Slots</description>
       	    <routePartitionName>" + storeprefix + @"-Internal</routePartitionName>
            <callManagerName>10.250.40.31</callManagerName>
         </callPark>
      </ns:addCallPark>";





            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP request for add Park is: " + soapreq);

            this.sendAXLCommand(soapreq);



            soapreq = this.getSoapHeader();

            soapreq += "<ns:addCallPark sequence=\"1\">";
            soapreq += @"    
 <callPark>
            <pattern>705[0-9]</pattern>
            <description>" + storeprefix + @" Call Park Slots</description>
       	    <routePartitionName>" + storeprefix + @"-Internal</routePartitionName>
            <callManagerName>10.120.40.33</callManagerName>
         </callPark>
      </ns:addCallPark>";





            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP request for add Park is: " + soapreq);

            this.sendAXLCommand(soapreq);



            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP request for add Park is: " + soapreq);

            this.sendAXLCommand(soapreq);



            soapreq = this.getSoapHeader();

            soapreq += "<ns:addCallPark sequence=\"1\">";
            soapreq += @"    
 <callPark>
            <pattern>706[0-9]</pattern>
            <description>" + storeprefix + @" Call Park Slots</description>
       	    <routePartitionName>" + storeprefix + @"-Internal</routePartitionName>
            <callManagerName>CM_uccmsrv6.wakefern.com</callManagerName>
         </callPark>
      </ns:addCallPark>";





            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP request for add Park is: " + soapreq);

            this.sendAXLCommand(soapreq);




        }

        public void addPaging(string storeprefix, string storenum)
        {
            /*
             * \  <ns:addRoutePattern sequence="?">
         <routePattern>
            <pattern>?</pattern>
            
            <description>?</description>
            <routePartitionName>?</routePartitionName>
            
            <provideOutsideDialtone>false</provideOutsideDialtone>
            <destination>
               
               <routeListName>Store-Local-RL</routeListName>
            </destination>
            
         </routePattern>
      </ns:addRoutePattern>*/


            string soapreq;
            soapreq = this.getSoapHeader();

            soapreq += " <ns:addRoutePattern>";
            soapreq += @"    
 <routePattern>
            <pattern>10" + storenum + @"5000</pattern>
            <description>Store " + storenum + @" Paging Route Pattern</description>
       	    <routePartitionName>" + storeprefix + @"-Internal</routePartitionName>
            <provideOutsideDialtone>false</provideOutsideDialtone>
            <destination>
               
               <routeListName>Store-Local-RL</routeListName>
            </destination>
         </routePattern>
      </ns:addRoutePattern>";

            





            soapreq += this.getSoapFooter();

            Debug.WriteLine("Full SOAP request for add paging is: " + soapreq);

            this.sendAXLCommand(soapreq);

        }


        private void sendAXLCommandNoResponse(string soapreq)
        {

            System.Net.ServicePointManager.CertificatePolicy = new BruteForcePolicy();

            try
            {


                
                System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)WebRequest.Create("https://" + cucmip + ":8443/axl/");
                req.ProtocolVersion = System.Net.HttpVersion.Version10;
                req.ContentType = "text/xml;";
                req.Method = "POST";
                req.Headers.Add("SOAPAction: CUCM:DB ver=8.5");

                req.Credentials = new System.Net.NetworkCredential(cucmusername, cucmpassword);



                StreamWriter sw = new StreamWriter(req.GetRequestStream());
                System.Text.StringBuilder soapRequest = new System.Text.StringBuilder();
                soapRequest.Append(soapreq);
                sw.Write(soapRequest.ToString());
                sw.Close();



                //Get response and display
                using (System.Net.WebResponse webresp = (System.Net.WebResponse)req.GetResponse())
                {


                    DataSet resultXML = new DataSet();

                    Stream returnedData = webresp.GetResponseStream();


                    resultXML.ReadXml(returnedData);



#if DEBUG
                    StreamReader reader = new StreamReader(returnedData);
                    Debug.WriteLine("Resulting SOAP Response is: " + reader.ReadToEnd());
#endif







                    Debug.WriteLine("Complete");
                }
            }


            catch (WebException ConnectFailed)
            {
                Debug.WriteLine("Failed to connect for some reason...");


            }
            catch (UriFormatException UriInvalid)
            {
                Debug.WriteLine("invalid URI");


            }
            catch (Exception AllOtherErrors)
            {
                Debug.WriteLine(false, "Error received: " + AllOtherErrors.Message);

            }



        }










































        public string checkAXLConnect()
        {

            string soapreq = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ns=\"http://www.cisco.com/AXL/API/8.5\"> <soapenv:Header/> <soapenv:Body> <ns:executeSQLQuery sequence=\"?\"> <sql>Select name from device where tkclass = 1</sql> </ns:executeSQLQuery> </soapenv:Body> </soapenv:Envelope>";





            System.Net.ServicePointManager.CertificatePolicy = new BruteForcePolicy();

            try
            {



                System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)WebRequest.Create("https://" + cucmip + ":8443/axl/");
                req.ProtocolVersion = System.Net.HttpVersion.Version10;
                req.ContentType = "text/xml;";
                req.Method = "POST";
                req.Headers.Add("SOAPAction: CUCM:DB ver=8.5");

                req.Credentials = new System.Net.NetworkCredential(cucmusername, cucmpassword);



                StreamWriter sw = new StreamWriter(req.GetRequestStream());
                System.Text.StringBuilder soapRequest = new System.Text.StringBuilder();
                soapRequest.Append(soapreq);
                sw.Write(soapRequest.ToString());
                sw.Close();


                //Get response and display
                using (System.Net.WebResponse webresp = (System.Net.WebResponse)req.GetResponse())
                {



                    Stream returnedData = webresp.GetResponseStream();

                    StreamReader reader = new StreamReader(returnedData);
                    return reader.ReadToEnd();







                }
            }


            catch (WebException ConnectFailed)
            {
                throw new WebException(@"Cannot connect to host", ConnectFailed);

            }
            catch (UriFormatException UriInvalid)
            {
                Debug.WriteLine("invalid URI");
                throw new UriFormatException(@"Invalid URL");

            }
            catch (Exception AllOtherErrors)
            {
                Debug.Assert(false, "Error received: " + AllOtherErrors.Message);
                throw new Exception(@"Unknown Error", AllOtherErrors);
            }




        }

        private string getSoapFooter()
        {
            // This returns the standard SOAP header 

            string soapfooter;
            soapfooter = "</soapenv:Body>";
            soapfooter += "</soapenv:Envelope>";



            Debug.WriteLine("Soap Footer: " + soapfooter);


            return soapfooter;

        }




        internal object addUser()
        {
            throw new NotImplementedException();
        }
    }
}
public class BruteForcePolicy : System.Net.ICertificatePolicy
{
    public bool CheckValidationResult(System.Net.ServicePoint sp, System.Security.Cryptography.X509Certificates.X509Certificate cert,
            System.Net.WebRequest request, int problem)
    {
        return true;
    }
}