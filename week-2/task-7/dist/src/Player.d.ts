import { Board } from './Board';
import { GameConfig } from './types';
export declare class Player {
    protected readonly name: string;
    protected readonly board: Board;
    protected readonly guesses: Set<string>;
    constructor(name: string, config: GameConfig);
    private setupShips;
    /**
     * Make a guess at the opponent's board
     * @param coordinate The coordinate to guess
     * @param opponentBoard The opponent's board
     */
    makeGuess(coordinate: string, opponentBoard: Board): {
        hit: boolean;
        sunk: boolean;
        alreadyGuessed: boolean;
    };
    /**
     * Receive a guess from the opponent
     * @param coordinate The coordinate being guessed
     */
    receiveGuess(coordinate: string): {
        hit: boolean;
        sunk: boolean;
    };
    /**
     * Check if this player has lost (all ships sunk)
     */
    hasLost(): boolean;
    /**
     * Get the player's name
     */
    getName(): string;
    /**
     * Get the player's board
     */
    getBoard(): Board;
    /**
     * Get all guesses made by this player
     */
    getGuesses(): string[];
    /**
     * Get the number of active ships remaining
     */
    getActiveShipsCount(): number;
    /**
     * Validate coordinate format (should be exactly 2 digits)
     */
    protected isValidCoordinateFormat(coordinate: string): boolean;
    /**
     * Parse coordinate string into row and column
     */
    protected parseCoordinate(coordinate: string): {
        row: number;
        col: number;
    };
}
//# sourceMappingURL=Player.d.ts.map