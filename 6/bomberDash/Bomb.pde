// Tie-21106_2016 / TIE21106-2016-G08
// Project assignment: BomberDash
// File: Bomb.pde
// Description: A lootable and triggerable bomb
// File modified by: Francois Mazeau, Vilhelmiina, Usman

// Class: Bomb
class Bomb extends Object implements AnimationEndedCallback
{
  Collider collider;
  int timer;
  boolean exploded;
  boolean triggered;
  int radius;
  int x, y;

  boolean exploding;
  int framesBetweenExplosions;
  int explosionPhase;
  Animation triggeredAnimation;

  // Bomb constructor
  // Parameters:
  // int type: the radius and timer varies with the type of the bomb.
  //
  Bomb(int type)
  {    
    name = "bomb";
    radius = type; // radius in blocks, not pixels
    x = 1;
    y = 1;
    id = 1;
    exploded = false;
    PImage[] animationSprites = images.bombAnimationSprites;
    triggeredAnimation = new Animation(animationSprites, 1);
    framesBetweenExplosions = 10;
    explosionPhase = 0;
    timer = 0;
    exploding = false;
    init();
  }

  // Name: void update
  // Parameters: -
  // Description: updates the bomb at each frame
  //
  void update()
  {

    if (triggered)
    {
      triggeredAnimation.update(x, y);
    }
    if (exploding)
    {      
      if (timer >= framesBetweenExplosions)
      {
        explode(explosionPhase);
        if (explosionPhase == radius)
        {
          exploded=true;
        }
        explosionPhase++;
        timer = 0;
      }
      ++timer;
    }
  }

  // Name: void trigger
  // Parameters:
  // int xCoord: x-coordinate
  // int yCoord: y-coordinate
  // Description: sets triggered to true, so that update display the bomb at xCoord, yCoord.
  //
  void trigger()
  {
    triggered = true;
    x = (int)(player.position.x+player.Width*map.block_size/2f) - (int)(player.position.x+player.Width*map.block_size/2f) % map.block_size;
    y = (int)(player.position.y+player.Height*map.block_size/2f) - (int)(player.position.y+player.Height*map.block_size/2f) % map.block_size;
    collider = new Collider(new PVector(x, y), map.block_size, map.block_size);
    player.t_bombs.add(this);
    player.bombs.remove(this);
    triggeredAnimation.StartAnimationWithCallback(this);
  }

  // Name: void explode
  // Parameters: -
  // Description: damages the blocks nearby, triggers animations.
  //
  void explode()
  {
    int range = 2 * radius + 1;

    PVector[] temp = new PVector[range * range];

    float xCoord = x - (range / 2) * map.block_size;
    float yCoord = y - (range / 2) * map.block_size;

    PVector start = new PVector(xCoord, yCoord);
    int sum = 0;
    for (int i = 0; i  < range; i++)
    {
      for (int j = 0; j < range; j++)
      {
        temp[sum] = new PVector(start.x + i * map.block_size, start.y + j * map.block_size);
        Block block; 

        try // dirty way to do it, should better check coordinates
        {
          //block = map.blocks[(int)xCoord / map.block_size + i + 1][(int)yCoord / map.block_size + j + 1];
          block = map.getBlock((int)(start.x+i*map.block_size), (int)(start.y+j*map.block_size));
        }
        catch(Exception e)
        {
          block = null;
        }
        if (block != null)
          block.takeDamage(2);

        map.addBGDetail(images.explosionMarkSprite, temp[i].x, temp[i].y);
        sum++;
      }
    }
    Collider explosionCollider = new Collider(start, range*map.block_size, range*map.block_size);
    for (int i=0; i< enemies.size(); ++i)
    {
      if (explosionCollider.collides(enemies.get(i).collider))
      {
        enemies.get(i).die();
      }
    }
    if (explosionCollider.collides(player.collider))
    {
      player.die();
    }
    effects.explosion(temp);
  }
  
  // Name: void explode
  // Parameters:
  // int _radius: radius
  // Description: explosion only at radius
  void explode(int _radius)
  {
    int range = 2 * _radius + 1;
    PVector[] temp;
    if (_radius == 0)
    {
      temp = new PVector[1];
    } else
    {
      temp = new PVector[range * 4 - 4];
    }


    float xCoord = x - (range / 2) * map.block_size;
    float yCoord = y - (range / 2) * map.block_size;

    PVector start = new PVector(xCoord, yCoord);
    int sum = 0;
    for (int i = 0; i  < range; i++)
    {
      for (int j = 0; j < range; j++)
      {
        if (i!=0 && i!=range-1 && j!=0 && j != range-1)
        {
          continue;
        }
        temp[sum] = new PVector(start.x + i * map.block_size, start.y + j * map.block_size);
        Collider collider = new Collider(temp[sum], map.block_size, map.block_size);
        Block block; 

        try // dirty way to do it, should better check coordinates
        {
          //block = map.blocks[(int)xCoord / map.block_size + i + 1][(int)yCoord / map.block_size + j + 1];
          block = map.getBlock((int)(start.x+i*map.block_size), (int)(start.y+j*map.block_size));
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

        map.addBGDetail(images.explosionMarkSprite, temp[sum].x, temp[sum].y);
        sum++;
      }
    }

    effects.explosion(temp);
  }
  
  // Name: void Callback
  // Parameters:
  // Animation animationThatEnded
  //
  // Description: Explodes the bomb
  void Callback(Animation animationThatEnded)
  {
    if (!exploding)
    {
      explode(explosionPhase);
      explosionPhase++;
      triggered = false;
      exploding = true;
    }
  }
}