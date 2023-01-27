using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris {
	public class GameState {
		private Block currentBlock;

		public Block CurrentBlock {
			get => currentBlock;
			private set {
				currentBlock = value;
				currentBlock.Reset();

				for(int i = 0; i < 2; i++) {
					currentBlock.Move(1, 0);
					if(!BlockFits()) currentBlock.Move(-1, 0);
				}
			}
		}

		public GameGrid GameGrid { get; }
		public BlockQueue BlockQueue { get; }
		public bool GameOver { get; private set; }
		public int Score { private set; get; }

		public Block HeldBlock { get; private set; }
		public bool CanHold { get; private set; }

		public GameState() {
			GameGrid = new GameGrid(22, 10);
			BlockQueue = new BlockQueue();
			CurrentBlock = BlockQueue.GetAndUpdate();
			CanHold = true;
		}

		private bool BlockFits() {
			foreach(Position position in CurrentBlock.TilePositions()) {
				if(!GameGrid.IsEmpty(position.Row, position.Column)) {
					return false;
				}
			}
			return true;
		}

		public void HoldBlock() {
			if(!CanHold) return;

			if(HeldBlock == null) {
				HeldBlock = CurrentBlock;
				CurrentBlock = BlockQueue.GetAndUpdate();
			} else {
				Block tmp = currentBlock;
				CurrentBlock = HeldBlock;
				HeldBlock = tmp;
			}

			CanHold = false;
		}

		public void RotateBlockClockWise() {
			CurrentBlock.RotateClockWise();
			if(!BlockFits()) CurrentBlock.RotateCounterClockWise();
		}

		public void RotateBlockCounterClockWise() {
			CurrentBlock.RotateCounterClockWise();
			if(!BlockFits()) CurrentBlock.RotateClockWise();
		}

		public void MoveBlockLeft() {
			CurrentBlock.Move(0, -1);
			if(!BlockFits()) CurrentBlock.Move(0, 1);
		}

		public void MoveBlockRight() {
			CurrentBlock.Move(0, 1);
			if(!BlockFits()) CurrentBlock.Move(0, -1);
		}

		private bool IsGameOver() {
			return !(GameGrid.IsRowEmpty(0) && GameGrid.IsRowEmpty(1));
		}

		private void PlaceBlock() {
			foreach(Position position in CurrentBlock.TilePositions()) {
				GameGrid[position.Row, position.Column] = CurrentBlock.ID;
			}

			Score += GameGrid.ClearFullRows();

			if(IsGameOver()) GameOver = true;
			else {
				CurrentBlock = BlockQueue.GetAndUpdate();
				CanHold = true;
			}
		}

		public void MoveBlockDown() {
			CurrentBlock.Move(1, 0);
			if(!BlockFits()) {
				CurrentBlock.Move(-1, 0);
				PlaceBlock();
			}
		}

		private int TileDropDistance(Position position) {
			int drop = 0;
			while(GameGrid.IsEmpty(position.Row + drop + 1, position.Column)) {
				drop++;
			}

			return drop;
		}

		public int BlockDropDistance() {
			int drop = GameGrid.Rows;

			foreach(Position position in CurrentBlock.TilePositions()) {
				drop = System.Math.Min(drop, TileDropDistance(position));
			}
			return drop;
		}

		public void DropBlock() {
			CurrentBlock.Move(BlockDropDistance(), 0);
			PlaceBlock();
		}
	}
}
