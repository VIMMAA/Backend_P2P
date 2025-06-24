#!/bin/bash
set -e

dotnet ef database update

dotnet BackendP2P.dll