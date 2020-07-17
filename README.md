# altV-InteractionsJsWrapper

A Javascript Wrapper for C# Interactions

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

Removes interaction with specified parameters.

```typescript
export function removeInteraction(type: number, id: number): void;
```

### Example

> Client Side

```javascript
import alt from 'alt-client';

// We will use the already implemented event from alt to listen to key presses
alt.on('keydown', (key) => {
	alt.log(key);
	alt.emitServer('keyDown', key); // We use the name of the event that we registered on the server side
});
```

> Server Side

```javascript
import alt from 'alt-server';
import * as interactions from 'interactions';

console.log(alt.getResourceExports('interactions'));

interactions.registerInteractionEvent('keyDown'); // Registering an event with a name 'keyDown'
interactions.createInteraction(0, 12, new alt.Vector3(0, 0, 72), 0, 100); // We create an interaction on the specified position

// We listen to the registered event
// The second parameter is array of interactions to which we were closest when we pressed a key
// The third parameter is our key which we sent from the client side
alt.on('keyDown', (player, interactions, key) => {
	if (key === 69) {
		console.log(player.name);
		console.log(`Key: ${key}`);
		console.log(JSON.stringify(interactions));
	}
});
```
