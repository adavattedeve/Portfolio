using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
public enum Control{INPUT, AI};
public enum PlayerID{LEFT, RIGTH, NULL}
public enum TurnState{DEFAULT, SPELLSELECTED, UNITSELECTED}
public class CombatManager : MonoBehaviour {
	public struct Player{
		public PlayerID id;
		public Troop troop, losses;
		public Control control;
	}

	private Queue<CombatInfo> hitQueue; 
	//private AttackInfo attackInProcess;
	public Player player1, player2;
	public static CombatManager instance;
	private PlayerID turn;
	private TileMouseInput mouseInput;
	private TurnState turnState;

	private PathFinding pathFinding;
	private AI ai;
	private Spell selectedSpell;
	public Spell SelectedSpell{get{return selectedSpell;}
		set{

			if (value is ISpellMultiplePhasesComponent ){
				((ISpellMultiplePhasesComponent) value).Phase=0;
			}
			selectedSpell=value;

			if (selectedSpell!=null){
				turnState=TurnState.SPELLSELECTED;
				if (OnSelectedSpellChange!=null){
					OnSelectedSpellChange(value);
				}
			}
			else {
				NextUnit();
			}
		}}
	public delegate void SelectedSpellChangeAction(Spell spell);
	public  event SelectedSpellChangeAction OnSelectedSpellChange;

	private Node selectedUnit;
	public Node SelectedUnit{get{return selectedUnit;}
		set{
			if (value!=null && value.Unit!=null){
				Debug.Log ("Unit Selected: " +  value.Unit.name);
				turnState=TurnState.UNITSELECTED;
			}else {
				Debug.Log ("Unit Selected is null ");
				turnState=TurnState.DEFAULT;
			}
			selectedUnit=value;
			if (OnSelectedUnitChange!=null){
				OnSelectedUnitChange(value);
			}
			}
		}
	public delegate void SelectedUnitChangeAction(Node node);
	public  event SelectedUnitChangeAction OnSelectedUnitChange;

	public delegate void TurnChangeAction(Player player);
	public  event TurnChangeAction OnTurnChange;

	//public delegate void OnUnitStackDeathAction();
	//public  event OnUnitStackDeathAction OnUnitStackDeath;

	private Grid grid;

