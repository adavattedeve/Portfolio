// Tie-21106_2016 / TIE21106-2016-G08
// Project assignment: BomberDash
// File: Block.pde
// Description: A class for the blocks we display on screen
// File modified by: Francois Mazeau

// Class: Block
class Block
{
  protected int type;
  int resistance; // an undestructible has a negative resistance.
  String name;
  boolean destroyed; // a destroyed block has a resistance of 0.
  float x, y;
  int size;
  protected PImage image;
  Collider collider;

  // Block constructor
  // Parameters: -
  Block()
  {
  }

  // Name: protected void init
  // Parameters: -
  // Description: initializes the block
  //
  protected void init()
  {
    destroyed = false;
    x = 0; //actually arbitrary
    y = 0; //same
    size = blockSize; // if this is changed, must also be changed in Map constructor
    collider = new Collider(new PVector (x, y), size, size);
  }

  // Name: void update
  // Parameters: -
  // Description: updates the block, destroying and displaying
  //
  void update()
  {    
    collider.position.x=x;
    collider.position.y=y;
    if (!destroyed && resistance <= 0)
    {
      destroyed = true;
      map.blocks[int(x/size)][(int)(y/size)] = null;
      onDestruction();
    }

    if (!destroyed)
      display(x, y);

    inheritUpdate();
  }

  // Name: void inheritUpdate
  // Parameters: -
  // Description: if we need to add something to the update function for inheriting classes.
  //
  void inheritUpdate()
  {
  }

  // Name: void onDestruction
  // Parameters: -
  // Description: declared for some specific blocks, executed when block destroyed
  //
  void onDestruction()
  {
    if (effects!= null)
      effects.blockDestroyed(this);
    //soon will add an audio file to play at destruction.
  }

  // Name: void display
  // Parameters:
  // float xCoord: x-coordinate
  // float yCoord: y-coordinate
  //
  // Description: displays the image at the correct position
  //
  void display(float xCoord, float yCoord)
  {
    image(image, (int)(xCoord-cameraPosition.x), (int)(yCoord-cameraPosition.y));
  }

  // Name: void takeDamage
  // Parameters:
  // int damage: amount of damage
  //
  // Description: takes damage when mining the block
  //
  void takeDamage(int damage)
  {
    if (resistance > 0)
    {
      effects.blockTakeDamage(this);
      resistance-=damage;
    }
  }

  // Name: void getRndObj
  // Parameters:
  // float prob: probability of getting an item
  //
  // Description: returns a random item, or not.
  //
  void getRndObj(float prob)
  {
    Object rndObj;
    rndObj = new Bomb(1);
    float p = random(0, 1);
    if (prob > p)
    {
      player.bombs.add((Bomb)rndObj);
      println("New " + rndObj.name + " added to inventory");
    }
    //do that later, now focused on the falling blocks.
  }
}