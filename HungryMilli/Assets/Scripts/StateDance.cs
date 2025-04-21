using UnityEngine;
using System.Collections;

public class StateDance : State {
    public override void Execute(AIController character) {
        if (character.IsPlayerAlive()) {  // If the player revives, return to normal behavior
            float distanceToPlayer = character.getDistanceToPlayer();

            if (character.IsPlayerSeen()) {
                if (distanceToPlayer <= character.getAttackDistance()) {
                    character.ChangeState(new StateAttack());
                } 
                else if (distanceToPlayer <= character.getSightRadius() / 2) {
                    character.ChangeState(new StatePatrol());
                } 
            } 
            else if (character.IsDead) {
                character.ChangeState(new StateDead());
            } 
            else {
                character.ChangeState(new StateIdle());
            }
        } 
        else {
            character.BeDancing();  // Continue dancing while the player is dead
        }
    }
}