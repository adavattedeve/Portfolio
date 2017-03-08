// Tie-21106_2016 / TIE21106-2016-G08
// Project assignment: BomberDash
// File: SpriteSheetReader.pde
// Description: reads the animation sprite sheet
// File modified by: Francois, Vilhelmiina, Usman, Atte

// Class: SpriteSheetReader
class SpriteSheetReader
{
  
  // SpriteSheetReader constructor
  // Parameters: -
  SpriteSheetReader()
  {
    
  }
  
  // Name: PImage[][] getSprites
  //
  // Parameters:
  // PImage spriteSheet: (description)
  // int _x: number of x sprites in sprite sheet
  // int _y: number of y sprites in sprite sheet
  // int spriteWidth:  actual width of the sprite in the sheet
  // int spriteHeight:  actual height of the sprite in the sheet
  // int offsetX:  offsets from sprite's edge to the actual sprite from left(x)
  // int offsetY:  offsets from sprite's edge to the actual sprite from up(y)
  //
  // Description:
  // params _x and _y are for number of sprites in sprite sheet
  // params spriteHeight and spriteWidth are the actual sizes of the sprite in the sheet
  // params offsetX and offsetY are offsets from sprite's edge to the actual sprite from up(y) and left(x)
  //
  // Returns 2d array which consist all sprites from the sheet ordered from left to right, bottom to up. [0][0] element is bottom left sprite
  PImage[][] getSprites(PImage spriteSheet, int _x, int _y, int spriteWidth, int spriteHeight , int offsetX, int offsetY)
  {
    PImage[][] sprites = new PImage[_y][_x];
    float spriteSizeX = (float)spriteSheet.width/_x;

    float spriteSizeY = (float)spriteSheet.height/_y;

    for (int y = 0; y < sprites.length; ++y)
    {
        for (int x = 0; x < sprites[y].length; ++x)
        {
          sprites[y][x] = spriteSheet.get(offsetX + (int)(spriteSizeX*x), offsetY + (int)(spriteSizeY*(_y-y-1)), spriteWidth, spriteHeight);

        }
    }   
  return sprites;
  }
}