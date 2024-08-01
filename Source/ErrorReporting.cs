namespace Xarbrough.ScriptableObjectCreator
{
	using System;
	using UnityEngine;

	internal static class ErrorReporting
	{
		public static string GetMessage(string title, string additionalInfo, int errorCode)
		{
			return $"[ScriptableObjectCreator] {title} Error Code: {errorCode}\n" +
			       $"{additionalInfo}\n" +
			       "Please report this issue: " +
			       "<a href=\"https://github.com/chrisyarbrough/ScriptableObjectCreator/issues/new" +
			       $"?title=Error{errorCode}%20{Uri.EscapeDataString(title)}" +
			       $"&body=Unity{Application.unityVersion}" +
			       "\">GitHub Issues</a>\n";
		}
	}
}