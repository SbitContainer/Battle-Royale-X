#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace BattleRoyaleX.EditorTools
{
    /// <summary>Builds an immutable local APK. Never uploads, installs or regenerates gameplay data.</summary>
    public static class PrototypeApkReleaseBuild
    {
        const string PackageId = "com.sbitcontainer.battleroyalex.prototype";
        const string Scene = "Assets/BattleRoyaleX/GeneratedScenes/Prototype01_Arena.unity";

        [Serializable]
        sealed class ReleaseManifest
        {
            public int schemaVersion = 1;
            public string recordedAtUtc;
            public string status;
            public string file;
            public string packageId;
            public int versionCode;
            public string versionName;
            public long sizeBytes;
            public string sha256;
            public string unityVersion;
            public string sourceCommit;
            public bool sourceDirty;
            public string sourceProvenance = "HASHED_BUILD_INPUTS";
            public string sourceAttribution = "source-inputs.json hashes Assets, Packages and ProjectSettings before temporary build version settings. No secret contents are published; hash inventory is not a recoverable archive.";
            public string sourceInputsSha256;
            public string testsLogSha256;
            public string testsLogWrittenAtUtc;
            public string testEvidence = "Completed Editor matrix; does not prove physical Android validation or source freshness.";
            public string signatureVerification = "NOT_VERIFIED: verify package, version and certificate with aapt/apksigner before publishing.";
            public string deviceValidation = "NOT_EXECUTED";
            public string publicUrl;
            public int buildErrors;
        }

        [MenuItem("Battle Royale X/Prototype 01/Build Versioned APK (Existing Scene)")]
        public static void BuildVersionedApk()
        {
            try { Build(); }
            catch (Exception error)
            {
                Debug.LogException(error);
                // Batch invocation is synchronous and may use -quit. A failed build must not exit successfully.
                if (Application.isBatchMode) EditorApplication.Exit(1);
                else throw;
            }
        }

        static void Build()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
                throw new InvalidOperationException("Stop Play Mode and wait for compilation before building.");
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
                throw new InvalidOperationException("Select Android first, or launch Unity with -buildTarget Android.");
            string root = Directory.GetParent(Application.dataPath).FullName;
            if (!File.Exists(Path.Combine(root, Scene)))
                throw new FileNotFoundException("The existing test scene is missing. This builder does not regenerate it.");
            if (PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android) != PackageId)
                throw new InvalidOperationException("Android package differs from the approved Battle Royale X package. No settings were changed.");

            string testLog = Argument("-brxApprovedTestsLog");
            if (string.IsNullOrEmpty(testLog) && !Application.isBatchMode)
                testLog = EditorUtility.OpenFilePanel("Select completed, approved Editor matrix log", Path.Combine(root, "Logs"), "");
            if (string.IsNullOrWhiteSpace(testLog))
                throw new InvalidOperationException("A completed approved matrix log is required: -brxApprovedTestsLog <file>. Run PrototypeLiveTestLauncher.RunBatch first.");
            testLog = Path.GetFullPath(Path.IsPathRooted(testLog) ? testLog : Path.Combine(root, testLog));
            MatchCollection finals = Regex.Matches(File.ReadAllText(testLog), @"\[BRX LIVE FINAL\] pass=(\d+) fail=(\d+)");
            if (finals.Count == 0 || int.Parse(finals[finals.Count - 1].Groups[1].Value) <= 0 ||
                int.Parse(finals[finals.Count - 1].Groups[2].Value) != 0)
                throw new InvalidOperationException("The selected log does not end with a successful completed Editor matrix.");

            string releases = Path.Combine(root, "Builds", "Releases");
            RejectRedirectingLink(Path.Combine(root, "Builds"));
            if (Directory.Exists(releases)) RejectRedirectingLink(releases);
            int code = checked(LastReservedVersion(releases, PlayerSettings.Android.bundleVersionCode) + 1);
            string directory = Path.Combine(releases, code.ToString());
            string apkName = "BattleRoyaleX-" + code + ".apk";
            string apkPath = Path.Combine(directory, apkName);
            // Reserving a directory also reserves failed versions: never replace an artifact or silently reuse a code.
            if (Directory.Exists(directory) || File.Exists(apkPath))
                throw new IOException("Release version already exists; immutable artifacts cannot be overwritten.");

            var manifest = new ReleaseManifest {
                recordedAtUtc = DateTime.UtcNow.ToString("O"), status = "BUILDING",
                file = "Builds/Releases/" + code + "/" + apkName,
                packageId = PackageId, versionCode = code, versionName = "0.9." + code + "-lab",
                unityVersion = Application.unityVersion, sourceCommit = Git(root, "rev-parse HEAD").Trim(),
                sourceDirty = !string.IsNullOrWhiteSpace(Git(root, "status --porcelain")),
                testsLogSha256 = Hash(testLog), testsLogWrittenAtUtc = File.GetLastWriteTimeUtc(testLog).ToString("O")
            };
            Directory.CreateDirectory(directory);
            string sourceInputs = Path.Combine(directory, "source-inputs.json");
            WriteSourceInputs(root, sourceInputs);
            manifest.sourceInputsSha256 = Hash(sourceInputs);
            string manifestPath = Path.Combine(directory, "manifest.json");
            WriteManifest(manifestPath, manifest, false);
            string previousVersion = PlayerSettings.bundleVersion;
            int previousCode = PlayerSettings.Android.bundleVersionCode;
            bool previousBundle = EditorUserBuildSettings.buildAppBundle;
            bool previousExport = EditorUserBuildSettings.exportAsGoogleAndroidProject;
            try
            {
                // Signing settings, keystore and passwords remain completely untouched.
                PlayerSettings.bundleVersion = manifest.versionName;
                PlayerSettings.Android.bundleVersionCode = code;
                EditorUserBuildSettings.buildAppBundle = false;
                EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
                BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                    scenes = new[] { Scene }, locationPathName = apkPath,
                    target = BuildTarget.Android, options = BuildOptions.None
                });
                manifest.buildErrors = report.summary.totalErrors;
                if (report.summary.result != BuildResult.Succeeded || !File.Exists(apkPath))
                    throw new InvalidOperationException("APK build failed: " + report.summary.result + ", errors=" + report.summary.totalErrors);
                manifest.sizeBytes = new FileInfo(apkPath).Length;
                if (manifest.sizeBytes == 0) throw new IOException("Build returned an empty APK.");
                manifest.sha256 = Hash(apkPath);
                manifest.status = "BUILT_NOT_PUBLISHED";
                WriteManifest(manifestPath, manifest, true);
                Debug.Log("[BRX VERSIONED APK] " + manifest.file + " code=" + code + " sha256=" + manifest.sha256 +
                    " Signature and device validation NOT EXECUTED. No publication or installation occurred.");
            }
            catch
            {
                manifest.status = "FAILED_NOT_PUBLISHABLE";
                WriteManifest(manifestPath, manifest, true);
                throw;
            }
            finally
            {
                PlayerSettings.bundleVersion = previousVersion;
                PlayerSettings.Android.bundleVersionCode = previousCode;
                EditorUserBuildSettings.buildAppBundle = previousBundle;
                EditorUserBuildSettings.exportAsGoogleAndroidProject = previousExport;
            }
        }

        static int LastReservedVersion(string releases, int configured)
        {
            int highest = configured;
            if (!Directory.Exists(releases)) return highest;
            foreach (string directory in Directory.GetDirectories(releases))
            {
                if (!int.TryParse(Path.GetFileName(directory), out int code) || code < 1) continue;
                highest = Math.Max(highest, code);
                string path = Path.Combine(directory, "manifest.json");
                if (!File.Exists(path)) continue;
                ReleaseManifest manifest;
                try { manifest = JsonUtility.FromJson<ReleaseManifest>(File.ReadAllText(path)); }
                catch (Exception) { continue; }
                // Only matching local bytes are trusted as a successful release; directory reservations are still never reused.
                string apk = Path.Combine(directory, "BattleRoyaleX-" + code + ".apk");
                if (manifest == null || manifest.packageId != PackageId || manifest.versionCode != code ||
                    manifest.status != "BUILT_NOT_PUBLISHED" || !File.Exists(apk) ||
                    new FileInfo(apk).Length != manifest.sizeBytes || Hash(apk) != manifest.sha256) continue;
                highest = Math.Max(highest, manifest.versionCode);
            }
            return highest;
        }

        static string Argument(string name)
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int index = 0; index < args.Length - 1; index++)
                if (args[index] == name) return args[index + 1];
            return null;
        }

        [Serializable] sealed class SourceInput { public string path; public string sha256; }
        [Serializable] sealed class SourceInputs { public SourceInput[] files; }

        static void WriteSourceInputs(string root, string destination)
        {
            var files = new List<SourceInput>();
            foreach (string folder in new[] { "Assets", "Packages", "ProjectSettings" })
                HashDirectory(root, Path.Combine(root, folder), files);
            files.Sort((a, b) => string.CompareOrdinal(a.path, b.path));
            using (var stream = new FileStream(destination, FileMode.CreateNew, FileAccess.Write))
            using (var writer = new StreamWriter(stream))
                writer.Write(JsonUtility.ToJson(new SourceInputs { files = files.ToArray() }, true));
        }

        static void HashDirectory(string root, string directory, List<SourceInput> files)
        {
            RejectRedirectingLink(directory);
            foreach (string path in Directory.GetFiles(directory))
            {
                RejectRedirectingLink(path);
                files.Add(new SourceInput { path = path.Substring(root.Length + 1).Replace('\\', '/'), sha256 = Hash(path) });
            }
            foreach (string child in Directory.GetDirectories(directory)) HashDirectory(root, child, files);
        }

        // OneDrive placeholders are reparse points, but do not redirect filesystem names.
        // Reject junctions/symlinks; permit Windows cloud tags without following an external tree.
        static void RejectRedirectingLink(string path)
        {
            if (!File.Exists(path) && !Directory.Exists(path)) return;
            if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) == 0) return;
            if (Application.platform != RuntimePlatform.WindowsEditor)
                throw new IOException("Linked build inputs are not supported on this platform.");
            IntPtr handle = FindFirstFile(path, out FindData info);
            if (handle == new IntPtr(-1)) throw new IOException("Cannot inspect build input reparse tag.");
            FindClose(handle);
            const uint CloudTag = 0x9000001A;
            // Windows defines CLOUD and CLOUD_1..CLOUD_F; no arbitrary non-surrogate tags allowed.
            bool cloud = (info.reserved0 & 0xFFFF0FFFu) == CloudTag;
            if (!cloud) throw new IOException("Redirecting or unrecognized linked build path rejected: " + path);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct FindData
        {
            public uint attributes;
            public System.Runtime.InteropServices.ComTypes.FILETIME creation, access, write;
            public uint sizeHigh, sizeLow, reserved0, reserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public string name;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)] public string alternate;
        }
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "FindFirstFileW", SetLastError = true)]
        static extern IntPtr FindFirstFile(string path, out FindData info);
        [DllImport("kernel32.dll")] [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FindClose(IntPtr handle);

        static string Hash(string path)
        {
            using (var stream = File.OpenRead(path))
            using (var sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
        }

        static void WriteManifest(string path, ReleaseManifest manifest, bool updateReserved)
        {
            using (var stream = new FileStream(path, updateReserved ? FileMode.Truncate : FileMode.CreateNew, FileAccess.Write))
            using (var writer = new StreamWriter(stream)) writer.Write(JsonUtility.ToJson(manifest, true));
        }

        static string Git(string root, string arguments)
        {
            using (var process = new Process { StartInfo = new ProcessStartInfo {
                FileName = "git", Arguments = arguments, WorkingDirectory = root,
                UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true
            } })
            {
                process.Start();
                var output = process.StandardOutput.ReadToEndAsync();
                var errors = process.StandardError.ReadToEndAsync();
                if (!process.WaitForExit(10000)) { process.Kill(); throw new TimeoutException("Git provenance check timed out."); }
                if (process.ExitCode != 0) throw new InvalidOperationException("Git provenance check failed; no artifact was built.");
                // Never copy arbitrary Git stderr (or local credential paths) into the public manifest.
                errors.GetAwaiter().GetResult();
                return output.GetAwaiter().GetResult();
            }
        }
    }
}
#endif
