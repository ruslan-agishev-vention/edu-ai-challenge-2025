"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.AIPlayer = void 0;
const Player_1 = require("./Player");
class AIPlayer extends Player_1.Player {
    constructor(config) {
        super('CPU', config);
        this.mode = 'hunt';
        this.targetQueue = [];
        this.boardSize = config.boardSize;
    }
    /**
     * AI makes an intelligent guess using hunt/target strategy
     * @param opponentBoard The opponent's board
     */
    makeAIGuess(opponentBoard) {
        let coordinate;
        let attempts = 0;
        const maxAttempts = 100;
        do {
            if (this.mode === 'target' && this.targetQueue.length > 0) {
                coordinate = this.targetQueue.shift();
            }
            else {
                this.mode = 'hunt';
                coordinate = this.generateHuntModeGuess();
            }
            attempts++;
        } while ((this.guesses.has(coordinate) || opponentBoard.isAlreadyGuessed(coordinate)) &&
            attempts < maxAttempts);
        if (attempts >= maxAttempts) {
            throw new Error('AI unable to generate valid guess after maximum attempts');
        }
        // Record the guess
        this.guesses.add(coordinate);
        // Process the guess
        const result = opponentBoard.processGuess(coordinate);
        // Update AI state based on result
        this.updateAIState(coordinate, result.hit, result.sunk);
        return {
            coordinate,
            hit: result.hit,
            sunk: result.sunk
        };
    }
    /**
     * Generate a random coordinate for hunt mode
     */
    generateHuntModeGuess() {
        const row = Math.floor(Math.random() * this.boardSize);
        const col = Math.floor(Math.random() * this.boardSize);
        return `${row}${col}`;
    }
    /**
     * Update AI state based on guess result
     */
    updateAIState(coordinate, hit, sunk) {
        if (hit) {
            if (sunk) {
                // Ship sunk, return to hunt mode
                this.mode = 'hunt';
                this.targetQueue = [];
            }
            else {
                // Hit but not sunk, switch to target mode
                this.mode = 'target';
                this.addAdjacentTargets(coordinate);
            }
        }
        else {
            // Miss - if we were in target mode and have no more targets, return to hunt
            if (this.mode === 'target' && this.targetQueue.length === 0) {
                this.mode = 'hunt';
            }
        }
    }
    /**
     * Add adjacent coordinates to target queue
     */
    addAdjacentTargets(coordinate) {
        const { row, col } = this.parseCoordinate(coordinate);
        const adjacentCoords = [
            { row: row - 1, col }, // Up
            { row: row + 1, col }, // Down
            { row, col: col - 1 }, // Left
            { row, col: col + 1 } // Right
        ];
        for (const coord of adjacentCoords) {
            if (this.isValidTargetCoordinate(coord)) {
                const coordString = `${coord.row}${coord.col}`;
                if (!this.targetQueue.includes(coordString)) {
                    this.targetQueue.push(coordString);
                }
            }
        }
    }
    /**
     * Check if a coordinate is valid for targeting
     */
    isValidTargetCoordinate(coord) {
        return (coord.row >= 0 &&
            coord.row < this.boardSize &&
            coord.col >= 0 &&
            coord.col < this.boardSize &&
            !this.guesses.has(`${coord.row}${coord.col}`));
    }
    /**
     * Get current AI mode
     */
    getCurrentMode() {
        return this.mode;
    }
    /**
     * Get current target queue size
     */
    getTargetQueueSize() {
        return this.targetQueue.length;
    }
    /**
     * Get current target queue (for testing/debugging)
     */
    getTargetQueue() {
        return [...this.targetQueue];
    }
    /**
     * Reset AI state (useful for testing)
     */
    resetAIState() {
        this.mode = 'hunt';
        this.targetQueue = [];
    }
}
exports.AIPlayer = AIPlayer;
//# sourceMappingURL=AIPlayer.js.map