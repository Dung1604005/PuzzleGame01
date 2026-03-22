using System.Collections.Generic;
using UnityEngine;

public class LineClearPreviewPresenter
{
    private readonly GridVisualizer gridVisualizer;
    private readonly List<Vector2Int> activeCells = new List<Vector2Int>();

    public LineClearPreviewPresenter(GridVisualizer visualizer)
    {
        gridVisualizer = visualizer;
    }

    public void Show(List<Vector2Int> previewCells, Sprite previewSprite)
    {
        Clear();

        if (gridVisualizer == null || previewCells == null || previewCells.Count == 0 || previewSprite == null)
        {
            return;
        }

        gridVisualizer.DrawLineClearPreview(previewCells, previewSprite);
        activeCells.AddRange(previewCells);
    }

    public void Clear()
    {
        if (gridVisualizer == null || activeCells.Count == 0)
        {
            return;
        }

        gridVisualizer.ClearLineClearPreview(activeCells);
        activeCells.Clear();
    }
}
