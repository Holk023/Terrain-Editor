using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameState : BaseState
{
	#region Variables

	private TerrainEditor terrainEditor;
	private PlayerMovement playerMovement;
	private UnityAction toMenuState;
	private Brush brush;
	private GameObject gameHud;

	#endregion

	public GameState(TerrainEditor terrainEditor, PlayerMovement playerMovement, Brush brush,GameObject gameHud ,UnityAction toMenuState)
	{
		this.terrainEditor = terrainEditor;
		this.playerMovement = playerMovement;
		this.toMenuState = toMenuState;
		this.brush = brush;
		this.gameHud = gameHud;
	}

	public override void InitState()
	{
		gameHud.SetActive(true);
		playerMovement.cameraBrain.SetActive(true);
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public override void UpdateState()
	{
		terrainEditor.InputUpdate();
		playerMovement.InputUpdate();
		brush.InputUpdate();

		if(Input.GetKeyDown(KeyCode.Tab))
		{
			toMenuState.Invoke();
		}
	}

	public override void DestroyState()
	{
		gameHud.SetActive(false);
		playerMovement.cameraBrain.SetActive(false);
	}
}
