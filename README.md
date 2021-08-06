# Universal-Unity-ESP
This is a guide to creating an Internal ESP for any [Unity Engine](https://en.wikipedia.org/wiki/Unity_(game_engine)) game.


## What Is Unity Engine?
***Unity Engine*** is one of the worlds **most popular** game engines, supporting a large array of platforms including *Mobile*, *Desktop*, and *Console*. Due to its success, many of the games you play are made in Unity, examples including **Rust**, **Escape from Tarkov**, **Totally Accurate Battlegrounds**, and **Muck** just to name a few. 

Unity Engine is built on GameObjects and Components. To understand the basics of how the engine works, please refer to [this slideshow](https://docs.google.com/presentation/d/1WvvUawFCj1t6eMqJY-LkWjJ2yWhQhqUnKAMLNBLQ2pM/edit?usp=sharing)   

## Creating an ESP

### first steps in creating an ESP
In a *regular memory based ESP*, the first step is to find the **Entity List**. The Entity List contains information on each player in the game, such as positional data, health, ammo, and more. 

In our case since we are making an Internal ESP we have access to the game's [GameObjects](https://docs.unity3d.com/ScriptReference/GameObject.html), and we can find the **player** GameObject and manipulate its [Components](https://docs.unity3d.com/ScriptReference/Component.html) directly. To find the player gameobject we need to use the software [dnSpy](https://github.com/dnSpy/dnSpy), which will allow us to view Unity Assemblies. We simply drag in our Assembly-CSharp and Assembly-CSharp-firstpass DLLs and we can view the games classes. We simply trial-and-error our way through finding the right Player class (*usually it will be called something along the lines of player*), as pictured below I found it for **Muck**.

Instead of having to find pointers in memory to the entitylist and then getting the entity position from there, we can simply use the [Transform](https://docs.unity3d.com/ScriptReference/Transform.html) component to get the position of the gameobject as a Vector3. 
```csharp
Vector3 gameObjectPosition = gameObject.transform.position;
```
