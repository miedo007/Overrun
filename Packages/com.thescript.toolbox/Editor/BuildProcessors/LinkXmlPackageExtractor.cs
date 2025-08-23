using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.PackageManager;
using UnityEngine;

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
            if (File.Exists(LinkFilePath))
            {
                File.Delete(LinkFilePath);
                File.Delete($"{LinkFilePath}.meta");
            }

            if (!Directory.EnumerateFiles(TemporaryFolder, "*").Any())
            {
                Directory.Delete(TemporaryFolder);
                File.Delete($"{TemporaryFolder}.meta");
            }
        }

        private static void CreateMergedLinkFromPackages()
        {
            var packageRequest = Client.List();
            do
            {
            } while (!packageRequest.IsCompleted);

            if (packageRequest.Status == StatusCode.Failure)
            {
                Debug.LogError(packageRequest.Error.message);
                return;
            }

            var xmlPathList = new List<string>();
            foreach (var package in packageRequest.Result)
            {
                var path = package.resolvedPath;
                xmlPathList.AddRange(Directory.EnumerateFiles(path, LinkXml, SearchOption.AllDirectories).ToList());
            }

            if (xmlPathList.Count <= 0)
            {
                return;
            }

            var xmlList = xmlPathList.Select(XDocument.Load).ToArray();
            var combinedXml = xmlList.First() ?? new XDocument();

            for (int i = 1, iMax = xmlList.Length; i < iMax; ++i)
            {
                var xDocument = xmlList[i];
                // ReSharper disable PossibleNullReferenceException
                combinedXml.Root.Add(xDocument.Root.Elements());
                // ReSharper restore PossibleNullReferenceException
            }

            if (!Directory.Exists(TemporaryFolder))
            {
                Directory.CreateDirectory(TemporaryFolder);
            }

            combinedXml.Save(LinkFilePath);
        }
    }
}