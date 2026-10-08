from pathlib import Path
import re, subprocess
root = Path(__file__).resolve().parents[1]
files = subprocess.check_output(["git", "ls-files"], cwd=root, text=True).splitlines()
for name in files:
 data=(root/name).read_bytes()
 if re.search(rb"(?i)\x62\x6c\x6f\x6f\x6d[ &_-]*(?:fly|y)|phc_[A-Za-z0-9]{20,}",data):
  raise SystemExit("Private identifier or actual project key in " + name)
print("Public tree scan passed")
