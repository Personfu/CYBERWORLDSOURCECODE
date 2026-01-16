## Requirements to Open the Project

> [!IMPORTANT]
> ### Unity Version
> - **Unity 6000.3.4f1 LTS**

> [!IMPORTANT]
> ## **Builds can be found here:**
> - [Builds](https://opencpisland.github.io/)

> [!CAUTION]
> ## **How do I clone the repo?**
> - I do not recommend using GitHub's built in ZIP download feature. Because it tends to forget to download files which breaks the project. I only recommend using the older HTTPS git clone or the new Git CLI gh repo clone options to download the repo on Linux. If you are on Windows or macOS, I recommend using git clone or using the official GitHub Desktop app. 

> HTTPS method:
> ```git clone https://github.com/OpenCPIsland/CPI-Project.git```

> Git CLI method:
> ```gh repo clone OpenCPIsland/CPI-Project```

> [The Github Desktop app method](https://github.com/apps/desktop)

> [!IMPORTANT]
> ## **Commonly asked questions:**

> Q: Why is everything pink or not loading when I load the game?

> A: You need to run the Unity Editor menu item:```Project -> AssetBundles -> Generated -> Generate client-side AssetBundles```

> - For other commonly asked questions, you can find those in the ```#faq``` channel of our [Discord server](https://discord.gg/2V6tYJPbpc).

> [!IMPORTANT]
> ### Notes
> - **The Penguin data gets stored here in the Windows Registry:**  
>  - **Built .exe client:** `HKEY_CURRENT_USER\SOFTWARE\OpenCPI\CP Island`  
>  - **Unity Editor:** `HKEY_CURRENT_USER\SOFTWARE\Unity\UnityEditor\OpenCPI\CP Island`
> - **The Penguin data gets stored here on Linux for the Editor and built client:**
>  - `/home/your_username/.config/unity3d/OpenCPI/CP Island/prefs`
> - To launch the game in the Unity editor:
>   - Open `Assets/Game/Core/Scenes/Boot.unity`
>  - Hit the Play button.
> - **Android/Mobile support:** Please make sure to read everything carefully in the **Platforms** folder before proceeding. **Please note: The mobile assets are untested and unmaintained.**
> - **Join our Discord for support, chatting, or for future updates:** [join here](https://discord.gg/2V6tYJPbpc)

> [!IMPORTANT]
> ### New and fixed features within the game

> [!IMPORTANT]
> - What's new/changed:
>     - Added the unreleased igloo furniture ```grand father clock``` from version 1.6.1
>     - Added 5 custom party hat recolors (Halloween Party Hat, Holiday Party Hat, Anniversary 19 Party Hat, Anniversary 20 Party Hat, and Anniversary 21 Party Hat)
>     - Added 4 custom duck tube recolors (Blue, Green, Pink, and Purple)
>     - Added 1 custom recolor of the ```CakeCruiser``` Tube, the colors matches the 20th Anniversary cake and party hat.
>     - Refined the lightmap baking process
>     - The Classic Arcade machine has been moved from near Franky's Pizza in Island Central to the sewer in Island Central
>     - Added and optimized support for native macOS Arm Silicon (Apple M1, M2, M3, M4, and newer chips)
>     - Changed the Waddle On login coins award from ```1000000``` to ```0```
>     - Added 2 new lighting options to the igloos. Those are ```Holiday``` and ```Rainbow Migration```. The ```Holiday``` lighting can be unlocked at Penguin level 20 and the ```Rainbow Migration``` lighting can be unlocked at Penguin level 8.
>     - Added an optional skybox in the project to allow a day/night cycle that will cycle every 15 minutes (unfinished)
>     - Added 40 new Penguin colors
>     - Unlock the ```Valentine's Day chair``` at level 8. The ID for the chair is 278 and it will sell at the Igloo furniture shop for 40 coins
>     - And most importantly, the game is no longer in the original 32-bit state! This recreation is in a 64-bit state
>     - Version 1.13.1
>     - Changed the default 3 igloo save slots to 10 (10 is the max, higher than 10 causes data corruption and errors)
>     - Changed the default 130 max igloo furniture limit to 750
>     - Support for DirectX 12 and Vulkan
>     - Support for iL2CPP
>     - Added an annual looping event controller 3000
>     - Added a total of 58 new Igloo Music tracks. Now you have more different types of music to play in your igloo! 
>     - Added the April Fools Theme and Box Dimension to level 0 (before the tutorial level up). Added The Town (2014) to level 1. 
>     - Added Puffle Party, Come Out To Play, Shoot For The Sun, Coconut, The Best Beach Party, Backbeat Jammin, Forever Summer, Rockhopper Theme, Sunshine Holiday, and Summer Song to level 2. 
>     - Added Lucky One and Gotta Have A Wingman to level 3. 
>     - Added the Medieval Theme and The Royal Court to level 7. 
>     - Added Glam Jam and Rock The Boat Quartet Remix to level 8. 
>     - Added the Alien Lounge to level 9. 
>     - Added Dancing In The Sun, Rave Cave, Rock The Boat, Go Time, Go Time Remix, and Party In My Iggy to level 10.
>     - Added Checker Chuck, Downhill Hoedown, and Sunday Skool to Level 11. 
>     - Added Anchovy Jazz, Beat Them Keys, Maybe Baby, Puffle Dance Jazz Mix, and Cash Or Check to Level 12.
>     - Added Surf Monster, Haunted Disco, Discoween, Ghost just want to dance, Monster masquerade, Nightmare before Christmas's This is Halloween, Puffle Dance Rock Mix, Spooky Jazz, and What lurks in the night to level 14.
>     - Added Crossing Over to Level 16. 
>     - Added Steer The Funk and Dub Style Step to Level 17. 
>     - Added Holiday Lights, Tis the season, Snowy Holiday, Command Room, We Are The Penguins, and Catching snowflakes to level 20. 
>     - Added Sunny Side to Level 23. 
>     - Added Jazzy Pizza, Coffee Shop, and Pizza Parlor to level 24. 
>     - Added I've Been Delayed, Cumulonimbus, and the Ski Lodge to level 25.
>     - Support for .NET Standard 2.1
>     - Support for Unity's New Input System
>     - Support for Unity WebGL
>     - Added 2 new props, the 20th anniversary cake single and the 20th anniversary cake group. These can only be obtained from October 24, 2025 until October 31, 2025. November 1st and onward will not be claimable. 
>     - Support for the unreleased Penguin sprinting and skidding locomotion
>     - Added 1 new igloo furniture to level 14. It is a recolor of the level 13 ```Waterfall```, named the ```Slime Waterfall```.
>     - Added the Dubstep, Pop, and Rock genres for igloo music.
>     - Added Rainbow Migration and Holiday Party items to the Disney Shop.
>     - Added the ```RDMA 2017 Award``` to the Igloos. That unlocks at level 3.
>     - Added the ```Globe Bean Bag Chair``` from ```WorldPenguinDay2017``` to the Igloos. That unlocks at level 3.
>     - Added the ```Blizzard Beach Palm Tree``` and the ```Blizzard Beach Beach Chair``` from ```BlizzardBeach2017``` to the igloos. That unlocks at level 3.
>     - Added the ```Rainbow Migration``` ```Blender```, ```fruits```, ```Rainbow Smoothie```, and ```Color Post``` to the Igloos. Those unlocks at level 0, except for the ```Color Post``` which unlocks at level 0.
>	  - Added the unused PartySupplies ```Mint GlowStick Single``` to the igloo shop and the diving market which unlocks at Penguin Level 10.
> 	  - Brought back the older version of the effects particles for the ```Science Beaker``` Prop.
>     - Added a new igloo furniture item to the Igloos that is called the ```Chemistry Set``` that can be unlocked at Penguin Level 25 and can be bought at the ```Igloo Interiors``` shop for 75 coins.
>     - Added the ```Picnic Basket``` and ```Picnic Table``` from the Boardwalk to the igloos which can be unlocked at level 0.
>     - Added the ```Wish Squid``` from the Boardwalk to the igloos which can be unlocked at level 1.
>     - Added a whole bunch of Food Truck related igloo furniture to the igloos which can be unlocked at level 0.
>     - Added the Arcade Machine from the Town to thhe igloos which can be unlocked at level 10.
>     - Added the ```Rockhopper Picture Frame``` to the igloos which can be unlocked by completing the Chapter 1 Episode 1 of Rockhopper. This is a Quest Reward and can't be purchased from the Igloo & Interiors shop.
>     - Added a green variant of the Chemistry Beaker to level 25.
>     - Added a DJ Booth, DJ Pillar, and Purple Stage Curtain to the igloos. These furnitures will be unlocked at level 20.
>     - Added a new collectible named the ```Sea Crystals``` to the Sea Caves.

> [!IMPORTANT]
> - What has been fixed:
>     - The spawn points have been moved so you will no longer spawn into the void and endlessly fall randomly like in the original
>     - The ```shoulder pack``` blueprint will correctly work now
>     - The ```modern coffee table``` and ```kitchen island ``` igloo furnitures can now be obtained instead of having it give you the ```teleporter``` igloo furniture
>     - Fixed missing scripts from the Mt. Blizzard Halloween 2018 decorations (this was causing errors in the original client)
>     - Fixed the Halloween 2018 Pumpkins flicker speed to match how they wanted it in the original (the editor and built client shows 2 different results. So the built client would make it too fast)
>     - Added missing colliders to certain world and quest objects
>     - Performance improvements
>     - Added the missing Summer Splashdown chat phases and Rookie sound effects to the Regular sewer in Island Central
>     - Fixed original errors within the ```Unlit Dynamic Object No FOG```, ```World Object```, and Igloo ```CubeMap``` shaders
>     - Fixed the Disney Store banners and for sale items, they originally stopped working on: ```January 1, 2020```. Now they will stop working on: ```December 31, 4065```
>     - Fixed the coins and collectibles that would spawn once a day (this broke when the servers went offline). They will now spawn once every 24 hours
>     - Fixed the microphone and guitar interactables collision in Island Central. The collisions were swapped in the original
>     - Fixed the collider on the ```boss computer chair``` igloo furniture
>     - Fixed the ```boss computer chair``` from spawning a little bit into the ground
>     - Fixed the ```cushion``` and ```stool``` igloo furniture being labeled as a "tube" when it should be labeled as a "ManipulatableObject"
>     - Fixed the ```diamond flower pot``` igloo item collision
>     - Fixed the ```modern coffee table``` and the ```kitchen island``` igloo furniture so that you can properly place items on top of them
>     - Fixed the ```Indoor wall light``` igloo item so that it can be placed properly on the walls of your igloo
>     - Fixed the collision on the ```CrystalCave``` igloo building
>     - Fixed the cosmic daily spin chest reward. It can now be obtainable
>     - Fixed the fishing squid reward (it was appearing far up out of the camera view (original issue))
>     - Fixed the Igloo Music Track ```Too Yule For Skool``` from not playing anything (original issue)
>     - Fixed a pumpkin in the Boardwalk of Halloween 2018 using the wrong material at the ```Sky Cafe``` (original issue), the original issue was making the pumpkin shell light up as well as the inner glow
>     - Some music had their genres switched to fit the music better.
>     - Fixed classification of indoor light fixture to be considered a wall item.
>     - Fixed the SunSet Arcade collision. It wasn't using the ```Terrain Barrier``` layer. You would be able to move through it if you were on your tube originally.
>     - Fixed the HalloweenParty2018 ```SpookyWindow``` Igloo furniture using the wrong material for the frame (original issue).
>     - Fixed the ClassicMiniGame ```Smoothie Smash``` order of fruit animation not containing the fruit which makes the fruit not appear (original issue).
>     - Fixed the ```Snowy Pine Tree``` igloo furniture using the decoration category rather than Landscaping.
>     - Fixed the original bug where the first trampoline on the Platforming wall in the Mt. Blizzard would give the wrong bounce direction.

> [!IMPORTANT]  
> ## System Requirements

> [!IMPORTANT]
> ### Windows
> - Latest version of Visual Studio Community (Starting with Visual Studio 2026).
> - [Git for Windows](https://gitforwindows.org/) installed. **Restart your PC after installing Git.**

> [!IMPORTANT]
> ### macOS
> - Latest version of Xcode for your version of macOS.

> [!IMPORTANT]
> ### Linux
> - Make sure your system is updated:
>   ```bash
>   sudo apt update && sudo apt upgrade
>   ```
> - Install Git:
>   ```bash
>   sudo apt install git gh
>   ```
>  - Make sure to install this component for X11 Window manager distros:
>    ```bash
>    sudo apt-get install libx11-dev
>    ```

> [!IMPORTANT]
> For further documentation, refer to the [OpenCPI Docs](Offline-Project-Instructions). If something is missing, feel free to create a fork and send a pull request.

> [!IMPORTANT]
> ## Special thanks to the following people who have made this restoration possible:

> [Galaxyrelic](https://github.com/Galaxyrelic)

> [ChavalSaturado](https://github.com/ChavalSaturado?tab=repositories)

> [PickleOnAString](https://github.com/PickleOnAString)

> [broimluna](https://github.com/broimluna)

> [shinonasada9](https://github.com/shinonasada9)

> miraculizado (Discord)

> approt (Discord)

> [wednesday2024](https://github.com/wednesday2024)

> [AllinolCP](https://github.com/AllinolCP)

> [Minileandro](https://github.com/Minileandro)
