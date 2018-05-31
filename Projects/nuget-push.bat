..\utils\nuget setApiKey oy2b2tv3wreitkxe2ztasv5hgsmv7nlebeulwfs6aoikea  
FOR /F %i in (.\package-list.txt) do @..\utils\nuget push nupkgs\%i.*.nupkg -Source https://www.nuget.org/api/v2/package



