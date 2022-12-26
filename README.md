# MillaCubeMod 1.2

A mod for the game [Freedom Planet 2](https://freedomplanet2.com/) which allows you to increase (or lower) the range of Milla's projectile and cube attack as well as increasing (or limiting) the number of cubes that are spawned.  
By default the range is set to effectively infinite and we spawn 10 cubes. Setting values higher than 50 for the range might have **unintended effects**. Setting the cubes number over 30 will fill out the cube entity limit and **break the game**.  

For the range the values, the higher the number, the further cube will fly.  
For the cubes the number **must be** positive.  

The config file is generated on the first run in BepInEx/config/.  

## Prerequisites:
The mod requires [BepinEx 5](https://github.com/BepInEx/BepInEx) to function. You can download it here:
* [Direct Download](https://github.com/BepInEx/BepInEx/releases/download/v5.4.21/BepInEx_x86_5.4.21.0.zip)  

Extract the downloaded zip file in to the main game directory.  

## Installation:
To install the mod extract the downloaded zip file contents from the releases tab into the main game directory.  
If asked, agree to merge the BepInEx folders.  

## Building:
Follow the BepinEx guide for setting up Visual Studio found [here](https://docs.bepinex.dev/master/index.html).  
Open the solution in Visual Studio and build the project.
