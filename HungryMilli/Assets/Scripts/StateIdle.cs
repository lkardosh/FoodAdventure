using UnityEngine;

public class StateIdle : State {
	public override void Execute(AIController character) {
		int foodCount = character.GetPlayerFoodCount();
		if (foodCount>= 5) {
			character.ChangeState(new StatePatrol());
		} 
		else if (character.IsDead) {
            character.ChangeState(new StateDead());
        } 
        else if (character.IsPlayerSeen()) {
			character.ChangeState(new StateChasing());
		}
        else {
			character.BeIdle();
		}
	}
}