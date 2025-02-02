﻿#!/bin/bash
rm -rf lib/

# Build for the wanted platforms and place it in releaseTemp
RELEASE_TARGETS=("win-x64" "osx-x64")
for target in "${RELEASE_TARGETS[@]}"
do
    dotnet publish CLI -c Release --runtime "$target" /p:PublishSingleFile=true -o releaseTemp
done

# Move from releaseTemp to lib, this way unneeded files (such as .pdb files) are omitted from the release
mkdir lib
mv releaseTemp/CLI lib/ckc
mv releaseTemp/CLI.exe lib/ckc.exe
rm -rf releaseTemp/


git config user.email "travis@travis-ci.org"
git config user.name "Travis CI"
git remote rm origin
git remote add origin https://Baudin999:"$2"@github.com/Baudin999/ZDragon.NET.git
git add .
git commit --message "Travis Release: $NEWTAG"

# Use quiet to prevent leaking of tokens
git push --quiet origin master