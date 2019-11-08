using System;
using System.Xml;
using System.Reflection;
using System.IO;
using System.Collections.Specialized;
using System.Threading;


namespace SmartDeviceUtilities.ConfigurationManagement
{
	#region ConfigurationSectionHandlerActivator

	/// <summary>
	/// Provides access to the configuration settings in a specified 
	/// configuration file.
	/// </summary>
	internal sealed class ConfigurationSectionHandlerActivator
	{
		#region Private data members

		/// <summary>
		/// To check whether the configuration sections are read.
		/// </summary>
		private static Boolean isActivated = false;

		#endregion


		#region Constructor

		/// <summary>
		/// Private constructor to prevent instatiation
		/// </summary>
		private ConfigurationSectionHandlerActivator()
		{
		}

		#endregion


		#region Public properties

		/// <summary>
		/// Gets or Sets isActivated. 
		/// </summary>
		public static Boolean IsActivated
		{
			get
			{
				return isActivated;
			}
			set
			{
				isActivated = value;
			}
		}

		#endregion


		#region ReadConfigurationSettings

		/// <summary>
		/// Provides access to configuration sections and gets the section name and 
		/// type information.
		/// 
		/// Reads the name (which is the name of the element that contains the information the section handler reads)
		/// and type (which is the name of the type that reads the information) attributes of each configuration section. 
		/// 
	 	/// Activates the types specified in the type attribute (said as congiguration object)
		/// and stores the configuration object in a Hashtable with the 
		/// value in the name attribute as the key.
		/// </summary>
		/// 
		/// <remarks>
		/// 
		/// All the exception messages are being hard-coded in the method body itself to keep the 
		/// code simple.
		/// 
		/// Probably you could write your own message management utility and re-write the 
		/// method, so that the messages are getting picked up from the resource file.
		/// 
		/// </remarks>
		public static void ReadConfigurationSettings()
		{
			String sectionName = null;
			String typeName = null;
			String[] typeNames = null;
			Char typeSeparator = Convert.ToChar(",");

			XmlDocument configDocument = null;
			XmlNodeList configNodes = null;
			XmlNodeList sectionNodes = null;

			Assembly sectionHandlerAssembly = null;
			Type sectionHandlerType = null;

			ReadOnlyNameValueCollection appSettingsCollection = new ReadOnlyNameValueCollection();
			XmlNodeList appSettingsNodes = null;

			if (isActivated)
				return;

			try
			{
				isActivated = true;

				//Get the physical path of the executing assembly (excluding the assembly name)
				Char directorySeparator = Convert.ToChar("/");
				Char altDirectorySeparator = Convert.ToChar(@"\");

				String configFileName = Assembly.GetExecutingAssembly().GetName().CodeBase;

				if (configFileName.IndexOf(directorySeparator) > -1)
				{
					configFileName = configFileName.Substring(0, 
						configFileName.LastIndexOf(directorySeparator))
						+ directorySeparator.ToString() + "SmartConfig.xml";;
				}
				else if (configFileName.IndexOf(altDirectorySeparator) > -1)
				{
					configFileName = configFileName.Substring(0, 
						configFileName.LastIndexOf(altDirectorySeparator))
						+ altDirectorySeparator.ToString() + "SmartConfig.xml";;
				}
				else
				{
					throw new System.IO.FileNotFoundException();
				}
				
				//Declare a XML document object and Load the xml file
				configDocument = new XmlDocument();
				configDocument.Load(configFileName);
				configNodes =  configDocument.GetElementsByTagName("configSections");
				
				//return if no <configSections> are declared
				if (configNodes.Count == 0)
					return;

				//Oops!! more than one <configSections> defined
				if (configNodes.Count > 1)
					throw (new Exception("Configuration file cannot contain more than one configSections element"));

				//Get all the section elements
				sectionNodes = configNodes.Item(0).ChildNodes;

				//return if the <configSections> is an empty element
				if (sectionNodes.Count == 0)
					return;


				//iterate through the Nodelist for each section node
				foreach(XmlElement sectionNode in sectionNodes)
				{

					//get the section name
					sectionName = sectionNode.GetAttribute("name");

					//get the type name
					typeName = sectionNode.GetAttribute("type");

					if (sectionName == null)
						throw (new Exception("Invalid configuration section declaration. Section name cannot be null."));

					if (typeName == null)
						throw (new Exception("Invalid configuration section declaration. Type name cannot be null."));

					//get the type name and its encompassing aseembly name
					typeNames = sectionNode.GetAttribute("type").Split(typeSeparator);

					if (typeNames.Length < 2)
						throw (new Exception("Invalid type attribute."));

					//Load the configuration types' encompassing assembly
					sectionHandlerAssembly = Assembly.Load(typeNames[1]);

					//get the System.Type with the specified name
					sectionHandlerType = sectionHandlerAssembly.GetType(typeNames[0], true);

					//iteterate through the System.Type[] array which
					//stores all the implemented interfaces to check whether 
					//the type implements IConfigSectionHandler interface
					foreach(Type interfaceType in sectionHandlerType.GetInterfaces())
					{

						if (interfaceType.Name == "IConfigSectionHandler")
						{
							//instantiate the type to create the configuration object
							Object handlerInstance  = Activator.CreateInstance(sectionHandlerType);

							//get the "Create" method attributes
							MethodInfo methodInfo = sectionHandlerType.GetMethod("Create");

							//define the method parameters
							Object[] methodParams = new Object[1];
							methodParams[0] = configDocument.GetElementsByTagName(sectionName).Item(0);


							//add the configuration object to the hashtable
							ConfigurationSettings.AddConfigurationSettings(sectionName, 
								methodInfo.Invoke(handlerInstance, methodParams));
						}
					}


					//Get the "appSettings"
					appSettingsNodes = configDocument.GetElementsByTagName("appSettings");

					if (appSettingsNodes != null)
					{
						if (appSettingsNodes.Count > 1)
						{
							//Oops!! More than one "<appSettings>" section
							throw new Exception("Configuration file cannot contain more than one appSettings section.");
						}

						//Get the children - (all the <add> nodes)
						XmlNodeList appSettingsChildNodes = appSettingsNodes[0].ChildNodes;

						try
						{
							if (appSettingsChildNodes.Count > 0)
							{
								foreach (XmlNode node in appSettingsChildNodes)
								{
									//Add the key-value pairs to the collection
									appSettingsCollection.Add(node.Attributes["key"].Value, node.Attributes["value"].Value);
								}
							}

							//Put the collection in the ConfigurationSettings class
							ConfigurationSettings.AddAppSettings(appSettingsCollection);
						}

						catch (Exception ex)
						{
							throw new Exception("Error occured while accessing the application settings", ex);
						}
					}
				}
			}
			catch
			{
				throw;

				//Probably you could write your own custom application exception class deriving from 
				//System.ApplicationException and wrap the exception before being rethrown.
			}
			finally
			{
				
			}
		}

		#endregion
	}

	#endregion
}
