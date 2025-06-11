import { Board } from '../src/Board';
import { Ship } from '../src/Ship';

describe('Board', () => {
  let board: Board;
  const boardSize = 10;

  beforeEach(() => {
    board = new Board(boardSize);
  });

  describe('constructor', () => {
    it('should create a board with correct size', () => {
      expect(board.getSize()).toBe(boardSize);
    });

    it('should initialize all cells as water (~)', () => {
      const grid = board.getGrid();
      for (let i = 0; i < boardSize; i++) {
        for (let j = 0; j < boardSize; j++) {
          expect(grid[i]![j]).toBe('~');
        }
      }
    });

    it('should start with no ships', () => {
      expect(board.getShips()).toEqual([]);
      expect(board.getActiveShipsCount()).toBe(0);
    });
  });

  describe('placeShipRandomly', () => {
    it('should place a ship and return Ship object', () => {
      const ship = board.placeShipRandomly(3);
      expect(ship).toBeInstanceOf(Ship);
      expect(board.getShips().length).toBe(1);
      expect(board.getActiveShipsCount()).toBe(1);
    });

    it('should place ship with correct length', () => {
      const shipLength = 4;
      const ship = board.placeShipRandomly(shipLength);
      expect(ship!.getLength()).toBe(shipLength);
    });

    it('should place ships in valid positions', () => {
      const ship = board.placeShipRandomly(3);
      const locations = ship!.getLocations();
      
      // All locations should be valid coordinates
      locations.forEach(location => {
        const row = parseInt(location.charAt(0), 10);
        const col = parseInt(location.charAt(1), 10);
        expect(board.isValidCoordinate(row, col)).toBe(true);
      });
    });

    it('should not place overlapping ships', () => {
      // Place multiple ships and verify no overlap
      const ship1 = board.placeShipRandomly(3);
      const ship2 = board.placeShipRandomly(3);
      const ship3 = board.placeShipRandomly(3);

      expect(ship1).not.toBeNull();
      expect(ship2).not.toBeNull();
      expect(ship3).not.toBeNull();

      const allLocations = [
        ...ship1!.getLocations(),
        ...ship2!.getLocations(),
        ...ship3!.getLocations()
      ];

      // Check that all locations are unique
      const uniqueLocations = new Set(allLocations);
      expect(uniqueLocations.size).toBe(allLocations.length);
    });
  });

  describe('isValidCoordinate', () => {
    it('should return true for valid coordinates', () => {
      expect(board.isValidCoordinate(0, 0)).toBe(true);
      expect(board.isValidCoordinate(5, 5)).toBe(true);
      expect(board.isValidCoordinate(9, 9)).toBe(true);
    });

    it('should return false for invalid coordinates', () => {
      expect(board.isValidCoordinate(-1, 0)).toBe(false);
      expect(board.isValidCoordinate(0, -1)).toBe(false);
      expect(board.isValidCoordinate(10, 0)).toBe(false);
      expect(board.isValidCoordinate(0, 10)).toBe(false);
      expect(board.isValidCoordinate(10, 10)).toBe(false);
    });
  });

  describe('processGuess', () => {
    beforeEach(() => {
      // Place a ship at known location for testing
      const ship = new Ship(['23', '24', '25']);
      (board as any).ships.push(ship);
      const grid = (board as any).grid;
      grid[2][3] = 'S';
      grid[2][4] = 'S';
      grid[2][5] = 'S';
    });

    it('should process hit correctly', () => {
      const result = board.processGuess('23');
      expect(result.hit).toBe(true);
      expect(result.sunk).toBe(false);
      expect(board.getCellState(2, 3)).toBe('X');
    });

    it('should process miss correctly', () => {
      const result = board.processGuess('00');
      expect(result.hit).toBe(false);
      expect(result.sunk).toBe(false);
      expect(board.getCellState(0, 0)).toBe('O');
    });

    it('should detect sunk ship', () => {
      board.processGuess('23');
      board.processGuess('24');
      const result = board.processGuess('25');
      expect(result.hit).toBe(true);
      expect(result.sunk).toBe(true);
      expect(board.getActiveShipsCount()).toBe(0);
    });

    it('should throw error for invalid coordinate', () => {
      expect(() => board.processGuess('abc')).toThrow('Coordinate must be exactly 2 digits');
      expect(() => board.processGuess('1')).toThrow('Coordinate must be exactly 2 digits');
      expect(() => board.processGuess('123')).toThrow('Coordinate must be exactly 2 digits');
    });

    it('should throw error for already guessed location', () => {
      board.processGuess('23');
      expect(() => board.processGuess('23')).toThrow('Already guessed this location');
    });

    it('should throw error for out of bounds coordinate', () => {
      expect(() => board.processGuess('aa')).toThrow('Invalid coordinate format');
    });
  });

  describe('getCellState', () => {
    it('should return correct cell state', () => {
      expect(board.getCellState(0, 0)).toBe('~');
    });

    it('should throw error for invalid coordinates', () => {
      expect(() => board.getCellState(-1, 0)).toThrow('Invalid coordinate');
      expect(() => board.getCellState(0, -1)).toThrow('Invalid coordinate');
      expect(() => board.getCellState(10, 0)).toThrow('Invalid coordinate');
      expect(() => board.getCellState(0, 10)).toThrow('Invalid coordinate');
    });
  });

  describe('getGrid', () => {
    it('should return defensive copy of grid', () => {
      const grid = board.getGrid();
      grid[0]![0] = 'X';
      expect(board.getCellState(0, 0)).toBe('~');
    });
  });

  describe('getShips', () => {
    it('should return defensive copy of ships array', () => {
      board.placeShipRandomly(3);
      const ships = board.getShips();
      ships.push(new Ship(['99']));
      expect(board.getShips().length).toBe(1);
    });
  });

  describe('isAlreadyGuessed', () => {
    it('should return false for unguessed locations', () => {
      expect(board.isAlreadyGuessed('23')).toBe(false);
    });

    it('should return true for guessed locations', () => {
      board.placeShipRandomly(3);
      board.processGuess('00'); // Miss
      expect(board.isAlreadyGuessed('00')).toBe(true);
    });

    it('should handle invalid coordinates gracefully', () => {
      expect(board.isAlreadyGuessed('abc')).toBe(false);
      expect(board.isAlreadyGuessed('99')).toBe(false);
    });
  });

  describe('getActiveShipsCount', () => {
    it('should return correct count of active ships', () => {
      expect(board.getActiveShipsCount()).toBe(0);
      
      board.placeShipRandomly(3);
      expect(board.getActiveShipsCount()).toBe(1);
      
      board.placeShipRandomly(2);
      expect(board.getActiveShipsCount()).toBe(2);
    });

    it('should decrease count when ship is sunk', () => {
      // Place ship at known location
      const ship = new Ship(['00', '01', '02']);
      (board as any).ships.push(ship);
      const grid = (board as any).grid;
      grid[0][0] = 'S';
      grid[0][1] = 'S';
      grid[0][2] = 'S';

      expect(board.getActiveShipsCount()).toBe(1);
      
      board.processGuess('00');
      board.processGuess('01');
      expect(board.getActiveShipsCount()).toBe(1);
      
      board.processGuess('02');
      expect(board.getActiveShipsCount()).toBe(0);
    });
  });
}); 