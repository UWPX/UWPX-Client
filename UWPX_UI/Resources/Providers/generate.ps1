# Clone https://invent.kde.org/melvo/xmpp-providers/-/tree/master
# Then execute this and use the contents inside the 'output' directory.

if (Test-Path -Path output) {
    Remove-Item -Recurse -Force output
}

mkdir output

python.exe .\filter.py -A -s
Move-Item providers-A.json output/providers-A-simple.json
python.exe .\filter.py -A
Move-Item providers-A.json output/providers-A.json

python.exe .\filter.py -B -s
Move-Item providers-B.json output/providers-B-simple.json
python.exe .\filter.py -B
Move-Item providers-B.json output/providers-B.json

python.exe .\filter.py -C -s
Move-Item providers-C.json output/providers-C-simple.json
python.exe .\filter.py -C
Move-Item providers-C.json output/providers-C.json