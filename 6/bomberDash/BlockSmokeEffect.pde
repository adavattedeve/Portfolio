// Tie-21106_2016 / TIE21106-2016-G08
// Project assignment: BomberDash
// File: BlockSmokeEffect.pde
// Description: Visual block smoke effect
// File modified by: Francois, Vilhelmiina, Atte

// Class: BlockSmokeEffect
class BlockSmokeEffect extends VisualEffect
{
  int framesPerSprite;
  SpriteSheetReader spriteReader;
  PImage[] sprites;
  
  // BlockSmokeEffect constructor
  // Parameters: -
  BlockSmokeEffect()
  {
    framesPerSprite = 1;
    PImage sheet = loadImage("SmokeSpriteSheet.png");
    spriteReader = new SpriteSheetReader();
    PImage[][] allSprites = spriteReader.getSprites(sheet ,20, 1, 96, 94,  0, 0);
    sprites = new PImage[10];
    int i=0;
    for (int y = 0; y < allSprites.length; ++y)
    {
        for (int x = 0; x < 10; ++x)
        {
          sprites[i] = allSprites[y][x];
          sprites[i].resize(blockSize, blockSize);
          ++i;
        }
    }   
  }
  
  // Name:  protected Animation createNew
  // Parameters: -
  // Description: smoke animation
  protected Animation createNew()
  {
    return new Animation(sprites, framesPerSprite);
  }
}