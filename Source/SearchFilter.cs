namespace Xarbrough.ScriptableObjectCreator
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.UIElements;

	/// <summary>
	/// The filtering that the user performs on the MonoScript types that can be created.
	/// </summary>
	[Serializable]
	internal sealed class SearchFilter
	{
		public Action Changed;

		public TextField ClassNameField { get; set; }
		public TextField NamespaceField { get; set; }

		[SerializeField]
		private string className = string.Empty;

		[SerializeField]
		private string @namespace;

		public void Initialize()
		{
			// On first load, use the default from the user preferences.
			@namespace ??= Preferences.DefaultNamespaceFilter;

			ClassNameField = new TextField("Class Name");
			ClassNameField.Q<Label>().style.minWidth = 80f;
			ClassNameField.SetValueWithoutNotify(className);
			ClassNameField.RegisterValueChangedCallback(evt =>
			{
				className = evt.newValue;
				Changed?.Invoke();
			});

			NamespaceField = new TextField("Namespace");
			NamespaceField.Q<Label>().style.minWidth = 80f;
			NamespaceField.SetValueWithoutNotify(@namespace);
			NamespaceField.RegisterValueChangedCallback(evt =>
			{
				@namespace = evt.newValue;
				Changed?.Invoke();
			});
		}

		public IEnumerable<MonoScript> Apply(IEnumerable<MonoScript> items)
		{
			// Make the search more convenient by ignoring invalid whitespace and case.
			string classNameFilter = className.Replace(" ", string.Empty);
			string namespaceFilter = @namespace.Replace(" ", string.Empty);


			return items
				.Where(x => ClassNameMatches(x.name, classNameFilter))
				.Where(x =>
				{
					string namespaceName = x.GetClass().Namespace;

					if (!string.IsNullOrEmpty(namespaceName))
						return namespaceName.Contains(namespaceFilter, StringComparison.OrdinalIgnoreCase);
					else if (namespaceFilter.Length == 0)
						return true;
					else
						return false;
				})
				.ToList();
		}

		private bool ClassNameMatches(string className, string filter)
		{
			// Check for exact match
			if (className.Contains(filter, StringComparison.OrdinalIgnoreCase))
				return true;

			// Check for PascalCase match
			string pascalCaseLetters = new string(className.Where(char.IsUpper).ToArray());
			return pascalCaseLetters.Contains(filter, StringComparison.OrdinalIgnoreCase);
		}
	}
}