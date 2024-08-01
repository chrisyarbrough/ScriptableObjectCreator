namespace Xarbrough.ScriptableObjectCreator
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Adds a context menu to object reference properties to create an instance of the ScriptableObject field type.
	/// Instances can either be saved as part of the scene or as standalone assets.
	/// </summary>
	internal static class ContextualPropertyMenuExtension
	{
		[InitializeOnLoadMethod]
		private static void Initialize()
		{
			EditorApplication.contextualPropertyMenu += ContextualPropertyMenu;
		}

		private static void ContextualPropertyMenu(GenericMenu menu, SerializedProperty property)
		{
			if (property.propertyType != SerializedPropertyType.ObjectReference)
				return;

			Type fieldType = GetSerializedPropertyType(property);
			if (ScriptRepository.CanBeSavedToProject(fieldType))
			{
				SerializedProperty copy = property.Copy();
				menu.AddItem(new GUIContent("Create Asset..."), on: false, () =>
				{
					string path = EditorUtility.SaveFilePanelInProject(
						title: "Create ScriptableObject",
						defaultName: fieldType.Name,
						extension: "asset",
						message: "Select a location to save the new asset.");

					if (path.Length > 0)
					{
						ScriptableObject asset = CreateInstance(fieldType, copy);
						AssetDatabase.CreateAsset(asset, path);
					}
				});
				menu.AddItem(new GUIContent("Create Scene Instance"), on: false, () =>
					CreateInstance(fieldType, copy));
			}
		}

		private static ScriptableObject CreateInstance(Type fieldType, SerializedProperty copy)
		{
			var asset = ScriptableObject.CreateInstance(fieldType);
			copy.objectReferenceValue = asset;
			copy.serializedObject.ApplyModifiedProperties();
			return asset;
		}

		public static Type GetSerializedPropertyType(SerializedProperty property)
		{
			// Unity's property.type only returns something like 'PPtr<$MyScriptableObject>'
			// which is not enough to reconstruct the full type information.

			Type type;

			try
			{
				// This is a helpful Unity method that handles nested types and arrays.
				Type scriptAttributeUtilityType =
					typeof(Editor).Assembly.GetType("UnityEditor.ScriptAttributeUtility");

				MethodInfo method = scriptAttributeUtilityType.GetMethod("GetFieldInfoAndStaticTypeFromProperty",
					BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

				object[] arguments = { property, null };
				method.Invoke(null, arguments);

				type = arguments[1] as Type;
			}
			catch
			{
				Debug.LogWarning(ErrorReporting.GetMessage(
					title: "Failed to get field type from SerializedProperty.",
					additionalInfo: "Using backup method.",
					errorCode: 1));

				// In case the method is not available, fallback to a custom version.
				Type parentType = property.serializedObject.targetObject.GetType();
				type = GetFieldFromPropertyPath(parentType, property.propertyPath);
			}

			return type;
		}

		private static Type GetFieldFromPropertyPath(Type parentType, string path)
		{
			FieldInfo field = null;

			// Handle nested types.
			foreach (string component in path.Split('.'))
			{
				field = GetSerializedField(parentType, component);

				if (field == null)
				{
					// Search the type hierarchy upwards for private serialized fields because
					// Type.GetField does not return private fields from base classes.
					for (Type type = parentType; field == null && type != null; type = type.BaseType)
					{
						field = GetSerializedField(type, component);
					}
				}

				if (field != null)
					parentType = field.FieldType;
			}

			// This only handles first-level arrays of the ScriptableObject type, but not nested classes within an array
			// that contain a field of the ScriptableObject type.

			if (field == null)
			{
				if (parentType.IsArray)
					return parentType.GetElementType();
				else if (parentType.IsGenericType && parentType.GetGenericTypeDefinition() == typeof(List<>))
					return parentType.GetGenericArguments()[0];
			}

			return field?.FieldType;
		}

		private static FieldInfo GetSerializedField(Type type, string name)
		{
			return type.GetField(name,
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}
	}
}