
namespace zVirtualScenesAPI.Structs
{
    public class Command
    {
        public string Name;
        public string FriendlyName;
        public string CustomData1;
        public string CustomData2;
        public string HelpText;   
        public int CommandId;
        public cmdType cmdtype;
        public ParamType paramType;

        public override string ToString()
        {
            return FriendlyName;
        }
    }
}
