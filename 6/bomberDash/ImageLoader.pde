// Tie-21106_2016 / TIE21106-2016-G08
// Project assignment: BomberDash
// File: ImageLoader.pde
// Description: ImageLoader class
// File modified by: Vilhelmiina, Atte, Francois, Usman

public class ImageLoader
{
  //staticSprites
  public PImage heavySprite;
  public PImage heavyBloodSprite;

  public PImage rockSprite;
  public PImage rockDamagedSprite;
  public PImage pointsSprite;
  public PImage indestructibleSprite;

  public PImage bgSprite;

  public PImage rockDestroyedSprite;
  public PImage explosionMarkSprite;
  private PImage[] mudSprites;
  private PImage[] rockSprites;
  private PImage[] rockDamagedSprites;
  private PImage[] pointSprites;

  //animationSprites
  public PImage[] bombAnimationSprites;

  // ImageLoader constructor
  // Parameters: -
  ImageLoader()
  {
    SpriteSheetReader reader = new SpriteSheetReader();
    PImage[][] sprites = reader.getSprites(loadImage("StaticSprites.png"), 10, 3, 128, 128, 0, 0);

    //staticSprites
    bgSprite = sprites[1][6];
    bgSprite.resize(blockSize, blockSize);
    mudSprites = new PImage[16];
    int index=0;
    for (int i = sprites.length-1; i >=0; --i)
    {
      for (int i2 = 0; i2 < sprites[i].length; ++i2)
      {
        sprites[i][i2].resize(blockSize, blockSize);


        if (index < mudSprites.length)
        {
          PGraphics output = createGraphics(blockSize, blockSize);
          output.beginDraw();
          output.image(bgSprite, 0, 0);
          output.image(sprites[i][i2], 0, 0);
          output.endDraw();
          mudSprites[index] = output;
        }
        ++index;
      }
    }


    rockSprite = sprites[0][0];
    rockDamagedSprite = sprites[0][1];

    pointsSprite = sprites[0][2];
    rockSprites = new PImage[mudSprites.length];
    for (int i=0; i<mudSprites.length; ++i)
    {
      PGraphics output = createGraphics(blockSize, blockSize);
      output.beginDraw();
      output.image(mudSprites[i], 0, 0);
      output.image(rockSprite, 0, 0);
      output.endDraw();
      rockSprites[i] = output;
    }
    rockDamagedSprites = new PImage[mudSprites.length];
    for (int i=0; i<mudSprites.length; ++i)
    {
      PGraphics output = createGraphics(blockSize, blockSize);
      output.beginDraw();
      output.image(mudSprites[i], 0, 0);
      output.image(rockDamagedSprite, 0, 0);
      output.endDraw();
      rockDamagedSprites[i] = output;
    }
    pointSprites = new PImage[mudSprites.length];
    for (int i=0; i<mudSprites.length; ++i)
    {
      PGraphics output = createGraphics(blockSize, blockSize);
      output.beginDraw();
      output.image(mudSprites[i], 0, 0);
      output.image(pointsSprite, 0, 0);
      output.endDraw();
      pointSprites[i] = output;
    }

    heavySprite = sprites[1][7];
    heavyBloodSprite = sprites[1][8];

    indestructibleSprite = sprites[1][9];

    rockDestroyedSprite = sprites[0][3];

    explosionMarkSprite = sprites[0][4];
    //animationSprites
    //BOMB
    sprites = reader.getSprites(loadImage("BombAnimation.png"), 10, 4, 64, 64, 0, 0);
    bombAnimationSprites = new PImage[sprites.length * sprites[0].length];
    index = 0;
    for (int y = sprites.length-1; y >= 0; --y)
    {
      for (int x = 0; x <sprites[0].length; ++x)
      {

        bombAnimationSprites[index] = sprites[y][x];
        bombAnimationSprites[index].resize(blockSize, blockSize);
        ++index;
      }
    }
  }

 // Name: public PImage getMudSprite
 // Parameters:
 // boolean topEmpty: control top
 // boolean rightEmpty: control right
 // boolean underEmpty: control under
 // boolean leftEmpty: control left
 // Description: gets the right sprite for mud
  public PImage getMudSprite(boolean topEmpty, boolean rightEmpty, boolean underEmpty, boolean leftEmpty)
  {
    int emptyCount = 0;
    int directionID = 0;
    if (topEmpty)
    {
      directionID +=1;
      emptyCount++;
    }
    if (rightEmpty)
    {
      directionID +=2;
      emptyCount++;
    }
    if (underEmpty)
    {
      directionID +=3;
      emptyCount++;
    }
    if (leftEmpty)
    {
      directionID +=4;
      emptyCount++;
    }

    if (emptyCount == 0)
    {
      return mudSprites[15];
    }
    if (emptyCount == 4)
    {
      return mudSprites[14];
    }

    switch (emptyCount)
    {
    case 1:
      return mudSprites[directionID-1];
    case 2:
      if (directionID == 4)
      {
        return mudSprites[4];
      }
      if (directionID == 6)
      {
        return mudSprites[5];
      }
      if (directionID == 3)
      {
        return mudSprites[6];
      }
      if (directionID == 5 && !topEmpty)
      {
        return mudSprites[7];
      }
      if (directionID == 7)
      {
        return mudSprites[8];
      }
      if (directionID == 5)
      {
        return mudSprites[9];
      }

    case 3:
      return mudSprites[20-directionID-1];
    }
    return null;
  }
  
