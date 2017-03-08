// Tie-21106_2016 / TIE21106-2016-G08
// Project assignment: BomberDash
// File: Object.pde
// Description: this class stand for the collectibles object the player can loot.
// File modified by: Francois Mazeau, Vilhelmiina

// Class: Object
class Object
{
  String name;
  PImage image;
  int id; //unique identifier, like for blocks types
  
  
  // Object constructor
  // Parameters: -
  Object()
  {
  }
  
  // Name: void init
  // Parameters: -
  // Description: Initializes an object by setting the image
  //
  void init()
  {
    image = loadImage(name + ".png");
    image.resize(blockSize, blockSize);
  }
  
  // Name: void display
  // Parameters: 
  // int x: x-coordinate
  // int y: y-coordinate
  //
  // Description: Displays an object.
  // int x and int y give the coordinates.
  void display(int x, int y)
  {
    image(image, x - cameraPosition.x, y - cameraPosition.y);
  }
}