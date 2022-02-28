using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TerrainEditor : MonoBehaviour
{

    #region Variables

    [Header("Komponenty:")]
    [SerializeField] private Terrain terrainTarget;
    [SerializeField] private Brush brush;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask layerMask;

    [Header("Zasiêg rysowania:")]
    [SerializeField] private float minPlayerRadius;
    [SerializeField] private float maxPlayerRadius;

    [Header("Wykresy:")]
    [SerializeField] private AnimationCurve circleCurve;
    [SerializeField] private AnimationCurve triangleCurve;

    private Vector3 brushPoint;
    private int offset;

    [Header("Gizmos:")]
    [SerializeField] private bool drawGizmos;

	#endregion

	#region Getters

	public Vector3 GetGlobalToTerrainPosition(Vector3 globalPos)
    {
        var terrainSize = terrainTarget.terrainData.size;
        var terrainResolution = terrainTarget.terrainData.heightmapResolution;
        var terrainPos = globalPos - terrainTarget.GetPosition();
        terrainPos = new Vector3(terrainPos.x / terrainSize.x, terrainPos.y / terrainSize.y, terrainPos.z / terrainSize.z);
        return new Vector3(terrainPos.x * terrainResolution, 0, terrainPos.z * terrainResolution);
    }

    public Vector2Int GetBrushPosition(Vector3 worldPos)
    {
        var terrainResolution = terrainTarget.terrainData.heightmapResolution;
        var terrainPos = GetGlobalToTerrainPosition(worldPos);
        offset = brush.brushRadius / 2;
        return new Vector2Int((int)Mathf.Clamp(terrainPos.x - offset, 0.0f, terrainResolution), 
            (int)Mathf.Clamp(terrainPos.z - offset, 0.0f, terrainResolution));
    }

	public int GetBrushSize(int brushPosX, int brushPosY, int brushRadius)
	{
        var terrainData = terrainTarget.terrainData;
        var terrainResolution = terrainTarget.terrainData.heightmapResolution;
		while (terrainResolution - (brushPosX + brushRadius) < 0)
		{
			brushRadius--;
		}
		while (terrainResolution - (brushPosY + brushRadius) < 0)
		{
            brushRadius--;
		}
		return brushRadius;
	}

	#endregion

	public void InputUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, 100f, layerMask))
            {
                if(Vector3.Distance(player.position, hit.point) > minPlayerRadius 
                    && Vector3.Distance(player.position, hit.point) < maxPlayerRadius)
				{                 
                    RaiseTerrain(hit.point, brush.strength, brush.brushRadius);
                    brushPoint = new Vector3Int((int)hit.point.x, (int)hit.point.y, (int)hit.point.z);
				}
            }
        }

        if (Input.GetMouseButton(1))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, 100f, layerMask))
            {
                if (Vector3.Distance(player.position, hit.point) > minPlayerRadius
                    && Vector3.Distance(player.position, hit.point) < maxPlayerRadius)
				{
                    LowerTerrain(hit.point, brush.strength, brush.brushRadius);
                    brushPoint = new Vector3Int((int)hit.point.x, (int)hit.point.y, (int)hit.point.z);
                }
            }
        }
    }

    //=====================================================================
    //SetHeights w przypadku pêdzla circle i triagle musi pozostaæ w pêtli!
    //=====================================================================

    public void RaiseTerrain(Vector3 worldPos, float strength, int brushRadius)
	{
        var terrainData = terrainTarget.terrainData;
        var brushPos = GetBrushPosition(worldPos);
        var brushSize = GetBrushSize(brushPos.x, brushPos.y,brushRadius);
        var heights = terrainData.GetHeights(brushPos.x, brushPos.y, brushSize, brushSize);

        for (int i = 0; i < brushSize; i++)
		{
			for (int j = 0; j < brushSize; j++)
			{
				if (brush.BrushType == BrushType.Square)
				{
                    heights[i, j] += strength * Time.smoothDeltaTime;
                }
                else if (brush.BrushType == BrushType.Circle)
				{
                    Keyframe[] keyframes = circleCurve.keys;
                    keyframes[2].time = (float)brushSize;
                    keyframes[2].value = 0;
                    keyframes[1].time = (float)(brushSize) / 2;
                    keyframes[1].value = strength;
                    keyframes[0].value = 0;
                    keyframes[0].time = 0;

                    circleCurve.keys = keyframes;

                    heights[i, j] += circleCurve.Evaluate(i) * Time.smoothDeltaTime;
                    terrainData.SetHeights(brushPos.x, brushPos.y, heights);

                    heights[i, j] += circleCurve.Evaluate(j) * Time.smoothDeltaTime;
                    terrainData.SetHeights(brushPos.x, brushPos.y, heights);
                }
                else if(brush.BrushType == BrushType.Triangle)
				{
                    Keyframe[] keyframes = triangleCurve.keys;
                    keyframes[0].value = 0f;
                    keyframes[0].time = 0f;
                    keyframes[1].time = (float)brushSize * 0.4f;
                    keyframes[1].value = strength * 0.5f;
                    keyframes[2].time = (float)brushSize / 2;
                    keyframes[2].value = strength * 2;
                    keyframes[3].time = (float)brushSize * 0.6f;
                    keyframes[3].value = strength * 0.5f;
                    keyframes[4].time = (float)brushSize;
                    keyframes[4].value = 0f;

                    triangleCurve.keys = keyframes;

                    heights[i, j] += triangleCurve.Evaluate(i * Mathf.Sqrt(2)) * Time.fixedDeltaTime;
                    terrainData.SetHeights(brushPos.x, brushPos.y, heights);

                    heights[i, j] += triangleCurve.Evaluate(j * Mathf.Sqrt(2)) * Time.fixedDeltaTime;
                    terrainData.SetHeights(brushPos.x, brushPos.y, heights);
                }
            }
		}

        if (brush.BrushType == BrushType.Square)
        {
            terrainData.SetHeights(brushPos.x, brushPos.y, heights);
        }
    }

	public void LowerTerrain(Vector3 worldPos, float strength, int brushRadius)
	{
        var terrainData = terrainTarget.terrainData;
        var brushPos = GetBrushPosition(worldPos);
        var brushSize = GetBrushSize(brushPos.x, brushPos.y, brushRadius);
        var heights = terrainData.GetHeights(brushPos.x, brushPos.y, brushSize, brushSize);

        for (int i = 0; i < brushSize; i++)
        {
            for (int j = 0; j < brushSize; j++)
            {
                if (brush.BrushType == BrushType.Square)
                {
                    heights[i, j] -= strength * Time.smoothDeltaTime;
                }
                else if (brush.BrushType == BrushType.Circle)
                {
                    Keyframe[] keyframes = circleCurve.keys;
                    keyframes[2].time = (float)brushSize;
                    keyframes[2].value = 0;
                    keyframes[1].time = (float)(brushSize) / 2;
                    keyframes[1].value = strength;
                    keyframes[0].value = 0;
                    keyframes[0].time = 0;

                    circleCurve.keys = keyframes;

                    heights[i, j] -= circleCurve.Evaluate(i) * Time.smoothDeltaTime;
                    terrainData.SetHeights(brushPos.x, brushPos.y, heights);

                    heights[i, j] -= circleCurve.Evaluate(j) * Time.smoothDeltaTime;
                    terrainData.SetHeights(brushPos.x, brushPos.y, heights);
                }
                else if (brush.BrushType == BrushType.Triangle)
                {
                    Keyframe[] keyframes = triangleCurve.keys;
                    keyframes[0].value = 0f;
                    keyframes[0].time = 0f;
                    keyframes[1].time = (float)brushSize * 0.4f;
                    keyframes[1].value = strength * 0.5f;
                    keyframes[2].time = (float)brushSize / 2;
                    keyframes[2].value = strength * 2;
                    keyframes[3].time = (float)brushSize * 0.6f;
                    keyframes[3].value = strength * 0.5f;
                    keyframes[4].time = (float)brushSize;
                    keyframes[4].value = 0f;

                    triangleCurve.keys = keyframes;

                    heights[i, j] -= triangleCurve.Evaluate(i * Mathf.Sqrt(2)) * Time.fixedDeltaTime;
                    terrainData.SetHeights(brushPos.x, brushPos.y, heights);

                    heights[i, j] -= triangleCurve.Evaluate(j * Mathf.Sqrt(2)) * Time.fixedDeltaTime;
                    terrainData.SetHeights(brushPos.x, brushPos.y, heights);
                }
            }
        }
        
        if(brush.BrushType == BrushType.Square)
        {
            terrainData.SetHeights(brushPos.x, brushPos.y, heights);
		}
    }

    [ContextMenu("FlattenAll")]
    public void FlattenAll()
    {
        var terrainData = terrainTarget.terrainData;
        var heights = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);

        for (int i = 0; i < terrainData.heightmapWidth; i++)
        {
            for (int j = 0; j < terrainData.heightmapHeight; j++)
            {
                heights[i, j] = 5f / 600;
            }     
        }
        terrainData.SetHeights(0, 0, heights);
    }

    #if UNITY_EDITOR
    private void OnApplicationQuit()
	{
        FlattenAll();
	}
    #endif

    private void OnDrawGizmos()
	{
        if(drawGizmos)
		{
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, minPlayerRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.position, maxPlayerRadius);

            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(brushPoint, .1f);
		}
    }
}