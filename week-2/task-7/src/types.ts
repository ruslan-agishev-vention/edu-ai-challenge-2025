export interface Coordinate {
  row: number;
  col: number;
}

export interface ShipData {
  locations: string[];
  hits: boolean[];
}

export type CellState = '~' | 'S' | 'X' | 'O';

export type GameResult = 'player_win' | 'cpu_win' | 'ongoing';

export type AIMode = 'hunt' | 'target';

export interface GameConfig {
  boardSize: number;
  numShips: number;
  shipLength: number;
}

export interface GameState {
  playerShipsRemaining: number;
  cpuShipsRemaining: number;
  currentTurn: 'player' | 'cpu';
  gameResult: GameResult;
} 