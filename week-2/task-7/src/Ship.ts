import { Coordinate } from './types';

export class Ship {
  private readonly locations: string[];
  private readonly hits: boolean[];
  private readonly length: number;

  constructor(locations: string[]) {
    this.locations = [...locations]; // Create defensive copy
    this.hits = new Array(locations.length).fill(false);
    this.length = locations.length;
  }

  /**
   * Attempt to hit the ship at the given coordinate
   * @param coordinate The coordinate to hit in string format (e.g., "34")
   * @returns true if the coordinate hits this ship, false otherwise
   */
  hit(coordinate: string): boolean {
    const index = this.locations.indexOf(coordinate);
    if (index === -1) {
      return false; // Miss
    }

    this.hits[index] = true;
    return true; // Hit
  }

  /**
   * Check if the ship is completely sunk
   */
  isSunk(): boolean {
    return this.hits.every(hit => hit);
  }

  /**
   * Check if a coordinate is part of this ship
   */
  hasLocation(coordinate: string): boolean {
    return this.locations.includes(coordinate);
  }

  /**
   * Get all ship locations (defensive copy)
   */
  getLocations(): string[] {
    return [...this.locations];
  }

  /**
   * Get hit status for all locations (defensive copy)
   */
  getHits(): boolean[] {
    return [...this.hits];
  }

  /**
   * Get the length of the ship
   */
  getLength(): number {
    return this.length;
  }

  /**
   * Check if a specific location on the ship has been hit
   */
  isLocationHit(coordinate: string): boolean {
    const index = this.locations.indexOf(coordinate);
    return index !== -1 && this.hits[index] === true;
  }

  /**
   * Get ship data in the format used by the original game
   */
  toLegacyFormat(): { locations: string[]; hits: string[] } {
    return {
      locations: [...this.locations],
      hits: this.hits.map(hit => hit ? 'hit' : '')
    };
  }
} 