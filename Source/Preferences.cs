namespace Xarbrough.ScriptableObjectCreator
{
	using UnityEditor;

	internal static class Preferences
	{
		public static string DefaultNamespaceFilter
		{
			get => EditorPrefs.GetString("ScriptableObjectCreator.DefaultNamespaceFilter", string.Empty);
			set => EditorPrefs.SetString("ScriptableObjectCreator.DefaultNamespaceFilter", value);
		}

		public static bool CloseAfterCreation
		{
			get => EditorPrefs.GetBool("ScriptableObjectCreator.CloseAfterCreation", true);
			set => EditorPrefs.SetBool("ScriptableObjectCreator.CloseAfterCreation", value);
		}

		public static int DefaultViewMode
		{
			get => EditorPrefs.GetInt("ScriptableObjectCreator.DefaultViewMode", 1);
			set => EditorPrefs.SetInt("ScriptableObjectCreator.DefaultViewMode", value);
		}

		[SettingsProvider]
		public static SettingsProvider GetSettingsProvider()
		{
			return new SettingsProvider("Preferences/ScriptableObject Creator", SettingsScope.User)
			{
				label = "ScriptableObject Creator",
				guiHandler = _ =>
				{
					DefaultNamespaceFilter =
						EditorGUILayout.TextField("Default Namespace Filter", DefaultNamespaceFilter);

					CloseAfterCreation =
						EditorGUILayout.Toggle("Close After Creation", CloseAfterCreation);
				},
				keywords = new[] { "ScriptableObject", "Creator", "Namespace" },
			};
		}
	}
}