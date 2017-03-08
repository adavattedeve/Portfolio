// Tie-21106_2016 / TIE21106-2016-G08
// Project assignment: BomberDash
// File: FlyingEnemy.pde
// Description: FlyingEnemy class, a certain type of enemy
// File modified by: Vilhelmiina, Atte

// Class: FlyingEnemy
class FlyingEnemy extends Enemy
{
  boolean isIdle;
  float agroRange;
  

  Animation idle;
  //Right = 0, Left = 1 
  Animation fly;
  
  // FlyingEnemy constructor
  // Parameters: 
  // int x: x-coordinate
  // int y: y-coordinate
  FlyingEnemy (int x, int y) {
    position = new PVector(x, y);
    target = new PVector(x, y);
    velocity = new PVector(0, 0);
    direction= new PVector(0, -1);
    normalSpeed = blockSize/40;
    agroSpeed = blockSize/40;
    framesForAfterAggro = 20;
    aggroTimer = 0;
    agroRange = blockSize*8;
    aggro = false;
    isIdle = true;
    collider = new Collider(position, blockSize, blockSize);
    PImage spriteSheet = loadImage("BatSpriteSheet.png");
    SpriteSheetReader reader = new SpriteSheetReader();
    PImage[][] sprites = reader.getSprites(spriteSheet, 4, 4, 32, 32, 0, 0);
    for (int xi=0; xi<sprites.length; ++xi)
    {
      for (int yi=0; yi<sprites[0].length; ++yi)
      {
        sprites[xi][yi].resize(blockSize, blockSize);
      }
    }
    
    //fly
    PImage[][] flySprites = new PImage[2][3];
    flySprites[0][0] =sprites[2][1];
    flySprites[0][1] =sprites[2][2];
    flySprites[0][2] =sprites[2][3];
    flySprites[1][0] =sprites[0][1];
    flySprites[1][1] =sprites[0][2];
    flySprites[1][2] =sprites[0][3];
    
    fly = new Animation(flySprites, 3);
    
    //idle
    PImage[][] idleSprites = new PImage[1][3];
    idleSprites[0][0] =sprites[3][1];
    idleSprites[0][1] =sprites[3][2];
    idleSprites[0][2] =sprites[3][3];
    
    idle = new Animation(idleSprites, 5);
   
    currentAnimation = idle;
  }

 // Name: void agroMovement
 // Parameters: -
 // Description: agro movement, updates the position
  void agroMovement()
  {
    RefreshAnimation();
    if (target == null)
    {
      return;
    }    
    PVector newPosition;
    newPosition = calculateNewPosition(direction);
    position.x = newPosition.x;
    position.y = newPosition.y;
  }
  
 // Name: void movement
 // Parameters: -
 // Description: movement, updates the position
  void movement()
  {
    RefreshAnimation();
    if (target == null)
    {
      return;
    }
    else if (!isIdle)
    {
      PVector newPosition;
      newPosition = calculateNewPosition(direction);
      position.x = newPosition.x;
      position.y = newPosition.y;
    } 
  }

 // Name: void refreshTarget
 // Parameters: -
 // Description: refreshes the target
  void refreshTarget()
  {
    if (!targetAchieved())
    {
      return;
    }
    if (aggroTimer<=0)
    {
      isIdle = true;
      return;
    }
    else
    {
      isIdle = false;
    }
    PVector directionToPlayer = new PVector(player.position.x, player.position.y) ;
    directionToPlayer.sub(position);
    if (abs(directionToPlayer.x) > abs(directionToPlayer.y))
    {
      direction.y = 0;
      if (directionToPlayer.x>0)
      {
        direction.x = 1;
      } else
      {
        direction.x = -1;
      }
    } else
    {
      direction.x = 0;
      if (directionToPlayer.y>0)
      {
        direction.y = 1;
      } else
      {
        direction.y = -1;
      }
    }
    target = new PVector(map.block_size*((int)position.x/map.block_size) + direction.x*map.block_size, map.block_size*((int)position.y/map.block_size) + direction.y*map.block_size);

  }

 // Name: PVector calculateNewPosition
 // Parameters: 
 // PVector dir: coordinates
 // Description: calculates the new position for enemy
  PVector calculateNewPosition(PVector dir)
  {
    velocity.x=0;
    velocity.y=0;
    float speed = normalSpeed;
    if (aggroTimer>0)
    {
      speed = agroSpeed;
    }
    if (dir.y<0)
    {
      velocity.y -=speed;
    }
    if (dir.y>0)
    {
      velocity.y +=speed;
    }
    if (dir.x<0)
    {
      velocity.x -=speed;
    }
    if (dir.x>0)
    {
      velocity.x +=speed;
    }
    return new PVector(position.x + velocity.x, position.y + velocity.y);
  }

 // Name: boolean isAgroed
 // Parameters: -
 // Description: tells if agro
  boolean isAgroed()
  {
    return PVector.dist(player.position, position) < agroRange;
  }
  
 // Name: void die
 // Parameters: -
 // Description: when the flying enemy dies it will explode
  void die()
  {
    super.die();
    Bomb bomb = new CrossBomb(1);;
    bomb.x=((int)position.x/map.block_size)*map.block_size;
    bomb.y=((int)position.y/map.block_size)*map.block_size;
    player.t_bombs.add(bomb);
    bomb.Callback(null);
  }
  
 // Name: void RefreshAnimation
 // Parameters: -
 // Description: refreshes the animation of the flying enemy
  void RefreshAnimation()
  {
    if (isIdle)
    {
      currentAnimation = idle;
    }
    else
    {
      currentAnimation = fly;
      if (direction.x>0)
      {
        currentAnimation.changeDimension(0);
      } else
      {
        currentAnimation.changeDimension(1);
      }
    }

  }
}