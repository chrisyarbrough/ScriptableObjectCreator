// ReSharper disable NotAccessedField.Local

#pragma warning disable CS0414 // Field is assigned but its value is never used

namespace Xarbrough.ScriptableObjectCreator
{
	using NUnit.Framework;
	using System;
	using UnityEditor;
	using UnityEngine;

	public class ContextualPropertyMenuExtensionTests
	{
		[TestCase("myPublicField")]
		[TestCase("myPrivateField")]
		[TestCase("myNestingClassField.myPublicFieldWithinNested")]
		[TestCase("myArrayField.Array.data[0]")]
		[TestCase("myNestedClassArrayField.Array.data[0].myPublicFieldWithinNested")]
		public void ContextualPropertyMenuExtensionTestsSimplePasses(string propertyPath)
		{
			GameObject go = new();
			var script = go.AddComponent<MyMonoBehaviour>();
			SerializedObject serializedObject = new(script);

			Assert.AreEqual(
				typeof(MyScriptableObject),
				ContextualPropertyMenuExtension.GetSerializedPropertyType(serializedObject.FindProperty(propertyPath)));
		}

		private class MyMonoBehaviour : MonoBehaviour
		{
			public MyScriptableObject myPublicField;

			[SerializeField]
			private MyScriptableObject myPrivateField;

			[SerializeField]
			private NestedClass myNestingClassField;

			public MyScriptableObject[] myArrayField = new MyScriptableObject[1];

			[SerializeField]
			private NestedClass[] myNestedClassArrayField = new NestedClass[1];

			[Serializable]
			private class NestedClass
			{
				public MyScriptableObject myPublicFieldWithinNested;
			}
		}

		private class MyScriptableObject : ScriptableObject
		{
		}
	}
}