// Tie-21106_2016 / TIE21106-2016-G08
// Project assignment: BomberDash
// File: BlockEatingEnemy.pde
// Description: BlockEatingEnemy class, certain type of enemy
// File modified by: Francois, Vilhelmiina, Atte

// Class: BlockEatingEnemy
class BlockEatingEnemy extends Enemy implements AnimationEndedCallback
{
  boolean foundPath;
  boolean turnedRight;
  float agroRange;
  
  //Right = 0, Left = 1 
  Animation walk;
  Animation toAggro;
  Animation toNormal;
  Animation aggroWalk;
  
  // BlockEatingEnemy constructor
  // Parameters: 
  // int x: x-coordinate 
  // int y: y-coordinate
  BlockEatingEnemy (int x, int y){
    position = new PVector(x, y);
    target = new PVector(x, y);
    velocity = new PVector(0,0);
    direction= new PVector(0,-1);
    normalSpeed = blockSize/40;
    agroSpeed = blockSize/16.6;
    framesForAfterAggro = 60;
    aggroTimer = 0;
    agroRange = blockSize*5;
    aggro = false;
    foundPath = false;
    turnedRight = false;
    collider = new Collider(position,blockSize, blockSize);
    PImage spriteSheetRight = loadImage("BlockEatingEnemyRight.png");
    PImage spriteSheetLeft = loadImage("BlockEatingEnemyLeft.png");
    SpriteSheetReader reader = new SpriteSheetReader();
    PImage[][] spritesRight = reader.getSprites(spriteSheetRight, 4, 5, 80, 70, 0, 0);
    PImage[][] spritesLeft = reader.getSprites(spriteSheetLeft, 4, 5, 80, 70, 0, 0);
    for (int xi=0; xi<spritesRight.length; ++xi)
    {
      for (int yi=0; yi<spritesRight[0].length; ++yi)
      {
        spritesRight[xi][yi].resize(blockSize,(70/80)*blockSize);
        spritesLeft[xi][yi].resize(blockSize,(70/80)*blockSize);
      }
    }
    //aggrowalk
    PImage[][] agroWalkSprites = new PImage[2][];
    agroWalkSprites[0] =spritesRight[0];
    agroWalkSprites[1] =spritesLeft[0];
    aggroWalk = new Animation(agroWalkSprites, 5);
    
    //normalwalk
    PImage[][] walkSprites = new PImage[2][6];
    for (int i=0; i<walkSprites[0].length; ++i)
    {
      walkSprites[0][i] = spritesRight[3][i - (i/3)*2*(i%3)]; // 0, 0, 0, 0, 2, 4
      walkSprites[1][i] = spritesLeft[3][i - (i/3)*2*(i%3)];
    }
    walk = new Animation(walkSprites, 10);
    
    //toAgro
    PImage[][] toAgroSprites = new PImage[2][6];
    for (int i=0; i<toAgroSprites[0].length; ++i)
    {
      toAgroSprites[0][i] = spritesRight[2-(i/4)][i-4*(i/4)];
      toAgroSprites[1][i] = spritesLeft[2-(i/4)][i-4*(i/4)];
    }
    toAggro = new Animation(toAgroSprites, 5);
    
    //toAgro
    PImage[][] toNormalSprites = new PImage[2][6];
    for (int i=0; i<toNormalSprites[0].length; ++i)
    {
      toNormalSprites[0][i] = toAgroSprites[0][5-i];
      toNormalSprites[1][i] = toAgroSprites[1][5-i];
    }
    toNormal = new Animation(toNormalSprites, 5);
    
    currentAnimation = walk;
  }
  
  // Name:  void agroMovement
  // Parameters: -
  //
  // Description: updates the position and damages the blocks
  //
  void agroMovement()
  {
    RefreshAnimation();
      if (target == null)
    {
        return;
    }    
    Block block = map.getBlock((int)target.x, (int)target.y);
 
    if (!isBlockWalkable(block))
    {
      block.takeDamage(10);
    }
    PVector newPosition;
    newPosition = calculateNewPosition(direction);
    position.x = newPosition.x;
    position.y = newPosition.y;
  }
  
