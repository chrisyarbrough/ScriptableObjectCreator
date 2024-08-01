namespace Xarbrough.ScriptableObjectCreator
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.UIElements;

	internal abstract class ListViewMode : ViewMode
	{
		protected ListViewMode(string name) : base(name)
		{
		}

		public sealed override void CreateGUI(VisualElement root)
		{
			CreateFilters(root);
			SetupFilterFields(root);
			ScriptRepository.Filter.Changed = RefreshView;
			CreateView(root);
			RefreshView();
		}

		protected abstract void CreateView(VisualElement root);

		protected abstract void SetupFilterFields(VisualElement root);

		protected static void CreateFilters(VisualElement root)
		{
			var filtersLabel = new Label { text = "Filters" };
			filtersLabel.AddToClassList("header");
			root.Add(filtersLabel);
		}

		protected abstract void RefreshView();

		protected void OnListViewSelectionChanged(IEnumerable<object> items)
		{
			var script = items.FirstOrDefault() as MonoScript;
			OnScriptSelected(script);
		}

		protected void BindItem(VisualElement visualElement, int index)
		{
			var listView = visualElement.GetFirstAncestorOfType<ListView>();
			List<MonoScript> scripts = listView.itemsSource as List<MonoScript>;
			MonoScript script = scripts[index];

			// This is why we store the MonoScript, because Unity associates the icon with the script.
			Texture2D icon = AssetPreview.GetMiniThumbnail(script);
			visualElement.Q("Icon").style.backgroundImage = icon;

			BindLabels(visualElement, script);
		}

		protected abstract void BindLabels(VisualElement visualElement, MonoScript script);
	}
}