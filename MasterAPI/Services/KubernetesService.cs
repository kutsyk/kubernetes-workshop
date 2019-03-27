using System.Collections.Generic;
using k8s.Models;
using MasterAPI.Models;
using Newtonsoft.Json;

namespace MasterAPI.Services
{
    /// <summary>
    /// Service for working with Kubernetes API
    /// </summary>
    public class KubernetesService
    {
        private Dictionary<string, string> GetLabels(string deploymentName) => new Dictionary<string, string> { { "app", deploymentName } };

        /// <summary>
        /// Creates new deployment in current running Kubernetes env
        /// </summary>
        /// <param name="newDeviceId"></param>
        /// <param name="runOnDev"></param>
        /// <returns></returns>
        public V1Deployment CreateDefaultV1Deployment(string newDeviceId, bool runOnDev, string deviceEmulatorVersion)
        {
            var deploymentName = runOnDev ? $"dev-{newDeviceId}" : $"qa-{newDeviceId}";

            var deployment = new V1Deployment
            {
                Metadata = new V1ObjectMeta
                {
                    Name = deploymentName,
                    Labels = GetLabels(deploymentName)
                },
                Spec = new V1DeploymentSpec
                {
                    Replicas = 1,
                    Selector = new V1LabelSelector
                    {
                        MatchLabels = GetLabels(deploymentName)
                    },
                    Template = new V1PodTemplateSpec
                    {
                        Metadata = new V1ObjectMeta
                        {
                            Name = deploymentName,
                            Labels = GetLabels(deploymentName)
                        },
                        Spec = new V1PodSpec
                        {
                            Containers = new List<V1Container>
                            {
                                new V1Container
                                {
                                    Name = deploymentName,
                                    Image = $"devharmonycontainerreg.azurecr.io/deviceemulator:{deviceEmulatorVersion}",
                                    ImagePullPolicy = "IfNotPresent",
                                    Lifecycle = new V1Lifecycle
                                    {
                                        PreStop = new V1Handler
                                        {
                                            Exec = new V1ExecAction
                                            {
                                                Command = new List<string> {"sh", "clean.sh"}
                                            }
                                        }
                                    },
                                    Env = new List<V1EnvVar>
                                    {
                                        new V1EnvVar
                                        {
                                            Name = "isDeploying",
                                            Value = "true"
                                        },
                                        new V1EnvVar
                                        {
                                            Name = MasterConstants.IotHubConnectionStringEnvVariable,
                                            Value = runOnDev
                                                ? MasterConstants.DevIotHubConnStr
                                                : MasterConstants.QaIOtHubConnStr
                                        },
                                        new V1EnvVar
                                        {
                                            Name = "DLL",
                                            Value = "/app/emulator/DeviceEmulator.dll"
                                        }
                                    },
                                    Command = new List<string>
                                    {
                                        "dotnet"
                                    },
                                    Args = new List<string>
                                    {
                                        "$(DLL)"
                                    }
                                }
                            }
                        }
                    }
                }
            };
            var test = JsonConvert.SerializeObject(deployment);
            return deployment;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newDevice"></param>
        /// <param name="runOnDevEnv"></param>
        /// <returns></returns>
        public List<string> CreateArgsFromDeviceModel(SetUpNewDeviceModel newDevice, bool runOnDevEnv)
        {
            var argsResult = new List<string>();

            argsResult.AddRange(new[]
            {
                "-i", runOnDevEnv
                    ? MasterConstants.DevIotHubConnStr
                    : MasterConstants.QaIOtHubConnStr
            });
            argsResult.AddRange(new[] { "-d", newDevice?.deviceId ?? "" });
            argsResult.AddRange(new[] { "--ue", newDevice?.UE ?? "" });
            argsResult.AddRange(new[] { "--ui", newDevice?.UI ?? "" });
            argsResult.AddRange(new[] { "--chth", newDevice?.CHTH ?? "" });

            return argsResult;
        }
    }
}
