Write-Host 'Killing API processes...'
taskkill /F /IM WomenEmpower.API.exe 2>null
taskkill /F /IM dotnet.exe 2>null
Start-Sleep -Seconds 2
Write-Host 'Done! Now run dotnet run'