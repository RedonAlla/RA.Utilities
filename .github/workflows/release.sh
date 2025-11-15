#!/bin/bash

# exit when any command fails
set -e

# --- Arguments ---
PACKAGE_NAME=$1
NEW_VERSION=$2

# --- Validation ---
if [ -z "$PACKAGE_NAME" ] || [ -z "$NEW_VERSION" ]; then
  echo "Usage: ./scripts/release.sh <package-name> <new-version>"
  echo "Example: ./scripts/release.sh RA.Utilities.Api 10.0.3"
  exit 1
fi

# --- Variables ---
PROJECT_FILE_PATH=$(find . -type f -name "${PACKAGE_NAME}.csproj" | head -n 1)
TAG_NAME="${PACKAGE_NAME}-v${NEW_VERSION}"

# --- Pre-flight Checks ---
if [ -z "$PROJECT_FILE_PATH" ]; then
    echo "Error: Could not find .csproj file for package '${PACKAGE_NAME}'"
    exit 1
fi

if [[ -n $(git status --porcelain) ]]; then
  echo "Error: Working directory is not clean. Please commit or stash your changes."
  exit 1
fi

echo "Updating local branch..."
git pull

# --- Script Logic ---
echo "Project file found: ${PROJECT_FILE_PATH}"
echo "Updating version in .csproj to ${NEW_VERSION}..."

# Using sed to update the version. Works on both GNU and macOS sed.
sed -i.bak "s|<Version>.*</Version>|<Version>${NEW_VERSION}</Version>|" "${PROJECT_FILE_PATH}"
rm "${PROJECT_FILE_PATH}.bak" # Clean up backup file

echo "Committing version bump..."
git add "${PROJECT_FILE_PATH}"
git commit -m "chore(release): Bump ${PACKAGE_NAME} version to ${NEW_VERSION}"

echo "Creating tag ${TAG_NAME}..."
git tag -a "${TAG_NAME}" -m "Release ${PACKAGE_NAME} v${NEW_VERSION}"

echo "Pushing commit and tag to remote..."
git push
git push origin "${TAG_NAME}"

echo "✅ Done! Release for ${PACKAGE_NAME} version ${NEW_VERSION} is ready."
echo "The GitHub Actions workflow should now be triggered."