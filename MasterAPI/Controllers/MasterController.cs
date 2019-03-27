using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using MasterAPI.Models;
using MasterAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Rest;
#pragma warning disable 1591

namespace MasterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterController : ControllerBase
    {
        private readonly KubernetesService kubernetesService;
        private readonly IConfiguration configuration;
        private readonly IKubernetes kubernetesClient;
        public MasterController(KubernetesService kubernetesService, 
            IConfiguration configuration,
            IKubernetes kubernetesClient)
        {
            this.kubernetesService = kubernetesService;
            this.configuration = configuration;
            this.kubernetesClient = kubernetesClient;
        }

        /// <summary>
        ///     Creates additional emulator pod with passed configurations
        /// </summary>
        /// <remarks></remarks>
        /// <response code="200">Emulator created</response>
        /// <response code="500">Oops! Something happened</response>
        [HttpPost("create")]
        public async Task<ActionResult> Post([FromBody] SetUpNewDeviceModel newDevice, [FromQuery] bool runOnDevEnvironnment)
        {
            var depl = kubernetesService.CreateDefaultV1Deployment(newDevice.deviceId.ToLower(),
                runOnDevEnvironnment,
                configuration[MasterConstants.EmulatorVersion]);
            var device = kubernetesService.CreateArgsFromDeviceModel(newDevice, runOnDevEnvironnment);

            var argsFromTemplate = (List<string>) depl.Spec.Template.Spec.Containers.FirstOrDefault()?.Args;
            argsFromTemplate?.AddRange(device);

            try
            {
                await kubernetesClient.CreateNamespacedDeploymentWithHttpMessagesAsync(depl, configuration[MasterConstants.Namespace]);
            }
            catch (HttpOperationException e)
            {
                Console.WriteLine(e);
                return new ContentResult()
                {
                    Content = $"An error occured : {e.Message}\n{e.Response.Content}\n\n" +
                              $"Probable reason : a deployment with the same name already exists on AKS.",
                    StatusCode = 400
                };
            }

            return new OkResult();
        }

       
    }
}