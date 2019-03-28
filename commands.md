- `minikube start --vm-driver hyperv --hyperv-virtual-switch "PVN"`
  
  Starting minikube on Windows HyperV with dedicated virtual switch.

- `minikube dashboard`
  
  Opening dashboard
- `minikube docker-env | Invoke-Expression`

  Initializing kubernetes environment for credentialless access to image
  
- `kubectl create -f deployment.yaml -n development`

  Creating resource from file.
  
- `kubectl get pods -n kube-system`
  
  List pods in `kube-system` namespace.

- `kubectl expose deployment web --target-port=8080 --type=NodePort`

  Creating service which exposes port 8080 as `NodePort` for deployment `web`
- `kubectl get service web`
  
  Get service `web`

- `minikube service web --url`
  
  Retrieving local url of service `web`