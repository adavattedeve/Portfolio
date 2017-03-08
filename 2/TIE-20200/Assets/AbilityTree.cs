using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class AbilityInBranch{
	public AbilityIdentifier ID;
	public bool learned;
	public int  tier;
	public AbilityInBranch(AbilityIdentifier _ID, bool _learned,  int  _tier){
		ID = _ID;
		learned = _learned;
		tier = _tier;
	}
}
[System.Serializable]
public class Branch{
	public int maxTier;
	public List<AbilityInBranch> branch;
	public List<AbilityInBranch> GetAbilitiesFromBranchAtTier(int targetTier){
		List<AbilityInBranch> abilities = new List<AbilityInBranch> ();
		for (int i=0; i< branch.Count; ++i) {
			if (targetTier == branch[i].tier){
				abilities.Add (branch[i]);
			}
		}
		return abilities;
	}
	public Branch(){
		branch = new List<AbilityInBranch> ();
	}
}
[System.Serializable]
public class AbilityTree {

	public List<Branch> tree;
	public AbilityTree(){
		tree = new List<Branch> ();
	}
	public void Learn(AbilityIdentifier id){
		if (IsAvailable(id)){
			for (int i=0; i<tree.Count; ++i) {
				for (int i2=0; i2<tree[i].branch.Count; ++i2) {
					if (tree[i].branch[i2].ID ==id){
						tree[i].branch[i2].learned=true;
					}
				}
			}
		}
	}
	public bool IsAvailable(AbilityIdentifier id){
		int branchIndex = -1;
		int abilityIndex = -1;
		for (int i=0; i<tree.Count; ++i) {
			for (int i2=0; i2<tree[i].branch.Count; ++i2) {
				if (tree[i].branch[i2].ID ==id){
					if (tree[i].branch[i2].learned){return false;}
					branchIndex =i;
					abilityIndex =i2;
					break;
				}
			}
			if (branchIndex!=-1){break;}
		}
		int targetTier = tree[branchIndex].branch[abilityIndex].tier;
		for (int i=0; i< tree[branchIndex].branch.Count; ++i) {
			if (tree[branchIndex].branch[i].tier==targetTier-1 && !tree[branchIndex].branch[i].learned){
				return false;
			}
		}
		return true;
	}
	public List<Spell> GetSpells(){
		List<Spell> spells = new List<Spell> ();
		for (int i=0; i<tree.Count; ++i) {
			for (int i2=0; i2<tree[i].branch.Count; ++i2) {
				if (tree[i].branch[i2].learned){
					spells.Add ((Spell)DataBase.instance.GetAbility(tree[i].branch[i2].ID));
				}
			}
		}
		return spells;
	}
	public AbilityTree GetDublicate(){
		AbilityTree returnObject = new AbilityTree();
		for (int i=0; i<tree.Count; ++i) {
			returnObject.tree.Add (new Branch());
			returnObject.tree[i].maxTier= tree[i].maxTier;
			for (int i2=0; i2<tree[i].branch.Count; ++i2) {
				returnObject.tree[i].branch.Add(new AbilityInBranch(tree[i].branch[i2].ID, tree[i].branch[i2].learned, tree[i].branch[i2].tier));

			}
		}
		return returnObject;
	}
}
