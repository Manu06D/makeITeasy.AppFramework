namespace makeITeasy.CarCatalog.dotnet8.Models
{
    public partial class CompositeKeyTable
    {
        public object DatabaseID => new object[] { Id1, Id2 };
    }
}
