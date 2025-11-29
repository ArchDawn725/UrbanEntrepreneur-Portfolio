using UnityEngine;
using UnityEngine.UI;

public class GridOrganizer : MonoBehaviour
{
    public int numberOfColumns = 7; // Number of columns in the grid
    public int numberOfRows = 6;    // Number of rows in the grid
    private GridLayoutGroup gridLayoutGroup;
    private RectTransform rectTransform;

    void Start()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();

        AdjustCellSize();
    }

    void Update()
    {
        AdjustCellSize();
    }

    private void AdjustCellSize()
    {
        // Get the size of the panel (RectTransform)
        float panelWidth = rectTransform.rect.width;
        float panelHeight = rectTransform.rect.height;

        // Calculate the available width and height after subtracting padding
        float availableWidth = panelWidth - gridLayoutGroup.padding.left - gridLayoutGroup.padding.right;
        float availableHeight = panelHeight - gridLayoutGroup.padding.top - gridLayoutGroup.padding.bottom;

        // Calculate cell width and height to fit the panel with spacing and padding taken into account
        float cellWidth = (availableWidth - (gridLayoutGroup.spacing.x * (numberOfColumns - 1))) / numberOfColumns;
        float cellHeight = (availableHeight - (gridLayoutGroup.spacing.y * (numberOfRows - 1))) / numberOfRows;

        // Set the cell size
        gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
    }
}
