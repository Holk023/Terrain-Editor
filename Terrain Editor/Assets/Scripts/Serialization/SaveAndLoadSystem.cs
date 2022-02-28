using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveAndLoadSystem
{

    #region Variables

    private Terrain terrain;
    private SettingsUI settings;
    private List<VertexSerializable> serializeVertexHeight = new List<VertexSerializable>();

    #endregion

    public SaveAndLoadSystem(Terrain terrain, SettingsUI settings)
	{
        this.terrain = terrain;
        this.settings = settings;
	}

    public void SaveTerrain()
    {
        TerrainSerialize terrainSerialize = new TerrainSerialize();
        terrainSerialize.terrainSize = (int)terrain.terrainData.size.x;
        serializeVertexHeight = GetVertex();
        terrainSerialize.vertexList = serializeVertexHeight;

        var gameJSON = JsonUtility.ToJson(terrainSerialize);
        var savePath = Application.persistentDataPath + "/save.json";

        File.Open(savePath, FileMode.OpenOrCreate).Dispose();
        File.WriteAllText(savePath, gameJSON);
    }

    public void LoadTerrain()
    {
        TerrainSerialize terrainSerialize = Load();
        terrain.terrainData.size = new Vector3(terrainSerialize.terrainSize, terrain.terrainData.size.y, terrainSerialize.terrainSize);
        serializeVertexHeight = terrainSerialize.vertexList;
        var heights = new float[(int)terrain.terrainData.heightmapHeight, (int)terrain.terrainData.heightmapWidth];
        var vertId = 0;

        for (int i = 0; i < (int)terrain.terrainData.heightmapHeight; i++)
        {
            for (int j = 0; j < (int)terrain.terrainData.heightmapWidth; j++)
            {
                heights[j, i] = serializeVertexHeight[vertId].vertexHeight / 600f;
                vertId++;
            }
        }

        terrain.terrainData.SetHeights(0, 0, heights);
        settings.TextUpdate();
    }

    public TerrainSerialize Load()
    {
        var savePath = Application.persistentDataPath + "/save.json";

        File.Open(savePath, FileMode.OpenOrCreate).Dispose();
        var jsonData = File.ReadAllText(savePath);

        TerrainSerialize terrainSerialize = JsonUtility.FromJson<TerrainSerialize>(jsonData);
        return terrainSerialize;
    }

    public List<VertexSerializable> GetVertex()
    {
        List<VertexSerializable> vertexHeights = new List<VertexSerializable>();
        var heights = terrain.terrainData.GetHeights(0, 0, (int)terrain.terrainData.heightmapHeight, (int)terrain.terrainData.heightmapWidth);
        for (int i = 0; i < (int)terrain.terrainData.heightmapHeight; i++)
        {
            for (int j = 0; j < (int)terrain.terrainData.heightmapWidth; j++)
            {
                heights[i, j] = terrain.terrainData.GetHeight(i, j);
                VertexSerializable vertexSerializable = new VertexSerializable();
                vertexSerializable.vertexHeight = (float)heights[i, j];
                vertexHeights.Add(vertexSerializable);
            }
        }
        return vertexHeights;
    }
}
