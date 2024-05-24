# Documentation

## Author
Adam Balko

-----
You should make a copy of the rest of this template for
each checkpoint. You can remove irrelevant parts for the
clarity of the document.

## Checkpoint 2 (tag `Chk 2`)
The point of this checkpoint was to create an image using Raycasting, by defining the behaviour of solids, materials, and light.

### Command line arguments
`-w | --width` - overwrites ResolutionWidth of the scene's camera  
`-h | --height` - overwrites ResolutionHeight of the scene's camera    
`-s | --samples` - overwrites SamplesPerPixel of the scene's camera  
`-o | --output` - path to the output image -- (Default: "./Outputs/demo.pfm")  
`-c | --config` - path to the configuration json file -- (Default: "./Properties/config.json")  
`-t | --trace` - Trace scene's JSON deserialization


### Configuration arguments

`Author` - Author of the configuration file. It is printed to stdout  
`Scene` - Specifies properties of the scene's Camera, Lightsources and Solids  
`Materials[]` - List of possible materials assigned to the solids

#### Scene
`BackgroundColor` - Color returned by a ray that didn't hit any solid.  
`AmbientColor` - Color of the solid's ambient component  
  
`Camera` - Specifies ray generation
* `Position` - World coordinates of the camera
* `Forward` - Direction of the Camera's Z Axis
* `Up` - Vector perpendicular to `Forward`, direction of the Camera's Y Axis
* `ScreenDistance` - Nearest side of the camera's frustum (Doesn't work yet)
* `ViewingDistance` - Farthest side of the camera's frustum (Doesn't work yet)
* `ResolutionWidth` - Number of pixels generated along the Camera's X Axis
* `ResolutionHeight` - Number of pixels generated along the Camera's Y Axis
* `Fov` - Field of view in degrees
* `SamplesPerPixel` - Number of Rays generated for 1 pixel
  
`LightSources[]` - List of the scene's light sources  
* `Position` - World coordinates of the source
* `EmittingColor` - Color of the specular reflection
* `Ìntensity` - Affects the brightness of colors reflected from illuminated Solids
  
`Solids` - Objects occupying the scene's space
* `Position` - World coordinates of the solid
* `MaterialTag` - Name of material used for the solid
* `Tag` - Geometrical object the solid represents. 
  
Rest of the solid's properties depend on the object used

#### Available types of Solids
Tag: `SPHERE`
* `Radius` - Radius of the sphere
  
Tag: `RECTANGLE`
* `Normal` - Normal vector of the rectangle's plane
* `WidthAxis` - Vector along which the Width of the rectangle is defined. Is perpendicular to `Normal`
* `Width` - Units occupied by the rectangle along the WidthAxis.
* `Height` - Units occupied by the rectangle perpinducalr to the WidthAxis

#### Materials
`Name` - Name of the material  
`DiffuseColor` - Base color of the illuminated surface  
`kDiffuse` - Diffuse color contribution  
`kSpecular` - Light source's color contribution  
`kAmbient` - Ambient color contribution  
`Shininess` - Concentration of the specular reflection  
`RefractiveIndex` - Index of refraction

### Algorithm

#### 1. Configuration
I used JSON.NET from Newtonsoft in combination with JsonSubTypes to read from the config json file and determine what kind of polymorphic object to create. That did most of the work for me, but lead to uncomfortable decisions. I couldn't make Solid an abstract class, since the packages need to instantiate it. I also didn't use normal constructors, but Initialize methods, to avoid unpredictable behavior.

#### 2. Ray generation
First, the screen and pixel dimensions in the world space are calculated from the Camera's Fov and resolution. Then for every pixel, SamplesPerPixel amount of rays are generated, such that their origin is in the position of the camera, and their directive points towards an area occupied by the pixel. The precise coordinates of the directive are randomly generated.   

Then the ray is rotated. It's transformed by a matrix consisting the Camera's Up, Forward and Right orthogonal base vectors. Right is calculated as a cross product of Up and Forward. In the end, all rays corresponding to a pixel are bundled into a RayBatch struct.

#### 3. Intersections
All solids must implement GetRayIntersection method, which returns an Intersection struct, containg information of the intersection's position, intersecting ray and solid, and other useful properties. The position itself is calculated by finding the distance paramater t, and substituing it into the parametric definition of the ray.

Finding the parameter differs object by object. For rectangles, the incoming ray is compared to its normal vector. Then the UV coordinates, relative distance from the rectangle's corner, are calculated, and only if they are within the bounds of the rectangle, an intersection is reported. For spheres, the definition of the ray is substituted into the definition of the sphere, giving a quadratic equation.

