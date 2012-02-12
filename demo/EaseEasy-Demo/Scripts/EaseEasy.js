/// <reference path="../jquery/jquery-vsdoc.js" />

/*common*/
$(function () {
	$.ajaxSetup({
		cache: false
	});

	$("#loading").ajaxStart(function () {
		$(this).show('slow');
	}).ajaxComplete(function () {
		$(this).hide('slow');
	}).ajaxStop(function () {
		$(this).hide('slow');
	}).slideUp('slow');
});

(function ($) {
	$.fn.format = function (source, params) {
		if (arguments.length == 1)
			return function () {
				var args = $.makeArray(arguments);
				args.unshift(source);
				return _ajax.format.apply(this, args);
			};
		if (arguments.length > 2 && params.constructor != Array) {
			params = $.makeArray(arguments).slice(1);
		}
		if (params.constructor != Array) {
			params = [params];
		}
		$.each(params, function (i, n) {
			source = source.replace(new RegExp("\\{" + i + "\\}", "g"), n);
		});
		return source;
	};
})(jQuery);

/*grid*/
(function ($) {
	$.fn.reload = function (param) {
		var _grid = this;
		$.post(this.attr("url"), param, function (data) {
			try {
				var r = $.parseJSON(data);
				if (r.success == false) {
					alert(r.message);
					return;
				}
			}
			catch (e) {
			}
			_grid.html(data);
		});
	};

	$.fn.refresh = function () {
		this.find("div.pReload").parent().click();
	};

	$.fn.getSelectItem = function () {
		var selectrow = this.find("table tbody tr.selectrow td input");
		var selectitem = {};
		selectrow.each(function (i) {
			var item = $(this);
			selectitem[item.attr("class")] = item.val();
		});
		return selectitem;
	};

	//sorting or paging
	$("div.bbit-grid a.ajax-headerLink,div.bbit-grid a.ajax-pagerLink").live("click", function () {//.grid-header
		var _t = $(this);
		$.post(_t.attr("href"), $('form').serialize(), function (data) {
			try {
				var r = $.parseJSON(data);
				if (r.success == false) {
					alert(r.message);
					return;
				}
			}
			catch (e) {
			}
			var grid = _t.parent().parent().parent().parent().parent().parent();
			if (_t.attr("class") == "ajax-pagerLink") {
				grid = grid.parent();
			}
			grid.html(data);
		});
		return false;
	});

	// Select row when clicked.
	$("div.bbit-grid table tbody tr").live("click", function () {
		var _t = this;
		$("div.bbit-grid table tbody tr").filter(function (i) {
			return this != _t;
		})
		.removeClass("selectrow");

		$(this).toggleClass("selectrow");
	});

	// Check all checkboxes when the one in a table head is checked.
	$("div.bbit-grid table input[type='checkbox'].check-all").live("click",
		function () {
			$(this).parent().parent().parent().parent().find("input[type='checkbox'].check-item").attr('checked', $(this).is(':checked'));
		}
	);

	$("div.bbit-grid table input[type='checkbox'].check-item").live("click",
		function () {
			if (!$(this).is(':checked')) {
				$(this).parent().parent().parent().parent().find("input[type='checkbox'].check-all").attr('checked', false);
			}
			else {
				var len = $("div.bbit-grid table input[type='checkbox'][checked!='true'].check-item").length;
				if (len == 0) {
					$(this).parent().parent().parent().parent().find("input[type='checkbox'].check-all").attr('checked', true);
				}
			}
		}
	);

})(jQuery);

