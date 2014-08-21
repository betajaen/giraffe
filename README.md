[giraffe](https://github.com/betajaen/giraffe) - Minimal 2D drawing framework for Unity
=======================================================================================

What on Earth?
--------------

Giraffe is a framework/addon/library/tool to allow you to draw 2D sprites directly onto the screen. It's very simple to use, small in size and very efficient.

But Unity already does 2D!?
---------------------------

Yes it does, and there are plenty additional plugins for it as well, and they all do a great job. Giraffe does 2D differently, it doesn't use where 1 sprite/piece o'text/button is a GameObject/MeshRenderer/MeshFilter arrangment. It uses at the very minimum two GameObjects, if you want a single sprite on the screen then that's two GameObjects, want a million sprites on the screen - the same two GameObjects.

But How!?
---------

Giraffe uses a technique of building the 2D sprites into a single or multiple meshes, those meshes are rendered in a post Camera rendering process. Anything rendered in Giraffe will be rendered after all 3D elements, including Unity sprites, or 2D-GUIs (NGUI, DF-GUI, etc.) but underneath the classic Unity GUI.  It's very efficient in the sense it's a single mesh per layer, single pass of a material and everything is carefully updated when and if it's necessary.

More how, what do I do?
-----------------------

Essentially, Giraffe is organised into layers. A GiraffeLayer MonoBehaviour, it is attached to a GameObject which is then a child GameObject of the Giraffe MonoBehaviour. A Layer uses a single texture atlas which contains your sprites. You can think of it as; A bunch of sprites in an atlas, one atlas per layer, many instances of those sprites per layer.

An Atlas can be created from the Assets menu, and it's inspector. You can import any Textures and Tilesets as you like, into the image. Press the build button and it's done.

Putting sprites on the screen is easy, it's all done in Code. No editor, everything is referenced in pixels and you just build a batch of sprites via Begin and End.

    layer.Begin(2);
    layer.Add(100, 200, "Sprite1");
    layer.Add(205, 210, "Sprite2");
    layer.End();
  
Those sprites will stay on the screen continously, until you do another Begin/End. Have a look at the Examples for a more clearer understanding.


Alright, what is it licenced under. Can I use it in my millon dollar game?
--------------------------------------------------------------------------

Giraffe Copyright (c) 2014 Robin Southern

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Future
------

I consider the core part of Giraffe complete, additional features should be added in as forms of examples with reusable components, but I would like to add - in no particular order or commitment;

* Text support 
* BMFont font importer
* More sprite examples; animation, 2d physics with sprites, etc.
* Non-grid based tileset importer
* Unity sprite atlas importer
* Add primitive shape drawing; lines, circles, etc.
* Some support for non-atlases - just a texture; RenderTextures, Animation, etc.

Any examples, or screenshots I can see?
---------------------------------------

## [1 - Rectangles](https://github.com/betajaen/giraffe/tree/master/Examples/1-Rectangles)

![Rectangles](https://raw.githubusercontent.com/betajaen/giraffe/master/Examples/1-Rectangles/Example.png)


Rectangles just shows a random number of rectangles on the screen, demonstrating:

* An Atlas 'Rectangles'.  Which was created using the Box, Box1, Box2, Box3 and Box4 textures.
* Setting up the Giraffe Singleton and a Layer, using the Rectangles Atlas.
* Creation of many randomised boxes on the screen using Giraffe Layer.Begin/GiraffeLayer.End.

## [2 - Tilesets](https://github.com/betajaen/giraffe/tree/master/Examples/2-Tilesets)

![Tilesets](https://raw.githubusercontent.com/betajaen/giraffe/master/Examples/2-Tilesets/Example.png)

Tilesets shows of the Grid based Tileset importer for the Atlas, and showing that tileset as from a pre-made map, it , demonstrates:

* Importing a grid based tileset into an Atlas
* Turning off the white texture, border and padding options of the Atlas
* Loading a map created by Tiled, via a simple importer
* Showing that map on the screen


Artwork
-------

Some of the artwork was created by Hyptosis:

http://www.newgrounds.com/art/view/hyptosis/tile-art-batch-1
