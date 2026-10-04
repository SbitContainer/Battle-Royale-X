'use strict';
const fs = require('node:fs');
const path = require('node:path');
const ROOT = path.resolve(__dirname, '../..');

function localFile(root, relative) {
  if (typeof relative !== 'string' || !relative || path.isAbsolute(relative) || /^[a-z]:/i.test(relative))
    throw new Error('invalid_local_path');
  const base = fs.realpathSync(root);
  const resolved = fs.realpathSync(path.resolve(base, relative));
  const rel = path.relative(base, resolved);
  if (rel === '..' || rel.startsWith('..' + path.sep) || path.isAbsolute(rel)) throw new Error('outside_root');
  if (!fs.statSync(resolved).isFile()) throw new Error('not_file');
  return resolved;
}

function validateMap(root, map) {
  if (map.schemaVersion !== 1 || !Array.isArray(map.sectors) || !map.sectors.length) throw new Error('invalid_map');
  const ids = new Set(map.sectors.map(s => s.id));
  if (ids.size !== map.sectors.length || map.sectors.some(s => typeof s.id !== 'string' || !s.id)) throw new Error('invalid_sector_id');
  let references = 0;
  for (const sector of map.sectors) {
    if (!Array.isArray(sector.files) || !sector.files.length || !Array.isArray(sector.tests) || !sector.tests.length
        || !Array.isArray(sector.consumers)) throw new Error('incomplete_sector');
    for (const consumer of sector.consumers) if (!ids.has(consumer)) throw new Error('unknown_consumer:' + consumer);
    for (const item of sector.files) {
      const text = fs.readFileSync(localFile(root, item.path), 'utf8');
      if (!Array.isArray(item.symbols) || !item.symbols.length) throw new Error('missing_symbols');
      for (const symbol of item.symbols) if (typeof symbol !== 'string' || !symbol || !text.includes(symbol))
        throw new Error('missing_symbol:' + item.path + ':' + symbol);
      references++;
    }
    for (const test of sector.tests) { localFile(root, test); references++; }
  }
  return { sectors: ids.size, references };
}

function documents(root) {
  const names = ['AGENTS.md', 'Docs/HANDOFF.md', 'Docs/PROJETO.md', 'Docs/MAPA.md'];
  function visit(dir) {
    for (const entry of fs.readdirSync(path.join(root, dir), { withFileTypes: true })) {
      const rel = dir + '/' + entry.name;
      if (entry.isDirectory()) visit(rel);
      else if (entry.isFile() && entry.name.endsWith('.md')) names.push(rel);
    }
  }
  visit('Docs/engenharia');
  for (const policy of ['FLUXO','AGENTES','BUGS_E_TESTES','DADOS_E_COMUNICACAO','PUBLICACAO_E_ROLLBACK','QUALIDADE'])
    localFile(root, 'Docs/engenharia/' + policy + '.md');
  let links = 0;
  for (const name of names) {
    const text = fs.readFileSync(localFile(root, name), 'utf8');
    // Simple repository Markdown links only; not a Markdown renderer or anchor validator.
    for (const match of text.matchAll(/\[[^\]]*\]\(([^)]+)\)/g)) {
      const target = match[1].split('#')[0];
      if (!target || /^[a-z]+:\/\//i.test(target)) continue;
      localFile(root, path.join(path.dirname(name), decodeURIComponent(target)));
      links++;
    }
  }
  return { documents: names.length, localLinks: links };
}

if (require.main === module) {
  try {
    const map = JSON.parse(fs.readFileSync(path.join(ROOT, 'Docs/engenharia/mapa.json'), 'utf8'));
    console.log(JSON.stringify({ status: 'PASS', ...validateMap(ROOT, map), ...documents(ROOT),
      limits: 'Static references and local links only; not Unity gameplay or Android validation.' }, null, 2));
  } catch (error) { console.error(error.message); process.exitCode = 1; }
}
module.exports = { localFile, validateMap, documents };
