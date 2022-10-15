using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using SLZ.Marrow;
using SLZ.Marrow.Warehouse;

namespace SLZ.MarrowEditor
{
    public class ModBuilder
    {

        public static string BuildPath
        {
            get
            {
                return AddressablesManager.EvaluateProfileValue(AddressablesManager.ProfileVariables.LOCAL_BUILD_PATH);
            }
        }

        private static List<string> _gamePaths;
        public static List<string> GamePaths
        {
            get
            {
                if (_gamePaths == null)
                {
                    _gamePaths = new List<string>();
                    _gamePaths.AddRange(GamePathDictionary.Values);
                }
                return _gamePaths;
            }
        }

        private static Dictionary<string, string> _gamePathDictionary;

        public static Dictionary<string, string> GamePathDictionary
        {
            get
            {
                if (_gamePathDictionary == null)
                {
                    _gamePathDictionary = new Dictionary<string, string>();









                    string gamePathParent = System.IO.Directory.GetParent(System.IO.Directory.GetParent(UnityEngine.Application.persistentDataPath).ToString()).ToString();
                    foreach (var gameName in MarrowSDK.GAME_NAMES)
                    {
                        string gamePath = Path.Combine(gamePathParent, MarrowSDK.DEV_NAME, gameName);
                        if (System.IO.Directory.Exists(gamePath))
                            _gamePathDictionary.Add(gameName, gamePath);

                        gamePath = Path.Combine(gamePathParent, MarrowSDK.DEV_NAME_ALT, gameName);
                        if (System.IO.Directory.Exists(gamePath))
                            _gamePathDictionary.Add(gameName, gamePath);
                    }
                }
                return _gamePathDictionary;
            }
            set { _gamePathDictionary = value; }
        }

        public static void InstallMod(string modFolderPath, string gameModsFolder)
        {
            if (Directory.Exists(modFolderPath) && Directory.Exists(gameModsFolder))
            {
                string destModFolderPath = Path.Combine(gameModsFolder, Path.GetFileName(modFolderPath));
                Debug.Log($"install {modFolderPath} in {destModFolderPath}");

                if (Directory.Exists(destModFolderPath))
                    Directory.Delete(destModFolderPath, true);

                var modDirectories = Directory.GetDirectories(modFolderPath, "*", SearchOption.AllDirectories);
                foreach (string dir in modDirectories)
                {
                    Directory.CreateDirectory(dir.Replace(modFolderPath, destModFolderPath));
                }

                var modFiles = Directory.GetFiles(modFolderPath, "*.*", SearchOption.AllDirectories);
                foreach (string file in modFiles)
                {

                    if (!Directory.Exists(file))
                    {
                        File.Copy(file, file.Replace(modFolderPath, destModFolderPath));
                    }
                }
            }
            else
            {
                if (!Directory.Exists(modFolderPath))
                    Debug.LogError($"Mod folder missing at: {modFolderPath}");
                if (!Directory.Exists(gameModsFolder))
                    Debug.LogError($"Game Mods folder missing at: {gameModsFolder}");
            }
        }

        private static void OpenFolder(string folderPath)
        {
            string folderToOpen = folderPath;
            if (Directory.Exists(folderToOpen))
            {
                folderToOpen = Path.GetFullPath(folderPath);





                EditorUtility.RevealInFinder(folderToOpen);
            }
        }

        [MenuItem("Stress Level Zero/Void Tools/Open Built Pallets Folder")]
        public static void OpenBuiltModFolder()
        {
            OpenBuiltModFolder(null);
        }

        public static void OpenBuiltModFolder(Pallet currentPallet = null)
        {
            if (currentPallet != null)
            {
                PalletPackerEditor.CurrentPallet = currentPallet;
            }
            OpenFolder(Path.GetFullPath(BuildPath));
        }

        public static void OpenContainingBuiltModFolder(Pallet currentPallet = null)
        {
            if (currentPallet != null)
            {
                PalletPackerEditor.CurrentPallet = currentPallet;
            }
            OpenFolder(Path.GetFullPath(Path.Combine(BuildPath, "..")));
        }

    }
}

