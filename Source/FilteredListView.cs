namespace Xarbrough.ScriptableObjectCreator
{
	using UnityEditor;
	using UnityEngine.UIElements;

	internal sealed class FilteredListView : ListViewMode
	{
		private ListView listView;

		public FilteredListView() : base("Filtered List View")
		{
		}

		protected override void SetupFilterFields(VisualElement root)
		{
			VisualElement namespaceField = ScriptRepository.Filter.NamespaceField;
			VisualElement classNameField = ScriptRepository.Filter.ClassNameField;
			namespaceField.style.width = new StyleLength(StyleKeyword.Initial);
			classNameField.style.width = new StyleLength(StyleKeyword.Initial);
			root.Add(namespaceField);
			root.Add(classNameField);
		}

		protected override void CreateView(VisualElement root)
		{
			listView = new ListView(ScriptRepository.OriginalItems, itemHeight: 21f, MakeItem, BindItem);
			listView.style.flexGrow = 1;
			listView.selectionType = SelectionType.Single;
			listView.onSelectionChange += OnListViewSelectionChanged;
			listView.Q<ScrollView>().name = "MainScrollView";
			root.Add(listView);
		}

		protected override void RefreshView()
		{
			listView.itemsSource = ScriptRepository.FilteredItems;
			listView.Rebuild();
		}

		private VisualElement MakeItem()
		{
			var row = new VisualElement();

			var icon = new VisualElement { name = "Icon" };
			icon.AddToClassList("icon");

			row.Add(icon);
			row.Add(new Label());

			return row;
		}

		protected override void BindLabels(VisualElement visualElement, MonoScript script)
		{
			var label = visualElement.Q<Label>();
			label.text = script.GetClass().FullName;
			label.tooltip = script.GetClass().Assembly.FullName;
		}
	}
}