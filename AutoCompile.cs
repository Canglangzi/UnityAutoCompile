using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.Compilation;
using System.Net;

namespace Editor
{
	[InitializeOnLoad]
	public static class AutoCompile
	{
		private static HttpListener listener;
		private static bool needUpdate;
		private static string port = "10245";
		static AutoCompile()
		{
			needUpdate = false;
			CompilationPipeline.compilationStarted += OnCompilationStarted;
			// CompilationPipeline.compilationFinished += OnCompilationFinished;
			EditorApplication.quitting += _closeListener;
			EditorApplication.update += onUpdate;
			_createListener();
		}

		private static void _createListener()
		{
			if (listener != null)
			{
				return;
			};
			try
			{
				listener = new HttpListener();
				listener.Prefixes.Add("http://127.0.0.1:" + port + "/refresh/");
				listener.Start();
				listener.BeginGetContext(new AsyncCallback(OnRequest), listener);
				// Debug.Log("Auto Compilation HTTP server started");
			}
			catch (Exception e)
			{
				Debug.Log("Auto Compilation starting failed:" + e);
				throw;
			}

		}
		private static void OnRequest(IAsyncResult result)
		{

			if (listener.IsListening && !EditorApplication.isCompiling)
			{
				listener.EndGetContext(result);
				// var context = listener.EndGetContext(result);
				// var request = context.Request;
				needUpdate = true;
				listener.BeginGetContext(new AsyncCallback(OnRequest), listener);
			}
		}
		private static void _closeListener()
		{
			Debug.Log("Closing Listener");
			if(listener==null)return;
			listener.Stop();
			listener.Close();
			listener=null;
		}
		private static void onUpdate()
		{
			if (!EditorApplication.isCompiling && !EditorApplication.isUpdating && needUpdate)
			{
				needUpdate = false;
				//    Debug.Log("Compiled in background");
				AssetDatabase.Refresh();
			}
		}
		// [MenuItem("Tools/Stop Auto Compilation")]
		// public static void StopAutoCompilation()=>_closeListener();

		// [MenuItem("Tools/Start Auto Compilation")]
		// public static void StartAutoCompilation()=>_createListener();
		private static void OnCompilationStarted(object _) => _closeListener();
		// private static void OnCompilationFinished(object _) => _createListener();
	}
}

//TODO : if there is error,the auto compile will stop
//TODO : this should be a package to avoid compile