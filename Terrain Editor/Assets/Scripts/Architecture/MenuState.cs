using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuState : BaseState
{

	#region Variables

	private SettingsUI settings;
	private UnityAction toGameState;

	#endregion

	public MenuState(SettingsUI settings, UnityAction toGameState)
	{
		this.toGameState = toGameState;
		this.settings = settings;
	}

	public override void InitState()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		settings.ShowView();
	}

	public override void UpdateState()
	{

		if (Input.GetKeyDown(KeyCode.Tab))
		{
			toGameState.Invoke();
		}
	}

	public override void DestroyState()
	{
		settings.HideView();
	}
}
