using UnityEngine;

public class Grid : MonoBehaviour
{

    [Header("Grid Configuration")]
    [SerializeField]
    private int rows;
    [SerializeField]
    private int columns;

    [SerializeField]
    private float gridWidth;
    [SerializeField]
    private float gridHeight;

    [SerializeField]
    private bool firstColumnRight;

    [SerializeField]
    [Tooltip("Cells in the grid that are unavailable or can not be filled.")]
    private CellCoordsSet unavailable;

    [Header("Game Objects")]
    [SerializeField]
    private Cell cell;

    private Cell[][] _grid;

    private void Awake()
    {
        GenerateGrid();
        PositionGrid();
        ScaleGrid();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWidth, gridHeight, 1));
    }

    private void resetGrid()
    {
        foreach (Cell[] row in _grid) 
            for (int i=0; i<columns; i++) 
                row[i].ResetValue();
    }

    private void GenerateGrid()
    {
        _grid = new Cell[rows][];
        for (int i = 0; i < rows; i++)
        {
            _grid[i] = new Cell[columns];
            for (int j = 0; j < columns; j++)
            {
                _grid[i][j] = Instantiate(cell);
                _grid[i][j].transform.parent = transform;
                // all cells are made child gameobjects to the grid object
                // this make it easier to scale them to fit the space neately
                // as now only the grid gameobject needs to be scaled
                
                if (unavailable.Contains(i, j)) _grid[i][j].CloseCell();
                else _grid[i][j].OpenCell();
            }
        }
    }

    private void PositionGrid()
    {
        // shift width and height of the hex cells
        float cellWidth = 2.24f;
        float cellHeight = 1.82f;
        // because the cells are hexagon, the cells come down less then they are apart
        

        // and because they are hexagons they need to be shifted to fit neately
        // doing 1/4 offset was not suffecient and 1/5 was too much
        float offset = cellWidth / 4.5f;
        if (rows == 1) offset = 0;
        if (firstColumnRight) offset = -offset; // offsets the first placed row to the right
        
        // cellSpacing is used to change the offset direction so the grid remains centered to a specific position
        // the offset is applied to each row but in alternate directions
        float cellSpacing;

        for (int i = 0; i < rows; i++)
        {
            if (i % 2 == 1) cellSpacing = -offset;
            else cellSpacing = offset;
            
            for (int j = 0; j < columns; j++)
            {
                float x = (j - (columns - 1 + cellSpacing) / 2f) * cellWidth;
                float y = ((rows - 3) / 2f - i) * cellHeight;
                // rows - 3 because after scaling the grid was not centered from top and bottom

                _grid[i][j].transform.localPosition = transform.position + new Vector3(x, y, 0f);
            }
        }
    }

    private void ScaleGrid()
    {
        (int effectiveRows, int effectiveColumns) = GetEffectiveGridSize();

        // approx actual width and height to scale them to fill the space
        float width = (effectiveColumns + 1) * 2.35f;
        float height = (effectiveRows + 1) * 1.9f;

        if (width > height)
            transform.localScale = transform.localScale / width * gridWidth;
        else 
            transform.localScale = transform.localScale / height * gridHeight;
        // scale * (max size / current size)
        //
        // scale = localScale
        // max size = the max size the grid can be, i.e. the space the cells should take
        // current size = current size of the grid, i.e. the space the cells are taking
    }

    private (int effectiveRows, int effectiveCols) GetEffectiveGridSize()
    {
        int minRow = rows, maxRow = 0;
        int minCol = columns, maxCol = 0;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (_grid[i][j].State == CellState.Closed) continue;

                if (i < minRow) minRow = i;
                if (i > maxRow) maxRow = i;
                if (j < minCol) minCol = j;
                if (j > maxCol) maxCol = j;
            }
        }

        return (maxRow - minRow + 1, maxCol - minCol + 1);
    }

}