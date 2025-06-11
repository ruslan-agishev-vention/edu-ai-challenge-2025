import { CellState } from './types';
import { Ship } from './Ship';
export declare class Board {
    private readonly size;
    private readonly grid;
    private readonly ships;
    constructor(size: number);
    private initializeGrid;
    /**
     * Place a ship randomly on the board
     * @param shipLength Length of the ship to place
     * @returns The placed ship or null if placement failed
     */
    placeShipRandomly(shipLength: number): Ship | null;
    private getRandomStartCoordinate;
    private canPlaceShip;
    private placeShip;
    /**
     * Process a guess at the given coordinate
     * @param coordinate The coordinate string (e.g., "34")
     * @returns Object containing hit result and if a ship was sunk
     */
    processGuess(coordinate: string): {
        hit: boolean;
        sunk: boolean;
        ship?: Ship;
    };
    /**
     * Check if a coordinate is valid (within board bounds)
     */
    isValidCoordinate(row: number, col: number): boolean;
    /**
     * Parse a coordinate string into row and column numbers
     */
    private parseCoordinate;
    /**
     * Get the current state of a cell
     */
    getCellState(row: number, col: number): CellState;
    /**
     * Get a copy of the board grid
     */
    getGrid(): CellState[][];
    /**
     * Get all ships on the board
     */
    getShips(): Ship[];
    /**
     * Get the number of ships that are not sunk
     */
    getActiveShipsCount(): number;
    /**
     * Get the board size
     */
    getSize(): number;
    /**
     * Check if a coordinate has already been guessed
     */
    isAlreadyGuessed(coordinate: string): boolean;
}
//# sourceMappingURL=Board.d.ts.map