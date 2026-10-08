import hashlib, json, sys
from pathlib import Path
folder, lock = map(Path, sys.argv[1:])
files = {p.name: hashlib.sha256(p.read_bytes()).hexdigest() for p in sorted(folder.iterdir()) if p.is_file()}
expected = {"posthog-6.8.1.jar", "posthog-android-3.38.2.aar", "curtains-1.2.5.aar"}
if set(files) != expected: raise SystemExit("Missing or unexpected native dependencies: " + str(set(files)))
if lock.exists():
 if json.loads(lock.read_text()) != files: raise SystemExit("Native dependency integrity mismatch")
else:
 lock.parent.mkdir(parents=True, exist_ok=True); lock.write_text(json.dumps(files, indent=2)+"\n")
print("Native artifact integrity verified")
