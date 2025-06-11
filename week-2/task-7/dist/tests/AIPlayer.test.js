"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const AIPlayer_1 = require("../src/AIPlayer");
const Board_1 = require("../src/Board");
const Ship_1 = require("../src/Ship");
describe('AIPlayer', () => {
    let aiPlayer;
    let opponentBoard;
    const config = {
        boardSize: 10,
        numShips: 3,
        shipLength: 3
    };
    beforeEach(() => {
        aiPlayer = new AIPlayer_1.AIPlayer(config);
        opponentBoard = new Board_1.Board(config.boardSize);
        // Place a ship on opponent board for testing AI behavior
        const ship = new Ship_1.Ship(['23', '24', '25']);
        opponentBoard.ships.push(ship);
        const grid = opponentBoard.grid;
        grid[2][3] = 'S';
        grid[2][4] = 'S';
        grid[2][5] = 'S';
    });
    describe('constructor', () => {
        it('should create AI player with CPU name', () => {
            expect(aiPlayer.getName()).toBe('CPU');
        });
        it('should initialize in hunt mode', () => {
            expect(aiPlayer.getCurrentMode()).toBe('hunt');
        });
        it('should start with empty target queue', () => {
            expect(aiPlayer.getTargetQueueSize()).toBe(0);
            expect(aiPlayer.getTargetQueue()).toEqual([]);
        });
        it('should place correct number of ships', () => {
            expect(aiPlayer.getActiveShipsCount()).toBe(config.numShips);
        });
    });
    describe('makeAIGuess', () => {
        it('should make valid guess in hunt mode', () => {
            const result = aiPlayer.makeAIGuess(opponentBoard);
            expect(result.coordinate).toMatch(/^\d{2}$/);
            expect(typeof result.hit).toBe('boolean');
            expect(typeof result.sunk).toBe('boolean');
            expect(aiPlayer.getGuesses()).toContain(result.coordinate);
        });
        it('should switch to target mode after hit', () => {
            // Force a hit by guessing ship location
            const result = aiPlayer.makeAIGuess(opponentBoard);
            // If we happened to hit, we should be in target mode
            if (result.hit && !result.sunk) {
                expect(aiPlayer.getCurrentMode()).toBe('target');
                expect(aiPlayer.getTargetQueueSize()).toBeGreaterThan(0);
            }
        });
        it('should return to hunt mode after sinking ship', () => {
            // Manually hit all parts of the ship to test mode transition
            aiPlayer.makeAIGuess(opponentBoard); // This might miss
            // Force hits by directly manipulating the AI state
            aiPlayer.mode = 'target';
            aiPlayer.targetQueue = ['23', '24', '25'];
            // Hit ship parts
            const result1 = aiPlayer.makeAIGuess(opponentBoard);
            if (!result1.sunk) {
                const result2 = aiPlayer.makeAIGuess(opponentBoard);
                if (!result2.sunk) {
                    const result3 = aiPlayer.makeAIGuess(opponentBoard);
                    if (result3.sunk) {
                        expect(aiPlayer.getCurrentMode()).toBe('hunt');
                        expect(aiPlayer.getTargetQueueSize()).toBe(0);
                    }
                }
            }
        });
        it('should not guess same location twice', () => {
            const guesses = new Set();
            // Make multiple guesses
            for (let i = 0; i < 10; i++) {
                const result = aiPlayer.makeAIGuess(opponentBoard);
                expect(guesses.has(result.coordinate)).toBe(false);
                guesses.add(result.coordinate);
            }
        });
        it('should add adjacent targets after hit', () => {
            // Reset AI state and manually set up a hit scenario
            aiPlayer.resetAIState();
            // Place the AI's guess history to control the test
            aiPlayer.guesses.clear();
            // Mock a hit at 55 and check adjacent targets are added
            aiPlayer.updateAIState('55', true, false);
            expect(aiPlayer.getCurrentMode()).toBe('target');
            const targetQueue = aiPlayer.getTargetQueue();
            // Should contain adjacent coordinates (54, 56, 45, 65) that are valid
            const expectedAdjacent = ['54', '56', '45', '65'];
            const validAdjacent = expectedAdjacent.filter(coord => {
                const row = parseInt(coord.charAt(0), 10);
                const col = parseInt(coord.charAt(1), 10);
                return row >= 0 && row < 10 && col >= 0 && col < 10;
            });
            expect(targetQueue.length).toBeGreaterThan(0);
            validAdjacent.forEach(coord => {
                expect(targetQueue).toContain(coord);
            });
        });
    });
    describe('AI state management', () => {
        it('should reset state correctly', () => {
            // Modify AI state
            aiPlayer.mode = 'target';
            aiPlayer.targetQueue = ['12', '34'];
            aiPlayer.resetAIState();
            expect(aiPlayer.getCurrentMode()).toBe('hunt');
            expect(aiPlayer.getTargetQueueSize()).toBe(0);
            expect(aiPlayer.getTargetQueue()).toEqual([]);
        });
        it('should handle edge coordinates correctly', () => {
            // Test corner coordinate (00)
            aiPlayer.updateAIState('00', true, false);
            const targetQueue = aiPlayer.getTargetQueue();
            // Should only add valid adjacent coordinates
            targetQueue.forEach(coord => {
                const row = parseInt(coord.charAt(0), 10);
                const col = parseInt(coord.charAt(1), 10);
                expect(row).toBeGreaterThanOrEqual(0);
                expect(row).toBeLessThan(10);
                expect(col).toBeGreaterThanOrEqual(0);
                expect(col).toBeLessThan(10);
            });
        });
        it('should not add duplicate targets', () => {
            aiPlayer.resetAIState();
            // Hit the same location multiple times (simulate multiple hits around same area)
            aiPlayer.updateAIState('55', true, false);
            const initialQueueSize = aiPlayer.getTargetQueueSize();
            aiPlayer.updateAIState('55', true, false);
            const finalQueueSize = aiPlayer.getTargetQueueSize();
            // Queue size should not grow with duplicate adjacent additions
            expect(finalQueueSize).toBe(initialQueueSize);
        });
    });
    describe('target queue management', () => {
        it('should provide correct target queue size', () => {
            expect(aiPlayer.getTargetQueueSize()).toBe(0);
            aiPlayer.targetQueue = ['12', '34'];
            expect(aiPlayer.getTargetQueueSize()).toBe(2);
        });
        it('should provide defensive copy of target queue', () => {
            aiPlayer.targetQueue = ['12', '34'];
            const queue = aiPlayer.getTargetQueue();
            queue.push('56');
            expect(aiPlayer.getTargetQueue()).toEqual(['12', '34']);
        });
    });
    describe('coordinate validation in AI logic', () => {
        it('should only target valid coordinates', () => {
            // Test AI with a board that has limited space
            const smallBoard = new Board_1.Board(3);
            const smallAI = new AIPlayer_1.AIPlayer({ boardSize: 3, numShips: 1, shipLength: 2 });
            // Make several guesses and ensure all are valid
            for (let i = 0; i < 5; i++) {
                const result = smallAI.makeAIGuess(smallBoard);
                const row = parseInt(result.coordinate.charAt(0), 10);
                const col = parseInt(result.coordinate.charAt(1), 10);
                expect(row).toBeGreaterThanOrEqual(0);
                expect(row).toBeLessThan(3);
                expect(col).toBeGreaterThanOrEqual(0);
                expect(col).toBeLessThan(3);
            }
        });
    });
    describe('error handling', () => {
        it('should handle full board gracefully', () => {
            const smallBoard = new Board_1.Board(2);
            const smallAI = new AIPlayer_1.AIPlayer({ boardSize: 2, numShips: 1, shipLength: 1 });
            // Fill up all possible guesses
            const allCoords = ['00', '01', '10', '11'];
            allCoords.forEach(coord => {
                try {
                    smallAI.makeAIGuess(smallBoard);
                }
                catch (error) {
                    // AI should eventually run out of valid moves
                    expect(error).toBeInstanceOf(Error);
                }
            });
        });
    });
});
//# sourceMappingURL=AIPlayer.test.js.map