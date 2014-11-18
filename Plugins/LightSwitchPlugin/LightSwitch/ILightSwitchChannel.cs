namespace LightSwitchPlugin.LightSwitch
{
    interface ILightSwitchChannel
    {
        void onIphone();
        void onVersion();
        void onServer();
        void onTerminate();
        void onAList();
        void onSList();
        void onList();
        void onZList();
        void onPassword(string password);
        void onDevice(string deviceId, string level, string type);
        void onScene(string sceneId);
        void onZone(string zoneId, string level);
        void onThermTemp(string deviceId, string mode, string temp, string type);
        void onThermMode(string deviceId, string mode, string type);
    }
}
