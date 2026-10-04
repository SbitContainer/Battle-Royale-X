package com.sbitcontainer.brxupdates;

import android.app.Activity;
import android.content.Intent;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.pm.Signature;
import android.net.Uri;
import android.os.Build;
import android.provider.Settings;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.HashSet;
import java.util.Set;

/** Native boundary: no network, credentials or unattended installation. */
public final class ApkUpdates {
    private ApkUpdates() { }
    private static int flags() {
        return Build.VERSION.SDK_INT >= 28
            ? PackageManager.GET_SIGNING_CERTIFICATES : PackageManager.GET_SIGNATURES;
    }
    private static long code(PackageInfo info) {
        return Build.VERSION.SDK_INT >= 28 ? info.getLongVersionCode() : info.versionCode;
    }
    public static long installedVersionCode(Activity activity) throws Exception {
        return code(activity.getPackageManager().getPackageInfo(activity.getPackageName(), flags()));
    }
    private static Set<String> certificates(PackageInfo info) throws IOException {
        Signature[] signatures;
        if (Build.VERSION.SDK_INT >= 28) {
            if (info.signingInfo == null) throw new IOException("APK_WITHOUT_SIGNING_INFO");
            signatures = info.signingInfo.getApkContentsSigners();
        } else signatures = info.signatures;
        if (signatures == null || signatures.length == 0) throw new IOException("APK_WITHOUT_CERTIFICATE");
        Set<String> result = new HashSet<String>();
        for (Signature signature : signatures) result.add(signature.toCharsString());
        return result;
    }
    private static File source(Activity activity, String path) throws IOException {
        if (path == null || path.length() == 0) throw new IOException("APK_PATH_MISSING");
        File file = new File(path).getCanonicalFile();
        File[] roots = { activity.getFilesDir(), activity.getCacheDir(), activity.getExternalFilesDir(null) };
        File cacheRoot = activity.getCacheDir().getCanonicalFile();
        File prepared = new File(cacheRoot, "brxupdates/update.apk");
        boolean allowed = file.equals(prepared) && prepared.equals(prepared.getCanonicalFile());
        for (File root : roots) {
            if (root != null) {
                File expected = new File(root.getCanonicalFile(), "BRXUpdates/update.apk");
                if (file.equals(expected) && expected.equals(expected.getCanonicalFile())) allowed = true;
            }
        }
        if (!allowed || !file.isFile()) throw new IOException("APK_PATH_NOT_ALLOWED");
        return file;
    }
    private static PackageInfo inspect(Activity activity, File file, String expectedPackage, long expectedCode) throws Exception {
        String ownPackage = activity.getPackageName();
        if (!ownPackage.equals(expectedPackage)) throw new IOException("APK_PACKAGE_NOT_THIS_APP");
        PackageManager manager = activity.getPackageManager();
        PackageInfo archive = manager.getPackageArchiveInfo(file.getAbsolutePath(), flags());
        if (archive == null) throw new IOException("APK_NOT_READABLE");
        if (!expectedPackage.equals(archive.packageName)) throw new IOException("APK_PACKAGE_MISMATCH");
        PackageInfo installed = manager.getPackageInfo(ownPackage, flags());
        if (code(archive) != expectedCode || expectedCode <= code(installed)) throw new IOException("APK_VERSION_NOT_NEWER_OR_MISMATCH");
        if (!certificates(installed).equals(certificates(archive))) throw new IOException("APK_CERTIFICATE_MISMATCH");
        return archive;
    }
    public static String inspectApk(Activity activity, String path, String expectedPackage, long expectedCode) {
        try { inspect(activity, source(activity, path), expectedPackage, expectedCode); return ""; }
        catch (Exception error) { return error.getMessage() == null ? "APK_VALIDATION_FAILED" : error.getMessage(); }
    }
    public static boolean canInstall(Activity activity) {
        return activity.getPackageManager().canRequestPackageInstalls();
    }
    public static void requestInstallPermission(Activity activity) {
        activity.startActivity(new Intent(Settings.ACTION_MANAGE_UNKNOWN_APP_SOURCES,
            Uri.parse("package:" + activity.getPackageName())));
    }
    public static void install(Activity activity, String path) throws Exception {
        if (!canInstall(activity)) throw new IOException("INSTALL_PERMISSION_REQUIRED");
        File original = source(activity, path);
        PackageInfo candidate = activity.getPackageManager().getPackageArchiveInfo(original.getAbsolutePath(), flags());
        if (candidate == null) throw new IOException("APK_NOT_READABLE");
        long version = code(candidate);
        inspect(activity, original, activity.getPackageName(), version);
        File target = new File(activity.getCacheDir().getCanonicalFile(), "brxupdates/update.apk");
        if (!target.equals(target.getCanonicalFile())) throw new IOException("APK_CACHE_NOT_ALLOWED");
        if (!original.equals(target)) {
            File directory = target.getParentFile();
            if (!directory.isDirectory() && !directory.mkdirs()) throw new IOException("APK_CACHE_UNAVAILABLE");
            try (FileInputStream input = new FileInputStream(original); FileOutputStream output = new FileOutputStream(target)) {
                byte[] buffer = new byte[65536];
                int count;
                while ((count = input.read(buffer)) != -1) output.write(buffer, 0, count);
                output.getFD().sync();
            }
        }
        // Reinspect the actual provider bytes, not just the original download.
        inspect(activity, target, activity.getPackageName(), version);
        Uri uri = Uri.parse("content://" + activity.getPackageName() + ".brxupdates/update.apk");
        Intent intent = new Intent(Intent.ACTION_VIEW);
        intent.setDataAndType(uri, "application/vnd.android.package-archive");
        intent.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
        activity.startActivity(intent);
    }
}