  // Name: public PImage getRockSprite
  // Parameters:
  // boolean topEmpty: control top
  // boolean rightEmpty: control right
  // boolean underEmpty: control under
  // boolean leftEmpty: control left
  // Description: gets the right sprite for rock
   public PImage getRockSprite(boolean topEmpty, boolean rightEmpty, boolean underEmpty, boolean leftEmpty)
  {
    int emptyCount = 0;
    int directionID = 0;
    if (topEmpty)
    {
      directionID +=1;
      emptyCount++;
    }
    if (rightEmpty)
    {
      directionID +=2;
      emptyCount++;
    }
    if (underEmpty)
    {
      directionID +=3;
      emptyCount++;
    }
    if (leftEmpty)
    {
      directionID +=4;
      emptyCount++;
    }

    if (emptyCount == 0)
    {
      return rockSprites[15];
    }
    if (emptyCount == 4)
    {
      return rockSprites[14];
    }

    switch (emptyCount)
    {
    case 1:
      return rockSprites[directionID-1];
    case 2:
      if (directionID == 4)
      {
        return rockSprites[4];
      }
      if (directionID == 6)
      {
        return rockSprites[5];
      }
      if (directionID == 3)
      {
        return rockSprites[6];
      }
      if (directionID == 5 && !topEmpty)
      {
        return rockSprites[7];
      }
      if (directionID == 7)
      {
        return rockSprites[8];
      }
      if (directionID == 5)
      {
        return rockSprites[9];
      }

    case 3:
      return rockSprites[20-directionID-1];
    }

    return null;
  }
  
  // Name: public PImage getRockDamagedSprite
  // Parameters:
  // boolean topEmpty: control top
  // boolean rightEmpty: control right
  // boolean underEmpty: control under
  // boolean leftEmpty: control left
  // Description: gets the right sprite for damaged rock
   public PImage getRockDamagedSprite(boolean topEmpty, boolean rightEmpty, boolean underEmpty, boolean leftEmpty)
  {
    int emptyCount = 0;
    int directionID = 0;
    if (topEmpty)
    {
      directionID +=1;
      emptyCount++;
    }
    if (rightEmpty)
    {
      directionID +=2;
      emptyCount++;
    }
    if (underEmpty)
    {
      directionID +=3;
      emptyCount++;
    }
    if (leftEmpty)
    {
      directionID +=4;
      emptyCount++;
    }

    if (emptyCount == 0)
    {
      return rockDamagedSprites[15];
    }
    if (emptyCount == 4)
    {
      return rockDamagedSprites[14];
    }

    switch (emptyCount)
    {
    case 1:
      return rockDamagedSprites[directionID-1];
    case 2:
      if (directionID == 4)
      {
        return rockDamagedSprites[4];
      }
      if (directionID == 6)
      {
        return rockDamagedSprites[5];
      }
      if (directionID == 3)
      {
        return rockDamagedSprites[6];
      }
      if (directionID == 5 && !topEmpty)
      {
        return rockDamagedSprites[7];
      }
      if (directionID == 7)
      {
        return rockDamagedSprites[8];
      }
      if (directionID == 5)
      {
        return rockDamagedSprites[9];
      }

    case 3:
      return rockDamagedSprites[20-directionID-1];
    }

    return null;
  }
  
  // Name: public PImage getPointsSprite
  // Parameters:
  // boolean topEmpty: control top
  // boolean rightEmpty: control right
  // boolean underEmpty: control under
  // boolean leftEmpty: control left
  // Description: gets the right sprite for the points
   public PImage getPointsSprite(boolean topEmpty, boolean rightEmpty, boolean underEmpty, boolean leftEmpty)
  {
    int emptyCount = 0;
    int directionID = 0;
    if (topEmpty)
    {
      directionID +=1;
      emptyCount++;
    }
    if (rightEmpty)
    {
      directionID +=2;
      emptyCount++;
    }
    if (underEmpty)
    {
      directionID +=3;
      emptyCount++;
    }
    if (leftEmpty)
    {
      directionID +=4;
      emptyCount++;
    }

    if (emptyCount == 0)
    {
      return pointSprites[15];
    }
    if (emptyCount == 4)
    {
      return pointSprites[14];
    }

    switch (emptyCount)
    {
    case 1:
      return pointSprites[directionID-1];
    case 2:
      if (directionID == 4)
      {
        return pointSprites[4];
      }
      if (directionID == 6)
      {
        return pointSprites[5];
      }
      if (directionID == 3)
      {
        return pointSprites[6];
      }
      if (directionID == 5 && !topEmpty)
      {
        return pointSprites[7];
      }
      if (directionID == 7)
      {
        return pointSprites[8];
      }
      if (directionID == 5)
      {
        return pointSprites[9];
      }

    case 3:
      return pointSprites[20-directionID-1];
    }

    return null;
  }
}