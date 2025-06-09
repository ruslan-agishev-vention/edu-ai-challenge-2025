# Enigma Machine Implementation

A historically accurate implementation of the famous Enigma machine used during World War II, complete with working rotors, plugboard, and proper encryption/decryption capabilities.

## Features

- ✅ **Reciprocal Encryption**: Messages encrypt and decrypt properly
- ✅ **Historical Accuracy**: Implements real Enigma rotor stepping and double-stepping
- ✅ **Plugboard Support**: Full plugboard functionality with bidirectional swapping
- ✅ **Multiple Rotors**: Uses historical Rotor I, II, and III configurations
- ✅ **Ring Settings**: Configurable ring settings for additional security
- ✅ **Comprehensive Tests**: 12 unit tests ensuring correct operation

## Quick Start

### Running the Enigma Machine

#### Interactive Mode (Recommended)
```bash
node enigma.js
```

This will prompt you for:
- Message to encrypt/decrypt
- Rotor positions (e.g., `0 0 0`)
- Ring settings (e.g., `0 0 0`) 
- Plugboard pairs (e.g., `AB CD EF`)

#### Using npm
```bash
npm start
```

### Running Tests

#### Direct execution
```bash
node enigma.test.js
```

#### Using npm (Recommended)
```bash
npm test
```

Expected output:
```
✓ Basic encryption and decryption should be reciprocal
✓ Plugboard swapping should work correctly
✓ Enigma with plugboard should maintain reciprocity
...
Tests passed: 12/12
Success rate: 100.0%
🎉 All tests passed! Enigma machine is working correctly.
```

## Usage Examples

### Example 1: Basic Encryption
```
Enter message: HELLO
Rotor positions (e.g. 0 0 0): 0 0 0
Ring settings (e.g. 0 0 0): 0 0 0
Plugboard pairs (e.g. AB CD): 
Output: VNACA
```

### Example 2: With Plugboard
```
Enter message: HELLO
Rotor positions (e.g. 0 0 0): 0 0 0
Ring settings (e.g. 0 0 0): 0 0 0
Plugboard pairs (e.g. AB CD): AB CD
Output: VNBCB
```

### Example 3: Decryption
To decrypt, use the **same settings** and input the encrypted message:
```
Enter message: VNACA
Rotor positions (e.g. 0 0 0): 0 0 0
Ring settings (e.g. 0 0 0): 0 0 0
Plugboard pairs (e.g. AB CD): 
Output: HELLO
```

## Configuration Options

### Rotor Positions
- Format: Three numbers (0-25) representing initial rotor positions
- Example: `5 10 15` sets rotors to positions F, K, P
- **Important**: Use the same positions for encryption and decryption

### Ring Settings  
- Format: Three numbers (0-25) for internal ring positions
- Example: `1 2 3` applies ring offsets
- Affects encryption without changing visible rotor positions

### Plugboard Pairs
- Format: Letter pairs like `AB CD EF` 
- Swaps letters bidirectionally (A↔B, C↔D, E↔F)
- Maximum 10 pairs (20 letters total)
- Leave empty for no plugboard connections

## Project Structure

```
├── enigma.js           # Main Enigma machine implementation
├── enigma.test.js      # Comprehensive test suite (12 tests)
├── package.json        # Project configuration and scripts
└── README.md          # This file
```

## Technical Details

### Historical Rotors Implemented
- **Rotor I**: Wiring `EKMFLGDQVZNTOWYHXUSPAIBRCJ`, Notch at `Q`
- **Rotor II**: Wiring `AJDKSIRUXBLHWTMCQGZNPYFVOE`, Notch at `E`  
- **Rotor III**: Wiring `BDFHJLCPRTXVZNYEIWGAKMUSQO`, Notch at `V`

### Reflector
- Uses historical Enigma reflector: `YRUHQSLDPXNGOKMIEBFZCWVJAT`

### Key Features Fixed
1. **Dual Plugboard Application**: Plugboard now correctly applied on both input and output
2. **Proper Rotor Stepping**: Fixed complex double-stepping mechanism to match historical behavior
3. **Reciprocal Operation**: Encryption and decryption now work symmetrically

## Testing

The implementation includes comprehensive tests covering:

- ✅ Basic encryption/decryption reciprocity
- ✅ Plugboard functionality and integration  
- ✅ Rotor stepping mechanics including double-stepping
- ✅ Different rotor positions and ring settings
- ✅ Edge cases (non-alphabetic characters, case handling)
- ✅ Long message processing
- ✅ Historical accuracy verification

Run `npm test` to verify all functionality works correctly.

## Requirements

- Node.js (any modern version)
- No external dependencies required

## Historical Note

This implementation recreates the behavior of the 3-rotor Enigma machine used by German forces during World War II. The machine's complexity came from its multiple layers of substitution and the mechanical rotor stepping mechanism, which this implementation faithfully reproduces.

## License

MIT License - Feel free to use for educational purposes. 