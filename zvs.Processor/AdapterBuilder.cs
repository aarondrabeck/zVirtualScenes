namespace zvs.Processor
{
    public abstract class AdapterBuilder
    {
        protected Core Core { get; set; }
        protected zvsAdapter Adapter { get; set; }

        public AdapterBuilder(zvsAdapter zvsAdapter, Core core)
        {
            Adapter = zvsAdapter;
            Core = core;
        }
    }
}
