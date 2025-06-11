"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.playGame = playGame;
const Game_1 = require("./Game");
const PlayableGameUI_1 = require("./PlayableGameUI");
async function playGame() {
    console.log('🚢 Starting Modern Battleship Game...');
    try {
        // Create a new game instance
        const game = new Game_1.Game();
        const ui = new PlayableGameUI_1.PlayableGameUI(game);
        // Handle graceful shutdown
        process.on('SIGINT', () => {
            console.log('\n\n🛑 Game interrupted. Thanks for playing!');
            ui.close();
            process.exit(0);
        });
        // Start the interactive game
        await ui.start();
    }
    catch (error) {
        console.error('❌ An error occurred during the game:', error);
        console.log('The game will exit.');
        process.exit(1);
    }
}
// Start the game if this file is run directly
if (require.main === module) {
    playGame().catch((error) => {
        console.error('💥 Fatal error:', error);
        process.exit(1);
    });
}
//# sourceMappingURL=play.js.map