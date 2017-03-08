// Tie-21106_2016 / TIE21106-2016-G08
// Project assignment: BomberDash
// File:
// Description: (description)
// File modified by: Atte, Vilhelmiina, Fracois

// Class: 
// Description: (description)

class VisualEffect implements AnimationEndedCallback
{
ArrayList<Animation> inActive;
ArrayList<Animation> active;
ArrayList<PVector> activePositions;

  VisualEffect()
  {
    inActive = new ArrayList();
    active = new ArrayList();
    activePositions = new ArrayList();
  }
  void update()
  {
    for (int i=0; i<active.size(); ++i)
    {      
      active.get(i).update(activePositions.get(i).x, activePositions.get(i).y);
    }
  }
  
  void startEffect(PVector position)
  {
    activePositions.add(position);
    if (inActive.size()==0)
    {      
      Animation newAnim = createNew();
      newAnim.StartAnimationWithCallback(this);
      active.add(newAnim);
    }
    else
    {
      Animation anim = inActive.get(0);
      anim.StartAnimationWithCallback(this);
      active.add(anim);
     
      inActive.remove(0);
    }
  }
  void reset()
  {
    while(active.size()>0)
    {
      inActive.add(active.get(0));
      active.remove(0);
      activePositions.remove(0);
    }
  }
  void endEffect(Animation animation)
  {
    for (int i=0; i<active.size(); ++i)
    {
      if (active.get(i) == animation)
      {
        inActive.add(active.get(i));
        active.remove(i);
        activePositions.remove(i);
        return;
      }
    }
  }
  
  //override this in class that extends VisualEffect
  protected Animation createNew()
  {
      return null;
  }
  void Callback(Animation animation)
  {
   endEffect(animation);
  }
}

interface AnimationEndedCallback
{
  void Callback(Animation animation);
}