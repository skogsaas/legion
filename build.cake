var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var nugetsource = "http://192.168.1.100:81/nuget/Default/";
var packages = "./packages";
var artifacts = "./artifacts/";
var projectpath = "./Legion/";
var project = projectpath + "Legion.csproj";

var nugetRestoreSettings = new NuGetRestoreSettings
	{
		PackagesDirectory = packages
	};

var nugetUpdateSettings = new NuGetUpdateSettings
	{
	};

var nugetPackSettings = new NuGetPackSettings
	{
		OutputDirectory = artifacts,
		Symbols = true,
		Properties = new Dictionary<string, string>{{"Configuration", configuration}}
	};

var nugetPushSettings = new NuGetPushSettings
	{
		Source = nugetsource,
		ApiKey = "Development:Development"
	};

var msbuildSettings = new MSBuildSettings 
	{
		Verbosity = Verbosity.Minimal,
		ToolVersion = MSBuildToolVersion.VS2015,
		Configuration = configuration,
		PlatformTarget = PlatformTarget.MSIL
	};

Task("Clean")
	.Does(() =>
{
	CleanDirectory(artifacts);
});

Task("Restore")
	.IsDependentOn("Clean")
	.Does(() => 
{
	if(FileExists(projectpath + "packages.config"))
	{
		NuGetRestore(project, nugetRestoreSettings);
	}
});

Task("Update")
	.IsDependentOn("Restore")
	.Does(() =>
{
	if(FileExists(projectpath + "packages.config"))
	{
		NuGetUpdate(project, nugetUpdateSettings);
	}
});

Task("Build")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.IsDependentOn("Update")
	.Does(() =>
{
	MSBuild(project, msbuildSettings);
});

Task("Package")
	.IsDependentOn("Build")
	.Does(() =>
{
	NuGetPack(project, nugetPackSettings);
});

Task("Push")
	.IsDependentOn("Package")
	.Does(() =>
{
	var symbolpkg = GetFiles(System.IO.Path.Combine(artifacts, "*.symbols.nupkg"));
	NuGetPush(symbolpkg, nugetPushSettings);
});

Task("Default").IsDependentOn("Build");

RunTarget(target);
