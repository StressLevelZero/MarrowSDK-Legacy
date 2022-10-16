#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace MetaFileUpdater
{
    public static class Program {
        const string guidIdentifier = "guid: ";

        static string rootAssetsFolderOriginal;
        static string rootAssetsFolderNew;
        static bool autoFix;
        static bool autoFixedAny = false;


        static Dictionary<string, string> replacableGuids = new Dictionary<string, string>();
        static Dictionary<string, string> guidsWithoutLocalReplacement = new Dictionary<string, string>();
        static Dictionary<string, string> allFoundGuids = new Dictionary<string, string>();


        public static void WriteLine(string arg) => UnityEngine.Debug.Log(arg);
        public static void WriteWarning(string arg) => UnityEngine.Debug.LogWarning(arg);
        public static void WriteError(string arg) => UnityEngine.Debug.LogError(arg);

        public static string PathToLocal(string path)
        {
            if (path.StartsWith(rootAssetsFolderOriginal)) path = path.Substring(rootAssetsFolderOriginal.Length);
            if (path.StartsWith(rootAssetsFolderNew)) path = path.Substring(rootAssetsFolderNew.Length);
            return path.Replace("\\", "/");
        }

        public static string TrimDotMeta(string path) => path.Substring(0, path.Length - 5);

        public static string Fix(string rootAssetsFolderOriginal, string rootAssetsFolderNew, bool autoFix)
        {
            Program.rootAssetsFolderOriginal = rootAssetsFolderOriginal;
            Program.rootAssetsFolderNew = rootAssetsFolderNew;
            Program.autoFix = autoFix;

            replacableGuids.Clear();
            guidsWithoutLocalReplacement.Clear();
            allFoundGuids.Clear();
            autoFixedAny = false;

            string[] newMetaFilePaths = Directory.GetFiles(rootAssetsFolderNew, $"*.meta", SearchOption.AllDirectories)
                .Concat(Directory.GetFiles(rootAssetsFolderNew + "/../Library/PackageCache/", $"*.meta", SearchOption.AllDirectories)).ToArray();
            WriteLine($"Found {newMetaFilePaths.Length} meta files in new directorys");
            var newMetaFilesLookup = new Dictionary<string, string>();
            newMetaFilePaths.ToList().ForEach(path => {
                // Add both the file name, and local path to the lookup
                var fileName = Path.GetFileName(path);
                if (newMetaFilesLookup.ContainsKey(fileName)) {
                    //WriteWarning($"Duplicate meta filename: {fileName}. May hook up the wrong reference.");
                } else {
                    newMetaFilesLookup[fileName] = path;
                }
                if (!newMetaFilesLookup.ContainsKey(PathToLocal(path))) {
                    newMetaFilesLookup[PathToLocal(path)] = path;
                }
            });

            var foundFiles = 0;
            if (File.Exists($"{rootAssetsFolderOriginal}/GuidCache")) { // Use the cache! Woot!
                var lines = File.ReadAllLines($"{rootAssetsFolderOriginal}/GuidCache");
                foundFiles += lines.Length;
                FindReplacementGUIDs(
                    lines.ToDictionary(p => rootAssetsFolderOriginal + p.Substring(32), p => p.Substring(0, 32)),
                    newMetaFilesLookup);
            } else { // We need to go through all the meta files - then we can cache their guid against them 
                string[] oldMetaFilePaths = Directory.GetFiles(rootAssetsFolderOriginal, $"*.meta", SearchOption.AllDirectories);
                foundFiles += oldMetaFilePaths.Length;

                WriteLine($"Found {foundFiles} meta files in original directory");

                FindReplacementGUIDs(
                    oldMetaFilePaths.ToDictionary(p => p, p => (string) null),
                    newMetaFilesLookup);

                // Cache the things we just found
                File.WriteAllLines($"{rootAssetsFolderOriginal}/GuidCache", 
                    allFoundGuids
                        .Select((kv) => kv.Value + PathToLocal(kv.Key))
                        .ToArray()
                );
            }

            var fixedAtLeastOnce = false;
            do {
                autoFixedAny = false;
                var localPaths = Directory.GetFiles(rootAssetsFolderNew, "*", SearchOption.AllDirectories);
                ReplaceGUIDs(localPaths);
                if (autoFixedAny) fixedAtLeastOnce = true;
            } while (autoFixedAny);

            if (fixedAtLeastOnce) {
                AssetDatabase.Refresh();
            }

            return "Success. Meta files fixed";
        }

        private static string GetGuidFromMetaPath(string path)
        {
            using (StreamReader reader = new StreamReader(path)) {
                while (reader.Peek() >= 0) {
                    string currentLine = reader.ReadLine();
                    if (currentLine.StartsWith(guidIdentifier)) {
                        return currentLine.Split(new string[] { guidIdentifier }, StringSplitOptions.None)[1];
                    }
                }
            }
            return null;
        }

        private static void FindReplacementGUIDs(Dictionary<string, string> oldMetaFiles, Dictionary<string, string> newMetaFiles)
        {
            oldMetaFiles.Keys.ToList().ForEach(oldMetaPath =>
            {
                string oldGUID = oldMetaFiles[oldMetaPath];
                if (oldGUID == null) {
                    oldGUID = GetGuidFromMetaPath(oldMetaPath);
                    if (oldGUID != null)
                        allFoundGuids.Add(oldMetaPath, oldGUID);
                }
                if (oldGUID == null) {
                    return;
                }

                //Try to find a new meta file with the same path
                newMetaFiles.TryGetValue(PathToLocal(oldMetaPath), out string correspondingNewMetaFile);

                if (correspondingNewMetaFile == null) {
                    //Try to find a new meta file with the same name
                    newMetaFiles.TryGetValue(Path.GetFileName(oldMetaPath), out correspondingNewMetaFile);
                }

                if (correspondingNewMetaFile == null) {
                    if (!guidsWithoutLocalReplacement.ContainsKey(oldGUID)) {
                        guidsWithoutLocalReplacement.Add(oldGUID, oldMetaPath);
                    }
                    return;
                }

                string newGUID = GetGuidFromMetaPath(correspondingNewMetaFile);

                if (newGUID == null)
                    return;

                //Add the guids to the dictionary
                if (!replacableGuids.ContainsKey(oldGUID) && oldGUID != newGUID)
                    replacableGuids.Add(oldGUID, newGUID);
            });

            WriteLine($"Found replacement GUIDs for {replacableGuids.Count}/{replacableGuids.Count + guidsWithoutLocalReplacement.Count} meta files");
        }

        private static void ReplaceGUIDs(string[] prefabPaths)
        {
            for (int i = 0; i < prefabPaths.Length; i++) {
                var guidsToReplace = new List<string>();
                // This is much more efficient than reading the whole file to work out if it's a viable yaml file for replacement
                using (StreamReader reader = new StreamReader(prefabPaths[i])) {
                    if (reader?.ReadLine()?.StartsWith("%YAML") != true)
                        continue;
                }

                File.ReadAllLines(prefabPaths[i]).ToList().ForEach(line => {
                    foreach (var part in line.Split(guidIdentifier).Skip(1)) {
                        //Get the existing guid
                        string currentGUID = part.Substring(0, 32);
                        //Add the guid to a list to be replaced if it exists in the dictionary
                        if (replacableGuids.TryGetValue(currentGUID, out string newGUID)) {
                            guidsToReplace.Add(currentGUID);
                        } else if (guidsWithoutLocalReplacement.TryGetValue(currentGUID, out var originalPath)) {
                            if (autoFix) {
                                var path = PathToLocal(originalPath);
                                Directory.CreateDirectory($"{Path.GetDirectoryName($"{rootAssetsFolderNew}/{path}")}");
                                try {
                                    File.Copy(originalPath, $"{rootAssetsFolderNew}/{path}");
                                    File.Copy(TrimDotMeta(originalPath), $"{rootAssetsFolderNew}/{TrimDotMeta(path)}");
                                    WriteWarning($"Copied {rootAssetsFolderNew}/{TrimDotMeta(path)}");
                                    autoFixedAny = true;
                                } catch (Exception e) {
                                    if (e.Message.Contains("already exists") == false) {
                                        WriteWarning($"Failed to copy {rootAssetsFolderNew}/{TrimDotMeta(path)}: {e.Message}");
                                    }
                                }
                                continue;
                            }
                            var msg = $"Asset detected in original directory that does not exist locally: {originalPath}";
                            if (originalPath.EndsWith(".cs.meta")) {
                                WriteError(msg);
                            } else {
                                WriteWarning(msg);
                            }
                        }
                    }
                });

                if (guidsToReplace.Count > 0)
                {
                    //Read the entire prefab file
                    string fileData = File.ReadAllText(prefabPaths[i]);

                    //Replace all guids with the new ones
                    for (int j = 0; j < guidsToReplace.Count; j++)
                    {
                        string currentGUID = guidsToReplace[j];
                        string newGUID = replacableGuids[guidsToReplace[j]];
                        fileData = fileData.Replace(currentGUID, newGUID);
                        WriteLine($"Replacing {currentGUID} with {newGUID} in {PathToLocal(prefabPaths[i])}");
                    }

                    //Write the modified text back to the prefab file
                    File.WriteAllText(prefabPaths[i], fileData);
                }
            }
        }
    }
}
#endif