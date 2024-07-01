using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.Compilation;
using System.Net;

namespace Editor
{
    [InitializeOnLoad]
    public static class CLz_AutoCompile
    {
        private static HttpListener listener;
        private static bool needUpdate;
        private static string port = "10245";

        static CLz_AutoCompile()
        {
            needUpdate = false;
            CompilationPipeline.compilationStarted += OnCompilationStarted;
            EditorApplication.quitting += _closeListener;
            EditorApplication.update += onUpdate;
            _createListener();
        }

        private static void _createListener()
        {
            try
            {
                _closeListener(); // Ensure previous listener is closed

                listener = new HttpListener();
                listener.Prefixes.Add("http://127.0.0.1:" + port + "/refresh/");
                listener.Start();
                listener.BeginGetContext(new AsyncCallback(OnRequest), null);
                Debug.Log("自动编译监听器已启动");
            }
            catch (Exception e)
            {
                Debug.LogError("自动编译启动失败: " + e);
                throw;
            }
        }

        private static void OnRequest(IAsyncResult result)
        {
            if (listener == null || !listener.IsListening)
                return;

            HttpListenerContext context = null;

            try
            {
                context = listener.EndGetContext(result);
            }
            catch (HttpListenerException)
            {
                return; // Listener closed or disposed
            }
            catch (ObjectDisposedException)
            {
                return; // Listener was disposed
            }

            if (context != null && !EditorApplication.isCompiling)
            {
                needUpdate = true;
                listener.BeginGetContext(new AsyncCallback(OnRequest), null);
            }
        }

        private static void _closeListener()
        {
            if (listener != null && listener.IsListening)
            {
                listener.Stop();
                listener.Close();
                listener = null;
                Debug.Log("自动编译监听器已关闭");
            }
        }

        private static void onUpdate()
        {
            if (!EditorApplication.isCompiling && !EditorApplication.isUpdating && needUpdate)
            {
                needUpdate = false;
                AssetDatabase.Refresh();
                Debug.Log("编译完成，刷新资源");
            }
        }

        private static void OnCompilationStarted(object _) => _closeListener();
    }
}
