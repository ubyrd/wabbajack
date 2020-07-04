Wabbajack
===
  
Branch with auto mods downloading for non premium Nexus users.  

Also you can find here installation guide for non-steam versions of games:  
1. Install .NET Core SDK from: https://dotnet.microsoft.com/download  
2. Run:  
```bash
# you can also just download zip archive instead of using git:
# https://github.com/ubyrd/wabbajack/archive/2.1.0.0-auto-non-premium.zip
git clone https://github.com/ubyrd/wabbajack.git
cd wabbajack
git checkout 2.1.0.0-auto-non-premium

# build binaries
dotnet build
```

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;you can find binary files in  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`...\wabbajack\Wabbajack\bin\...`  

3. Download Steam from: https://store.steampowered.com/  
4. Run Steam at once. You can no login.  
5. (example for Skyrim SE) Create file at  
`C:\Program Files (x86)\Steam\steamapp\appmanifest_489830.acfs`  
with next content:  
```
"AppState"
{
	"appid" "489830"
	"Universe" "1"
	"name" "The Elder Scrolls V: Skyrim Special Edition"
	"StateFlags" "66"
	"installdir" "YOUR_SKYRIM_INSTALLATION_FULL_PATH"
	"LastUpdated" "0"
	"UpdateResult" "0"
	"SizeOnDisk" "0"
	"buildid" "0"
	"LastOwner" "0"
	"BytesToDownload" "0"
	"BytesDownloaded" "0"
	"AutoUpdateBehavior" "0"
	"AllowOtherDownloadsWhileRunning" "0"
	"UserConfig"
	{
	}
	"MountedDepots"
	{
	}
}
```
6. Now you can install modlist from one of official guides: https://www.wabbajack.org/modlists/gallery/  
