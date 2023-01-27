using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tetris {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		private readonly ImageSource[] tileImages = new ImageSource[] {
			new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
			new BitmapImage(new Uri("Assets/TileCyan.png", UriKind.Relative)),
			new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
			new BitmapImage(new Uri("Assets/TileOrange.png", UriKind.Relative)),
			new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
			new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
			new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
			new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative))
		};

		private readonly ImageSource[] blockImages = new ImageSource[] {
			new BitmapImage(new Uri("Assets/Block-Empty.png", UriKind.Relative)),
			new BitmapImage(new Uri("Assets/Block-I.png", UriKind.Relative)),
			new BitmapImage(new Uri("Assets/Block-J.png", UriKind.Relative)),
			new BitmapImage(new Uri("Assets/Block-L.png", UriKind.Relative)),
			new BitmapImage(new Uri("Assets/Block-O.png", UriKind.Relative)),
			new BitmapImage(new Uri("Assets/Block-S.png", UriKind.Relative)),
			new BitmapImage(new Uri("Assets/Block-T.png", UriKind.Relative)),
			new BitmapImage(new Uri("Assets/Block-Z.png", UriKind.Relative))
		};

		private readonly Image[,] imageControls;
		private readonly int maxDelay = 1000;
		private readonly int minDelay = 75;
		private readonly int delayDecreaseRate = 25;

		private GameState gameState = new GameState();

		public MainWindow() {
			InitializeComponent();
			imageControls = SetupGameCanvas(gameState.GameGrid);
		}

		private Image[,] SetupGameCanvas(GameGrid gameGrid) {
			Image[,] imageControls = new Image[gameGrid.Rows, gameGrid.Columns];
			int cellSize = 25;

			for(int row = 0; row < gameGrid.Rows; row++) {
				for(int column = 0; column < gameGrid.Columns; column++) {
					Image imageControl = new Image {
						Width = cellSize,
						Height = cellSize
					};

					Canvas.SetTop(imageControl, (row - 2) * cellSize + 10);
					Canvas.SetLeft(imageControl, column * cellSize);
					GameCanvas.Children.Add(imageControl);
					imageControls[row, column] = imageControl;
				}
			}

			return imageControls;
		}

		private void DrawGrid(GameGrid gameGrid) {
			for(int row = 0; row < gameGrid.Rows; row++) {
				for(int column = 0; column < gameGrid.Columns; column++) {
					int id = gameGrid[row, column];
					imageControls[row, column].Opacity = 1;
					imageControls[row, column].Source = tileImages[id];
				}
			}
		}

		private void DrawBlock(Block block) {
			foreach(Position position in block.TilePositions()) {
				imageControls[position.Row, position.Column].Opacity = 1;
				imageControls[position.Row, position.Column].Source = tileImages[block.ID];
			}
		}

		private void DrawNextBlock(BlockQueue blockQueue) {
			Block nextBlock = blockQueue.NextBlock;
			NextImage.Source = blockImages[nextBlock.ID];
		}

		private void DrawHeldBlock(Block heldBlock) {
			HoldImage.Source = heldBlock == null ? blockImages[0] : blockImages[heldBlock.ID];
		}

		private void DrawGhostBlock(Block block) {
			int dropDistance = gameState.BlockDropDistance();

			foreach(Position position in block.TilePositions()) {
				imageControls[position.Row + dropDistance, position.Column].Opacity = 0.25;
				imageControls[position.Row + dropDistance, position.Column].Source = tileImages[block.ID];
			}
		}

		private void Draw(GameState gameState) {
			DrawGrid(gameState.GameGrid);
			DrawGhostBlock(gameState.CurrentBlock);
			DrawBlock(gameState.CurrentBlock);
			DrawNextBlock(gameState.BlockQueue);
			DrawHeldBlock(gameState.HeldBlock);
			ScoreText.Text = $"Score: {gameState.Score}";
		}

		private async Task GameLoop() {
			Draw(gameState);

			while(!gameState.GameOver) {
				int delay = Math.Max(minDelay, maxDelay - (gameState.Score * delayDecreaseRate));
				await Task.Delay(delay);
				gameState.MoveBlockDown();
				Draw(gameState);
			}

			GameOverMenu.Visibility = Visibility.Visible;
			FinalScoreText.Text = $"Score: {gameState.Score}";
		}

		private void Window_KeyDown(object sender, KeyEventArgs e) {
			if(gameState.GameOver) return;

			switch(e.Key) {
				case Key.Left:
					gameState.MoveBlockLeft();
					break;
				case Key.Right:
					gameState.MoveBlockRight();
					break;
				case Key.Down:
					gameState.MoveBlockDown();
					break;
				case Key.Up:
					gameState.RotateBlockClockWise();
					break;
				case Key.Z:
					gameState.RotateBlockCounterClockWise();
					break;
				case Key.C:
					gameState.HoldBlock();
					break;
				case Key.Space:
					gameState.DropBlock();
					break;
				default:
					return;
			}

			Draw(gameState);
		}

		private async void GameCanvas_Loaded(object sender, RoutedEventArgs e) {
			await GameLoop();
		}

		private async void PlayAgain_Click(object sender, RoutedEventArgs e) {
			gameState = new GameState();
			GameOverMenu.Visibility = Visibility.Hidden;
			await GameLoop();
		}
	}
}
