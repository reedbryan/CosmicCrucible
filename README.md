# Cosmic Crucible

<hr/>

Made using the Unity Game Engine, Cosmic Crucible is a 2D physics based brawler with support for joystick contrtoller and local multiplyer with multiple gamemodes.

[Play online](https://simmer.io/@reedoover/cosmic-crucible)

[Download](https://reedoover.itch.io/cosmic-crucible)

## Features

### Character Builder
Cosmic Crucible has a built in "ship" creator where select attributes of the player's ship can be altered for unique variable gameplay. To balance te ship builder, the player only has a certain amount of "cash" to spend alterations. The more an attribute is increased the more cash it will cost. Likewise, the more attribute is deacreased the more cash it will give back. This way the player can still create unique ships but is forced to lose certain stats in order to gain others.
|  Before | After   |
|---------|---------|
| ![Alt text](https://raw.githubusercontent.com/reedbryan/CosmicCrucible/main/Assets/Sprites/UI/ReadmeScreenShots/PlayerBuilder2.png) | ![Alt text](https://raw.githubusercontent.com/reedbryan/CosmicCrucible/main/Assets/Sprites/UI/ReadmeScreenShots/PlayerBuilder1.png) |

All the stats from each ship are showcased in the "player list" before the players enter the game.
![Alt text](https://raw.githubusercontent.com/reedbryan/CosmicCrucible/main/Assets/Sprites/UI/ReadmeScreenShots/PlayerList.png)

### CPU Enemies
In the "survival" gamemode the player(s) will fight against CPU enemies that spawn throughout the environment. These CPUs act indopendently and will attack each other as soon as the player.

#### Entity Prioritatization
Each CPU keeps track of all entities within a 200(unity meters) radius. 
Valid entities include:
- Other players (CPU or Non-CPU)
- Powerups & Health Packs
- Projectiles: Bullets & Missiles

Each entity is given a "desirability" rating calculated every frame an based on qualities such as:
- Proximity
- Health (For other players)
- Possible gain (For health packs & powerups)
- Possible loss (For projectiles)

See the `getDesirability(GameObject subject)` function in [CPU_Logic.cs](https://github.com/reedbryan/CosmicCrucible/blob/main/Assets/Scripts/CPU/CPU_Logic.cs) for full desirability calculations.

The CPU will select the entity with the largest desirability magnitude (positive or negative) as its "target", identifying the target as either **moving** or **moving** and will approch the target occordingly.

#### Behavior
If the CPU's target entity is **moving** it will simply point itself towards the target, applying thrust to move in that direction (see `targetStatic(GameObject mostDesirable)` in [CPU_Logic.cs](https://github.com/reedbryan/CosmicCrucible/blob/main/Assets/Scripts/CPU/CPU_Logic.cs)). 

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

### Custom Physics
Unity's 2D Physics Engine is highly customizable, allowing devs to fine-tune gravity, friction, and other properties for unique physics behaviors at will. Its flexible simulation control allows for real time customizability via scripts, making it ideal for dynamic gameplay mechanics and perfect for this project.

#### Simulating Space
Turning off the gravity scale and adjusting for minimal angular and linear drag, allows for gameobjects to drift through the environment like they were in outer space (SO COOL).

The ships operated by the player and CPUs move through the evironment by applying a constant force to the back of the object. Allowing players to have a very high maximum velocity but with the consiquence of needing to apply equal force in the opposite direction to slow down again. This makes for choatic and in my opion very fun movement mechanics. 

#### Collisions
Unity's Physics Engine handle collisions on its own but not quite in the way I wanted for the project. Unity's basic 2D collisions do not allow for two objects (with collider components) to pass bewteen each other. This is good but when two players collide at high speeds it lead to varying andd unpredictable results. Sometimes they would slide passed each other, maintaining velocity other times they would both come to an abrupt stop. I wanted a uniform reaction to player collision that also made for interesting gameplay. 

To do this I wrote a script attached to each player to handle collisions (see [CPU_Logic.cs](https://github.com/reedbryan/CosmicCrucible/blob/main/Assets/Scripts/Player/PlayerGraphics.cs)). The script distrubutes the difference in velocities upon collison back to the players, creating a sort of *bouncing* effect. On collision it also calculates an "impact" value based on the pre-collison velocity of the player. Each player's collision script will assign damage to the other player based on the impact value, rewarding the player who came into the collision with higher velocity. It will also create a particle explosion (scaled to the impact size) that inherites the player's velocity to show the magnitude of the collision.

| Image 1 | Image 2 | Image 3 | Image 4 | Image 5 | Image 6 |
|---------|---------|---------|---------|---------|---------|
| ![Alt text](https://raw.githubusercontent.com/reedbryan/CosmicCrucible/main/Assets/Sprites/UI/ReadmeScreenShots/collisionSC1.png) | ![Alt text](https://raw.githubusercontent.com/reedbryan/CosmicCrucible/main/Assets/Sprites/UI/ReadmeScreenShots/collisionSC2.png) | ![Alt text](https://raw.githubusercontent.com/reedbryan/CosmicCrucible/main/Assets/Sprites/UI/ReadmeScreenShots/collisionSC3.png) | ![Alt text](https://raw.githubusercontent.com/reedbryan/CosmicCrucible/main/Assets/Sprites/UI/ReadmeScreenShots/collisionSC4.png) | ![Alt text](https://raw.githubusercontent.com/reedbryan/CosmicCrucible/main/Assets/Sprites/UI/ReadmeScreenShots/collisionSC5.png) | ![Alt text](https://raw.githubusercontent.com/reedbryan/CosmicCrucible/main/Assets/Sprites/UI/ReadmeScreenShots/collisionSC6.png) |


### Scaling Camera

### Graphics
#### Environment
#### Particle Explosions & Debris

## Motivation
My motivation for the project stemed from wanting to create a game I could play with my younger brothers. We would play all sorts of games together when we were younger and as a young indie game dev I felt obligated to create a game we could play together.

## Game Modes
