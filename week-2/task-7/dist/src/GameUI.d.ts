import { Game } from './Game';
export declare class GameUI {
    private readonly game;
    constructor(game: Game);
    /**
     * Display welcome message
     */
    displayWelcome(): string;
    /**
     * Display both game boards side by side as string
     */
    displayBoards(): string;
    /**
     * Create display version of opponent board (hide ships)
     */
    private createOpponentDisplayBoard;
    /**
     * Display game end message
     */
    displayGameEnd(): string;
    /**
     * Display game statistics
     */
    displayStats(): string;
}
//# sourceMappingURL=GameUI.d.ts.map