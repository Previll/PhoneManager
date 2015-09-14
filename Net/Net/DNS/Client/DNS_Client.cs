using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;
using System.Threading;

using LumiSoft.Net.UDP;

namespace LumiSoft.Net.DNS.Client
{
	/// <summary>
	/// DNS client.
	/// </summary>
	/// <example>
	/// <code>
	/// // Set dns servers
	/// Dns_Client.DnsServers = new string[]{"194.126.115.18"};
	/// 
	/// Dns_Client dns = Dns_Client();
	/// 
	/// // Get MX records.
	/// DnsServerResponse resp = dns.Query("lumisoft.ee",QTYPE.MX);
	/// if(resp.ConnectionOk &amp;&amp; resp.ResponseCode == RCODE.NO_ERROR){
	///		MX_Record[] mxRecords = resp.GetMXRecords();
	///		
	///		// Do your stuff
	///	}
	///	else{
	///		// Handle error there, for more exact error info see RCODE 
	///	}	 
	/// 
	/// </code>
	/// </example>
	public class Dns_Client : IDisposable
    {                
        private static IPAddress[] m_DnsServers  = null;
		private static bool        m_UseDnsCache = true;
        // 
        private bool                                  m_IsDisposed    = false;
        private Dictionary<int,DNS_ClientTransaction> m_pTransactions = null;
        private Socket                                m_pIPv4Socket   = null;
        private Socket                                m_pIPv6Socket   = null;
        private List<UDP_DataReceiver>                m_pReceivers    = null;

