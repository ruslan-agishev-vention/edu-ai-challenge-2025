import { Player } from './Player';
import { AIPlayer } from './AIPlayer';
import { GameConfig, GameResult, GameState } from './types';
export declare class Game {
    private readonly config;
    private readonly player;
    private readonly aiPlayer;
    private gameState;
    constructor(config?: Partial<GameConfig>);
    /**
     * Process a player's guess
     * @param coordinate The coordinate string (e.g., "34")
     * @returns Result of the guess including game state updates
     */
    processPlayerGuess(coordinate: string): {
        hit: boolean;
        sunk: boolean;
        alreadyGuessed: boolean;
        gameResult: GameResult;
        message: string;
    };
    /**
     * Process the AI's turn
     * @returns Result of the AI's guess
     */
    processAITurn(): {
        coordinate: string;
        hit: boolean;
        sunk: boolean;
        gameResult: GameResult;
        message: string;
    };
    /**
     * Get the current game state
     */
    getGameState(): GameState;
    /**
     * Get the game configuration
     */
    getConfig(): GameConfig;
    /**
     * Get the human player
     */
    getPlayer(): Player;
    /**
     * Get the AI player
     */
    getAIPlayer(): AIPlayer;
    /**
     * Check if the game is over
     */
    isGameOver(): boolean;
    /**
     * Get the winner of the game
     */
    getWinner(): string | null;
    /**
     * Reset the game to initial state
     */
    reset(): void;
    /**
     * Validate coordinate format
     */
    private isValidCoordinateFormat;
    /**
     * Parse coordinate string
     */
    private parseCoordinate;
    /**
     * Validate coordinate bounds
     */
    private isValidCoordinateBounds;
}
//# sourceMappingURL=Game.d.ts.map