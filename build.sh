#!/bin/bash

weaverFolder=/home/hifu/git/CaptainTurretBeacon/CaptainTurretBeacon/NetworkWeaver
weaverLibsFolder=/home/hifu/git/CaptainTurretBeacon/CaptainTurretBeacon/networkweaverlibs

modName=CaptainTurretBeacon
modBinFolder=/home/hifu/git/CaptainTurretBeacon/CaptainTurretBeacon/bin
modBuildFolder=/home/hifu/git/CaptainTurretBeacon/CaptainTurretBeacon/bin/Debug/netstandard2.1
modFolderBase=/home/hifu/git/CaptainTurretBeacon
assetBundleFolder=/home/hifu/CaptainTurretBeacon/Assets/captainturretbeacon

testingProfileName=testing
modGaleFolder=/home/hifu/.local/share/com.kesomannen.gale/riskofrain2/profiles/$testingProfileName/BepInEx/plugins

rm -rf "$modBinFolder"

dotnet restore
dotnet build

# delete weaver folder if patch already exists, else weaver doesn't work
if [ -e "$weaverFolder"/CaptainTurretBeacon.dll ]
then
    echo "deleting previous weaver folder dll and pdb"
    rm -rf "$weaverFolder"/CaptainTurretBeacon.dll
    rm -rf "$weaverFolder"/CaptainTurretBeacon.pdb
fi

# copy dll and pdb into networkweaver folder
echo "copying CaptainTurretBeacon dll and pdb to the weaver folder"
cp "$modBuildFolder"/CaptainTurretBeacon.dll "$weaverFolder"
cp "$modBuildFolder"/CaptainTurretBeacon.pdb "$weaverFolder"

# patch dlls with weaver (coreModulePath, networkingPath, outputPath, dllToPatchPath, dllLibsPath)
echo "running weaver"
wine "$weaverFolder"/Unity.UNetWeaver.exe "$weaverLibsFolder/UnityEngine.CoreModule.dll" "$weaverLibsFolder/com.unity.multiplayer-hlapi.Runtime.dll" "$weaverFolder" "$weaverFolder/CaptainTurretBeacon.dll" "$weaverLibsFolder"

# copy over files to gale profile folder and mod release folder
echo "copying over files to gale profile folder and mod release folder"
cp "$weaverFolder"/CaptainTurretBeacon.dll "$modGaleFolder"
cp "$weaverFolder"/CaptainTurretBeacon.pdb "$modGaleFolder"
cp "$assetBundleFolder" "$modGaleFolder"

# launch game with the testing profile if provided any argument
if [ -n "$1" ]
then
    echo "launching game"
    gale --game riskofrain2 --profile "$testingProfileName" --launch --no-gui
fi