	private List<Node> selectedUnitsValidMovement;
//	private List<Node> validAttackTargets;
	private List<Node> additionalMouseoverUpScaledNodes;
	public bool processingAttackVisualization;
	void Awake(){
		if (instance == null) {
			instance = this;
			grid = GetComponent<Grid>();
			mouseInput = GetComponent<TileMouseInput>();
			mouseInput.enabled=false;
			ai = GetComponent<AI>();
			hitQueue = new Queue<CombatInfo>();
			additionalMouseoverUpScaledNodes=new List<Node>();
			pathFinding=GetComponent<PathFinding>();
		}

	}
	private void SelectedSpellChange(Spell spell){
		Node[] allNodes = grid.GetAllNodes ();
		for (int i=0; i<allNodes.Length; ++i) {
			allNodes [i].visualizationTile.SetEmission (false);
			allNodes [i].visualizationTile.ScaleDown();
		}
		switch (spell.targetinType) {
			case TargetingType.ALLENEMY:
				GetPlayer(turn).troop.hero.actionPoints--;
				CastSpell(spell, null);
				return;
			case TargetingType.ALLFRIENDLY:
				GetPlayer(turn).troop.hero.actionPoints--;
				CastSpell(spell, null);
				return;
		}
		List<Node> validCastTargets = ValidSpellTargets(spell.targetinType);
		for (int i=0; i<validCastTargets.Count; ++i) {
			validCastTargets[i].visualizationTile.SetEmission(true);
		}
	}
	public void CastSpell(Spell spell, Node target){
		SpellInfo spellInfo = new SpellInfo (spell, target);
		List<Node> targets=new List<Node>();
		switch (spell.targetinType) {
		case TargetingType.ALLENEMY:
			List<Unit> units = GetPlayer(turn, true).troop.units;
			for (int i=0; i<units.Count; ++i){
				if (units[i]!=null && units[i].amount>0){
					targets.Add (grid.UnitsNode(units[i]));
				}
			}
			break;
		case TargetingType.ALLFRIENDLY:
			units = GetPlayer(turn).troop.units;
			for (int i=0; i<units.Count; ++i){
				if (units[i]!=null && units[i].amount>0){
					targets.Add (grid.UnitsNode(units[i]));
				}
			}
			break;
		case TargetingType.TARGETEDAOE:
			targets.Add(target);
			Node[] neighbours = grid.GetNeighbours(target);
			for (int i=0; i<neighbours.Length; ++i){
				if (neighbours[i]!=null){
					targets.Add(neighbours[i]);
				}
			}
			break;
		default:
			targets.Add(target);
			break;
		}
		int intelligence = GetPlayer (turn).troop.hero.stats.GetStat (StatType.INTELLIGENCE).Value;
		for (int i=0; i<targets.Count; ++i) {
			spellInfo.hitInfos.Add(new HitInfo(targets[i], 0,0));
			if (targets[i].Unit!=null && spell is ISpellDamageComponent){
				spellInfo.hitInfos[i].damage = ((ISpellDamageComponent)spell).CalculateDamage(intelligence);
				spellInfo.hitInfos[i].deathCount = spellInfo.hitInfos[i].target.TakeDamage(spellInfo.hitInfos[i].damage);
				spellInfo.hitInfos[i].targetDead = CheckIfDead(spellInfo.hitInfos[i].target);
			}
		}
		if (spell is ISpellEffectComponent){
			((ISpellEffectComponent)spell).ApplyEffects(targets, intelligence);
		}
		GetPlayer (turn).troop.hero.stats.GetStat (StatType.MANA).AdditionalValue -= spell.stats.GetStat (StatType.MANA).Value;
		hitQueue.Enqueue ((CombatInfo)spellInfo);
		NextUnit ();
	}
	private void SelectedUnitChange(Node node){

		Node[] allNodes = grid.GetAllNodes ();
		for (int i=0; i<allNodes.Length; ++i) {
			allNodes [i].visualizationTile.SetEmission (false);
			allNodes [i].visualizationTile.ScaleDown();
		}
		if (node == null){
			return;
		}
		List<Node> validMovement = ValidMovementTargets(node);
		List<Node> validAttackTargets =  ValidAttackTargets(node);
		selectedUnitsValidMovement = validMovement;
		List<Unit> friendlyUnits = GetPlayer (turn).troop.units;
		for (int i=0; i<friendlyUnits.Count; ++i) {
			if (friendlyUnits[i]!=null && friendlyUnits[i].actionPoints>0){
				grid.UnitsNode(friendlyUnits[i]).visualizationTile.SetEmission(true);
			}
		}
		node.visualizationTile.ScaleUp ();
		for (int i=0; i<validMovement.Count; ++i){
			validMovement[i].visualizationTile.SetEmission(true);
		}

		for (int i=0; i<validAttackTargets.Count; ++i){
			if (validAttackTargets[i]!=null){
				validAttackTargets[i].visualizationTile.SetEmission(true);
			}
		}
	}
	public List<Node> ValidAttackTargets(Node from){
		List<Node> validAttackTargets = new List<Node> ();
		List<Node> validMovement = ValidMovementTargets (from);
		if (from.Unit.ableToAttack) {
			List<Unit> enemyUnits;
			if (turn == PlayerID.LEFT) {
				enemyUnits = GetPlayer (PlayerID.RIGTH).troop.units;
			} else {
				enemyUnits = GetPlayer (PlayerID.LEFT).troop.units;
			}
			if (from.Unit.ranged && !EnemyInNeighbour(from)) {
				for (int i=0; i<enemyUnits.Count; ++i) {
					if (enemyUnits[i]!=null && enemyUnits[i].Visible){
						validAttackTargets.Add (grid.UnitsNode (enemyUnits [i]));
					}
				}
			} else {
				bool found;
				Node[] neighbours;
				for (int i=0; i<enemyUnits.Count; ++i) {
					if (enemyUnits[i]==null || !enemyUnits[i].Visible){
						continue;
					}
					found = false;
					neighbours = grid.GetNeighbours (from);
					for (int i2=0; i2<neighbours.Length; ++i2) {
						if (neighbours [i2] == grid.UnitsNode (enemyUnits [i])) {
							validAttackTargets.Add (grid.UnitsNode (enemyUnits [i]));
							found = true;
							break;
						}
					}
					if (found) {
						continue;
					}
					neighbours = grid.GetNeighbours (grid.UnitsNode (enemyUnits [i]));
					for (int i2=0; i2<neighbours.Length; ++i2) {
						if (validMovement.Contains (neighbours [i2])) {
							validAttackTargets.Add (grid.UnitsNode (enemyUnits [i]));
							break;
						}
					}
				}
			}
		}
		Debug.Log (validAttackTargets.Count +" attack targets.");
		return validAttackTargets;
	}
	public List<Node> ValidSpellTargets(TargetingType targetingType){
		Node[] allNodes = grid.GetAllNodes ();
		List<Node> validCastTargets = new List<Node>();
		switch(targetingType){
		case TargetingType.ALLENEMY:
			return validCastTargets;
		case TargetingType.ALLFRIENDLY:
			return validCastTargets;
		case TargetingType.TARGETEDAOE:
			for (int i=0; i<allNodes.Length; ++i) {
				validCastTargets.Add(allNodes [i]);
			}
			return validCastTargets;
		case TargetingType.UNIT:
			List<Unit> units = GetPlayer (turn).troop.units;
			for (int i=0; i<units.Count; ++i) {
				if (units[i]!=null ){
					//grid.UnitsNode(units[i]).visualizationTile.SetEmission(true);
					validCastTargets.Add(grid.UnitsNode(units[i]));
				}
			}
			units = GetPlayer (turn, true).troop.units;
			for (int i=0; i<units.Count; ++i) {
				if (units[i]!=null && units[i].Visible ){
					validCastTargets.Add(grid.UnitsNode(units[i]));
				}
			}
			return validCastTargets;
		case TargetingType.UNOCCUPIEDNODE:
			for (int i=0; i<allNodes.Length; ++i) {
				if (allNodes[i].Unit==null || (allNodes[i].Unit!= null && !allNodes[i].Unit.Visible && !IsThisUnitsTurn(allNodes[i].Unit))){
					validCastTargets.Add(allNodes [i]);
				}
			}
			return validCastTargets;
		}
		return null;
	}
	public bool EnemyInNeighbour(Node from){
		Node[] neighbours = grid.GetNeighbours (from);
		for (int i=0; i< neighbours.Length; ++i) {
			if (neighbours[i]!=null && neighbours[i].Unit!=null && neighbours[i].Unit.owner!=from.Unit.owner){
				return true;
			}
		}
		return false;
	}
	public List<Node> ValidMovementTargets(Node from){
		if (from.Unit == null) {
			Debug.Log (from.gridX + "  " + from.gridY);
		}
		for (int i=0; i<from.Unit.TriggableAbilities.Count; ++i) {
			if (from.Unit.TriggableAbilities[i] is IOverrideMovement){
				return ((IOverrideMovement) from.Unit.TriggableAbilities[i]).OnCheckValidMovement(from);
			}
		}
		if (from.Unit.ableToMove) {
			return grid.GetValidMovement (from);
		}
		return null;
	}
	public void OnMouseOverTile(Vector3 position){
		Node node = grid.NodeFromWorldPoint (position);
		if (node == null)
			return;
		if (additionalMouseoverUpScaledNodes != null) {
			for (int i=0; i<additionalMouseoverUpScaledNodes.Count; ++i){
				additionalMouseoverUpScaledNodes[i].visualizationTile.ScaleDown();
			}
			additionalMouseoverUpScaledNodes.Clear();
		}
		node.visualizationTile.ScaleUp ();
		switch (turnState) {
			case TurnState.DEFAULT:
				if (node.Unit != null && GetPlayer (turn).troop.units.Contains (node.Unit)) {
					//own unit
				}
				break;
			case TurnState.SPELLSELECTED:
				List<Node> validSpellTargets = ValidSpellTargets(SelectedSpell.targetinType);
				if (validSpellTargets.Contains(node)){
					node.visualizationTile.ScaleUp();
					additionalMouseoverUpScaledNodes.Add(node);
					if (SelectedSpell.targetinType==TargetingType.TARGETEDAOE){
						Node[] neighbours = grid.GetNeighbours(node);
						for (int i=0; i< neighbours.Length; ++i){
							if (neighbours[i]!=null){
								neighbours[i].visualizationTile.ScaleUp();
								additionalMouseoverUpScaledNodes.Add(neighbours[i]);
							}
						}
					}
				}
				break;
			case TurnState.UNITSELECTED:
				if (node.Unit == null && selectedUnitsValidMovement!=null && selectedUnitsValidMovement.Contains(node)) {
					bool pathFound=false;
					Node[] path=null;
					for (int i=0; i< SelectedUnit.Unit.TriggableAbilities.Count; ++i){
						if (SelectedUnit.Unit.TriggableAbilities[i] is IOverrideMovement){
							path =  ((IOverrideMovement)SelectedUnit.Unit.TriggableAbilities[i]).FindPath(SelectedUnit.worldPosition , node.worldPosition, pathFinding);
							pathFound=true;
						}
					}
					if (!pathFound) {
						path = pathFinding.FindPath(SelectedUnit.worldPosition, node.worldPosition);
					}
					
					for (int i=0; i<path.Length; ++i){
						path[i].visualizationTile.ScaleUp();
						additionalMouseoverUpScaledNodes.Add(path[i]);
					}
				} else if (node.Unit!=null && !GetPlayer (turn).troop.units.Contains (node.Unit)) {
					if (!(selectedUnit.Unit.ranged && !EnemyInNeighbour(selectedUnit)) && selectedUnit.Unit.ableToAttack){
						mouseInput.OnMouseStayOnTile += MouseStayOnTile;
					}
				}
				break;
		}
	}
	public void HandeMouseRigthClickTile(Vector3 position){
		Node node = grid.NodeFromWorldPoint (position);
		if (node.Unit != null) {
			GuiManager.instance.ShowEntityInfo ((Entity)node.Unit, new Vector2(Input.mousePosition.x, Input.mousePosition.y));
		}
	}
	public void MouseStayOnTile(Vector3 position){
		Node node = grid.NodeFromWorldPoint (position);
		if (node == null)
			return;
		if (additionalMouseoverUpScaledNodes != null) {
			for (int i=0; i<additionalMouseoverUpScaledNodes.Count; ++i){
				additionalMouseoverUpScaledNodes[i].visualizationTile.ScaleDown();
			}
			additionalMouseoverUpScaledNodes.Clear();
		}
		if (SelectedUnit == null)
			return;
		Node moveTo = grid.GetNearestNeighbour (node, position);
		if (selectedUnitsValidMovement!=null && selectedUnitsValidMovement.Contains(moveTo)){
			Node[] path=null;
			bool pathFound =false;
			for (int i=0; i< SelectedUnit.Unit.TriggableAbilities.Count; ++i){
				if (SelectedUnit.Unit.TriggableAbilities[i] is IOverrideMovement){
					path =  ((IOverrideMovement)SelectedUnit.Unit.TriggableAbilities[i]).FindPath(SelectedUnit.worldPosition , moveTo.worldPosition, pathFinding);
					pathFound=true;
				}
			}
			if (!pathFound){
				path = pathFinding.FindPath(SelectedUnit.worldPosition, moveTo.worldPosition);
			}
			for (int i=0; i<path.Length; ++i){
				path[i].visualizationTile.ScaleUp();
				additionalMouseoverUpScaledNodes.Add(path[i]);
			}
			moveTo.visualizationTile.ScaleUp ();
			additionalMouseoverUpScaledNodes.Add(moveTo);
		}
	}
	public void OnMouseExitTile(Node lastNode){
		if (lastNode != SelectedUnit && ! additionalMouseoverUpScaledNodes.Contains(lastNode)) {
			lastNode.visualizationTile.ScaleDown ();
		}
	}
	public void OnTileSelected(Vector3 position){

		Node node = grid.NodeFromWorldPoint (position);
		if (node == null)
			return;
		switch (turnState) {
		case TurnState.DEFAULT:
			if (node.Unit!=null && GetPlayer(turn).troop.units.Contains(node.Unit) && node.Unit.actionPoints>0){
				SelectedUnit=node;
			}
			break;
		case TurnState.SPELLSELECTED:
			if (ValidSpellTargets(SelectedSpell.targetinType).Contains(node)){
				ISpellMultiplePhasesComponent temp=null;
				if (SelectedSpell is ISpellMultiplePhasesComponent){
					temp = (ISpellMultiplePhasesComponent) SelectedSpell;
				}
				if (temp!=null && temp.Phase<SelectedSpell.targetingTypes.Length-1){

					temp.PhaseTargets[temp.Phase] =node;
					temp.Phase++;
					OnSelectedSpellChange(SelectedSpell);
				}
				else {
					Debug.Log ("Casting spell: " + SelectedSpell.name);
					GetPlayer(turn).troop.hero.actionPoints--;
					CastSpell(SelectedSpell, node);
				}
			}
			break;
		case TurnState.UNITSELECTED:
			if (node.Unit==null && selectedUnitsValidMovement.Contains(node)){
				MoveUnit(SelectedUnit, node);
				SelectedUnit.Unit.actionPoints--;
			}else if (!IsThisUnitsTurn(node.Unit) &&selectedUnit.Unit.ableToAttack){
				if (SelectedUnit.Unit.ranged && !EnemyInNeighbour(SelectedUnit)){
					Attack(SelectedUnit.Unit, node.Unit);
					SelectedUnit.Unit.actionPoints--;
				}else{
					Node moveTo = grid.GetNearestNeighbour(node, position);
					if (selectedUnitsValidMovement.Contains(moveTo)){
						MoveUnit(SelectedUnit, moveTo);
						if (SelectedUnit.Unit.actionPoints>0){
							Attack(moveTo.Unit, node.Unit);
						}
						if (SelectedUnit.Unit!=null){
							SelectedUnit.Unit.actionPoints--;
						}
					}else if (moveTo == SelectedUnit){
						Attack(moveTo.Unit, node.Unit);
						SelectedUnit.Unit.actionPoints--;
					}
					}
			}else if (node.Unit!=null && GetPlayer(turn).troop.units.Contains(node.Unit) && node.Unit.actionPoints>0){
				SelectedUnit=node;
			}
			break;
		}
		if (SelectedUnit != null) {
			if (SelectedUnit.Unit==null){
				NextUnit();
			}else if (SelectedUnit.Unit.actionPoints <= 0){
				NextUnit();
			}
		}
	}
	private void NextUnit (){
		List<Unit> units = GetPlayer(turn).troop.units;
		for (int i=0; i< units.Count; ++i) {
			if (units[i]!=null && units[i].actionPoints>0){
				SelectedUnit = grid.UnitsNode(units[i]);
				return;
			}
		}
		SelectedUnit = null;
	}
	public void Attack(Unit attacker,  Unit defender){
		if (attacker == null || defender == null) {
			return;
		}
		Debug.Log ("Attack: " + attacker.name + "   " + defender.name);
		AttackInfo attackInfo = new AttackInfo ();
		attackInfo.attack = new HitInfo(grid.UnitsNode(defender), 0, 0);
		attackInfo.retalition =  new HitInfo(grid.UnitsNode(attacker), 0, 0);
		attackInfo.isRetalitiated = true;
		Stat damageStat = attacker.stats.GetStat (StatType.DAMAGE);
		int offence = attacker.stats.GetStat (StatType.OFFENCE).Value;
		int defence = defender.stats.GetStat (StatType.DEFENCE).Value;
		int luck = attacker.stats.GetStat (StatType.LUCK).Value;
		Hero attackerHero = GetPlayer (attacker.owner).troop.hero;
		Hero defenderHero = GetPlayer (defender.owner).troop.hero;
		if (attackerHero != null) {
			offence+=attackerHero.stats.GetStat(StatType.OFFENCE).Value;
		}
		if (defenderHero != null) {
			defence+=defenderHero.stats.GetStat(StatType.DEFENCE).Value;
		}
		float random = UnityEngine.Random.Range (damageStat.Value, damageStat.AdditionalValue);

		attackInfo.attack.damage = (int)(random*attacker.amount);
		if (UnityEngine.Random.Range (0f, 1f) <= (float)(luck) / 10f) {
			attackInfo.attack.damage *=2;
		}
		for (int i=0; i<attacker.TriggableAbilities.Count; ++i) {
			if (attacker.TriggableAbilities[i] is IOnAttackTrigger){
				IOnAttackTrigger attackTrigger = (IOnAttackTrigger)attacker.TriggableAbilities[i];
				attackTrigger.OnAttack(attackInfo);
			}
		}
		attackInfo.attack.damage = (int)(attackInfo.attack.damage * Mathf.Clamp(1 + ((offence - defence) *DataBase.instance.gameData.offenceDefenceMpl), 0.25f, 2f));
			
		int distanceBetween = grid.DistanceBetween (grid.UnitsNode(attacker),grid.UnitsNode(defender) );
		if (distanceBetween >= DataBase.instance.gameData.rangePenaltyRange) {
			attackInfo.attack.damage = (int)(attackInfo.attack.damage*DataBase.instance.gameData.rangePenaltyMpl);
		}
		for (int i=0; i<attackInfo.additionalTargets.Count; ++i) {
			defence = attackInfo.additionalTargets[i].target.stats.GetStat (StatType.DEFENCE).Value;
			defenderHero = GetPlayer (attackInfo.additionalTargets[i].target.owner).troop.hero;
			if (defenderHero != null) {
				defence+=defenderHero.stats.GetStat(StatType.DEFENCE).Value;
			}
			random = UnityEngine.Random.Range (damageStat.Value, damageStat.AdditionalValue);
			attackInfo.additionalTargets[i].damage = (int)((1+((offence-defence)*
			                              DataBase.instance.gameData.offenceDefenceMpl))*random*attacker.amount);
			
			distanceBetween = grid.DistanceBetween (grid.UnitsNode(attacker),grid.UnitsNode(defender) );
			if (distanceBetween >= DataBase.instance.gameData.rangePenaltyRange) {
				attackInfo.additionalTargets[i].damage = (int)(attackInfo.additionalTargets[i].damage*DataBase.instance.gameData.rangePenaltyMpl);
			}
			attackInfo.additionalTargets[i].deathCount = attackInfo.additionalTargets[i].target.TakeDamage(attackInfo.additionalTargets[i].damage);
			attackInfo.additionalTargets[i].targetDead = CheckIfDead(attackInfo.additionalTargets[i].target);
		}
		for (int i=0; i<defender.TriggableAbilities.Count; ++i) {
			if (defender.TriggableAbilities [i] is IOnDefendTrigger) {
				IOnDefendTrigger defendTrigger = (IOnDefendTrigger)defender.TriggableAbilities [i];
				defendTrigger.OnDefend (attackInfo);
			}
		}
		//isRetalitiated init value true, so if it isn't changed by abilities || effects then calculate it
		attackInfo.isMelee = grid.IsNeighbours(grid.UnitsNode(attacker), grid.UnitsNode(defender));
		if (attackInfo.isRetalitiated) {
			attackInfo.isRetalitiated = attackInfo.isMelee && defender.ableToRetaliate;
		}
		attackInfo.attack.deathCount=defender.TakeDamage (attackInfo.attack.damage);
		//if unit stack dead, set it null
		attackInfo.attack.targetDead=CheckIfDead (defender);
		//Retalition
		if (attackInfo.attack.targetDead){
			attackInfo.isRetalitiated=false;
			if (attackInfo.movementVisualizationAfterAttack!=null){
				attackInfo.movementVisualizationAfterAttack.Unit=null;
				attackInfo.movementVisualizationAfterAttack=null;
			}
		}
		else{
			for (int i=0; i<defender.TriggableAbilities.Count; ++i) {
				if (defender.TriggableAbilities[i] is IOnTakeDamage){
					IOnTakeDamage takeDamageTrigger = (IOnTakeDamage)defender.TriggableAbilities[i];
					takeDamageTrigger.OnTakeDamage(defender);
				}
			}
			if (attackInfo.isRetalitiated && attackInfo.isMelee && defender.ableToRetaliate){
				defender.ableToRetaliate = false;
				damageStat = defender.stats.GetStat (StatType.DAMAGE);
				offence = defender.stats.GetStat (StatType.OFFENCE).Value;
				defence = attacker.stats.GetStat (StatType.DEFENCE).Value;
				luck = defender.stats.GetStat (StatType.LUCK).Value;
				if (attackerHero != null) {
					defence+=attackerHero.stats.GetStat(StatType.DEFENCE).Value;
				}
				if (defenderHero != null) {
					offence+=defenderHero.stats.GetStat(StatType.OFFENCE).Value;
				}
				random = UnityEngine.Random.Range (damageStat.Value, damageStat.AdditionalValue);
				attackInfo.retalition.damage = (int)(random * defender.amount);
				if (UnityEngine.Random.Range (0f, 1f) <= (float)(luck) / 10f) {
					attackInfo.retalition.damage *=2;
				}
				for (int i=0; i<defender.TriggableAbilities.Count; ++i) {
					if (defender.TriggableAbilities [i] is IOnAttackTrigger) {
						IOnAttackTrigger attackTrigger = (IOnAttackTrigger)defender.TriggableAbilities [i];
						attackTrigger.OnRetalitionAttack (attackInfo);
					}
				}
				attackInfo.retalition.damage = (int)(attackInfo.retalition.damage* Mathf.Clamp(1 + ((offence - defence) *DataBase.instance.gameData.offenceDefenceMpl), 0.25f, 2f));

				for (int i=0; i<attackInfo.retalitionAdditionalTargets.Count; ++i) {
					defence = attackInfo.retalitionAdditionalTargets[i].target.stats.GetStat (StatType.DEFENCE).Value;
					defenderHero = GetPlayer (attackInfo.retalitionAdditionalTargets[i].target.owner).troop.hero;
					if (defenderHero != null) {
						defence+=defenderHero.stats.GetStat(StatType.DEFENCE).Value;
					}
					random = UnityEngine.Random.Range (damageStat.Value, damageStat.AdditionalValue);
					attackInfo.retalitionAdditionalTargets[i].damage = (int)((1+((offence-defence)*
					                                                   DataBase.instance.gameData.offenceDefenceMpl))*random*attacker.amount);

					attackInfo.retalitionAdditionalTargets[i].deathCount = attackInfo.retalitionAdditionalTargets[i].target.TakeDamage(attackInfo.retalitionAdditionalTargets[i].damage);
					attackInfo.retalitionAdditionalTargets[i].targetDead = CheckIfDead(attackInfo.retalitionAdditionalTargets[i].target);
				}

				for (int i=0; i<defender.TriggableAbilities.Count; ++i) {
					if (defender.TriggableAbilities [i] is IOnDefendTrigger) {
						IOnDefendTrigger defendTrigger = (IOnDefendTrigger)defender.TriggableAbilities [i];
						defendTrigger.OnDefend (attackInfo);
					}
				}

				attackInfo.retalition.deathCount = attacker.TakeDamage (attackInfo.retalition.damage);

				if (CheckIfDead (attacker)) {
					attackInfo.retalition.targetDead=true;
				}
				if (!attackInfo.retalition.targetDead){
					for (int i=0; i<attacker.TriggableAbilities.Count; ++i) {
						if (attacker.TriggableAbilities[i] is IOnTakeDamage){
							IOnTakeDamage takeDamageTrigger = (IOnTakeDamage)attacker.TriggableAbilities[i];
							takeDamageTrigger.OnTakeDamage(attacker);
						}
					}
				}
			}
		}

		hitQueue.Enqueue((CombatInfo)attackInfo);
		for (int i=0; i<attacker.TriggableAbilities.Count; ++i) {
			if (attacker.TriggableAbilities[i] is IOnAfterAttacks){
				((IOnAfterAttacks)attacker.TriggableAbilities[i]).OnAfterAttacks(attackInfo);
			}
		}
	}
	IEnumerator ProcessAttackVisuals(){
		while (true) {
			if (hitQueue.Count > 0){
				//Damage from physical attack
				if (hitQueue.Peek () is AttackInfo){
					Debug.Log ("processing attackInfo!");
					AttackInfo attackInfo = (AttackInfo)hitQueue.Peek ();
					while (true) {
						yield return new WaitForEndOfFrame ();
						if (attackInfo.retalition.target.state == UnitState.DEFAULT && attackInfo.attack.target.state == UnitState.DEFAULT) {
							break;
						}
					}
						processingAttackVisualization=true;
						attackInfo.retalition.target.RotateTowards(attackInfo.attack.target);
						attackInfo.attack.target.RotateTowards(attackInfo.retalition.target);
						Debug.Log ("Processing: rotations");
						while (true) {
							yield return new WaitForEndOfFrame ();
							if (attackInfo.retalition.target.state == UnitState.DEFAULT && attackInfo.attack.target.state == UnitState.DEFAULT) {
								break;
							}
						}
						Debug.Log ("Processing: attack");
						attackInfo.retalition.target.VisualizeAttack (grid.UnitsNode(attackInfo.attack.target));
						while (true) {
							yield return new WaitForEndOfFrame ();
							if (attackInfo.retalition.target.state == UnitState.DEFAULT) {
								break;
							}
						}
						Debug.Log ("Processing: takedamage");
						attackInfo.attack.target.VisualizeTakeDamage (attackInfo.attack.damage, attackInfo.attack.deathCount, attackInfo.attack.targetDead);
						for (int i=0; i<attackInfo.additionalTargets.Count; ++i) {
							attackInfo.additionalTargets[i].target.VisualizeTakeDamage(attackInfo.additionalTargets[i].damage, attackInfo.additionalTargets[i].deathCount, attackInfo.additionalTargets[i].targetDead);
						}
						while (true) {
							yield return new WaitForEndOfFrame ();
							if (attackInfo.attack.target.state == UnitState.DEFAULT) {
								break;
							}
						}

						if (attackInfo.isRetalitiated) {
							attackInfo.attack.target.VisualizeAttack (grid.UnitsNode(attackInfo.retalition.target));
							Debug.Log ("Processing: retalition");
							while (true) {
								yield return new WaitForEndOfFrame ();
								if (attackInfo.attack.target.state == UnitState.DEFAULT) {
									break;
								}
							}
							Debug.Log ("Processing: retalition takedamage");
							attackInfo.retalition.target.VisualizeTakeDamage (attackInfo.retalition.damage, attackInfo.retalition.deathCount, attackInfo.retalition.targetDead);
							for (int i=0; i<attackInfo.retalitionAdditionalTargets.Count; ++i) {
								attackInfo.retalitionAdditionalTargets[i].target.VisualizeTakeDamage(attackInfo.retalitionAdditionalTargets[i].damage,
							                                                                     attackInfo.retalitionAdditionalTargets[i].deathCount, attackInfo.retalitionAdditionalTargets[i].targetDead);
							}
							while (true) {
								yield return new WaitForEndOfFrame ();
								if (attackInfo.retalition.target.state == UnitState.DEFAULT) {
									break;
								}
							}

						}
						Debug.Log ("Processing: back to default rotations");
						if (!attackInfo.attack.targetDead){
							attackInfo.attack.target.RotateTowards();
						}
						if (!attackInfo.retalition.targetDead){
							attackInfo.retalition.target.RotateTowards();
						}

						while (true) {
							yield return new WaitForEndOfFrame ();
							if (attackInfo.retalition.target.state == UnitState.DEFAULT && attackInfo.attack.target.state == UnitState.DEFAULT) {
								break;
							}
						}
						if (attackInfo.movementVisualizationAfterAttack!=null){
							MovementVisualization(attackInfo.movementVisualizationAfterAttack.Unit, attackInfo.movementVisualizationAfterAttack);
						}
						processingAttackVisualization=false;
						hitQueue.Dequeue ();
						CheckIfBattleHasEnded();
						
				}
				//Damage from spell
				else if (hitQueue.Peek () is SpellInfo && ((SpellInfo)hitQueue.Peek ()).spell!=null){
					processingAttackVisualization=true;
					SpellInfo spellInfo = (SpellInfo)hitQueue.Peek ();
					Hero hero = GetPlayer(turn).troop.hero;
					bool ready=false;
					Debug.Log ("wait till target anims ready");
					while (!ready) {
						yield return new WaitForEndOfFrame ();
						for (int i=0; i<spellInfo.hitInfos.Count; ++i){
							if (spellInfo.hitInfos[i].target!=null && spellInfo.hitInfos[i].target.state == UnitState.DEFAULT) {
								ready=true;
								continue;
							}else if(spellInfo.hitInfos[i].target==null ){
								ready=true;
								continue;
							}
							ready=false;
						}
					}
					hero.VisualizeSpellCast(spellInfo.spell, spellInfo.target);
					Debug.Log ("wait hero anim");
					while (true) {
						yield return new WaitForEndOfFrame ();
						if (hero.state == HeroState.DEFAULT) {
							break;
						}
					}
					if (spellInfo.spell is ISpellEffectComponent){
						for (int i=0; i<spellInfo.hitInfos.Count; ++i){
							((ISpellEffectComponent)spellInfo.spell).EffectVisualization (grid.UnitsNode(spellInfo.hitInfos[i].target));
						}
					}
					if (spellInfo.spell is ISpellDamageComponent){
						for (int i=0; i<spellInfo.hitInfos.Count; ++i){
							if (spellInfo.hitInfos[i].target!=null){
								spellInfo.hitInfos[i].target.VisualizeTakeDamage (spellInfo.hitInfos[i].damage, spellInfo.hitInfos[i].deathCount, spellInfo.hitInfos[i].targetDead);
							}
						}
					}

					ready=false;
					Debug.Log ("wait till target anims ready");
					while (!ready) {
						yield return new WaitForEndOfFrame ();
						for (int i=0; i<spellInfo.hitInfos.Count; ++i){
							if (spellInfo.hitInfos[i].target!=null && spellInfo.hitInfos[i].target.state == UnitState.DEFAULT) {
								ready=true;
								continue;
							}else if(spellInfo.hitInfos[i].target==null ){
								ready=true;
								continue;
							}
							ready=false;
						}
					}
					processingAttackVisualization=false;
					hitQueue.Dequeue ();
					CheckIfBattleHasEnded();

				}
				//Damage from neutral source, from effect, from tile etc
				else {
					processingAttackVisualization=true;
					SpellInfo spellInfo = (SpellInfo)hitQueue.Peek ();

					bool ready=false;
					Debug.Log ("wait till target anims ready");
					while (!ready) {
						yield return new WaitForEndOfFrame ();
						for (int i=0; i<spellInfo.hitInfos.Count; ++i){
							if (spellInfo.hitInfos[i].target!=null && spellInfo.hitInfos[i].target.state == UnitState.DEFAULT) {
								ready=true;
								continue;
							}else if(spellInfo.hitInfos[i].target==null ){
								ready=true;
								continue;
							}
							ready=false;
						}
					}

					for (int i=0; i<spellInfo.hitInfos.Count; ++i){
						if (spellInfo.hitInfos[i].target!=null){
							spellInfo.hitInfos[i].target.VisualizeTakeDamage (spellInfo.hitInfos[i].damage, spellInfo.hitInfos[i].deathCount, spellInfo.hitInfos[i].targetDead);
						}
					}
					ready=false;
					Debug.Log ("wait till target anims ready");
					while (!ready) {
						yield return new WaitForEndOfFrame ();
						for (int i=0; i<spellInfo.hitInfos.Count; ++i){
							if (spellInfo.hitInfos[i].target!=null && spellInfo.hitInfos[i].target.state == UnitState.DEFAULT) {
								ready=true;
								continue;
							}else if(spellInfo.hitInfos[i].target==null ){
								ready=true;
								continue;
							}
							ready=false;
						}
					}

					processingAttackVisualization=false;
					hitQueue.Dequeue ();
					CheckIfBattleHasEnded();

				}

			}
			yield return new WaitForEndOfFrame();
		}
	}
	public void DamageFromNeutralSource(Node target, int damage, TargetingType targetingType = TargetingType.UNIT){
		if (target.Unit == null) {
			return;
		}
		SpellInfo spellInfo = new SpellInfo (null, target);
		List<Node> targets=new List<Node>();
		switch (targetingType) {
		case TargetingType.ALLENEMY:
			List<Unit> units = GetPlayer(turn, true).troop.units;
			for (int i=0; i<units.Count; ++i){
				if (units[i]!=null){
					targets.Add (grid.UnitsNode(units[i]));
				}
			}
			break;
		case TargetingType.ALLFRIENDLY:
			units = GetPlayer(turn).troop.units;
			for (int i=0; i<units.Count; ++i){
				if (units[i]!=null){
					targets.Add (grid.UnitsNode(units[i]));
				}
			}
			break;
		case TargetingType.TARGETEDAOE:
			targets.Add(target);
			Node[] neighbours = grid.GetNeighbours(target);
			for (int i=0; i<neighbours.Length; ++i){
				if (neighbours[i]!=null){
					targets.Add(neighbours[i]);
				}
			}
			break;
		default:
			targets.Add(target);
			break;
		}
		for (int i=0; i<targets.Count; ++i) {
			spellInfo.hitInfos.Add(new HitInfo(targets[i], 0,0));
			if (targets[i].Unit!=null){
				spellInfo.hitInfos[i].damage = damage;
				spellInfo.hitInfos[i].deathCount = spellInfo.hitInfos[i].target.TakeDamage(spellInfo.hitInfos[i].damage);
				spellInfo.hitInfos[i].targetDead = CheckIfDead(spellInfo.hitInfos[i].target);
			}
		}
		hitQueue.Enqueue ((CombatInfo)spellInfo);
	}
	public bool TurnIsAbleToEnd(){
		if (hitQueue.Count > 0 || processingAttackVisualization) {
			return false;
		}
		List<Unit> units = GetPlayer(turn).troop.units;
		for (int i=0; i<units.Count; ++i) {
			if (units[i]!=null && units [i].state != UnitState.DEFAULT) {
				return false;
			}
		}
		return true;
	}
	private void CheckIfBattleHasEnded(){
		if (hitQueue.Count > 0) {
			return;
		}
		List<Unit> units = player1.troop.units;
		bool unitsAlive = false;
		for (int i=0; i<units.Count; ++i) {
			if (units[i]!=null){
				unitsAlive=true;
				break;
			}
		}
		if (!unitsAlive) {
			EndBattle();
		}
		units = player2.troop.units;
		unitsAlive = false;
		for (int i=0; i<units.Count; ++i) {
			if (units[i]!=null){
				unitsAlive=true;
				break;
			}
		}
		if (!unitsAlive) {
			EndBattle();
		}
	}
	private bool CheckIfDead(Unit unit){
		Debug.Log ("check if dead");
		if (unit.amount <= 0) {
			Player player = GetPlayer(unit.owner);
			List<Unit> units = player.troop.units;
			for (int i=0; i<units.Count; ++i){
				if (units[i]==unit){
					Debug.Log ("Unit is dead");
					player.troop.units[i]=null;
					break;
				}
			}
			grid.UnitsNode (unit).Unit = null;
//			if (OnUnitStackDeath!=null){
//				OnUnitStackDeath();
//			}
			return true;
		}
		return false;
	}
	public void MoveUnit(Node from, Node to, bool visualize=true){
		//If invisible unit is at node where unit tries to move, then move next to it and attack invisible unit
		if (to.Unit != null) {
			Node moveToTarget=to;
			Node attackTarget =null;
			while (moveToTarget.Unit!=null){
				Debug.Log ("tried to walk on top of invisible unit!");
				attackTarget=moveToTarget;
				int xDir = Mathf.Clamp (from.gridX - moveToTarget.gridX, -1, 1);
				int yDir = Mathf.Clamp (from.gridY - moveToTarget.gridY, -1, 1);
				int posX =  Mathf.Clamp (moveToTarget.gridX+xDir , 0, grid.gridSizeX-1);
				int posY =  Mathf.Clamp (moveToTarget.gridY+yDir , 0, grid.gridSizeY-1);

				moveToTarget = grid.GetNode (posX, posY);
			}
			from.Unit.actionPoints--;
			Debug.Log ("move next to inv");
			MoveUnit(from, moveToTarget);
			Debug.Log ("attack to inv");
			Debug.Log (attackTarget.Unit.name);
			Attack(moveToTarget.Unit, attackTarget.Unit);
			return;
		}
		if (visualize) {
			for (int i=0; i<from.Unit.TriggableAbilities.Count; ++i) {
				if (from.Unit.TriggableAbilities [i] is IOnStartMovement) {
					((IOnStartMovement)from.Unit.TriggableAbilities [i]).OnStartMovement (from.Unit);
				}
			}
		}

		to.Unit = from.Unit;
		from.Unit = null;
		if (visualize) {
			MovementVisualization(to.Unit, to);
	//		to.Unit.unitController.MoveTo (to.worldPosition);
		}
		if (IsThisUnitsTurn (to.Unit)) {
			SelectedUnit = to;
		}
	}
	public void MovementVisualization(Unit unit, Node to){
		unit.unitController.MoveTo (to.worldPosition);
	}
	public void NextTurn(){
		Debug.Log ("starting next turn function");
		if (turn != PlayerID.NULL && GetPlayer (turn).control == Control.INPUT) {
			mouseInput.OnMouseLeftClickTile -=OnTileSelected;
			OnSelectedUnitChange-=SelectedUnitChange;
			OnSelectedSpellChange -= SelectedSpellChange;
		}
		if (turn == PlayerID.NULL) {
			turn = PlayerID.LEFT;
		} else if (turn == PlayerID.LEFT) {
			turn = PlayerID.RIGTH;
		} else {
			turn = PlayerID.LEFT;
		}
		List<Unit> units = GetPlayer (turn).troop.units;
		Hero hero = GetPlayer (turn).troop.hero;
		if (hero != null) {
			hero.actionPoints=1;
		}
		for (int i=0; i<units.Count; ++i){
			if (units[i]!=null){
				units[i].actionPoints=1;
				units[i].ableToMove=true;
				units[i].ableToAttack=true;
				for (int i2=0; i2<units[i].TriggableAbilities.Count; ++i2){
					if (units[i].TriggableAbilities[i2] is IOnTurnChangeTrigger ){
						((IOnTurnChangeTrigger) units[i].TriggableAbilities[i2]).OnTurnChange(grid.UnitsNode(units[i]));
					}
					if (units[i]==null){
						break;
					}
				}
			}
		}
		units = GetPlayer (turn, true).troop.units;
		for (int i=0; i<units.Count; ++i){
			if (units[i]!=null){
				units[i].ableToRetaliate=true;
				for (int i2=0; i2<units[i].TriggableAbilities.Count; ++i2){
					if (units[i].TriggableAbilities[i2] is IOnTurnChangeTrigger ){
						((IOnTurnChangeTrigger) units[i].TriggableAbilities[i2]).OnTurnChange(grid.UnitsNode(units[i]));
					}
					if (units[i]==null){
						break;
					}
				}
			}
		}
		if (OnTurnChange != null) {
			OnTurnChange (GetPlayer (turn));
		}
		Debug.Log ("next turn: " + turn.ToString());
		if (GetPlayer (turn).control == Control.INPUT) {
			StartCoroutine(WaitForAllActions(delegate{
				mouseInput.OnMouseLeftClickTile += OnTileSelected;
				OnSelectedUnitChange += SelectedUnitChange;
				OnSelectedSpellChange += SelectedSpellChange;
				mouseInput.enabled=true;
				NextUnit ();
			}));
		} else {
			mouseInput.enabled=false;
			StartCoroutine(WaitForAllActions(delegate{ai.PlayTurn (turn);}));
		}


	}
	public void StartBattle(Troop troop1, Control control1, Troop troop2, Control control2){
	//	OnUnitStackDeath += CheckIfBattleHasEnded;
		player1.id = PlayerID.LEFT;
		player1.troop = troop1;
		player1.control = control1;
		player1.losses =  new Troop();

		for (int i=0; i<troop1.units.Count; ++i){
			if (troop1.units[i]!=null){
				troop1.units[i].owner = player1.id;
//				for (int i2=0; i2<troop1.units[i].abilities.Count; ++i2) {
//					if (troop1.units[i].abilities[i2] is IOnStartMovement){
//						((IOnStartMovement)troop1.units[i].abilities[i2]).OnStartMovement(troop1.units[i]);
//					}
//				}
			}
		}
		player2.id = PlayerID.RIGTH;
		player2.troop = troop2;
		player2.control = control2;
		player2.losses =  new Troop();
		for (int i=0; i<troop2.units.Count; ++i){
			if (troop2.units[i]!=null){
				troop2.units[i].owner = player2.id;
//				for (int i2=0; i2<troop2.units[i].abilities.Count; ++i2) {
//					if (troop2.units[i].abilities[i2] is IOnStartMovement){
//						((IOnStartMovement)troop2.units[i].abilities[i2]).OnStartMovement(troop2.units[i]);
//					}
//				}
			}
		}

		grid.CreateGrid();
		grid.SetUnitsToGrid (player1.troop, player1.id);
		grid.SetUnitsToGrid (player2.troop, player2.id);
		mouseInput.enabled =true;

		turn = PlayerID.NULL;
		mouseInput.OnMouseOnTile += OnMouseOverTile;
		mouseInput.OnMouseExitTile += OnMouseExitTile;
		mouseInput.OnMouseRigthClickTile += HandeMouseRigthClickTile;
		mouseInput.OnMouseRigthButtonkUp+= HandleOnMouseRigthButtonUp;
		OnTurnChange += ScaleVisualsToZeroOrDefault;
		OnTurnChange += RefreshVisualizationTilesOnTurnChange;

		StartCoroutine (ProcessAttackVisuals());
		NextTurn ();
	}

