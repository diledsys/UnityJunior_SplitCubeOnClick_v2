# Unity C# Architecture and Coding Standard

These rules apply to all C# code created, reviewed, refactored, or modified in this Unity project.

The priorities are:

1. Correctness
2. Readability
3. Maintainability
4. Clear responsibility boundaries
5. SOLID architecture
6. Runtime performance
7. Memory efficiency
8. Testability
9. Extensibility

Optimization must not make the architecture unnecessarily complicated.

## 1. Inspect Before Editing

Before changing code:

- inspect the relevant scripts;
- find callers and dependencies;
- understand which class owns the state;
- check prefab and scene references;
- search for an existing component with the same responsibility;
- preserve existing behavior unless a behavior change is requested;
- prefer the smallest coherent change.

Do not rewrite a working subsystem merely because another architecture is possible.

## 2. SOLID

### Single Responsibility

Every class should have one primary responsibility.

Avoid one MonoBehaviour handling input, movement, health, combat, animation, audio, UI, and saving.

Prefer focused components such as:

- PlayerInputReader
- PlayerMover
- PlayerAttack
- Health
- GroundChecker
- CharacterAnimatorView
- HealthView
- TargetDetector
- ProjectileSpawner

### Open / Closed

Prefer extending behavior through composition, configuration, interfaces, or strategies instead of continually expanding large conditional blocks.

Do not create abstractions without a real need.

### Liskov Substitution

Use inheritance only for genuine "is-a" relationships.

Do not inherit only to reuse code.

Prefer composition to deep inheritance.

### Interface Segregation

Keep interfaces small and focused.

Example:

```csharp
public interface IDamageable
{
    void TakeDamage(int damage);
}
```

Do not create universal interfaces containing unrelated behavior.

Do not create an interface automatically for every class.

### Dependency Inversion

Use abstractions when they create a useful architectural boundary, support multiple implementations, or improve testing.

Do not introduce unnecessary interfaces merely to satisfy SOLID mechanically.

## 3. Composition Over Inheritance

Composition is the default.

Preferred structure:

```text
Player
├── PlayerInputReader
├── PlayerMover
├── PlayerAttack
├── Health
├── GroundChecker
└── CharacterAnimatorView
```

Avoid deep inheritance hierarchies.

## 4. MonoBehaviour

Use MonoBehaviour only when Unity behavior is needed, such as:

- Awake / Start / OnEnable / Update / FixedUpdate;
- Transform;
- Rigidbody;
- Animator;
- Collider;
- Coroutines;
- Inspector serialization;
- scene or prefab integration.

Use ordinary C# classes for pure gameplay calculations when practical.

## 5. Explicit Dependencies

Dependencies must be clear.

Prefer serialized private references for scene/prefab dependencies:

```csharp
[SerializeField] private Rigidbody _rigidbody;
[SerializeField] private Animator _animator;
```

Avoid hidden scene searches.

## 6. Component Lookup

Do not repeatedly call these methods in hot paths:

- GetComponent
- GetComponentInChildren
- GetComponentInParent
- Find
- FindAnyObjectByType
- FindFirstObjectByType
- GameObject.Find
- GameObject.FindWithTag

Especially avoid them inside Update, FixedUpdate, LateUpdate, frequently called callbacks, or loops over many objects.

Cache stable references.

## 7. Magic Numbers

Magic numbers and unexplained strings are prohibited.

Prefer:

```csharp
[SerializeField, Min(0f)] private float _movementSpeed = 7.5f;
[SerializeField, Min(0f)] private float _attackDistance = 2.4f;
```

Use const for real invariants and SerializeField for tunable values.

## 8. Field Rules

Fields are private by default.

Prefer:

```csharp
[SerializeField] private float _speed;
```

instead of:

```csharp
public float Speed;
```

Expose read-only state through properties when required:

```csharp
public float Speed => _speed;
```

## 9. Boolean Naming

Boolean fields and properties should clearly express a condition.

Prefer:

- isGrounded
- isAlive
- isAttacking
- canMove
- canAttack
- hasTarget
- hasWeapon

