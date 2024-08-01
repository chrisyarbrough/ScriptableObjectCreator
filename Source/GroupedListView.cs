namespace Xarbrough.ScriptableObjectCreator
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEngine.UIElements;

	internal sealed class GroupedListView : ListViewMode
	{
		private readonly Dictionary<ListView, List<MonoScript>> listViewScriptMapping = new();

		public GroupedListView() : base("Grouped List View")
		{
		}

		protected override void SetupFilterFields(VisualElement root)
		{
			VisualElement filtersRow = new();
			filtersRow.name = "Filters";
			filtersRow.style.flexDirection = FlexDirection.Row;
			filtersRow.style.flexShrink = 0f;
			filtersRow.AddToClassList("indent");
			filtersRow.style.marginRight = 21f;
			VisualElement namespaceField = ScriptRepository.Filter.NamespaceField;
			VisualElement classNameField = ScriptRepository.Filter.ClassNameField;
			namespaceField.style.width = new Length(50, LengthUnit.Percent);
			classNameField.style.width = new Length(50, LengthUnit.Percent);
			filtersRow.Add(namespaceField);
			filtersRow.Add(classNameField);
			root.Add(filtersRow);
		}

		protected override void CreateView(VisualElement root)
		{
			Dictionary<string, List<MonoScript>> scriptsByNamespaceRoot = new();
			foreach (MonoScript script in ScriptRepository.OriginalItems)
			{
				string namespaceRoot = script.GetClass().Namespace?.Split('.').FirstOrDefault() ?? "Global";
				if (!scriptsByNamespaceRoot.TryGetValue(namespaceRoot, out List<MonoScript> scripts))
				{
					scripts = new List<MonoScript>();
					scriptsByNamespaceRoot.Add(namespaceRoot, scripts);
				}

				scripts.Add(script);
			}

			var scrollView = new ScrollView();
			scrollView.name = "MainScrollView";
			root.Add(scrollView);

			listViewScriptMapping.Clear();

			foreach ((string key, List<MonoScript> scripts) in scriptsByNamespaceRoot)
			{
				var listView = new ListView(scripts, itemHeight: 21f, MakeItem, BindItem);
				listView.name = key;
				listView.style.flexShrink = 0;
				listView.headerTitle = key;
				listView.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
				listView.showBoundCollectionSize = false;
				listView.showFoldoutHeader = true;
				listView.selectionType = SelectionType.Single;
				listView.onSelectionChange += OnListViewSelectionChanged;
				scrollView.Add(listView);

				listViewScriptMapping.Add(listView, scripts);
			}
		}

		protected override void RefreshView()
		{
			foreach ((ListView listView, List<MonoScript> scripts) in listViewScriptMapping)
			{
				listView.itemsSource = ScriptRepository.Filter.Apply(scripts).ToList();
				listView.Rebuild();
				listView.style.display = listView.itemsSource.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;
			}
		}

		private VisualElement MakeItem()
		{
			var row = new VisualElement();
			row.AddToClassList("indent");

			var namespaceLabel = new Label { name = "Namespace" };
			namespaceLabel.style.width = new Length(50, LengthUnit.Percent);
			row.Add(namespaceLabel);

			VisualElement classNameContainer = new();
			classNameContainer.style.flexDirection = FlexDirection.Row;
			classNameContainer.style.width = new Length(50, LengthUnit.Percent);
			var icon = new VisualElement { name = "Icon" };
			icon.AddToClassList("icon");

			classNameContainer.Add(icon);
			classNameContainer.Add(new Label { name = "ClassName" });
			row.Add(classNameContainer);

			return row;
		}

		protected override void BindLabels(VisualElement visualElement, MonoScript script)
		{
			var label = visualElement.Q<Label>("Namespace");
			string namespaceName = script.GetClass().Namespace;
			// Remove the first ns component: "Namespace.SubNamespace.ClassName" -> "SubNamespace.ClassName"
			namespaceName = namespaceName?[(namespaceName.IndexOf('.') + 1)..];
			label.text = namespaceName;
			label.tooltip = script.GetClass().Assembly.FullName;

			label = visualElement.Q<Label>("ClassName");
			label.text = script.GetClass().Name;
			label.tooltip = script.GetClass().Assembly.FullName;
		}
	}
}