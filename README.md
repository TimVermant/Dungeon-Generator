

Vermant Tim
Making a dungeon generator with a 3D navigation layout using the wave function collapse algorithm
Graduation work 2022-2023
Digital Arts and Entertainment
Howest.be

 
CONTENTS
ABSTRACT	2
INTRODUCTION	3
RELATED WORK	4
1.	Wave function collapse	4
1.1.	About wave function collapse	4
1.2.	Why wave function collapse?	4
2.	Procedural dungeon generator	5
2.1.	WFC applied to a dungeon generator	5
2.2.	GEnerating a 3D Layout	5
CASE STUDY	6
1.	introduction	6
1.1.	Intro	6
2.	WFc algorithm	6
2.1.	Basic implementation	6
3.	2D dungeon generator	7
3.1.	Basic implementation	7
3.2.	additional logic detached from the algorithm	8
4.	3D dungeon generator	9
4.1.	Basic implementation	9
4.2.	difficulty going from 2d to 3d	9
DISCUSSION	10
CONCLUSION & FUTURE WORK	10
APPENDICES	12

 
ABSTRACT
This paper will attempt to delve deep into the workings of the wave function collapse algorithm, how it could be used for generating 3D dungeon layouts and its practical usefulness inside of a gameplay context. The WFC algorithm itself is already fairly common among procedural content generation, and as such was a good candidate in attempting to create a working generator. 
By researching the algorithm itself and experimenting with a possible form of a dungeon generator, this paper showcases that it is definitely possible to achieve a working 3D dungeon generator and goes more in depth as to how well it does that and its limitations. The algorithm achieves this goal due to its main advantage being the adaptability to different layouts when going from a 2D layout to a 3D layout. The main issues it comes across are the fact that it has trouble generating certain dungeon layouts as opposed to others, for instance when generating a layout with smaller rooms connected to each other via corridors. While we do maintain some control over the result, the algorithm will always have a hint of randomness to it which in turn makes it so you can’t clearly define where a room might start or end. 
INTRODUCTION
How possible is it to make a 3D dungeon generator applying the wave function collapse algorithm? Although there are many algorithmic ways to go about creating a working 3D dungeon generator, the wave function collapse algorithm tends to have certain advantages making it desirable over other similar answers. It is a constraint based programming algorithm which allows for easy adaptability into different forms of layouts making it very useful when wanting to update the algorithm for example from 2D space into 3D space. 
While there are many possibilities as to what one might consider to be a dungeon, there are certain factors that are considered important when talking about the generated dungeons. They need to be procedurally generated, with the user having the ability to control the size of the dungeon and the frequency at which each piece could spawn. The dungeon also needs to be functional in the sense that it needs to allow for a player to walk through and reach every possible room from its starting position. 
This paper will attempt to further delve into and explain the inner workings of a WFC algorithm inside the context of a dungeon generator. We research and experiment exactly how the algorithm itself works and its usefulness when used for generating a 3D dungeon layout. 
RELATED WORK
1.	WAVE FUNCTION COLLAPSE
1.1.	ABOUT WAVE FUNCTION COLLAPSE
The Wave Function Collapse algorithm [1] is an constraint based programming algorithm[4]. With constraint programming you supply the computer with a clearly defined problem, which it then will try to find the best solution for on its own using the specified methods supplied by the user. 
The way the WFC algorithm works is by firstly taking a given input and generating an array with the dimensions of the desired output. With a set of rules, the algorithm then proceeds to try and collapse every element into its definite state, by collapsing a node you give it its final value and mark it as a collapsed node. It does this by looping over and finding the element with the lowest entropy, entropy meaning the amount of possible values a node could have after applying all the given rules, randomly selecting one of the possible states and finally collapsing that element, giving it its final value. Once it’s looped over and collapsed every given element, the code will exit and return the output. 
Due to those forementioned reasons, this algorithm does not only return a randomly generated output based on how you set it up, but it will also make sure that 2 random outputs with the same rules will result in 2 similar results. This allows for procedural content generation while still producing similar results which could be an advantage when wanting to produce consistently similar results as seen in the above figure 1. 
1.2.	WHY WAVE FUNCTION COLLAPSE?
When talking about possible algorithms for generating a dungeon or likewise procedural content generation, there are quite a few options as stated per [6]. A notable example of this being Cellular automata, a somewhat similar algorithm to wave function collapse. 

