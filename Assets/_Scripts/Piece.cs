using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Empty GameObject parent of two Tiles.<br></br>
/// Handles all mouse input via new Input System raycasting against Tile colliders.<br></br>
/// Handles drag, rotation (6 orientations), placement validation,
/// and snap-back on invalid drop.
/// </summary>
public class Piece : MonoBehaviour
{
    [Header("GameObject")]
    [SerializeField] 
    private Tile tileA;
    [SerializeField] 
    private Tile tileB;
    [SerializeField]
    private Transform rotateIcon;
    [SerializeField]
    private TMP_Text remainingPiecesTextBox;

    [Header("Parameters")]
    [SerializeField]
    private int availablePieces;

    // The 6 hex orientations as offsets of tileB relative to tileA
    // tileA and tileB are shifted half of each vector in opposite sides to maintain center of mass.
    private static readonly Vector3[] Orientations = new Vector3[]
    {
        new Vector3( 2.24f,  0f,     0f), // 0: right
        new Vector3( 1.12f, -1.82f,  0f), // 1: lower-right
        new Vector3(-1.12f, -1.82f,  0f), // 2: lower-left
        new Vector3(-2.24f,  0f,     0f), // 3: left
        new Vector3(-1.12f,  1.82f,  0f), // 4: upper-left
        new Vector3( 1.12f,  1.82f,  0f), // 5: upper-right
    };

    private int _usedPieces;
    private int _orientationIndex = 0;
    private bool _isDragging = false;
    private bool _clickedThisPress = false; // true if press didn't move far enough to count as drag
    private Vector3 _spawnPosition;
    private Vector3 _dragOffset;
    private Vector3 _pressPosition;
    private Rigidbody2D _rb;
    private Camera _cam;

    private const float DragThreshold = 0.1f; // world units before a press counts as a drag

    private void Awake()
    {
        _cam = Camera.main;
        _rb = GetComponent<Rigidbody2D>();
        _spawnPosition = transform.position;
    }

    private void Start()
    {
        transform.localScale *= GameManager.Instance.GetHexScale();
        Initialise();
    }

    private void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 mouseWorld = GetMouseWorldPos();

        if (mouse.leftButton.wasPressedThisFrame)
            OnPress(mouseWorld);

        if (mouse.leftButton.isPressed && _isDragging)
            OnDrag(mouseWorld);

        if (mouse.leftButton.wasReleasedThisFrame)
            OnRelease(mouseWorld);
    }

    private void OnEnable()
    {
        GameManager.OnGameReset -= ResetGame;
        GameManager.OnGameOver -= GameOver;
        GameManager.OnGameReset += ResetGame;
        GameManager.OnGameOver += GameOver;
    }

    private void OnApplicationQuit()
    {
        GameManager.OnGameReset -= ResetGame;
        GameManager.OnGameOver -= GameOver;
    }

    public void Initialise()
    {
        _usedPieces = 0;
        remainingPiecesTextBox.text = $"x{availablePieces}";
        GeneratePiece();
    }

    private void GeneratePiece()
    {
        transform.position = _spawnPosition;
        _orientationIndex = Random.Range(0, Orientations.Length);
        (int a, int b) = GameManager.Instance.GetValidPiece();
        tileA.SetValue(a);
        tileB.SetValue(b);
        ApplyOrientation();

    }

    // ── Input ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Handles a left mouse button press. Raycasts to check if either tile was clicked.<br></br>
    /// If hit, begins drag and records press position for click-vs-drag detection.<br></br>
    /// </summary>
    private void OnPress(Vector2 mouseWorld)
    {
        // Only respond if click hit one of our tiles
        RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero);
        if (hit.collider == null) return;

        bool hitA = hit.collider == tileA.GetComponent<Collider2D>();
        bool hitB = hit.collider == tileB.GetComponent<Collider2D>();
        if (!hitA && !hitB) return;

        _pressPosition = mouseWorld;
        _dragOffset = transform.position - (Vector3)mouseWorld;
        _isDragging = true;
        _clickedThisPress = true;
    }

    /// <summary>
    /// Called every frame while the left mouse button is held and dragging is active.<br></br>
    /// Moves the piece via Rigidbody2D.MovePosition so physics-based trigger detection works.<br></br>
    /// Cancels click intent if the mouse has moved past DragThreshold from the press position.<br></br>
    /// Keeps the rotate icon anchored at the spawn point while dragging.
    /// </summary>
    private void OnDrag(Vector2 mouseWorld)
    {
        // If moved past threshold, it's a drag not a click
        if (_clickedThisPress && Vector2.Distance(mouseWorld, _pressPosition) > DragThreshold)
            _clickedThisPress = false;

        rotateIcon.position = _spawnPosition + Vector3.forward;
        _rb.MovePosition(mouseWorld + (Vector2)_dragOffset);
        //transform.position = (Vector3)mouseWorld + _dragOffset;
        UpdateHighlight();
    }

    /// <summary>
    /// Called when the left mouse button is released.<br></br>
    /// If the press was a short click (no drag), rotates the piece and returns it to spawn.<br></br>
    /// If both tiles are over valid, distinct cells, places the piece.<br></br>
    /// Otherwise returns the piece to its spawn position.
    /// </summary>
    private void OnRelease(Vector2 mouseWorld)
    {
        if (!_isDragging) return;
        _isDragging = false;

        if (_clickedThisPress)
        {
            // Short press with no drag = rotate
            ReturnToSpawn();
            Rotate();
        }
        if (tileA.IsPlaceable && tileB.IsPlaceable && tileA.HoveredCell != tileB.HoveredCell)
            PlacePiece();
        else
            ReturnToSpawn();

        _clickedThisPress = false;
        rotateIcon.position = _spawnPosition + Vector3.forward;
    }

    // ── Internal ────────────────────────────────────────────────────────────

    private void Rotate()
    {
        ClearHighlight();
        _orientationIndex = (_orientationIndex + 1) % Orientations.Length;
        ApplyOrientation();
    }

    private void ApplyOrientation()
    {
        tileA.transform.localPosition = -Orientations[_orientationIndex]/2;
        tileB.transform.localPosition = Orientations[_orientationIndex]/2;
    }

    private void UpdateHighlight()
    {
        bool bothValid = tileA.IsPlaceable && tileB.IsPlaceable && tileA.HoveredCell != tileB.HoveredCell;
        tileA.HighlightHoveredCell(bothValid);
        tileB.HighlightHoveredCell(bothValid);
    }

    private void ClearHighlight()
    {
        tileA.HighlightHoveredCell(false);
        tileB.HighlightHoveredCell(false);
    }

    private void PlacePiece()
    {
        _usedPieces++;
        ClearHighlight();
        tileA.Place();
        tileB.Place();

        remainingPiecesTextBox.text = $"x{availablePieces - _usedPieces}";

        if (_usedPieces == availablePieces) GameManager.Instance.GameOver();
        else GeneratePiece();
    }

    private void ReturnToSpawn()
    {
        ClearHighlight();
        transform.position = _spawnPosition;
    }

    private Vector2 GetMouseWorldPos()
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        return _cam.ScreenToWorldPoint(screenPos);
    }

    private void ResetGame()
    {
        gameObject.SetActive(true);
        Initialise();
    }

    private void GameOver()
    {
        gameObject.SetActive(false);
        remainingPiecesTextBox.text = "";
    }

}