"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Ship = void 0;
class Ship {
    constructor(locations) {
        this.locations = [...locations]; // Create defensive copy
        this.hits = new Array(locations.length).fill(false);
        this.length = locations.length;
    }
    /**
     * Attempt to hit the ship at the given coordinate
     * @param coordinate The coordinate to hit in string format (e.g., "34")
     * @returns true if the coordinate hits this ship, false otherwise
     */
    hit(coordinate) {
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
    isSunk() {
        return this.hits.every(hit => hit);
    }
    /**
     * Check if a coordinate is part of this ship
     */
    hasLocation(coordinate) {
        return this.locations.includes(coordinate);
    }
    /**
     * Get all ship locations (defensive copy)
     */
    getLocations() {
        return [...this.locations];
    }
    /**
     * Get hit status for all locations (defensive copy)
     */
    getHits() {
        return [...this.hits];
    }
    /**
     * Get the length of the ship
     */
    getLength() {
        return this.length;
    }
    /**
     * Check if a specific location on the ship has been hit
     */
    isLocationHit(coordinate) {
        const index = this.locations.indexOf(coordinate);
        return index !== -1 && this.hits[index] === true;
    }
    /**
     * Get ship data in the format used by the original game
     */
    toLegacyFormat() {
        return {
            locations: [...this.locations],
            hits: this.hits.map(hit => hit ? 'hit' : '')
        };
    }
}
exports.Ship = Ship;
//# sourceMappingURL=Ship.js.map