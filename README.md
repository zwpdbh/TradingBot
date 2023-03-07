# Initialize project
```
dotnet tool restore
paket install
```

# About Paket
### Install Paket
```
# Install it globally
dotnet tool install paket -g

# Or
dotnet new tool-manifest
dotnet tool install paket
dotnet tool restore
```

### Initialize Paket from solution
```
dotnet paket init
```

### Useful commands
```
# Convert projects from NuGet to Paket in the solution root.
paket convert-from-nuget --force

# Install dependencies
paket install

# Check outdate and update
paket outdated
paket update
```