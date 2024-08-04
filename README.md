# Unity Editor Helpers
This repository contains some useful stuff when working with Unity Editor.

## Install
Via Package Manager:
```
https://github.com/oneir0mancer/unity-editor-helpers.git
```

## Attributes 

```
[ReadOnly] public ScriptableObject SomeField;
```
Easy way to prohibit changing field from Inspector.

```
[Nested] public ScriptableObject SomeField;
```
Allows to create an asset of type that can be assigned to that field as a subasset inside the parent asset.

```
[Expandable] public ScriptableObject SomeField;
```
Allows to expand the fields of referenced Scriptable Object.

## Optional

```
public Optional<ScriptableObject> SomeField;
```
A struct wrapper that allows to set if inner reference is used or not, instead of checking for null.

## Editor Windows

### Create Scriptable Object
A window for creating Scriptable Objects with a search function over types and CreateAssetMenu attribute names. 

### Rename Asset Window
A window that allows to rename any asset, including subassets.