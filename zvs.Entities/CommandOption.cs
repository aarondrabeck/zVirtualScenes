using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.DataModel
{
    [Table("CommandOptions", Schema = "ZVS")]
    public class CommandOption : BaseOption, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? CommandId { get; set; }
        public virtual Command Command { get; set; }
    }
}