Avoid vague names such as flag, check, bool1, tempBool, value.

## 10. Naming

Use names that describe responsibility.

Prefer:

- PlayerMover
- EnemyPatrol
- GroundChecker
- Health
- DamageDealer
- CharacterAnimatorView
- ProjectileSpawner
- TargetDetector

Avoid vague class names such as Manager, Controller, Helper, Handler, Utility, System unless the name accurately describes the responsibility.

Avoid unclear abbreviations.

## 11. Methods

Methods should do one clear thing.

Prefer clear names:

- Move
- Rotate
- Attack
- TakeDamage
- Heal
- TryAttack
- CanAttack
- Spawn
- Despawn
- ResetState

Avoid very long methods and excessive nesting.

Use early returns when they improve readability.

## 12. Course-Style Rules

For educational/course code:

- avoid `while (true)`;
- avoid `break` as a shortcut for poorly structured logic;
- use explicit loop conditions;
- keep control flow easy to follow;
- do not use advanced language features only to shorten code;
- prioritize readable educational structure.

## 13. Input Architecture

Separate input reading from gameplay execution.

Preferred flow:

```text
Unity Input System
        ↓
PlayerInputReader
        ↓
PlayerMover / PlayerAttack / PlayerInteraction
```

Input code reports intent.

Gameplay components decide how that intent affects the game.

Do not combine input, physics, animation, combat, health, and UI in one class.

## 14. Update

Treat Update as a hot path.

Before adding code, ask:

> Does this operation really need to execute every frame?

Prefer events, state changes, timers, cached values, coroutines, or lower-frequency checks where appropriate.

Do not add empty or unnecessary Update methods.

## 15. FixedUpdate

Use FixedUpdate primarily for Rigidbody physics operations.

Use Update primarily for input and normal frame-based gameplay logic.

Do not move logic to FixedUpdate merely because it must repeat.

## 16. Memory Allocations

Avoid avoidable allocations in hot paths.

Do not repeatedly create:

- List<T>
- Dictionary<TKey, TValue>
- arrays
- temporary classes
- temporary collections
- strings
- captured lambdas

inside frequently executed gameplay code.

Initialization-time allocations are normally acceptable.

## 17. Garbage Collector

Before repeatedly allocating, consider whether:

- the object can be reused;
- the collection can be cached;
- recalculation can happen only when data changes;
- an event can replace polling;
- object pooling is appropriate.

Prevent avoidable GC spikes.

Do not perform premature micro-optimization.

## 18. LINQ

LINQ is acceptable for initialization, Editor tooling, tests, configuration processing, and rare operations.

Avoid LINQ in gameplay hot paths unless profiling demonstrates that it is harmless.

Prefer explicit loops for performance-critical runtime logic.

## 19. Object Pooling

Evaluate pooling for frequently spawned/despawned objects such as:

- bullets;
- projectiles;
- VFX;
- damage indicators;
- repeated enemies;
- collectables.

Avoid frequent Instantiate/Destroy when pooling is practical.

Do not add pooling for rare objects where it adds unnecessary complexity.

## 20. Collections

Choose collections intentionally:

- List<T>: ordered dynamic collection
- Dictionary<TKey,TValue>: key lookup
- HashSet<T>: uniqueness / membership
- Queue<T>: FIFO
- Stack<T>: LIFO

Preallocate capacity when the approximate size is known and meaningful.

Reuse collections when practical.

## 21. Distance Checks

When only comparing distances, prefer squared distance:

```csharp
Vector3 offset = target.position - transform.position;

if (offset.sqrMagnitude <= _attackDistance * _attackDistance)
{
    Attack();
}
```

Use Vector3.Distance when the actual distance is required or when clarity matters more than a negligible optimization.

## 22. Physics

Avoid excessive physics queries every frame.

Use LayerMask and collision layers to reduce unnecessary checks.

For AI vision, targeting, and environmental scanning, consider lower-frequency checks when immediate response is not required.

Do not optimize physics blindly; consider frequency and object count.

