import { Player } from '../src/Player';
import { Board } from '../src/Board';
import { Ship } from '../src/Ship';
import { GameConfig } from '../src/types';

describe('Player', () => {
  let player: Player;
  let opponentBoard: Board;
  const config: GameConfig = {
    boardSize: 10,
    numShips: 3,
    shipLength: 3
  };

  beforeEach(() => {
    player = new Player('TestPlayer', config);
    opponentBoard = new Board(config.boardSize);
    
    // Place a ship on opponent board for testing
    const ship = new Ship(['23', '24', '25']);
    (opponentBoard as any).ships.push(ship);
    const grid = (opponentBoard as any).grid;
    grid[2][3] = 'S';
    grid[2][4] = 'S';
    grid[2][5] = 'S';
  });

  describe('constructor', () => {
    it('should create player with correct name', () => {
      expect(player.getName()).toBe('TestPlayer');
    });

    it('should create board with correct size', () => {
      expect(player.getBoard().getSize()).toBe(config.boardSize);
    });

    it('should place correct number of ships', () => {
      expect(player.getActiveShipsCount()).toBe(config.numShips);
    });

    it('should initialize with empty guesses', () => {
      expect(player.getGuesses()).toEqual([]);
    });
  });

  describe('makeGuess', () => {
    it('should make successful hit', () => {
      const result = player.makeGuess('23', opponentBoard);
      expect(result.hit).toBe(true);
      expect(result.sunk).toBe(false);
      expect(result.alreadyGuessed).toBe(false);
      expect(player.getGuesses()).toContain('23');
    });

    it('should make successful miss', () => {
      const result = player.makeGuess('00', opponentBoard);
      expect(result.hit).toBe(false);
      expect(result.sunk).toBe(false);
      expect(result.alreadyGuessed).toBe(false);
      expect(player.getGuesses()).toContain('00');
    });

    it('should detect sunk ship', () => {
      player.makeGuess('23', opponentBoard);
      player.makeGuess('24', opponentBoard);
      const result = player.makeGuess('25', opponentBoard);
      expect(result.hit).toBe(true);
      expect(result.sunk).toBe(true);
      expect(result.alreadyGuessed).toBe(false);
    });

    it('should detect already guessed by player', () => {
      player.makeGuess('23', opponentBoard);
      const result = player.makeGuess('23', opponentBoard);
      expect(result.alreadyGuessed).toBe(true);
      expect(result.hit).toBe(false);
      expect(result.sunk).toBe(false);
    });

    it('should detect already guessed on opponent board', () => {
      opponentBoard.processGuess('12'); // Someone else guessed first
      const result = player.makeGuess('12', opponentBoard);
      expect(result.alreadyGuessed).toBe(true);
    });

    it('should throw error for invalid coordinate format', () => {
      expect(() => player.makeGuess('abc', opponentBoard)).toThrow('Invalid coordinate format');
      expect(() => player.makeGuess('1', opponentBoard)).toThrow('Invalid coordinate format');
      expect(() => player.makeGuess('123', opponentBoard)).toThrow('Invalid coordinate format');
    });

    it('should throw error for out of bounds coordinate', () => {
      expect(() => player.makeGuess('aa', opponentBoard)).toThrow('Invalid coordinate format');
    });
  });

  describe('receiveGuess', () => {
    it('should process incoming guess correctly', () => {
      // Find a ship location on player's board
      const ships = player.getBoard().getShips();
      const shipLocation = ships[0]!.getLocations()[0]!;
      
      const result = player.receiveGuess(shipLocation);
      expect(result.hit).toBe(true);
    });

    it('should handle miss correctly', () => {
      const result = player.receiveGuess('00');
      expect(result.hit).toBe(false);
      expect(result.sunk).toBe(false);
    });

    it('should throw error for invalid guess', () => {
      expect(() => player.receiveGuess('abc')).toThrow('Invalid guess received');
    });
  });

  describe('hasLost', () => {
    it('should return false when ships remain', () => {
      expect(player.hasLost()).toBe(false);
    });

    it('should return true when all ships are sunk', () => {
      // Sink all ships
      const ships = player.getBoard().getShips();
      ships.forEach(ship => {
        ship.getLocations().forEach(location => {
          player.receiveGuess(location);
        });
      });
      expect(player.hasLost()).toBe(true);
    });
  });

  describe('getActiveShipsCount', () => {
    it('should return correct number of active ships', () => {
      expect(player.getActiveShipsCount()).toBe(config.numShips);
    });

    it('should decrease when ship is sunk', () => {
      const initialCount = player.getActiveShipsCount();
      
      // Sink one ship
      const ships = player.getBoard().getShips();
      const firstShip = ships[0]!;
      firstShip.getLocations().forEach(location => {
        player.receiveGuess(location);
      });
      
      expect(player.getActiveShipsCount()).toBe(initialCount - 1);
    });
  });

  describe('coordinate validation', () => {
    it('should validate coordinate format correctly', () => {
      // Testing protected method through public interface
      expect(() => player.makeGuess('00', opponentBoard)).not.toThrow();
      expect(() => player.makeGuess('ab', opponentBoard)).toThrow();
      expect(() => player.makeGuess('1', opponentBoard)).toThrow();
    });
  });

  describe('defensive copying', () => {
    it('should return defensive copy of guesses', () => {
      player.makeGuess('00', opponentBoard);
      const guesses = player.getGuesses();
      guesses.push('99');
      expect(player.getGuesses()).toEqual(['00']);
    });
  });
}); 