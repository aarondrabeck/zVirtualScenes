namespace LightSwitchPlugin.LightSwitch
{
    interface ILightSwitchChannel
    {
        void OnIphone();
        void OnVersion();
        void OnServer();
        void OnTerminate();
        void OnAList();
        void OnSList();
        void OnList();
        void OnZList();
        void OnPassword(string password);
        void OnDevice(string deviceId, string level, string type);
        void OnScene(string sceneId);
        void OnZone(string zoneId, string level);
        void OnThermTemp(string deviceId, string mode, string temp, string type);
        void OnThermMode(string deviceId, string mode, string type);
    }
}
