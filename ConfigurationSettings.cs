using System;
using System.Collections;
using System.Collections.Specialized;


namespace SmartDeviceUtilities.ConfigurationManagement
{
	#region ConfigurationSettings class Definition

	/// <summary>
	/// Provides access to configuration settings in a specified configuration section. 
	/// This class cannot be inherited or instantiated.
	/// </summary>
	public sealed class ConfigurationSettings
	{

		#region Private static data members

		/// <summary>
		/// Stores the configuration settings
		/// </summary>
		private static Hashtable internalConfigSectionsStore = new Hashtable();

		/// <summary>
		/// Thread safe wrapper for internalConfigSectionsStore
		/// </summary>
		private static Hashtable configSectionsStore = Hashtable.Synchronized(internalConfigSectionsStore);

		/// <summary>
		/// Stores the application settings 
		/// </summary>
		private static ReadOnlyNameValueCollection appSettingsStore;
		
		#endregion

		#region Constructor 

		/// <summary>
		/// Private constructor to prevent instatiation
		/// </summary>
		private ConfigurationSettings()
		{
		}

		#endregion

		#region Public AppSettings property

		/// <summary>
		/// Allows the user to access the key-value pairs of the appSettings section
		/// </summary>
		public static NameValueCollection AppSettings
		{
			get
			{
				return (appSettingsStore);
			}
		}

		#endregion

		#region Internal AddAppSettings Method

		/// <summary>
		/// Adds the application settings and marks the collection as read only
		/// </summary>
		/// <param name="collection"></param>
		internal static void AddAppSettings(ReadOnlyNameValueCollection collection)
		{
			appSettingsStore = collection;
			appSettingsStore.SetReadOnly();
		}
		
		#endregion

		#region Public GetConfig Method

		/// <summary>
		/// Returns configuration settings for a user-defined configuration section.
		/// </summary>
		/// 
		/// <param name="sectionName">The configuration section to read.</param>
		/// 
		/// <returns>The configuration settings for sectionName.</returns>
		public static object GetConfig(String sectionName)
		{
			if (!ConfigurationSectionHandlerActivator.IsActivated)
				ConfigurationSectionHandlerActivator.ReadConfigurationSettings();
			

			if (configSectionsStore.ContainsKey(sectionName))
                return configSectionsStore[sectionName];
			else
				return null;
		}

		#endregion

		#region AddConfigurationSettings

		/// <summary>
		/// Adds the configuration object to a hashtable with the sectionName as key.
		/// </summary>
		/// 
		/// <param name="sectionName">Name of the configuration section</param>
		/// 
		/// <param name="configObject">Configuration object</param>
		internal static void AddConfigurationSettings(String sectionName, Object configObject)
		{
			if (!configSectionsStore.ContainsKey(sectionName))
				configSectionsStore.Add(sectionName, configObject);
		}

		#endregion
	}

	#endregion
}
