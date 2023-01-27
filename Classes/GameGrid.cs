using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris {
	public class GameGrid {
		private readonly int[,] grid;
		public int Rows { get; }
		public int Columns { get; }

		public int this[int row, int column] {
			get => grid[row, column];
			set => grid[row, column] = value;
		}

		public GameGrid(int numberOfRows, int numberOfColumns) {
			Rows = numberOfRows;
			Columns = numberOfColumns;
			grid = new int[numberOfRows, numberOfColumns];
		}

		public bool IsInside(int row, int column) => row >= 0 && row < Rows && column >= 0 && column < Columns;
		public bool IsEmpty(int row, int column) => IsInside(row, column) && grid[row, column] == 0;

		private bool IsRowFull(int row) {
			for(int column = 0; column < Columns; column++) {
				if(grid[row, column] == 0) return false;
			}
			return true;
		}

		public bool IsRowEmpty(int row) {
			for(int column = 0; column < Columns; column++) {
				if(grid[row, column] != 0) return false;
			}
			return true;
		}

		private void ClearRow(int row) {
			for(int column = 0; column < Columns; column++) {
				grid[row, column] = 0;
			}
		}

		private void MoveRowDown(int row, int numberOfRow) {
			for(int column = 0; column < Columns; column++) {
				grid[row + numberOfRow, column] = grid[row,column];
				grid[row, column] = 0;
			}
		}

		public int ClearFullRows() {
			int cleared = 0;
			for(int row = Rows-1; row >= 0; row--) {
				if(IsRowFull(row)) {
					ClearRow(row);
					cleared++;
				} else if(cleared > 0) {
					MoveRowDown(row, cleared);
				}
			}

			return cleared;
		}
	}
}
