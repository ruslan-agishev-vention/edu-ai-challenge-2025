# Modern Battleship Game

A modernized and refactored version of the classic Sea Battle (Battleship) game, rewritten in TypeScript with modern ES6+ features, comprehensive unit tests, and clean architecture.

## 🚀 Features

- **TypeScript**: Full type safety and modern language features
- **ES6+ Syntax**: Classes, modules, arrow functions, const/let, async/await  
- **Modular Architecture**: Clean separation of concerns with dedicated modules
- **Comprehensive Unit Tests**: Jest testing framework with 60%+ coverage
- **Smart AI**: CPU opponent with 'hunt' and 'target' modes
- **10x10 Grid**: Standard Battleship board size with turn-based gameplay

## 📁 Project Structure

```
├── src/
│   ├── types.ts          # TypeScript type definitions
│   ├── Ship.ts           # Ship class implementation
│   ├── Board.ts          # Game board management
│   ├── Player.ts         # Human player implementation
│   ├── AIPlayer.ts       # AI opponent with hunt/target logic
│   ├── Game.ts           # Main game orchestration
│   ├── GameUI.ts         # User interface handling
│   └── index.ts          # Entry point and demo
├── tests/
│   ├── Ship.test.ts      # Ship class unit tests
│   ├── Board.test.ts     # Board management tests
│   ├── Player.test.ts    # Player behavior tests
│   ├── AIPlayer.test.ts  # AI logic and strategy tests
│   └── Game.test.ts      # Game flow and integration tests
├── package.json          # Dependencies and scripts
├── tsconfig.json         # TypeScript configuration
├── jest.config.js        # Jest testing configuration
└── README.md             # This file
```

## 🛠️ Installation & Setup

### Prerequisites
- Node.js (v16 or higher)
- npm (v7 or higher)

### Installation Steps

1. **Install Dependencies**
   ```bash
   npm install
   ```

2. **Build the Project**
   ```bash
   npm run build
   ```

3. **Run Tests**
   ```bash
   npm test
   ```

4. **Run with Coverage**
   ```bash
   npm run test:coverage
   ```

5. **Play the Game**
   ```bash
   npm run play
   ```

6. **Run Demo**
   ```bash
   npm run demo
   ```

## 🧪 Testing

The project includes comprehensive unit tests covering:

- **Ship Logic**: Hit detection, sinking, state management
- **Board Management**: Ship placement, coordinate validation, guess processing
- **Player Behavior**: Guess tracking, win/loss detection, input validation
- **AI Strategy**: Hunt/target modes, adjacent coordinate logic, edge case handling
- **Game Flow**: Turn management, game state transitions, win conditions

### Running Tests

```bash
# Run all tests
npm test

# Run tests in watch mode
npm run test:watch

# Generate coverage report
npm run test:coverage
```

## 🎮 How to Play

### Game Rules
1. Both player and CPU have 3 ships of length 3 on a 10x10 grid
2. Players take turns guessing coordinates (e.g., "23" for row 2, column 3)
3. Hits are marked with "X", misses with "O"
4. Ships are marked with "S" on your board, hidden on opponent's board
5. First player to sink all opponent ships wins

### Game Symbols
- `~` - Water (unknown)
- `S` - Your ships
- `X` - Hit
- `O` - Miss

## 🔧 Key Improvements from Original

1. **Modern JavaScript/TypeScript**
   - Replaced `var` with `const`/`let`
   - Used arrow functions and modern syntax
   - Added full type safety with TypeScript
   - Implemented proper error handling

2. **Modular Architecture**
   - Separated concerns into dedicated classes
   - Eliminated global variables
   - Encapsulated state and behavior
   - Added clear interfaces and abstractions

3. **Enhanced Code Quality**
   - Consistent naming conventions
   - Comprehensive documentation
   - Defensive programming practices
   - Clean, readable code structure

4. **Robust Testing**
   - Unit tests for all core functionality
   - Edge case and error condition coverage
   - Automated testing pipeline
   - 60%+ coverage requirement

## 🤝 Original Game Compatibility

This modernized version maintains **100% compatibility** with the original game mechanics:

- ✅ 10x10 grid
- ✅ Turn-based coordinate input (e.g., 00, 34)
- ✅ Standard hit/miss/sunk logic
- ✅ CPU opponent with hunt/target modes
- ✅ Same victory conditions and game flow

## 📊 Architecture Overview

### Core Components

- **Ship Class**: Manages individual ship state and hit detection
- **Board Class**: Handles the 10x10 game grid and ship placement
- **Player Class**: Base class for human player interactions
- **AIPlayer Class**: Intelligent AI with hunt/target strategy
- **Game Class**: Central game orchestration and state management
- **GameUI Class**: User interface abstraction layer

## 📝 License

MIT License - Feel free to use, modify, and distribute.

## 🙏 Acknowledgments

Based on the classic Battleship game concept, modernized with current software development best practices and comprehensive testing methodologies. 