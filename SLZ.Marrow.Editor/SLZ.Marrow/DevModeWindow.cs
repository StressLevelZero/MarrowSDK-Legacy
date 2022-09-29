#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using SLZ.Marrow.Warehouse;
using SLZ.MarrowEditor;
using UnityEditor;
using UnityEngine;
using WebSocketSharp;

namespace SLZ.Marrow
{
    public sealed class DevModeWindow : EditorWindow
    {
        [MenuItem("Stress Level Zero/Void Tools/DevMode", false, 104)]
        public static void ShowConsoleCommandTester()
        {
            var window = GetWindow<DevModeWindow>();
            window.ShowUtility();
        }
        private static WebSocketSharp.WebSocket websocket;

        public string path = "ws://127.0.0.1:50152/console";
        public string command = "";

        public static bool DevModeEnabled
        {
            get
            {
                return websocket != null && websocket.ReadyState == WebSocketState.Open;
            }
        }

        private List<Pallet> cachedPallets;

        private void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                path = EditorGUILayout.TextField("WebSocket", path);
                command = EditorGUILayout.TextField("Command", command);

                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUI.DisabledScope(websocket is { ReadyState: WebSocketState.Open }))
                    {
                        if (GUILayout.Button("Connect"))
                        {
                            if (websocket != null)
                            {
                                ((IDisposable)websocket).Dispose();
                                websocket = null;
                            }

                            try
                            {
                                websocket = new WebSocketSharp.WebSocket(path);
                                websocket.OnMessage += (sender, e) => Debug.Log(e.Data);
                                websocket.Connect();
                            }
                            catch (Exception e)
                            {
                                Debug.LogException(e);
                            }
                        }
                    }

                    using (new EditorGUI.DisabledScope(websocket is not
                    { ReadyState: WebSocketState.Open or WebSocketState.Connecting }))
                    {
                        if (GUILayout.Button("Disconnect"))
                        {
                            try
                            {
                                websocket.Close();
                                ((IDisposable)websocket).Dispose();
                                websocket = null;
                            }
                            catch (Exception e)
                            {
                                Debug.LogException(e);
                            }
                        }
                    }
                }

                if (websocket != null)
                {
                    GUILayout.Label($"Ready state: {websocket.ReadyState.ToString()}");
                    if (GUILayout.Button("Send"))
                    {
                        websocket.Send(command);
                    }
                }

                EditorGUILayout.Space();

                if (DevModeEnabled)
                {
                    EditorGUILayout.LabelField("Dev Tools: ", EditorStyles.boldLabel);

                    if (GUILayout.Button(new GUIContent("Reload Level", "Reloads current loaded level"), MarrowGUIStyles.DefaultButton))
                    {
                        websocket.Send($"level.reload");
                    }

                    if (AssetWarehouse.Instance != null)
                    {
                        EditorGUILayout.LabelField("Reload Pallet: ", EditorStyles.boldLabel);
                        AssetWarehouse.Instance.GetPallets(ref cachedPallets);
                        foreach (var pallet in cachedPallets)
                        {
                            if (GUILayout.Button(new GUIContent(pallet.Title, $"Reload {pallet.Title}"), MarrowGUIStyles.DefaultButton))
                            {
                                websocket.Send($"assetwarehouse.reload {pallet.Barcode}");
                            }
                        }
                    }
                }
            }
        }
    }
}
#endif