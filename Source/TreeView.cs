namespace Xarbrough.ScriptableObjectCreator
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEditor.IMGUI.Controls;
	using UnityEngine;
	using UnityEngine.UIElements;

	internal sealed class TreeView : ViewMode
	{
		public TreeView() : base("Tree View")
		{
		}

		public override void CreateGUI(VisualElement root)
		{
			// Group all types by their namespaces and draw a foldout for each level:
			var treeView = new ScriptableObjectTreeView(new TreeViewState(), ScriptRepository.OriginalItems);
			treeView.ScriptSelected += OnScriptSelected;
			var container = new IMGUIContainer(() => treeView.OnGUI(new Rect(0, 0, root.worldBound.width, root.worldBound.height)));
			container.style.flexGrow = 1f;
			root.Add(container);
		}

		internal class ScriptableObjectTreeView : UnityEditor.IMGUI.Controls.TreeView
		{
			public Action<MonoScript> ScriptSelected;

			private List<MonoScript> scripts;

			public ScriptableObjectTreeView(TreeViewState state, List<MonoScript> scripts) : base(state)
			{
				this.scripts = scripts;
				showBorder = true;
				Reload();
			}

			protected override TreeViewItem BuildRoot()
			{
				var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
				var allItems = new List<TreeViewItem>();

				var noNamespace = new TreeViewItem
					{ id = "Global".GetHashCode(), depth = 0, displayName = "Global" };

				for (int scriptIndex = 0; scriptIndex < scripts.Count; scriptIndex++)
				{
					MonoScript script = scripts[scriptIndex];
					string[] path = script.GetClass().FullName.Split('.');

					// No namespace:
					if (path.Length == 1)
					{
						noNamespace.AddChild(new TreeViewItem
							{ id = scriptIndex, depth = 1, displayName = script.name });
						continue;
					}

					TreeViewItem parent = root;

					for (var i = 0; i < path.Length; i++)
					{
						string name = path[i];
						string id = string.Join(".", path.Take(i + 1));
						TreeViewItem item = allItems.FirstOrDefault(x => x.id == id.GetHashCode());

						if (item == null)
						{
							int itemId = i == path.Length - 1 ? scriptIndex : id.GetHashCode();
							item = new TreeViewItem { id = itemId, depth = i, displayName = name };
							allItems.Add(item);
							parent.AddChild(item);
						}

						parent = item;
					}
				}

				if (noNamespace.hasChildren)
					root.AddChild(noNamespace);

				return root;
			}

			protected override void SelectionChanged(IList<int> selectedIds)
			{
				int id = selectedIds.FirstOrDefault();
				if (id >= 0 && id < scripts.Count)
					ScriptSelected.Invoke(scripts[id]);
			}
		}
	}
}