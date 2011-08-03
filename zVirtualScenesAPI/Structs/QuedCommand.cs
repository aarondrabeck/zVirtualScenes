namespace zVirtualScenesAPI.Structs
{
    public class QuedCommand
    {
        public int Id { get; set; }
        public int ObjectId { get; set; }
        public cmdType cmdtype { get; set; }
        public int CommandId { get; set; }
        public string Argument { get; set; }               
    }
}
