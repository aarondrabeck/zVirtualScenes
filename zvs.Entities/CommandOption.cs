using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.Entities
{
    [Table("CommandOptions", Schema = "ZVS")]
    public partial class CommandOption : BaseOption, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? CommandId { get; set; }
        public virtual Command Command { get; set; }
    }
}
