# Endless Hourglass

Endless Hourglass is a 2D game, developed with C#.

Gameplay is focused on avoiding enemies and projectiles, while also dishing out your own projectiles to eliminate threats.
As the name suggests, the game is "Endless" and the challenge is to beat your own score, or the score of your friend, while the difficulty gradually increases.

## Features

* Two types of player weapons: Although functionally similar, they have their own animated projectiles.

* Two types of enemies and enemy attacks: one melee type, and one ranged type which fires projectiles.

* Enemies actively track and aim towards the player.

* Health bars for each enemy, as well as the player.

* Stages are tied to time and are visually indicated with roman numerals. Stage affects spawn rate and spawn selection.

* All visual assets are designed from scratch for this project, except a generated Tier font which is rarely seen.

## Programming

The gameplay logic is mostly object-oriented.
Patterns like Dependency Injection (via manual constructor injection), 
and Interface Segregation (small, focused contracts) are used to promote flexibility.

Some of the central files are the following:

[EnemyManager.cs](/EndlessHourglass/Gameplay/Enemy/EnemyManager.cs)  
[ActivePlayer.cs](/EndlessHourglass/Gameplay/Player/ActivePlayer.cs)  
[InputManager.cs](/EndlessHourglass/Gameplay/Player/InputManager.cs)  
[ProjectileManager.cs](/EndlessHourglass/Gameplay/Projectile/ProjectileManager.cs)  
[Geometry.cs](/EndlessHourglass/Gameplay/Static/Geometry.cs)  
[TextureStore.cs](/EndlessHourglass/Gameplay/Static/TextureStore.cs)  
[EndlessHourglass.cs](/EndlessHourglass/EndlessHourglass.cs)  

## Simple controls

* Movement input is traditional, and is tied to W/A/S/D or arrow keys.
* To change weapon, move to your desired weapon and press E.
* Aim with your cursor and fire by clicking or holding LMB, left mouse button.
* Exit the game with ESC, escape.
* On game over, restart with Space or Enter.

## Getting the game

You can download the game here:

[Release v.0.1.0](https://github.com/Radors/EndlessHourglass/releases/tag/v0.1.0)

You can also build the game yourself:

1. Clone the repository: *git clone https://github.com/Radors/EndlessHourglass*
2. Go to the project directory, which carries the same name as the solution: /EndlessHourglass/EndlessHourglass/
3. Build:
* Windows: *dotnet publish -c Release -r win-x64 -p:PublishReadyToRun=false -p:TieredCompilation=false --self-contained*
* Linux: *dotnet publish -c Release -r linux-x64 -p:PublishReadyToRun=false -p:TieredCompilation=false --self-contained*
4. Find the output:
* Windows: \EndlessHourglass\EndlessHourglass\bin\Release\net8.0\win-x64\publish
* Linux: /EndlessHourglass/EndlessHourglass/bin/Release/net8.0/linux-x64/publish
5. Run the game:
* Windows: EndlessHourglass.exe
* Linux: EndlessHourglass (no extension)

Building requires that your machine has the .NET 8 SDK installed:

[.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
