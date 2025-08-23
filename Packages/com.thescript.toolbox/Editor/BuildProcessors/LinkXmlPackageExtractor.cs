using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.PackageManager;
using UnityEngine;
using System;

namespace Mtl.Toolbox
{
    public class LinkXmlPackageExtractor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private const string LinkXml = "link.xml";
        private static string TemporaryFolder => Path.Combine(Application.dataPath, "Temporary");
        private static string LinkFilePath => Path.Combine(TemporaryFolder, LinkXml);

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report) => CreateMergedLinkFromPackages();

        public void OnPostprocessBuild(BuildReport report)
        {
            // If the temp folder never existed, there's nothing to clean.
            if (!Directory.Exists(TemporaryFolder))
            {
                Debug.Log($"[LinkXmlPackageExtractor] '{TemporaryFolder}' not found. Nothing to clean.");
                return;
            }

            // Remove merged link.xml if present.
            if (File.Exists(LinkFilePath))
            {
                try
                {
                    File.Delete(LinkFilePath);
                }
                catch (IOException e)
                {
                    Debug.LogWarning($"[LinkXmlPackageExtractor] Could not delete file '{LinkFilePath}': {e.Message}");
                }

                var linkMeta = $"{LinkFilePath}.meta";
                if (File.Exists(linkMeta))
                {
                    try { File.Delete(linkMeta); }
                    catch (IOException e) { Debug.LogWarning($"[LinkXmlPackageExtractor] Could not delete file '{linkMeta}': {e.Message}"); }
                }
            }

            // If folder is now empty, delete it (and its meta) safely.
            try
            {
                if (!Directory.EnumerateFileSystemEntries(TemporaryFolder).Any())
                {
                    Directory.Delete(TemporaryFolder, true);

                    var folderMeta = $"{TemporaryFolder}.meta";
                    if (File.Exists(folderMeta))
                    {
                        try { File.Delete(folderMeta); }
                        catch (IOException e) { Debug.LogWarning($"[LinkXmlPackageExtractor] Could not delete file '{folderMeta}': {e.Message}"); }
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                // Already gone; fine.
            }
        }

        private static void CreateMergedLinkFromPackages()
        {
            // Ask Package Manager for the installed packages.
            var packageRequest = Client.List();
            while (!packageRequest.IsCompleted) { }

            if (packageRequest.Status == StatusCode.Failure)
            {
                Debug.LogError($"[LinkXmlPackageExtractor] Package list failed: {packageRequest.Error?.message}");
                return;
            }

            // Collect all 'link.xml' files inside packages (ignore inaccessible spots).
            var xmlPathList = new List<string>();
            foreach (var package in packageRequest.Result)
            {
                var path = package.resolvedPath;
                if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                    continue;

                try
                {
                    xmlPathList.AddRange(Directory.EnumerateFiles(path, LinkXml, SearchOption.AllDirectories));
                }
                catch (IOException e)
                {
                    Debug.LogWarning($"[LinkXmlPackageExtractor] Skipping '{path}': {e.Message}");
                }
                catch (UnauthorizedAccessException e)
                {
                    Debug.LogWarning($"[LinkXmlPackageExtractor] Skipping '{path}': {e.Message}");
                }
            }

            if (xmlPathList.Count == 0)
            {
                Debug.Log("[LinkXmlPackageExtractor] No package link.xml files found. Skipping merge.");
                return;
            }

            // Load and merge all link.xml docs under a single root.
            var docs = xmlPathList.Select(p =>
            {
                try { return XDocument.Load(p); }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[LinkXmlPackageExtractor] Could not load '{p}': {e.Message}");
                    return null;
                }
            }).Where(d => d != null).ToList();

            if (docs.Count == 0)
            {
                Debug.Log("[LinkXmlPackageExtractor] No valid link.xml documents loaded. Skipping merge.");
                return;
            }

            // Ensure we always have a root element ('linker' is the conventional root for link.xml).
            var combined = new XDocument(new XElement("linker"));

            foreach (var doc in docs)
            {
                var root = doc.Root;
                if (root == null)
                    continue;

                // Append all child elements from each doc under combined root.
                foreach (var el in root.Elements())
                {
                    combined.Root.Add(new XElement(el));
                }
            }

            // Make sure the temp folder exists before saving.
            if (!Directory.Exists(TemporaryFolder))
            {
                Directory.CreateDirectory(TemporaryFolder);
            }

            combined.Save(LinkFilePath);
            Debug.Log($"[LinkXmlPackageExtractor] Merged link.xml saved to '{LinkFilePath}'.");
        }
    }
}
