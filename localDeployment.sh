﻿NEWTAG=$1
EXISTINGTAG="${2:-$(git describe --tag --abbrev=0)}"
echo $NEWTAG
echo $EXISTINGTAG
if [ -z "$(git status --porcelain)" ]; then 
    # clean working dir
    read -p "This will replace $EXISTINGTAG with $NEWTAG . Are you sure? " -n 1 -r
    if [[ $REPLY =~ ^[Yy]$ ]]
    then
        FILES=("ZDragon.NET.sln" "CLI/CLI.csproj" "Compiler/Compiler.csproj" "CompilerTests/CompilerTests.csproj" "Mapper.XSD/Mapper.XSD.csproj" "README.md"
        "Mapper.HTML/Mapper.HTML.csproj" "Mapper.JSON/Mapper.JSON.csproj"
        "CLI/Program.cs"
        )
        git checkout master

        for FILE in "${FILES[@]}"
        do
            sed -i '' -e "s/$EXISTINGTAG/$NEWTAG/g" $FILE
        done
        git add .
        git commit -m "Release $NEWTAG"
        git tag $NEWTAG
        git push origin master --tags
    
        else
        echo "Exiting"
        exit 0
    fi
else 
    # uncommited changes
    echo "You have uncommited changes. Stash, commit or remove them. Aborting."
    exit 1
fi

function askConfirm() {
    
}