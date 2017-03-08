// Tie-21106_2016 / TIE21106-2016-G08
// Project assignment: BomberDash
// File: CrossBomb.pde
// Description: Class that extends Bomb
// File modified by: Atte, Vilhelmiina, Usman

// Class: CrossBomb

class CrossBomb extends Bomb
{
  // CrossBomb constructor
  // Parameters:
  // int type: affects on the size of the explosion
  CrossBomb(int type)
  {
    super(type);
    radius = type*2; // radius in blocks, not pixels

    PImage[] animationSprites = images.bombAnimationSprites;
    triggeredAnimation = new Animation(animationSprites, 1);
  }
  
  // Name: void explode
  // Parameters: -
  //
  // Description: 
  // full explosion
  void explode()
  {
    int range = 2 * radius + 1;

    PVector[] temp = new PVector[2 * range];
    //float xCoord = x - map.block_size / 2 - (range / 2) * map.block_size;
    //float yCoord = y - map.block_size / 2 - (range / 2) * map.block_size;
    float xCoord = x - (range / 2) * map.block_size;
    float yCoord = y - (range / 2) * map.block_size;
    PVector start = new PVector(xCoord, yCoord);
    int sum = 0;
    for (int i = 0; i  < range; i++)
    {
      temp[sum] = new PVector(start.x + i * map.block_size, y);
      Block block; 

      try // dirty way to do it, should better check coordinates
      {
        //block = map.blocks[(int)xCoord / map.block_size + i + 1][(int)yCoord / map.block_size + j + 1];
        block = map.getBlock((int)(start.x+i*map.block_size), (int)(y));
      }
      catch(Exception e)
      {
        block = null;
      }
      if (block != null && block.resistance > 0)
        block.takeDamage(2);

      map.addBGDetail(images.explosionMarkSprite, temp[i].x, temp[i].y);
      sum++;
    }
    for (int i = 0; i  < range; i++)
    {
      temp[sum] = new PVector(x, start.y+i*map.block_size);
      Block block; 

      try // dirty way to do it, should better check coordinates
      {
        //block = map.blocks[(int)xCoord / map.block_size + i + 1][(int)yCoord / map.block_size + j + 1];
        block = map.getBlock((int)(x), (int)(start.y+i*map.block_size));
      }
      catch(Exception e)
      {
        block = null;
      }
      if (block != null && block.resistance > 0)
        block.takeDamage(2);

      map.addBGDetail(images.explosionMarkSprite, temp[i].x, temp[i].y);
      sum++;
    }
    Collider explosionColliderH = new Collider(new PVector(start.x, y), range*map.block_size, map.block_size);
    Collider explosionColliderV = new Collider(new PVector(x, start.y), map.block_size, range*map.block_size);
    for (int i=0; i< enemies.size(); ++i)
    {
      if (explosionColliderH.collides(enemies.get(i).collider))
      {
        enemies.get(i).die();
      }
    }
    if (explosionColliderH.collides(player.collider))
    {
      player.die();
    }
    for (int i=0; i< enemies.size(); ++i)
    {
      if (explosionColliderV.collides(enemies.get(i).collider))
      {
        enemies.get(i).die();
      }
    }
    if (explosionColliderV.collides(player.collider))
    {
      player.die();
    }
    effects.explosion(temp);
  }
  
  // Name: void explode
  // Parameters:
  // int _radius: radius in blocks, not pixels
  // Description: 
  // explosion only at radius
  void explode(int _radius)
  {

    PVector[] temp;
    if (_radius == 0)
    {
      temp = new PVector[1];
      temp[0] = new PVector(x, y);
    } else
    {
      temp = new PVector[4];
      temp[0] = new PVector(x, y -_radius*blockSize);
      temp[1] = new PVector(x, y +_radius*blockSize);
      temp[2] = new PVector(x+_radius*blockSize, y);
      temp[3] = new PVector(x-_radius*blockSize, y);
    }

    for (int i=0; i< temp.length; ++i)
    {
      Collider collider = new Collider(temp[i], map.block_size, map.block_size);
      Block block; 

      try // dirty way to do it, should better check coordinates
      {
        //block = map.blocks[(int)xCoord / map.block_size + i + 1][(int)yCoord / map.block_size + j + 1];
        block = map.getBlock((int)(temp[i].x), (int)(temp[i].y));
      }
      catch(Exception e)
      {
        block = null;
      }
      if (block != null && block.resistance > 0)
        block.takeDamage(2);

      for (int index=0; index< enemies.size(); ++index)
      {
        if (collider.collides(enemies.get(index).collider))
        {
          enemies.get(index).die();
        }
      }
      if (collider.collides(player.collider))
      {
        player.die();
      }

      map.addBGDetail(images.explosionMarkSprite, temp[i].x, temp[i].y);
    }

    effects.explosion(temp);
  }
}