It works by taking a noise grid as seen in the below figure 3 on the left, it will then iterate over the given grid X amount of times, the amount of times you iterate over the grid will also determine the eventual result you get. We will then on each individual tile check its neighboring tiles, meaning on a 2D grid the 8 surrounding tiles, and depending on the rules you give it will then transform that tile. The terminology for considering all 8 surrounding neighbors is referred to as a ‘Moore Neighborhood’ inside of cellular automata. For example if you have a rule that anything above 4 wall neighbors means you fill it in and anything less you don’t, the more you iterate over this grid the more it starts to form a shape as seen in the below figure 3 on the right. 
Some of the similarities between cellular automata and wave function collapse are that they both iterate over a given layout and will then decide each cell’s value based on their neighboring tiles. While the WFC algorithm iterates once however, the cellular automaton can iterate multiple times changing the result based on the amount of iterations. Another difference would be that the WFC algorithm starts out on an empty grid before collapsing every node while cellular automata would require an existing noise grid, for which the noise density can also be a factor in determining the result. While it is possible to have a cellular automaton work inside of a 3D space, the result in both 2D and 3D end up leaning more towards natural cave like generations rather then actual dungeon generation which is the main reason we won’t be using this for our purposes.  

As briefly mentioned in the previous chapter, the WFC algorithm is a form of constraint algorithm which means it also comes with certain pros and cons. One of the advantages of the algorithm being constraint programming is that there are a lot of possibilities to extend the algorithm by changing the constraints we use [4]. This makes it that although the original algorithm wasn’t written for 3D usage, it still is easily adaptable to be used in 3D and in just about any type of layout the user would like to apply it to. For example if one were to move on from a 2D square based grid to a 2D hexagonal based grid this would be very easily doable inside of the wave function collapse algorithm with minimum alteration required. Another advantage worth mentioning is the fact that the algorithm itself is quite independent minded, meaning that it requires almost no outside help or instructions once its set up properly. 

These advantages make the algorithm quite useful in just about any form of procedural content creation as seen here [2], making it quite common inside games requiring procedural content. An example of this would be Caves of Quds, a game containing various parts that are procedurally generated and uses the WFC technique as part of that content generation. 
 

2.	PROCEDURAL DUNGEON GENERATOR
2.1.	WFC APPLIED TO A DUNGEON GENERATOR 
The setup we need to have a WFC algorithm work as a dungeon generator is similar to that of a regular WFC algorithm, you start off by generating a list of cells in a desired layout with dimensions you set beforehand, to then start iterating over every node and find the node with the lowest entropy. It calculates this by checking the adjacency and making sure that if for instance node A has a wall on the edge to node B, node B then also needs to have a wall going to node A. Later when we move on to a 3D layout when also need to do this check with the ground and ceiling when spawning in staircases. 
Even when using the WFC as a basis, there are still a multitude of possibilities as to how exactly you would start creating your generator, depending on how exactly you want your final result to look like. A major thought-point being the desired layout of your dungeon outputs. We could decide for instance if you want big rooms with a higher chance of walls to generate next to each other, or have a smaller chance for walls to generate next to each other to limit the sizes of each room. Another way to set up the generator would be to instead of having an amount of smaller rooms adjacent to each other have a big maze-like area with corridors, dead-ends and only one possible way to enter and exit. 
One thing to note however is that while technically you could achieve almost any result with this algorithm applied to a generator, some layouts require more works then others. For example the algorithm might have an easier time generating a big open maze-like area compared to having a number of smaller rooms connected via open corridors. This is due to the fact that a lot of the content generated from this algorithm relies on randomization which  makes it harder to control precisely how the layout per level might look like. 
2.2.	WAYS TO IMPROVE THE CODE THAT BENEFIT YOUR GENERATOR
2.2.1.	WEIGHTED RANDOMIZER
The algorithm itself wasn’t written for the sole purpose of being used inside of a dungeon generator, so naturally there are some changes separate from the algorithm itself that could improve the end result of your generators. Whilst having a weighted randomness isn’t necessarily a requirement, it is definitely recommended to add this since it vastly improves the amount of control the user could have on the end result.  
One way you can give more control to the user about the layout without having to alter the source code is by adding weighted randomness to each possibility. It works by giving each individual cell a ‘weight’ value, usually a float number ranging from 0 to 1, which will then be used to have a more weighted randomness inside of your algorithm. When the algorithm then gets to a point where it tries to collapse a cell with an entropy not equal to 1, entropy meaning the possible values a cell could have, it will instead of a total randomness take the weight of each piece into account before randomly selecting the value it wants to collapse to. 
 
