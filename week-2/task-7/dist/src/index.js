"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GameUI = exports.Game = void 0;
const Game_1 = require("./Game");
Object.defineProperty(exports, "Game", { enumerable: true, get: function () { return Game_1.Game; } });
const GameUI_1 = require("./GameUI");
Object.defineProperty(exports, "GameUI", { enumerable: true, get: function () { return GameUI_1.GameUI; } });
// Demonstration of the modern Battleship game
function demonstrateGame() {
    // Create a new game instance
    const game = new Game_1.Game();
    const ui = new GameUI_1.GameUI(game);
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
// Run demo if this file is executed directly
if (require.main === module) {
    demonstrateGame();
}
//# sourceMappingURL=index.js.map