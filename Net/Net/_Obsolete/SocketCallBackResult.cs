using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net
{
    /// <summary>
	/// 
	/// </summary>
    [Obsolete("Get rid of it.")]
	public delegate void SocketCallBack(SocketCallBackResult result,long count,Exception x,object tag);

    /// <summary>
	/// Asynchronous command execute result.
	/// </summary>
    [Obsolete("Get rid of it.")]
	public enum SocketCallBackResult
	{
		/// <summary>
		/// Operation was successfull.
		/// </summary>
		Ok,

		/// <summary>
		/// Exceeded maximum allowed size.
		/// </summary>
		LengthExceeded,

		/// <summary>
		/// Connected client closed connection.
		/// </summary>
		SocketClosed,

		/// <summary>
		/// Exception happened.
		/// </summary>
		Exception
	}
}
