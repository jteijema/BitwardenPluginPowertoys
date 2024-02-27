# PowerShell script that downloads latest release from GitHub and installs it to PowerToys Run plugin folder
# Usage: .\installer.ps1

$installLocation = "%LocalAppData%\Microsoft\PowerToys\PowerToys Run\Plugins\BitwardenPlugin"

# Get latest release from GitHub
$latestRelease = Invoke-RestMethod -Uri "https://api.github.com/repos/jteijema/BitwardenPluginPowertoys/releases/latest"

# Get download URL for latest release
$downloadUrl = $latestRelease.assets.browser_download_url

# Download latest release
Invoke-WebRequest -Uri $downloadUrl -OutFile "BitwardenPlugin.tmp.zip"

# Unzip latest release
Expand-Archive -Path "BitwardenPlugin.tmp.zip" -DestinationPath "$installLocation.tmp"

# Move files to plugin folder
Move-Item "$installLocation.tmp\BitwardenPlugin" $installLocation

# Remove zip file
Remove-Item "BitwardenPlugin.tmp.zip"

# Remove temporary folder
Remove-Item "$installLocation.tmp" -Recurse