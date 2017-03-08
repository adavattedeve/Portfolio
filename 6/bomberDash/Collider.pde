// Tie-21106_2016 / TIE21106-2016-G08
// Project assignment: BomberDash
// File: Collider.pde
// Description: Class that checks collisions
// File modified by: Atte, Vilhelmiina

// Class: Collider
class Collider
{
  PVector position;
  PVector offset;
  int sizeX;
  int sizeY;
  
  // Collider constructor
  // Parameters:
  // PVector _position: position
  // int _sizeX: size x
  // int _sizeY: size y
  Collider(PVector _position, int _sizeX, int _sizeY)
  {
    position = _position;
    offset = new PVector(0,0);
    sizeX = _sizeX;
    sizeY = _sizeY;
  }
  Collider(PVector _position, PVector _offset, int _sizeX, int _sizeY)
  {
    position = _position;
    offset = _offset;
    sizeX = _sizeX;
    sizeY = _sizeY;
  }
  
  // Name: boolean collides
  // Parameters:
  // Collider other: the other object
  //
  // Description: checks if collides
  //
  boolean collides(Collider other)
  {
    if (other == null)
    {
      return false;
    }
    if (position.x+offset.x < other.position.x + other.offset.x + other.sizeX &&
   position.x + offset.x + sizeX > other.position.x + other.offset.x &&
   position.y+offset.y < other.position.y + other.offset.y + other.sizeY &&
   sizeY + position.y + offset.y > other.position.y + other.offset.y) 
   {
     return true;
   }
  return false;
  }
}