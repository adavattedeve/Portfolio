// Tie-21106_2016 / TIE21106-2016-G08
// Project assignment: BomberDash
// File: Player.pde
// Description: Player
// File modified by: Francois, Vilhelmiina, Usman, Atte

// Class: Player
class Player implements AnimationEndedCallback
{
  String name;
  PVector position;
  float maxSpeed;
  float correctionSpeed;
  PVector velocity;
  PVector direction;

  Block mining;
  int miningCooldown;
  int miningCooldownLeft;
  float Width;
  float Height;

  int framesBetweenSteps;
  int stepFrameCount;

  boolean dead;

  //UP = 0, RIGHT = 1 DOWN = 2 LEFT = 3
  Animation idle;
  Animation walk;

  Animation death;

  Animation currentAnimation;

  int score;
  ArrayList<Bomb> bombs;
  ArrayList<Bomb> t_bombs; //triggered bombs, this is the array to always update

  Collider collider;
  Collider horiCollider;
  Collider vertCollider;

  // Player constructor
  // Parameters: 
  // float x: x-coordinate
  // float y: y-coordinate
  Player(float x, float y)
  {
    Width = 0.66;
    Height = 0.7;
    float correctionOffset = 0.05;
    name = "";
    maxSpeed = blockSize/16.6;
    correctionSpeed = blockSize/16.6;
    miningCooldown = 20;
    miningCooldownLeft = miningCooldown;
    framesBetweenSteps = 15;
    stepFrameCount = 0;
    velocity = new PVector(0, 0);
    direction = new PVector(0, -1);
    dead = false;
    position = new PVector(x, y);

    mining = null; 
    InitAnimations();
    score = 0;
    collider = new Collider(position, new PVector(0.2*blockSize, 0.3*blockSize), (int)(Width*blockSize), (int)(Height*blockSize));
    horiCollider = new Collider(position, new PVector(0+collider.offset.x, (1-correctionOffset)/2*collider.sizeY+collider.offset.y), (int)(collider.sizeX), (int)(correctionOffset*collider.sizeY));
    vertCollider = new Collider(position, new PVector((1-correctionOffset)/2*collider.sizeX+collider.offset.x, 0+collider.offset.y), (int)(correctionOffset*collider.sizeX), (int)(collider.sizeY));
    bombs = new ArrayList<Bomb>();
    t_bombs = new ArrayList<Bomb>();
  }

  // Name: void update
  // Parameters: -
  // Description: updates the players properties,
  // calls mining, playerStep, currentAnimation and updateBombs
  void update()
  {
  
    if (!dead)
    {
      PVector newPosition = calculateNewPosition();
      if (!MoveToNewPosition(newPosition))
      {
        if ((velocity.x != 0 || velocity.y != 0) && (velocity.x == 0 || velocity.y == 0))
        {
          mineMineral(newPosition);
        } else
        {
          mining=null;
        }
      } else if (velocity.x != 0 || velocity.y != 0)
      {
        mining = null;
        ++stepFrameCount;
        if (stepFrameCount>=framesBetweenSteps)
        {
          effects.playerStep();
          stepFrameCount=0;
        }
      }
      if (mining!=null && !mining.destroyed)
      {     
        if (miningCooldownLeft<=0)
        {
          effects.mine();
          if (mining.getClass() == Heavy.class)
          {
            mining.takeDamage(0);
          } else
          {
            mining.takeDamage(1);
          }

          miningCooldownLeft = miningCooldown;
        }
      }

      if (miningCooldownLeft>0)
      {
        miningCooldownLeft--;
      }

      refreshCurrentAnimation();
    }
    updateBombs();
    currentAnimation.update(position.x, position.y);
  }

  // Name: PVector calculateNewPosition
  // Parameters: -
  // Description: calculates the new position, depends on which keys are pressed
  //
  PVector calculateNewPosition()
  {
    velocity.x=0;
    velocity.y=0;
    if (keys['w'])
    {
      velocity.y -=maxSpeed;
    }
    if (keys['s'])
    {
      velocity.y +=maxSpeed;
    }
    if (keys['a'])
    {
      velocity.x -=maxSpeed;
    }
    if (keys['d'])
    {
      velocity.x +=maxSpeed;
    }

    if (velocity.x!=0 || velocity.y!=0)
    {
      direction.x = velocity.x;
      direction.y = velocity.y;
    }
    return new PVector(position.x + velocity.x, position.y + velocity.y);
  }

