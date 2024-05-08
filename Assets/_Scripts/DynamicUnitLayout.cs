using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class DynamicUnitLayout : MonoBehaviour
{

    private GridLayoutGroup gridLayout;

    [SerializeField] private float cellSizeMultiplier = 1.0f;
    [SerializeField] private float spacingMultiplier = 1.0f;

    private void Awake()
    {
        gridLayout = GetComponent<GridLayoutGroup>();
    }

    private void Update()
    {
        UpdateGridLayout();
    }

    private void UpdateGridLayout()
    {
        if (gridLayout == null)
            return;

        int childCount = transform.childCount;

        if (childCount == 0)
            return;

        // If there's only one child, disable the GridLayoutGroup
        if (childCount == 1)
        {
            gridLayout.enabled = false;
            return;
        }
        else
        {
            gridLayout.enabled = true;
        }

        // Calculate cell size based on child count
        Vector2 cellSize = gridLayout.cellSize;
        cellSize.x = Mathf.Max(1, Mathf.RoundToInt(cellSize.x * cellSizeMultiplier));
        cellSize.y = Mathf.Max(1, Mathf.RoundToInt(cellSize.y * cellSizeMultiplier));
        gridLayout.cellSize = cellSize;

        // Calculate spacing based on child count
        Vector2 spacing = gridLayout.spacing;
        spacing.x = Mathf.Max(0, Mathf.RoundToInt(spacing.x * spacingMultiplier));
        spacing.y = Mathf.Max(0, Mathf.RoundToInt(spacing.y * spacingMultiplier));
        gridLayout.spacing = spacing;
    }

    // You can call this method to manually trigger the update of the GridLayoutGroup
    public void UpdateGridLayoutManually()
    {
        UpdateGridLayout();
    }
}
