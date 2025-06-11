"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Board = void 0;
const Ship_1 = require("./Ship");
class Board {
    constructor(size) {
        this.size = size;
        this.grid = this.initializeGrid();
        this.ships = [];
    }
    initializeGrid() {
        const grid = [];
        for (let i = 0; i < this.size; i++) {
            grid[i] = new Array(this.size).fill('~');
        }
        return grid;
    }
    /**
     * Place a ship randomly on the board
     * @param shipLength Length of the ship to place
     * @returns The placed ship or null if placement failed
     */
    placeShipRandomly(shipLength) {
        const maxAttempts = 100;
        let attempts = 0;
        while (attempts < maxAttempts) {
            const orientation = Math.random() < 0.5 ? 'horizontal' : 'vertical';
            const startCoord = this.getRandomStartCoordinate(shipLength, orientation);
            if (startCoord !== null && this.canPlaceShip(startCoord, shipLength, orientation)) {
                const ship = this.placeShip(startCoord, shipLength, orientation);
                return ship;
            }
            attempts++;
        }
        return null; // Failed to place ship after max attempts
    }
    getRandomStartCoordinate(shipLength, orientation) {
        if (orientation === 'horizontal') {
            const row = Math.floor(Math.random() * this.size);
            const col = Math.floor(Math.random() * (this.size - shipLength + 1));
            return { row, col };
        }
        else {
            const row = Math.floor(Math.random() * (this.size - shipLength + 1));
            const col = Math.floor(Math.random() * this.size);
            return { row, col };
        }
    }
    canPlaceShip(startCoord, shipLength, orientation) {
        for (let i = 0; i < shipLength; i++) {
            const row = orientation === 'horizontal' ? startCoord.row : startCoord.row + i;
            const col = orientation === 'horizontal' ? startCoord.col + i : startCoord.col;
            if (!this.isValidCoordinate(row, col)) {
                return false;
            }
            const gridRow = this.grid[row];
            if (!gridRow || gridRow[col] !== '~') {
                return false;
            }
        }
        return true;
    }
    placeShip(startCoord, shipLength, orientation) {
        const locations = [];
        for (let i = 0; i < shipLength; i++) {
            const row = orientation === 'horizontal' ? startCoord.row : startCoord.row + i;
            const col = orientation === 'horizontal' ? startCoord.col + i : startCoord.col;
            const gridRow = this.grid[row];
            if (!gridRow) {
                throw new Error('Invalid grid row during ship placement');
            }
            gridRow[col] = 'S';
            locations.push(`${row}${col}`);
        }
        const ship = new Ship_1.Ship(locations);
        this.ships.push(ship);
        return ship;
    }
    /**
     * Process a guess at the given coordinate
     * @param coordinate The coordinate string (e.g., "34")
     * @returns Object containing hit result and if a ship was sunk
     */
    processGuess(coordinate) {
        const { row, col } = this.parseCoordinate(coordinate);
        if (!this.isValidCoordinate(row, col)) {
            throw new Error('Invalid coordinate');
        }
        const gridRow = this.grid[row];
        if (!gridRow) {
            throw new Error('Invalid grid row');
        }
        // Check if already guessed
        if (gridRow[col] === 'X' || gridRow[col] === 'O') {
            throw new Error('Already guessed this location');
        }
        // Try to hit each ship
        for (const ship of this.ships) {
            if (ship.hit(coordinate)) {
                gridRow[col] = 'X'; // Hit
                const sunk = ship.isSunk();
                return { hit: true, sunk, ship };
            }
        }
        // Miss
        gridRow[col] = 'O';
        return { hit: false, sunk: false };
    }
    /**
     * Check if a coordinate is valid (within board bounds)
     */
    isValidCoordinate(row, col) {
        return row >= 0 && row < this.size && col >= 0 && col < this.size;
    }
    /**
     * Parse a coordinate string into row and column numbers
     */
    parseCoordinate(coordinate) {
        if (coordinate.length !== 2) {
            throw new Error('Coordinate must be exactly 2 digits');
        }
        const rowChar = coordinate.charAt(0);
        const colChar = coordinate.charAt(1);
        const row = parseInt(rowChar, 10);
        const col = parseInt(colChar, 10);
        if (isNaN(row) || isNaN(col)) {
            throw new Error('Invalid coordinate format');
        }
        return { row, col };
    }
    /**
     * Get the current state of a cell
     */
    getCellState(row, col) {
        if (!this.isValidCoordinate(row, col)) {
            throw new Error('Invalid coordinate');
        }
        const gridRow = this.grid[row];
        if (!gridRow) {
            throw new Error('Invalid row');
        }
        const cellState = gridRow[col];
        if (cellState === undefined) {
            throw new Error('Invalid column');
        }
        return cellState;
    }
    /**
     * Get a copy of the board grid
     */
    getGrid() {
        return this.grid.map(row => [...row]);
    }
    /**
     * Get all ships on the board
     */
    getShips() {
        return [...this.ships];
    }
    /**
     * Get the number of ships that are not sunk
     */
    getActiveShipsCount() {
        return this.ships.filter(ship => !ship.isSunk()).length;
    }
    /**
     * Get the board size
     */
    getSize() {
        return this.size;
    }
    /**
     * Check if a coordinate has already been guessed
     */
    isAlreadyGuessed(coordinate) {
        try {
            const { row, col } = this.parseCoordinate(coordinate);
            if (!this.isValidCoordinate(row, col)) {
                return false;
            }
            const gridRow = this.grid[row];
            if (!gridRow) {
                return false;
            }
            const cellState = gridRow[col];
            return cellState === 'X' || cellState === 'O';
        }
        catch {
            return false;
        }
    }
}
exports.Board = Board;
//# sourceMappingURL=Board.js.map