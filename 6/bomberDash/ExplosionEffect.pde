// Tie-21106_2016 / TIE21106-2016-G08
// Project assignment: BomberDash
// File: ExplosionEffect.pde
// Description: Explosion effects, inherits VisualEffect
// File modified by: Atte, Vilhelmiina

// Class: ExplosionEffect
class ExplosionEffect extends VisualEffect
{
  int framesPerSprite;
  SpriteSheetReader spriteReader;
  PImage[] sprites;
  
  // ExplosionEffect constructor
  // Parameters: -
  ExplosionEffect()
  {
    framesPerSprite = 1;
    PImage sheet = loadImage("ExplosionSpriteSheet.png");
    spriteReader = new SpriteSheetReader();
    PImage[][] allSprites = spriteReader.getSprites(sheet, 10, 5, 96, 96, 0, 0);
    sprites = new PImage[allSprites.length * allSprites[0].length];
    for (int y = 0; y < allSprites.length; ++y)
    {
        for (int x = 0; x < allSprites[y].length; ++x)
        {
          allSprites[y][x].resize(blockSize, blockSize);
          sprites[(allSprites.length-y-1)*allSprites[y].length + x] = allSprites[y][x];
        }
    }   
  }
  
 // Name: protected Animation createNew
 // Parameters: -
 // Description: creates a new animation
  protected Animation createNew()
  {
    return new Animation(sprites, framesPerSprite);
  }
}