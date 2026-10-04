'use strict';
const { test } = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const crypto = require('node:crypto');
const { verifyFile } = require('./verify-file.cjs');
const { validateMap, localFile, documents } = require('./validate.cjs');
const { verifyArtifact } = require('./verify-artifact.cjs');

function fixture(t) {
  const root = fs.mkdtempSync(path.join(os.tmpdir(), 'brx-engineering-test-'));
  // This unique directory contains only this test's synthetic files.
  t.after(() => fs.rmSync(root, { recursive: true, force: true }));
  fs.writeFileSync(path.join(root, 'sample.apk'), 'synthetic, not an APK');
  fs.writeFileSync(path.join(root, 'script.cs'), 'class Example { void TryUse() {} }');
  fs.writeFileSync(path.join(root, 'test.cs'), 'synthetic test reference');
  const descriptor = { schemaVersion: 1, file: 'sample.apk', packageId: 'com.sbitcontainer.battleroyalex.prototype',
    versionCode: 11, versionName: '0.9.2-effects-lab', sizeBytes: 21,
    sha256: crypto.createHash('sha256').update('synthetic, not an APK').digest('hex') };
  descriptor.sizeBytes = fs.statSync(path.join(root, 'sample.apk')).size;
  const map = { schemaVersion: 1, sectors: [{ id:'input', files:[{path:'script.cs', symbols:['TryUse']}], consumers:[], tests:['test.cs'] }] };
  return { root, descriptor, map };
}

test('accepts exact bytes including a Unity version suffix', async t => { const f=fixture(t); assert.equal((await verifyArtifact(f.root,f.descriptor)).status,'PASS'); });
test('rejects different bytes of the same size', async t => { const f=fixture(t); fs.writeFileSync(path.join(f.root,'sample.apk'),'x'.repeat(f.descriptor.sizeBytes)); await assert.rejects(verifyArtifact(f.root,f.descriptor),/hash_mismatch/); });
test('rejects truncated artifact', async t => { const f=fixture(t); fs.writeFileSync(path.join(f.root,'sample.apk'),'x'); await assert.rejects(verifyArtifact(f.root,f.descriptor),/size_mismatch/); });
test('rejects oversized artifact', async t => { const f=fixture(t); fs.appendFileSync(path.join(f.root,'sample.apk'),'x'); await assert.rejects(verifyArtifact(f.root,f.descriptor),/size_mismatch/); });
test('rejects invalid hash descriptor', async t => { const f=fixture(t); f.descriptor.sha256='bad'; await assert.rejects(verifyArtifact(f.root,f.descriptor),/invalid_descriptor/); });
test('rejects wrong package descriptor', async t => { const f=fixture(t); f.descriptor.packageId='other'; await assert.rejects(verifyArtifact(f.root,f.descriptor),/invalid_artifact_descriptor/); });
test('rejects absolute paths', t => { const f=fixture(t); assert.throws(()=>localFile(f.root,path.join(f.root,'sample.apk')),/invalid_local_path/); });
test('rejects path escape to an existing file', t => { const f=fixture(t); const other=fixture(t); assert.throws(()=>localFile(f.root,path.relative(f.root,path.join(other.root,'sample.apk'))),/outside_root/); });
test('missing file is a failure', async t => { const f=fixture(t); await assert.rejects(verifyFile(path.join(f.root,'missing.apk'),f.descriptor),/ENOENT/); });
test('accepts mapped symbols and test paths', t => { const f=fixture(t); assert.deepEqual(validateMap(f.root,f.map),{sectors:1,references:2}); });
test('rejects stale symbol', t => { const f=fixture(t); f.map.sectors[0].files[0].symbols=['RemovedMethod']; assert.throws(()=>validateMap(f.root,f.map),/missing_symbol/); });
test('rejects unknown consumer', t => { const f=fixture(t); f.map.sectors[0].consumers=['network']; assert.throws(()=>validateMap(f.root,f.map),/unknown_consumer/); });
test('rejects duplicate sector', t => { const f=fixture(t); f.map.sectors.push(f.map.sectors[0]); assert.throws(()=>validateMap(f.root,f.map),/invalid_sector_id/); });
test('rejects missing mapped test', t => { const f=fixture(t); f.map.sectors[0].tests=['removed.cs']; assert.throws(()=>validateMap(f.root,f.map),/ENOENT/); });

function docFixture(t) {
  const f=fixture(t);
  fs.mkdirSync(path.join(f.root,'Docs/engenharia'),{recursive:true});
  for(const name of ['AGENTS.md','Docs/HANDOFF.md','Docs/PROJETO.md','Docs/MAPA.md',
    ...['FLUXO','AGENTES','BUGS_E_TESTES','DADOS_E_COMUNICACAO','PUBLICACAO_E_ROLLBACK','QUALIDADE'].map(n=>'Docs/engenharia/'+n+'.md')])
    fs.writeFileSync(path.join(f.root,name),'# Synthetic policy\n');
  return f;
}
test('validates local documentation links', t => {
  const f=docFixture(t); fs.writeFileSync(path.join(f.root,'AGENTS.md'),'[Start](Docs/HANDOFF.md)');
  assert.deepEqual(documents(f.root),{documents:10,localLinks:1});
});
test('rejects a broken documentation link', t => {
  const f=docFixture(t); fs.writeFileSync(path.join(f.root,'AGENTS.md'),'[Missing](Docs/removed.md)');
  assert.throws(()=>documents(f.root),/ENOENT/);
});
