nuget.exe pack ..\nuget-package\ArtZilla.Wpf.Common.nuspec -symbols
for %%f in (*.nupkg) do copy %%f C:\Web\Nuget\www\Packages