  // Name:   void movement
  // Parameters: -
  //
  // Description: updates the position and damages the blocks
  //
  void movement()
  {
    RefreshAnimation();
    if (target == null)
    {
        return;
    }
    Block block = map.getBlock((int)target.x, (int)target.y);

    if (!isBlockWalkable(block))
    {
      block.takeDamage(10);
    }
    PVector newPosition;
    newPosition = calculateNewPosition(direction);
    position.x = newPosition.x;
    position.y = newPosition.y;
  }
  
  // Name:  void refreshTarget
  // Parameters: -
  //
  // Description: updates target and direction
  //
  void refreshTarget()
  {
    if (!targetAchieved())
    {
        return;
    }
    if (aggroTimer<=0)
    {
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
    }
    else
    {
      foundPath = false;
      PVector directionToPlayer = new PVector(player.position.x, player.position.y) ;
      directionToPlayer.sub(position);
      if (abs(directionToPlayer.x) > abs(directionToPlayer.y))
      {
        direction.y = 0;
        if (directionToPlayer.x>0)
        {
          direction.x = 1;
          
        }
        else
        {
          direction.x = -1;
        }
      }
      else
      {
        direction.x = 0;
        if (directionToPlayer.y>0)
        {
          direction.y = 1;
        }
        else
        {
          direction.y = -1;
        }
      }
      target = new PVector(map.block_size*((int)position.x/map.block_size) + direction.x*map.block_size, map.block_size*((int)position.y/map.block_size) + direction.y*map.block_size);
      return;
    }
    target = null;
  }

  // Name:  boolean tryTurningRight
  // Parameters: -
  //
  // Description: updates target if turning is possible
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
  
  // Name:  boolean tryTurningLeft
  // Parameters: -
  //
  // Description: updates target if turning is possible
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
  
  // Name:  boolean tryTurningForward
  // Parameters: -
  //
  // Description: updates target if turning is possible
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
  
  // Name:  boolean tryTurningBackward
  // Parameters: -
  //
  // Description: updates target if turning is possible
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

  // Name:  PVector calculateNewPosition
  // Parameters: 
  // PVector dir: coordinates
  //
  // Description: calculates the new position
  //
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
  //
  // Description: Check if the monster is targetting the player
  //
  boolean isAgroed()
  {
      if (((int)player.position.x+blockSize/2)/map.block_size == ((int)position.x+blockSize/2)/map.block_size || ((int)player.position.y-blockSize/2)/map.block_size == ((int)position.y-blockSize/2)/map.block_size)
      {
        if (PVector.dist(player.position, position) < agroRange)
        {
          return true;
        }
      }
    if (aggro)
    {
      foundPath = false;
    }
    return false;
  }
  
  // Name: boolean isBlockWalkable
  // Parameters: 
  // Block block: that is checked
  //
  // Description: check if the block is walkable
  //
  boolean isBlockWalkable(Block block)
  {
    return block == null;
  }
  
  // Name: void RefreshAnimation
  // Parameters: -
  //
  // Description: updates the currentAnimation
  //
  void RefreshAnimation()
  {
    if (currentAnimation == toAggro || currentAnimation == toNormal)
    {
    
    }
    else if (aggroTimer>0 && currentAnimation == walk)
    {
      currentAnimation = toAggro;
      currentAnimation.StartAnimationWithCallback(this);
    }
    else if (aggroTimer<=0 && currentAnimation == aggroWalk)
    {
      currentAnimation = toNormal;
      currentAnimation.StartAnimationWithCallback(this);
    }
    if (direction.x>0)
    {
      currentAnimation.changeDimension(0);
    }
    else
    {
      currentAnimation.changeDimension(1);
    }
     
  }
  
  // Name:  void Callback
  // Parameters: 
  // Animation animation: animation
  //
  // Description: updates the currentAnimation
  //
  void Callback(Animation animation)
  {
    if (currentAnimation == toAggro)
    {
      currentAnimation = aggroWalk;
    }
    else if (currentAnimation == toNormal)
    {
      currentAnimation = walk;
    }
  }
}