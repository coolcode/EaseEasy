using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using EaseEasy.Web.Mvc.Html;

namespace EaseEasy.Web.Mvc.UI {
    public class GridColumnBuilder<T> : ViewComponentBuilder<GridColumn<T>, GridColumnBuilder<T>>, IGridColumnBuilder<T> {
        public GridColumnBuilder(GridColumn<T> component, ViewContext context, IViewDataContainer container)
            : base(component, context, container) {

        }

        public IGridColumnBuilder<T> Header(string value) {
            Component.Header = value;
            return this;
        }

        public IGridColumnBuilder<T> Header(Func<GridColumn<T>, object> func) {
            Component.HeaderFunc = func;
            return this;
        }

        public IGridColumnBuilder<T> Sortable(bool value) {
            Component.Sortable = value;
            return this;
        }

        public IGridColumnBuilder<T> Format(string value) {
            Component.Format = value;
            return this;
        }

        public IGridColumnBuilder<T> Format(Func<object, object> func) {
            Component.TemplateFunc = c => func(Component.GetRawValue(c));
            return this;
        }

        public IGridColumnBuilder<T> Template(Func<T, object> func) {
            Component.TemplateFunc = c => func(c);
            return this;
        }

        public IGridColumnBuilder<T> Partial(string partialName) {
            return Template(item => Component.Html.Partial(partialName, item));
        }

        public IGridColumnBuilder<T> HtmlEncode(bool value) {
            Component.HtmlEncode = value;
            return this;
        }

        public IGridColumnBuilder<T> CheckBox() {
            return this
                .Header("<input name='items_all' class='check-all' type='checkbox'/>")
                .Width(20)
                .Format("<input name='items' class='check-item' type='checkbox' value='{0}'/>")
                .HtmlEncode(false)
                .Align(Alignment.Center);
        }

        public IGridColumnBuilder<T> Align(Alignment alignment) {
            Component.MergeAttribute("align", alignment.GetAttachedString(), true);
            return this;
        }

        public IGridColumnBuilder<T> Visible(bool value) {
            Component.Visible = value;
            return this;
        }

        public IGridColumnBuilder<T> CharsLength(int length) {
            return this.Format(c => Convert.ToString(c).Cut(length));
        }
    }

}
