namespace Base.ViewObject
{
    public class SummaryData
    {
        public SummaryData() { }

        public SummaryData(int page, int listSize, int returnedListSize, int totalPages)
        {
            this.listSize = listSize;
            this.returnedListSize = returnedListSize;
            this.page = page;
            this.totalPages = totalPages;
        }

        public int listSize { get; set; }

        public int returnedListSize { get; set; }

        public int page { get; set; }

        public int totalPages { get; set; }
    }
}