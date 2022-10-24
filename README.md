# PerspectiveProjection
PerspectiveProjection is a Unity package intended for camera frustum projection, a Technique that is used in virtual production to turn LED screens into a backdrop where CG content is projected in real-time. It can be used to give depth to flat screens when captured by a real-world camera which position and rotation are tracked in some way (i.e. Vive Tracker) and passed to a virtual camera within a Unity scene.

The preview below shows a possible result seen from the point-of-view of the recording camera (top half) and from an arbitrary point-of-view to illustrate the overall setup (bottom half) - LED Screen as backdrop to physical "actors" and a tracked camera (in this istance, an HTC Vive Tracker attached to a phone).

![PerspectiveProjection_001](https://user-images.githubusercontent.com/1048085/197563133-0d2cea24-2b22-42b5-a9be-5f43c842c3c1.gif)

## Notes
* Tested with Unity 2021 LTS HDRP, in Editor and Play mode.
* Intended for projection onto one single screen (might work with multiple ones with some extra work).
* This solution does not rely on any particular camera tracking method which must be implemented separately.
* It is NOT intented for production work, is an explorative project and it hasn't been thoroughly tested.

## Setup
### Installation
* Create a new layer called "PerspectiveProjection".
<img width="161" alt="PerspectiveProjection_layer" src="https://user-images.githubusercontent.com/1048085/197573318-261b5d31-f065-4acc-877d-4e5a00cdf9e1.png">

* Import *PerspectiveProjection.unitypackage* into a project.
* Locate and select the ScreenProjection and CameraOrigin prefaps (PerspectiveProjection > Prefabs) and place both of them in the scene.
<img width="340" alt="PerspectiveProjection_prefabs" src="https://user-images.githubusercontent.com/1048085/197573766-0d000705-fa48-4415-931f-79c8a42e0a7a.png">

### Checklist
* The hierarchy of the gameobjects should look as follows.
<img width="143" alt="PerspectiveProjection_Hierarchy" src="https://user-images.githubusercontent.com/1048085/197589481-2b81410d-1da2-4444-9b46-86f7e377f45d.png">

* Layer should be set to PerspectiveProjection for all gameobjects with the exception of Debug. Debug should be deactivated unless you need to see the virtual representation of the physical screen (ScreenProjection) in the Game window for debug purposes.
* One material should be assigned to ScreenProjection (ScreenProjectionMat).
* One script should also be assigned to ScreenProjection linking properties to the CameraOrigin, TrackedCamera and Display Camera gameobjects as follows.

<img width="330" alt="PerspectiveProjection_Inspector" src="https://user-images.githubusercontent.com/1048085/197590797-1f0da1f0-d450-4ba2-a709-d28bd65e27ac.png">

### Scale and Resolution
* The ScreenProjection prefab contains a quad that represents the physical screen position and dimension relative to the virtual world. The visible side of the quad represents the front of the physical screent. Use the transform gizmo for positioning the ScreenProjection prefab where preferred.
<img width="614" alt="PerspectiveProjection_screenprojection" src="https://user-images.githubusercontent.com/1048085/197585630-f73ee71b-ac82-49a8-a698-57f5968d4b3a.png">

**Note:** DO NOT try to resize it by rescaling it (the attached script will prevent so anyways), instead do the following to set resolution and scale:

* In the Project folder, locate and select the RenderTexture named PerspectiveProjectionRT (PerspectiveProjection > Textures) and set its size in the Inspector to match the aspect ration of the physical screen (i.e. 1920x1080 for a Full HD 16:9 screen).
<img width="275" alt="PerspectiveProjection_rt" src="https://user-images.githubusercontent.com/1048085/197574512-de778426-09de-44ca-92e2-044b50edc6e3.png">

* Set the Game window resolution to the same size or aspect ratio (i.e. 1920x1080 or 16:9).
<img width="256" alt="PerspectiveProjection_game_resolution" src="https://user-images.githubusercontent.com/1048085/197574544-757fbf2f-f907-4fcc-b0ff-c5c22f81d531.png">

* In the Hierarchy, select ScreenProjection and in the Inspector set the Resolution property (Perspective Projection Script) with the same values used for the RenderTexture (i.e. 1920x1080).
<img width="270" alt="PerspectiveProjection_pp_resolution" src="https://user-images.githubusercontent.com/1048085/197580878-e536c5ac-efca-4d6b-92ee-32a80b0bb807.png">

Then, set screen real-world size and scene scale as follows:

* With ScreenProjection still selected, change the Screen Height property to match the physical screen real-world height in meters (i.e. 32cm = 0.32).
<img width="270" alt="PerspectiveProjection_pp_height" src="https://user-images.githubusercontent.com/1048085/197580918-0bff3b2d-790a-49f0-845f-b121737856be.png">

* If you need to change the scale of ScreenProjection relative to the virtual world, set the Scale Factor property value to a different one. This is useful if your scene content is not following 1 unit = 1 meter or if you simply want the scene to be projected smaller or bigger onto the physcal screen.
<img width="270" alt="PerspectiveProjection_pp_scale" src="https://user-images.githubusercontent.com/1048085/197580943-139fbcf8-f7c6-4918-b369-18d5a8affe72.png">

**Note:** By rescaling PerspectiveProjection following the above steps, you will notice that CameraOrigin scale values will also change accordingly. This ensures that the tracked camera movements in the real-world are properly scaled in the Unity scene.

### Camera Tracking and Positioning
This package should work with any tracking method (i.e. Vive Tracker) that allows to mirror the position of a real-world camera to that of the TrackedCamera gameobject in the scene.

* To position the camera in relation to the screen (ScreenProjection) move and rotate CameraOrigin as needed.
<img width="442" alt="PerspectiveProjection_cameraorigin" src="https://user-images.githubusercontent.com/1048085/197582202-5a9cffdc-3ce3-45e9-a2c9-77cd355c03cb.png">

* Add the necessary component for tracking the real-world position of the camera to the Tracker gameobject (i.e. SteamVR Tracked Object). The position and rotation of Tracker should be zero since it will be overwritten by the added component anyways.
<img width="413" alt="PerspectiveProjection_tracker" src="https://user-images.githubusercontent.com/1048085/197582821-2c70fab8-ff48-4ae1-ab67-d0ed4614ef89.png">

* It is unlikely that the position of the tracker device (i.e. Vive Tracker) and the camera's lens are exactly aligned. Apply necessary offset values to the TrackedCamera gameobject.
<img width="412" alt="PerspectiveProjection_trackedcamera" src="https://user-images.githubusercontent.com/1048085/197583414-e70e20fb-67a0-4e6a-8d39-2f0cff4a4756.png">

### Camera FOV
* Select TrackedCamera and change its Field of View property to match that of the physical camera.
<img width="260" alt="PerspectiveProjection_trackedcamera_fov" src="https://user-images.githubusercontent.com/1048085/197592049-8932f3f4-80eb-4d1f-9f2d-8f6db8b73849.png">

* If you don't know the FOV of your camera you can approximate a value and fine tune it later when projecting on the screen.

**Important Notes:**
* DO NOT try to manually position the camera by moving TrackedCamera, this should only be use for offset values between the physical tracker and the camera lens. ALWAYS use the parent gameobject CameraOrigin for manual positioning.
* DO NOT move TrackedCamera outside of Tracker or the latter outside of CameraOrigin. This ensures that the tracked camera movements are scaled and offset correctly.
* If for any reason TrackedCamera must be replaced by another camera gameobject, this must be made a child of Tracker. Also its Target Texture property must be set to the PerspectiveProjectionRT RenderTexture (PerspectiveProjection > Textures).

### Projecting on a physical screen

* Set the Game window at full screen on the target physcal display. To remove the top bar there's a handy tool available on the Unity Asset Store called "Fullscreen Editor". Note that this is a paid third party tool - use it at your own risk.
* Once the Tracked gameobject starts getting real-world position and rotation from the tracker device (i.e. Vive Tracker) you might need to offset its position in the editor so that they are "aligned". Do so by modifying the position and rotation of CameraOrigin.
* It's important to correctly match the relative distance between the camera and the screen between real-world and the Unity scene.

<img width="1387" alt="PerspectiveProjection" src="https://user-images.githubusercontent.com/1048085/197571453-4be71347-4497-4362-bdc8-a0969d0870b5.png">

## Aknowledgements
The PerspectiveProjection.cs script contains a snippet from https://rosettacode.org/wiki/Gaussian_elimination available under Creative Commons Attribution-ShareAlike 4.0 International (CC BY-SA 4.0, https://creativecommons.org/licenses/by-sa/4.0/).

## License
[Unlicense license](https://unlicense.org/)
