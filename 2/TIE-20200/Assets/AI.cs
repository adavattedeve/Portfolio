using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AI : MonoBehaviour {
	PlayerID playerID;
	List<Unit> ownUnits, opponentUnits;
	Grid grid;
	PathFinding pathFinding;

	void Awake(){
		grid = GetComponent<Grid> ();
		pathFinding = GetComponent<PathFinding> ();
	}
	public void PlayTurn(PlayerID _playerID){
		Debug.Log ("AI turn started");
		playerID = _playerID;
		StartCoroutine (Turn ());
	}
	private IEnumerator Turn(){
		yield return new WaitForEndOfFrame();
		ownUnits = CombatManager.instance.GetPlayer (playerID).troop.units;
		opponentUnits = CombatManager.instance.GetPlayer (playerID, true).troop.units;
		List<Node> validMovement;
		List<Node> validAttack;

		while (true) {
			Debug.Log ("ai turn loop");
			//Select unit
			Unit currentUnit= NextUnit();
			if (currentUnit==null){
				Debug.Log ("ending turn");
				break;
			}
//			CombatManager.instance.SelectedUnit = grid.UnitsNode(currentUnit);
			validMovement=CombatManager.instance.ValidMovementTargets(grid.UnitsNode(currentUnit));
			validAttack = CombatManager.instance.ValidAttackTargets(grid.UnitsNode(currentUnit));
			yield return new WaitForEndOfFrame();
			//Decide units action
			List<Unit> validAttackTargets = new List<Unit>();
			for (int i=0; i<validAttack.Count ; ++i){
				if (validAttack[i]!=null){
					validAttackTargets.Add(validAttack[i].Unit);
				}
			}
			List<Unit> priorityUnits = HighestPriorityUnits(validAttackTargets);
			Unit target = null;
			if (priorityUnits!=null && priorityUnits.Count>0){
				target = priorityUnits[0];
			}
			if (currentUnit.ranged && !CombatManager.instance.EnemyInNeighbour(grid.UnitsNode(currentUnit))){
				Debug.Log ("RangedAttack, AI");
				CombatManager.instance.Attack(currentUnit, target);
				currentUnit.actionPoints--;
			}else {
				if (target!=null){

					Node[] neighbours = grid.GetNeighbours(grid.UnitsNode(target));
					bool targetInNeighbour=false;
					//Check if target is neighbour
					for (int i=0; i<neighbours.Length; ++i){
						if (neighbours[i]!=null && neighbours[i].Unit==currentUnit){
							Debug.Log ("JustAttack, AI");
							currentUnit.actionPoints--;
							CombatManager.instance.Attack(currentUnit, target);
							targetInNeighbour=true;
							break;
						}
					}
					if (targetInNeighbour){
						yield return new WaitForEndOfFrame();
						continue;
					}
					//Move and attack
					Debug.Log ("Move & Attack, AI");
					for (int i=0; i<neighbours.Length; ++i){
						if (neighbours[i]!=null && validMovement.Contains(neighbours[i])){

							CombatManager.instance.MoveUnit(grid.UnitsNode(currentUnit), neighbours[i]);
							if (currentUnit.actionPoints>0){
								CombatManager.instance.Attack(currentUnit, target);
							}
							currentUnit.actionPoints--;
							break;
						}
					}
				}else{
					Debug.Log ("Move, AI");
					priorityUnits = HighestPriorityUnits(opponentUnits);
					target = null;
					for (int i=0; i<priorityUnits.Count; ++i){
						if (priorityUnits[i]!=null){
							Node[] path  = pathFinding.FindPath(currentUnit.unitController.transform.position, priorityUnits[i].unitController.transform.position);
							if (path!=null){
								target = priorityUnits[i];
								break;
							}
						}
					}
					if (target==null){
						currentUnit.actionPoints=0;
					}
					else {
						Debug.Log ("target position: " + target.unitController.transform.position.ToString());
						Node[] path  = pathFinding.FindPath(currentUnit.unitController.transform.position, target.unitController.transform.position);
						Debug.Log ("path Length: " + path.Length + "validMovement Count: " + validMovement.Count);
						for (int i=path.Length-1; i>=0; --i){
							if (validMovement.Contains(path[i])){
								currentUnit.actionPoints--;
								CombatManager.instance.MoveUnit(grid.UnitsNode(currentUnit), path[i]);
								break;
							}
						}
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}
		bool allUnitsInDefaultState = false;
		while (true) {
			yield return new WaitForSeconds(0.1f);
			for (int i=0; i<ownUnits.Count; ++i){
				if (ownUnits[i] !=null){
					Debug.Log (ownUnits[i].state.ToString () + "  " + ownUnits[i].name);
				}
				if (ownUnits[i] !=null && ownUnits[i].state != UnitState.DEFAULT){
					allUnitsInDefaultState = false;
					break;
				}
				allUnitsInDefaultState = true;
			}
			if (allUnitsInDefaultState && CombatManager.instance.TurnIsAbleToEnd()){
				EndTurn ();
				break;
			}
		}
	}
	private List<Unit> HighestPriorityUnits(List<Unit> units){
		//arbitary pririty float, ranged gets something, max damage/health something
		//int priorityIndex = -1;
		List<Unit> unitList = new List<Unit> ();
		List<float> priorities = new List<float>();
		if (units == null){
			return null;
		}
		for (int i=0; i< units.Count; ++i) {
			if (units[i]!=null && units[i].Visible){
				float priority = 1f;
				if (units[i].ranged){
					priority*=1.25f;
				}
				Debug.Log (units[i].stats.GetStat(StatType.DAMAGE).Value +"   "+ units[i].stats.GetStat(StatType.HEALTH).Value);
				priority*= (float)(units[i].stats.GetStat(StatType.DAMAGE).Value)/ units[i].stats.GetStat(StatType.HEALTH).Value;
				Debug.Log (priority);
				priorities.Add(priority);
			}else {
				priorities.Add (0);
			}
		}
		while (true) {
			float highest=0;
			int index=-1;
			for (int i=0; i<priorities.Count; ++i){
				if (priorities[i]>=highest){
					highest = priorities[i];
					index = i;
				}
			}
			if (index >=0){
				unitList.Add (units[index]);
				priorities.RemoveAt(index);
			}else {
				break;
			}
		}
		return unitList;
	}
	private Unit NextUnit(){
		for (int i=0; i< ownUnits.Count; ++i){
			if (ownUnits[i]!=null && ownUnits[i].ranged && ownUnits[i].actionPoints>0){
				return ownUnits[i];
			}
		}
		for (int i=0; i< ownUnits.Count; ++i){
			if (ownUnits[i]!=null && ownUnits[i].actionPoints>0){
				return ownUnits[i];
			}
		}
		return null;
	}
	private void EndTurn(){
		StopAllCoroutines ();
		CombatManager.instance.NextTurn ();
	}
}
