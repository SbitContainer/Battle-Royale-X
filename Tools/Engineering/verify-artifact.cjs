'use strict';
const fs = require('node:fs');
const path = require('node:path');
const { verifyFile } = require('./verify-file.cjs');
const { localFile } = require('./validate.cjs');
const ROOT = path.resolve(__dirname, '../..');

async function verifyArtifact(root, descriptor) {
  if (descriptor.schemaVersion !== 1 || descriptor.packageId !== 'com.sbitcontainer.battleroyalex.prototype'
      || !Number.isSafeInteger(descriptor.versionCode) || descriptor.versionCode < 1
      || typeof descriptor.versionName !== 'string' || !descriptor.versionName.trim()
      || typeof descriptor.file !== 'string' || !descriptor.file.endsWith('.apk')) throw new Error('invalid_artifact_descriptor');
  const result = await verifyFile(localFile(root, descriptor.file), descriptor);
  return { status: 'PASS', file: descriptor.file, ...result,
    limits: 'Bytes only. APK signature/package/version and installed device require separate checks.' };
}
if (require.main === module) {
  (async () => {
    if (process.argv.length !== 3) throw new Error('Usage: node Tools/Engineering/verify-artifact.cjs <repo-relative-manifest.json>');
    const descriptor = JSON.parse(fs.readFileSync(localFile(ROOT, process.argv[2]), 'utf8'));
    console.log(JSON.stringify(await verifyArtifact(ROOT, descriptor), null, 2));
  })().catch(error => { console.error(error.message); process.exitCode = 1; });
}
module.exports = { verifyArtifact };
