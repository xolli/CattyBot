#!/bin/zsh

d=$(date +%Y-%m-%d)
docker build -t docker.io/xolli/catty -t docker.io/xolli/catty:$d -f Containerfile . 