#### 4. Colors
An intersection is then processed by the Phong's shading model. Material is used to determine its diffuse, specular and ambient components. The diffuse color is defined in the material, specular color in the reflected light source, and ambient color is uniform for the entire scene. In the end, all colors given by the rays of a RayBatch are averaged out, and put on the screen as a single pixel.

### Use of AI
I used GitHub Copilot for occasional slave work


## Checkpoint 3 (tag `Chk 3`)
The point of this checkpoint was to implement Raytracing and casting shadows. The code went through a major refactorization. I also attempted implementing the Microfacet model but it doesn't work correctly yet.

### New configuration arguments
`Renderer` - Rendering model used for the image creation  
* `Tag` - Name of the model

`Brdf` - Model used for biderectional reflectence function  
* `Tag` - Name of the model

#### Available renderers
Tag: `RAYCAST` - model used in Checkpoint 2  
Tag: `RAYTRACE` - model based on recursive following of reflected rays, and their contribution based on the Reflectance function
* `MaxDepth` - Maximum depth of the recursion

#### Available BRDFs
Tag: `PHONG` - Phong's model  
Tag: `MICROFACET` - Microfacet model introduced by Cook and Torrance. Functions used by the model must be specified
* `FresnelFunction` - available are `Schlicks`
* `DistributionFunction` - available are `BlinnPhong`, `TrowbridgeReitz`, `GGX`
* `GeometryFunction` - available are `CookTorrance`

### Changed configuration arguments
#### Light sources
Light sources were refactored to support more types. Their common arguments are:  
  
`Tag` - type of the light object  
`EmittedColor` - Color of the specular reflection  
`Intensity` - Affects the brightness of colors reflected from illuminated Solids

The types used in the program are `PointLight` and `RayTracedLight`. The latter is generated within the RAYTRACE model, do it cannot be defined in the scene.  
  
Tag: `POINT` - All light is emitted equally into all directions originating in a signular point
* `Position` - world coordinates of the light's origin

### Algorithms

#### 5. Raytracing  

The ray generation, and usage of RayBatches remains unchanged. Finding intersections as well. At each intersection, diffuse and specular light is contributed from each light source. Then another ray is generated, in the direction of perfect reflection from the solid's surface. This direction is used to calculate reflectance at the point, and to calculate intersection with another solid in that direction. This process is repeated recursively. At the base of recursion a RayTracedLight source is returned and muliplied by the reflectance.

### Use of AI

Still the same, but Copilot is getting more and more unreliable.

## Checkpoint 4 (tag `Chk 4`)

### New Configuration arguments

There's a new argument for `Camera`:

`Sampler` determines how points used for Ray directions are generated within a space of 1 pixel
- `Tag` - Name of the sampler

There's also a new argument for Materials.

`KTrasparent` - How much light is contributed from refracted rays in the Phong model.

#### Available samplers

`UNIFORM` - points are spaced evenly in rows and columns. SPP is rounded down to a square.  
`RANDOM` - sampler used until now. All points are generated randomly.
`JITTERING` - combination of `UNIFORM` and `JITTERING`, the pixel is divided into even squares, 1 point is chosen randomly within the square  
`HAMMERSLEY` - x coordinate is given by n/N and y coordinate is given by the Halton sequence at base 2

### Changed configuration arguments

In lights `EmittedColor` changed to `BaseColor`, since it got confusing.  
In `MICROFACET` Brdf the available Geometry functions changed to `Beckmann` for use with `BlinnPhong` distribution, and `GGX` for use with `GGX` and `TrowbridgeReitz` ditribution functions.

### Algorithms

#### 6. Samplers

`UNIFORM` and `RANDOM` are trivial to implement. `JITTERING` is really just combination of the two. I zipped the result of both. And to each uniform point I added a random point scaled by the width of each square. For the `HAMMERSLEY` sampler I followed [this chapter](https://www.pbr-book.org/3ed-2018/Sampling_and_Reconstruction/The_Halton_Sampler) of the book Physically Based Rendering.

#### 7. Refractions

At each point, after reflections are contributed, refractions are contributed in the same manner. Direction of the refraction is given by Snell's law. Intensity of the light is implemented in each Brdf differently. In `PHONG` its just multiplied by KTransparent. In `MICROFACET` I tried using Btdf equation 8.19 stated [here](https://www.pbr-book.org/3ed-2018/Reflection_Models/Microfacet_Models#eq:microfacet-g1) and equation 21 stated [here](https://www.cs.cornell.edu/~srm/publications/EGSR07-btdf.pdf), but the resulting transmittance values are so small, the effect is negligable.

At this point we expected that there are no overlapping solids in the scene. Because of that, each Ray remembers in which solid is currently encapsulated. If it's Void (outside any defined solid), the interactions are same as they used to. If it's not Void, in the next intersection, the surface normal is inverted. Upon reflection the solid stays the same, upon transmission the next solid is automatically the Void.