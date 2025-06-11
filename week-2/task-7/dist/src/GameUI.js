"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GameUI = void 0;
// Simplified GameUI for demonstration purposes
class GameUI {
    constructor(game) {
        this.game = game;
    }
    /**
     * Display welcome message
     */
    displayWelcome() {
        return "Welcome to Sea Battle!\n" +
            `Try to sink the ${this.game.getConfig().numShips} enemy ships.\n` +
            "Boards created and ships placed randomly.";
    }
    /**
     * Display both game boards side by side as string
     */
    displayBoards() {
        const config = this.game.getConfig();
        const opponentBoard = this.createOpponentDisplayBoard();
        const playerBoard = this.game.getPlayer().getBoard().getGrid();
        let output = '\n   --- OPPONENT BOARD ---          --- YOUR BOARD ---\n';
        // Header with column numbers
        let header = '  ';
        for (let h = 0; h < config.boardSize; h++) {
            header += h + ' ';
        }
        output += header + '     ' + header + '\n';
        // Display rows
        for (let i = 0; i < config.boardSize; i++) {
            let rowStr = i + ' ';
            // Opponent board row
            for (let j = 0; j < config.boardSize; j++) {
                rowStr += opponentBoard[i][j] + ' ';
            }
            rowStr += '    ' + i + ' ';
            // Player board row
            for (let j = 0; j < config.boardSize; j++) {
                rowStr += playerBoard[i][j] + ' ';
            }
            output += rowStr + '\n';
        }
        output += '\n';
        return output;
    }
    /**
     * Create display version of opponent board (hide ships)
     */
    createOpponentDisplayBoard() {
        const aiBoard = this.game.getAIPlayer().getBoard().getGrid();
        const config = this.game.getConfig();
        const displayBoard = [];
        for (let i = 0; i < config.boardSize; i++) {
            displayBoard[i] = [];
            for (let j = 0; j < config.boardSize; j++) {
                const cell = aiBoard[i][j];
                // Hide ships ('S') on opponent board, show everything else
                displayBoard[i][j] = cell === 'S' ? '~' : cell;
            }
        }
        return displayBoard;
    }
    /**
     * Display game end message
     */
    displayGameEnd() {
        const boards = this.displayBoards();
        const winner = this.game.getWinner();
        let message = '';
        if (winner === 'Player') {
            message = '\n*** CONGRATULATIONS! You sunk all enemy battleships! ***\n';
        }
        else {
            message = '\n*** GAME OVER! The CPU sunk all your battleships! ***\n';
        }
        message += '\nThanks for playing Sea Battle!';
        return boards + message;
    }
    /**
     * Display game statistics
     */
    displayStats() {
        const playerGuesses = this.game.getPlayer().getGuesses();
        const aiGuesses = this.game.getAIPlayer().getGuesses();
        const gameState = this.game.getGameState();
        return '\n=== GAME STATISTICS ===\n' +
            `Player guesses: ${playerGuesses.length}\n` +
            `CPU guesses: ${aiGuesses.length}\n` +
            `Player ships remaining: ${gameState.playerShipsRemaining}\n` +
            `CPU ships remaining: ${gameState.cpuShipsRemaining}\n` +
            `Current AI mode: ${this.game.getAIPlayer().getCurrentMode()}\n` +
            `AI target queue size: ${this.game.getAIPlayer().getTargetQueueSize()}`;
    }
}
exports.GameUI = GameUI;
//# sourceMappingURL=GameUI.js.map