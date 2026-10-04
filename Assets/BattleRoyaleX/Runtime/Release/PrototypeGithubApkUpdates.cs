using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace BattleRoyaleX
{
    /// <summary>Opt-in APK updates. Android, not the game, authorizes installation.</summary>
    public sealed class PrototypeGithubApkUpdates : MonoBehaviour
    {
        const string Package = "com.sbitcontainer.battleroyalex.prototype";
        const string Latest = "https://api.github.com/repos/SbitContainer/Battle-Royale-X/releases/latest";
        const string DownloadPrefix = "/SbitContainer/Battle-Royale-X/releases/download/";
        const string Bridge = "com.sbitcontainer.brxupdates.ApkUpdates";
        const long MaxApkBytes = 512L * 1024 * 1024;
        static PrototypeGithubApkUpdates instance;

        [Serializable] public sealed class UpdateManifest
        {
            public int schemaVersion;
            public string packageName;
            public int versionCode;
            public string versionName;
            public string apkUrl;
            public long apkBytes;
            public string apkSha256;
        }
        [Serializable] sealed class Release { public bool draft; public bool prerelease; public Asset[] assets; }
        [Serializable] sealed class Asset { public string name; public string browser_download_url; public long size; }

        UpdateManifest pending;
        bool busy, panelOpen, verifiedCache;
        string status = "";
        string cachePath;
        float progress;
        UnityWebRequest activeRequest;
        RectTransform inputBlocker;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Initialize()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (instance != null) return;
            var host = new GameObject("BRX APK Updates");
            DontDestroyOnLoad(host);
            instance = host.AddComponent<PrototypeGithubApkUpdates>();
#endif
        }

        void Start()
        {
            cachePath = Path.Combine(Application.persistentDataPath, "BRXUpdates", "update.apk");
            var canvasObject = new GameObject("Update Input Shield", typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(transform, false);
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 32000;
            var shield = new GameObject("Shield", typeof(RectTransform), typeof(Image));
            shield.transform.SetParent(canvasObject.transform, false);
            shield.GetComponent<Image>().color = Color.clear;
            inputBlocker = shield.GetComponent<RectTransform>();
            inputBlocker.anchorMin = inputBlocker.anchorMax = inputBlocker.pivot = Vector2.zero;
            shield.SetActive(false);
            StartCoroutine(CheckPeriodically());
        }

        IEnumerator CheckPeriodically()
        {
            yield return new WaitForSecondsRealtime(5);
            while (true)
            {
                if (!busy && !panelOpen) yield return Check();
                yield return new WaitForSecondsRealtime(900);
            }
        }

        IEnumerator Check()
        {
            busy = true;
            int installed = -1;
            try { installed = InstalledVersion(); }
            catch (Exception) { }
            if (installed < 0) { busy = false; yield break; }
            string json = null;
            yield return ReadText(Latest, 512 * 1024, value => json = value, true);
            Release release = null;
            try { if (json != null) release = JsonUtility.FromJson<Release>(json); }
            catch (Exception) { /* Offline, incomplete or invalid catalog is never an update. */ }
            if (release == null || release.draft || release.prerelease || release.assets == null)
            { busy = false; yield break; }
            Asset manifestAsset = null;
            foreach (Asset asset in release.assets)
                if (asset != null && asset.name == "brx-update.json") { manifestAsset = asset; break; }
            if (manifestAsset == null || manifestAsset.size < 1 || manifestAsset.size > 32768 || !IsReleaseUrl(manifestAsset.browser_download_url))
            { busy = false; yield break; }
            json = null;
            yield return ReadText(manifestAsset.browser_download_url, 32768, value => json = value, false);
            UpdateManifest candidate = null;
            try { if (json != null) candidate = JsonUtility.FromJson<UpdateManifest>(json); }
            catch (Exception) { }
            if (ValidateManifest(candidate, installed) == null)
            {
                pending = candidate;
                verifiedCache = false;
                status = "Nova versão " + candidate.versionName + ". Baixar exige sua confirmação.";
            }
            busy = false;
        }

        public static string ValidateManifest(UpdateManifest value, int installedVersion)
        {
            if (value == null || value.schemaVersion != 1) return "Manifesto incompatível.";
            if (value.packageName != Package) return "Pacote incorreto.";
            if (value.versionCode <= installedVersion || value.versionCode < 1) return "Versão não é mais nova.";
            if (string.IsNullOrWhiteSpace(value.versionName) || value.versionName.Length > 80 || Regex.IsMatch(value.versionName, @"[\p{C}]"))
                return "Nome de versão inválido.";
            if (!IsReleaseUrl(value.apkUrl)) return "APK fora do repositório autorizado.";
            if (value.apkBytes < 1 || value.apkBytes > MaxApkBytes) return "Tamanho de APK inválido.";
            if (value.apkSha256 == null || !Regex.IsMatch(value.apkSha256, @"\A[0-9a-fA-F]{64}\z")) return "Hash inválido.";
            return null;
        }

        public static bool IsReleaseUrl(string value)
        {
            if (!Uri.TryCreate(value, UriKind.Absolute, out Uri uri) || uri.Scheme != "https" || uri.Port != 443 ||
                uri.Host != "github.com" || !string.IsNullOrEmpty(uri.UserInfo) || !string.IsNullOrEmpty(uri.Query) ||
                !string.IsNullOrEmpty(uri.Fragment)) return false;
            string path = Uri.UnescapeDataString(uri.AbsolutePath);
            if (!path.StartsWith(DownloadPrefix, StringComparison.Ordinal)) return false;
            string[] segments = path.Substring(DownloadPrefix.Length).Split('/');
            return segments.Length == 2 && !string.IsNullOrWhiteSpace(segments[0]) && !string.IsNullOrWhiteSpace(segments[1]) &&
                segments[0] != "." && segments[0] != ".." && segments[1] != "." && segments[1] != ".." &&
                !path.Contains("\\") && !path.Contains("/../") && !path.EndsWith("/", StringComparison.Ordinal);
        }

        static bool IsTransportUrl(string value, bool allowApi)
        {
            if (!Uri.TryCreate(value, UriKind.Absolute, out Uri uri) || uri.Scheme != "https" || uri.Port != 443 ||
                !string.IsNullOrEmpty(uri.UserInfo)) return false;
            return IsReleaseUrl(value) || uri.Host == "release-assets.githubusercontent.com" || uri.Host == "assets.githubusercontent.com" ||
                (allowApi && value == Latest);
        }

        IEnumerator ReadText(string url, int limit, Action<string> receive, bool allowApi)
        {
            for (int redirect = 0; redirect < 5; redirect++)
            {
                if (!IsTransportUrl(url, allowApi)) yield break;
                using (var request = UnityWebRequest.Get(url))
                {
                    Configure(request, 15);
                    activeRequest = request;
                    var operation = request.SendWebRequest();
                    while (!operation.isDone)
                    {
                        if (request.downloadedBytes > (ulong)limit) { request.Abort(); break; }
                        yield return null;
                    }
                    activeRequest = null;
                    if (request.responseCode >= 300 && request.responseCode <= 399)
                    {
                        string location = request.GetResponseHeader("Location");
                        if (!Uri.TryCreate(new Uri(url), location, out Uri next)) yield break;
                        url = next.AbsoluteUri;
                        continue;
                    }
                    if (request.result == UnityWebRequest.Result.Success && request.downloadedBytes <= (ulong)limit)
                        receive(request.downloadHandler.text);
                    yield break;
                }
            }
        }

        static void Configure(UnityWebRequest request, int timeout)
        {
            request.timeout = timeout;
            request.redirectLimit = 0; // Inspect each HTTPS redirect instead of following arbitrary hosts.
            request.SetRequestHeader("User-Agent", "BattleRoyaleX-APK-Updater");
        }

        IEnumerator Download()
        {
            busy = true;
            verifiedCache = false;
            progress = 0;
            status = "Baixando APK...";
            string part = cachePath + ".part";
            try { Directory.CreateDirectory(Path.GetDirectoryName(cachePath)); DeleteOwnFile(part); }
            catch (Exception) { status = "Não foi possível preparar armazenamento."; busy = false; yield break; }
            string url = pending.apkUrl;
            bool downloaded = false;
            for (int redirect = 0; redirect < 5; redirect++)
            {
                if (!IsTransportUrl(url, false)) break;
                using (var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET))
                {
                    UnityWebRequestAsyncOperation operation = null;
                    try
                    {
                        request.downloadHandler = new DownloadHandlerFile(part) { removeFileOnAbort = true };
                        Configure(request, 120);
                        activeRequest = request;
                        operation = request.SendWebRequest();
                    }
                    catch (Exception)
                    { activeRequest = null; status = "Não foi possível iniciar o download ou gravar o arquivo."; busy = false; }
                    if (operation == null) { DeleteOwnFile(part); yield break; }
                    while (!operation.isDone)
                    {
                        progress = Mathf.Clamp01((float)request.downloadedBytes / pending.apkBytes);
                        if (request.downloadedBytes > (ulong)pending.apkBytes) { request.Abort(); break; }
                        yield return null;
                    }
                    activeRequest = null;
                    if (request.responseCode >= 300 && request.responseCode <= 399)
                    {
                        string location = request.GetResponseHeader("Location");
                        if (!Uri.TryCreate(new Uri(url), location, out Uri next)) break;
                        url = next.AbsoluteUri;
                        DeleteOwnFile(part);
                        continue;
                    }
                    downloaded = request.result == UnityWebRequest.Result.Success;
                    break;
                }
            }
            if (!downloaded) { DeleteOwnFile(part); status = "Download falhou. Confira a conexão e tente novamente."; busy = false; yield break; }
            status = "Conferindo integridade...";
            string hash = null;
            yield return HashFile(part, value => hash = value);
            if (hash == null || !string.Equals(hash, pending.apkSha256, StringComparison.OrdinalIgnoreCase))
            { DeleteOwnFile(part); status = "APK inválido: tamanho ou hash divergente."; busy = false; yield break; }
            try
            {
                DeleteOwnFile(cachePath);
                File.Move(part, cachePath);
                string error = Inspect();
                if (error != string.Empty)
                { DeleteOwnFile(cachePath); status = "APK recusado: pacote, versão ou assinatura incompatível."; }
                else { verifiedCache = true; status = "APK verificado. Toque em Instalar; o Android pedirá confirmação."; }
            }
            catch (Exception) { status = "Não foi possível validar o APK neste aparelho."; }
            busy = false;
        }

        IEnumerator HashFile(string path, Action<string> receive)
        {
            FileStream stream = null;
            SHA256 sha = null;
            try
            {
                if (File.Exists(path) && new FileInfo(path).Length == pending.apkBytes)
                { stream = File.OpenRead(path); sha = SHA256.Create(); }
            }
            catch (Exception) { stream?.Dispose(); stream = null; }
            if (stream == null) yield break;
            try
            {
                byte[] buffer = new byte[65536];
                int processed = 0;
                while (true)
                {
                    int read = -1;
                    try
                    {
                        read = stream.Read(buffer, 0, buffer.Length);
                        if (read > 0) sha.TransformBlock(buffer, 0, read, buffer, 0);
                    }
                    catch (Exception) { }
                    if (read < 0) yield break;
                    if (read == 0) break;
                    processed += read;
                    if (processed >= 1024 * 1024) { processed = 0; yield return null; }
                }
                sha.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
                receive(BitConverter.ToString(sha.Hash).Replace("-", "").ToLowerInvariant());
            }
            finally { stream?.Dispose(); sha?.Dispose(); }
        }

        void Install()
        {
            try
            {
                if (ValidateManifest(pending, InstalledVersion()) != null) { verifiedCache = false; status = "Versão instalada mudou; confira novamente."; return; }
                if (!verifiedCache || !File.Exists(cachePath)) { verifiedCache = false; status = "Baixe o APK primeiro."; return; }
                string error = Inspect(); // Revalidate package/version/certificate immediately before handoff.
                if (error != string.Empty) { verifiedCache = false; status = "APK recusado pela verificação de assinatura."; return; }
#if UNITY_ANDROID && !UNITY_EDITOR
                using (var java = new AndroidJavaClass(Bridge))
                using (var activity = Activity())
                {
                    if (!java.CallStatic<bool>("canInstall", activity))
                    {
                        status = "Autorize instalar deste app no Android, volte e toque em Instalar novamente.";
                        java.CallStatic("requestInstallPermission", activity);
                        return;
                    }
                    java.CallStatic("install", activity, cachePath);
                    status = "Confirme a instalação na tela do Android. Seu jogo e preferências serão preservados.";
                }
#endif
            }
            catch (Exception) { status = "Instalador não pôde abrir. Confira a permissão e tente novamente."; }
        }

        static int InstalledVersion()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (var java = new AndroidJavaClass(Bridge))
            using (var activity = Activity()) return checked((int)java.CallStatic<long>("installedVersionCode", activity));
#else
            throw new PlatformNotSupportedException();
#endif
        }

        string Inspect()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (var java = new AndroidJavaClass(Bridge))
            using (var activity = Activity()) return java.CallStatic<string>("inspectApk", activity, cachePath, Package, (long)pending.versionCode);
