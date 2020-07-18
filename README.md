# altV-InteractionsJsWrapper

A Simple Javascript Wrapper to work with C# Interactions

## Installation

You can create a C # resource by following the instructions in the documentation [Create Resource](https://fabianterhorst.github.io/coreclr-module/articles/create-resource.html).

## Usage

First step is to import our C# resource exports.

```javascript
import * as interactions from 'resource_name';
```

> Note: Default import won't work.

### Functions

Registers a client event on the server that listens for incoming events from the client itself.

```typescript
export function registerInteractionEvent(name: string): void;
```

Removes a client event from the server.

```typescript
export function unregisterInteractionEvent(name: string): void;
```

Creates a new interaction with the specified parameters.

```typescript
export function createInteraction(
	type: number,
	id: number,
	position: Vector3,
	dimension: number,
	range: number
): number;
```

Changes the position of the created interaction.

```typescript
export function setInteractionPosition(type: number, id: number, position: Vector3): void;
```

Changes the distance of the created interaction.

```typescript
export function setInteractionRange(type: number, id: number, range: number): void;
```

Changes the dimension of the created interaction.

```typescript
export function setInteractionDimension(type: number, id: number. dimension: number): void;
```

Removes interaction with specified parameters.

```typescript
export function removeInteraction(type: number, id: number): void;
```

### Example

> Client Side

```javascript
import alt from 'alt-client';

alt.on('keydown', (key) => {
	alt.log(key);
	alt.emitServer('customKeyDown', key);
});
```

> Server Side

```javascript
import alt from 'alt-server';
import * as interactions from 'interactions';

console.log(alt.getResourceExports('interactions'));

interactions.registerInteractionEvent('customKeyDown');
interactions.createInteraction(0, 12, new alt.Vector3(0, 0, 72), 0, 100);

alt.on('customKeyDown', (player, interactions, key) => {
	if (key === 69) {
		console.log(player.name);
		console.log(`Key: ${key}`);
		console.log(JSON.stringify(interactions));
	}
});
```
