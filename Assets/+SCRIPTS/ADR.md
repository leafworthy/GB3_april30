# Architecture Decision Records (ADR)

## ADR 001: Singleton Pattern Usage

### Context
Singletons are widely used throughout the codebase for managing global state and ensuring that there is only one instance of particular components, like the `GameManager` and `ASSETS`.

### Decision
To fully utilize the Singleton pattern for core game management objects such as `GameManager`.

### Consequences
- **Pros:** Simplifies access to global components.
- **Cons:** May create hidden dependencies and reduce modularity.

## ADR 002: Service Locator Pattern

### Context
The Service Locator pattern provides a versatile way to manage service dependencies between components.

### Decision
Implemented to decouple service provisioning from components using `ServiceLocator` and `ServiceUser`.

### Consequences
- **Pros:** Encourages modularity and reusability.
- **Cons:** Can obscure dependencies which may affect testability and clarity.

## ADR 003: State Machine Implementation

### Context
State machines manage different AI and game states, enabling clear transitions and encapsulated logic.

### Decision
Adopted the State pattern with `State<T>` and `StateMachine<T>` for AI management.

### Consequences
- **Pros:** Improves clarity and simplifies state transitions.
- **Cons:** Requires understanding of state pattern by developers.

## ADR 004: Event-Driven Architecture

### Context
The event-driven architecture is employed for decoupling components and managing asynchronous behaviors.

### Decision
Used extensively in the `Life` class, providing events like `OnAttackHit` and `OnDamaged`.

### Consequences
- **Pros:** Enhances flexibility and responsiveness.
- **Cons:** Complex event chains can be difficult to debug.
