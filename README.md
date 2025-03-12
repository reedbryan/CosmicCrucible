# Cosmic Crucible

<hr/>

Made using the Unity Game Engine, Cosmic Crucible is a 2D physics based brawler with support for joystick contrtoller and local multiplyer.

[Play online](https://simmer.io/@reedoover/cosmic-crucible)

[Download](https://reedoover.itch.io/cosmic-crucible)

## Motivation
My motivation for the project stemed from wanting to create a game I could play with my younger brothers. We would play all sorts of games together when we were younger and as a young indie game dev I felt obligated to create a game we could play together.

## GameModes

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

The CPU will select the entity with the largest desirability magnitude (positive or negative) as its "target", identifying the target as either *moving* or *static* and will approch the target occordingly.

#### Behavior
If the CPU's target entity is *static* it will simply point itself towards the target, applying thrust to move in that direction (see `targetStatic(GameObject mostDesirable)` in [CPU_Logic.cs](https://github.com/reedbryan/CosmicCrucible/blob/main/Assets/Scripts/CPU/CPU_Logic.cs)). 

If the target is *moving* there are different considerations. If the target has negative desirebility it will point try to avoid colliding with that entity to avoid collision damage or get out of the line of fire. If the target has positive desirebility, which in this case means it is a player that has been evaluated as attackable. It will use the targets current velocity and the CPU's expected projectile path to calculate the point where they are most likely to meet, targeting this point instead of the targets actual posistion.
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


### Custom Physics
#### Collisions
#### Drag

### Scaling Camera

### Graphics
#### Environment
#### Particle Explosions & Debris