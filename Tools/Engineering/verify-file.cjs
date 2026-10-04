// Extracted verifyFile from reusable kit v1.1 primitives.cjs. See PROVENIENCIA.md.
'use strict';
const crypto = require('node:crypto');
const fs = require('node:fs');

// Trusted descriptor required. Integrity of bytes is not publisher authentication.
async function verifyFile(filePath, descriptor) {
  if (!descriptor || !Number.isSafeInteger(descriptor.sizeBytes) || descriptor.sizeBytes < 0
      || !/^[a-f0-9]{64}$/i.test(descriptor.sha256 || '')) throw new TypeError('invalid_descriptor');
  const hash = crypto.createHash('sha256');
  let size = 0;
  for await (const chunk of fs.createReadStream(filePath)) {
    size += chunk.length;
    if (size > descriptor.sizeBytes) throw new Error('size_mismatch');
    hash.update(chunk);
  }
  if (size !== descriptor.sizeBytes) throw new Error('size_mismatch');
  const sha256 = hash.digest('hex');
  if (sha256 !== descriptor.sha256.toLowerCase()) throw new Error('hash_mismatch');
  return { sizeBytes: size, sha256 };
}
module.exports = { verifyFile };
