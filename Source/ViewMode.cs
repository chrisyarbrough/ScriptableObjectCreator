namespace Xarbrough.ScriptableObjectCreator
{
	using System;
	using UnityEditor;
	using UnityEngine.UIElements;

	internal abstract class ViewMode
	{
		public readonly string Name;
		public event Action<MonoScript> ScriptSelected;
		public ScriptRepository ScriptRepository;

		protected ViewMode(string name)
		{
			Name = name;
		}

		public abstract void CreateGUI(VisualElement root);

		protected void OnScriptSelected(MonoScript script)
		{
			ScriptSelected?.Invoke(script);
		}
	}
}