2.2.2.	MAKING SURE EACH AREA IS ACCESSIBLE
When procedurally creating a randomly generated dungeon, most of the time we don’t actually want to have complete randomness. A big factor in having a successfully generated dungeon is the fact that it needs to work gameplay-wise and correctly allow for the player to traverse this dungeon without running into dead ends or certain inaccessible areas. 
In order for us to check whether or not each area is accessible, we would end up using a pathfinding algorithm. What specific pathfinding algorithm you end up going for comes down to personal preference, but a popular one to use would be the A* pathfinding algorithm. An algorithm that takes into account the distance between the start node and current node and the distance between the current node to the end node to calculate an optimized path to the final destination. 
Because of the way that the WFC algorithm works, you will have to take into account both the collapsed and the non-collapsed nodes since a node that isn’t accessible could become accessible once the algorithm iterated and collapsed other nearby nodes. Once you have this set up and then run into a certain area that is inaccessible, assuming you set it up correctly taking all the nodes into account, there are a few things you could do to fix this. 
You either leave the node as is and at the end when the algorithm is done iterating, you remove any nodes the user won’t be able to access replacing them with walled off nodes. This way you won’t have to worry about those nodes getting deleted and leaving behind big gaping holes the player could fall through. Another option is to check the pathfinding before collapsing each node and then removing that option until you get a value that doesn’t block off part of the dungeon. You would then of course have to take into account the edge cases when for instance no ‘correct’ value gets found to then give it a default walled off value, at which point the previous option would be a better choice. 
 
2.3.	GENERATING A 3D LAYOUT
While there are always more ways in which we optimize and add to the code, an important aspect that needs to be taken into account would be the transition from a 2D grid layout to a 3D grid layout. This won’t be that big of a task since one of the previously mentioned advantages of using the WFC algorithm is the fact that updating the workings from a 2D layout to a 3D layout doesn’t require a lot of effort. This is because, as mentioned previously, it works in a similar way to the 2D implementation. The user here when creating the rules for the algorithm will have to take into account an extra dimension when calculating and collapsing a given node. 
The initial result will not be our desired result as having the ability to walk and go upwards in a dungeon is not taken into account in a 2D version since the only roadblock would be a physical obstacle blocking off the player. We would probably want to implement some form of staircases or sloped ramps as seen in the figure example above to be able to walk onto different levels. This however will require more work beyond the basic WFC implementation, depending on how we want the player to be able to travel in between different levels. An obvious way you could do this is by adding staircases that would need to be accessible on both the level below as the level above. This would require us to additionally make sure you have pieces present that have no floor to allow the staircase to be useable.
 
CASE STUDY	
1.	INTRODUCTION
1.1.	INTRO
The case studies inside of this research are rather straightforward, being incremental steps towards the final goal of this research, a working 3D dungeon generator using the WFC algorithm. We start off with writing a basic form of the WFC implementation inside of C++, just to show a basic example of how the algorithm might work on a 2D grid. We then move on and apply this algorithm to a 2D dungeon generator inside of Unity to actually visualize the output better. And as a final step we alter our code to work on a 3D layout to then have our final result.
2.	WFC ALGORITHM
2.1.	BASIC IMPLEMENTATION
Before getting started on the dungeon generator, we first I want to try and figure out the workings of a very basic Wave Function Collapse algorithm. We can make this inside of C++ with a basic 2D grid layout and using numbers to represent the state of each item. The language is not of importance here as this is more a proof of concept then anything. The rules were also very simple, where each number would either be one above, below or equal to the 4 neighbor(s) going up to the maximum. In this example the maximum is 4, after which the counter loops back around and so one above 4 would be 1. In the figure below you can see an example of this with a before and after showing the effect the algorithm might have on a grid of numbers.
Of course this version is not complex enough and doesn’t take into account for example the non-existent nodes next to the edges outside of the grid, but is sufficient enough to have a reference example when getting started on the next step in the process. 
 
