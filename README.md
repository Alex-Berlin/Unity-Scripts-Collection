# Unity Scripts Collection
 Miscenallious utility scripts for Unity, made to be flexible and easy-to-use. Mostly for annoying systems you don't want to waste a lot of time on then prototyping.
 <br>More detailed description in the scripts themselves.

### PlaylistManager
Script for music playlist management. You want a lot different tracks to play during gameplay - use this. Play entire playlist, play specific tracks, play tracks by index, shuffle, add and remove tracks at runtime. Just add this component on your music player game object and use editor to set it up.

### PoolManager
Simple and performant object pool manager. Put the PoolManager component in your scene, put the prefabs you want pooled into component and use Get() and Return() methods instead of Instantiate() and Destroy(). Every script and variable is explained and has tooltips. Doesn't ruin your hierarchy using parenting system.
