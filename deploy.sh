#!/bin/bash
cd /opt/chicane-backend
git pull
docker compose build
docker compose up -d

# Prime Redis cache — pre-fetch common endpoints so first user gets instant response
sleep 5
for endpoint in \
  "sessions?year=$(date +%Y)" \
  "meetings?year=$(date +%Y)" \
  "drivers?session_key=$(curl -s http://localhost:5000/api/proxy/sessions?year=$(date +%Y)\&session_type=Race | python3 -c 'import sys,json;d=json.load(sys.stdin);print(d[0][\"session_key\"] if d else 0)' 2>/dev/null || echo 0)"; do
  echo "[Prime] warming $endpoint"
  curl -s "http://localhost:5000/api/proxy/$endpoint" > /dev/null
done
curl -s "http://localhost:5000/api/weather?city=Monza&country=IT" > /dev/null
echo "[Prime] cache warmed"
