# PerspectiveProjection
PerspectiveProjection is a Unity package intended for camera frustum projection, a Technique that is used in virtual production to turn LED screens into a backdrop where CG content is projected in real-time. It can be used to give depth to flat screens when captured by a real-world camera which position and rotation are tracked in some way (i.e. Vive Tracker) and passed to a virtual camera within a Unity scene.

## Notes
* Tested with Unity 2021 LTS HDRP, in Editor and Play mode.
* Intended for projection onto one single screen (might work with multiple ones with some extra work).
* This solution does not rely on any particular camera tracking method which must be implemented separately.
* It is NOT intented for production work, is an explorative project and it hasn't been thoroughly tested.

## Usage
### Installation
* Import *PerspectiveProjection.unitypackage* into a project.
* Locate the ScreenProjection prefap (PerspectiveProjection > Prefabs) and place it in the scene.

### Position, resolution and scale
The ScreenProjection prefab contains a quad that represents the physical screen position and dimension relative to the virtual world. The visible side of the quad represents the front of the physical screent. Use the transform gizmo for positioning the ScreenProjection prefab where preferred. 
DO NOT try to resize it by rescaling it (the attached script will prevent so anyways), instead do the following to set resolution and scale:

* In the Project folder, locate and select the RenderTexture named PerspectiveProjectionRT (PerspectiveProjection > Textures) and set its size in the Inspector to match the aspect ration of the physical screen (i.e. 1920x1080 for a Full HD 16:9 screen).
* Set the Game window resolution to the same size or aspect ratio (i.e. 1920x1080 or 16:9).
* In the Hierarchy, select SreenProjection and in the Inspector set the Resolution property (Perspective Projection Script) with the same values used for the RenderTexture (i.e. 1920x1080).

By rescaling PerspectiveProjection following the above steps, you will notice that the CaptureCamera position will also "rescale" accordingly.

### Camera Tracking
This package should work with any tracking method (i.e. Vive Tracker) that allows to mirror the position of a real-world camera to that of the CaptureCamera gameobject in the scene. 
Important note:
* Do not move CaptureCamera outside of ScreenProjection, instead add the necessary components for traking real-world camera position to it. This ensures that the scale of tracked movements match that of ScreenProjection.
* If for any reason CaptureCamera must be replaced by another camera gameobject, this must be a child of ScreenProjection and its Target Texture property must be set to the PerspectiveProjectionRT RenderTexture (PerspectiveProjection > Textures).

Then set its real-world size and scene scale as follows:

* With ScreenProjection still selected, change the Screen Height property to match the physical screen real-world height in meters (i.e. 32cm = 0.32).
* If you need to change the scale of ScreenProjection relative to the virtual world, set the Scale Factor property value to a different one. This is useful if your scene content is not following 1 unit = 1 meter or if you simply want the scene to be projected smaller or bigger onto the physcal screen.

### Projecting on a physical screen

* Set the Game window at full screen on the target physcal display. To remove the top bar there's a handy tool available on the Unity Asset Store called "Fullscreen Editor". Note that this is a paid third party tool so use at your own risk.
* The DisplayCamera gameobject must not be moved from within ScreenProjection and its Target Display property must be set to that of the Display being used in the full screen Game window (Display 1 by default).

## Aknowledgements
The PerspectiveProjection.cs script contains a snippet from https://rosettacode.org/wiki/Gaussian_elimination available under Creative Commons Attribution-ShareAlike 4.0 International (CC BY-SA 4.0, https://creativecommons.org/licenses/by-sa/4.0/).

## License
License: Attribution-ShareAlike 4.0 International (CC BY-SA 4.0) https://creativecommons.org/licenses/by-sa/4.0/
