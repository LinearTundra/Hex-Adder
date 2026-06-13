using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Cell : MonoBehaviour
{
    // 2.24 x, 1.82 y

    public int Value { get; private set; }
    public CellState State { get; private set; }
    public CellCoords Coords { get; private set; }
    public bool RightOffset { get; private set; }

    private SpriteRenderer _sprite;


    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _sprite.enabled = false;
        _sprite.color = Color.HSVToRGB(0, 0, 70);
    }

    public void ResetValue()
    {
        Value = 0;
        _sprite.color = Color.HSVToRGB(0, 0, 70);
    }

    public void Initiate(CellState state, CellCoords coords)
    {
        Coords = coords;
        State = state;

        if (state == CellState.Open)
        {
            _sprite.enabled = true;
            _sprite.color = Color.HSVToRGB(0, 0, 70);
        }
    }
    
    public void Initiate(CellState state, int row, int column)
    {
        Coords = new CellCoords(row, column);
        State = state;

        if (state == CellState.Open)
        {
            _sprite.enabled = true;
            _sprite.color = Color.HSVToRGB(0, 0, 70);
        }
    }

    public void SetOffsetDirection(bool rightOffset)
    {
        RightOffset = rightOffset;
    }

    public void Fill(int Value)
    {
        this.Value += Value;
        State = CellState.Filled;
        _sprite.color = Color.HSVToRGB(this.Value * 10, 100, 100);
    }

}