## 23. Events

Prefer events when another system needs to react to a state change.

Example:

```csharp
public event Action<int> HealthChanged;
public event Action Died;
```

For MonoBehaviour subscriptions, normally subscribe in OnEnable and unsubscribe in OnDisable.

Avoid event leaks.

Avoid global static event buses unless clearly justified.

## 24. Health Architecture

Health owns health state.

Example responsibilities:

- CurrentHealth
- MaxHealth
- TakeDamage()
- Heal()
- HealthChanged
- Died

Other systems must not keep independent authoritative copies of health.

Preferred flow:

```text
DamageDealer
     ↓
Health
     ↓ events
HealthView
CharacterAnimatorView
Audio / Death behavior
```

## 25. UI

UI displays gameplay state; it should not own authoritative gameplay state.

Preferred flow:

```text
Health
   ↓ HealthChanged
HealthView
   ↓
Slider / Text
```

The Slider value is not the source of truth for player health.

## 26. Animator

Separate animation presentation from gameplay logic when practical.

Preferred flow:

```text
Gameplay state
      ↓
CharacterAnimatorView
      ↓
Animator
```

Cache Animator references.

Use Animator.StringToHash for repeatedly used parameters when appropriate.

Do not make gameplay architecture depend excessively on Animator internals.

## 27. Animation Events

Animation Events may synchronize meaningful points such as:

- attack impact;
- footstep;
- animation completion.

Keep gameplay ownership clear.

Do not place unrelated business logic in animation event handlers.

## 28. Rigidbody

Use Rigidbody-based movement correctly.

Do not manipulate Transform directly for a Rigidbody-controlled object unless there is a clear reason.

Do not mix incompatible movement strategies without understanding the consequences.

## 29. Runtime GameObjects

Do not create GameObjects merely to hold trivial data or logic.

Before creating one, consider:

- existing Transform;
- ordinary C# object;
- cached object;
- pooled GameObject;
- existing prefab hierarchy.

Every runtime-created GameObject needs a clear lifecycle and responsibility.

## 30. ScriptableObject

Use ScriptableObject mainly for reusable configuration such as:

- WeaponConfig
- EnemyConfig
- CharacterConfig
- AbilityConfig
- SpawnConfig

Keep configuration separate from runtime state.

Do not use ScriptableObject as accidental global mutable state.

## 31. Prefab and Serialization Safety

Before changing serialized field names, component types, required references, or prefab hierarchy, consider existing Unity serialization.

Do not casually rename serialized fields.

Do not remove components without checking dependencies.

Preserve Inspector assignments whenever possible.

Use Unity-supported migration tools such as FormerlySerializedAs when a serialized rename is necessary.

## 32. RequireComponent

Use RequireComponent when a MonoBehaviour cannot function without another component.

Do not use it for optional dependencies.

## 33. Static State

Avoid global mutable static gameplay state.

Static is appropriate for:

- constants;
- stateless helper methods;
- readonly Animator hashes;
- immutable shared data.

Gameplay state should have explicit ownership.

## 34. Strings

Do not identify gameplay types through object names.

Avoid:

```csharp
if (gameObject.name == "Enemy")
```

Prefer components, interfaces, enums, tags, layers, or explicit configuration depending on the problem.

Avoid repeated string creation in hot paths.

## 35. Debug Logging

Do not leave Debug.Log calls in hot loops.

Use logs for meaningful diagnostics.

Remove temporary debug output after resolving the issue unless it has lasting diagnostic value.

## 36. State Ownership

Every mutable gameplay value must have one clear owner.

Examples:

- Health owns health.
- PlayerMover owns movement state.
- PlayerAttack owns attack state.
- Inventory owns inventory data.

Avoid duplicate sources of truth.

## 37. State Machines

When behavior becomes complex, prefer explicit states instead of many booleans that can form invalid combinations.

Example:

```text
Idle
Move
Attack
Hit
Dead
```

Do not introduce a state-machine framework for trivial behavior.

## 38. Coroutines

