# Cosmic Crucible



Made using the Unity Game Engine, Cosmic Crucible is a 2D physics based brawler with support for joystick contrtollers and local multiplyer with multiple gamemodes.

[Play online](https://cosmic-crucible.netlify.app/)

[Download](https://reedoover.itch.io/cosmic-crucible)



## Table of Contents
- [Features](#features)
- [Local Multiplayer](#local-multiplayer)
- [Keyboard/Controller Input](#keyboardcontroller-input)
- [Character Creator](#character-creator)
- [CPU Enemies](#cpu-enemies)
    - [Entity Prioritization](#entity-prioritization)
    - [Behavior](#behavior)
- [Physics](#physics)
    - [Simulating Space](#simulating-space)
    - [Collisions](#collisions)
- [Scaling Camera](#scaling-camera)
- [Graphics](#graphics)
    - [Stars](#stars)
    - [Gameplay Feedback](#gameplay-feedback)
- [Game Modes](#game-modes)
- [Survival](#survival)
- [Sandbox](#sandbox)
- [Motivation](#motivation)



## Features

### Local Multiplayer
This project supports local multiplayer. Simply connect controller(s) to your computer and click the "add player" button in the menu to create profiles for each player.
![Alt text](https://raw.githubusercontent.com/reedbryan/CosmicCrucible/main/Assets/Sprites/UI/ReadmeScreenShots/PlayerList.png)

#### Keyboard/Controller input
In the character creator (see [Character Creator](#character-creator) below) you can select either "keyboard" or "controller" input, with the default being keyboard. Note that only one player can use keyboard input.

Unity has customizable [input configurations](https://docs.unity3d.com/6000.0/Documentation/Manual/ios-handle-game-controller-input.html) for controller (they call it joystick) input. Inputs can be configured in project settings under "input manager" (edit->project settings->input manager). Unity provides customization for both Joystick buttons and Axis for multiple controllers for attributes such as:
- Sensitivity
- Snap
- Inversion
- Alternate keys/buttons
- Axis Gravity Scale
- Dead Time

Using the input manager I configured inputs for the keyboard as well as 3 controllers. When a player profile is created in the menu they are assigned a controllerNumber (0=keyboard, 1-3=controller) and are mapped to that players ID for in-game input handling. See `createPlayerProfile` function in [GameManager.cs](https://github.com/reedbryan/CosmicCrucible/blob/main/Assets/Scripts/Main/GameManager.cs) for ID mapping and `ControllerInputs`/`KeyBoardInputs` functions in [Inputs.cs](https://github.com/reedbryan/CosmicCrucible/blob/main/Assets/Scripts/Player/Inputs.cs) for in-game input handling.

### Character Creator
Cosmic Crucible has a built in "ship" creator where select attributes of the player's ship can be altered for unique and variable gameplay. To balance te ship builder, the player only has a certain amount of "cash" to spend alterations. The more an attribute is increased the more cash it will cost. Likewise, the more attribute is deacreased the more cash it will give back. This way the player can still create unique character-builds but is forced to lose certain stats in order to gain others.
|  Before | After   |
|---------|---------|
| ![Alt text](https://raw.githubusercontent.com/reedbryan/CosmicCrucible/main/Assets/Sprites/UI/ReadmeScreenShots/PlayerBuilder2.png) | ![Alt text](https://raw.githubusercontent.com/reedbryan/CosmicCrucible/main/Assets/Sprites/UI/ReadmeScreenShots/PlayerBuilder1.png) |


### CPU Enemies
In the "survival" gamemode (see [Game Modes](#game-modes)), the player(s) will fight against CPU enemies that spawn throughout the environment. These CPUs act independently and will attack the player as well as each other.

#### Entity Prioritization
Each CPU keeps track of all entities within a 200(unity meters) radius. 
Valid entities include:
- Other players (CPU or Non-CPU)
- Powerups & Health Packs
- Projectiles: Bullets & Missiles

Each entity is given a "desirability" rating calculated every frame, based on qualities such as:
- Proximity
- Health (For other players)
- Possible gain (For health packs & powerups)
- Possible loss (For projectiles)

See the `getDesirability` function in [CPU_Logic.cs](https://github.com/reedbryan/CosmicCrucible/blob/main/Assets/Scripts/CPU/CPU_Logic.cs) for full desirability calculations.

The CPU will select the entity with the largest desirability magnitude (can be positive or negative) as its "target", identifying the target as either **static** or **moving** and act occordingly (see [Behavior](#behavior) below).

#### Behavior
If the CPU's target entity is **static** it will simply point itself towards the target, applying thrust to move in that direction (see `targetStatic` function in [CPU_Logic.cs](https://github.com/reedbryan/CosmicCrucible/blob/main/Assets/Scripts/CPU/CPU_Logic.cs)). 

If the target is **moving** there are different considerations. If the target has negative desirebility it will point try to avoid colliding with that entity or get out of the line of fire. If the target has positive desirebility, which in this case means it is a player that has been evaluated as attackable. It will use the targets current velocity and the CPU's expected projectile path to calculate the point where they are most likely to meet, targeting this point instead of the target's actual posistion.
```c#
// Get projectile velocity in vector2 form
float radAngle = transform.eulerAngles.z * Mathf.Deg2Rad;
float x = Mathf.Sin(radAngle) * -1f;
float y = Mathf.Cos(radAngle);
Vector2 primFireV = (ID.primaryFireForce * 100 / ID.primaryFireMass / 50 * new Vector2(x,y)) + rb.velocity;

// Get the time it would take for the bullet to reach the other player and
// distance the other player will travel during that time
float timeForProj = General.timeToReach(transform.position, mostDesirable.transform.position, primFireV, ID.primaryFireDrag);
Vector2 newTargetPosition = General.distanceInTime(mostDesirable.transform.position, timeForProj, mostDrb.velocity, mostDrb.drag);
Vector2 targetPos = ((Vector2)transform.position - newTargetPosition).normalized;
```
See [CPU_Logic.cs](https://github.com/reedbryan/CosmicCrucible/blob/main/Assets/Scripts/Main/General.cs) for my time/space evaluation functions.

### Physics
Unity's 2D Physics Engine is highly customizable, allowing devs to fine-tune gravity, friction, and other properties for unique physics behaviors at will. Its flexible simulation control allows for real time customizability via scripts, making it ideal for dynamic gameplay mechanics and perfect for this project.

#### Simulating Space
Turning off the gravity scale and adjusting for minimal angular and linear drag, allows for gameobjects to drift through the environment like they were in outer space (SO COOL).

The ships operated by the player and CPUs move through the evironment by applying a constant force to the back of the object. Allowing players to have a very high maximum velocity, with the consiquence of needing to apply equal force in the opposite direction to slow down again. This makes for choatic and in my opion, very fun movement mechanics (see [PlayerMovement.cs](https://github.com/reedbryan/CosmicCrucible/blob/main/Assets/Scripts/Player/PlayerMovement.cs)).

#### Collisions
Unity's Physics Engine handle collisions on its own but not quite in the way I wanted for the project. Unity's basic 2D collisions do not allow for two objects (with collider components) to pass bewteen each other. This is good but when two players collide at high speeds it lead to varying and unpredictable results. Sometimes they would slide passed each other, maintaining velocity other times they would both come to an abrupt stop. I wanted a uniform reaction to player collision that also made for interesting gameplay. 

To do this I wrote a script attached to each player to handle collisions (see [PlayerCollisions.cs](https://github.com/reedbryan/CosmicCrucible/blob/main/Assets/Scripts/Player/PlayerCollisions.cs)). The script distributes the difference in velocities upon collison back to the players, creating a sort of bouncing effect. On collision it also calculates an "impact" value based on the pre-collison velocity of the player. Each player's collision script will assign damage to the other player based on the impact value, rewarding the player who came into the collision with higher velocity. It will also create a particle explosion (scaled to the impact size) that inherites the player's velocity to show the magnitude of the collision.
```c#
if (hit.CompareTag("Player")) // If this projectile makes contact with another projectile
{            
    // Get other player components
    DamageIntake other_damageIntake = hit.GetComponent<DamageIntake>();
    Transform other_transform = hit.GetComponent<Transform>();
    PlayerCollisions other_playerCollisions = hit.GetComponent<PlayerCollisions>();

    // Disable movement
    ID.movement_enabled = false;
    
    // Calculate impact value using stored velocity
    int impact = (int)rb.velocity.magnitude;
    //Debug.Log("Impact (before collision): " + impact);

    // Create explosion (based on impact value)
    createExplosion(impact);

    // Update velocity based on angle of collision
    Vector2 angle_of_collision = (Vector2)(transform.position - other_transform.position).normalized;
    rb.velocity = other_playerCollisions.rb.velocity + rb.velocity * angle_of_collision * 0.5f; // 0.5f for dampening

    // Deal damage
    other_damageIntake.alterHP(-1.5f * impact, other_transform.position);
}
```

Below is a frame-by-frame display of a collision between a player (red) and a CPU (black). The player has more initial velocity, (indicated by a longer jet stream) and thus apon collision, takes less damage and maintains more of its velocity.

| Frame 1 | Frame 2 | Frame 3 |
|---------|---------|---------|
| ![Alt text](https://raw.githubusercontent.com/reedbryan/CosmicCrucible/main/Assets/Sprites/UI/ReadmeScreenShots/collisionSC1.png) | ![Alt text](https://raw.githubusercontent.com/reedbryan/CosmicCrucible/main/Assets/Sprites/UI/ReadmeScreenShots/collisionSC2.png) | ![Alt text](https://raw.githubusercontent.com/reedbryan/CosmicCrucible/main/Assets/Sprites/UI/ReadmeScreenShots/collisionSC3.png) |

| Frame 4 | Frame 5 | Frame 6 |
|---------|---------|---------|
| ![Alt text](https://raw.githubusercontent.com/reedbryan/CosmicCrucible/main/Assets/Sprites/UI/ReadmeScreenShots/collisionSC4.png) | ![Alt text](https://raw.githubusercontent.com/reedbryan/CosmicCrucible/main/Assets/Sprites/UI/ReadmeScreenShots/collisionSC5.png) | ![Alt text](https://raw.githubusercontent.com/reedbryan/CosmicCrucible/main/Assets/Sprites/UI/ReadmeScreenShots/collisionSC6.png) |

### Scaling Camera
The game's camera is something that took a lot of thought and editing to get in a state I was OK with. Due to the game being local multiplayer I needed all players to be on screen at once, which in a large envirnonment can lead to some problems I still have not fully addressed.

The current game camera posistions itself at the average between all player positions and scales its size to largest distance between two player positions (ignoring CPUs after it reaches 40m in diameter). This works just fine to get all players in view but can become problematic as players distance themselves further and further, making the camera view too large to see your character. I thought about splitting the screen when players got too far apart but desided agaist it, assuming players will stick togther. This is one area I plan on improving if I revisit this project.

Another problem I incountered was the camera jittering and glitching when adjusting it's position. To make camera adjustments seemless with gameplay, the camera gameobject increments itself towards the ideal position & size instead of simply being set to those values.
```c#
/* increment camera position */
// Current Values:
float current_orthSize = Camera.main.orthographicSize;
Vector2 current_pos = transform.position;

// Difference Between Current and Ideal Values:
float diff_orthSize = ideal_orthSize - current_orthSize;
Vector2 diff_pos = ideal_pos - current_pos;

// Increment orthagraphic camera size in real time
Camera.main.orthographicSize += diff_orthSize * Time.deltaTime;

// Increment position in real time
transform.position += new Vector3(diff_pos.x, diff_pos.y, 0) * Time.deltaTime;
```

See [GameCamera.cs](https://github.com/reedbryan/CosmicCrucible/blob/main/Assets/Scripts/Main/GameCamera.cs) for all the in game camera code.

### Graphics
#### Stars
The in-game environment has 3 layers of stars moving in the background. These stars move at different speeds reletive to the player to create the effect of moving through space. This is done by updating the position of each layer of stars by the players position multplied a value from 0 to 1 representing their depth:
```c#
transform.position = (Vector2)mainCamera.transform.position * depth;
```
A layer with a depth of 1 moves with the player, making it look like the stars are so far in the background that the player doesn't even move reletive to their posistion. A layer with a depth of 0.5 moves at half that speed to give the effect that the player is passing through the stars. In game the 3 star layers have depth values of 1, 0.8 & 0.4, which I found to have most pleasing effect. 

The stars were created using Unities [particle system](https://docs.unity3d.com/Manual/ParticleSystems.html) and are moved through the envirnoment from script (see [BackgroundLayer.cs](https://github.com/reedbryan/CosmicCrucible/blob/main/Assets/Scripts/Environment/BackgroundLayer.cs)).

#### Gameplay Feedback
When a player (or CPU) is hit by a projectile, collides with another player or collects a health pack, their health is altered and that alteration is displayed to the player via hit markers. These hit markers are UI objects, instantiated on a damage event and have different sizes depending on the amount of hitpoint gained/lost (see [DamageMarker.cs](https://github.com/reedbryan/CosmicCrucible/blob/main/Assets/Scripts/UI/InGame/DamageMarker.cs)).
|  Gain   | Loss   |
|---------|---------|
| ![Alt text](https://raw.githubusercontent.com/reedbryan/CosmicCrucible/main/Assets/Sprites/UI/ReadmeScreenShots/greenHM.png) | ![Alt text](https://raw.githubusercontent.com/reedbryan/CosmicCrucible/main/Assets/Sprites/UI/ReadmeScreenShots/redHM.png) |

Each player also has a "health bar" UI object attached to them representing the difference between their current HP and max HP.
See [HealthBar.cs](https://github.com/reedbryan/CosmicCrucible/blob/main/Assets/Scripts/UI/InGame/HealthBar.cs).

## Game Modes
### Survival
In survival mode, 1 or more players attempt to survive in the environment for as long as possble without being destroyed by enemies or by each other, the time spent alive is called runtime. In this game mode CPU enemies will spawn periodically throughout the environment with the spawn rate increasing as the player(s) runtime increases. There is a runtime counter at the top of the screen to indicate how long they have survived for. The goal of the game is simply to survive as long as possible.

![Alt text](https://raw.githubusercontent.com/reedbryan/CosmicCrucible/main/Assets/Sprites/UI/SurvivalSC2.png)

### Sandbox
In sandbox mode, players are free to do as they please, without CPU interruption. Test out your ship's mobility and battle your friends in an open environment. Should you wish to fight CPUs, or watch CPUs fight each other, simply press 'n' on your keyboard and one will be spawned in near the player's location.

![Alt text](https://raw.githubusercontent.com/reedbryan/CosmicCrucible/main/Assets/Sprites/UI/SandboxSC.png)


## Motivation
My motivation for the project stemed from wanting to create a game I could play with my younger brothers. We would play all sorts of games together when we were younger and as a young indie game dev I felt obligated to create a game we could play together.