3.	2D DUNGEON GENERATOR
3.1.	BASIC IMPLEMENTATION
Once done with the basic algorithm implementation, we can move on to applying this in our context: the generator. The basic implementation of the generator makes use of square tiles, each tile having 4 possible directions where a wall would be present or not. Inside the program there is a list of handcrafted tiles with each tile having 4 sides and the possibility to have a wall on each side. The generator will then apply the algorithm and decide its entropy based on the amount of nearby collapsed items, the applied rule here is that two adjacent tiles should either both have no wall or both have a wall. Many more extra rules will be and could be added in the future but those rules are the basis on which the algorithm works and generates with. 
As we can see above in figure 11, this iteration has some issues and rules that are missing and should still be added. One of these being the fact that the generated dungeon isn’t closed off and seemingly ignores the out of bounds items next to the edges. Underneath the visuals, the program doesn’t just check every direction whether or not it contains a wall but rather stores all possible tiles that could go in each possible direction. This doesn’t allow for much scalability when wanting to add new pieces, so in the following iteration this got adjusted to storing a Boolean linked to each direction to fix this issue, as seen below in figure 12.
3.2.	ADDITIONAL LOGIC DETACHED FROM THE ALGORITHM
We are almost done with the 2D layout, but will still have to adjust and add a few extra things before we can move on to our 3D layout. One of those additional features is the addition of a weighted randomness, which will grant the user more control over occurrence of each piece and thus control the eventual result(s) more. 
Every piece will be given a value ranging from 0 to 1 and the higher the value, the higher percentage change that piece has of being picked as a random option. Doing this will allow us to add more potential pieces without having the layout be too randomized and generate different results depending on the type of dungeon that we want to generate.
Once this is all done, the basic 2D version is ready and we can get started on adjusting the code to work with a 3D layout instead. Starting from this point on, the preparation is finished and we will start working on the final desired result being a 3D procedural dungeon generator. Later we might revisit and adjust some smaller things if needed, but for now this part will remain as it is.
 
4.	3D DUNGEON GENERATOR
4.1.	BASIC IMPLEMENTATION
One of the clear advantages of the WFC algorithm is easy adaptability and by adding an extra dimension in the layout the 2D grid becomes 3D. Of course just adding an extra dimension doesn’t give a desired result, we also need to take into account the 3D adjacency for cells that are above or below each other. This is a requirement for having clean connections between the different height levels, to then for instance use staircases as a way of connecting each level with one another.
4.2.	DIFFICULTY GOING FROM 2D TO 3D 
As mentioned before, making the generator work from a 2D space to a 3D space comes with its difficulties and issues we will have to be able to fix. One of these prominent issues is having staircases that won’t just connect but also make sense and allow for traversing the different levels when wanting to play the level. At first the staircase pieces had 3 walls, and the staircase having an opening in 1 direction and an exit in the other. This turned out to generate either a lot of issues or would have the staircases spawn almost never since the requirements for their appearance were too specific. 
So what ended up being the solution that we went for is to have each staircase take up less space and make it so that it has no walls and can be accessed both at the entry and exit point from any direction. This made it so the staircases would have less chance of closing off certain spaces or making it lead to a dead end. Another benefit of this was the fact that the rotation of the staircases didn’t matter as much as before. Where before it needed to aim in the most correct way for the staircases to work, in this iteration the staircases only cared about the wall behind it and whether or not the adjacent tile above either had not been collapsed or was collapsed into a tile piece with an open floor. 
4.3.	CONCLUDING THE CASE STUDY
After many iterations, we were able to iron out some of the existing issues and bug in the system to have a final working result. We were able to add certain parameter values to the user so that, when this tool eventually might get used in a gameplay context, they could tweak and change those values in order to control and generate a desired output depending on what game it might be used for. It is a tool to be used that takes over parts of the grunt work one might have to do in a game development pipeline for quickly generating layouts, but beyond that it shouldn’t be used as a procedural tool that could generate a dungeon at runtime since some tweaking afterwards is necessary for adding points of interest like dungeon chests or certain enemies. 
To start off we have the basic values controlling the amount of rows, columns and levels going up to determine the basic outline shape and to not limit the user to just having a cuboid grid, meaning it potentially has 3 different values per dimension as seen in the above figure. Secondly, a big part of what would make this generator useful is the ability to control the weight of each type of piece to have different possible layouts the user could decide on, allowing for variety in the inner layout. And then a final extra added as a way of making it easier on the user by pressing the ‘Generate Example’ button, one can store the current setup values as an existing example so that the user can play around and decide what result they prefer without having to remember each value and manually filling them in each time. 
 

