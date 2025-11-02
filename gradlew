#!/usr/bin/env bash
# Minimal gradle wrapper bootstrapper
# If the standard gradle-wrapper.jar is present, delegate to it. Otherwise download
# the Gradle distribution declared in gradle/wrapper/gradle-wrapper.properties and
# run the gradle binary directly.
set -euo pipefail
DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

WRAPPER_JAR="$DIR/gradle/wrapper/gradle-wrapper.jar"
PROPERTIES="$DIR/gradle/wrapper/gradle-wrapper.properties"

if [ -f "$WRAPPER_JAR" ]; then
	exec java -jar "$WRAPPER_JAR" "$@"
fi

if [ ! -f "$PROPERTIES" ]; then
	echo "Missing $PROPERTIES and gradle-wrapper.jar. Cannot proceed." >&2
	exit 1
fi

# Read distributionUrl from properties
DIST_URL_RAW=$(grep distributionUrl "$PROPERTIES" | sed -E "s/.*=\s*//")
# Unescape backslash-escaped characters and remove stray backslashes so we get a proper URL
# e.g. convert "https\://..." to "https://..."
DIST_URL=$(printf '%b' "$DIST_URL_RAW" | sed 's#\\##g')
if [ -z "$DIST_URL" ]; then
	echo "Could not find distributionUrl in $PROPERTIES" >&2
	exit 1
fi

DOWNLOAD_DIR="$HOME/.gradle/wrapper/dists"
mkdir -p "$DOWNLOAD_DIR"

# compute a name for this distribution
DIST_FILE="$DOWNLOAD_DIR/$(echo "$DIST_URL" | sed -E 's#https?://##; s#[/?]#_#g')"

if [ ! -f "$DIST_FILE" ]; then
	echo "Downloading Gradle distribution from $DIST_URL..."
	if command -v curl >/dev/null 2>&1; then
		curl -L -o "$DIST_FILE" "$DIST_URL"
	elif command -v wget >/dev/null 2>&1; then
		wget -O "$DIST_FILE" "$DIST_URL"
	else
		echo "Neither curl nor wget is available to download Gradle." >&2
		exit 1
	fi
fi

TMPDIR=$(mktemp -d)
unzip -q "$DIST_FILE" -d "$TMPDIR"
GRADLE_BIN=$(find "$TMPDIR" -type f -path "*/bin/gradle" | head -n1)
if [ -z "$GRADLE_BIN" ]; then
	echo "Could not find gradle binary in distribution" >&2
	rm -rf "$TMPDIR"
	exit 1
fi

exec "$GRADLE_BIN" "$@"
