# mod manager

simple console app to sync mods between client and server

creates and maintains json database:
- installed.json - mods that are currently installed in .minecraft/mods folder
- known.json - all mods that were ever installed
- backup.json - backup of installed.json before overriding 

It's helpful when deciding which mods could be added/removed.
Values are edited directly in the json files.

# useful MC commands:

/kill @e[type=!minecraft:player]
/kill @e[type=item]
/spark profiler --only-ticks-over 150
/spark tickmonitor --threshold-tick 150

# run server
ssh -i ~/.ssh/id_ed25519 user@{public-ip}
./run.sh nogui
