# 🏰 CrusaderAI

### *An Unhinged Experiment in Medieval Game State Archaeology*

> "What if we taught a language model to play a 22-year-old RTS game?"
> — Someone who clearly has their priorities straight

---

## 🎯 What Is This?

**CrusaderAI** is an experimental research project (and by "research" we mean "weekend rabbit hole") aimed at:

1. **Extracting game state** from *Stronghold Crusader: Definitive Edition*
2. **Visualizing & investigating** the inner workings of medieval castle economics
3. **Building a dataset** for training LLM agents to (maybe, someday, possibly) play the game

This is not a serious endeavor. This is what happens when you mix nostalgia, curiosity about game memory structures, and an unhealthy interest in siege warfare AI.

---

## 🔬 The "Research" Goals

```
┌─────────────────────────────────────────────────────────────────┐
│  PHASE 1: The Archaeology                                       │
│  └─► Memory scanning, pointer chasing, structure mapping        │
│                                                                 │
│  PHASE 2: The Observatory                                       │
│  └─► Real-time game state visualization & logging               │
│                                                                 │
│  PHASE 3: The Frankenstein                                      │
│  └─► Feed it all to an LLM and see what happens                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🧠 Why LLMs + Stronghold Crusader?

Because:
- RTS games require **complex multi-objective reasoning**
- Resource management is basically **token budgeting** (the LLM understands this spiritually)
- Medieval economics is **peak content** for training data
- *"The Rat has arrived"* energy is something AI should learn

### The Vision™

Imagine an LLM that can:
- Read the current game state (gold, wood, population, enemy positions)
- Reason about strategy ("The Wolf is massing archers, I should build shields")
- Output valid game commands
- Lose to The Pig anyway because that's the Crusader experience

---

## 📊 Data We're Hunting

| Category | Examples | Status |
|----------|----------|--------|
| **Resources** | Gold, Wood, Stone, Iron, Food types | 🔍 Investigating |
| **Population** | Workers, Military units, Happiness | 🔍 Investigating |
| **Buildings** | Placement, Health, Production state | 🔍 Investigating |
| **Military** | Unit positions, Health, Commands | 🔍 Investigating |
| **AI Lords** | Enemy state, Personality quirks | 🔍 Investigating |

---

## 🛠️ Tech Stack (Probably)

- **Memory Reading**: Windows API shenanigans
- **Game State Parsing**: Reverse engineering & pattern matching
- **Visualization**: TBD (something that looks cool)
- **LLM Integration**: Future problem for future us

---

## ⚠️ Disclaimer

This project is:
- ✅ For educational and entertainment purposes
- ✅ A love letter to a classic game
- ✅ An excuse to learn about game memory structures
- ❌ Not affiliated with Firefly Studios
- ❌ Not intended for cheating or online play
- ❌ Not guaranteed to produce a functioning AI (lol)

---

## 🗺️ Roadmap

- [ ] Successfully read *any* game value from memory
- [ ] Map out core game structures
- [ ] Build real-time state extraction
- [ ] Create training data format for LLM consumption
- [ ] Fine-tune a model on Crusader gameplay
- [ ] Watch it fail spectacularly
- [ ] Blog about it

---

## 🤝 Contributing

Found a memory offset? Figured out how the game stores unit data? Want to help train an AI to finally defeat The Wolf?

Open an issue or PR. This is uncharted territory and all explorers are welcome.

---

## 📜 License

MIT — Do whatever you want, just don't blame us when your AI builds 47 wheat farms and no army.

---