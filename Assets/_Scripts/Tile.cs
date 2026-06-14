using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a single hex value unit within a Piece.
/// Owns trigger detection only — tracks which Cell it is currently hovering over.
/// All input is handled by parent Piece.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Tile : Hex
{
    public Cell HoveredCell { get; private set; }

    private List<Cell> _overlappingCells = new List<Cell>();

    private void Update()
    {
        if (_overlappingCells.Count == 0)
        {
            HoveredCell = null;
            return;
        }

        Cell closest = null;
        float minDist = float.MaxValue;
        foreach (Cell c in _overlappingCells)
        {
            float dist = Vector2.Distance(transform.position, c.transform.position);
            if (dist < minDist) { minDist = dist; closest = c; }
        }

        if (closest == HoveredCell) return;

        if (HoveredCell != null) HoveredCell.HighlightCell(false);
        HoveredCell = closest;
    }

    /// <summary>
    /// True if hovering over a cell that can accept this tile's value.
    /// </summary>
    public bool IsPlaceable =>
        HoveredCell != null &&
        HoveredCell.State != CellState.Closed &&
        (HoveredCell.State == CellState.Open || HoveredCell.Value == Value);

    public void SetValue(int value)
    {
        Value = value;
        valueTextBox.enabled = true;
        _sprite.enabled = true;
        SetHexColor(Color.HSVToRGB((float)Value / 36 % 1, .6f, 1));
        UpdateTextBoxValue();
    }

    public void HighlightHoveredCell(bool highlight)
    {
        if (HoveredCell == null) return;
        HoveredCell.HighlightCell(highlight);
    }

    public void Place()
    {
        if (HoveredCell == null) return;
        HoveredCell.Fill(Value);
        HoveredCell = null;
        
        Value = 0;
        _sprite.enabled = false;
        valueTextBox.enabled = false;
    }

    // ── Trigger detection ───────────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D other)
    {
        Cell cell = other.GetComponent<Cell>();
        if (cell == null) return;
        _overlappingCells.Add(cell);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Cell cell = other.GetComponent<Cell>();
        if (cell == null) return;
        if (cell == HoveredCell) HoveredCell.HighlightCell(false);
        _overlappingCells.Remove(cell);
        HoveredCell = null; // recalculated next Update
    }
}