<<<<<<< Updated upstream
NEWTAG=$1
=======
﻿NEWTAG=$1
>>>>>>> Stashed changes
EXISTINGTAG="${2:-$(git describe --tag --abbrev=0)}"
# "${1:-/usr/local/bin/}"
echo $EXISTINGTAG
if [ -z "$(git status --porcelain)" ]; then 
	# clean working dir
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
<<<<<<< Updated upstream
		git push origin master
=======
        git tag $NEWTAG
		git push origin master --tags
>>>>>>> Stashed changes
else 
	# uncommited changes
	echo "You have uncommited changes. Stash, commit or remove them. Aborting."
	exit 1
fi