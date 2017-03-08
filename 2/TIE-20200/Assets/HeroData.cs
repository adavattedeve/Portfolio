using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroData : ScriptableObject {
	public List<Hero> heroes;
	
	public Hero[] GetHeroes(){
		return heroes.ToArray();
	}
}
