#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace BattleRoyaleX.EditorTools
{
    /// <summary>Pure contract tests, without network, APK generation or Android JNI.</summary>
    public static class PrototypeApkUpdateValidation
    {
        const string Url = "https://github.com/SbitContainer/Battle-Royale-X/releases/download/lab-12/BattleRoyaleX-12.apk";

        [MenuItem("Battle Royale X/Prototype 01/Validate APK Update Contract")]
        public static void RunBatch()
        {
            int passed = 0, failed = 0;
            void Check(string name, bool value)
            {
                if (value) { passed++; Debug.Log("[BRX UPDATE PASS] " + name); }
                else { failed++; Debug.LogError("[BRX UPDATE FAIL] " + name); }
            }
            PrototypeGithubApkUpdates.UpdateManifest Valid()
            {
                return new PrototypeGithubApkUpdates.UpdateManifest {
                    schemaVersion = 1, packageName = "com.sbitcontainer.battleroyalex.prototype",
                    versionCode = 12, versionName = "0.9.12-lab", apkUrl = Url,
                    apkBytes = 57131167, apkSha256 = new string('a', 64)
                };
            }
            void Reject(string name, Action<PrototypeGithubApkUpdates.UpdateManifest> mutate)
            {
                var value = Valid(); mutate(value);
                Check(name, PrototypeGithubApkUpdates.ValidateManifest(value, 11) != null);
            }
            Check("valid manifest", PrototypeGithubApkUpdates.ValidateManifest(Valid(), 11) == null);
            Check("null manifest", PrototypeGithubApkUpdates.ValidateManifest(null, 11) != null);
            Reject("schema mismatch", value => value.schemaVersion = 2);
            Reject("different package", value => value.packageName = "com.other.app");
            Reject("same version", value => value.versionCode = 11);
            Reject("downgrade", value => value.versionCode = 10);
            Reject("invalid version", value => value.versionCode = -1);
            Reject("empty display version", value => value.versionName = " ");
            Reject("control character", value => value.versionName = "lab\n12");
            Reject("long display version", value => value.versionName = new string('x', 81));
            Reject("zero bytes", value => value.apkBytes = 0);
            Reject("negative bytes", value => value.apkBytes = -1);
            Reject("size limit", value => value.apkBytes = 512L * 1024 * 1024 + 1);
            Reject("missing hash", value => value.apkSha256 = null);
            Reject("short hash", value => value.apkSha256 = new string('a', 63));
            Reject("invalid hex", value => value.apkSha256 = new string('z', 64));
            Reject("trailing hash newline", value => value.apkSha256 = new string('a', 64) + "\n");
            var upper = Valid(); upper.apkSha256 = new string('A', 64);
            Check("uppercase hex accepted", PrototypeGithubApkUpdates.ValidateManifest(upper, 11) == null);
            Check("approved release URL", PrototypeGithubApkUpdates.IsReleaseUrl(Url));
            string[] forbidden = {
                null, "", Url.Replace("https://", "http://"),
                Url.Replace("github.com/", "github.com.evil.example/"),
                Url.Replace("github.com/", "user:password@github.com/"),
                Url.Replace("github.com/", "github.com:444/"),
                Url.Replace("SbitContainer/", "OtherPublisher/"),
                Url.Replace("Battle-Royale-X/", "OtherRepo/"),
                Url + "?token=example", Url + "#fragment",
                "https://release-assets.githubusercontent.com/any.apk",
                "https://github.com/SbitContainer/Battle-Royale-X/releases/download/lab-12/",
                "https://github.com/SbitContainer/Battle-Royale-X/releases/download//file.apk",
                "https://github.com/SbitContainer/Battle-Royale-X/releases/download/lab-12/sub/file.apk",
                "https://github.com/SbitContainer/Battle-Royale-X/releases/download/lab-12/%5cfile.apk"
            };
            for (int index = 0; index < forbidden.Length; index++)
                Check("unapproved URL " + index, !PrototypeGithubApkUpdates.IsReleaseUrl(forbidden[index]));
            Reject("external APK origin", value => value.apkUrl = "https://evil.example/update.apk");
            Debug.Log("[BRX UPDATE TEST FINAL] pass=" + passed + " fail=" + failed +
                " layer=PURE_CONTRACT network=NOT_EXECUTED android=NOT_EXECUTED");
            if (Application.isBatchMode) EditorApplication.Exit(failed == 0 ? 0 : 1);
            else if (failed != 0) throw new InvalidOperationException("APK update contract validation failed.");
        }
    }
}
#endif
