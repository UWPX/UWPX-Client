# Clone https://invent.kde.org/melvo/xmpp-providers/-/tree/master
# Then execute this and use the contents inside the 'output' directory.

git clone https://invent.kde.org/melvo/xmpp-providers.git
cd xmpp-providers

python.exe .\filter.py -A -s
Move-Item -Force providers-A.json ../providers-A-simple.json
python.exe .\filter.py -A
Move-Item -Force providers-A.json ../providers-A.json

python.exe .\filter.py -B -s
Move-Item -Force providers-B.json ../providers-B-simple.json
python.exe .\filter.py -B
Move-Item -Force providers-B.json ../providers-B.json

python.exe .\filter.py -C -s
Move-Item -Force providers-C.json ../providers-C-simple.json
python.exe .\filter.py -C
Move-Item -Force providers-C.json ../providers-C.json

cd ..
Remove-Item -Recurse -Force xmpp-providers