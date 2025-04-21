using UnityEngine;

public class StateChasing : State {
	public override void Execute(AIController character) {
		int foodCount = character.GetPlayerFoodCount();
		if (!character.IsPlayerSeen() && (foodCount >= 5)) {
			character.ChangeState(new StatePatrol());
		}
        else if (!character.IsPlayerSeen() && foodCount < 5) {
			character.ChangeState(new StateIdle());
		}
        else if (!character.IsPlayerSeen() && foodCount >= 10) {
			character.ChangeState(new StatePatrolFast());
		}
        else if (character.IsPlayerSeen() && character.getDistanceToPlayer()<10){
			character.ChangeState(new StateAttack());
		}
		else if (character.IsDead) {
            character.ChangeState(new StateDead());
        } 
        else {
			character.BeRunning();
		}

	}
}