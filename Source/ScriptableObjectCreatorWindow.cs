namespace Xarbrough.ScriptableObjectCreator
{
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.UIElements;

	internal class ScriptableObjectCreatorWindow : EditorWindow
	{
		[MenuItem("Assets/Create/ScriptableObject... (Creator)", priority = 100)]
		public static void ShowWindow()
		{
			GetWindow<ScriptableObjectCreatorWindow>(utility: true, title: "ScriptableObject Creator");
		}

		[SerializeField]
		private StyleSheet styleSheet;

		[SerializeField]
		private ScriptRepository scripts = new();

		[SerializeField]
		private int currentViewMode = -1;

		private ViewMode[] viewModes =
		{
			new FilteredListView(),
			new GroupedListView(),
			new TreeView(),
		};

		private void OnEnable()
		{
			scripts.Load();

			foreach (ViewMode mode in viewModes)
			{
				mode.ScriptSelected += CreateAsset;
				mode.ScriptRepository = scripts;
			}

			if (currentViewMode == -1)
				currentViewMode = Preferences.DefaultViewMode;

			var viewModeContentContainer = new VisualElement();
			viewModeContentContainer.style.flexGrow = 1f;

			var viewModeDropdown = new DropdownField(viewModes.Select(x => x.Name).ToList(), currentViewMode);
			viewModeDropdown.RegisterValueChangedCallback(_ =>
			{
				currentViewMode = viewModeDropdown.index;
				Preferences.DefaultViewMode = currentViewMode;
				viewModeContentContainer.Clear();
				viewModes[currentViewMode].CreateGUI(viewModeContentContainer);
			});
			viewModes[viewModeDropdown.index].CreateGUI(viewModeContentContainer);
			rootVisualElement.Add(viewModeDropdown);

			rootVisualElement.Add(viewModeContentContainer);


			if (styleSheet != null)
				rootVisualElement.styleSheets.Add(styleSheet);
		}

		private void OnDisable()
		{
			foreach (ViewMode mode in viewModes)
				mode.ScriptSelected -= CreateAsset;
		}

		private void CreateAsset(MonoScript script)
		{
			ScriptRepository.CreateAsset(script);

			if (Preferences.CloseAfterCreation)
				Close();
		}
	}
}