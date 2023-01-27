# Tetris
Tetris made from C# WPF

## Controls
- Left Arrow: Move current block to the left.
- Right Arrow: Move current block to the right.
- Up Arrow: Rotate current block clockwise.
- Z: Rotate current block counterclockwise.
- Down Arrow: Move current block down by 1 row.
- Space: Instantly drop down the current block.
- C: Put the current block on hold; or swap the current block for the current block on hold.

## List of Classes
- GameGrid.cs: Handles the grid generation and behaviour.
- GameState.cs: Handles the game's states and data.
- Position.cs: Position of a block in the grid.
- BlockQueue.cs: Handles blocks generation.
- Block: An abstract class where that represent a block. Base class of all block classes:
  - IBlock
  - JBlock
  - LBlock
  - OBlock
  - SBlock
  - TBlock
  - ZBlock
- MainWindow.xaml.cs: Handles the UI behaviour and controls.
