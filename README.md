# JSDocParser
Lightweight simple JSDoc documentation parser for .NET

Purpose: Read javascript files and parse JSDoc style documentation (within code), available in a .NET (C# project).

# Quick mini-doc (before doing it for real)
```C#
var js = new JSDocParser();
var parsed = js.Parse("test1.js"); 
```

This js code:
```javascript
/**
 * createTile
 *
 * @param {string} text Text to display on the tile.
 * @param {float} durationSeconds Duration to display the tile, in seconds. Defaults to 10.
 *
 */

function createTile(text, durationSeconds = 10) {
...
}
```
would result in this result from js.Parse(...):

```C#
parsed.Function.Count == 1
parsed.Function[0].Name == "createTile"
parsed.Function[0].Parameters.Count == 2
parsed.Function[0].Parameters[0].Name == "text"
parsed.Function[0].Parameters[0].Type == "string"
parsed.Function[0].Parameters[0].Description == "Text to display on the tile."
parsed.Function[0].Parameters[1].Default == "10" //retrieved from signature line, not from docs
```
