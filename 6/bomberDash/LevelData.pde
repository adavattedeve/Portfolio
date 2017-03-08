// Tie-21106_2016 / TIE21106-2016-G08
// Project assignment: BomberDash
// File: LevelData.pde
// Description: LevelData class
// File modified by: Vilhelmiina, Atte, Francois

// LevelData constructor
// Parameters: -
public class LevelData
{
  int h, w;
  PVector playerSpawn;
  
  int startBasicEnemies;
  int startFlyingEnemies;
  int startBlockEatingEnemies;

  float basicEnemyChance;
  float flyingEnemyChance;
  float blockEatingEnemyChance;
  
  int enemySpawnDeltaTime; //how many milliseconds between spawns
  int minEnemySpawnTime; // minimum value for enemySpawnDeltaTime
  int enemySpawnTimeReduction; //how many millis spawn time is after every spawn
  
}