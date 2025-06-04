# Developing wave effects in Unity

## Gerstner Waves

Gerstner waves are one of the most popular approaches for modeling realistic ocean waves. This is a comprehensive extension of the traditional Gerstner wave, also known as the trochoidal wave. Gerstner waves offer a computationally efficient and visually appealing way to model water surfaces. They are based on a parametric representation of individual wave components, each of which is set by amplitude, frequency, direction, and phase. The final water surface is obtained by summing all the components.

To implement the Gerstner wave effect in Unity, you must first create a Lit Shader Graph and change the Surface Type to Transparent in the Graph Inspector settings, and set the Blending Mode to Alpha mode in order to further add a transparency effect to the water surface. Each wave is characterized by its amplitude, wavelength, speed, and phase. For each vertex of the wave, its displacement along the X, Y, and Z axes is calculated based on the wave parameters. The total height and offset are calculated by adding the contributions of all the waves. For each individual Gerstner wave, the parameters that need to be added in the Blackboard section are used (Fig. 1):

* WaveDirection type Vector3 (x = 1, y = 0, z = 0): waves move in the X-axis direction by default;

* WaveLenght of Float type (x = 4.5): using this variable, the distance between the tops of the waves is controlled;

* Float type waveHeight (x = 0.2): wave height;

* Float type PeakSharpness (x = 0.3): allows you to control the shape of the wave crest – how sharp or round the top of the wave is.

