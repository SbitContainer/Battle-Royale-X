package com.sbitcontainer.brxupdates;

import android.content.ContentProvider;
import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.MatrixCursor;
import android.net.Uri;
import android.os.ParcelFileDescriptor;
import android.provider.OpenableColumns;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.IOException;

/** Private provider granting read-only access to one prepared installer APK. */
public final class ApkProvider extends ContentProvider {
    static File apkFile(Context context) { return new File(context.getCacheDir(), "brxupdates/update.apk"); }
    private File resolve(Uri uri) throws FileNotFoundException {
        if (getContext() == null || !"content".equals(uri.getScheme())
            || !(getContext().getPackageName() + ".brxupdates").equals(uri.getAuthority())
            || !"/update.apk".equals(uri.getEncodedPath()) || uri.getQuery() != null || uri.getFragment() != null)
            throw new FileNotFoundException("APK_URI_NOT_ALLOWED");
        try {
            File cache = getContext().getCacheDir().getCanonicalFile();
            File expected = new File(cache, "brxupdates/update.apk");
            File actual = expected.getCanonicalFile();
            if (!expected.getAbsolutePath().equals(actual.getAbsolutePath()) || !actual.isFile())
                throw new FileNotFoundException("APK_CACHE_NOT_ALLOWED");
            return actual;
        } catch (IOException error) { throw new FileNotFoundException("APK_CACHE_UNAVAILABLE"); }
    }
    @Override public boolean onCreate() { return true; }
    @Override public String getType(Uri uri) {
        try { resolve(uri); return "application/vnd.android.package-archive"; }
        catch (FileNotFoundException error) { throw new IllegalArgumentException(error.getMessage()); }
    }
    @Override public ParcelFileDescriptor openFile(Uri uri, String mode) throws FileNotFoundException {
        if (!"r".equals(mode)) throw new FileNotFoundException("APK_READ_ONLY");
        return ParcelFileDescriptor.open(resolve(uri), ParcelFileDescriptor.MODE_READ_ONLY);
    }
    @Override public Cursor query(Uri uri, String[] projection, String selection, String[] selectionArgs, String sortOrder) {
        try {
            File file = resolve(uri);
            String[] columns = projection == null
                ? new String[] { OpenableColumns.DISPLAY_NAME, OpenableColumns.SIZE } : projection;
            MatrixCursor cursor = new MatrixCursor(columns, 1);
            Object[] values = new Object[columns.length];
            for (int i = 0; i < columns.length; i++) {
                if (OpenableColumns.DISPLAY_NAME.equals(columns[i])) values[i] = "update.apk";
                else if (OpenableColumns.SIZE.equals(columns[i])) values[i] = file.length();
            }
            cursor.addRow(values);
            return cursor;
        } catch (FileNotFoundException error) { throw new IllegalArgumentException(error.getMessage()); }
    }
    @Override public Uri insert(Uri uri, ContentValues values) { throw new UnsupportedOperationException("APK_READ_ONLY"); }
    @Override public int update(Uri uri, ContentValues values, String selection, String[] args) { throw new UnsupportedOperationException("APK_READ_ONLY"); }
    @Override public int delete(Uri uri, String selection, String[] args) { throw new UnsupportedOperationException("APK_READ_ONLY"); }
}
