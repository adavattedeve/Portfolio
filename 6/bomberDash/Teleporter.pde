// Tie-21106_2016 / TIE21106-2016-G08
// Project assignment: BomberDash
// File: Teleporter.pde
// Description: Teleporter class
// File modified by: Vilhelmiina, Atte, Francois, Usman



class Teleporter
{
  PImage image, image_2; // image_2 used to refresh the sprite to have some 'animation'
  PVector coords;
  int size;
  Collider collider;
  
  Teleporter()
  {
    size = blockSize;
    
    image = loadImage("teleporter.png");
    image_2 = loadImage("teleporter2.png");
    image.resize(size, size);
    image_2.resize(size, size);
  }
  
  void update()
  {
    if (coords == null)
    {
      coords = map.getRandomEmptyLocation(3);
      collider = new Collider(coords, size, size);
    }
    display();
    checkCoords();
  }
  
  void display()
  {
    if ((frameCount % 60) < 30)
      image(image, (int)(coords.x-cameraPosition.x), (int)(coords.y-cameraPosition.y));
    else
      image(image_2, (int)(coords.x-cameraPosition.x), (int)(coords.y-cameraPosition.y));
  }
  
  void checkCoords()
  {
    float pX = player.position.x;
    float pY = player.position.y;

      if (collider.collides(player.collider))
        teleportPlayer();
  }
  
  void teleportPlayer()
  {
    PVector newPos = map.getRandomEmptyLocation(5);
    player.position.x = newPos.x;
    player.position.y = newPos.y;
  }
}