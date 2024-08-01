namespace Xarbrough.ScriptableObjectCreator
{
	using UnityEditor;

	/// <summary>
	/// Adds a context menu to script assets to create an instance of the ScriptableObject class type.
	/// <br/>
	/// <list type="number">
	/// <listheader>
	/// <term>To use the menu:</term>
	/// </listheader>
	/// <item>Select a <i>MonoScript</i> asset in the <i>Project</i> window.</item>
	/// <item>
	/// In the <i>Inspector</i>, navigate to the <i>Imported Object</i> section
	/// (the second header that says <i>ScriptName (Mono Script)</i>.
	/// </item>
	/// <item>Context-click this header area or use the burger menu.</item>
	/// </list>
	/// </summary>
	internal static class MonoScriptContextMenuExtension
	{
		private const string monoScriptContextItem = "CONTEXT/MonoScript/Create Asset";

		[MenuItem(monoScriptContextItem)]
		private static void CreateAsset(MenuCommand command)
		{
			var script = (MonoScript)command.context;
			ScriptRepository.CreateAsset(script);
		}

		[MenuItem(monoScriptContextItem, isValidateFunction: true)]
		private static bool CreateAsset_Validate(MenuCommand command)
		{
			var script = (MonoScript)command.context;
			return ScriptRepository.CanBeSavedToProject(script.GetClass());
		}
	}
}