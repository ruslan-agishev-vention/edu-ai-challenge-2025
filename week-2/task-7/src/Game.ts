import { Player } from './Player';
import { AIPlayer } from './AIPlayer';
import { GameConfig, GameResult, GameState } from './types';

export class Game {
  private readonly config: GameConfig;
  private readonly player: Player;
  private readonly aiPlayer: AIPlayer;
  private gameState: GameState;

  constructor(config?: Partial<GameConfig>) {
    this.config = {
      boardSize: 10,
      numShips: 3,
      shipLength: 3,
      ...config
    };

    this.player = new Player('Player', this.config);
    this.aiPlayer = new AIPlayer(this.config);
    
    this.gameState = {
      playerShipsRemaining: this.config.numShips,
      cpuShipsRemaining: this.config.numShips,
      currentTurn: 'player',
      gameResult: 'ongoing'
    };
  }

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
  } {
    if (this.gameState.gameResult !== 'ongoing') {
      throw new Error('Game has already ended');
    }

    if (this.gameState.currentTurn !== 'player') {
      throw new Error('Not player turn');
    }

    try {
      // Validate coordinate format
      if (!this.isValidCoordinateFormat(coordinate)) {
        return {
          hit: false,
          sunk: false,
          alreadyGuessed: false,
          gameResult: 'ongoing',
          message: 'Oops, input must be exactly two digits (e.g., 00, 34, 98).'
        };
      }

      // Parse and validate coordinate bounds
      const { row, col } = this.parseCoordinate(coordinate);
      if (!this.isValidCoordinateBounds(row, col)) {
        return {
          hit: false,
          sunk: false,
          alreadyGuessed: false,
          gameResult: 'ongoing',
          message: `Oops, please enter valid row and column numbers between 0 and ${this.config.boardSize - 1}.`
        };
      }

      const result = this.player.makeGuess(coordinate, this.aiPlayer.getBoard());

      if (result.alreadyGuessed) {
        return {
          hit: false,
          sunk: false,
          alreadyGuessed: true,
          gameResult: 'ongoing',
          message: 'You already guessed that location!'
        };
      }

      // Update game state
      if (result.sunk) {
        this.gameState.cpuShipsRemaining--;
      }

      let message: string;
      if (result.hit) {
        message = result.sunk ? 'PLAYER HIT! You sunk an enemy battleship!' : 'PLAYER HIT!';
      } else {
        message = 'PLAYER MISS.';
      }

      // Check for game end
      if (this.gameState.cpuShipsRemaining === 0) {
        this.gameState.gameResult = 'player_win';
        message += ' *** CONGRATULATIONS! You sunk all enemy battleships! ***';
      } else {
        this.gameState.currentTurn = 'cpu';
      }

      return {
        hit: result.hit,
        sunk: result.sunk,
        alreadyGuessed: false,
        gameResult: this.gameState.gameResult,
        message
      };

    } catch (error) {
      return {
        hit: false,
        sunk: false,
        alreadyGuessed: false,
        gameResult: 'ongoing',
        message: error instanceof Error ? error.message : 'Unknown error occurred'
      };
    }
  }

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
  } {
    if (this.gameState.gameResult !== 'ongoing') {
      throw new Error('Game has already ended');
    }

    if (this.gameState.currentTurn !== 'cpu') {
      throw new Error('Not CPU turn');
    }

    const result = this.aiPlayer.makeAIGuess(this.player.getBoard());

    // Update game state
    if (result.sunk) {
      this.gameState.playerShipsRemaining--;
    }

    let message: string;
    if (result.hit) {
      message = result.sunk ? 
        `CPU HIT at ${result.coordinate}! CPU sunk your battleship!` : 
        `CPU HIT at ${result.coordinate}!`;
    } else {
      message = `CPU MISS at ${result.coordinate}.`;
    }

    // Check for game end
    if (this.gameState.playerShipsRemaining === 0) {
      this.gameState.gameResult = 'cpu_win';
      message += ' *** GAME OVER! The CPU sunk all your battleships! ***';
    } else {
      this.gameState.currentTurn = 'player';
    }

    return {
      coordinate: result.coordinate,
      hit: result.hit,
      sunk: result.sunk,
      gameResult: this.gameState.gameResult,
      message
    };
  }

  /**
   * Get the current game state
   */
  getGameState(): GameState {
    return { ...this.gameState };
  }

  /**
   * Get the game configuration
   */
  getConfig(): GameConfig {
    return { ...this.config };
  }

  /**
   * Get the human player
   */
  getPlayer(): Player {
    return this.player;
  }

  /**
   * Get the AI player
   */
  getAIPlayer(): AIPlayer {
    return this.aiPlayer;
  }

  /**
   * Check if the game is over
   */
  isGameOver(): boolean {
    return this.gameState.gameResult !== 'ongoing';
  }

  /**
   * Get the winner of the game
   */
  getWinner(): string | null {
    switch (this.gameState.gameResult) {
      case 'player_win':
        return this.player.getName();
      case 'cpu_win':
        return this.aiPlayer.getName();
      default:
        return null;
    }
  }

  /**
   * Reset the game to initial state
   */
  reset(): void {
    // Create new players with fresh boards
    const newPlayer = new Player('Player', this.config);
    const newAIPlayer = new AIPlayer(this.config);
    
    // Replace the current players
    (this as any).player = newPlayer;
    (this as any).aiPlayer = newAIPlayer;

    // Reset game state
    this.gameState = {
      playerShipsRemaining: this.config.numShips,
      cpuShipsRemaining: this.config.numShips,
      currentTurn: 'player',
      gameResult: 'ongoing'
    };
  }

  /**
   * Validate coordinate format
   */
  private isValidCoordinateFormat(coordinate: string): boolean {
    return /^\d{2}$/.test(coordinate);
  }

  /**
   * Parse coordinate string
   */
  private parseCoordinate(coordinate: string): { row: number; col: number } {
    const row = parseInt(coordinate.charAt(0), 10);
    const col = parseInt(coordinate.charAt(1), 10);
    return { row, col };
  }

  /**
   * Validate coordinate bounds
   */
  private isValidCoordinateBounds(row: number, col: number): boolean {
    return row >= 0 && row < this.config.boardSize && 
           col >= 0 && col < this.config.boardSize;
  }
} 