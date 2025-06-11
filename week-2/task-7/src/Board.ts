import { CellState, Coordinate } from './types';
import { Ship } from './Ship';

export class Board {
  private readonly size: number;
  private readonly grid: CellState[][];
  private readonly ships: Ship[];

  constructor(size: number) {
    this.size = size;
    this.grid = this.initializeGrid();
    this.ships = [];
  }

  private initializeGrid(): CellState[][] {
    const grid: CellState[][] = [];
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
  placeShipRandomly(shipLength: number): Ship | null {
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

  private getRandomStartCoordinate(shipLength: number, orientation: string): Coordinate | null {
    if (orientation === 'horizontal') {
      const row = Math.floor(Math.random() * this.size);
      const col = Math.floor(Math.random() * (this.size - shipLength + 1));
      return { row, col };
    } else {
      const row = Math.floor(Math.random() * (this.size - shipLength + 1));
      const col = Math.floor(Math.random() * this.size);
      return { row, col };
    }
  }

  private canPlaceShip(startCoord: Coordinate, shipLength: number, orientation: string): boolean {
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

  private placeShip(startCoord: Coordinate, shipLength: number, orientation: string): Ship {
    const locations: string[] = [];

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

    const ship = new Ship(locations);
    this.ships.push(ship);
    return ship;
  }

  /**
   * Process a guess at the given coordinate
   * @param coordinate The coordinate string (e.g., "34")
   * @returns Object containing hit result and if a ship was sunk
   */
  processGuess(coordinate: string): { hit: boolean; sunk: boolean; ship?: Ship } {
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
  isValidCoordinate(row: number, col: number): boolean {
    return row >= 0 && row < this.size && col >= 0 && col < this.size;
  }

  /**
   * Parse a coordinate string into row and column numbers
   */
  private parseCoordinate(coordinate: string): Coordinate {
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
  getCellState(row: number, col: number): CellState {
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
  getGrid(): CellState[][] {
    return this.grid.map(row => [...row]);
  }

  /**
   * Get all ships on the board
   */
  getShips(): Ship[] {
    return [...this.ships];
  }

  /**
   * Get the number of ships that are not sunk
   */
  getActiveShipsCount(): number {
    return this.ships.filter(ship => !ship.isSunk()).length;
  }

  /**
   * Get the board size
   */
  getSize(): number {
    return this.size;
  }

  /**
   * Check if a coordinate has already been guessed
   */
  isAlreadyGuessed(coordinate: string): boolean {
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
    } catch {
      return false;
    }
  }
} 