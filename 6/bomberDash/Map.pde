// Tie-21106_2016 / TIE21106-2016-G08
// Project assignment: BomberDash
// File: Map.pde
// Description: Map
// File modified by: Francois Mazeau, Vilhelmiina

// Class: Map
class Map
{
  protected int different_blocks; /* This is only for the random generation, it counts the number of Blocks classes */
  int block_size;
  int mapWidth;
  int mapHeight;
  Block blocks[][];
  PImage bg[][];
  int maxScore;

  private int bgSize;

  int playerSpawnX; //as block index
  int playerSpawnY; //as block index
  Exit exit; // used to check if the player reached exit.
  boolean exitExists; // used to check if an exit already exists or not
  LevelData currentLevel;
  int minimumEnemySpawnDistance; //minumum distance where enemy can spawn from player

  int startTime;
  int enemySpawnRate;
  ArrayList<Teleporter> teleporters;
  
  // Map constructor
  // Parameters:
  // LevelData level: level
  Map(LevelData level)
  {
    bgSize = (int)(blockSize);
    images.bgSprite.resize(bgSize, bgSize);
    mapHeight = level.h;
    mapWidth = level.w;
    different_blocks = 4;
    maxScore = 0;
    block_size = blockSize;
    exitExists = false;     
    currentLevel = level;
    playerSpawnX = (int)level.playerSpawn.x/blockSize;
    playerSpawnY = (int)level.playerSpawn.y/blockSize;
    player = new Player(playerSpawnX* blockSize, playerSpawnY* blockSize);
    minimumEnemySpawnDistance = 6;
    enemySpawnRate = currentLevel.enemySpawnDeltaTime;
    teleporters = new ArrayList<Teleporter>();
    teleporters.add(new Teleporter());
    init();
  }

  /* BEWARE : Here, w and h represent the number of blocks, not the number of pixels */


  // Name: void init
  // Parameters: -
  // Description: initializes the map
  //
  void init()
  {
    blocks = new Block[mapHeight][mapWidth];
    bg = new PImage[mapHeight][mapWidth];
    for (int i = 0; i < mapWidth; i++)
    {
      for (int y = 0; y < mapHeight; y++)
      {
        bg[i][y] = null;
        Block new_block;
        if (i == 0 || y == 0 || i == mapWidth - 1 || y == mapHeight - 1)
          new_block = new Indestructible();        
        else if (playerSpawnX == y && playerSpawnY-1 == i)
          new_block = new Mud();
        else if (playerSpawnX !=y || playerSpawnY != i)
          new_block = generate_block();
        else//it's player spawnpoint
          new_block = null;

        if (new_block != null)
        {
          blocks[y][i] = new_block;
          blocks[y][i].x = y * new_block.size;
          blocks[y][i].y = i * new_block.size;
        } 
        else if (blocks[y][i-1] != null && blocks[y][i-1].getClass() == Heavy.class)
        {
          --y;
          continue;
        }
      }
      startTime = millis();
    }
    spawnStartEnemies();
  }

  // Name: Block generate_block
  // Parameters: -
  // Description: generates a block, randomized material or empty area
  //
  Block generate_block()
  {

    int rnd = (int)random(0, 101);
    if (rnd >=0 && rnd < 10)
    {
      maxScore += 10;
      return new Points();
    }
    if (rnd >= 10 && rnd < 25)
      return new Stone();
    if (rnd >= 25 && rnd < 35)
      return new Heavy();
    if (rnd >= 35 && rnd < 65)
      return null;
    else
      return new Mud();
  }
  
  // Name: void updateBackground
  // Parameters: -
  // Description: Updates the background behind the blocks
  //
  void updateBackground()
  {
    int xOff = (int)(cameraPosition.x-cameraOffset.x) /bgSize;
    int yOff = (int)(cameraPosition.y-cameraOffset.y) / bgSize;

    xOff = min(xOff, mapWidth - ((width / bgSize) + 2));
    xOff = max(xOff, 0);

    yOff = min(yOff, mapHeight- ((height / bgSize) + 2));
    yOff = max(yOff, 0);

    for (int i = xOff; i < xOff + (width / bgSize)+2; i++)
    {
      if (i==mapWidth)
      {
        break;
      }
      for (int y = yOff; y < yOff +  (height / bgSize)+2; y++)
      {
        if (y==mapHeight)
        {
          break;
        }
        if (blocks[i][y] == null || blocks[i][y].getClass() == Heavy.class )
        {
          if (bg[i][y]== null)
          {
            image(images.bgSprite, (int)(i*bgSize-cameraPosition.x), (int)(y*bgSize-cameraPosition.y));
          } else
          {
            image(bg[i][y], (int)(i*bgSize-cameraPosition.x), (int)(y*bgSize-cameraPosition.y));
          }
        }
      }
    }
  }
  
