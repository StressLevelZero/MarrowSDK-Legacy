using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace SLZ.MarrowEditor
{
    public class TextureStreamTool
    {
        private static List<string> GetAllTexture2DInProject()
        {
            List<string> texturePaths = new List<string>();

            var textureGuids = AssetDatabase.FindAssets("t:Texture2D");

            foreach (var textureGuid in textureGuids)
            {
                var texturePath = AssetDatabase.GUIDToAssetPath(textureGuid);
                if (typeof(Texture2D).IsAssignableFrom(AssetDatabase.GetMainAssetTypeAtPath(texturePath))
                    && !texturePaths.Contains(textureGuid))
                {
                    texturePaths.Add(texturePath);
                }
            }

            return texturePaths;
        }


        public static TextureStreamToolResults ApplyTextureStreamingToAllTextures()
        {
            var allTextures = GetAllTexture2DInProject();
            Debug.Log("TextureStreamTool: Found " + allTextures.Count + " textures, enabling Texture Streaming...");
            var results = ApplyTextureStreamingToTextures(allTextures, false);
            return results;
        }


        public static TextureStreamToolResults Analyze()
        {
            var allTextures = GetAllTexture2DInProject();
            Debug.Log("TextureStreamTool: Found " + allTextures.Count + " textures");
            var results = ApplyTextureStreamingToTextures(allTextures, true);
            return results;
        }


        public static void CountAssets()
        {
            var guids = AssetDatabase.FindAssets("");
            Debug.Log("Assets: Found " + guids.Length + " assets");
            var assetPaths = new string[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                assetPaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            }
            int objectCount = 0;
            string textureObjs = "assets with textures: ";
            foreach (var assetPath in assetPaths)
            {
                var objects = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
                objectCount += objects.Length;
                bool hasTextures = false;
                foreach (var obj in objects)
                {
                    if (obj is Texture)
                    {
                        hasTextures = true;
                    }
                }
                if (hasTextures)
                {
                    textureObjs += assetPath + "\n";
                }
            }

            Debug.Log(objectCount + " sub objects");
            Debug.Log(textureObjs);
        }


        public static void GetAssetInfo()
        {
            var asset = Selection.activeObject;
            string path = AssetDatabase.GetAssetPath(asset);
            var subObjects = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);

            string text = $"{asset} [{subObjects.Length}]: \n";
            foreach (var subObject in subObjects)
            {
                text += subObject.ToString() + "\n";
            }
            Debug.Log(text);
        }

        public class TextureStreamToolResults
        {
            public int totalTextures;
            public List<string> enabledList;
            public List<string> alreadyList;
            public List<string> skippedList;
            public List<string> disabledList;
            public List<string> updatedList;

            public string enabledFullText;
            public string alreadyFullText;
            public string skippedFullText;
            public string disabledFullText;
            public string updatedFullText;

            public TextureStreamToolResults(int totalTextures, List<string> enabledList, List<string> alreadyList, List<string> skippedList, List<string> disabledList, List<string> updatedList, string enabledFullText, string alreadyFullText, string skippedFullText, string disabledFullText, string updatedFullText)
            {
                this.totalTextures = totalTextures;
                this.enabledList = enabledList;
                this.alreadyList = alreadyList;
                this.skippedList = skippedList;
                this.disabledList = disabledList;
                this.updatedList = updatedList;
                this.enabledFullText = enabledFullText;
                this.alreadyFullText = alreadyFullText;
                this.skippedFullText = skippedFullText;
                this.disabledFullText = disabledFullText;
                this.updatedFullText = updatedFullText;
            }
        }

        private static Dictionary<string, string> textureResults = new Dictionary<string, string>();
        public static TextureStreamToolResults ApplyTextureStreamingToTextures(List<string> assetPaths, bool analysisMode = false)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            TextureImporter textureImporter = null;
            textureResults.Clear();

            List<string> texturesToReimport = new List<string>();
            List<TextureImporter> importersToReimport = new List<TextureImporter>();

            Dictionary<string, List<string>> importers = new Dictionary<string, List<string>>();

            foreach (var assetPath in assetPaths)
            {
                var importer = AssetImporter.GetAtPath(assetPath);
                if (importer == null)
                    continue;

                List<string> importerAssets;
                if (!importers.TryGetValue(importer.GetType().Name, out importerAssets))
                {
                    importerAssets = new List<string>();
                    importers.Add(importer.GetType().Name, importerAssets);
                }
                importerAssets.Add(assetPath);

                textureImporter = importer as TextureImporter;
                if (textureImporter == null)
                    continue;
                bool appliedChanges = ApplyTextureStreamingToTexture(textureImporter, assetPath, analysisMode);

                if (appliedChanges && !analysisMode)
                {
                    texturesToReimport.Add(assetPath);
                    importersToReimport.Add(textureImporter);
                }
            }

            string appliedTextures = "";
            int appliedCount = 0;
            string alreadyStreaming = "";
            int alreadyStrimCount = 0;
            string skippedTextures = "";
            int skippedCount = 0;
            string disabledTextures = "";
            int disabledCount = 0;
            string updatedTextures = "";
            int updatedCount = 0;

            List<string> enabledList = new List<string>();
            List<string> alreadyList = new List<string>();
            List<string> skippedList = new List<string>();
            List<string> disabledList = new List<string>();
            List<string> updatedList = new List<string>();

            foreach (var texResult in textureResults)
            {
                if (texResult.Value.Contains("[enabled]"))
                {
                    appliedTextures += $"\n{texResult.Key}:  {texResult.Value}";
                    enabledList.Add($"{texResult.Key}:  {texResult.Value}");
                    appliedCount++;
                }
                if (texResult.Value.Contains("[already]"))
                {
                    alreadyStreaming += $"\n{texResult.Key}:  {texResult.Value}";
                    alreadyList.Add($"{texResult.Key}:  {texResult.Value}");
                    alreadyStrimCount++;
                }
                else if (texResult.Value.Contains("[disabled]"))
                {
                    disabledTextures += $"\n{texResult.Key}:  {texResult.Value}";
                    disabledList.Add($"{texResult.Key}:  {texResult.Value}");
                    disabledCount++;
                }
                else if (texResult.Value.Contains("[skipped]"))
                {
                    skippedTextures += $"\n{texResult.Key}:  {texResult.Value}";
                    skippedList.Add($"{texResult.Key}:  {texResult.Value}");
                    skippedCount++;
                }
                else if (texResult.Value.Contains("[updated]"))
                {
                    updatedTextures += $"\n{texResult.Key}:  {texResult.Value}";
                    updatedList.Add($"{texResult.Key}:  {texResult.Value}");
                    updatedCount++;
                }
            }

            Debug.Log($"TextureStreamTool: Finished analyzing {assetPaths.Count} textures, took {stopwatch.Elapsed}");
            Debug.Log($"TextureStreamTool: Enable Texture Streaming on {appliedCount} textures: {appliedTextures}");
            Debug.Log($"TextureStreamTool: Already set Texture Streaming on {alreadyStrimCount} textures: {alreadyStreaming}");
            Debug.Log($"TextureStreamTool: Could not enable Texture Streaming on {skippedCount} textures: {skippedTextures}");
            Debug.Log($"TextureStreamTool: Disabled Texture Streaming on {disabledCount} textures: {disabledTextures}");
            Debug.Log($"TextureStreamTool: Updated {updatedCount} textures: {updatedTextures}");

            if (!analysisMode)
            {
                Debug.Log("TextureStreamTool: Reimporting " + texturesToReimport.Count + " textures");
                AssetDatabase.StartAssetEditing();
                for (var index = 0; index < texturesToReimport.Count; index++)
                {
                    var texturePath = texturesToReimport[index];
                    importersToReimport[index].SaveAndReimport();

                }

                AssetDatabase.StopAssetEditing();

            }

            enabledList.Sort();
            alreadyList.Sort();
            skippedList.Sort();
            disabledList.Sort();
            updatedList.Sort();

            Debug.Log("TextureStreamTool: Finished, took " + stopwatch.Elapsed);

            return new TextureStreamToolResults(assetPaths.Count, enabledList, alreadyList, skippedList, disabledList, updatedList,
                    appliedTextures, alreadyStreaming, skippedTextures, disabledTextures, updatedTextures);
        }

        private static bool ApplyTextureStreamingToTexture(TextureImporter importer, string assetPath, bool analysisMode = false)
        {
            bool enableStreaming = true;
            bool changed = false;
            string detailsString = "";
            List<string> details = new List<string>();
            List<string> alwaysUpdate = new List<string>();
            List<string> disableReasons = new List<string>();





            if (importer == null)
            {
                details.Add("null importer [skipped]");
                enableStreaming = false;
            }
            else
            {

                if (importer.textureType == TextureImporterType.Sprite)
                {

                    enableStreaming = false;
                    disableReasons.Add("Sprite");
                }

                if (assetPath.Contains("/Editor/") || assetPath.Contains("\\Editor\\"))
                {

                    enableStreaming = false;
                    disableReasons.Add("Editor");
                }

                if (assetPath.Contains("/Resources/") || assetPath.Contains("\\Resources\\"))
                {

                    enableStreaming = false;
                    disableReasons.Add("Resources");
                }

                if (assetPath.StartsWith("Packages"))
                {

                    enableStreaming = false;
                    disableReasons.Add("Package");
                }

                if (!importer.mipmapEnabled)
                {

                    enableStreaming = false;
                    disableReasons.Add("!mipMapEnabled");



                }








                if (TextureStreamSettings.Instance.IsTexturePreventedFromStreaming(assetPath))
                {
                    enableStreaming = false;
                    disableReasons.Add("prevented");
                }

                if (importer.isReadable && !disableReasons.Contains("Package"))
                {
                    if (!analysisMode)
                        importer.isReadable = false;
                    details.Add("readable");
                    alwaysUpdate.Add("readable");
                }




                var sb = new StringBuilder();
                if (disableReasons.Count > 0)
                {
                    sb.Append("! ");
                    bool first = true;
                    foreach (var disableReason in disableReasons)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            sb.Append(" ");
                        }

                        sb.Append($"{disableReason}");
                    }

                    sb.Append(" ! ");
                }

                foreach (var detail in details)
                {
                    sb.Append($"{detail} ");
                }
                detailsString = sb.ToString();




                if (importer.streamingMipmaps != enableStreaming)
                {
                    if (!analysisMode)
                    {
                        importer.streamingMipmaps = enableStreaming;
                        changed = true;
                    }

                    if (enableStreaming)
                    {
                        detailsString += " [enabled]";
                    }
                    else
                    {
                        detailsString += " [disabled]";
                    }
                }
                else
                {
                    if (alwaysUpdate.Count > 0)
                    {
                        detailsString += " [updated]";
                    }
                    else
                    {
                        if (enableStreaming)
                        {
                            detailsString += " [already]";
                        }
                        else
                        {
                            detailsString += " [skipped]";
                        }
                    }
                }

            }

            if (textureResults.ContainsKey(assetPath))
            {



            }
            else
            {
                textureResults.Add(assetPath, detailsString);
            }

            return changed;
        }

        public static TextureImporter GetTextureImporterForTextureGuid(string textureGuid)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(textureGuid);
            var importer = AssetImporter.GetAtPath(assetPath);
            if (importer is TextureImporter textureImporter)
            {
                return textureImporter;
            }
            return null;
        }

        public static bool IsTextureValidForStreaming(Texture2D texture)
        {
            bool valid = false;

            var assetPath = AssetDatabase.GetAssetPath(texture);
            var guid = AssetDatabase.GUIDFromAssetPath(assetPath).ToString();
            valid = IsTextureValidForStreaming(guid);

            return valid;
        }

        public static bool IsTextureValidForStreaming(string textureGuid)
        {
            bool valid = false;

            var assetPath = AssetDatabase.GUIDToAssetPath(textureGuid);
            var importer = AssetImporter.GetAtPath(assetPath);
            if (importer is TextureImporter textureImporter)
            {
                valid = IsTextureValidForStreaming(textureImporter, assetPath);
            }

            return valid;
        }

        public static bool IsTextureValidForStreaming(TextureImporter importer, string assetPath)
        {

            bool valid = true;

            if (importer.textureType == TextureImporterType.Sprite)
            {

                valid = false;
            }

            if (assetPath.Contains("/Editor/") || assetPath.Contains("\\Editor\\"))
            {

                valid = false;
            }

            if (assetPath.Contains("/Resources/") || assetPath.Contains("\\Resources\\"))
            {

                valid = false;
            }

            if (assetPath.StartsWith("Packages"))
            {

                valid = false;
            }

            if (!importer.mipmapEnabled)
            {

                valid = false;
            }

            return valid;
        }


    }
}










































