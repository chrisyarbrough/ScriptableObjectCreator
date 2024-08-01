namespace Xarbrough.ScriptableObjectCreator
{
	using JetBrains.Annotations;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Encapsulates the collection of MonoScripts that ScriptableObject assets can be created from.
	/// </summary>
	[Serializable]
	internal class ScriptRepository
	{
		public List<MonoScript> OriginalItems { get; } = new();

		public List<MonoScript> FilteredItems => filter.Apply(OriginalItems).ToList();

		public SearchFilter Filter => filter;

		[SerializeField]
		private SearchFilter filter = new SearchFilter();

		public void Load()
		{
			filter.Initialize();

			OriginalItems.Clear();

			OriginalItems.AddRange(
				AssetDatabase.FindAssets("t: MonoScript")
					.Select(AssetDatabase.GUIDToAssetPath)
					.Select(AssetDatabase.LoadAssetAtPath<MonoScript>)
					.Where(script => CanBeSavedToProject(script.GetClass()))
					.OrderBy(script => script.GetClass().FullName));
		}

		public static bool CanBeSavedToProject([CanBeNull] Type type)
		{
			// The type is null when the class name does not match the file name.
			// Such a type cannot be created via ScriptableObject.CreateInstance.\
			return type != null &&
			       type.IsAbstract == false &&
			       type.IsSubclassOf(typeof(ScriptableObject)) &&
			       !type.IsSubclassOf(typeof(Editor)) &&
			       !type.IsSubclassOf(typeof(EditorWindow));
		}

		public static void CreateAsset(MonoScript script)
		{
			Type type = script.GetClass();
			CreateAsset(type);
		}

		public static ScriptableObject CreateAsset(Type type)
		{
			Debug.Assert(type != null, "The UI must ensure only valid script files are selected.");
			var asset = ScriptableObject.CreateInstance(type);
			ProjectWindowUtil.CreateAsset(asset, type.Name + ".asset");
			return asset;
		}
	}
}