  // Name: void update
  // Parameters: -
  // Description: Updates the blocks that are in the screen area
  //
  void update() 
  {    
    int xOff = (int)(cameraPosition.x-cameraOffset.x) / block_size;
    int yOff = (int)(cameraPosition.y-cameraOffset.y) / block_size;

    xOff = min(xOff, mapWidth - ((width / block_size) + 2));
    xOff = max(xOff, 0);

    yOff = min(yOff, mapHeight- ((height / block_size) + 2));
    yOff = max(yOff, 0);
    for (int i = xOff; i < xOff + (width / block_size) + 2; i++)
    {
      if (i==mapWidth)
      {
        break;
      }
      for (int y = yOff; y < yOff + (height / block_size)+2; y++)
      {
        if (y==mapHeight)
        {
          break;
        }
        if (blocks[i][y] != null)
        {
          if (blocks[i][y].destroyed)
          {
            blocks[i][y] = null;
            continue;
          }          

          blocks[i][y].update();
        }
      }
    }

    if (millis()-startTime > enemySpawnRate)
    {
      spawnRandomEnemy();
      enemySpawnRate -= currentLevel.enemySpawnTimeReduction;
      enemySpawnRate = max (enemySpawnRate, currentLevel.minEnemySpawnTime);
      startTime = millis();
    }
    
    for (Teleporter t : teleporters)
      t.update();
    if (player.score >= maxScore / 3)
    {
      if (!exitExists)
      {
        exitExists = true;
        exit = new Exit();
      }
      
      exit.update();
    }
  }

  // Name: Block getBlock
  // Parameters: 
  // int xCoord: x-coordinate
  // int yCoord: y-coordinate
  //
  // Description: returns the block
  //
  Block getBlock(int xCoord, int yCoord)
  {
    xCoord = xCoord/block_size;
    yCoord = yCoord/block_size;
    if (blocks.length>xCoord && blocks[0].length>yCoord && xCoord>=0 && yCoord >=0)
    {
      return blocks[xCoord][yCoord];
    }

    return null;
  }

  // Name: void save
  // Parameters: -
  // Description: saves the map as an ascii file (I have problems with seriasization right now)
  //
  void save()
  {
    PrintWriter out = createWriter(dataPath("map"));

    for (int i = 0; i < mapWidth; i++)
    {
      for (int y = 0; y < mapHeight; y++)
      {
        if (map.blocks[y][i] != null)
          out.print(map.blocks[y][i].type);
        else
          out.print("x");
      }
      out.println();
    }
    out.flush();
    out.close();
  } 
  
  // Name: void spawnStartEnemies
  // Parameters: -
  // Description: creates enemies at the start of the game
  //
  void spawnStartEnemies()
  {
    for (int i=0; i<currentLevel.startBasicEnemies; ++i)
    {
      PVector pos = getRandomEmptyLocation(minimumEnemySpawnDistance);
      enemies.add(new BasicEnemy((int)pos.x, (int)pos.y));
    }
    for (int i=0; i<currentLevel.startFlyingEnemies; ++i)
    {
      PVector pos = getRandomEmptyLocation(minimumEnemySpawnDistance);
      enemies.add(new FlyingEnemy((int)pos.x, (int)pos.y));
    }
    for (int i=0; i<currentLevel.startBlockEatingEnemies; ++i)
    {
      PVector pos = getRandomEmptyLocation(minimumEnemySpawnDistance);
      enemies.add(new BlockEatingEnemy((int)pos.x, (int)pos.y));
    }
  }
  
