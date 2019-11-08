using System;
using System.Collections.Specialized;

namespace SmartDeviceUtilities.ConfigurationManagement
{
	#region ReadOnlyNameValueCollection class definition

	/// <summary>
	/// Provides read only version NameValueCollection.
	/// </summary>
	internal class ReadOnlyNameValueCollection : NameValueCollection
	{
		#region Constructor

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ReadOnlyNameValueCollection() : base()
		{
		}

		#endregion

		#region Public SetReadOnly Method

		/// <summary>
		/// Marks the collection as "ReadOnly"
		/// </summary>
		public void SetReadOnly()
		{
			base.IsReadOnly = true;
		}

		#endregion
	}

	#endregion
}
