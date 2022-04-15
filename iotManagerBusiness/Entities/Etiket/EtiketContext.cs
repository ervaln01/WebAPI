namespace IotManagerBusiness.Entities.Etiket
{
	using System.Data.Entity;

	/// <summary>
	/// Контекст базы данных Etiket.
	/// </summary>
	public partial class EtiketContext : DbContext
	{
		/// <summary>
		/// Конструктор класса <see cref="EtiketContext"/>.
		/// </summary>
		public EtiketContext(string connectionString) : base(connectionString) { }

		/// <summary>
		/// Таблица T_ATR_VALUE_tr_TR.
		/// </summary>
		public virtual DbSet<TAtrValueTrTr> TAtrValueTrTrs { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder) => OnModelCreatingPartial(modelBuilder);

		partial void OnModelCreatingPartial(DbModelBuilder modelBuilder);
	}
}