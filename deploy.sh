#!/bin/bash
cd /opt/chicane-backend
git pull
docker compose build
docker compose up -d