DISCUSSION
The results indicate that the wave function collapse algorithm is indeed an algorithm that works well for the purpose of creating a 3D dungeon generator. The algorithm is easily adaptable to different layouts as seen in the case study when moving from 2D to 3D. By giving the user control over the main parameters such as the size of the layout and the frequency of each possible value allows for random results that still maintain some general consistency every time it gets executed.  Its main strength lies in granting a variety of possible outputs with a limited amount of interference necessary, once the main generator part gets finished. However, a weakness present that other algorithmic approaches might not have would be that this algorithm isn’t well made for generating a dungeon layout with smaller rooms that connect via a corridor. This is due to the fact that it has a certain degree of randomness that won’t guarantee that result. 
 
CONCLUSION & FUTURE WORK
As procedural content generation is a popular way of making new content inside of the gaming sector, it is useful to have a look at how you would go about making a 3D dungeon generator utilizing the wave function collapse algorithm. The way it works is by adding rules for taking the adjacency into account and adding weights to the different pieces to give the ability to control and manipulate the result of each iteration. It might not work as well for certain types of dungeons such as a layout with a number of smaller rooms connected via corridors, it is not only possible but also a viable option depending on exactly what end result the user would want to achieve. And while other algorithms might have an easier time generating such layouts, the WFC algorithm given enough attention would still be able to achieve these results with enough alteration.
There are ways in which we could go about improving this generator, for instance by implementing a pathfinding algorithm to ensure that there are no dead ends by limiting the possible values. One could also remove certain areas that are not accessible to the player and perhaps add certain gameplay elements such as in game items or dungeon chests to spawn in areas designated as treasure rooms.   

 
BIBLIOGRAPHY
1.	"GitHub - mxgmn/WaveFunctionCollapse: Bitmap & tilemap generation from a ...." https://github.com/mxgmn/WaveFunctionCollapse.
2.	Roguelike Celebration. (2019, 23 oktober). Brian Bucklew - Dungeon Generation via Wave Function Collapse. YouTube. https://www.youtube.com/watch?v=fnFj3dOKcIQ 
3.	Parker. (z.d.). Generating Worlds With Wave Function Collapse - PROCJAM Tutorials. https://www.procjam.com/tutorials/wfc/ 
4.	Procedural Dungeon Generation: Cellular Automata · jrheard’s blog. (z.d.). https://blog.jrheard.com/procedural-dungeon-generation-cellular-automata 
5.	Johnson, Lawrence & Yannakakis, Georgios & Togelius, Julian. (2010). Cellular automata for real-time generation of. 10.1145/1814256.1814266.
6.	The Brave, B. (2020, 3 april). Wave Function Collapse Explained. BorisTheBrave.Com. https://www.boristhebrave.com/2020/04/13/wave-function-collapse-explained/
7.	GDC & Bucklew, B. B. (2019, 11 april). Tile-Based Map Generation using Wave Function Collapse in “Caves of Qud” [Video]. YouTube. Geraadpleegd op 29 december 2022, van https://www.youtube.com/watch?v=AdCgi9E90jw 
8.	Dungeon Generation - Procedural Content Generation Wiki. (z.d.). http://pcg.wikidot.com/pcg-algorithm:dungeon-generation 
9.	Novoseltseva, E. (2022, 29 maart). Constraint Programming. Apiumhub. https://apiumhub.com/tech-blog-barcelona/constraint-programming/ 
10.	Pepe, F. (2022, 3 november). What makes a good RPG dungeon? A look at 10 games. Medium. https://felipepepe.medium.com/what-makes-a-good-rpg-dungeon-505180c69d00 
11.	Klayton Kowalski. (2020, 19 september). Game Development Tutorial | Cellular Automata and Procedural Map Generation. YouTube. https://www.youtube.com/watch?v=slTEz6555Ts 
12.	Swift, N. (2020, 30 mei). Easy A* (star) Pathfinding - Nicholas Swift. Medium. https://medium.com/@nicholas.w.swift/easy-a-star-pathfinding-7e6689c7f7b2 
13.	457036, R. (z.d.). GitHub - raj457036/Path-Finding-Visualizer: An easy to use Interactive Graph Path visualizer with batteries included to implement your own Algorithm. GitHub. https://github.com/raj457036/Path-Finding-Visualizer 
14.	The Coding Train. (2022, 3 juli). Coding Challenge 171: Wave Function Collapse. YouTube. https://www.youtube.com/watch?v=rI_y2GAlQFM 










