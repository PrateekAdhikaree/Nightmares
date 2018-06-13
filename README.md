# NIGHTMARES

**A Survival Shooter Game**

Inspired by the Unity 3D tutorial at https://unity3d.com/learn/tutorials/s/survival-shooter-tutorial

## Specifications:

- Developed on Unity Game Engine
- C# scripting

### Supported Platforms

- MacOS
- Windows PC

### Summary

An infinity survival shooter game with incremental waves

## Installation:

**Download the project and follow the steps (based on your OS):**

### Windows

Navigate to _/Builds/Windows_ and run the _.exe_ file to install

### MacOS

Navigate to _/Builds/MacOS_ and run the _.app_ file to install

## Game Menu:

![game_menu](https://github.com/PrateekAdhikaree/Nightmares/blob/master/images/game_menu.jpg "Pause/Game Menu")

- A game menu with options for controlling brightness, music and effect volumes
- There us data persistence of the volumes so the settings do not have to be set repeatedly
- There are 2 buttons to Start/Resume and Exit
- Can be reached by pressing 'Esc' key to Pause the game

## Player (Character):

![player_character](https://github.com/PrateekAdhikaree/Nightmares/blob/master/images/player.jpg "Player character")

- Animated child with running, idle and dying animations
- Has a gun which fires only 1 bullet at a time ...... (initially)
- Moves with movement keys and fires on _Click_ or pressing _Left Ctrl_ key. Pointer to be used for setting firing direction
- Starts with 100 health

## Enemies:

![enemies](https://github.com/PrateekAdhikaree/Nightmares/blob/master/images/enemies.jpg "Enemies")

- **Zombear:** Can attack when in proximity and runs to player **if** in its line of sight
- **Zombunny:** Functionally similar to _Zombear_, but looks different
- **Hellephant:** Can fly around, attacking with bullets
- Each type has different kill points
- Each has their own set of animations (for idle, moving, dying actions)
- They burn up in flames when killed and drop pickup items randomly

## Waves:

![waves](https://github.com/PrateekAdhikaree/Nightmares/blob/master/images/waves.jpg "Waves")

- Fixed set of enemies per wave
- Next wave triggered if all enemies killed or none killed in the last 20 seconds
- After wave 10, the difficulty increases automatically
- With each wave - the number of enemies, enemy health, score points increases

## Pickups:

![pickups1](https://github.com/PrateekAdhikaree/Nightmares/blob/master/images/pickups1.jpg "Pickups Example 1")

![pickups2](https://github.com/PrateekAdhikaree/Nightmares/blob/master/images/pickups2.jpg "Pickups Example 2")

- Dropped on random when enemies killed
- **Health:** Adds 25 points to health
- **Bounce:** Bullets become green and bounce 4 times when they hit other game objects (30 secs)
- **Pierce:** A single bullet can pierce through enemies but not game objects (20 secs)
- **Extra bullet:** Adds an extra bullet to be fired

## Gameplay:

![gameplay](https://github.com/PrateekAdhikaree/Nightmares/blob/master/images/gameplay.jpg "Gameplay")

- Mini map toggle by pressing 'M' key for seeing a wider area of the environment
- Timers showing the time remaining for bounce/pierce pickups to expire
- Health bar in red when health under 30%
- Counters showing current wave, score and enemies alive

## Some more screenshots:

![screen1](https://github.com/PrateekAdhikaree/Nightmares/blob/master/images/screen1.jpg "Screenshot 1")

![screen2](https://github.com/PrateekAdhikaree/Nightmares/blob/master/images/screen2.jpg "Screenshot 2")

![screen3](https://github.com/PrateekAdhikaree/Nightmares/blob/master/images/screen3.jpg "Screenshot 3")

## GAME OVER

![game_over](https://github.com/PrateekAdhikaree/Nightmares/blob/master/images/game_over.jpg "GAME OVER")
