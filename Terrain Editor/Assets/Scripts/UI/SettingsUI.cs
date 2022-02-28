using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class SettingsUI : MonoBehaviour
{

    #region Variables

    [Header("Komponenty:")]
    [SerializeField] private Terrain terrain;
    [SerializeField] private TerrainEditor terrainEditor;
    [SerializeField] private Brush brush;

    [Header("Brush Settings")]
    [SerializeField] private Button radiusPlus;
    [SerializeField] private Button radiusMinus;
    [SerializeField] private TextMeshProUGUI radiusValue;
    [SerializeField] private Button strengthPlus;
    [SerializeField] private Button strengthMinus;
    [SerializeField] private TextMeshProUGUI strengthValue;

    [Header("Terrain Settings")]
    [SerializeField] private Button sizePlus;
    [SerializeField] private Button sizeMinus;
    [SerializeField] private TextMeshProUGUI sizeValue;
    [SerializeField] private Button flattenAll;

    [Header("Serialization")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;

    [SerializeField] private Button exitButton;

    private SaveAndLoadSystem saveAndLoadSystem;

    #endregion

    void Start()
    {
        saveAndLoadSystem = new SaveAndLoadSystem(terrain, this);

        TextUpdate();
        radiusPlus.onClick.AddListener( () => ChangeBrushRadius(1));
        radiusMinus.onClick.AddListener(() => ChangeBrushRadius(-1));

        strengthPlus.onClick.AddListener(() => ChangeBrushStrength(1));
        strengthMinus.onClick.AddListener(() => ChangeBrushStrength(-1));

        sizePlus.onClick.AddListener(() => ChangeTerrainSize(1));
        sizeMinus.onClick.AddListener(() => ChangeTerrainSize(-1));

        flattenAll.onClick.AddListener(() => terrainEditor.FlattenAll());
        exitButton.onClick.AddListener(() => Exit());

        saveButton.onClick.AddListener(() => saveAndLoadSystem.SaveTerrain());
        loadButton.onClick.AddListener(() => saveAndLoadSystem.LoadTerrain());
    }

    public void ShowView()
	{
        gameObject.SetActive(true);
	}

    public void HideView()
    {
        gameObject.SetActive(false);
    }

    public void TextUpdate()
	{
        radiusValue.text = $"{brush.brushRadius}";
        strengthValue.text = $"{(brush.strength * 1000):F0}";
        sizeValue.text = $"{terrain.terrainData.size.x}";
    }

    public void ChangeBrushRadius(int multiplier)
	{
        brush.brushRadius += 1 * multiplier;
        if (brush.brushRadius > 10)
        {
            brush.brushRadius = 10;
        }
        else if (brush.brushRadius < 3)
        {
            brush.brushRadius = 3;
        }

        brush.UpdateBrushScale();
        TextUpdate();
	}

    public void ChangeBrushStrength(int multiplier)
    {
        brush.strength += 0.001f * multiplier;
        if(brush.strength > 0.01f)
		{
            brush.strength = 0.01f;
        }
        else if(brush.strength < 0.003f)
		{
            brush.strength = 0.003f;
        }
        TextUpdate();
    }

    public void ChangeTerrainSize(int multiplier)
    {
        terrain.terrainData.size = new Vector3((int)terrain.terrainData.size.x + (50 * multiplier), (int)terrain.terrainData.size.y, 
            terrain.terrainData.size.z + (50 * multiplier));
        if (terrain.terrainData.size.x > 1000)
        {
            terrain.terrainData.size = new Vector3((int)1000, terrain.terrainData.size.y, (int)1000);
        }
        else if (terrain.terrainData.size.x < 250)
        {
            terrain.terrainData.size = new Vector3((int)250, terrain.terrainData.size.y, (int)250);
        }
        brush.UpdateBrushScale();
        TextUpdate();
    }

    public void BrushSclaeUpdateAfterLoad()
	{
        brush.UpdateBrushScale();
    }

	public void Exit()
	{
        Application.Quit();
	}
}