#else
            return "Android bridge unavailable";
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        static AndroidJavaObject Activity()
        {
            using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                return player.GetStatic<AndroidJavaObject>("currentActivity");
        }
#endif

        static void DeleteOwnFile(string path)
        {
            try { if (File.Exists(path)) File.Delete(path); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }

        void OnGUI()
        {
            if (pending == null) return;
            Rect safe = Screen.safeArea;
            float scale = Mathf.Max(0.8f, Mathf.Min(Screen.width / 960f, Screen.height / 540f));
            Matrix4x4 previous = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1));
            float x = (safe.x + safe.width / 2) / scale;
            float y = (Screen.height - safe.yMax) / scale + 8;
            if (!panelOpen)
            {
                Rect button = new Rect(x - 90, y, 180, 44);
                Shield(button, scale);
                if (GUI.Button(button, "ATUALIZAÇÃO")) panelOpen = true;
            }
            else
            {
                float width = Mathf.Min(460, safe.width / scale - 16);
                Rect panel = new Rect(x - width / 2, y, width, 230);
                Shield(panel, scale);
                GUI.Box(panel, "Battle Royale X — atualização");
                GUI.Label(new Rect(panel.x + 12, panel.y + 30, width - 24, 92), status);
                if (busy) GUI.Label(new Rect(panel.x + 12, panel.y + 125, width - 24, 30), "Aguarde... " + Mathf.RoundToInt(progress * 100) + "%");
                bool previousEnabled = GUI.enabled;
                GUI.enabled = !busy;
                if (GUI.Button(new Rect(panel.x + 12, panel.y + 163, width - 120, 48), verifiedCache ? "Instalar (confirmação Android)" : "Confirmar download do APK"))
                {
                    if (verifiedCache) Install(); else StartCoroutine(Download());
                }
                GUI.enabled = previousEnabled;
                if (GUI.Button(new Rect(panel.xMax - 100, panel.y + 163, 88, 48), "Fechar")) panelOpen = false;
            }
            GUI.matrix = previous;
        }

        void Shield(Rect guiRect, float scale)
        {
            // OnGUI is not an EventSystem target. This transparent raycast shield prevents touches
            // on the update panel from starting the floating joystick or a skill underneath it.
            if (inputBlocker == null) return;
            inputBlocker.gameObject.SetActive(true);
            inputBlocker.anchoredPosition = new Vector2(guiRect.x * scale, Screen.height - guiRect.yMax * scale);
            inputBlocker.sizeDelta = new Vector2(guiRect.width * scale, guiRect.height * scale);
        }

        void OnDestroy()
        {
            activeRequest?.Abort();
            if (instance == this) instance = null;
        }
    }
}
