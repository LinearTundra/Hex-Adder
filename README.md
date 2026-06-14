# Hex Adder

A casual mobile-style puzzle game built in Unity where players place numbered hex tiles on a grid, merging matching numbers to score points.

## Gameplay

- A piece consisting of two numbered hex tiles spawns at the bottom of the screen
- **Drag** the piece onto the grid to place it
- **Click** the piece to rotate it through 6 orientations
- Tiles can be placed on **empty cells** or **cells with the same number**
- Matching numbers **merge** — placing a 2 on a 2 creates a 4
- Newly discovered numbers are added to the spawn pool
- The game ends after **6 pieces** are placed, or when no valid placement exists
- Score = sum of all values on the board

## Project Structure

```
Assets/
├── _Scripts/       # All game logic
├── _Sprites/       # Hex tile and UI sprites
├── Prefab/         # Cell and Tile prefabs
└── Scenes/         # GamePlay scene
```

## Scripts Overview

| Script | Responsibility |
|---|---|
| `GameManager` | Singleton. Manages game state, events, unlocked number pool |
| `Grid` | Generates, positions, and scales the hex grid dynamically |
| `Cell` | Individual grid cell — tracks state (Open/Filled/Closed) and value |
| `Piece` | Holds two Tiles. Handles drag, rotation, and placement input |
| `Tile` | Single hex value unit. Detects nearest valid cell via trigger overlap |
| `Hex` | Abstract base for Cell and Tile — shared sprite and value logic |
| `GameOverUI` | Displays score, high score, and restart button on game over |
| `CellCoords` | Serializable row/column coordinate with equality support |

## Setup

1. Open in **Unity 6.2+**
2. Open `Assets/Scenes/GamePlay.unity`
3. Press Play

## Controls

| Input | Action |
|---|---|
| Left Click + Drag | Move piece |
| Left Click (no drag) | Rotate piece |
| Release on valid cells | Place piece |
| Release on invalid | Returns to spawn |