  // Name: boolean MoveToNewPosition
  // Parameters: 
  // PVector newPosition: coordinates where to move
  //
  // Description: moves the player if allowed
  //
  boolean MoveToNewPosition (PVector newPosition)
  {
    boolean succesful = false;
    if (velocity.y!=0 && velocity.x==0)
    {
      int colliderY = (int)(newPosition.y + collider.offset.y);
      int colliderX = (int)(collider.position.x+collider.offset.x);
      Block east;
      Block west;
      if (velocity.y>0)
      {
        east = map.getBlock(colliderX, colliderY+collider.sizeY);
        west = map.getBlock(colliderX + collider.sizeX, colliderY+collider.sizeY);
      } 
      else
      {
        east = map.getBlock(colliderX, colliderY);
        west = map.getBlock(colliderX + collider.sizeX, colliderY);
      }
      vertCollider.position = newPosition;
      if (east==null && west==null)
        {
          position.y=newPosition.y;
          succesful = true;
        }
        else if ((east!= null && !east.collider.collides(vertCollider)) && west == null)
        {
          position.x += correctionSpeed;
          succesful = true;
        }
        else if ((west!= null && !west.collider.collides(vertCollider)) && east == null)
        {
          position.x -= correctionSpeed;
          succesful = true;
        }
        vertCollider.position = position;
    } 
    else if (velocity.y==0 && velocity.x!=0)
    {
      int colliderY = (int)(collider.position.y+collider.offset.y);
      int colliderX =  (int)(newPosition.x + collider.offset.x);
      Block north;
      Block south;
      if (velocity.x>0)
      {
        north = map.getBlock(colliderX + collider.sizeX, colliderY);
        south = map.getBlock(colliderX + collider.sizeX, colliderY+collider.sizeY);
      } else
      {
        north = map.getBlock(colliderX , colliderY);
        south = map.getBlock(colliderX, colliderY+collider.sizeY);
      }
      horiCollider.position = newPosition;
      if (north==null && south==null)
        {
          position.x=newPosition.x;
          succesful = true;
        }
        else if ((north!= null && !north.collider.collides(horiCollider)) && south == null)
        {
          if (north.getClass() == Heavy.class)
          {
            succesful = false;
          }
          else
          {
            position.y += correctionSpeed;
            succesful = true;
          }        
        }
        else if ((south!= null && !south.collider.collides(horiCollider)) && north == null)
        {
          position.y -= correctionSpeed;
          succesful = true;
        }
        horiCollider.position = position;
    } 
    else
    {
      if (velocity.y!=0)
      {
        // Check if oldposition.x and newposition.y is valid location
        int colliderY = (int)(newPosition.y + collider.offset.y);
        int colliderX = (int)(collider.position.x+collider.offset.x);
        Block ne = map.getBlock(colliderX, colliderY);
        Block nw = map.getBlock(colliderX + collider.sizeX, colliderY);
        Block se = map.getBlock(colliderX, colliderY+collider.sizeY);
        Block sw = map.getBlock(colliderX + collider.sizeX, colliderY+collider.sizeY);

        if (ne ==null && nw == null && se == null && sw == null)
        {
          position.y=newPosition.y;
          succesful = true;
        }
      }
      if (velocity.x!=0)
      {
        // Check if newposition.x and oldposition.y is valid location
        int colliderY = (int)(collider.position.y+collider.offset.y);
        int colliderX =  (int)(newPosition.x + collider.offset.x);
        Block ne = map.getBlock(colliderX, colliderY);
        Block nw = map.getBlock(colliderX + collider.sizeX, colliderY);
        Block se = map.getBlock(colliderX, colliderY+collider.sizeY);
        Block sw = map.getBlock(colliderX + collider.sizeX, colliderY+collider.sizeY);

        if (ne ==null && nw == null && se == null && sw == null)
        {
          position.x=newPosition.x;
          succesful = true;
        }
      }
    }
    return succesful;
  }

