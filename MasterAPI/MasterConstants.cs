#pragma warning disable 1591
namespace MasterAPI
{
    public static class MasterConstants
    {
        public const string DevIotHubConnStr = "HostName=dev-harmony-iothub.azure-devices.net;SharedAccessKeyName=DeviceEmulatorService;SharedAccessKey=7IucPWS5jyAv+6wQP3wXv2XFGh5DBstbhvp/Q76U8qM=";
        public const string QaIOtHubConnStr = "HostName=qa-harmony-iot-hub.azure-devices.net;SharedAccessKeyName=master;SharedAccessKey=4WEtqVrmHql/zT2/cOHcv9fhOmGG4ShTnn9DbTwhGeo=";
        public const string IotHubConnectionStringEnvVariable = "IOTHUB_CONNECTION_STRING";
        public const string ConfigFilePath = "CONFIG_FILE_PATH";
        public const string Namespace = "NAMESPACE";
        public const string EmulatorVersion = "EMULATOR_VERSION";
    }
}
