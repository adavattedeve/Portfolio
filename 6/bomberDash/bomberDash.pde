// Tie-21106_2016 / TIE21106-2016-G08
// Project assignment: BomberDash
// File: bomberDash.pde
// Description: Setting up the game, drawing the map,
// controlling the game and the player, printing the scores, 
// game states.
//
// File modified by: Francois, Vilhelmiina, Usman, Atte


import ddf.minim.*;

import javax.swing.*; //for user input


Map map; //Map for the game
Player player; //Player for the game
ImageLoader images; //For loading game files
PauseMenu menu; //Menu for the game
PVector cameraPosition; //For coordinating the camera
PVector cameraOffset; //For coordinating the camera
boolean[] keys; //
EffectsManager effects; //For visual and sound effects
Minim minim; //
ArrayList<Enemy> enemies; //The enemies of the game
int SCREEN_HEIGHT, SCREEN_WIDTH; //Screen size
String userName; // Player's name 
int blockSize; //
ArrayList<LevelData> levels; //Levels of the game
int currentLevel; //Current level

enum GameState {
  START, PLAY, END, MENU
}; //Possible game states
  GameState state = GameState.START; //Default state for the game


int scoreLength = 10; // Top 5 results
String filename = "scores.txt";


//int playerScore = 200; // just test 

// Name: void setup
// Parameters: -
// Description: sets up the game
//
void setup()
{
  surface.setResizable(true);
  fullScreen();
  
  background(0);
  minim = new Minim(this); 
  menu = new PauseMenu();
  keys = new boolean[128];  
  frameRate(60);
  blockSize = (width/18);
  if (blockSize%2 != 0)
  {
    ++blockSize;
  }
  images = new ImageLoader();
  effects = new EffectsManager();

  currentLevel=0;
  levels = createLevelDatas();
  newGame();
}

// Name: void newGame
// Parameters: -
// Description: starting a new game
//
void newGame()
{
  effects.reset();
  enemies = new ArrayList<Enemy>();
  cameraPosition = new PVector(0, 0);
  cameraOffset = new PVector(0, 0);  
  map = new Map(levels.get(0)); 
  player.bombs.add(new Bomb(1));
  player.bombs.add(new Bomb(1));
  player.bombs.add(new Bomb(1));
  player.bombs.add(new Bomb(1));
  for (int i=0; i<keys.length; ++i)
    keys[i] = false;

  state = GameState.START;
}

// Name: void draw
// Parameters: -
// Description: updates the map, player and the enemies.
// Action depends on the game state.
void draw()
{
  //width and height here in case the screen is resized
  SCREEN_WIDTH = width;
  SCREEN_HEIGHT = height;
  if (state == GameState.START) // start of the game
  {
    userName = javax.swing.JOptionPane.showInputDialog( "Please fill your username:");
    String hello = "Hello ";
    String story = "a long time ago in a galaxy far far away... blablabla story.";
    String text = hello + userName + "!\n" + story; 

    JFrame frame = new JFrame("");
    JOptionPane.showMessageDialog( frame, text, "BomberDash", 
      JOptionPane.INFORMATION_MESSAGE, new ImageIcon(dataPath("storyPic.png")));
    state = GameState.PLAY;
    //scores(); // for testing scores
    //showScore(); // for testing results
  } else if (state == GameState.PLAY) // game
  {
    updateCamera(cameraPosition);
    map.updateBackground();
    map.update();
    player.update();
    effects.update();
    for (int i = 0; i < enemies.size(); ++i)
    {
      enemies.get(i).update();
    }    
    player.displayScore();
  } else if (state == GameState.END) // end of the game
  {
    scores();
    showScore();
    newGame();
  }

  if (state == GameState.MENU)
    menu.update();
}


