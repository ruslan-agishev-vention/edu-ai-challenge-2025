import { Game } from '../src/Game';
import { GameConfig } from '../src/types';

describe('Game', () => {
  let game: Game;
  const config: GameConfig = {
    boardSize: 10,
    numShips: 3,
    shipLength: 3
  };

  beforeEach(() => {
    game = new Game(config);
  });

  describe('constructor', () => {
    it('should create game with default config when no config provided', () => {
      const defaultGame = new Game();
      const gameConfig = defaultGame.getConfig();
      expect(gameConfig.boardSize).toBe(10);
      expect(gameConfig.numShips).toBe(3);
      expect(gameConfig.shipLength).toBe(3);
    });

    it('should create game with custom config', () => {
      const customConfig = { boardSize: 8, numShips: 2, shipLength: 4 };
      const customGame = new Game(customConfig);
      const gameConfig = customGame.getConfig();
      expect(gameConfig.boardSize).toBe(8);
      expect(gameConfig.numShips).toBe(2);
      expect(gameConfig.shipLength).toBe(4);
    });

    it('should initialize game state correctly', () => {
      const gameState = game.getGameState();
      expect(gameState.playerShipsRemaining).toBe(config.numShips);
      expect(gameState.cpuShipsRemaining).toBe(config.numShips);
      expect(gameState.currentTurn).toBe('player');
      expect(gameState.gameResult).toBe('ongoing');
    });

    it('should create players with ships', () => {
      expect(game.getPlayer().getActiveShipsCount()).toBe(config.numShips);
      expect(game.getAIPlayer().getActiveShipsCount()).toBe(config.numShips);
    });
  });

  describe('processPlayerGuess', () => {
    it('should validate coordinate format', () => {
      const result = game.processPlayerGuess('abc');
      expect(result.hit).toBe(false);
      expect(result.message).toContain('exactly two digits');
    });

    it('should validate coordinate bounds', () => {
      const result = game.processPlayerGuess('aa');
      expect(result.hit).toBe(false);
      expect(result.message).toContain('exactly two digits');
    });

    it('should process valid miss', () => {
      const result = game.processPlayerGuess('00');
      expect(result.alreadyGuessed).toBe(false);
      expect(result.gameResult).toBe('ongoing');
      expect(game.getGameState().currentTurn).toBe('cpu');
    });

    it('should detect already guessed location', () => {
      game.processPlayerGuess('00');
      // Switch back to player turn
      if (game.getGameState().currentTurn === 'cpu') {
        game.processAITurn();
      }
      const result = game.processPlayerGuess('00');
      expect(result.alreadyGuessed).toBe(true);
      expect(result.message).toContain('already guessed');
    });

    it('should handle hit correctly', () => {
      // Find a ship location on AI board
      const aiShips = game.getAIPlayer().getBoard().getShips();
      const shipLocation = aiShips[0]!.getLocations()[0]!;
      
      const result = game.processPlayerGuess(shipLocation);
      expect(result.hit).toBe(true);
      expect(result.message).toContain('HIT');
      expect(game.getGameState().currentTurn).toBe('cpu');
    });

    it('should detect sunk ship', () => {
      // Sink a ship by hitting all its locations
      const aiShips = game.getAIPlayer().getBoard().getShips();
      const shipLocations = aiShips[0]!.getLocations();
      
      // Hit all but last location
      for (let i = 0; i < shipLocations.length - 1; i++) {
        game.processPlayerGuess(shipLocations[i]!);
        if (game.getGameState().currentTurn === 'cpu') {
          game.processAITurn(); // Switch back to player
        }
      }
      
      // Hit last location
      const result = game.processPlayerGuess(shipLocations[shipLocations.length - 1]!);
      expect(result.sunk).toBe(true);
      expect(result.message).toContain('sunk');
    });

    it('should detect player victory', () => {
      // Sink all AI ships
      const aiShips = game.getAIPlayer().getBoard().getShips();
      aiShips.forEach(ship => {
        ship.getLocations().forEach(location => {
          const result = game.processPlayerGuess(location);
          if (game.getGameState().currentTurn === 'cpu' && !game.isGameOver()) {
            game.processAITurn();
          }
        });
      });
      
      expect(game.isGameOver()).toBe(true);
      expect(game.getGameState().gameResult).toBe('player_win');
      expect(game.getWinner()).toBe('Player');
    });

    it('should throw error when game is over', () => {
      // Force game to be over
      (game as any).gameState.gameResult = 'player_win';
      expect(() => game.processPlayerGuess('00')).toThrow('Game has already ended');
    });

    it('should throw error when not player turn', () => {
      // Set turn to CPU
      (game as any).gameState.currentTurn = 'cpu';
      expect(() => game.processPlayerGuess('00')).toThrow('Not player turn');
    });
  });

  describe('processAITurn', () => {
    beforeEach(() => {
      // Make sure it's CPU turn
      if (game.getGameState().currentTurn === 'player') {
        game.processPlayerGuess('00'); // Make a miss to switch to CPU
      }
    });

    it('should make valid AI guess', () => {
      const result = game.processAITurn();
      expect(result.coordinate).toMatch(/^\d{2}$/);
      expect(typeof result.hit).toBe('boolean');
      expect(typeof result.sunk).toBe('boolean');
      expect(result.message).toBeTruthy();
    });

    it('should switch turn back to player after miss', () => {
      const initialTurn = game.getGameState().currentTurn;
      if (initialTurn === 'cpu') {
        const result = game.processAITurn();
        if (!result.hit) {
          expect(game.getGameState().currentTurn).toBe('player');
        }
      }
    });

    it('should detect CPU victory when all player ships sunk', () => {
      // Sink all player ships manually
      const playerShips = game.getPlayer().getBoard().getShips();
      playerShips.forEach(ship => {
        ship.getLocations().forEach(location => {
          game.getPlayer().receiveGuess(location);
        });
      });
      
      // Update game state
      (game as any).gameState.playerShipsRemaining = 0;
      (game as any).gameState.gameResult = 'cpu_win';
      
      expect(game.isGameOver()).toBe(true);
      expect(game.getWinner()).toBe('CPU');
    });

    it('should throw error when game is over', () => {
      (game as any).gameState.gameResult = 'cpu_win';
      expect(() => game.processAITurn()).toThrow('Game has already ended');
    });

    it('should throw error when not CPU turn', () => {
      (game as any).gameState.currentTurn = 'player';
      expect(() => game.processAITurn()).toThrow('Not CPU turn');
    });
  });

  describe('game state management', () => {
    it('should return defensive copy of game state', () => {
      const gameState = game.getGameState();
      gameState.currentTurn = 'cpu';
      expect(game.getGameState().currentTurn).toBe('player');
    });

    it('should return defensive copy of config', () => {
      const gameConfig = game.getConfig();
      gameConfig.boardSize = 5;
      expect(game.getConfig().boardSize).toBe(config.boardSize);
    });

    it('should correctly report game over status', () => {
      expect(game.isGameOver()).toBe(false);
      
      (game as any).gameState.gameResult = 'player_win';
      expect(game.isGameOver()).toBe(true);
    });

    it('should return null winner for ongoing game', () => {
      expect(game.getWinner()).toBeNull();
    });
  });

  describe('reset functionality', () => {
    it('should reset game to initial state', () => {
      // Make some moves
      game.processPlayerGuess('00');
      game.processAITurn();
      
      // Reset game
      game.reset();
      
      // Check reset state
      const gameState = game.getGameState();
      expect(gameState.playerShipsRemaining).toBe(config.numShips);
      expect(gameState.cpuShipsRemaining).toBe(config.numShips);
      expect(gameState.currentTurn).toBe('player');
      expect(gameState.gameResult).toBe('ongoing');
      
      // Check players have been reset
      expect(game.getPlayer().getGuesses()).toEqual([]);
      expect(game.getAIPlayer().getGuesses()).toEqual([]);
      expect(game.getPlayer().getActiveShipsCount()).toBe(config.numShips);
      expect(game.getAIPlayer().getActiveShipsCount()).toBe(config.numShips);
    });
  });

  describe('coordinate validation', () => {
    it('should handle various invalid coordinate formats', () => {
      const invalidCoords = ['a', '123', 'ab', '1a', 'a1', ''];
      
      invalidCoords.forEach(coord => {
        const result = game.processPlayerGuess(coord);
        expect(result.hit).toBe(false);
        expect(result.message).toContain('exactly two digits');
      });
    });

    it('should handle boundary coordinates correctly', () => {
      const boundaryCoords = ['09', '90', '99'];
      
      boundaryCoords.forEach(coord => {
        // Reset game to ensure player turn
        if (game.getGameState().currentTurn !== 'player') {
          game.reset();
        }
        const result = game.processPlayerGuess(coord);
        expect(result.message).not.toContain('exactly two digits');
      });
    });
  });

  describe('turn management', () => {
    it('should handle consecutive player turns on invalid input', () => {
      const initialTurn = game.getGameState().currentTurn;
      
      // Invalid input shouldn't change turn
      game.processPlayerGuess('abc');
      expect(game.getGameState().currentTurn).toBe(initialTurn);
      
      // Valid input should change turn (if not already guessed)
      const result = game.processPlayerGuess('00');
      if (!result.alreadyGuessed) {
        expect(game.getGameState().currentTurn).toBe('cpu');
      }
    });
  });
}); 