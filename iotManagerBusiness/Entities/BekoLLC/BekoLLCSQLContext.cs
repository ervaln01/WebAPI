namespace IotManagerBusiness.Entities.BekoLLC
{
	using System.Data.Entity;

	public partial class BekoLLCSQLContext : DbContext
	{
		public BekoLLCSQLContext(string connectionString) : base(connectionString) { }

		public virtual DbSet<KkiMatch> KkiMatches { get; set; }
		public virtual DbSet<TKktsConnectedDisplayCard> TKktsConnectedDisplayCards { get; set; }
		public virtual DbSet<TKktsIotLog> TKktsIotLogs { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<TKktsConnectedDisplayCard>().HasKey(e => new { e.Product, e.Serial, e.RegisterState });
			OnModelCreatingPartial(modelBuilder);
		}

		partial void OnModelCreatingPartial(DbModelBuilder modelBuilder);
	}
}