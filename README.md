# Unity Auto Compile
this extesion will trigger Unity to compile when saved a file in VS Code.
![Animation](https://github.com/PostCyberPunk/UnityAutoCompile/assets/134976996/cb126a25-b72b-4152-898c-06ae374a7ffe)
## Requirement  

Install [this package](https://github.com/PostCyberPunk/VSCodeUnityAutoCompliation) to your projcet [Not yet]  

------------------------------------------------------------------------------------------------------------------
Unity:   
Place CLz_AutoCompile under Editor

VsCode：  
In VsCode-Extensions-upper right corner-install with .VSIX here


## Known isssues
~~You have to force compile once before the auto-compilation works~~

~~If there is errors in Unity the auto compilation will stop~~


## Commands
Unity Force Compile: unity-auto-compile.compile
Toggle Unity Compilation :unity-auto-compile.toggle

## How it works
Unity will host a HTTP listener,Code will send a GET request to unity when a file is saved,then Unity will call `assetDatabase.Refresh()`

Inspried by [baba-s's project](https://github.com/baba-s/unity-compile-in-background)

## Notes
Extension will auto activate when there is a folder named "ProjectSettings" in workspace,or you can run `Unity Force Compile` once to activate it.

## Extension Settings
```
"unity-auto-compile.enabled":true,
"unity-auto-compile.port": "10245",//the HTTP port between unity and vscode 
```
