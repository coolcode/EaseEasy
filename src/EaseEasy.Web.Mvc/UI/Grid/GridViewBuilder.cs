using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EaseEasy.Web.Mvc.UI {
    public class GridViewBuilder<T> : ViewComponentBuilder<GridView<T>, GridViewBuilder<T>>, IGridViewBuilder<T> {
        public GridViewBuilder(GridView<T> component, ViewContext context, IViewDataContainer container)
            : base(component, context, container) {
        }

        public IGridViewBuilder<T> Columns(Action<IGridColumnCollectionBuilder<T>> columnBuilder) {
            var builder = new GridColumnCollectionBuilder<T>(new GridColumnCollection<T>(), ViewContext, ViewDataContainer);
            columnBuilder(builder);
            builder.Component.ForEach(c => this.Component.Columns.Add(c));

            return this;
        }

        public IGridViewBuilder<T> Pager(
            bool pagable = true,
            int pageSize = 10,
            PagerModes mode = PagerModes.All,
            string pageFieldName = GridView.DefaultPageFieldName,
            string pageSizeFieldName = GridView.DefaultPageSizeFieldName,
            string firstText = "<<",
            string previousText = "<",
            string nextText = ">",
            string lastText = ">>",
            string summaryFormat = " 第 <b>{PageIndex}</b>/<b>{PageCount}</b> 页, 总数：{TotalRecords}") {
            Component.Pagable = pagable;
            Component.PageSize = pageSize;
            Component.PageMode = mode;
            Component.PageFieldName = pageFieldName;
            Component.PageSizeFieldName = pageSizeFieldName;
            Component.PageFirstText = firstText;
            Component.PagePreviousText = previousText;
            Component.PageNextText = nextText;
            Component.PageLastText = lastText;
            Component.PageSummaryFormat = summaryFormat;
            return this;
        }

        public IGridViewBuilder<T> Sort(
            bool sortable = true,
            string defaultSort = null,
            string sortFieldName = GridView.DefaultSortFieldName,
            string sortDirFieldName = GridView.DefaultSortDirFieldName) {
            Component.Sortable = sortable;
            Component.DefaultSort = defaultSort;
            Component.SortFieldName = sortFieldName;
            Component.SortDirFieldName = sortDirFieldName;
            return this;
        }

        public IGridViewBuilder<T> Bind(IEnumerable<T> dataSource) {
            Component.DataSource = dataSource;
            return this;
        }

        public IGridViewBuilder<T> Ajax(bool isAjax = true, string updateTargetId = "") {
            Component.IsAjax = isAjax;
            Component.UpdateTargetId = updateTargetId;
            return this;
        }
    }

}
