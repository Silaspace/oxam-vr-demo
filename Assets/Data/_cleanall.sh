#!/bin/bash

# Find all directories in the current directory and execute the _clean.sh script
for d in */ ; do
    if [ -f "${d}_clean.sh" ]; then
        echo "Running _clean.sh in $d"
        (cd "$d" && ./_clean.sh)
    fi
done
