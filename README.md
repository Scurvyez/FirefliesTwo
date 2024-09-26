Discord: steveo.o

steam: Scurvyez

A simple, yet... heart-warming revamp of my original mod which adds fireflies to the Rim (as pure visual effects).
Will I be adding more visual effects such as butterflies, embers, falling leaves in autumn, etc.? No.
This mod is about fireflies only. I might make a separate mod for more general ambient effects in the future though.

### Supported Biomes

+ [Core] BorealForest, TemperateForest, TemperateSwamp, TropicalRainforest, TropicalSwamp, AridShrubland, Desert, and ExtremeDesert
+ [More Vanilla Biomes] ZBiome_AlpineMeadow, ZBiome_CloudForest, ZBiome_CoastalDunes, ZBiome_DesertOasis, ZBiome_Grasslands, and ZBiome_Marsh
+ [Alien Biomes (WIP)] SZ_RadiantPlains, SZ_DeliriousDunes, and SZ_CrystallineFlats
+ [Alpha Biomes] AB_FeraliskInfestedJungle and AB_IdyllicMeadows

### Features
+ Fireflies spawn if...
    - the weather is clear
    - ambient light from sun is less than 30% (so from around dusk til dawn)
    - additional log message pops if you chose a biome not supported
+ Jobs
    - pawns can periodically chase fireflies for fun, 75% base chance
    - will count towards joy/recreation
+ Thoughts
    - good (short-term) memory for being outside while fireflies are active
    - good (short-term) memory for catching a firefly while chasing them, 25% base chance


### Mod Authors
+ "FirefliesTwoO.FFConfigDef"
    - patch your custom BiomeDef into the "allowedBiomes" field
+ "FirefliesTwoO.NightlySpawningExtension"
    - add to your custom BiomeDef via a PatchOperationFindMod + PatchOperationAddModExtension
    - specify a value for the "biomeEmissionRate" field, 1 = base/default value
    - less than 1 means a slightly lower final emission rate; greater than 1, the opposite
