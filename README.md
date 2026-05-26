# DOTS-Space-Game
### Movement System
The Entity Object stores the information on how a given character should move, which is accessed by a Job in the Movement System, which then operates that movement. Meanwhile, the system moves the projectiles linearly and makes sure they don't move outside a certain area. 

The player controls this input manually while the computer analyses the position and rotation of each enemy relative to the player's position and assigns the given input accordingly.
### Damage System
A SystemBase inheriting script using BURST checks the distance between projectiles and enemíes, and enemies and the player, respectively, and assigns damage.
### UI
After some basic research, I concluded that operating the UI purely with Entities would be a waste of time, so I used SystemBase, inheriting scripts and calling Events to update the UI.
