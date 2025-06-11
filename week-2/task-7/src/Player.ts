import { Board } from './Board';
import { GameConfig } from './types';

export class Player {
  protected readonly name: string;
  protected readonly board: Board;
  protected readonly guesses: Set<string>;

  constructor(name: string, config: GameConfig) {
    this.name = name;
    this.board = new Board(config.boardSize);
    this.guesses = new Set<string>();
    
    // Place ships randomly
    this.setupShips(config);
  }

  private setupShips(config: GameConfig): void {
    for (let i = 0; i < config.numShips; i++) {
      const ship = this.board.placeShipRandomly(config.shipLength);
      if (!ship) {
        throw new Error(`Failed to place ship ${i + 1} for ${this.name}`);
      }
    }
  }

  /**
   * Make a guess at the opponent's board
   * @param coordinate The coordinate to guess
   * @param opponentBoard The opponent's board
   */
  makeGuess(coordinate: string, opponentBoard: Board): { hit: boolean; sunk: boolean; alreadyGuessed: boolean } {
    // Check if already guessed
    if (this.guesses.has(coordinate)) {
      return { hit: false, sunk: false, alreadyGuessed: true };
    }

    // Validate coordinate format
    if (!this.isValidCoordinateFormat(coordinate)) {
      throw new Error('Invalid coordinate format');
    }

    // Check if coordinate is on the board
    const { row, col } = this.parseCoordinate(coordinate);
    if (!opponentBoard.isValidCoordinate(row, col)) {
      throw new Error('Coordinate out of bounds');
    }

    // Check if already guessed on opponent board
    if (opponentBoard.isAlreadyGuessed(coordinate)) {
      return { hit: false, sunk: false, alreadyGuessed: true };
    }

    // Record the guess
    this.guesses.add(coordinate);

    // Process the guess
    try {
      const result = opponentBoard.processGuess(coordinate);
      return { hit: result.hit, sunk: result.sunk, alreadyGuessed: false };
    } catch (error) {
      // Should not happen if validation is correct
      throw new Error(`Failed to process guess: ${error instanceof Error ? error.message : 'Unknown error'}`);
    }
  }

  /**
   * Receive a guess from the opponent
   * @param coordinate The coordinate being guessed
   */
  receiveGuess(coordinate: string): { hit: boolean; sunk: boolean } {
    try {
      const result = this.board.processGuess(coordinate);
      return { hit: result.hit, sunk: result.sunk };
    } catch (error) {
      throw new Error(`Invalid guess received: ${error instanceof Error ? error.message : 'Unknown error'}`);
    }
  }

  /**
   * Check if this player has lost (all ships sunk)
   */
  hasLost(): boolean {
    return this.board.getActiveShipsCount() === 0;
  }

  /**
   * Get the player's name
   */
  getName(): string {
    return this.name;
  }

  /**
   * Get the player's board
   */
  getBoard(): Board {
    return this.board;
  }

  /**
   * Get all guesses made by this player
   */
  getGuesses(): string[] {
    return Array.from(this.guesses);
  }

  /**
   * Get the number of active ships remaining
   */
  getActiveShipsCount(): number {
    return this.board.getActiveShipsCount();
  }

  /**
   * Validate coordinate format (should be exactly 2 digits)
   */
  protected isValidCoordinateFormat(coordinate: string): boolean {
    return /^\d{2}$/.test(coordinate);
  }

  /**
   * Parse coordinate string into row and column
   */
  protected parseCoordinate(coordinate: string): { row: number; col: number } {
    const row = parseInt(coordinate.charAt(0), 10);
    const col = parseInt(coordinate.charAt(1), 10);
    return { row, col };
  }
} 