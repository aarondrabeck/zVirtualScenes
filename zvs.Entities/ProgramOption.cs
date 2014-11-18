using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Entity;

namespace zvs.DataModel
{
    [Table("ProgramOptions", Schema = "ZVS")]
    public class ProgramOption : INotifyPropertyChanged, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        private string _uniqueIdentifier;
        [StringLength(255)]
        public string UniqueIdentifier
        {
            get
            {
                return _uniqueIdentifier;
            }
            set
            {
                if (value != _uniqueIdentifier)
                {
                    _uniqueIdentifier = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _value;
        [StringLength(512)]
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value != _value)
                {
                    _value = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public static async Task<Result> TryAddOrEditAsync(ZvsContext context, ProgramOption programOption, CancellationToken cancellationToken)
        {
            if (programOption == null)
                throw new ArgumentNullException("programOption");

            var existingOption = await context.ProgramOptions.FirstOrDefaultAsync(o => o.UniqueIdentifier == programOption.UniqueIdentifier, cancellationToken);

            var changed = false;

            if (existingOption == null)
            {
                context.ProgramOptions.Add(programOption);
                changed = true;
            }
            else
            {
                if (existingOption.Value != programOption.Value)
                {
                    changed = true;
                    existingOption.Value = programOption.Value;
                }
            }

            if (changed)
                return await context.TrySaveChangesAsync(cancellationToken);

            return Result.ReportSuccess();
        }

    }
}
