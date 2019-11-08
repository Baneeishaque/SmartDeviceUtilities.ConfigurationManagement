using System;
using System.Xml;

namespace SmartDeviceUtilities.ConfigurationManagement
{
	#region IConfigSectionHandler

	/// <summary>
	/// Defines the contract that all smart device configuration section handlers must 
	/// implement in order to participate in the resolution of configuration settings.
	/// </summary>
	public interface IConfigSectionHandler
	{

		#region Create

		/// <summary>
		/// Implemented by all configuration section handlers to 
		/// parse the XML of the configuration section. 
		/// 
		/// The returned object is added to the configuration collection 
		/// and is accessed by ConfigurationManager.GetConfigurationSettings method.
		/// </summary>
		/// 
		/// <param name="sectionNode">
		/// The XmlNode that contains the configuration information from the 
		/// configuration file. 
		/// 
		/// Provides direct access to the XML contents of the configuration section.
		/// </param>
		/// 
		/// <returns>A congifuration objecty</returns>
		object Create(XmlNode sectionNode);

		#endregion
	}

	#endregion
}