	void HandleOnMouseRigthButtonUp ()
	{
		GuiManager.instance.CloseEntityInfo ();
	}
	private void ScaleVisualsToZeroOrDefault(Player player){
		Node[] nodes = grid.GetAllNodes();
		if (player.control == Control.AI) {
			for (int i=0; i< nodes.Length; ++i) {
				nodes [i].visualizationTile.ScaleToZero ();
			}
		}
		else {
			for (int i=0; i< nodes.Length; ++i) {
				nodes [i].visualizationTile.ScaleDown ();
			}
		}
	}
	private void RefreshVisualizationTilesOnTurnChange(Player player){
		List<Unit> units = player.troop.units;
		for (int i=0; i<units.Count; ++i) {
			if (units[i] != null){
				grid.UnitsNode(units[i]).visualizationTile.SetYellow();
			}
		}
		units = GetPlayer(player.id, true).troop.units;
		for (int i=0; i<units.Count; ++i) {
			if (units[i] != null){
				grid.UnitsNode(units[i]).visualizationTile.SetRed();
			}
		}
	}
	public void EndBattle(){
		List<Unit> units = player1.troop.units;
		for (int i=0; i<units.Count; ++i) {
			if (units[i]!=null){
				units[i].Reset();
			}
		}
		units = player2.troop.units;
		for (int i=0; i<units.Count; ++i) {
			if (units[i]!=null){
				units[i].Reset();
			}
		}
		StopAllCoroutines ();
		additionalMouseoverUpScaledNodes = new List<Node>();
		selectedUnitsValidMovement = null;
	//	OnUnitStackDeath -= CheckIfBattleHasEnded;
		mouseInput.Reset ();
		OnSelectedSpellChange = null;
		OnSelectedUnitChange = null;
		OnTurnChange = null;
		mouseInput.enabled =false;
		hitQueue.Clear ();
		Debug.Log ("battleended");
		StartCoroutine (WaitForAllActions (GameManager.instance.EndQuest));
	}
	private IEnumerator WaitForAllActions(Action callBack){
		while (true) {
			if (TurnIsAbleToEnd()){
				break;
			}
			yield return new WaitForEndOfFrame();
		}
		callBack ();
	}
	public Player GetPlayer(PlayerID id, bool other=false){
		if (player1.id == id) {
			if (other){
				return player2;
			}
			return player1;
		}
		if (other){
			return player1;
		}
		return player2;
	}
	public Unit ClosestUnit(Unit fromUnit, PlayerID whoseUnit=PlayerID.NULL){
		//make list of units, with parameter whoseUnit in mind
		List<Unit> units;
		if (whoseUnit == PlayerID.NULL) {
			units = new List<Unit>();
			List<Unit> leftUnits = GetPlayer (PlayerID.LEFT).troop.units;
			List<Unit> rightUnits = GetPlayer (PlayerID.RIGTH).troop.units;
			for (int i=0; i<leftUnits.Count; ++i){
				if (leftUnits[i]!=null){
					units.Add(leftUnits[i]);
				}
			}
			for (int i=0; i<rightUnits.Count; ++i){
				if (rightUnits[i]!=null){
					units.Add(rightUnits[i]);
				}
			}

		} else {
			units = GetPlayer(whoseUnit).troop.units;
		}

		//Search closest unit from units list
		Unit closest = null;
		int closestDistance = 10000;
		Node fromNode = grid.UnitsNode (fromUnit);
		Node otherNode;
		for (int i=0; i< units.Count; ++i) {
			otherNode = grid.UnitsNode(units[i]);
			if (otherNode==null){
				otherNode = units[i].unitController.currentMovementTarget;
			}
			int distance = grid.DistanceBetween(fromNode, otherNode);
			if (distance<closestDistance){
				if (units[i]==fromUnit){
					continue;
				}
				closestDistance = distance;
				closest =units[i];
			}
		}
		return closest;
	}
	//Order: first closest, last farthest
	public List<Unit> ClosestUnitsInOrder(Unit fromUnit, int maxRange=100, PlayerID whoseUnit=PlayerID.NULL){
		//make list of units, with parameter whoseUnit in mind
		List<Unit> units;
		if (whoseUnit == PlayerID.NULL) {
			units = new List<Unit>();
			List<Unit> leftUnits = GetPlayer (PlayerID.LEFT).troop.units;
			List<Unit> rightUnits = GetPlayer (PlayerID.RIGTH).troop.units;
			for (int i=0; i<leftUnits.Count; ++i){
				if (leftUnits[i]!=null){
					units.Add(leftUnits[i]);
				}
			}
			for (int i=0; i<rightUnits.Count; ++i){
				if (rightUnits[i]!=null){
					units.Add(rightUnits[i]);
				}
			}

		} else {
			units = GetPlayer(whoseUnit).troop.units;
		}
		List<Unit> inRangeUnits = new List<Unit>();
		for (int i=0; i<units.Count; ++i){
			if (grid.DistanceBetween(grid.UnitsNode(fromUnit), grid.UnitsNode(units[i]))<=maxRange){
				inRangeUnits.Add(units[i]);
			}
		}
		units = inRangeUnits;
		//Search closest unit from units list
		List<int> order = new List<int>();
		Node fromNode = grid.UnitsNode (fromUnit);
		Node otherNode;
		int unitAmount = units.Count;
		for (int i=0; i< unitAmount-1; ++i) {
			int closestIndex=0;
			int closestDistance = maxRange;
			for (int i2=0; i2< units.Count; ++i2) {
				otherNode = grid.UnitsNode(units[i2]);
				int distance = grid.DistanceBetween(fromNode, otherNode);
				if (distance<closestDistance && !order.Contains(i2)){
					if (units[i2]==fromUnit){
						continue;
					}
					closestDistance = distance;
					closestIndex =i2;
				}
			}

			order.Add(closestIndex);
		}
		return order.Select(i => units[i]).ToList(); 
	}
	public bool IsUnitInAttackQueue(Unit unit){
		Debug.Log ("queue Length: " + hitQueue.Count);
		for (int i=0; i<hitQueue.Count; ++i) {
			CombatInfo temp = hitQueue.ElementAt(i);
			if (temp is AttackInfo){
				AttackInfo attackInfo = (AttackInfo)temp;
				if (unit == attackInfo.attack.target || unit == attackInfo.retalition.target){
					return true;
				}
				for (int ii=0; ii<attackInfo.additionalTargets.Count; ++ii){
					if (attackInfo.additionalTargets[ii].target==unit){
						return true;
					}
					
				}
				
			}else if (temp is SpellInfo){
				SpellInfo attackInfo = (SpellInfo)temp;
				if (unit == attackInfo.target.Unit){
					return true;
				}
				for (int ii=0; ii<attackInfo.hitInfos.Count; ++ii){
					if (attackInfo.hitInfos[ii].target==unit){
						return true;
					}
				}
			}
		}
		return false;
	}
	public bool IsThisUnitsTurn(Unit unit){
		Player player = GetPlayer (turn);
		if (player.troop.units.Contains (unit)) {
			return true;
		}
		return false;
	}
}
