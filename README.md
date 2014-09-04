[giraffe](https://github.com/betajaen/giraffe) - Minimal 2D drawing framework for Unity
=======================================================================================

What on Earth?
--------------

Giraffe is a framework/addon/library/tool to allow you to draw 2D sprites directly onto the screen. It's very simple to use, small in size and very efficient.

But Unity already does 2D!?
---------------------------

Yes it does, and there are plenty additional plugins for it as well, and they all do a great job. 

These types of systems use the traditional Unity approach where 1 sprite, piece of text or button tends to be a complicated GameObject with multiple mono-behaviours approach where it's all created in the Editor. There is nothing wrong with it, but it is limiting in some aspects.

Giraffe does 2D differently, the very minimum it will use two GameObjects with a MonoBehaviour each. Those two GameObjects can show 1 sprite, or a million sprites - it makes no difference to Giraffe. Giraffe prefers to draw sprites in code, it’s more flexible and there is much less overhead.

But How!?
---------

Giraffe uses a technique of building the 2D sprites into a single or multiple meshes, those meshes are rendered in a post Camera rendering process.

Anything rendered in Giraffe will be rendered after all 3D elements, including Unity sprites, or 2D-GUIs (NGUI, DF-GUI, etc.) but underneath the classic Unity GUI, you also won't also see any meshes in the Editor either.

It's very efficient in the sense it's a single mesh per layer, single pass of a material and everything is carefully updated when and if it's necessary to do so.

What do I do?
-----------------------

Essentially, Giraffe is organised into layers, sprites are drawn in a layer. You can have as many sprites in a layer as you like (within reason of memory and Unity), a mesh is assigned per layer. A Texture Atlas is used to store the sprites into a single image, it's more efficient this way - A layer will use one Atlas.

In the Editor, there will be a root GameObject containing the Giraffe MonoBehaviour, this is the main Giraffe singleton MonoBehaviour responsible for drawing. For each layer, the Giraffe GameObject will have a sub-GameObject with a GiraffeLayer MonoBehaviour attached to it. 

The GiraffeLayer MonoBehaviour has only a few options, the atlas to use and the order in drawing on the screen.  You can attach your own custom MonoBehaviours to this Layer (or do things differently if you prefer) to access it's three main functions; Begin, Add and End.

When you want to show things on the screen, you tell the layer how many sprites you want drawn, what they are and where they go. The Layer will then draw them on the screen each frame, until it's told not to, or draw them differently. There is no editor, just code:

    layer.Begin(2);
    layer.Add(100, 200, "Sprite1");
    layer.Add(205, 210, "Sprite2");
    layer.End();
    
Just call Begin/Add/End as many times or combinations as you need to.
  

Alright, what is it licenced under. Can I use it in my million dollar game?
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

* ~~Text support~~
* BMFont font importer
* More sprite examples; animation, 2d physics with sprites, etc.
* ~~Rotation~~ and flipping effects
* A primitive GUI
* Non-grid based tileset importer
* Unity sprite atlas importer
* Add primitive shape drawing; lines, circles, etc.
* Some support for non-atlases - just a texture; RenderTextures, Animation, etc.

Any examples, or screenshots I can see?
---------------------------------------

Yes, there are plenty of [examples and sample components](https://github.com/betajaen/giraffe-examples) for integration into Unity, including:

## [1 - Rectangles](https://github.com/betajaen/giraffe-examples/tree/master/1-Rectangles)

![Rectangles](https://raw.githubusercontent.com/betajaen/giraffe-examples/master//1-Rectangles/Example.png)


Rectangles just shows a random number of rectangles on the screen, demonstrating:

* An Atlas 'Rectangles'.  Which was created using the Box, Box1, Box2, Box3 and Box4 textures.
* Setting up the Giraffe Singleton and a Layer, using the Rectangles Atlas.
* Creation of many randomised boxes on the screen using Giraffe Layer.Begin/GiraffeLayer.End.

## [2 - Tilesets](https://github.com/betajaen/giraffe-examples/tree/master/2-Tilesets)

![Tilesets](https://raw.githubusercontent.com/betajaen/giraffe-examples/master//2-Tilesets/Example.png)

Tilesets shows of the Grid based Tileset importer for the Atlas, and showing that tileset as from a pre-made map, it , demonstrates:

* Importing a grid based tileset into an Atlas
* Turning off the white texture, border and padding options of the Atlas
* Loading a map created by Tiled, via a simple importer
* Showing that map on the screen

## [3 - Updates and Transforms](https://github.com/betajaen/giraffe-examples/tree/master/3-UpdatesAndTransforms)

![Updates and Transforms](https://raw.githubusercontent.com/betajaen/giraffe-examples/master//3-UpdatesAndTransforms/Example.png)

This is a demonstration  of updating the layer each frame, and rotating and scaling the sprite using the Matrix2D struct, it shows:

* Frame-by-frame updating
* Using the TRS (Translate-Rotate-Scale) function of Matrix2D to generate a matrix for a rotatable sprite.

## [4 - Text Rendering](https://github.com/betajaen/giraffe-examples/tree/master/4-TextRendering)

![Text Rendering](https://raw.githubusercontent.com/betajaen/giraffe-examples/master//4-TextRendering/Example.png)

This is a demonstration of rendering text, by using the optional GorillaFont component. In this example, it uses a bitmap font to draw text on the screen. In particular it demonstrates;

* GiraffeFont usage
* Getting quad estimation of text, then drawing it to a layer
* Switching colours
* Using the 'Giraffe/White' sprite for drawing solids of colour.
* Frame-by-frame updating


Artwork
-------

Some of the artwork was created by Hyptosis:

http://www.newgrounds.com/art/view/hyptosis/tile-art-batch-1


Last Question - Why is it called Giraffe?
-----------------------------------------

Why not? It's also pronounced 'PEN-GWIN'.
