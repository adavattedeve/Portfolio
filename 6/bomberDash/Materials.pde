// Tie-21106_2016 / TIE21106-2016-G08
// Project assignment: BomberDash
// File: Materials.pde
// Description: Different kind of blocks.
// File modified by: Francois Mazeau

// Class: Mud
// Description: (description)
class Mud extends Block implements IsMud
{

  private boolean needsRefreshing;
  
  // Mud constructor
  // Parameters: -
  Mud()
  {
    resistance = 1;
    type = 1;
    name = "mud";
    image = images.getMudSprite(false, false, false, false);
    init();    
    needsRefreshing = true;
  }
  
  // Name: void refreshMud
  // Parameters: -
  // Description: refreshes the mud sprite
  //
  void refreshMud()
  {
    image = images.getMudSprite(!(map.getBlock((int)x, (int)y-size) instanceof IsMud), !(map.getBlock((int)x+size, (int)y) instanceof IsMud), 
      !(map.getBlock((int)x, (int)y+size) instanceof IsMud), !(map.getBlock((int)x-size, (int)y) instanceof IsMud));
  }
  
  // Name: void inheritUpdate
  // Parameters: -
  // Description: refreshes the mud if it needs refreshing
  //
  void inheritUpdate()
  {
    if (needsRefreshing)
    {
      refreshMud();
      needsRefreshing = false;
    }
  }
  
  // Name: void onDestruction
  // Parameters: -
  // Description: mud destruction wrapper
  //
  void onDestruction()
  {
    map.OnMudDestruction(x, y);
  }
  
  // Name: void setFlag
  // Parameters: -
  // Description: sets mud refreshing flag
  //
  void setFlag()
  {
    needsRefreshing = true;
  }
}

// Class: Stone
// Description: class for the stone block
class Stone extends Block implements IsMud
{
  boolean needsRefreshing;
  boolean damaged;
  
  // Stone constructor
  // Parameters: -
  Stone()
  {
    resistance = 2;
    type = 2;
    name = "stone";
    image = images.getRockSprite(false, false, false, false);
    init();       
    damaged = false;
    needsRefreshing = true;
  }

  // Name: void inheritUpdate
  // Parameters: -
  // Description: refreshes if it needs refreshing
  //
  void inheritUpdate()
  {
    if (needsRefreshing)
    {
      refreshMud();
      needsRefreshing = false;
    }
  }
  
  // Name: void takeDamage
  // Parameters:
  // int damage: amount of damage
  // Description: damages the stone
  //
  void takeDamage(int damage)
  {
    super.takeDamage(damage);
    if (resistance>0)
    {
      damaged = true;
      refreshMud();
    }
  }
  
  // Name: void onDestruction
  // Parameters: -
  // Description: destruction of the stone
  //
  void onDestruction()
  {
    super.onDestruction();
    getRndObj(0.16);
    map.addBGDetail(images.rockDestroyedSprite, x, y);
    map.OnMudDestruction(x, y);
  }

  // Name: void refreshMud
  // Parameters: -
  // Description: refeshes the stone sprites
  //
  void refreshMud()
  {
    if (damaged)
    {
      image = images.getRockDamagedSprite(!(map.getBlock((int)x, (int)y-size) instanceof IsMud), !(map.getBlock((int)x+size, (int)y) instanceof IsMud), 
        !(map.getBlock((int)x, (int)y+size) instanceof IsMud), !(map.getBlock((int)x-size, (int)y) instanceof IsMud));
      return;
    }
    image = images.getRockSprite(!(map.getBlock((int)x, (int)y-size) instanceof IsMud), !(map.getBlock((int)x+size, (int)y) instanceof IsMud), 
      !(map.getBlock((int)x, (int)y+size) instanceof IsMud), !(map.getBlock((int)x-size, (int)y) instanceof IsMud));
  }
  
  // Name: void setFlag
  // Parameters: -
  // Description: sets stone refreshing flag
  //
  void setFlag()
  {
    needsRefreshing = true;
  }
}

// Class: Indestructibe
// Description: (description)
class Indestructible extends Block implements IsMud
{

  // Indestructible constructor
  // Parameters: -
  Indestructible()
  {
    resistance = -1;
    type = 0;
    name = "indestructible";
    image = images.indestructibleSprite;
    init();
    
  }
  
