using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StateController : MonoBehaviour
{

    #region Variables

    [Header("Komponenty:")]
    [SerializeField] private TerrainEditor terrainEditor;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Brush brush;
    [SerializeField] private SettingsUI settings;
    [SerializeField] private GameObject gameHud;

    private GameState gameState;
    private MenuState menuState;
    private BaseState currentlyActiveState;

    private UnityAction toGameState;
    private UnityAction toMenuState;

    #endregion

    void Start()
    {
        settings.HideView();
        toGameState = () => ChangeState(gameState);
        toMenuState = () => ChangeState(menuState);

        gameState = new GameState(terrainEditor, playerMovement, brush, gameHud, toMenuState);
        menuState = new MenuState(settings, toGameState);

        ChangeState(gameState);
    }

    void Update()
    {
        currentlyActiveState.UpdateState();
    }

    public void ChangeState(BaseState newState)
	{
        currentlyActiveState?.DestroyState();
        currentlyActiveState = newState;
        currentlyActiveState?.InitState();
    }
}
