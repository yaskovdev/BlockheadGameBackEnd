# Blockhead Game Back End

```shell
curl -v -H "Content-Type: application/json" -d '{ "field": [ ".....", ".....", "БАЛДА", ".....", "....." ], "usedWords": [ "БАЛДА" ],"difficulty": 13 }' http://localhost:5122/api/move-requests
```

## Running With Docker in Linux Container

```shell
docker build -f BlockheadGameBackEnd/Dockerfile -t yaskovdev/block-head-game-back-end .
docker run -p 5122:80 -d yaskovdev/block-head-game-back-end
curl -v http://localhost:5122/api/field
```
