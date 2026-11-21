# Pedalboard Power Match

Match synthetic pedal requirements against individual power outputs by voltage and current capacity.

![Sample run from Pedalboard Power Match](docs/demo.webp)

## Run

Install .NET 8 SDK and runtime. Run these commands from the repository root.

```sh
dotnet build --nologo
dotnet bin/Debug/net8.0/App.dll sample.json
```

Replace `sample.json` with a JSON file following the same structure. The app prints a JSON report to standard output.

Each entry is a unique name, positive voltage and positive current in mA. This worksheet models one pedal per output and leaves polarity and manufacturer requirements to the user.

## Verify

Run `python check.py` with Python 3.11 or newer after the build step when applicable. The check verifies the bundled scenario and confirms that an invalid input returns an error.
