using UnityEngine;
using System.Collections;

public class StateDead : State
{
	public override void Execute(AIController character)
	{
		character.BeDead();
	}
}
