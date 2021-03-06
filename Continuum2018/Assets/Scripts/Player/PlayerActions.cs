﻿using UnityEngine;
using InControl;

public class PlayerActions : PlayerActionSet
{
	public PlayerAction MoveLeft, MoveRight, MoveUp, MoveDown; // Movement.
	public PlayerTwoAxisAction Move; // Axes for movement.

	public PlayerAction Shoot; // Shoot action.
	public PlayerAction Ability; // Ability action.
	public PlayerAction Pause; // Pause action.
	public PlayerAction DebugMenu; // Debug menu action.
	public PlayerAction CheatConsole; // Cheat console action.

	public PlayerAction Back; // For UI returning one step back.

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
		CheatConsole = CreatePlayerAction ("CheatConsole");

		Back = CreatePlayerAction ("Back");
	}
}