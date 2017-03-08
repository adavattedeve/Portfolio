// Tie-21106_2016 / TIE21106-2016-G08
// Project assignment: BomberDash
// File: Enemy.pde
// Description: Parent class for enemies
// File modified by: Atte, Vilhelmiina

// Class: Enemy
class Enemy{
  PVector position;
  PVector velocity;
  PVector direction;
  PVector target;
  
  float normalSpeed;
  float agroSpeed;
  
  int framesForAfterAggro;
  int aggroTimer;
  boolean aggro;
  Collider collider;
  
  Animation currentAnimation;
  
 // Name: void update
 // Parameters: -
 // Description: updates the enemys properties and calls animation
  void update()
  {
    aggro = isAgroed();
    if (targetAchieved() && target!=null)
    {
      position.x = target.x;
      position.y = target.y;
    }
    refreshTarget();
    
    if (aggro){
      agroMovement();
      aggroTimer = framesForAfterAggro;
    }
    else{
      if (aggroTimer>0)
      {
        agroMovement();
        --aggroTimer;
      }
      else
      {
        movement();
        aggroTimer = 0;
      }  
    }
    if (collider.collides(player.collider))
    {
      player.die();
    }
    currentAnimation.update(position.x, position.y);
  }
  
 // Name: boolean isAgroed
 // Parameters: -
 // Description: tells if the enemy state is agro
  boolean isAgroed()
  {
    return false;
  }
  
 // Name: refreshTarget
 // Parameters: -
 // Description: refreshes the target
  void refreshTarget()
  {
  
  }
  
 // Name: void movement
 // Parameters: -
 // Description: movement for the enemy
  void movement()
  {
  
  }
 
 // Name: void agroMovement
 // Parameters: -
 // Description: agro movement for the enemy
  void agroMovement()
  {
  
  }
  
 // Name: void die
 // Parameters: -
 // Description: calls the death function
  void die()
  {
    effects.enemyDeath(position);
    enemies.remove(this);
  }
  
 // Name: boolean targetAchieved
 // Parameters: -
 // Description: tells if the target was achieved
  boolean targetAchieved()
  {
    if (target == null)
    {
      return true;
    }
    if (PVector.dist(target, position)<=agroSpeed)
    {
      return true;
    }
    return false;
  }
}