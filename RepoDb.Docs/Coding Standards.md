This page is still in-progress.

## Folders

1. Interfaces
2. Attributes
3. Enumerations
4. Operations
5. Resolvers
6. Requests

## Properties

1. Use *ProperCase* when implementing the class properties.
2. Usage of *get/set* directly.
3. Usage of direct assignment.

## Variables

1. Use the `var` keyword when declaring the method-level variables.
2. Use `camelCase` when declaring the method-level variables.
3. Declare a meaningful variable name (ie: `propertyIndex` over `x`).
4. Usage of prefix `m_` for private variables.

## Regions

1. Create a region for the properties.
2. Create a region for the private variables.
3. Create a region for the instance methods.
4. Create a region for the static methods.
5. Create a region for the static properties.
6. Create a region for the static private variables.

## Looping

1. Always use `foreach` or `for (var i)`. Do not use *Linq ForEach*.

## Coding

1. Always open and close the conditional statements with curly-brackets.
2. Always add an XML-comments in all public (method, properties, classes, interfaces, enumerations, etc).
3. The shorter the better (less then 25 lines of codes per method).
4. Avoid the usage of `this` and `base` keywords, unless very necesarry.
5. Always use the `AsList()` over `ToList()`.

## Operations

1. Separation by file name (partial class declaration).