Use coroutines for Unity-oriented timed sequences.

Avoid starting duplicate coroutines accidentally.

Store Coroutine references when cancellation or replacement is required.

## 39. KISS / YAGNI / DRY

Keep the architecture as simple as possible while maintaining clear responsibilities.

Do not implement speculative functionality.

Avoid meaningful duplication, but do not create a bad abstraction merely to remove several repeated lines.

## 40. Optimization Priority

Optimize in this order:

1. Algorithm
2. Execution frequency
3. Number of objects executing the code
4. Unity API calls
5. Physics queries
6. Instantiate / Destroy
7. Memory allocations / GC
8. Collection choice
9. Micro-optimizations

Fix architectural costs before tiny arithmetic costs.

## 41. Hot-Path Review

For frequently executed code, check:

- Does it allocate?
- Does it search the scene?
- Does it call GetComponent repeatedly?
- Does it use LINQ?
- Does it construct strings?
- Does it Instantiate or Destroy?
- Does it execute physics queries?
- Does it scan large collections?
- Does it recalculate unchanged data?
- Does it actually need to run this often?

## 42. Testing

Prioritize tests for logic such as:

- health;
- damage;
- healing;
- cooldowns;
- inventory;
- state transitions;
- score;
- resource calculations;
- target selection;
- spawn logic;
- pure algorithms.

When fixing a reproducible bug, add a regression test when practical.

Do not create meaningless tests only to increase test count.

## 43. Refactoring

When refactoring:

- preserve behavior;
- avoid unrelated changes;
- preserve prefab references;
- preserve serialized Inspector data;
- keep changes small and reviewable;
- check event subscriptions;
- check null dependencies;
- inspect allocations in modified hot paths;
- verify newly introduced Update methods are necessary.

## 44. Comments

Comments should primarily explain WHY.

For educational code, detailed Russian comments are allowed when useful for learning.

Do not fill production code with comments that only repeat the code.

## 45. Codex Behavior

When implementing a feature:

1. Inspect existing relevant code.
2. Search for an existing component with the same responsibility.
3. Identify the owner of state.
4. Identify required dependencies.
5. Determine whether Update/FixedUpdate is necessary.
6. Consider event-driven communication.
7. Consider runtime allocations and physics frequency.
8. Preserve existing scenes/prefabs.
9. Make the smallest coherent change.
10. Verify compilation when possible.

Do not invent architecture without understanding the project.

Do not silently delete functionality.

Do not modify unrelated files.

## 46. Existing Problems Outside Scope

If unrelated architectural problems are found:

- do not automatically rewrite them;
- mention significant issues;
- fix them only if they directly affect the requested task or the user explicitly requests refactoring.

## 47. Report After Changes

After modifying code, report:

1. What changed.
2. Why it changed.
3. Which files changed.
4. What architecture was used.
5. Relevant SOLID considerations.
6. Performance/memory considerations.
7. Required Unity Inspector setup.
8. How to test the result.
9. Remaining risks or known issues.

## 48. Definition of Done

A task is complete when:

- code compiles;
- responsibilities are clear;
- existing behavior is preserved unless intentionally changed;
- no unnecessary magic numbers were introduced;
- dependencies are explicit;
- no obvious per-frame allocation was introduced;
- no unnecessary scene search was introduced;
- no unnecessary GetComponent call was introduced into a hot path;
- no unnecessary Update method was introduced;
- events are unsubscribed correctly;
- runtime state has one owner;
- names clearly express intent;
- prefab/Inspector requirements are identified;
- architecture is not more complex than necessary.

## Final Principle

Prefer:

```text
small focused components
+ explicit dependencies
+ clear data ownership
+ composition
+ events where useful
+ cached stable references
+ minimal hot-path allocations
+ profiling-driven optimization
```

over:

```text
giant MonoBehaviours
+ global mutable state
+ magic numbers
+ scene searches
+ per-frame allocations
+ frequent Instantiate/Destroy
+ hidden dependencies
+ duplicate sources of truth
+ unnecessary abstractions
```
