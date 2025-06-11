export declare class Ship {
    private readonly locations;
    private readonly hits;
    private readonly length;
    constructor(locations: string[]);
    /**
     * Attempt to hit the ship at the given coordinate
     * @param coordinate The coordinate to hit in string format (e.g., "34")
     * @returns true if the coordinate hits this ship, false otherwise
     */
    hit(coordinate: string): boolean;
    /**
     * Check if the ship is completely sunk
     */
    isSunk(): boolean;
    /**
     * Check if a coordinate is part of this ship
     */
    hasLocation(coordinate: string): boolean;
    /**
     * Get all ship locations (defensive copy)
     */
    getLocations(): string[];
    /**
     * Get hit status for all locations (defensive copy)
     */
    getHits(): boolean[];
    /**
     * Get the length of the ship
     */
    getLength(): number;
    /**
     * Check if a specific location on the ship has been hit
     */
    isLocationHit(coordinate: string): boolean;
    /**
     * Get ship data in the format used by the original game
     */
    toLegacyFormat(): {
        locations: string[];
        hits: string[];
    };
}
//# sourceMappingURL=Ship.d.ts.map