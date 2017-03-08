// Tie-21106_2016 / TIE21106-2016-G08
// Project assignment: BomberDash
// File: Animation.pde
// Description: Animation class. Steps and sprites.
// File modified by: Francois, Vilhelmiina, Usman, Atte

// Class: Animation
class Animation
{
  private PImage[][] sprites;
  private int currentIndex;
  private int currentDimension;
  private int framesPerSprite;
  private int framesBeforeNextStep;
  private AnimationEndedCallback callback;
  
  boolean loop;
  
  // Animation constructor
  // Parameters: 
  // PImage[] _sprites: animation sprites
  // int _framesPerSprite: frames per animation sprite
  Animation(PImage[] _sprites, int _framesPerSprite)
  {
    loop=true;
    sprites = new PImage[1][];
    sprites[0] = _sprites;
    currentIndex = 0;
    currentDimension = 0;
    framesPerSprite = _framesPerSprite;
    framesBeforeNextStep = framesPerSprite;
  }
  
  // Animation constructor
  // Parameters: 
  // PImage[][] _sprites: animation sprites
  // int _framesPerSprite: frames per animation sprite
  Animation(PImage[][] _sprites, int _framesPerSprite)
  {
    loop=true;
    sprites = _sprites;
    currentIndex = 0;
    currentDimension = 0;
    framesPerSprite = _framesPerSprite;
    framesBeforeNextStep = framesPerSprite;
  }
  
  // Name: void update
  // Parameters: 
  // float x: x-coordinate
  // float y: y-coordinate
  //
  // Description: updates the currentIndex 
  // draws the right image to the correct place.
  //
  void update(float x, float y)
  {
    if (framesBeforeNextStep==0)
    {
      NextStep();
    }
    else
    {
      framesBeforeNextStep--;
    }
    currentIndex = min(currentIndex, sprites[currentDimension].length-1);
    image(sprites[currentDimension][currentIndex], (int)(x-cameraPosition.x),  (int)(y-cameraPosition.y));  
  }
  
  // Name: void StartAnimation
  // Parameters: -
  // Description: Starts the animation
  //
  void StartAnimation()
  {
    currentIndex = 0;
  }
  
  // Name: void StartAnimationWithCallback
  // Parameters: 
  // AnimationEndedCallback _callback:
  //
  // Description: Starts the animation with callback
  //
  void StartAnimationWithCallback(AnimationEndedCallback _callback)
  {
    currentIndex = 0;
    callback = _callback;
  }
  
  // Name: private void NextStep
  // Parameters: -
  // Description: gets the next step
  //
  private void NextStep()
  {
    framesBeforeNextStep = framesPerSprite;
    if (currentIndex< sprites[currentDimension].length-1)
    {
      currentIndex++;
      if (callback!=null && currentIndex == sprites[currentDimension].length-1)
      {
        callback.Callback(this);
      }     
    }
    else if (loop)
    { 
      currentIndex=0;
    }
  }
  
  // Name: public void changeDimension
  // Parameters: int newDimension
  // Description: updates the currenDimension value
  //
  public void changeDimension(int newDimension)
  {
    currentDimension = newDimension;
  }
  
  // Name: PImage getCurrentSprite
  // Parameters: -
  // Description: returns the current animation sprite
  //
  PImage getCurrentSprite()
  {
    return sprites[currentDimension][currentIndex];
  }
}