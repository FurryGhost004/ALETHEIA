Thank you for downloading PBR Apartment.
If you have any questions you can contact me at : kwinty75@gmail.com 

All the demos are located in the "Demo" Folder.
All the new stuff are located in the "Models Material and prefabs from update 2.0" Folder. 



In PBR Version, import Unity Post Processing 2 if you want to use unity post process. It's in unity package manager. Window -> Package Manager -> Post Processing

----------------------------------------

HDRP : 
BEFORE YOU START:
- you need Unity  2019.4 or higher
- you need HDRP pipline 7.3.1 or higher.


Import the "HDRP Apartment" Package
Open one of the Demo Scene

Find the "Sky and fog Volume" Gameobject and make sure the "Sky and Fog Settings Profile 1" is assigned (If not it is located in the HDRP settings folder)
Find the "Post Effect Volume" Gameobject and make sure the "Post effect volume Profile" is assigned (If not it is located in the HDRP settings folder)
Enable the screen space reflection for a better rendering.

Depending on the version you are using, you will probably need to bake the lightmaps (Window>Rendrering>Lightning Settings)

Some artifacts may appears, depending on your version : To correct them, it will very often be a question of playing with the value of the height map and the normal map of the material (lower the value of the height map to 0 and increase the value of the normal map)

----------------------------------------

URP:
BEFORE YOU START:
- you need Unity  2019.4 or higher
- you need URP pipline 7.3.1 or higher.


Import the "URP Apartment" Package
Open one of the Demo Scene

Find the "Post Effect Volume" Gameobject and make sure the "Post effect volume Profile" is assigned (If not it is located in the URP settings folder)
For a better rendering, enable the Ambient Occlusion. At the time I'm writting this, the Screen Space Reflection is not yet compatible with URP.

Depending on the version you are using, you will probably need to bake the lightmaps (Window>Rendrering>Lightning Settings)

Some artifacts may appears, depending on your version : To correct them, it will very often be a question of playing with the value of the height map and the normal map of the material (lower the value of the height map to 0 and increase the value of the normal map)



