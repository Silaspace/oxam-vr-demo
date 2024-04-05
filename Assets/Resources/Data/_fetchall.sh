#!/bin/bash

# Find all directories in the current directory and execute the _fetch.sh script
for d in */ ; do
    if [ -f "${d}_fetch.sh" ]; then
        echo "Running _fetch.sh in $d"
        (cd "$d" && ./_fetch.sh)
    fi
done
