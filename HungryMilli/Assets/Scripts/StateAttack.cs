using UnityEngine;

public class StateAttack : State {
    public override void Execute(AIController character) {
        if (!character.IsPlayerAlive()) {  
            character.ChangeState(new StateDance());
        }
        else if (character.IsDead) {
            character.ChangeState(new StateDead());
        } 
        else if (character.IsPlayerSeen()) {
            float distanceToPlayer = character.getDistanceToPlayer();
            
            if (distanceToPlayer > character.getAttackDistance() && distanceToPlayer <= (character.getSightRadius() / 2)) {
                character.ChangeState(new StatePatrol());
            } 
            // else if (distanceToPlayer > (character.getSightRadius() / 2)) {
            //     character.ChangeState(new StateRun());
            // } 
            else if (distanceToPlayer <= character.getAttackDistance()) {
                character.BeAttacking();
            }
        } 
        else {
            character.ChangeState(new StateIdle());
        }
    }
}