#!/bin/zsh

export $(grep -v '^#' .env | xargs) && dotnet ef migrations add UserDefinedPrompts