		/// <summary>
		/// Static constructor.
		/// </summary>
		static Dns_Client()
		{
			// Try to get default NIC dns servers.
			try{
				List<IPAddress> dnsServers = new List<IPAddress>();
                foreach(NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces()){
                    if(nic.OperationalStatus == OperationalStatus.Up){
                        foreach(IPAddress ip in nic.GetIPProperties().DnsAddresses){
                            if(ip.AddressFamily == AddressFamily.InterNetwork){
                                if(!dnsServers.Contains(ip)){
                                    dnsServers.Add(ip);
                                }
                            }
                        }

                        break;
                    }
                }

                m_DnsServers = dnsServers.ToArray();
			}
			catch{
			}
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Dns_Client()
		{
            m_pTransactions = new Dictionary<int,DNS_ClientTransaction>();

            m_pIPv4Socket = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
            m_pIPv4Socket.Bind(new IPEndPoint(IPAddress.Any,0));

            if(Socket.OSSupportsIPv6){
                m_pIPv6Socket = new Socket(AddressFamily.InterNetworkV6,SocketType.Dgram,ProtocolType.Udp);
                m_pIPv6Socket.Bind(new IPEndPoint(IPAddress.IPv6Any,0));
            }

            m_pReceivers = new List<UDP_DataReceiver>();

            // Create UDP data receivers.
            for(int i=0;i<5;i++){
                UDP_DataReceiver ipv4Receiver = new UDP_DataReceiver(m_pIPv4Socket);
                ipv4Receiver.PacketReceived += delegate(object s1,UDP_e_PacketReceived e1){
                    ProcessUdpPacket(e1);
                };
                m_pReceivers.Add(ipv4Receiver);
                ipv4Receiver.Start();

                if(m_pIPv6Socket != null){
                    UDP_DataReceiver ipv6Receiver = new UDP_DataReceiver(m_pIPv6Socket);
                    ipv6Receiver.PacketReceived += delegate(object s1,UDP_e_PacketReceived e1){
                        ProcessUdpPacket(e1);
                    };
                    m_pReceivers.Add(ipv6Receiver);
                    ipv6Receiver.Start();
                }
            }
        }

        #region method Dispose

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        public void Dispose()
        {
            if(m_IsDisposed){
                return;
            }
            m_IsDisposed = true;

            m_pIPv4Socket.Close();
            m_pIPv4Socket = null;

            if(m_pIPv6Socket != null){
                m_pIPv6Socket.Close();
                m_pIPv6Socket = null;
            }

            m_pTransactions = null;
            m_pReceivers = null;
        }

        #endregion


        #region method CreateTransaction

        /// <summary>
        /// Creates new DNS client transaction.
        /// </summary>
        /// <param name="queryType">Query type.</param>
        /// <param name="queryText">Query text. It depends on queryType.</param>
        /// <param name="timeout">Transaction timeout in milliseconds. DNS default value is 2000, value 0 means no timeout - this is not suggested.</param>
        /// <returns>Returns DNS client transaction.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>queryText</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        /// <remarks>Creates asynchronous(non-blocking) DNS transaction. Call <see cref="DNS_ClientTransaction.Start"/> to start transaction.
        /// It is allowd to create multiple conccurent transactions.</remarks>
        public DNS_ClientTransaction CreateTransaction(DNS_QType queryType,string queryText,int timeout)
        {            
            if(queryType == DNS_QType.PTR){
                IPAddress ip = null;
                if(!IPAddress.TryParse(queryText,out ip)){
                    throw new ArgumentException("Argument 'queryText' value must be IP address if queryType == DNS_QType.PTR.","queryText");
                }
            }
            if(queryText == null){
                throw new ArgumentNullException("queryText");
            }
            if(queryText == string.Empty){
                throw new ArgumentException("Argument 'queryText' value may not be \"\".","queryText");
            }

            if(queryType == DNS_QType.PTR){
				string ip = queryText;

				// See if IP is ok.
				IPAddress ipA = IPAddress.Parse(ip);		
				queryText = "";

				// IPv6
				if(ipA.AddressFamily == AddressFamily.InterNetworkV6){
					// 4321:0:1:2:3:4:567:89ab
					// would be
					// b.a.9.8.7.6.5.0.4.0.0.0.3.0.0.0.2.0.0.0.1.0.0.0.0.0.0.0.1.2.3.4.IP6.ARPA
					
					char[] ipChars = ip.Replace(":","").ToCharArray();
					for(int i=ipChars.Length - 1;i>-1;i--){
						queryText += ipChars[i] + ".";
					}
					queryText += "IP6.ARPA";
				}
				// IPv4
				else{
					// 213.35.221.186
					// would be
					// 186.221.35.213.in-addr.arpa

					string[] ipParts = ip.Split('.');
					//--- Reverse IP ----------
					for(int i=3;i>-1;i--){
						queryText += ipParts[i] + ".";
					}
					queryText += "in-addr.arpa";
				}
			}

            DNS_ClientTransaction retVal = new DNS_ClientTransaction(this,new Random().Next(0xFFFF),queryType,queryText,timeout);
            retVal.StateChanged += delegate(object s1,EventArgs<DNS_ClientTransaction> e1){
                if(retVal.State == DNS_ClientTransactionState.Completed){
                    m_pTransactions.Remove(e1.Value.ID);
                }
            };
            m_pTransactions.Add(retVal.ID,retVal);

            return retVal;
        }

        #endregion

        #region method Query

        /// <summary>
		/// Queries server with specified query.
		/// </summary>
		/// <param name="queryText">Query text. It depends on queryType.</param>
		/// <param name="queryType">Query type.</param>
		/// <returns>Returns DSN server response.</returns>
		public DnsServerResponse Query(string queryText,DNS_QType queryType)
		{
            return Query(queryText,queryType,2000);
        }

		/// <summary>
		/// Queries server with specified query.
		/// </summary>
		/// <param name="queryText">Query text. It depends on queryType.</param>
		/// <param name="queryType">Query type.</param>
        /// <param name="timeout">Query timeout in milli seconds.</param>
		/// <returns>Returns DSN server response.</returns>
		public DnsServerResponse Query(string queryText,DNS_QType queryType,int timeout)
		{
            DnsServerResponse retVal = null;
            ManualResetEvent  wait   = new ManualResetEvent(false);

            DNS_ClientTransaction transaction = CreateTransaction(queryType,queryText,timeout);            
            transaction.Timeout += delegate(object s,EventArgs e){
                wait.Set();
            };
            transaction.StateChanged += delegate(object s1,EventArgs<DNS_ClientTransaction> e1){
                if(transaction.State == DNS_ClientTransactionState.Completed || transaction.State == DNS_ClientTransactionState.Disposed){ 
                    retVal = transaction.Response;

                    wait.Set();
                }
            };
            transaction.Start();

            // Wait transaction to complete.
            wait.WaitOne();

            return retVal;
		}

		#endregion
        
        #region method GetHostAddresses

        /// <summary>
        /// Gets specified host IP addresses(A and AAAA).
        /// </summary>
        /// <param name="host">Host name.</param>
        /// <returns>Returns specified host IP addresses.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>host</b> is null reference.</exception>
        public IPAddress[] GetHostAddresses(string host)
        {
            if(host == null){
                throw new ArgumentNullException("host");
            }

            List<IPAddress> retVal = new List<IPAddress>();

            // This is probably NetBios name
			if(host.IndexOf(".") == -1){
				return System.Net.Dns.GetHostEntry(host).AddressList;
			}
            else{
                DnsServerResponse response = Query(host,DNS_QType.A);
                if(response.ResponseCode != DNS_RCode.NO_ERROR){
                    throw new DNS_ClientException(response.ResponseCode);
                }

                foreach(DNS_rr_A record in response.GetARecords()){
                    retVal.Add(record.IP);
                }

                response = Query(host,DNS_QType.AAAA);
                if(response.ResponseCode != DNS_RCode.NO_ERROR){
                    throw new DNS_ClientException(response.ResponseCode);
                }

                foreach(DNS_rr_AAAA record in response.GetAAAARecords()){
                    retVal.Add(record.IP);
                }
            }

            return retVal.ToArray();
        }

        #endregion


        #region static method Resolve

        /// <summary>
        /// Resolves host names to IP addresses.
        /// </summary>
        /// <param name="hosts">Host names to resolve.</param>
        /// <returns>Returns specified hosts IP addresses.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>hosts</b> is null.</exception>
        public static IPAddress[] Resolve(string[] hosts)
        {
            if(hosts == null){
                throw new ArgumentNullException("hosts");
            }

            List<IPAddress> retVal = new List<IPAddress>();
            foreach(string host in hosts){
                IPAddress[] addresses = Resolve(host);
                foreach(IPAddress ip in addresses){
                    if(!retVal.Contains(ip)){
                        retVal.Add(ip);
                    }
                }
            }

            return retVal.ToArray();
        }

		/// <summary>
		/// Resolves host name to IP addresses.
		/// </summary>
		/// <param name="host">Host name or IP address.</param>
		/// <returns>Return specified host IP addresses.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>host</b> is null.</exception>
		public static IPAddress[] Resolve(string host)
		{
            if(host == null){
                throw new ArgumentNullException("host");
            }

			// If hostName_IP is IP
			try{
				return new IPAddress[]{IPAddress.Parse(host)};
			}
			catch{
			}

			// This is probably NetBios name
			if(host.IndexOf(".") == -1){
				return System.Net.Dns.GetHostEntry(host).AddressList;
			}
			else{
				// hostName_IP must be host name, try to resolve it's IP
				using(Dns_Client dns = new Dns_Client()){
				    DnsServerResponse resp = dns.Query(host,DNS_QType.A);
				    if(resp.ResponseCode == DNS_RCode.NO_ERROR){
					    DNS_rr_A[] records = resp.GetARecords();
					    IPAddress[] retVal = new IPAddress[records.Length];
					    for(int i=0;i<records.Length;i++){
						    retVal[i] = records[i].IP;
					    }

					    return retVal;
				    }
				    else{
					    throw new Exception(resp.ResponseCode.ToString());
				    }
                }
			}
		}

		#endregion


        #region method Send

        /// <summary>
        /// Sends specified packet to the specified target IP end point.
        /// </summary>
        /// <param name="target">Target end point.</param>
        /// <param name="packet">Packet to send.</param>
        /// <param name="count">Number of bytes to send from <b>packet</b>.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>target</b> or <b>packet</b> is null reference.</exception>
        internal void Send(IPAddress target,byte[] packet,int count)
        {
            if(target == null){
                throw new ArgumentNullException("target");
            }
            if(packet == null){
                throw new ArgumentNullException("packet");
            }

            try{
                if(target.AddressFamily == AddressFamily.InterNetwork){
                    m_pIPv4Socket.SendTo(packet,count,SocketFlags.None,new IPEndPoint(target,53));
                }
                else if(target.AddressFamily == AddressFamily.InterNetworkV6){
                    m_pIPv6Socket.SendTo(packet,count,SocketFlags.None,new IPEndPoint(target,53));
                }                
            }
            catch{
            }
        }

        #endregion


        #region method ProcessUdpPacket

        /// <summary>
        /// Processes received UDP packet.
        /// </summary>
        /// <param name="e">UDP packet.</param>
        private void ProcessUdpPacket(UDP_e_PacketReceived e)
        {
            try{
                if(m_IsDisposed){
                    return;
                }
                                
                DnsServerResponse serverResponse = ParseQuery(e.Buffer);
                DNS_ClientTransaction transaction = null;
                // Pass response to transaction.
                if(m_pTransactions.TryGetValue(serverResponse.ID,out transaction)){
                    if(transaction.State == DNS_ClientTransactionState.Active){
                        // Cache query.
                        if(m_UseDnsCache && serverResponse.ResponseCode == DNS_RCode.NO_ERROR){
	                        DnsCache.AddToCache(transaction.QName,(int)transaction.QType,serverResponse);
		                }
                        
                        transaction.ProcessResponse(serverResponse);
                    }
                }
                // No such transaction or transaction has timed out before answer received.
                //else{
                //}
            }
            catch{
                // We don't care about receiving errors here, skip them.
            }
        }

        #endregion


        #region method GetQName

        internal static bool GetQName(byte[] reply,ref int offset,ref string name)
		{				
			try{				
				while(true){
                    // Invalid DNS packet, offset goes beyound reply size, probably terminator missing.
                    if(offset >= reply.Length){
                        return false;
                    }
                    // We have label terminator "0".
                    if(reply[offset] == 0){
                        break;
                    }

					// Check if it's pointer(In pointer first two bits always 1)
					bool isPointer = ((reply[offset] & 0xC0) == 0xC0);
					
					// If pointer
					if(isPointer){
						/* Pointer location number is 2 bytes long
						    0 | 1 | 2 | 3 | 4 | 5 | 6 | 7  # byte 2 # 0 | 1 | 2 | | 3 | 4 | 5 | 6 | 7
						    empty | < ---- pointer location number --------------------------------->
                        */
						int pStart = ((reply[offset] & 0x3F) << 8) | (reply[++offset]);
						offset++;
			
						return GetQName(reply,ref pStart,ref name);
					}
					else{
						/* Label length (length = 8Bit and first 2 bits always 0)
						    0 | 1 | 2 | 3 | 4 | 5 | 6 | 7
						    empty | lablel length in bytes 
                        */
						int labelLength = (reply[offset] & 0x3F);
						offset++;
				
						// Copy label into name 
						name += Encoding.UTF8.GetString(reply,offset,labelLength);
						offset += labelLength;
					}
									
					// If the next char isn't terminator, label continues - add dot between two labels.
					if (reply[offset] != 0){
						name += ".";
					}					
				}

				// Move offset by terminator length.
				offset++;

				return true;
			}
			catch{
				return false;
			}
		}

		#endregion

		#region method ParseQuery

		/// <summary>
		/// Parses query.
		/// </summary>
		/// <param name="reply">Dns server reply.</param>
		/// <returns></returns>
		private DnsServerResponse ParseQuery(byte[] reply)
		{	
			//--- Parse headers ------------------------------------//

			/* RFC 1035 4.1.1. Header section format
			 
											1  1  1  1  1  1
			  0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
			 +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			 |                      ID                       |
			 +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			 |QR|   Opcode  |AA|TC|RD|RA|   Z    |   RCODE   |
			 +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			 |                    QDCOUNT                    |
			 +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			 |                    ANCOUNT                    |
			 +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			 |                    NSCOUNT                    |
			 +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			 |                    ARCOUNT                    |
			 +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			 
			QDCOUNT
				an unsigned 16 bit integer specifying the number of
				entries in the question section.

			ANCOUNT
				an unsigned 16 bit integer specifying the number of
				resource records in the answer section.
				
			NSCOUNT
			    an unsigned 16 bit integer specifying the number of name
                server resource records in the authority records section.

			ARCOUNT
			    an unsigned 16 bit integer specifying the number of
                resource records in the additional records section.
				
			*/
		
			// Get reply code
			int       id                     = (reply[0]  << 8 | reply[1]);
			OPCODE    opcode                 = (OPCODE)((reply[2] >> 3) & 15);
			DNS_RCode replyCode              = (DNS_RCode)(reply[3]  & 15);	
			int       queryCount             = (reply[4]  << 8 | reply[5]);
			int       answerCount            = (reply[6]  << 8 | reply[7]);
			int       authoritiveAnswerCount = (reply[8]  << 8 | reply[9]);
			int       additionalAnswerCount  = (reply[10] << 8 | reply[11]);
			//---- End of headers ---------------------------------//
		
			int pos = 12;

			//----- Parse question part ------------//
			for(int q=0;q<queryCount;q++){
				string dummy = "";
				GetQName(reply,ref pos,ref dummy);
				//qtype + qclass
				pos += 4;
			}
			//--------------------------------------//

			// 1) parse answers
			// 2) parse authoritive answers
			// 3) parse additional answers
			List<DNS_rr> answers = ParseAnswers(reply,answerCount,ref pos);
			List<DNS_rr> authoritiveAnswers = ParseAnswers(reply,authoritiveAnswerCount,ref pos);
			List<DNS_rr> additionalAnswers = ParseAnswers(reply,additionalAnswerCount,ref pos);

			return new DnsServerResponse(true,id,replyCode,answers,authoritiveAnswers,additionalAnswers);
		}

		#endregion

		#region method ParseAnswers

		/// <summary>
		/// Parses specified count of answers from query.
		/// </summary>
		/// <param name="reply">Server returned query.</param>
		/// <param name="answerCount">Number of answers to parse.</param>
		/// <param name="offset">Position from where to start parsing answers.</param>
		/// <returns></returns>
		private List<DNS_rr> ParseAnswers(byte[] reply,int answerCount,ref int offset)
		{
			/* RFC 1035 4.1.3. Resource record format
			 
										   1  1  1  1  1  1
			 0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			|                                               |
			/                                               /
			/                      NAME                     /
			|                                               |
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			|                      TYPE                     |
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			|                     CLASS                     |
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			|                      TTL                      |
			|                                               |
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			|                   RDLENGTH                    |
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--|
			/                     RDATA                     /
			/                                               /
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			*/

			List<DNS_rr> answers = new List<DNS_rr>();
			//---- Start parsing answers ------------------------------------------------------------------//
			for(int i=0;i<answerCount;i++){        
				string name = "";
				if(!GetQName(reply,ref offset,ref name)){
					break;
				}

				int type     = reply[offset++] << 8  | reply[offset++];
				int rdClass  = reply[offset++] << 8  | reply[offset++];
				int ttl      = reply[offset++] << 24 | reply[offset++] << 16 | reply[offset++] << 8  | reply[offset++];
				int rdLength = reply[offset++] << 8  | reply[offset++];
                				
                if((DNS_QType)type == DNS_QType.A){
                    answers.Add(DNS_rr_A.Parse(name,reply,ref offset,rdLength,ttl));
                }
                else if((DNS_QType)type == DNS_QType.NS){
                    answers.Add(DNS_rr_NS.Parse(name,reply,ref offset,rdLength,ttl));
                }
                else if((DNS_QType)type == DNS_QType.CNAME){
                    answers.Add(DNS_rr_CNAME.Parse(name,reply,ref offset,rdLength,ttl));
                }
                else if((DNS_QType)type == DNS_QType.SOA){
                    answers.Add(DNS_rr_SOA.Parse(name,reply,ref offset,rdLength,ttl));
                }
                else if((DNS_QType)type == DNS_QType.PTR){
                    answers.Add(DNS_rr_PTR.Parse(name,reply,ref offset,rdLength,ttl));
                }
                else if((DNS_QType)type == DNS_QType.HINFO){
                    answers.Add(DNS_rr_HINFO.Parse(name,reply,ref offset,rdLength,ttl));
                }
                else if((DNS_QType)type == DNS_QType.MX){
                    answers.Add(DNS_rr_MX.Parse(name,reply,ref offset,rdLength,ttl));
                }
                else if((DNS_QType)type == DNS_QType.TXT){
                    answers.Add(DNS_rr_TXT.Parse(name,reply,ref offset,rdLength,ttl));
                }
                else if((DNS_QType)type == DNS_QType.AAAA){
                    answers.Add(DNS_rr_AAAA.Parse(name,reply,ref offset,rdLength,ttl));
                }
                else if((DNS_QType)type == DNS_QType.SRV){
                    answers.Add(DNS_rr_SRV.Parse(name,reply,ref offset,rdLength,ttl));
                }
                else if((DNS_QType)type == DNS_QType.NAPTR){
                    answers.Add(DNS_rr_NAPTR.Parse(name,reply,ref offset,rdLength,ttl));
                }
                else if((DNS_QType)type == DNS_QType.SPF){
                    answers.Add(DNS_rr_SPF.Parse(name,reply,ref offset,rdLength,ttl));
                }
                else{
                    // Unknown record, skip it.
                    offset += rdLength;
                }
			}

			return answers;
		}

		#endregion

        #region method ReadCharacterString

        /// <summary>
        /// Reads character-string from spefcified data and offset.
        /// </summary>
        /// <param name="data">Data from where to read.</param>
        /// <param name="offset">Offset from where to start reading.</param>
        /// <returns>Returns readed string.</returns>
        internal static string ReadCharacterString(byte[] data,ref int offset)
        {
            /* RFC 1035 3.3.
                <character-string> is a single length octet followed by that number of characters. 
                <character-string> is treated as binary information, and can be up to 256 characters 
                in length (including the length octet).
            */

            int dataLength = (int)data[offset++];
            string retVal = Encoding.Default.GetString(data,offset,dataLength);
            offset += dataLength;

            return retVal;
        }

        #endregion


        #region Properties Implementation

        /// <summary>
		/// Gets or sets dns servers.
		/// </summary>
        /// <exception cref="ArgumentNullException">Is raised when null value is passed.</exception>
		public static string[] DnsServers
		{
			get{
                string[] retVal = new string[m_DnsServers.Length];
                for(int i=0;i<m_DnsServers.Length;i++){
                    retVal[i] = m_DnsServers[i].ToString();
                }

                return retVal; 
            }

			set{
                if(value == null){
                    throw new ArgumentNullException();
                }

                IPAddress[] retVal = new IPAddress[value.Length];
                for(int i=0;i<value.Length;i++){
                    retVal[i] = IPAddress.Parse(value[i]);
                }

                m_DnsServers = retVal; 
            }
		}

		/// <summary>
		/// Gets or sets if to use dns caching.
		/// </summary>
		public static bool UseDnsCache
		{
			get{ return m_UseDnsCache; }

			set{ m_UseDnsCache = value; }
		}

		#endregion

	}
}
