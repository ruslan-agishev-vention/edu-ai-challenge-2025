"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.PlayableGameUI = void 0;
const readline_1 = require("readline");
class PlayableGameUI {
    constructor(game) {
        this.game = game;
        this.rl = (0, readline_1.createInterface)({
            input: process.stdin,
            output: process.stdout,
        });
    }
    /**
     * Start the interactive game
     */
    async start() {
        this.displayWelcome();
        await this.gameLoop();
    }
    /**
     * Display welcome message
     */
    displayWelcome() {
        console.log('\n=== Modern Sea Battle Game ===');
        console.log(`Try to sink the ${this.game.getConfig().numShips} enemy ships.`);
        console.log('Ships placed randomly for both players.\n');
        console.log('Instructions:');
        console.log('- Enter coordinates as two digits (e.g., 00, 34, 56)');
        console.log('- ~ = water, S = your ships, X = hit, O = miss');
        console.log('- Good luck!\n');
    }
    /**
     * Main interactive game loop
     */
    async gameLoop() {
        while (!this.game.isGameOver()) {
            this.displayBoards();
            if (this.game.getGameState().currentTurn === 'player') {
                await this.handlePlayerTurn();
            }
            else {
                await this.handleAITurn();
            }
        }
        // Game is over
        this.displayGameEnd();
        await this.askPlayAgain();
    }
    /**
     * Handle player's turn with input validation
     */
    async handlePlayerTurn() {
        return new Promise((resolve) => {
            const askForGuess = () => {
                this.rl.question('Enter your guess (e.g., 00): ', (input) => {
                    const result = this.game.processPlayerGuess(input.trim());
                    console.log('\n' + result.message);
                    // Continue asking if input was invalid
                    if (result.alreadyGuessed &&
                        (result.message.includes('exactly two digits') ||
                            result.message.includes('valid row and column numbers'))) {
                        askForGuess();
                    }
                    else {
                        resolve();
                    }
                });
            };
            askForGuess();
        });
    }
    /**
     * Handle AI's turn with dramatic pause
     */
    async handleAITurn() {
        if (this.game.isGameOver())
            return;
        console.log("\n--- CPU's Turn ---");
        console.log('CPU is thinking...');
        // Add suspense
        await this.delay(1500);
        const result = this.game.processAITurn();
        console.log(result.message);
        // Brief pause to read the result
        await this.delay(1000);
    }
    /**
     * Display both game boards side by side
     */
    displayBoards() {
        const config = this.game.getConfig();
        const opponentBoard = this.createOpponentDisplayBoard();
        const playerBoard = this.game.getPlayer().getBoard().getGrid();
        console.log('\n   --- OPPONENT BOARD ---          --- YOUR BOARD ---');
        // Header with column numbers
        let header = '  ';
        for (let h = 0; h < config.boardSize; h++) {
            header += h + ' ';
        }
        console.log(header + '     ' + header);
        // Display rows
        for (let i = 0; i < config.boardSize; i++) {
            let rowStr = i + ' ';
            // Opponent board row (hide ships)
            for (let j = 0; j < config.boardSize; j++) {
                rowStr += opponentBoard[i][j] + ' ';
            }
            rowStr += '    ' + i + ' ';
            // Player board row (show ships)
            for (let j = 0; j < config.boardSize; j++) {
                rowStr += playerBoard[i][j] + ' ';
            }
            console.log(rowStr);
        }
        console.log();
    }
    /**
     * Create display version of opponent board (hide ships until hit)
     */
    createOpponentDisplayBoard() {
        const aiBoard = this.game.getAIPlayer().getBoard().getGrid();
        const config = this.game.getConfig();
        const displayBoard = [];
        for (let i = 0; i < config.boardSize; i++) {
            displayBoard[i] = [];
            for (let j = 0; j < config.boardSize; j++) {
                const cell = aiBoard[i][j];
                // Hide ships ('S') on opponent board, show hits/misses
                displayBoard[i][j] = cell === 'S' ? '~' : cell;
            }
        }
        return displayBoard;
    }
    /**
     * Display game end and final boards
     */
    displayGameEnd() {
        console.log('\n' + '='.repeat(50));
        console.log('GAME OVER!');
        console.log('='.repeat(50));
        // Show final boards with all ships revealed
        this.displayFinalBoards();
        const winner = this.game.getWinner();
        if (winner === 'Player') {
            console.log('\n🎉 CONGRATULATIONS! You sunk all enemy battleships! 🎉');
        }
        else {
            console.log('\n💥 GAME OVER! The CPU sunk all your battleships! 💥');
        }
        this.displayStats();
    }
    /**
     * Display final boards with all ships revealed
     */
    displayFinalBoards() {
        const config = this.game.getConfig();
        const aiBoard = this.game.getAIPlayer().getBoard().getGrid();
        const playerBoard = this.game.getPlayer().getBoard().getGrid();
        console.log('\n   --- FINAL OPPONENT BOARD ---      --- FINAL YOUR BOARD ---');
        let header = '  ';
        for (let h = 0; h < config.boardSize; h++) {
            header += h + ' ';
        }
        console.log(header + '     ' + header);
        for (let i = 0; i < config.boardSize; i++) {
            let rowStr = i + ' ';
            // Show all ships on opponent board
            for (let j = 0; j < config.boardSize; j++) {
                rowStr += aiBoard[i][j] + ' ';
            }
            rowStr += '    ' + i + ' ';
            // Show player board
            for (let j = 0; j < config.boardSize; j++) {
                rowStr += playerBoard[i][j] + ' ';
            }
            console.log(rowStr);
        }
    }
    /**
     * Display game statistics
     */
    displayStats() {
        const playerGuesses = this.game.getPlayer().getGuesses();
        const aiGuesses = this.game.getAIPlayer().getGuesses();
        const gameState = this.game.getGameState();
        console.log('\n--- GAME STATISTICS ---');
        console.log(`Your guesses: ${playerGuesses.length}`);
        console.log(`CPU guesses: ${aiGuesses.length}`);
        console.log(`Your ships remaining: ${gameState.playerShipsRemaining}`);
        console.log(`CPU ships remaining: ${gameState.cpuShipsRemaining}`);
        console.log(`Final AI mode: ${this.game.getAIPlayer().getCurrentMode()}`);
    }
    /**
     * Ask if player wants to play again
     */
    async askPlayAgain() {
        return new Promise((resolve) => {
            this.rl.question('\nWould you like to play again? (y/n): ', (answer) => {
                if (answer.toLowerCase().startsWith('y')) {
                    console.log('\nStarting new game...\n');
                    this.game.reset();
                    this.gameLoop().then(() => resolve());
                }
                else {
                    console.log('\nThanks for playing Sea Battle! Goodbye! 👋');
                    this.rl.close();
                    resolve();
                }
            });
        });
    }
    /**
     * Utility method for delays
     */
    delay(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }
    /**
     * Clean up resources
     */
    close() {
        this.rl.close();
    }
}
exports.PlayableGameUI = PlayableGameUI;
//# sourceMappingURL=PlayableGameUI.js.map