  // Name: void mineMineral
  // Parameters: 
  // PVector newPosition: coordinates where will be mined
  //
  // Description: mines the block
  void mineMineral(PVector newPosition)
  {
    Block block;
    if (velocity.y>0)
    {
      //Check if there is block at bottom of the player
      block = map.getBlock((int)(position.x + collider.offset.x + collider.sizeX/2), (int)(newPosition.y+ collider.offset.y+collider.sizeY));
      mining = block;
      return;
    } else if (velocity.y<0)
    {
      //Check if there is block at top of the player
      block = map.getBlock((int)(position.x + collider.offset.x + collider.sizeX/2), (int)(newPosition.y + collider.offset.y));
      mining = block;
      return;
    }
    if (velocity.x>0)
    {
      //Check if there is block at right of the player
      block = map.getBlock((int)(newPosition.x + collider.offset.x + collider.sizeX), (int)(position.y+ collider.offset.y+collider.sizeY/2));
      mining = block;
      return;
    } else if (velocity.x<0)
    {
      //Check if there is block at left of the player
      block = map.getBlock((int)(newPosition.x + collider.offset.x), (int)(position.y+ collider.offset.y+collider.sizeY/2));
      mining = block;
      return;
    }
  }

  // Name: void die
  // Parameters: -
  // Description: Players death, updates the animation, calls the animation and effects.
  void die()
  {
    if (!dead)
    {
      dead = true;
      currentAnimation = death;
      death.StartAnimationWithCallback(this);
      effects.playerDeath();
    }
  }

  // Name: void refreshCurrentAnimation
  // Parameters: -
  // Description: Updates the currentAnimation.
  //
  void refreshCurrentAnimation()
  {
    Animation previousAnimation = currentAnimation;
    //animation
    if (velocity.x==0 && velocity.y==0)
    {
      currentAnimation = idle;
    } else
    {
      currentAnimation = walk;
    }
    if (direction.x>0)
    {
      currentAnimation.changeDimension(1);
    } else if (direction.x<0)
    {
      currentAnimation.changeDimension(3);
    }


    if (currentAnimation != previousAnimation)
    {
      currentAnimation.StartAnimation();
    }
  }

  // Name:  private void InitAnimations
  // Parameters: -
  // Description: Up, down, left, right and death animations.
  //
  private void InitAnimations()
  {
    int spritesPerWalkAnim = 8;
    int spritesForIdleAnim = 1;
    SpriteSheetReader spriteReader = new SpriteSheetReader();
    PImage[][] allSprites = spriteReader.getSprites(loadImage("PlayerSpriteSheet.png"), spritesPerWalkAnim + spritesForIdleAnim, 4, 32, 44, 14, 14);

    int spriteRowIndex = 3;
    PImage[][] walkSprites = new PImage[4][spritesPerWalkAnim];
    PImage[][] idleSprites = new PImage[4][spritesForIdleAnim];
    PImage[] deathSprites = new PImage[31];
    for (int i=0; i<allSprites.length; ++i)
    {
      for (int i2 = 0; i2<allSprites[i].length; ++i2)
      {
        allSprites[i][i2].resize((int)(blockSize*Width), (int)(blockSize*Height));
      }
    }

    //Up animations
    for (int i = 0; i<allSprites[spriteRowIndex].length; ++i)
    {

      if (i<spritesForIdleAnim)
      {
        idleSprites[0][i] = allSprites[spriteRowIndex][i];
      } else
      {
        walkSprites[0][i-spritesForIdleAnim] = allSprites[spriteRowIndex][i];
      }
    }


    //Left animations
    --spriteRowIndex;
    for (int i = 0; i<allSprites[spriteRowIndex].length; ++i)
    {
      if (i<spritesForIdleAnim)
      {
        idleSprites[3][i] = allSprites[spriteRowIndex][i];
      } else
      {
        walkSprites[3][i-spritesForIdleAnim] = allSprites[spriteRowIndex][i];
      }
    }


    //Down animations
    --spriteRowIndex;
    for (int i = 0; i<allSprites[spriteRowIndex].length; ++i)
    {
      if (i<spritesForIdleAnim)
      {
        idleSprites[2][i] = allSprites[spriteRowIndex][i];
      } else
      {
        walkSprites[2][i-spritesForIdleAnim] = allSprites[spriteRowIndex][i];
      }
    }


    //Right animations
    --spriteRowIndex;
    for (int i = 0; i<allSprites[spriteRowIndex].length; ++i)
    {
      if (i<spritesForIdleAnim)
      {
        idleSprites[1][i] = allSprites[spriteRowIndex][i];
      } else
      {
        walkSprites[1][i-spritesForIdleAnim] = allSprites[spriteRowIndex][i];
      }
    }
    walk = new Animation(walkSprites, 5);
    idle = new Animation(idleSprites, 5);
    currentAnimation = idle;

    //Death animation
    allSprites = spriteReader.getSprites(loadImage("PlayerDeathAnimation.png"), 5, 7, 68, 68, 0, 0);
    //resize sprites
    int i=0;
    for (int y=allSprites.length-1; y>=0; --y)
    {
      for (int x=0; x< allSprites[0].length; ++x)
      {
        if ((x>2 && y==0) || (x<2 && y == allSprites[0].length-1))
        {
          continue;
        }
        deathSprites[i] = allSprites[y][x];
        deathSprites[i].resize(blockSize, blockSize);
        ++i;
      }
    }
    death = new Animation(deathSprites, 1);
    death.loop=false;
    initNewAnims();
  }

