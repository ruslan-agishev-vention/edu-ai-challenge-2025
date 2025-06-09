const { Enigma, Rotor, plugboardSwap } = require('./enigma.js');

// Simple test framework
let testCount = 0;
let passCount = 0;

function test(description, testFn) {
  testCount++;
  try {
    testFn();
    console.log(`✓ ${description}`);
    passCount++;
  } catch (error) {
    console.log(`✗ ${description}`);
    console.log(`  Error: ${error.message}`);
  }
}

function assert(condition, message) {
  if (!condition) {
    throw new Error(message || 'Assertion failed');
  }
}

function assertEquals(actual, expected, message) {
  if (actual !== expected) {
    throw new Error(message || `Expected "${expected}", got "${actual}"`);
  }
}

console.log('Running Enigma Machine Tests...\n');

// Test 1: Basic Encryption/Decryption Reciprocity
test('Basic encryption and decryption should be reciprocal', () => {
  const enigma1 = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], []);
  const plaintext = 'HELLO';
  const encrypted = enigma1.process(plaintext);
  
  const enigma2 = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], []);
  const decrypted = enigma2.process(encrypted);
  
  assertEquals(decrypted, plaintext, 'Decryption should return original text');
  assert(encrypted !== plaintext, 'Encrypted text should be different from plaintext');
});

// Test 2: Plugboard Functionality
test('Plugboard swapping should work correctly', () => {
  assertEquals(plugboardSwap('A', [['A', 'B']]), 'B', 'A should map to B');
  assertEquals(plugboardSwap('B', [['A', 'B']]), 'A', 'B should map to A');
  assertEquals(plugboardSwap('C', [['A', 'B']]), 'C', 'C should remain unchanged');
  assertEquals(plugboardSwap('A', [['A', 'B'], ['C', 'D']]), 'B', 'Multiple pairs should work');
});

// Test 3: Plugboard Integration
test('Enigma with plugboard should maintain reciprocity', () => {
  const plugPairs = [['A', 'B'], ['C', 'D']];
  const enigma1 = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], plugPairs);
  const plaintext = 'HELLO';
  const encrypted = enigma1.process(plaintext);
  
  const enigma2 = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], plugPairs);
  const decrypted = enigma2.process(encrypted);
  
  assertEquals(decrypted, plaintext, 'Plugboard encryption should be reciprocal');
});

// Test 4: Rotor Position Effects
test('Different rotor positions should produce different outputs', () => {
  const enigma1 = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], []);
  const enigma2 = new Enigma([0, 1, 2], [1, 0, 0], [0, 0, 0], []);
  
  const result1 = enigma1.process('A');
  const result2 = enigma2.process('A');
  
  assert(result1 !== result2, 'Different rotor positions should produce different outputs');
});

// Test 5: Ring Settings Effects
test('Different ring settings should produce different outputs', () => {
  const enigma1 = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], []);
  const enigma2 = new Enigma([0, 1, 2], [0, 0, 0], [1, 0, 0], []);
  
  const result1 = enigma1.process('A');
  const result2 = enigma2.process('A');
  
  assert(result1 !== result2, 'Different ring settings should produce different outputs');
});

// Test 6: Rotor Stepping Behavior
test('Right rotor should step on every character', () => {
  const enigma = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], []);
  const initialPos = enigma.rotors[2].position;
  
  enigma.process('A');
  assertEquals(enigma.rotors[2].position, (initialPos + 1) % 26, 'Right rotor should step once');
  
  enigma.process('A');
  assertEquals(enigma.rotors[2].position, (initialPos + 2) % 26, 'Right rotor should step again');
});

// Test 7: Middle Rotor Stepping at Notch
test('Middle rotor should step when right rotor reaches notch', () => {
  // Set right rotor (rotor III) to notch position (V = 21)
  const enigma = new Enigma([0, 1, 2], [0, 0, 21], [0, 0, 0], []);
  const initialMiddlePos = enigma.rotors[1].position;
  
  enigma.process('A'); // This should trigger middle rotor step
  assertEquals(enigma.rotors[1].position, (initialMiddlePos + 1) % 26, 'Middle rotor should step when right rotor at notch');
});

// Test 8: Double Stepping
test('Double stepping should occur when middle rotor is at notch', () => {
  // Set middle rotor (rotor II) to notch position (E = 4)
  const enigma = new Enigma([0, 1, 2], [0, 4, 0], [0, 0, 0], []);
  const initialLeftPos = enigma.rotors[0].position;
  const initialMiddlePos = enigma.rotors[1].position;
  
  enigma.process('A'); // This should trigger double stepping
  assertEquals(enigma.rotors[0].position, (initialLeftPos + 1) % 26, 'Left rotor should step in double stepping');
  assertEquals(enigma.rotors[1].position, (initialMiddlePos + 1) % 26, 'Middle rotor should step in double stepping');
});

// Test 9: Long Message Processing
test('Long messages should maintain reciprocity', () => {
  const longMessage = 'THEQUICKBROWNFOXJUMPSOVERTHELAZYDOG';
  const enigma1 = new Enigma([0, 1, 2], [5, 10, 15], [1, 2, 3], [['A', 'B'], ['C', 'D'], ['E', 'F']]);
  const encrypted = enigma1.process(longMessage);
  
  const enigma2 = new Enigma([0, 1, 2], [5, 10, 15], [1, 2, 3], [['A', 'B'], ['C', 'D'], ['E', 'F']]);
  const decrypted = enigma2.process(encrypted);
  
  assertEquals(decrypted, longMessage, 'Long message encryption should be reciprocal');
});

// Test 10: Non-alphabetic Characters
test('Non-alphabetic characters should pass through unchanged', () => {
  const enigma = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], []);
  const result = enigma.process('HELLO 123!');
  
  assert(result.includes(' '), 'Spaces should be preserved');
  assert(result.includes('1'), 'Numbers should be preserved');
  assert(result.includes('!'), 'Punctuation should be preserved');
});

// Test 11: Case Handling
test('Lowercase input should be converted to uppercase', () => {
  const enigma = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], []);
  const result1 = enigma.process('hello');
  
  const enigma2 = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], []);
  const result2 = enigma2.process('HELLO');
  
  assertEquals(result1, result2, 'Lowercase and uppercase input should produce same result');
});

// Test 12: Self-Encryption Prevention
test('No character should encrypt to itself (basic check)', () => {
  const enigma = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], []);
  
  // Test a few characters at initial position
  for (let char of 'ABCDEFGHIJ') {
    const encrypted = enigma.process(char);
    assert(encrypted !== char, `Character ${char} should not encrypt to itself`);
    
    // Reset enigma for next test
    const resetEnigma = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], []);
    resetEnigma.rotors[2].position = enigma.rotors[2].position - 1; // Account for stepping
    enigma.rotors = resetEnigma.rotors;
  }
});

console.log(`\n--- Test Results ---`);
console.log(`Tests passed: ${passCount}/${testCount}`);
console.log(`Success rate: ${((passCount/testCount) * 100).toFixed(1)}%`);

if (passCount === testCount) {
  console.log('🎉 All tests passed! Enigma machine is working correctly.');
} else {
  console.log('❌ Some tests failed. Please check the implementation.');
  process.exit(1);
} 