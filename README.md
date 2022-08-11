# WowNav

## Requirements for modifying/compiling code:

- Visual Studio 2022 (make sure below are installed via Visual Studio Installer)
  - Installed workloads:
      - ASP.NET and web development
      - Desktop development with C++
  - Installed individual components:
      - .NET 6.0 SDK/Runtime
      - .NET 4.7.2 targeting pack
      - .NET SDK
      - MSVC v143 - VS 2022 C++ x64/x86 build tools (Latest)
      - C++ ATL for latest v143 build tools (x86 & x64)
      - Windows 10 SDK (10.0.19041.0)

## Solution Projects

- Navigation - C++ library that wraps [Recast/Detour](https://github.com/recastnavigation/recastnavigation) which reads the raw WoW map geometry to generate navigation meshes and calculate movement in the game world.
- NavigationTests - unit tests for the Navigation project.
- WowNavApi - .NET Web API that exposes a single `CalculatePath` endpoint which calls into the Navigation library under the hood. Targets .NET 6.
- WowNavApiTests - unit tests for the WowNavApi project.
- WowNavBase - contains some types that are used across various projects. Targets .NET Standard 2.0 to ensure we can consume it from both .NET 6 and .NET Framework 4.7.2 projects.
- WowNavClient - C# library that wraps an HttpClient to abstract the network calls to the WowNavApi server. Targets .NET Framework 4.7.2.
- WowNavClientExample - C# console application providing example usage of WowNavClient. Targets .NET Framework 4.7.2.

## Usage

Compilation output goes into either `WowNav\Output\x86\Debug\net6.0` or `WowNav\Output\x86\Release\net6.0` depending on build configuration.

To start WowNavApi, open a cmd prompt and navigate to `WowNav\Output\x86\Release\net6.0`, then run `WowNavApi.exe <listenHostandPort>` (ie: `WowNavApi.exe http://localhost:5000`). WowNavApi will start listening on whatever hostname and port provided here.

**IMPORTANT** the `mmaps` folder must be adjacent to `WowNavApi.exe`. These are the move maps generated from the Wow static game geometry, and are used by Recast/Detour while performing pathfinding. The `mmaps` folder is included in the base solution folder (`WowNav/mmaps`), and has also been copied to both `WowNav\Output\x86\Debug\net6.0` and `WowNav\Output\x86\Release\net6.0`. If you move the WowNavApi or change the compilation output directory, make sure you also move the `mmaps` folder as well.

Once `WowNavApi.exe` is running, you can test it using `WowNavClientExample.exe`. Either navigate to `C:\Users\Drew\source\repos\WowNav\WowNavClientExample\bin\x86\Release` in a cmd prompt and run `WowNavClientExample.exe`, or debug the project inside Visual Studio.

You can also just start `WowNavApi.exe` and hit the API using something like [Insomnia](https://insomnia.rest/).

Note that when generating a path for a given mapId, the first time will have to parse all the .mmtile files for the given mapId and load them into memory. This takes a few seconds. After they're loaded the first time, path generation should be quite fast.

Example request:

```
POST http://localhost:5000/Navigation/CalculatePath

JSON Body:

{
  "mapId": 1,
  "start": {
    "X": -614.7,
    "Y": -4335.4,
    "Z": 40.4
  },
  "end": {
    "X": -590.2,
    "Y": -4206.1,
    "Z": 38.7
  },
  "straightPath": false
}
```

Example response:
```
[
	{
		"x": -614.7,
		"y": -4335.4,
		"z": 40.4
	},
	{
		"x": -612.2178,
		"y": -4332.263,
		"z": 40.97542
	},
	{
		"x": -609.73553,
		"y": -4329.1265,
		"z": 40.8632
	}

    ...
]
```

StraightPath is a boolean argument that, when true, will ignore any obstacles and generate a straight line between the two points. In most general cases this should be `false`.

## Troubleshooting

Open two instances of Visual Studio. In one, start debugging WowNavApi. Once that's running, in the other Visual Studio instance, start debugging WowNavClientExample. Step through the code in both instances of Visual Studio to determine the source of failure.

For issues inside the C++ Navigation library, debugging the unit tests in NavigationTests can be helpful because you can step into the Navigation library code. Consider adding additional unit tests to cover edge cases that are uncovered.