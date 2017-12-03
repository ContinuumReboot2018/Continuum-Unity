using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class PlayerActions : PlayerActionSet
{
	public PlayerAction MoveLeft, MoveRight, MoveUp, MoveDown;
	public PlayerTwoAxisAction Move;

	public PlayerAction Shoot;
	public PlayerAction Ability;
	public PlayerAction Pause;
	public PlayerAction DebugMenu;

	public PlayerActions ()
	{
		MoveLeft = CreatePlayerAction ("Move Left");
		MoveRight = CreatePlayerAction ("Move Right");
		MoveUp = CreatePlayerAction ("Move Up");
		MoveDown = CreatePlayerAction ("Move Down");
		Move = CreateTwoAxisPlayerAction (MoveLeft, MoveRight, MoveDown, MoveUp);

		Shoot = CreatePlayerAction ("Shoot");
		Ability = CreatePlayerAction ("Ability");
		Pause = CreatePlayerAction ("Pause");
		DebugMenu = CreatePlayerAction ("Debug Menu");
	}
}
