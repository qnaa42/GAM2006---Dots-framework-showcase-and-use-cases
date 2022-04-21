# GAM2006 - Dots framework showcase and use cases


This repo has been created for GAM 2006 assignment. It showcases use cases and the possible application of a data-oriented approach using the unity ECS framework. 

All of the projects are built on Unity version: 2020.3.24f1 It is highly recommended to use the same version due to the instability of packages.

Repo contains 5 projects showcasing different functionality available. Due to the early stages of stack development, it is extremely hard to produce meaningful gameplay; however, I hope this project will be able to highlight the importance and flexibility of DOTS.

Content: 
 01: Transformation Mathemathics - project showcase how to spawn and manage entities and apply Transformation mathematics to them. 
 02: Pivoting Rotations - showcase how to apply known from OOTS Vector transformation of following target rotation and how data must be translated to secure immutability of data. 
 03: Base gameplay example 1 - a showcase of covered above solutions on the example of simple gameplay part using spaceships following and attacking waypoints present in the environment. Limited to Transformation mathematics and without Physics use.
 04: Procedural generation - a showcase of the pure computational power of the framework on the example of dynamic Perlin noise generation. Without using any explored professional solutions like rendered skinning or GPU instantiating. 
 05: Base gameplay example 2 - showcasing the application of ECS physics and application of mixed solution where both Entities and GameObjects are used to achieve gameplay. 

Examples 3 and 5 showcases the generation of entity-based particle effects. 