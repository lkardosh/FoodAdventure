using UnityEngine;

public class StatePatrolFast : State {
	public override void Execute(AIController character) {
		if (character.IsPlayerSeen()) {
			character.ChangeState(new StateChasing());
		}
		else if (character.IsDead) {
            character.ChangeState(new StateDead());
        } 
        else{
            character.bePatrollingFast();
        }
	}
}