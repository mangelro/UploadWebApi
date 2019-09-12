using System;
using System.Collections.Generic;
using System.Linq;

namespace UploadWebApi.Models
{
    public class PaginatedList<T>
    {

        public int PageNumber { get; }

        public int PageSize { get; }

        public int TotalCount { get; }

        public int TotalPageCount { get; }

        bool HasPreviousPage => (PageNumber > 1);

        bool HasNextPage => (PageNumber < TotalPageCount);

        string BaseUrl { get; }

        public string NextPage => HasNextPage ? $"{BaseUrl}?pageNumber={(PageNumber + 1).ToString()}&pageSize={PageSize.ToString()}&orden=desc" : "#";

        public string PreviousPage => HasPreviousPage ? $"{BaseUrl}?pageNumber={(PageNumber - 1).ToString()}&pageSize={PageSize.ToString()}&orden=desc" : "#";

        public IEnumerable<T> Rows { get; private set; }

        public PaginatedList(string baseUrl,IEnumerable<T> rows, int pageNumber, int pageSize, int totalCount)
        {
            BaseUrl = baseUrl;
            Rows = rows ?? throw new ArgumentNullException(nameof(rows));
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPageCount = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        public static PaginatedList<T> Empty() =>  new PaginatedList<T>("",Enumerable.Empty<T>(),1,1,0);

    }




    



    //public class PaginatedList<T> : List<T>
    //{

    //    public int PageIndex { get; private set; }

    //    public int PageSize { get; private set; }

    //    public int TotalCount { get; private set; }

    //    public int TotalPageCount { get; private set; }

    //    public bool HasPreviousPage   =>  (PageIndex > 1);

    //    public bool HasNextPage => (PageIndex < TotalPageCount);

    //    public PaginatedList(IEnumerable<T> source, int pageIndex, int pageSize, int totalCount)
    //    {

    //        if(source == null)
    //        {
    //            throw new ArgumentNullException("source");
    //        }

    //        AddRange(source);
    //        PageIndex = pageIndex;
    //        PageSize = pageSize;
    //        TotalCount = totalCount;
    //        TotalPageCount = (int)Math.Ceiling(totalCount / (double)pageSize);
    //    }
    //}




    //public class PaginatedItemsList<TEntity> where TEntity : class
    //{
    //    public int PageNumber { get; private set; }

    //    public int PageSize { get; private set; }

    //    public long Count { get; private set; }

    //    public IEnumerable<TEntity> Data { get; private set; }

    //    public PaginatedItemsList(int pageNumber, int pageSize, long count, IEnumerable<TEntity> data)
    //    {
    //        this.PageNumber = pageNumber;
    //        this.PageSize = pageSize;
    //        this.Count = count;
    //        this.Data = data;
    //    }
    //}
}
