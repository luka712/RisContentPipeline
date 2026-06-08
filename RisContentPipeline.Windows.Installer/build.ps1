param(
	[string]$Version
)

if (-not $Version) {
	$now = Get-Date
	$buildNumber = ($now.Day * 1000) + ($now.Hour * 60) + $now.Minute
	$Version = "{0}.{1}.{2}" -f ($now.Year - 2000), $now.Month, $buildNumber
}

$publishDir = "..\RisContentPipeline.GUI.Windows\bin\Release\net10.0-windows\win-x64\publish"
if (-not (Test-Path $publishDir)) {
	throw "Publish directory not found: $publishDir"
}

wix build Product.wxs Files.wxs -d ProductVersion=$Version -o bin/RisContentPipeline.msi -acceptEula wix7