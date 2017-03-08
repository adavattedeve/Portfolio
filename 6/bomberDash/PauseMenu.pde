// Tie-21106_2016 / TIE21106-2016-G08
// Project assignment: BomberDash
// File: Object.pde
// Description: Class PauseMenu. This class implements the pause menu, when "ESCAPE" is pressed.
// File modified by: Francois Mazeau, Vilhelmiina

// TODO : find a clever way to implement several buttons with different onClick funcs.

// Class: PauseMenu
class PauseMenu
{  
  Button[] butArray;
  Button but1;
  
  //PauseMenu constructor
  // Parameters: -
  PauseMenu()
  {
    int h = height / 14;
    int w = width / 5;
    int x = 2 * width / 5;
    
    butArray = new Button[4];
    butArray[0] = new Resume(x, height / 4, h, w);
    butArray[1] = new Save(x, height / 6  + height / 4, h, w);
    butArray[2] = new Settings(x, 2 * height / 6  + height / 4, h, w);
    butArray[3] = new ExitBut(x, 3 * height / 6  + height / 4, h, w);
  }
  
  // Name: void update
  // Parameters: -
  // Description: updates the menu buttons
  void update()
  {
    for (Button but : butArray)
      but.update();
  }
}


//class: Button
class Button
{
  String name;
  PVector pos;
  PImage img_clear;
  PImage img_hovered;
  int Width;
  int Height;
  
  // Button constructor
  // Parameters: 
  // float x: x-coordinate
  // float y: y-coordinate
  // int h: height
  // int w: width
  Button(float x, float y, int h, int w)
  {
    init();
    pos = new PVector(x, y);
    Height = h;
    Width = w;
    img_clear = loadImage(dataPath(name));
    img_hovered = loadImage(dataPath(name));
    img_clear.resize(Width, Height);
    img_hovered.resize(Width, Height);
    img_hovered.filter(GRAY);
  }
  
  // Name: void init
  // Parameters: -
  // Description: initializes the button
  void init()
  {
    name = "resume_button.png";
  }

  // Name: void update
  // Parameters: -
  // Description: draws the correct button picture, hovered or not 
  void update()
  {
    PImage img;
    if (detectHover())
      img = img_hovered;
    else
      img = img_clear;
      
    image(img, pos.x, pos.y);
  }

  // Name: Boolean detectHover
  // Parameters: -
  // Description: detects if the button is hovered 
  Boolean detectHover()
  {
    return (mouseX > pos.x && mouseX < pos.x + Width
      && mouseY > pos.y && mouseY < pos.y + Height);
  }
  
  // Name: void onClick
  // Parameters: -
  // Description:  
  void onClick()
  {
    //state = GameState.PLAY; // tmp, will change
  }
}


// Class: Save
class Save extends Button
{
  // Save constructor
  // Parameters: 
  // float x: x-coordinate
  // float y: y-coordinate
  // int h: height
  // int w: width
  Save(float x, float y, int h, int w)
  {
    super(x, y, h, w);
  }
  
  // Name: void init
  // Parameters: -
  // Description: initializes the button
  void init()
  {
    name = "save_button.png";
  }
  
  // Name: void onClick
  // Parameters: -
  // Description: only saves the map for now
  void onClick()
  {
    map.save(); // only saves the map for now.
  }
}


//Class: Resume
class Resume extends Button
{
  // Resume constructor
  // Parameters: 
  // float x: x-coordinate
  // float y: y-coordinate
  // int h: height
  // int w: width
  Resume(float x, float y, int h, int w)
  {
    super(x, y, h, w);
  }
  
  // Name: void init
  // Parameters: -
  // Description: initializes the button
  void init()
  {
    name = "resume_button.png";
  }
  
  // Name: void onClick
  // Parameters: -
  // Description: changes the game state
  void onClick()
  {
    state = GameState.PLAY;
  }
}

//Class: ExitBut

class ExitBut extends Button
{
  // ExitBut constructor
  // Parameters: 
  // float x: x-coordinate
  // float y: y-coordinate
  // int h: height
  // int w: width
  
  ExitBut(float x, float y, int h, int w)
  {
    super(x, y, h, w);
  }
  
    
  // Name: void init
  // Parameters: -
  // Description: initializes the button
  void init()
  {
    name = "exit_button.png";
  }
  
  // Name: void onClick
  // Parameters: -
  // Description:  
  void onClick()
  {
    exit();
  }
}


//Class: Settings
class Settings extends Button
{
 
  // Settings constructor
  // Parameters: 
  // float x: x-coordinate
  // float y: y-coordinate
  // int h: height
  // int w: width
  Settings(float x, float y, int h, int w)
  {
    super(x, y, h, w);
  }
  
  // Name: void onClick
  // Parameters: -
  // Description: initializes the button
  void init()
  {
    name = "settings_button.png";
  }
  
  // Name: void onClick
  // Parameters: -
  // Description: 
  void onClick()
  {
    println("AND NOTHING HAPPENS MUAHAHAHA");
  }
}