// Name: void scores
// Parameters: -
// Description: Saves the new score. 
// Creates a new file 'scores.txt' if it doesn't exist.
// Empty file is ok, no other validity check for file format.
// 
// Correct file format:
// first row: name, second row: score, etc
void scores()
{
  String[] data = new String[scoreLength]; //first name, second score, etc
  Boolean emptyFile = false;

  try
  {
    String[] test = loadStrings(filename);
    if (test != null && test.length > 0)
    {
      data = loadStrings(filename);
      //print("file exists");
      for (int i = 1; i < scoreLength; i+=2) //check scores
      {
        if (player.score > Integer.valueOf(data[i]))
        {

          for (int x = scoreLength-1; x > i; x -= 2)
          {
            data[x] = data[x-2];
            data[x-1] = data[x-3];
          } // move all


          data[i-1] = userName; 
          data[i] = String.valueOf(player.score); //save new score
          break;
        }
      }
    } else
    {
      emptyFile = true;
    }
  }

  catch(NullPointerException e)
  {
    emptyFile = true;
  }

  if (emptyFile)
  {
    //print("create new file");
    data[0] = userName;
    data[1] = String.valueOf(player.score);
    for (int i = 2; i < scoreLength; i+=2)
    {
      data[i] = "default name";
      data[i+1] = "0";
    }
  }

  saveStrings(filename, data);
}

// Name: void showScore
// Parameters: -
// Description: Saves the new score. 
// Creates a new file 'scores.txt' if it doesn't exist.
// Empty file is ok, no other validity check for file format.
//
void showScore()
{
  String[] data = new String[scoreLength]; //first name, second score, etc
  data = loadStrings(filename);

  String text = "Top 5:\n\n";
  int rank = 1;
  for (int i = 0; i < scoreLength; i += 2)
  {
    text += String.valueOf(rank) + ": " + data[i] + ": " + data[i+1] + "\n";
    ++rank;
  }
  JOptionPane.showMessageDialog(null, text);
}

// Name: void mousePressed
// Parameters: -
// Description: 
// Navigating menu.
// Using bombs.
//
void mousePressed()
{
  if (state == GameState.MENU)
  {
    for (Button but : menu.butArray)
    {
      if (but.detectHover())
        but.onClick();
    }
  }
}

// Name: void keyPressed
// Parameters: -
// Description: Controlling the game.
// Action depends on the game state.
void keyPressed()
{
  if (key == ESC)
  {
    key = 0;

    if (state == GameState.PLAY)
    {
      state = GameState.MENU;
      filter(BLUR, 2);
    } else
    {
      state = GameState.PLAY;
    }
  }

  if (key == ' ' && player.bombs.size() != 0)
    player.bombs.get(0).trigger();

  if (key < keys.length) 
    keys[key] = true;
}

// Name: void keyReleased
// Parameters: -
// Description: activated when a key is released
//
void keyReleased()
{
  if (key < keys.length)
    keys[key] = false;
}

// Name: void updateCamera
// Parameters:
// PVector cam: camera position
//
// Description: updating camera. 
//
void updateCamera(PVector cam)
{
  float x = player.position.x;
  float y = player.position.y;

  int w = map.mapWidth * map.block_size;
  int h = map.mapHeight * map.block_size;

  if (w>width)
  {
    if (x <= width / 2)
    {
      cam.x = 0;
    } else if (x >= w - width / 2)
    {
      cam.x = w - width;
    } else
    {
      cam.x = x - width/2;
    }
  } else
  {
    cam.x = map.mapWidth*map.block_size/2-width/2;
  }

  if (h>height)
  {
    if (y <= height / 2)
    {
      cam.y = 0;
    } else if (y >= h - height / 2)
    {
      cam.y = h - height;
    } else
    {
      cam.y = y - height/2;
    }
  } else
  {
    cam.y = map.mapHeight*map.block_size/2 - height/2;
  }
  cam.x +=  cameraOffset.x;
  cam.y +=  cameraOffset.y;
}

// Name: ArrayList<LevelData> createLevelDatas
// Parameters: -
// Description: Creates the data for the level.
//
ArrayList<LevelData> createLevelDatas()
{
  ArrayList<LevelData> levelDatas = new ArrayList<LevelData>();
  LevelData level = new LevelData();
  level.w = 20;
  level.h = 20;

  level.playerSpawn = new PVector(level.w/2 * blockSize, level.h / 2 * blockSize);

  level.startBasicEnemies = 2;
  level.basicEnemyChance =0.4;

  level.startFlyingEnemies = 2;
  level.flyingEnemyChance = 0.4;

  level.startBlockEatingEnemies = 1;
  level.blockEatingEnemyChance = 0.2;

  level.enemySpawnDeltaTime = 15000;
  level.minEnemySpawnTime = 10000;
  level.enemySpawnTimeReduction = 1000;

  levelDatas.add(level);


  return levelDatas;
}