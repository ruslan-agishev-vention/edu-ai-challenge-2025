import { Player } from './Player';
import { Board } from './Board';
import { GameConfig, AIMode } from './types';
export declare class AIPlayer extends Player {
    private mode;
    private targetQueue;
    private readonly boardSize;
    constructor(config: GameConfig);
    /**
     * AI makes an intelligent guess using hunt/target strategy
     * @param opponentBoard The opponent's board
     */
    makeAIGuess(opponentBoard: Board): {
        coordinate: string;
        hit: boolean;
        sunk: boolean;
    };
    /**
     * Generate a random coordinate for hunt mode
     */
    private generateHuntModeGuess;
    /**
     * Update AI state based on guess result
     */
    private updateAIState;
    /**
     * Add adjacent coordinates to target queue
     */
    private addAdjacentTargets;
    /**
     * Check if a coordinate is valid for targeting
     */
    private isValidTargetCoordinate;
    /**
     * Get current AI mode
     */
    getCurrentMode(): AIMode;
    /**
     * Get current target queue size
     */
    getTargetQueueSize(): number;
    /**
     * Get current target queue (for testing/debugging)
     */
    getTargetQueue(): string[];
    /**
     * Reset AI state (useful for testing)
     */
    resetAIState(): void;
}
//# sourceMappingURL=AIPlayer.d.ts.map