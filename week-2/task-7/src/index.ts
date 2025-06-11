import { Game } from './Game';
import { GameUI } from './GameUI';

// Demonstration of the modern Battleship game
function demonstrateGame(): void {
  // Create a new game instance
  const game = new Game();
  const ui = new GameUI(game);
  
  console.log('=== Modern Sea Battle Game Demo ===');
  console.log(ui.displayWelcome());
  console.log(ui.displayBoards());
  
  // Demonstrate some game moves
  console.log('Making some demo moves...\n');
  
  // Player makes a guess
  const playerResult = game.processPlayerGuess('00');
  console.log(`Player guesses 00: ${playerResult.message}`);
  
  if (game.getGameState().currentTurn === 'cpu') {
    // AI makes a guess
    const aiResult = game.processAITurn();
    console.log(`${aiResult.message}`);
  }
  
  // Show updated boards
  console.log(ui.displayBoards());
  
  // Show game statistics
  console.log(ui.displayStats());
  
  console.log('\nDemo complete! The game architecture is ready for full implementation.');
  console.log('To run the full game, implement readline input handling in the GameUI class.');
}

// Export classes for external use
export { Game, GameUI };

// Run demo if this file is executed directly
if (require.main === module) {
  demonstrateGame();
} 