# Debug Console Setup Guide

## 🚀 **AUTOMATED SETUP** (Recommended)

### One-Click Setup
1. In Unity, go to **Tools → Gangsta Bean → Setup Console System**
2. Click **"Setup Console System"**
3. Done! Console is ready to use in play mode.

*The automated setup creates everything for you: ConsoleManager, dedicated Console Canvas, UI hierarchy, Input Actions, New Input System integration, references, and styling.*

**Toggle Keys:** ~ (backtick) or C key

---

## Manual Setup Instructions (If Needed)

### 1. Add Console Manager to Scene
1. Create an empty GameObject named "ConsoleManager"
2. Add the `ConsoleManager` script to it
3. Configure the toggle key (default: ~ key)

### 2. Create Console UI
1. Create a dedicated Canvas for the console (recommended)
2. Add the console UI structure:

```
Console Canvas (dedicated, sorting order 1000)
└── ConsolePanel (Panel)
    ├── OutputScrollView (Scroll View)
    │   └── Viewport
    │       └── Content
    │           └── OutputText (TextMeshPro - Text)
    ├── InputField (TMP_InputField)
    └── SuggestionPanel (Panel)
        └── SuggestionContainer (Vertical Layout Group)
            └── SuggestionButton (Button with TextMeshPro)
```

### 3. Connect UI Components
1. Add `ConsoleUI` script to the ConsolePanel
2. Drag references:
   - Console Panel → ConsolePanel
   - Input Field → InputField
   - Output Text → OutputText
   - Scroll Rect → OutputScrollView
   - Suggestion Panel → SuggestionPanel
   - Suggestion Container → SuggestionContainer
   - Suggestion Button Prefab → SuggestionButton

### 4. Configure Console Panel
- Set ConsolePanel to inactive by default
- Position at top half of screen
- Make semi-transparent background
- Set appropriate sorting order

## Available Commands

### Player Commands
- `respawn <player> [x y] [fromsky]` - Respawn player
- `heal <player> [amount]` - Heal player
- `kill <player>` - Kill player
- `teleport <player> <x> <y>` - Teleport player
- `god <player> [on/off]` - Toggle god mode
- `players` - List all players
- `sethealth <player> <amount>` - Set specific health

### Ammo Commands
- `fillammo [player/all] [ammo_type]` - Fill ammo
- `giveammo <player> <ammo_type> <amount>` - Give specific ammo
- `clearammo <player> [ammo_type]` - Clear ammo
- `ammostatus <player>` - Show ammo status

### Enemy Commands
- `spawnenemy <type> <x> <y>` - Spawn enemy
- `killenemies [x y radius]` - Kill enemies
- `clearenemies` - Clear all enemies
- `wave [wave_number]` - Trigger wave
- `enemystatus` - Show enemy info

### Game Flow Commands
- `pause [on/off]` - Pause/unpause game
- `timescale <scale>` - Change game speed
- `restart` - Restart level
- `loadlevel <name>` - Load specific level
- `status` - Show game status
- `reloadstats` - Reload unit stats
- `quit` - Quit game

### Basic Commands
- `help [command]` - Show help
- `clear` - Clear console

## Usage Examples

```
> respawn player1 100 200 fromsky
> heal all
> fillammo all primary
> spawnenemy zombie 150 100
> timescale 0.5
> god player1 on
> wave 3
```

## Adding Custom Commands

1. Create a new class implementing `IConsoleCommand`
2. Add `[ConsoleCommand]` attribute
3. Implement required methods:
   - `Name` - Command name
   - `Description` - Help description
   - `Aliases` - Alternative names
   - `Usage` - Usage example
   - `ValidateParameters()` - Parameter validation
   - `Execute()` - Command execution

Example:
```csharp
[ConsoleCommand]
public class MyCommand : IConsoleCommand
{
    public string Name => "mycommand";
    public string Description => "Does something cool";
    public string[] Aliases => new[] { "mc" };
    public string Usage => "mycommand <parameter>";
    
    public bool ValidateParameters(string[] parameters)
    {
        return parameters.Length == 1;
    }
    
    public string Execute(string[] parameters)
    {
        // Your command logic here
        return "Command executed!";
    }
}
```

## Features

- **New Input System** - Uses Unity's modern Input System (not legacy Input)
- **Auto-completion** - Tab to complete commands
- **Command history** - Up/Down arrows to navigate
- **Parameter validation** - Built-in parameter checking
- **Color-coded output** - Different colors for different message types
- **Extensible** - Easy to add new commands
- **Help system** - Built-in help for all commands
- **Error handling** - Graceful error messages
- **Multiple toggle keys** - ~ (backtick) or C key
- **Input Actions asset** - Configurable key bindings

## Tips

- Use `help` command to see all available commands
- Use `help <command>` for specific command help
- Commands and parameters are case-insensitive
- Use quotes for parameters with spaces: `teleport "Player 1" 100 200`
- Press Escape to close the console
- Console remembers command history between sessions