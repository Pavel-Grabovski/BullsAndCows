$path = Get-Location
$parentFolder = Split-Path -Path $path -Parent

docker rm -f bullsandcowsconsoleappcontainer

docker build -f "./Dockerfile" --force-rm -t bullsandcowsconsoleapp  --build-arg "BUILD_CONFIGURATION=Release" $parentFolder
docker run -d --name bullsandcowsconsoleappcontainer bullsandcowsconsoleapp