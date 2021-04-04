using System.Collections.Generic;

namespace Base.ViewObject
{
    public class BaseResponseVO
    {
        public BaseResponseVO()
        {
            summary = new SummaryData();
        }
        public SummaryData summary { get; set; }
    }

    public class BaseResponseVO<T> : BaseResponseVO
    {
        public BaseResponseVO()
            : base()
        {
            dataList = new List<T>();
        }
        public List<T> dataList { get; set; }
    }
}