  private void initNewAnims() //FOR TESTING
  {
    int spritesPerWalkAnim = 14;
    int spritesForIdleAnim = 1;
    SpriteSheetReader spriteReader = new SpriteSheetReader();
    PImage[][] allSprites = spriteReader.getSprites(loadImage("RunAnimations.png"), 8, 8, 128, 128, 0, 0);
    PImage[][] idleSpriteSheetSprites = spriteReader.getSprites(loadImage("IdleAnimations.png"), 1, 4, 128, 128, 0, 0);

    PImage[][] walkSprites = new PImage[4][spritesPerWalkAnim];
    PImage[][] idleSprites = new PImage[4][spritesForIdleAnim];
    for (int i=0; i<allSprites.length; ++i)
    {
      for (int i2 = 0; i2<allSprites[i].length; ++i2)
      {
        allSprites[i][i2].resize((int)(blockSize), (int)(blockSize));
      }
    }
    for (int i=0; i<idleSpriteSheetSprites.length; ++i)
    {
      for (int i2 = 0; i2<idleSpriteSheetSprites[i].length; ++i2)
      {
        idleSpriteSheetSprites[i][i2].resize((int)(blockSize), (int)(blockSize));
      }
    }
    //Up animations
    for (int i = 0; i<walkSprites[0].length; ++i)
    {

      if (i<spritesForIdleAnim)
      {
        idleSprites[0][i] = idleSpriteSheetSprites[3][0];
      }
      walkSprites[0][i] = allSprites[1 - i/8][i%8];
    }


    //Left animations
    for (int i = 0; i<walkSprites[0].length; ++i)
    {

      if (i<spritesForIdleAnim)
      {
        idleSprites[3][i] = idleSpriteSheetSprites[2][0];
      }
      walkSprites[3][i] = allSprites[5 - i/8][i%8];
    }


    //Down animations
    for (int i = 0; i<walkSprites[0].length; ++i)
    {

      if (i<spritesForIdleAnim)
      {
        idleSprites[2][i] = idleSpriteSheetSprites[1][0];
      }
      walkSprites[2][i] = allSprites[5 - i/8][i%8];
    }



    //Right animations
    for (int i = 0; i<walkSprites[0].length; ++i)
    {

      if (i<spritesForIdleAnim)
      {
        idleSprites[1][i] = idleSpriteSheetSprites[0][0];
      }
      walkSprites[1][i] = allSprites[1 - i/8][i%8];
    }

    walk = new Animation(walkSprites, 1);
    idle = new Animation(idleSprites, 10);
    currentAnimation = idle;
    currentAnimation.changeDimension(1);
  }

  // Name:  void displayScore
  // Parameters: -
  // Description: displays the players score
  //
  void displayScore()
  {
    textSize(height / 50);
    fill(250);
    text("Score : " + score, 5, 20);
  }

  // Name:  void updateBombs
  // Parameters: -
  // Description: updates players bombs
  //
  void updateBombs()
  {
    for (int i = 0; i < t_bombs.size(); i++)
    {
      Bomb bomb = t_bombs.get(i);
      if (bomb.exploded)
        t_bombs.remove(bomb);
      else
        bomb.update();
    }
  }
  // Name: void Callback
  // Parameters: -
  // Animation animation: animation
  //
  // Description: Animation ended callback
  void Callback(Animation animation)
  {
    state = GameState.END;
  }
}