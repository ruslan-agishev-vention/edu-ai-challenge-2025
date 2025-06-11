import { Game } from './Game';
export declare class PlayableGameUI {
    private readonly game;
    private readonly rl;
    constructor(game: Game);
    /**
     * Start the interactive game
     */
    start(): Promise<void>;
    /**
     * Display welcome message
     */
    private displayWelcome;
    /**
     * Main interactive game loop
     */
    private gameLoop;
    /**
     * Handle player's turn with input validation
     */
    private handlePlayerTurn;
    /**
     * Handle AI's turn with dramatic pause
     */
    private handleAITurn;
    /**
     * Display both game boards side by side
     */
    private displayBoards;
    /**
     * Create display version of opponent board (hide ships until hit)
     */
    private createOpponentDisplayBoard;
    /**
     * Display game end and final boards
     */
    private displayGameEnd;
    /**
     * Display final boards with all ships revealed
     */
    private displayFinalBoards;
    /**
     * Display game statistics
     */
    private displayStats;
    /**
     * Ask if player wants to play again
     */
    private askPlayAgain;
    /**
     * Utility method for delays
     */
    private delay;
    /**
     * Clean up resources
     */
    close(): void;
}
//# sourceMappingURL=PlayableGameUI.d.ts.map