  // Name: void spawnRandomEnemy
  // Parameters: 
  // Description: spawn random enemy at random location far enough from the player
  //
  void spawnRandomEnemy()
  {

    PVector pos = getRandomEmptyLocation(minimumEnemySpawnDistance);
    ;
    float max=0;
    max +=currentLevel.basicEnemyChance;
    max +=currentLevel.flyingEnemyChance;
    max +=currentLevel.blockEatingEnemyChance;

    float rnd = random(0, max);
    float current=0;

    current += currentLevel.basicEnemyChance;
    if (rnd < current)
    {
      enemies.add(new BasicEnemy((int)pos.x, (int)pos.y));
      return;
    }
    current += currentLevel.flyingEnemyChance;
    if (rnd < current)
    {
      enemies.add(new FlyingEnemy((int)pos.x, (int)pos.y));
      return;
    }
    current += currentLevel.blockEatingEnemyChance;
    if (rnd < current)
    {
      enemies.add(new BlockEatingEnemy((int)pos.x, (int)pos.y));
      return;
    }
  }
  
  // Name: PVector getRandomEmptyLocation
  // Parameters: 
  // int minDistanceFromPlayer: minimum distance from player in blocks
  // Description: returns random empty location.
  //
  PVector getRandomEmptyLocation(int minDistanceFromPlayer)
  {
    int playerX = (int)player.position.x/blockSize;
    int playerY = (int)player.position.y/blockSize;

    ArrayList<PVector> emptyBlocks = new ArrayList<PVector>(); // empty block positions
    for (int x=0; x<blocks.length; ++x)
    {
      for (int y=0; y<blocks[x].length; ++y)
      {
        if (blocks[x][y] == null)
        {
          emptyBlocks.add(new PVector(x*blockSize, y*blockSize));
        }
      }
    }
    //try random blocks
    int tryRandomCount = 10;
    while (tryRandomCount>0)
    {
      PVector random = emptyBlocks.get(floor(random(0, emptyBlocks.size())));
      if (abs(random.x/blockSize-playerX)>minDistanceFromPlayer || abs(random.y/blockSize-playerY)>minDistanceFromPlayer)
      {
        return random;
      }

      --tryRandomCount;
    }

    //loop every block if randoms not succesful
    for (int i=0; i<emptyBlocks.size(); ++i)
    {
      PVector current = emptyBlocks.get(i);
      if (abs(current.x/blockSize-playerX)>minDistanceFromPlayer || abs(current.y/blockSize-playerY)>minDistanceFromPlayer)
      {
        return current;
      }
    }

    //Return farthest point
    int farthestIndex=0;
    for (int i=0; i<emptyBlocks.size(); ++i)
    {
      PVector current = emptyBlocks.get(i);
      if (abs(current.x/blockSize-playerX) + abs(current.y/blockSize-playerY)>abs(emptyBlocks.get(farthestIndex).x/blockSize-playerX) + abs(emptyBlocks.get(farthestIndex).y/blockSize-playerY))
      {
        farthestIndex = i;
      }
    }

    return emptyBlocks.get(farthestIndex);
  }
  
  // Name: void OnMudDestruction
  // Parameters: 
  // float _x: x-coordinate
  // float _y: y-coordinate
  //
  // Description: Mud destructs when the player mines it
  void OnMudDestruction(float _x, float _y)
  {
    Block block = map.getBlock((int)_x, (int)(_y-blockSize));
    if (block instanceof IsMud)
    {
      ((IsMud)block).setFlag();
    }
    block = map.getBlock((int)_x, (int)(_y+blockSize));
    if (block instanceof IsMud)
    {
      ((IsMud)block).setFlag();
    }
    block = map.getBlock((int)_x-blockSize, (int)(_y));
    if (block instanceof IsMud)
    {
      ((IsMud)block).setFlag();
    }
    block = map.getBlock((int)_x+blockSize, (int)(_y));
    if (block instanceof IsMud)
    {
      ((IsMud)block).setFlag();
    }
  }

  // Name:  void addBGDetail
  // Parameters: 
  // PImage image
  // float _x: x-coordinate
  // float _y: y-coordinate
  //
  // Description: Creates background sprites
  void addBGDetail(PImage image, float _x, float _y)
  {
    _x = min(_x, bg.length*blockSize- blockSize);
    _y = min(_y, bg[0].length*blockSize- blockSize);
    _x = max(_x, 0);
    _y = max(_y, 0);
    int xIndex =(int)_x/blockSize;
    int yIndex =(int)_y/blockSize;
    PImage base = images.bgSprite;
    if (bg[xIndex][yIndex] != null )
    {
      base = bg[xIndex][yIndex];
    }
    PGraphics output = createGraphics(blockSize, blockSize);
    output.beginDraw();
    output.image(base, 0, 0);
    output.image(image, 0, 0);
    output.endDraw();
    bg[xIndex][yIndex]= output;
  }
}