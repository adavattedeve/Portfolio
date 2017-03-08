// Tie-21106_2016 / TIE21106-2016-G08
// Project assignment: BomberDash
// File: BasicEnemy.pde
// Description: BasicEnemy class, certain type of enemy
// File modified by: Francois, Vilhelmiina, Usman, Atte

// Class: BasicEnemy
class BasicEnemy extends Enemy
{
  boolean foundPath;
  boolean turnedRight;
  PVector size;
  Animation moveAnimation;
  
  // BasicEnemy constructor
  // Parameters: 
  // int x: x-coordinate
  // int y: y-coordinate
  BasicEnemy (int x, int y)
  {
    size = new PVector(0.64, 0.64);
    position = new PVector(x, y);
    velocity = new PVector(0,0);
    direction= new PVector(0,-1);
    target = new PVector(x, y);
    normalSpeed = blockSize/12.5;
    agroSpeed = blockSize/10;
    framesForAfterAggro = 0;
    aggroTimer = 0;
    aggro = false;
    foundPath = false;
    turnedRight = false;
    PImage spriteSheet = loadImage("SlimeSpriteSheet.png");
    SpriteSheetReader reader = new SpriteSheetReader();
    PImage[][] sprites = reader.getSprites(spriteSheet, 10, 2, 32, 32, 0, 0);
    for (int xi=0; xi<sprites.length; ++xi)
    {
      for (int yi=0; yi<sprites[0].length; ++yi)
      {
        sprites[xi][yi].resize(blockSize,blockSize);
      }
    }
    collider = new Collider(position, (int)(blockSize*size.x),(int)(blockSize*size.y));
    PImage[][] moveSprites = new PImage[1][];
    moveSprites[0] = sprites[1];
    moveAnimation = new Animation(moveSprites, 2);
    currentAnimation = moveAnimation;
  }
  
  // Name: void movement
  // Parameters: -
  //
  // Description: 
  // updates the position
  void movement()
  {
    if (target == null)
    {
        return;
    }

    PVector newPosition;
    newPosition = calculateNewPosition(direction);
    position.x = newPosition.x;
    position.y = newPosition.y;
  }
  
  // Name: void refreshTarget
  // Parameters: -
  //
  // Description: defines the route for BasicEnemy 
  //
  void refreshTarget()
  {
    if (!targetAchieved())
    {
      return;
    }
    if (foundPath)
    {
     if (tryTurningRight())
     {
       if (turnedRight)
       {
         foundPath = false;
         turnedRight = false;
       }
       turnedRight = true;
       return;
     }
     if (tryTurningForward())
     {
       turnedRight = false;
       return;
     }
     if (tryTurningLeft())
     {
       turnedRight = false;
       return;
     }
     if (tryTurningBackward())
     {
       turnedRight = false;
       return;
     }
    }
    else
    {
     if (tryTurningForward())
     {
       return;
     }
          if (tryTurningLeft())
     {
       foundPath=true;
       return;
     }
          if (tryTurningRight())
     {
       return;
     }
          if (tryTurningBackward())
     {
       return;
     }
    }
    target = null;
  }

  // Name: boolean tryTurningRight
  // Parameters: -
  //
  // Description: tells if it possible to turn
  //
  boolean tryTurningRight()
  {
   PVector newDir = new PVector(direction.y, -direction.x);
   PVector newTargetPoint = new PVector(map.block_size*((int)position.x/map.block_size)+newDir.x*map.block_size, map.block_size*((int)position.y/map.block_size)+newDir.y*map.block_size); 
   Block block = map.getBlock((int)newTargetPoint.x, (int)newTargetPoint.y);
   if (isBlockWalkable(block))
   {
     
     direction = newDir;
     if (target == null)
     {
       target = new PVector(newTargetPoint.x, newTargetPoint.y);
     }
     else
     {
       target.x = newTargetPoint.x;
       target.y = newTargetPoint.y;
     }

     return true;
   }
   return false;
  }
  
  // Name: boolean tryTurningLeft
  // Parameters: -
  //
  // Description: tells if it possible to turn
  //
  boolean tryTurningLeft()
  {
   PVector newDir = new PVector(-direction.y, direction.x);
   PVector newTargetPoint = new PVector(map.block_size*((int)position.x/map.block_size)+newDir.x*map.block_size, map.block_size*((int)position.y/map.block_size)+newDir.y*map.block_size);  
   Block block = map.getBlock((int)newTargetPoint.x, (int)newTargetPoint.y);
   if (isBlockWalkable(block))
   {
     
     direction = newDir;
     if (target == null)
     {
       target = new PVector(newTargetPoint.x, newTargetPoint.y);
     }
     else
     {
       target.x = newTargetPoint.x;
       target.y = newTargetPoint.y;
     }
     return true;
   }
   return false;
  }
  
  // Name: boolean tryTurningForward
  // Parameters: -
  //
  // Description: tells if it possible to turn
  //
  boolean tryTurningForward()
  {
   PVector newDir = new PVector(direction.x, direction.y);
   PVector newTargetPoint = new PVector(map.block_size*((int)position.x/map.block_size)+newDir.x*map.block_size, map.block_size*((int)position.y/map.block_size)+newDir.y*map.block_size);  
   Block block = map.getBlock((int)newTargetPoint.x, (int)newTargetPoint.y);
   if (isBlockWalkable(block))
   {
     
     direction = newDir;
     if (target == null)
     {
       target = new PVector(newTargetPoint.x, newTargetPoint.y);
     }
     else
     {
       target.x = newTargetPoint.x;
       target.y = newTargetPoint.y;
     }
     return true;
   }
   return false;
  }
  
  // Name: boolean tryTurningBackward
  // Parameters: -
  //
  // Description: tells if it possible to turn
  //
  boolean tryTurningBackward()
  {
   PVector newDir = new PVector(-direction.x, -direction.y);
   PVector newTargetPoint = new PVector(map.block_size*((int)position.x/map.block_size)+newDir.x*map.block_size, map.block_size*((int)position.y/map.block_size)+newDir.y*map.block_size);
   Block block = map.getBlock((int)newTargetPoint.x, (int)newTargetPoint.y);
   if (isBlockWalkable(block))
   {
     
     direction = newDir;
     if (target == null)
     {
       target = new PVector(newTargetPoint.x, newTargetPoint.y);
     }
     else
     {
       target.x = newTargetPoint.x;
       target.y = newTargetPoint.y;
     }
     return true;
   }
   return false;
  }
  
  // Name: PVector calculateNewPosition
  // Parameters: 
  // PVector dir: coordinates
  //
  // Description: calculates the new position for the BasicEnemy
  //
  PVector calculateNewPosition(PVector dir)
  {
    velocity.x=0;
    velocity.y=0;
    float speed = normalSpeed;
    if (aggro)
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
  //
  // Description:  returns false
  //
  boolean isAgroed()
  {
    return false;
  }
  
  // Name:  boolean isBlockWalkable
  // Parameters: 
  // Block block: the block that is checked
  //
  // Description:  tells if the block is walkable
  //
  boolean isBlockWalkable(Block block)
  {
    return block == null;
  }
}