  // Name: void update
  // Parameters: -
  // Description: updates the indestructible block
  //
  // override update so this won't be destroyed even if negative resistance left
  void update()
  {
    display(x, y);
  }
  
  // Name: void takeDamage
  // Parameters: 
  // int damage: the amount of the damage
  // Description: damages the block
  void takeDamage(int damage)
  {
     effects.blockTakeDamage(this);
  }
  
  // Name:  void refreshMud
  // Parameters: -
  // Description: -
  void refreshMud()
  {
  }
  
  // Name:  void setFlag
  // Parameters: -
  // Description: -
  void setFlag()
  {
  }
}

// Class: Points
class Points extends Block implements IsMud
{
  private boolean needsRefreshing;
  // Points constructor
  // Parameters: -
  // Description: Player will get points from these
  Points()
  {
    name = "points";
    type = 3;
    resistance = 1;
    image = images.getMudSprite(false, false, false, false);
    init();
    needsRefreshing = true;
  }

  // Name: void onDestruction
  // Parameters: -
  // Description: on destruction gives points for player
  void onDestruction()
  {
    super.onDestruction();
    player.score += 10;
    map.OnMudDestruction(x, y);
  }
  
  // Name: void inheritUpdate
  // Parameters: -
  // Description: refreshes if it needs refreshing
  //
  void inheritUpdate()
  {
    if (needsRefreshing)
    {
      refreshMud();
      needsRefreshing = false;
    }
  }

  // Name: void refreshMud
  // Parameters: -
  // Description: refeshes the points sprites
  //
  void refreshMud()
  {
    image = images.getPointsSprite(!(map.getBlock((int)x, (int)y-size) instanceof IsMud), !(map.getBlock((int)x+size, (int)y) instanceof IsMud), 
      !(map.getBlock((int)x, (int)y+size) instanceof IsMud), !(map.getBlock((int)x-size, (int)y) instanceof IsMud));
  }
  
  // Name: void setFlag
  // Parameters: -
  // Description: sets points refreshing flag
  //
  void setFlag()
  {
    needsRefreshing = true;
  }
}

// Class: Heavy
// Description: The heavy blocks that can fall down
class Heavy extends Block
{
  float speed;
  boolean falling;
  PImage killedSprite;   //Sprite after heavy is killed creature/player.
  
  // Heavy constructor
  // Parameters: -
  Heavy()
  {
    speed = blockSize/20;
    resistance = 2;
    type = 4;
    name = "heavy";
    falling = false;
    image = images.heavySprite;
    killedSprite = images.heavyBloodSprite;
    init();
  }

  // Name: void inheritUpdate
  // Parameters: -
  // Description: 
  // updating and falling of the block
  // might be a better way to do that
  void inheritUpdate() 
  {
    if (map == null)
      return;
    if (map.blocks != null)
    {
      if (map.blocks[(int)x/size][(int)y/size + 1] == null) //check to change
      {
        //float px = player.position.x;
        //float py = player.position.y;
        
        //if ((py + speed > y && py + speed - y < size && (px + player.Width > x && px < x + size)) && !player.dead)
        //if (collider.collides(player.collider) && (player.collider.position.y + player.collider.offset.y)<y+size)
          //falling = false;
        //else
          falling = true;
      }
    }
    if (falling)
    {
      float offset = (y % size + speed) - size;
      if (offset < 0)
        offset = 0;
      y += speed - offset;
      collider.position.x = x;
      collider.position.y = y;
      for (int i=0; i< enemies.size(); ++i)
      {
        if (collider.collides(enemies.get(i).collider))
        {
          image = killedSprite;
          effects.heavyCollision(x, y);
          enemies.get(i).die();
        }
      }
      if (collider.collides(player.collider))
      {
        image = killedSprite;
        effects.heavyCollision(x, y);       
        player.die();
      }
      for (int i=0; i< player.t_bombs.size(); ++i)
      {
        if (collider.collides(player.t_bombs.get(i).collider))
        {
          player.t_bombs.get(i).Callback(null);
        }
      }
    }
    if (falling  && y % size == 0)
    {
      falling = false;
      map.blocks[(int)x/size][(int)y/size] = this;
      map.blocks[(int)x/size][(int)y/size - 1] = null;

      if (map.blocks[(int)x/size][(int)y/size + 1] != null)
      {
        effects.heavyCollision(x, y);
      }
    }
  }
}