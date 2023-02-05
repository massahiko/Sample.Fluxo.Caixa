namespace Sample.Fluxo.Caixa.Core.Pageable
{
    public class ByPage
    {
        public int Page { get; set; }

        public int Size { get; set; }

        public int Skip => (Page - 1) * Size;

        public ByPage() : this(1, 10)
        {
        }

        public ByPage(int page, int amount)
        {
            Page = page;
            Size = amount;
        }
    }
}
