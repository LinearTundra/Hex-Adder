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
    private float _effectiveRows;
    private float _effectiveColumns;

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

    /// <summary>
    /// Instantiates all the cells of the grid.
    /// </summary>
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
                
                if (unavailable.Contains(i, j)) _grid[i][j].Initiate(CellState.Closed, i, j);
                else _grid[i][j].Initiate(CellState.Open, i, j);
            }
        }
    }

    /// <summary>
    /// Positions all cells in the grid using hex geometry (alternating row stagger),
    /// simultaneously tracking the bounding box of open cells,
    /// then recenters the grid so only the open cell region is centered.
    /// </summary>
    private void PositionGrid()
    {
        float cellWidth = 2.24f;
        float cellHeight = 1.82f;

        // Hex stagger offset — 1/4.5 was empirically the right fit for these hex cells.
        // 1/4 was too much, 1/5 was too little.
        float offset = cellWidth / 4.5f;
        if (rows == 1) offset = 0;
        if (firstColumnRight) offset = -offset;

        // ─────── Place every cell and track open cell bounding box ─────────

        int minRow = rows, maxRow = 0;
        float minCol = columns + 1, maxCol = -1;

        for (int i = 0; i < rows; i++)
        {
            float cellSpacing = (i % 2 == 1) ? -offset : offset;

            for (int j = 0; j < columns; j++)
            {
                float x = (j - (columns - 1 + cellSpacing) / 2f) * cellWidth;
                float y = ((rows - 1) / 2f - i) * cellHeight;

                _grid[i][j].transform.localPosition = new Vector3(x, y, 0f);

                bool rightOffset = firstColumnRight ^ i % 2 == 1;
                _grid[i][j].SetOffsetDirection(rightOffset);

                if (_grid[i][j].State == CellState.Closed) continue;

                if (i < minRow) minRow = i;
                if (i > maxRow) maxRow = i;

                float offsetDir = rightOffset ? 0.25f : -0.25f;
                if (j < minCol) minCol = j + offsetDir;
                if (j > maxCol) maxCol = j + offsetDir;
            }
        }

        _effectiveRows = maxRow - minRow + 1;
        _effectiveColumns = maxCol - minCol + 1;

        // ─────── Shift all cells so open cell bounding box is centered ───────

        // minCol = offset form left side
        // maxCol + 1 - columns = offset from right side
        float xOffset = minCol + (maxCol + 1 - columns);
        float yOffset = minRow + (maxRow + 1 - rows);

        for (int i = 0; i < rows; i++)
            for (int j = 0; j < columns; j++)
                _grid[i][j].transform.localPosition += new Vector3(-xOffset, yOffset, 0f);
        // -xOffset as we need to correct the offset
        // +yOffset as measured offset is in the same direction as the correction direction
    }

    /// <summary>
    /// Scales the grid so it takes maximises the space it can occupy.
    /// </summary>
    private void ScaleGrid()
    {
        // approx actual width and height to scale them to fill the space
        float width = (_effectiveColumns + 1) * 2.35f;
        float height = (_effectiveRows + 1) * 1.9f;

        if (width > height)
            transform.localScale = transform.localScale * gridWidth / width;
        else 
            transform.localScale = transform.localScale * gridHeight / height;
        // scale * (max size / current size)
        //
        // scale = localScale
        // max size = the max size the grid can be, i.e. the space the cells should take
        // current size = current size of the grid, i.e. the space the cells are taking
    }

}