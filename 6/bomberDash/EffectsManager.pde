// Tie-21106_2016 / TIE21106-2016-G08
// Project assignment: BomberDash
// File: EffectsManager.pde
// Description: Magaging the effects
// File modified by: Atte, Vilhelmiina,Usman

// Class: EffectsManager
class EffectsManager
{
  private AudioPlayer mainTrack;
  
  private AudioPlayer playerDeath;
  private AudioPlayer enemyDeath; 
  private AudioPlayer[] playerSteps;
  private AudioPlayer mine;
  
  private AudioPlayer mudTakeDamage;
  private AudioPlayer mudDestroyed;
  
  private AudioPlayer stoneTakeDamage;
  private AudioPlayer stoneDestroyed;
  
  private AudioPlayer heavyTakeDamage;
  private AudioPlayer heavyDestroyed;
  
  private AudioPlayer indestructibleTakeDamage;
  
  private AudioPlayer pointsGained;
  
  private AudioPlayer explosionSound;
  
  private AudioPlayer heavyCollisionSound;
  
  private VisualEffect[] visualEffects;
  private ExplosionEffect explosionEffect;
  private SmokeEffect smokeEffect;
  private BlockSmokeEffect blockSmokeEffect;
  private int cameraShakeTime;
  private float cameraShakeAmplitude;
  
  // EffectsManager constructor
  // Parameters: -
  EffectsManager()
  {
    cameraShakeTime = 0;
    cameraShakeAmplitude = 0;
    explosionEffect = new ExplosionEffect();
    smokeEffect = new SmokeEffect();
    blockSmokeEffect = new BlockSmokeEffect();
    
    visualEffects = new VisualEffect[3];
    visualEffects [0] = explosionEffect;   
    visualEffects [1] = smokeEffect; 
    visualEffects [2] = blockSmokeEffect; 
    
    mainTrack = minim.loadFile(dataPath("MainTrack.wav"), 2048);
    mainTrack.loop();
    playerDeath = minim.loadFile("PlayerDeath.wav", 512);
    enemyDeath = minim.loadFile("EnemyDeath.wav", 512);
    mine = minim.loadFile("Mine.wav", 512);
    mine.setGain(15);
    
    mudTakeDamage = minim.loadFile("MudTakeDamage.wav", 512);
    //mudDestroyed = minim.loadFile("MudDestroyed.wav", 512);
  
    stoneTakeDamage = minim.loadFile("StoneTakeDamage.wav", 512);
    stoneDestroyed = minim.loadFile("StoneDestroyed.wav", 512);
    
    //heavyTakeDamage = minim.loadFile("HeavyTakeDamage.wav", 512);
    //heavyDestroyed = minim.loadFile("HeavyDestroyed.wav", 512);
  
    indestructibleTakeDamage = minim.loadFile("IndestructibleTakeDamage.wav", 512);
    
    pointsGained =  minim.loadFile("PointsGained.wav", 512);
    pointsGained.setGain(-10);
    
    explosionSound = minim.loadFile("Explosion.wav", 512);
    
    heavyCollisionSound = minim.loadFile("HeavyCollision.wav", 512);
    
    
    int stepClipsFound=0;
    while (true)
    {
      if (minim.loadFile("Step_" + stepClipsFound +".wav", 512) == null)
      {
       break;
      }
      ++stepClipsFound;
    }
    playerSteps = new AudioPlayer[stepClipsFound];
    for (int i=0; i< stepClipsFound; ++i)
    {
      playerSteps[i] = minim.loadFile("Step_" + i +".wav", 512);
      playerSteps[i].setGain(50);
    }
    
    
  }
  
 // Name: void reset
 // Parameters: -
 //
 // Description: resets the visual effects
  void reset()
  {
    for (int i=0; i<visualEffects.length; ++i)
    {
      visualEffects[i].reset();
    }
  }
  
 // Name: void update
 // Parameters: -
 //
 // Description: manages the drawing and use of the visual effects
  void update()
  {
    for (int i=0; i< visualEffects.length; ++i)
    {
      visualEffects[i].update();
    }
    if (cameraShakeTime>0)
    {
      cameraOffset.x = random(-cameraShakeAmplitude, cameraShakeAmplitude);
      cameraOffset.y = random(-cameraShakeAmplitude, cameraShakeAmplitude);
      --cameraShakeTime;
    }
    else
    {
      cameraOffset.x = 0;
      cameraOffset.y = 0;
    }
  }
  
 // Name: void explosion
 // Parameters: 
 // PVector[] positions: positions
 //
 // Description: explosions, uses sound and camerashake effects
  void explosion(PVector[] positions)
  {
    explosionSound.play(0);
    cameraShake(30, 7);
    for (int i=0; i<positions.length; ++i)
    {
      explosionEffect.startEffect(positions[i]);
    }
    
  }
  
 // Name: void playerDeath
 // Parameters: -
 // Description: player takes a step
  void playerStep()
  {
    playerSteps[(int)random(0, playerSteps.length-1)].play(0);
  }
  
 // Name: void playerDeath
 // Parameters: -
 // Description: dying of the player
  void playerDeath()
  {
    playerDeath.play(0);
  }
  
 // Name: void enemyDeath
 // Parameters: 
 // PVector position: position
 // Description: dying of the enemy
  void enemyDeath(PVector position)
  {
    enemyDeath.play(0);
  }
  
 // Name: void blockTakeDamage
 // Parameters: 
 // Block block: the block that will be damaged
 // Description: damaging block effect
  void blockTakeDamage(Block block)
  {
    blockSmokeEffect.startEffect(new PVector(block.x, block.y) );
    
    if (block.getClass() == Stone.class )
    {
      stoneTakeDamage.play(0);
      return;
    }
    if (block.getClass() == Mud.class)
    {
      mudTakeDamage.play(0);
      return;
    }
    if (block.getClass() == Indestructible.class || block.getClass() == Heavy.class)
    {
      indestructibleTakeDamage.play(0);
      return;
    }

  }
  
 // Name: void heavyCollision
 // Parameters: 
 // float x: x-coordinate
 // float y: y-coordinate
 // Description: heavy collision effect
  void heavyCollision(float x, float y)
  {
    heavyCollisionSound.play(0);
    smokeEffect.startEffect(new PVector(x, y));
  }
  
 // Name: void blockDestroyed
 // Parameters: 
 // Block block: block that will be destroyed
 // Description: destroying effect, giving points
 void blockDestroyed(Block block)
 {
    if (block.getClass() == Stone.class || block.getClass() == Heavy.class)
    {
      
      stoneDestroyed.play(0);
      return;
    }
    if (block.getClass() == Points.class)
    {
      pointsGained.play(0);
      return;
    }
 }
 
 // Name: void mine
 // Parameters: -
 // Description: mining effect
 void mine()
 {
   mine.play(70);
 }
 
 // Name: void cameraShake
 // Parameters: 
 // int shakeLength: length for shaking
 // float amplitude: amplitude for shaking
 //
 // Description: shakes the camera
 void cameraShake(int shakeLength, float amplitude)
 {
   cameraShakeTime = shakeLength;
   cameraShakeAmplitude = amplitude;
 }
}