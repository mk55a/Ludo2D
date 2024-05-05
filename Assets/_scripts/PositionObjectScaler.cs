using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionObjectScaler : MonoBehaviour
{
    public RectTransform boardImage; // Reference to the board image RectTransform
    public RectTransform[] positionObjects; // Array of position GameObjects

    private Vector2 originalBoardSize;

    void Start()
    {
        originalBoardSize = boardImage.sizeDelta;
        UpdatePositionObjects();
    }

    void Update()
    {
        if (boardImage.sizeDelta != originalBoardSize)
        {
            UpdatePositionObjects();
        }
    }

    void UpdatePositionObjects()
    {
        Vector2 scaleFactor = boardImage.sizeDelta / originalBoardSize;

        foreach (RectTransform positionObject in positionObjects)
        {
            positionObject.localScale = new Vector3(scaleFactor.x, scaleFactor.y, 1f);
            // Adjust position if needed
            // For example, if positionObject anchored to the bottom-left corner:
            // positionObject.anchoredPosition *= scaleFactor;
        }
    }
}