![image](https://github.com/user-attachments/assets/ea8940e9-29b6-4e2a-ab49-412340b7ed0f)


                                Image 1 GerstnerWave parameters



For each wave at each point in the plane, it is necessary to calculate the phase of the wave at point and time t (Fig. 2). Here the notation:

* ʎ is the distance between the crests of the waves, in Unity the variable WaveLenght; 

* k is the vector of the wave number; 

* φ is the initial phase of the wave, which allows you to shift the wave in time.; 

* ω is the oscillatory frequency of the wave (velocity).

  
![image](https://github.com/user-attachments/assets/57ed9415-c21f-4043-87d0-8a687f497680)


                               Image 2 Gerstner wave formulas



In order to set the speed or time of movement of the wave tops, you will need a constant equal to the number 2π. In Unity, you need to create a Constant node and set the TAU parameter to it (system number 6.28). Using the Divide node, divide the WaveLenght by 2π. Then you need to enter a gravitational constant to control the speed of the waves. Using the Multiply node, multiply the gravitational constant with the result of dividing the Divide node, extract the square root from the resulting number, and finally multiply by time. This mathematical formula allows you to control the speed of movement of the wave tops (Image 3).

![image](https://github.com/user-attachments/assets/01232f93-e6ce-4073-bda3-6c125beaad97)

                               Image 3 The velocity of the wave peaks of the Gerstner wave


   

The phase of the wave depends on the position of the vertex along the direction of propagation of the wave and the current time, taking into account the speed. When subtracting the scalar product from time, a wave is modeled that moves along the direction of the wave at a given speed. In order to allow the phase to "shift" in time and the wave to "ride" on the surface, you will need to create a Position node and set the Space parameter to Object mode. The WaveDirection parameter is a wave direction vector that is normalized using the Normalize node to obtain a unit vector, a vector with length equal to 1 that preserves the direction of the wave. Normalization is necessary in order to further use this vector for calculations related to the direction of the wave (for the scalar product), while avoiding scale distortion due to the length of the vector. Then the result of the Divide node must be connected to input B of the new Multiply node, and the Normalize node must be connected to input A. A Dot Product node is created between the Position and Multiply nodes to calculate the dot product. The value of the Dot Product node is subtracted from the wave velocity using the Substrate node. 

Any implementation of a sea wave uses a sine for horizontal vibrations, creating a shift of the wave back and forth in the direction of movement, and a cosine for vertical displacement (raising and lowering the top). First, a Cosine node is created, a Substrate is connected to it, and then multiplied by a Vector 3 node, which filters the values along the X and Z axes. In order for the vertices of the wave to move only up and down, the values are set: x = 0, y = 1, z = 0. Then you need to multiply the result of the Vector3 multiplication by the waveHeight parameter. The result of the last Multiply is how much the vertices are shifted along the Y axis. It is necessary to add the position of the vertex in the object's space and add it to the displacement value. This is the basis of the Gerstner wave formula (Image 4).

![image](https://github.com/user-attachments/assets/66340856-0001-4f0c-a65c-9020cc8b0773)

                                Image 4 The basis of the Gerstner wave formula



For a more realistic Gerstner wave effect, it is also necessary to shift the vertices not only vertically (Y-axis), but also horizontally (X and Z axes). This allows you to create the effect of shifting the wave crest, which visually enhances the feeling of a real wave. The sine of the wave phase is used to calculate the horizontal displacement. In this case, the shift occurs along the direction of the wave movement, which is achieved by multiplying the direction vector of the wave by a scalar displacement value (Image 5).

![image](https://github.com/user-attachments/assets/a870f31e-a274-4c55-bdd1-368e8c8d3660)
![image](https://github.com/user-attachments/assets/fd659d69-792a-45fd-8fa6-7d817eac6863)


                                Image 5 The formula of horizontal displacement



It is necessary to create a Sin node that accepts the input value of the wave phase (the same as used in Cosine). In order to adjust the offset depending on the amplitude of the wave, the Devide node from the Speed block must be connected to input A of the new Multiply node, and input B must be connected to waveHeight.
Then divide the PeakSharpness by the resulting Multiply value, and multiply the result again by waveHeight. This procedure allows us to obtain a coefficient that scales the horizontal displacement according to the sharpness of the peak and the height of the wave. Using the new Multiply node, you need to multiply the value of the Sin node with the value of the previous node. The wave shifts the peaks along the direction of its movement. Multiplication by the normalized WaveDirection vector allows you to "direct" the horizontal displacement in the right direction. Normalization of the vector is important so that it does not affect the amount of displacement, but only the direction of movement (Image 6).

![image](https://github.com/user-attachments/assets/a923f212-9388-4ad5-8208-8371c1ac3091)

                                Image 6 Addition of the Gerstner wave formula

				

In order for the water surface to acquire a more realistic reflection, it is necessary to shift the normals, that is, you need to calculate the derivatives of the displacements obtained in the previous steps and apply them to change the direction of the normals. Both sine and cosine are used to calculate normals in tangent space. To calculate the X and Z components, the sine value must be multiplied by the result of multiplying the first Multiply node from the Sharpness block, then combine this value with the normalized WaveDirection vector and divide it into the X and Z components using the Split node. The Y component is calculated in the same way, but the cosine value is used instead of the sine. This value is then subtracted from one using the One Minus node to "lift" the normal up, reflecting the actual waveform, not just its plane. This adjusts the orientation of the normals for a visually correct reflection. The inputs of the Combine node are connected to the resulting outputs of the One Minus and Split nodes and connected to the Master Node (input Normal) (Image 7).

![image](https://github.com/user-attachments/assets/b1c3a840-a2f4-464f-9dcb-3a45c75da377)

                                Image 7. Gerstner waves in Shader Graphics

	After creating and saving the shader, you need to create a new material based on it and add this material to the plane to demonstrate the Gerstner wave effect. This shader allows you to control the direction of the wave, the height of the wave from the lowest point to the crest, the distance between the peaks of the wave and the sharpness of the crest of the wave. Due to the correct displacement of the normals, a visually pleasing result is obtained – the waves deform, creating the illusion of light reflecting from the surface of the waves (Image 8).

 ![image](https://github.com/user-attachments/assets/d9b75f4e-ba8b-4e7a-a6de-802df2881d29)

                                Image 8 Gerstner wave effect

The effect of the classical Gerstner wave looks quite attractive, but not realistic enough. To create more complex and realistic waves, you can combine several Gerstner waves with different parameters (direction, amplitude, frequency).

First, you need to create a SubGraph from a Gerstner wave. This will make it possible to encapsulate the logic of a single Gerstner wave and use it several times without cluttering the main shader graph. Set the Output Node to two outputs: Offset type Vector3 and Normal type Vector 3. Connect the outputs of the received values to the corresponding inputs of the Master Node. Then, in the main Shader Graph, you need to create three GerstnerWave nodes and create the appropriate input parameters for them in the BlackBoard section. It is necessary to sum the Offset and Normal outputs for each instance of the SubGraph, and connect the results to the Vertex Position and Vertex Normal inputs (Image 9).

![image](https://github.com/user-attachments/assets/d725e8f1-88f0-4e99-90a7-567421ca0ef6)

                                Image 9. Gerstner waves in Shader Graphics

After applying a new material based on the Gerstner wave shader, a dynamic water surface consisting of several Gerstner waves is obtained. By combining waves with different parameters, much more complex and interesting wave patterns are obtained than using a single wave. The ability to adjust the parameters of each wave in the Inspector section provides full control over the appearance of the water surface, allowing you to create realistic and beautiful visual effects (Image 10).

![image](https://github.com/user-attachments/assets/5ef1c4d9-537a-4257-9b2d-4e7eeac720d7)

                                Image 10. The finished Gerstner wave effect


https://github.com/user-attachments/assets/956741f9-5ffe-4173-a0c3-ac2fff31f567

                                the Gerstner wave effect
    
# Visualization of the caustic effect

Caustics are understood as light patterns that occur when rays meet a curved or uneven surface (for example, water, glass). The rays pass through a transparent medium, refract and create dynamic bright lines or shapes. In the context of developing visual effects for game projects, accurate modeling of the caustic effect significantly increases the realism and visual appeal of scenes that use transparent or reflective materials.

Historically, accurate visualization of caustics has required complex calculations of global illumination and photonic imaging, which are computationally expensive and impractical for game projects. Approximate or precomputed methods are often preferred instead.: 

* Projected textures (English lightsookies): A common real-time approximation involves projecting a texture onto a surface from a light source, simulating the appearance of caustics without complex light tracing.

* Screen Space effects: The use of screen space shaders that distort and brighten areas depending on the depth of the scene and the normals to simulate the concentration of light. This method uses information about the screen space to approximate the caustic effect. It is less computationally expensive than other methods, but provides lower accuracy. This method usually involves visualizing a separate passage, in which the light intensity is calculated based on a map of the normals of the water surface and the position of the light source. This intensity map is then mixed with the final rendering of the scene to simulate the caustic effect. Limitations include insufficiently precise refraction and possible artifacts at high wave frequencies or near the edges of the water surface.

* Blob (English blob) or spotlights with animated textures.
Pre-calculated caustic values: For static or slowly changing water surfaces, high visual accuracy can be achieved with acceptable performance using pre-calculated caustics. This method involves creating a set of caustic textures offline based on different profiles of the water surface and the location of light sources. During the execution, the appropriate texture is selected and applied to the surface of the water. Although this method provides high-quality results, it is limited to scenarios with predictable water movement. Significant changes on the water surface will require repeated calculation and regeneration of textures.

* Ray Tracing: Involves tracing rays from the camera through the surface of the water to the scene to simulate refraction and reflection. By accumulating the intensity of light along these rays, the algorithm can accurately generate caustic images. However, this method requires high computational costs, which makes it unsuitable for large-scale modeling of water bodies or scenes with a large number of polygons. It is better suited for smaller, more detailed water bodies or as an additional method to enhance individual areas.

* Decal projector: Acts as a tool that projects a texture (decal) onto the surface of objects in a scene. This allows you to add details such as scratches, inscriptions or, in our case, caustic patterns, without having to change the basic models. The projection is applied only in a certain volume, which allows you to control the scope of the effect.

* Custom shaders.

* Decal Projector and Shader Graph together allow you to create a dynamically changing and customizable caustic effect. Decal Projector is responsible for projecting the caustic texture onto the target surfaces, and Shader Graph provides fine-tuning of the appearance of this texture, its animation and interaction with the lighting of the scene. Combining these methods allows you to achieve a relatively realistic caustic simulation with moderate computational costs, offering a flexible and customizable way to improve the visual quality of game scenes.

So, to create the simplest caustic effect using Decal Projector and Shader Graph, you need to check the visualization tool used by the camera in the scene and add a Decal object. Then create an Empty Object in the hierarchy of the scene and add the Decal Projector as a component of the empty object. The type of shader is important because Decal shaders are specifically designed to overlay existing materials without changing their basic properties, so it is necessary to create a Shader Graph Decal to realize the caustic effect (Image 11).

![image](https://github.com/user-attachments/assets/e16520f7-cf3a-4cca-bcb6-0aa0d3c59860)
![image](https://github.com/user-attachments/assets/6b91180c-c738-429a-b873-6ea8ed6d9cac)

                                                Image 11. Inspector parameters for the Decal object

In the Graph Inspector settings, you must disable the Normal and Blend effects in order not to change the surface normals or mix colors in a standard way. Then add the parameters in the Blackboard section (Image 12):

* Color type (default: white): allows you to set the color of caustic highlights;

* Offset type Float (x = 1): shifts the coordinates of the texture to create movement.

* Float type Density (x = 5): affects the frequency or density of the caustic texture.

* Float type Power (x = 2): determines the contrast between the light and dark areas of the caustic.

* Alpha Float type (mode in Slider mode): transparency of the caustic effect.

![image](https://github.com/user-attachments/assets/5cdc626b-aa78-4470-b74f-51c5bf606f85)

                                                Image 12. CausticShader parameters

To simulate the refraction of light under water, creating a caustic effect, the method uses the Voronoi node, which creates a Voronoi texture. This texture generates random cells that resemble the structure that occurs when light is refracted. By changing the UV coordinates (the world coordinates of the object in this case) of the Voronoi texture over time, the illusion of light and shadow movement characteristic of caustics is created. Multiplying time by Offset allows you to control the speed of this movement.

To begin with, a Position node is created, which provides information about the object's position on the scene, thereby allowing the object's position to be used for the UV coordinates of the Voronoi texture so that the caustic remains anchored to the object. Using the Split node, the position is divided into separate components (X, Y, Z). Then the Offset animation speed control variable is multiplied by the time of the Time node using the Multiply node to create movement and connected to the Voronoi node. The Density parameter is connected to the corresponding Voronoi node input to control the density of the effect. The Voronoi node is then connected to the Power node to control the color gamut and contrast of the effect. The Saturate node closes the caustic effect, which limits the output values in the range from zero to one. The Alpha parameter is necessary to control the transparency of the caustics. It is set to Slider mode because Alpha should have values from zero to one (Image 13).

![image](https://github.com/user-attachments/assets/08ae4c2c-25d0-4f46-85c7-d621182f81d7)

                                                Image 13. Caustics in Shader Graphics



To demonstrate the caustic effect, you need to save the shader, create a material based on it, and add this material to a Decal object. This shader allows you to create a dynamic and realistic caustic effect that is anchored to an object and animated in time. Using procedural textures such as Voronoi allows you to avoid using static textures and get a more flexible and scalable result. This method is particularly suitable for cases where it is necessary for the caustic to move realistically with the object, for example, when simulating light passing through water on the surface of a moving object. Using decals allows you to project the effect onto existing surfaces without changing their geometry. 

This shader allows you to set the color that is used for caustic highlights, shift the coordinates of the caustic texture to create the illusion of movement, control the frequency of the texture and the contrast of the pattern (Image 14).

![image](https://github.com/user-attachments/assets/6b0e43fe-5778-4630-a1e2-b026d7951c00)

                                                Image 14. The finished caustic effect

https://github.com/user-attachments/assets/f38996b1-018c-459f-8e51-fe0cf9b208e3

                               the caustic effect        

      				
                                                
# Underwater effects and their implementation

Several methods can be used to simulate underwater effects in Unity.:

* Post-processing effects: They are a relatively effective way to create an overall underwater look.  These effects can be implemented using Unity's built-in post-processing stack
or custom shaders.

* Custom shaders.

* Particle systems can be used to simulate the visual effects of suspended particles in water. Particles of different sizes, colors, and speeds can be used to create a variety of effects, from clear water with fine particles to turbid water with high sediment concentrations.  These particles can interact with light, which further enhances the realism of the underwater scene.

* The interaction of objects with the water surface requires special attention. The effects of refraction on a surface can be modeled using techniques such as ray tracing or refractive shaders.  The distortion of objects visible through the water should be visually convincing, which increases the overall plausibility of the scene.

Given the variety of approaches to modeling underwater effects, two key elements are implemented within the framework of the task: first, realistic underwater bubbles using a particle system combined with the capabilities of a shader graph to fine-tune visual characteristics; secondly, a dynamic school of fish animated using a script that allows you to create plausible behavior and movement. underwater inhabitants.

# Creating an underwater bubble effect

To create a visually appealing and believable bubble, it is necessary to take into account properties such as transparency, slight distortion of the image behind the bubble to simulate the effect of light refraction, Fresnel glare and animation. For the underwater bubble texture, you need to create a Shader Graph Lit and change the Surface Type to Transparent in the Graph Inspector settings, and set the Blending Mode to Alpha mode in order to add the effect of bubble transparency. Then add the parameters in the Blackboard section (Image 15):

* Float type NoiseScale (x = 0.5): Controls the scale of the noise texture, which is used to create distortions and irregularities on the bubble surface.

* Float type NoiseSpeed (x = 0.5): controls the animation speed of the noise texture.

* FresnelPower type Float (x = 0.5): controls the intensity and width of Fresnel glare. The Fresnel effect simulates the reflection of light from a surface depending on the viewing angle. The closer the viewing angle is to the tangent, the stronger the reflection. This creates distinctive bright highlights around the edges of the bubble. 

* FresnelPower determines how strongly this effect will be expressed.

* Float type TimeColor (x = 5): allows you to adjust the rate of color change of the bubble over time.

* Smoothness of the Float type (x = 1): smoothness of the bubble surface.

  ![image](https://github.com/user-attachments/assets/3abb02cf-c8b1-482c-b3b7-9caf7e4f8fc2)

                                                Image 15. BubbleShader parameters

	First you need to create a Simple Noise node and connect the NoiseScale variable to the input Scale. This controls the size and detail of the noise. Increasing the NoiseScale creates a finer and more detailed noise. Then create a Time node, multiply it by NoiseSpeed using the Multiply node, and add the multiplication result to the UV coordinates of the Simple Noise node to create a noise animation. Shifting UV coordinates over time is a standard technique for animating textures. 

To control the glare intensity, you need to create a Fresnel Effect node. This node calculates the Fresnel effect, which simulates the reflection of light from a surface depending on the viewing angle. A viewing angle close to the tangent gives a stronger reflection. The nodes Normal Vector, View Direction, and the FresnelPower parameter must be connected to the corresponding inputs of the Fresnel Effect node to calculate reflection. The outputs of the Fresnel Effect and Simple Noise nodes are multiplied using the Multiply node to modulate the Fresnel effect noise and create glare on the bubble surface. The output of the Fresnel Effect node is connected to the Alpha Master Node to make the edges of the bubble more transparent and the center more opaque.
To control the rate of color change, a Time node is created, multiplied by the TimeColor parameter, and the result of the multiplication is connected to the Sine node. The Sine node generates a sine wave that smoothly changes from -1 to 1. Then the Sine node will connect to the Remap node (parameters: In Min = -1; In Max = 1; Out Min = 0; Out Max = 1). Remap converts the range of values to use it for color selection. The Remap output is connected to the Sample Gradient, which takes values from 0 to 1 and is used to select a color from a specified range. To simulate the color and emission of a bubble by noise and the Fresnel effect, it is necessary to multiply the output of the Sample Gradient by the product value. Fresnel Effect and Simple Noise using the Multiply node. The Multiply output is connected to the Base Color and Emission inputs of the Master Node (Image 16).

![image](https://github.com/user-attachments/assets/7e74ac6b-7415-44ea-912b-43be3be0be32)

                                                Image 16. Underwater bubble in Shader Graphics

https://github.com/user-attachments/assets/7c2a7781-d595-4c19-8998-679cbeff68c9 

                                Underwater Bubble Particle System      
   

So, a shader that dynamically changes the color of the bubble using a sine wave, noise and Fresnel effect has been created. Now it is necessary to implement a particle system that will create many bubbles in the water. To begin with, a new Particle System object is created and the following parameters are configured:

* Duration (x = 5): the duration of particle emission (in seconds).

* Looping (Mode: On): it is activated so that the particle system repeats.

* Start Lifetime (Random Between Two Constant):  the lifetime of each bubble. It is important to set up a realistic bubble lifetime here, because each bubble should exist long enough to be visible, but also not too long so as not to create the impression of static. The range is set from 1 to 5 seconds.

* Start Speed (Random Between Two Constant): The initial speed of the bubbles. A small upward velocity is set in the range from 0.1 to 0.5. so that the bubbles float to the surface.

* Start Size (Random Between Two Constant):   the initial size of the bubbles. The range is set from 0.1 to 1 to make it more realistic, since the bubbles are not the same size.

* Start Rotation (Random Between Two Constant): Bubble rotation. It is set in the range from -20 to 20. It is not required if the bubble texture has the shape of a sphere.

* Gravity Modifier (x = -1): the force of gravity acting on particles. For bubbles, you need to set a negative value so that they pop up.

* Max Particles (x = 100): the maximum number of particles that can exist at the same time.
Module Emission Rate over Time (x = 25): the number of particles emitted per second. Determines the density of the bubble flow.

* Module Shape: the shape of the area from which the particles are emitted. A cone is used to create a beautiful bubble ceiling.

* The Size over Lifetime module (Separate Axis On): It is activated to change the curve in three planes to create a bubble wobble effect.

* Renderer module: The "Mesh" parameter must be set. Choose a simple sphere as a mesh. Assign the created bubble shader.

By combining two approaches (Shader Graph and Particle System), it turns out to create a convincing simulation of underwater bubbles with dynamically changing colors and realistic behavior. It is also possible to control the rate of color change, the intensity of the Fresnel effect, and adjust the noise and smoothness on the surface of each bubble (Image 17).

![image](https://github.com/user-attachments/assets/2342ad0a-87ff-4964-bf69-2c247e7edd81)

                                Image 17. The finished effect of underwater bubbles using the Particle System

https://github.com/user-attachments/assets/9c0c9120-88c0-4e16-a70d-232d9ccf4c33


                                the school of fish effect      

# Conclusion

As a result of the work, a demonstration pool scene was developed, including a set of visual effects implemented using shaders and particle systems. To enhance the realism of the scene and simulate the aquatic environment, the following effects were created:

1. A caustic shader that creates the effect of refraction of light rays. In materials with this shader, you can adjust the effect color, transparency, offset, and intensity.

2. A comprehensive system for simulating complex wave motion using the Gerstner wave formula, the Fresnel effect, and the Depth Mask method. The shader allows you to adjust the direction of wave propagation, the distance between waves, the height and sharpness of the crest to illustrate different water surfaces.

3. A bubble shader with the ability to adjust transparency, gradient, color change rate, and noise intensity.

4. Underwater bubble effect based on a particle system for dynamically displaying bubbles rising from the bottom of the pool. Using the Particle System allowed us to create a realistic and flexibly customizable effect.

# Literature

1. Ludum Dare – URL: https://ldjam.com/?ref=steemhunt (Date of request - 04/18/2025)

2. Guidelines for using cubic maps – URL: https://docs.unity3d.com/2023.2/Documentation/Manual/class-Cubemap (Date of request - 04/25/2025)

3. Guide to using the normal map – URL: https://docs.unity3d.com/2023.1/Documentation/Manual/StandardShaderMaterialParameterNormalMap (Accessed 04/25/2025)

4. "Gerstner waves and their generalizations in hydrodynamics and geophysics" by A.A.Abrashkin, E.N.Pelinovsky, 2022 (Accessed 04/25/2025)

5. Waveform analysis using fast Fourier transform – URL: https://www.dataq.com/data-acquisition/general-education-tutorials/fft-fast-fourier-transform-waveform-analysis (Accessed 04/25/2025)

6. SSR URL Usage Guide: https://docs.unity.cn/560/Documentation/Manual/PostProcessing-ScreenSpaceReflection (Accessed 04/25/2025)

7. Navier-Stokes Equations – URL: https://en.wikipedia.org/wiki/Navier-Stokes_equations (Accessed 04/25/2025)

8. Post-processing URL Usage Guide: 
https://docs.unity.cn/es/2021.1/Manual/PostProcessingOverview (Accessed 04/25/2025)

9. DOF URL: https://en.m.wikipedia.org/wiki/Depth_of_field (Accessed 04/25/2025)




