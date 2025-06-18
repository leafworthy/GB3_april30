# Gangsta Bean 3 - Development Guidelines

## Standard Workflow
1. First think through the problem, read the codebase for relevant files, and write a plan to projectplan.md
2. The plan should have a list of todo items that you can check off as you complete them
3. Before you begin working, check in with me and I will verify the plan
4. Then, begin working on the todo items, marking them as complete as you go
5. Please every step of the way just give me a high level explanation of what changes you made
6. Make every task and code change you do as simple as possible. We want to avoid making any massive or complex changes. Every change should impact as little code as possible. Everything is about simplicity
7. Finally, add a review section to the projectplan.md file with a summary of the changes you made and any other relevant information

## Communication Guidelines
- Never use pleasantries like "you're right!" or similar acknowledgments
- Be direct and concise in all responses
- Focus on technical content and implementation details
- Avoid unnecessary commentary or validations

## Technical Requirements
- **Always use Unity's New Input System** - Never implement legacy Input Manager code
- All input handling must use InputActions and InputActionMaps
- Input controls should be configured through .inputactions files
- Use InputAction callbacks and event-driven input patterns

## Code Standards
- Prioritize simplicity over complexity in all implementations
- Minimize the scope of changes - affect as few files as possible
- Follow existing code patterns and conventions in the project
- Maintain consistent naming and organization structures
- Use minimal commenting, only for uncommon terms or non-obvious logic

## Codebase Analysis Summary
This Unity project shows strong organization at the folder level but has several architectural areas for improvement:

### Main Areas for Improvement
1. **Dependency Management**: Moving away from Singletons toward dependency injection or service locators
2. **Performance Optimization**: Addressing component access patterns and physics optimization  
3. **Architecture Standardization**: Consistent initialization, state management, and inheritance patterns

### Suggested Development Order
1. Start with performance optimizations to improve current gameplay experience
2. Address specific subsystem issues (scene management, enemy AI) to fix immediate bugs
3. Gradually implement architectural improvements as you work on new features

## Project Structure Notes
- Scripts are organized in the `Assets/++SCRIPTS/` directory with clear categorical folders
- Each major system has its own folder (+ABILITIES, +ATTACKS, +PLAYER, etc.)
- FX, SFX, and UI components are properly separated
- Console system is available for debugging and testing