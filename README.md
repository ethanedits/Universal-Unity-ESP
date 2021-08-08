# Universal-Unity-ESP
This is a *guide to creating* an **Internal ESP** for any [Unity Engine](https://en.wikipedia.org/wiki/Unity_(game_engine)) game. (Source included in the Universal-Unity-ESP folder, video guide [here](https://youtu.be/ww5OKW7GwCk))

NOTICE: If you have not created a Unity Internal before, I suggest you watch [my tutorial](https://youtu.be/VeMZ8NM9f3o) that will introduce you to the basics, and give you knowledge that will be needed for this guide (ex: creating a class library, reversing gameobjects, adding references, setting up loader, etc.)

![Example](https://github.com/ethanedits/Universal-Unity-ESP/blob/main/images/slapshotESP.gif)

## What Is Unity Engine?
***Unity Engine*** is one of the worlds **most popular** game engines, supporting a large array of platforms including *Mobile*, *Desktop*, and *Console*. Due to its success, many of the games you play are made in Unity, examples including **Rust**, **Escape from Tarkov**, **Totally Accurate Battlegrounds**, and **Muck** just to name a few. 

Unity Engine is built on GameObjects and Components. To understand the basics of how the engine works, please refer to [this slideshow](https://docs.google.com/presentation/d/1WvvUawFCj1t6eMqJY-LkWjJ2yWhQhqUnKAMLNBLQ2pM/edit?usp=sharing)   

## Creating an ESP

### *Getting the Player GameObject*
---------------------------------------------------

In a *regular memory based ESP*, the first step is to find the **Entity List**. The Entity List contains information on each player in the game, such as positional data, health, ammo, and more. 

In our case since we are making an Internal ESP we have access to the game's [GameObjects](https://docs.unity3d.com/ScriptReference/GameObject.html), and we can find the **player** GameObject and manipulate its [Components](https://docs.unity3d.com/ScriptReference/Component.html) directly, creating an **entity loop** by finding *every gameobject of a type in the game scene*. To find the player gameobject we need to use the software [dnSpy](https://github.com/dnSpy/dnSpy), which will allow us to view Unity Assemblies. We simply drag in our Assembly-CSharp and Assembly-CSharp-firstpass DLLs and we can view the games classes. Then trial-and-error your way through finding the right Player class (*usually it will be called something along the lines of player*), as pictured below I found it for **Muck**. All we need is the name of the class, so once you have found it you are ready for the next step.

![OnlinePlayer](https://github.com/ethanedits/Universal-Unity-ESP/blob/main/images/OnlinePlayer.PNG)

### *Getting GameObject Position*
---------------------------------------------------

Instead of having to find *pointers in memory* to the **entitylist** and then getting the entity position from there, we can simply use the [Transform](https://docs.unity3d.com/ScriptReference/Transform.html) component to get the position of the gameobject as a **Vector3**. 
```csharp
Vector3 gameObjectPosition = gameObject.transform.position;
```

### *Entity Loop*
---------------------------------------------------

Now that we know how to get a GameObject's position, and what ***GameObject is our player***, we can start *writing our ESP*. Inside of the `OnGui()` function we are putting our Entity Loop, which is a foreach loop that finds loaded objects of the type `OnlinePlayer` in the scene, and stores that as an array.
```csharp
void OnGUI() 
{
  foreach (OnlinePlayer player in FindObjectsOfType(typeof(OnlinePlayer)) as OnlinePlayer[])
  {
  

  }
}
```
From here we can implement the player position, but before we do so we need to distinguish the **difference** between the *pivot point* of a gameObject and the *origin point*.

### *Pivot Point and Origin Point*
---------------------------------------------------

In Unity, when you are getting the `transform.position` of a gameObject, you are getting the location of the pivot point, **not** the location of the origin point. What is the difference, and **why does this matter**?

<p float="left">
  <img src="https://user-images.githubusercontent.com/58463523/128572441-8c4bf97e-ac6a-4b42-996d-0f763ad22fc6.png" width="400" />
  <img src="https://user-images.githubusercontent.com/58463523/128572716-a0f43a2e-a2d2-4f4d-a2bf-043386907410.png" width="400" /> 
</p>

As you can see _above to the left_, the pivot point is in the _middle_ of the player gameObject on the Y (up/down) axis; with the player model being 2 Units tall, the pivot point is at Y:1.
This **causes issues** once we start rendering our ESP, because we want the ESP box to start at the bottom of the player and end at the head. So to fix this we need to offset the player position on the Y axis to get it to be at where the origin point should be. Some games will fix their pivot points and make them set to the feet of the player, but more often than not, you will find that the pivot point is in the center of the player, **not** at the origin.

### *Player Position inside Entity Loop and Player Height*
---------------------------------------------------

Now that you understand the **pivot point dilemma**, we can add onto our entity loop.
```csharp
Vector3 pivotPos = player.transform.position; //Pivot point NOT at the origin, at the center
Vector3 playerFootPos; playerFootPos.x = pivotPos.x; playerFootPos.z = pivotPos.z; playerFootPos.y = pivotPos.y - 2f; //At the feet
Vector3 playerHeadPos; playerHeadPos.x = pivotPos.x; playerHeadPos.z = pivotPos.z; playerHeadPos.y = pivotPos.y + 2f; //At the head
```
In the `pivotPos` Vector3 we are setting it to the pivot position (gameObject.transform.position), and using that to get the `playerFootPos` / **origin** position by offsetting the pivotPos by -2f on the Y Axis (this **WILL vary** depending on the height of the player model in **YOUR** game, but for Muck,  _2_ works for me.) We are then doing the same for the `playerHeadPos` but instead of subtracting to get the feet, we are adding to the pivot position to get the head position. The Image below should visualize what is being offset, with the total height being roughly 4 Units, and from the pivot point down or up is 2 units.

![Offsetting](https://user-images.githubusercontent.com/58463523/128575874-28875bf7-e188-456b-95ea-5c3a97ebe652.png)

### *WorldToScreen*
---------------------------------------------------

A _WorldToScreen_ function is used for **converting** _in-game coordinates_ (X,Y,Z) to _screen coordinates_ (X,Y). In [memory based ESPs](https://github.com/HeathHowren/CSGO-Cheats/blob/master/CSGO-GDI-ESP/Source.cpp), you need to get the viewmatrix, and use that in the WorldToScreen function. Fortunately, Unity has a [WorldToScreenPoint](https://docs.unity3d.com/ScriptReference/Camera.WorldToScreenPoint.html) function that allows you to get the screenpoint of a Vector3 from a [Camera](https://docs.unity3d.com/Manual/class-Camera.html) component.
```csharp
Camera.main.WorldToScreenPoint(Vector3 pos);
```
**Time to implement it!**

Inside our Entity Loop, under the player position code we wrote previously, we will create two Vector3's, that will be getting the **screen position** of the **head and foot positions**.

```csharp

Vector3 w2s_footpos = Camera.main.WorldToScreenPoint(playerFootPos);
Vector3 w2s_headpos = Camera.main.WorldToScreenPoint(playerHeadPos);
```

### *Rendering*
---------------------------------------------------

To _render_ our ESP, we need to use the [Render.cs](https://github.com/ethanedits/Universal-Unity-ESP/blob/main/Universal-Unity-ESP/Render.cs) class provided in the source.
This will allow us to simply _draw_ **lines**, **boxes**, and **text** on the screen (the functions simplify Unity's GUI functions)
