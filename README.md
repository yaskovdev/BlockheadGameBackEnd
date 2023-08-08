# Blockhead Game Back End

```shell
curl -v -H "Content-Type: application/json" -d '{ "field": [ ".....", ".....", "БАЛДА", ".....", "....." ], "usedWords": [ "БАЛДА" ],"difficulty": "Medium" }' http://localhost:5122/api/move-requests
```

## Running With Docker in Linux Container

```shell
docker build -f BlockheadGameBackEnd/Dockerfile -t yaskovdev/block-head-game-back-end .
docker run -p 5122:8080 -d yaskovdev/block-head-game-back-end
curl -v http://localhost:5122/api/field
```

## Running In AKS Using NodePort

Note: in order for this to work, your AKS must be created with `--enable-node-public-ip` parameter.

```shell
docker image push yaskovdev/block-head-game-back-end
kubectl create deploy blockhead-game-back-end --image yaskovdev/block-head-game-back-end --replicas 3 --port 8080
kubectl expose deploy blockhead-game-back-end --type="NodePort" --port=5122 --target-port=8080 --name=blockhead-game-back-end
```

In Azure Portal, you have to go
to `MC_${RESOURCE_GROUP_NAME}_${AKS_NAME}_${AZURE_REGION}` -> `aks-agentpool-*-nsg` -> `Inbound security rules` -> `Add`
and specify `30000-32767` (Kubernetes NodePort range) as the `Destination port ranges`, then create the rule.

Then in `kubectl get no -o wide` take the value of the `EXTERNAL-IP` of any node.

Then in `kubectl get svc blockhead-game-back-end` take the NodePort value from `PORT(S)`.

Then combine this into a curl command similar to `curl -v http://13.95.141.220:32423/api/field`.

Then run the command from your local PC to see the output.

## Troubleshooting

Use `kubectl debug node/aks-nodepool1-20223646-vmss000000 -it --image=ubuntu` to connect to Kubernetes node. Get the
node name from `kubectl get no`.

Once in the debug container, run `apt-get update; apt-get install curl`.

Then you can use `curl` to access the app API.
