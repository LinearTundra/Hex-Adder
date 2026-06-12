using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CellCoords
{
    public int row;
    public int column;

    public CellCoords(int row, int column)
    {
        this.row = row;
        this.column = column;
    }

    public override int GetHashCode() => HashCode.Combine(row, column);
    public override bool Equals(object obj) => obj is CellCoords c && c.row == row && c.column == column;
}

[Serializable]
public class CellCoordsSet : ISerializationCallbackReceiver
{
    [SerializeField]
    private List<CellCoords> cells;

    private HashSet<CellCoords> _set;
    private readonly CellCoords _placeholder = new CellCoords(0, -1);

    public void OnBeforeSerialize()
    {
        //cells.Clear();
        //foreach (CellCoords item in _set)
        //    cells.Add(item);
    }

    public void OnAfterDeserialize()
    {
        _set = new HashSet<CellCoords>(cells);
    }

    public int Count()
    {
        return _set.Count;
    }

    public bool Add(CellCoords item)
    {
        return _set.Add(item);
    }
    
    public bool Add(int row, int column)
    {
        return _set.Add(new CellCoords(row, column));
    }

    public bool Remove(CellCoords item)
    {
        return _set.Remove(item);
    }
    
    public bool Remove(int row, int column)
    {
        return _set.Remove(new CellCoords(row, column));
    }

    public bool Contains(CellCoords item)
    {
        return _set.Contains(item);
    }
    
    public bool Contains(int row, int column)
    {
        return _set.Contains(new CellCoords(row, column));
    }

    public void Clear()
    {
        _set.Clear();
        cells.Clear();
    }
}