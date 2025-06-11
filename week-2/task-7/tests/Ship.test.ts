import { Ship } from '../src/Ship';

describe('Ship', () => {
  let ship: Ship;
  const testLocations = ['23', '24', '25'];

  beforeEach(() => {
    ship = new Ship(testLocations);
  });

  describe('constructor', () => {
    it('should create a ship with given locations', () => {
      expect(ship.getLocations()).toEqual(testLocations);
      expect(ship.getLength()).toBe(3);
    });

    it('should initialize all hits as false', () => {
      expect(ship.getHits()).toEqual([false, false, false]);
    });

    it('should create defensive copy of locations', () => {
      const originalLocations = ['00', '01'];
      const testShip = new Ship(originalLocations);
      originalLocations.push('02');
      expect(testShip.getLocations()).toEqual(['00', '01']);
    });
  });

  describe('hit', () => {
    it('should return true and mark hit when coordinate hits ship', () => {
      const hitResult = ship.hit('24');
      expect(hitResult).toBe(true);
      expect(ship.getHits()).toEqual([false, true, false]);
    });

    it('should return false when coordinate misses ship', () => {
      const hitResult = ship.hit('99');
      expect(hitResult).toBe(false);
      expect(ship.getHits()).toEqual([false, false, false]);
    });

    it('should allow multiple hits on same location', () => {
      ship.hit('23');
      const secondHit = ship.hit('23');
      expect(secondHit).toBe(true);
      expect(ship.getHits()).toEqual([true, false, false]);
    });
  });

  describe('isSunk', () => {
    it('should return false when ship is not completely hit', () => {
      expect(ship.isSunk()).toBe(false);
      ship.hit('23');
      expect(ship.isSunk()).toBe(false);
      ship.hit('24');
      expect(ship.isSunk()).toBe(false);
    });

    it('should return true when all locations are hit', () => {
      ship.hit('23');
      ship.hit('24');
      ship.hit('25');
      expect(ship.isSunk()).toBe(true);
    });
  });

  describe('hasLocation', () => {
    it('should return true for coordinates that are part of the ship', () => {
      expect(ship.hasLocation('23')).toBe(true);
      expect(ship.hasLocation('24')).toBe(true);
      expect(ship.hasLocation('25')).toBe(true);
    });

    it('should return false for coordinates not part of the ship', () => {
      expect(ship.hasLocation('22')).toBe(false);
      expect(ship.hasLocation('26')).toBe(false);
      expect(ship.hasLocation('99')).toBe(false);
    });
  });

  describe('isLocationHit', () => {
    it('should return false for unhit locations on ship', () => {
      expect(ship.isLocationHit('23')).toBe(false);
      expect(ship.isLocationHit('24')).toBe(false);
    });

    it('should return true for hit locations on ship', () => {
      ship.hit('23');
      expect(ship.isLocationHit('23')).toBe(true);
      expect(ship.isLocationHit('24')).toBe(false);
    });

    it('should return false for coordinates not on ship', () => {
      expect(ship.isLocationHit('99')).toBe(false);
    });
  });

  describe('getLocations', () => {
    it('should return defensive copy of locations', () => {
      const locations = ship.getLocations();
      locations.push('99');
      expect(ship.getLocations()).toEqual(testLocations);
    });
  });

  describe('getHits', () => {
    it('should return defensive copy of hits array', () => {
      const hits = ship.getHits();
      hits[0] = true;
      expect(ship.getHits()).toEqual([false, false, false]);
    });
  });

  describe('toLegacyFormat', () => {
    it('should return legacy format object', () => {
      ship.hit('23');
      const legacy = ship.toLegacyFormat();
      expect(legacy).toEqual({
        locations: ['23', '24', '25'],
        hits: ['hit', '', '']
      });
    });

    it('should return defensive copies in legacy format', () => {
      const legacy = ship.toLegacyFormat();
      legacy.locations.push('99');
      legacy.hits.push('hit');
      expect(ship.getLocations()).toEqual(testLocations);
      expect(ship.getHits()).toEqual([false, false, false]);
    });
  });
}); 