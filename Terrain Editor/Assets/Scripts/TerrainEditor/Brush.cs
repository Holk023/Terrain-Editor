using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BrushType
{
    Square,
    Circle,
    Triangle
}

public class Brush : MonoBehaviour
{

    #region Variables

    private BrushType brushType;
    public BrushType BrushType => brushType;

    [Header("Komponenty:")]
    [SerializeField] private Terrain terrain;
    [SerializeField] private Transform squareBrush;
    [SerializeField] private Transform circleBrush;
    [SerializeField] private Transform triangleBrush;
    [SerializeField] private LayerMask layerMask;

    [Header("HUD:")]
    [SerializeField] private Image square;
    [SerializeField] private Image circle;
    [SerializeField] private Image triangle;

    public int brushRadius;
    public float strength;

    #endregion

    private void Start()
	{
        brushType = BrushType.Square;
        circleBrush.gameObject.SetActive(false);
        triangleBrush.gameObject.SetActive(false);
        UpdateBrushScale();
        HudUpdate();
    }

	public void InputUpdate()
	{
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, 100f, layerMask))
		{
            squareBrush.position = new Vector3((int)hit.point.x, hit.point.y + +0.5f, (int)hit.point.z);
            circleBrush.position = new Vector3((int)hit.point.x, hit.point.y + +0.5f, (int)hit.point.z);
            triangleBrush.position = new Vector3((int)hit.point.x, hit.point.y + 0.5f, (int)hit.point.z);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
		{
            brushType = BrushType.Square;
            triangleBrush.gameObject.SetActive(false);
            circleBrush.gameObject.SetActive(false);
            squareBrush.gameObject.SetActive(true);
            HudUpdate();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            brushType = BrushType.Circle;
            triangleBrush.gameObject.SetActive(false);
            squareBrush.gameObject.SetActive(false);
            circleBrush.gameObject.SetActive(true);
            HudUpdate();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            brushType = BrushType.Triangle;
            squareBrush.gameObject.SetActive(false);
            circleBrush.gameObject.SetActive(false);
            triangleBrush.gameObject.SetActive(true);
            HudUpdate();
        }
    }

    public void HudUpdate()
	{
        if (BrushType == BrushType.Square)
        {
            square.color = Color.green;
            circle.color = Color.white;
            triangle.color = Color.white;
        }
        else if (BrushType == BrushType.Circle)
        {
            circle.color = Color.green;
            square.color = Color.white;
            triangle.color = Color.white;
        }
        else if (BrushType == BrushType.Triangle)
        {
            triangle.color = Color.green;
            square.color = Color.white;
            circle.color = Color.white;
        }
    }

    [ContextMenu("UpdateScale")]
    public void UpdateBrushScale()
	{
        float scaleRadius;

        scaleRadius = (terrain.terrainData.size.x * 1.5f * brushRadius) / 1000;
        squareBrush.localScale = new Vector3(scaleRadius, 0.01f, scaleRadius);
        circleBrush.localScale = new Vector3(scaleRadius, 0.01f, scaleRadius);
        triangleBrush.localScale = new Vector3(scaleRadius, 1f, scaleRadius);
    }

}
