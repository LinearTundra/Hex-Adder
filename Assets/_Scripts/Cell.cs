using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Cell : Hex
{
   
    public CellState State { get; private set; }
    public CellCoords Coords { get; private set; }
    public bool RightOffset { get; private set; }

    private Color _openCellColor = Color.HSVToRGB(0, 0, .7f);
    private Color _highlightCellColor = Color.HSVToRGB(.6f, .35f, 1);
    private Color _valueCellColor
    {
        get => Color.HSVToRGB((float)Value / 36 % 1, .6f, 1);
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (State == CellState.Open) SetHexColor(_openCellColor);
        else SetHexColor(_valueCellColor);
    }

    public void ResetCell()
    {
        if (State != CellState.Closed)
            State = CellState.Open;
        Value = 0;
        SetHexColor(_openCellColor);
        UpdateTextBoxValue();
    }
    
    public void Initiate(CellState state, int row, int column)
    {
        Initiate(state, new CellCoords(row, column));
    }

    public void Initiate(CellState state, CellCoords coords)
    {
        Coords = coords;
        State = state;

        if (state == CellState.Open)
        {
            valueTextBox.enabled = true;
            _sprite.enabled = true;
            SetHexColor(_openCellColor);
        }
    }

    public void SetOffsetDirection(bool rightOffset)
    {
        RightOffset = rightOffset;
    }

    public void Fill(int Value)
    {
        this.Value += Value;
        GameManager.Instance.UnlockNumber(this.Value);
        State = CellState.Filled;
        SetHexColor(_valueCellColor);
        UpdateTextBoxValue();
    }

    public void HighlightCell(bool x)
    {
        if (x) SetHexColor(_highlightCellColor);
        else if (State == CellState.Open) SetHexColor(_openCellColor);
        else SetHexColor(_valueCellColor);
    }

}
