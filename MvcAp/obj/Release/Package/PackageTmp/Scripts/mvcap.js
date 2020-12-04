(function (window) {
    var MvcAp = window.MvcAp =
	{
	    AppName: 'MvcAp',
	    ActionUrls: {},
	    ErrorMsgs: {}
	};
    //===========================================================================================
    jQuery(function($) {
        //The function of TAG options
        var tag_input = $(this).find('input[name^=TagField]');
        try {
            tag_input.each(function() {
                $(this).tag({
                    placeholder: tag_input.attr('placeholder'),
                });
                var $tag_obj = $(this).data('tag');
                var tagInput = $(this).parent().siblings('input[name^=TagOpts]');
                if (tagInput.length > 0) {
                    var tagContents = tagInput.val().split('^');
                    tagContents.pop();
                    $.each(tagContents,
                        function(key, value) {
                            $tag_obj.add(value);
                        });
                }
            });

            //programmatically add a new
        } catch (e) {
            tag_input.after('<textarea id="' +
                tag_input.attr('id') +
                '" name="' +
                tag_input.attr('name') +
                '" rows="3">' +
                tag_input.val() +
                '</textarea>').remove();
        }

    });
    jQuery.extend(MvcAp,
	{
	    ShowActivity: function (interval) {
	        //alert('ajaxStart');
	    },

	    HideActivity: function () {
	        //alert('ajaxEnd');
	    },

	    Download: function (url) {
	        var $iframe = $("<iframe style='display:none;' id='MvcAp-" + new Date().getTime() + "' src='" + url + "' width='0' height='0'></iframe>");
	        $("body").append($iframe);
	    },

	    InitList: function (tableName) {
	        $('body').on('click', '#pager a', function (event) {
	            var hrefVal = $(this).attr('href');
	            var page = hrefVal.split("=")[1];
	            if (hrefVal != '#') {
	                MvcAp.LoadList(hrefVal, page - 1);
	            }
	            event.preventDefault();
	            return false;
	        });

	        $('.search-text').keydown(function (event) {
	            if (event.keyCode == 13) {
	                $('.search-btn').trigger("click");
	            }
	        });

	        $('.search-btn').click(function () {
	            $('.LIST-SOURCE-SEARCHTYPE').val($('.search-type option:selected').val());
	            $('.LIST-SOURCE-SEARCHTEXT').val($('.search-text').val());
	            var hrefVal = $('.LIST-SOURCE-URL').val();
	            MvcAp.LoadList(hrefVal + "?page=1", 0);
	        });

	        if ($('.no-data')) {
	            $('.no-data').attr('colspan', $('.LIST-SOURCE th').length);
	        }

	        var active_class = 'active';
	        if (!tableName) {
	            tableName = 'main-table';
	        }
	        $('#' + tableName + ' > thead > tr > th input[type=checkbox]').eq(0).on('click', function () {
	            var th_checked = this.checked;//checkbox inside "TH" table header

	            $(this).closest('table').find('tbody > tr').each(function () {
	                var row = this;
	                if (th_checked) $(row).addClass(active_class).find('input[type=checkbox]').eq(0).prop('checked', true);
	                else $(row).removeClass(active_class).find('input[type=checkbox]').eq(0).prop('checked', false);
	            });
	        });

	        //select/deselect a row when the checkbox is checked/unchecked
	        $('#' + tableName).on('click', 'td input[type=checkbox]', function () {
	            var $row = $(this).closest('tr');
	            if (this.checked) $row.addClass(active_class);
	            else $row.removeClass(active_class);
	        });

	        $('.LIST-SOURCE').on('click', '.sorting, .sorting_asc, .sorting_desc', function (event) {
	            var sortColumn = $(this).attr('sort-column');
	            var style = $(this).hasClass('sorting_desc') ? "sorting_desc" : $(this).hasClass('sorting_asc') ? "sorting_asc" : "sorting";
	            var sortBy = '';

	            switch (style) {
	                case "sorting_asc":
	                    sortBy = "DESC";
	                    break;
	                case "sorting_desc":
	                default:
	                    sortBy = "ASC";
	                    break;
	            }

	            $('input[class=LIST-SOURCE-ORDERBY]').val(sortColumn);
	            $('input[class=LIST-SOURCE-ORDERDIR]').val(sortBy);
	            var page = $('input[class=LIST-SOURCE-PAGEINDEX]').val();
	            MvcAp.LoadList('', page);
	        });
	    },

	    LoadList: function (hrefVal, pageindex) {
	        if (!hrefVal) {
	            hrefVal = parent.$('.LIST-SOURCE-URL').val();
	        }
	        if (!pageindex) {
	            pageindex = 0;
	        }

	        $('input[class=LIST-SOURCE-PAGEINDEX]').val(pageindex);

	        var requestdata = '';
	        $('select[class^=LIST-SOURCE-]').each(function () {
	            var classname = $(this).attr('class').split(' ')[0];
	            requestdata += '&' + classname + '=' + encodeURIComponent($(this).val());
	        });
	        $('input[class^=LIST-SOURCE-]').each(function () {
	            var classname = $(this).attr('class').split(' ')[0];
	            requestdata += '&' + classname + '=' + encodeURIComponent($(this).val());
	        });
	        if (hrefVal.indexOf("?") == -1) {
	            hrefVal += "?ver=1";
	        }
	        $.ajax
            ({
                type: "GET",
                cache: false,
                url: hrefVal + requestdata,
                success: function (data) {
                    $('.LIST-SOURCE').html(data);
                    if ($('.no-data')) {
                        $('.no-data').attr('colspan', $('.LIST-SOURCE th').length);
                    }
                }
            });
	    },

	    ModalCancel: function (obj) {
	        parent.$('.modal').modal('hide');
	        //$(obj).parents('html').html('<div class="loading">Loading..Please wait</div>');
	    }
	});

    jQuery(document).ready(function () {
        jQuery("body")
        		.ajaxStart(MvcAp.ShowActivity)
        		.ajaxStop(MvcAp.HideActivity)
        		.ajaxError(function (event, xhr, options, thrownError) { if (!typeof options.error === 'function') { $.colorbox({ html: xhr.responseText }); } });

        //var isHome = true;
        //$('.lv2').each(function () {
        //    var thisUrl = $(this).find('a').attr('href');
        //    if (thisUrl == location.pathname && thisUrl.indexOf('#') == -1) {
        //        isHome = false;
        //        $(this).addClass('active');
        //        $(this).parents('.lv1').addClass('active');

        //        var lv2ItemName = $(this).attr('item-name');
        //        var currentMapPath = "";
        //        currentMapPath += "<ul class='breadcrumb'> ";
        //        currentMapPath += "<li> ";
        //        currentMapPath += "<i class='ace-icon fa fa-home home-icon'></i> ";
        //        currentMapPath += "<a href='/'> Home </a> ";
        //        currentMapPath += "</li> ";
        //        currentMapPath += "<li class='active'>" + lv2ItemName + " </li> ";
        //        currentMapPath += "</ul> ";

        //        $("#breadcrumbs").html(currentMapPath)
        //    }
        //});
        //if (isHome) {
        //    $('.home').addClass('active');
        //}

        $('body').on('click', '#modal-btn-cancel', function (event) {
            MvcAp.ModalCancel($(this));
        });
    });
})(window);