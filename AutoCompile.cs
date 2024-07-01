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
            EditorApplication.quitting += _closeListener;
            EditorApplication.update += onUpdate;
            _createListener();
        }

        private static void _createListener()
        {
            try
            {
                if (listener != null)
                {
                    _closeListener();
                }

                listener = new HttpListener();
                listener.Prefixes.Add("http://127.0.0.1:" + port + "/refresh/");
                listener.Start();
                listener.BeginGetContext(new AsyncCallback(OnRequest), listener);
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
            catch (ObjectDisposedException)
            {
                return; // 监听器已关闭
            }

            if (context != null && !EditorApplication.isCompiling)
            {
                needUpdate = true;
                listener.BeginGetContext(new AsyncCallback(OnRequest), listener);
            }
        }

        private static void _closeListener()
        {
            if (listener == null)
                return;

            Debug.Log("关闭监听器");
            listener.Stop();
            listener.Close();
            listener = null;
        }

        private static void onUpdate()
        {
            if (!EditorApplication.isCompiling && !EditorApplication.isUpdating && needUpdate)
            {
                needUpdate = false;
                AssetDatabase.Refresh();
            }
        }

        private static void OnCompilationStarted(object _) => _closeListener();
    }
}
