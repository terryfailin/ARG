(function (window) {
    var SwitchDatePicker = window.SwitchDatePicker =
	{

	};
    jQuery.extend(SwitchDatePicker,
	{
	    initialall: function (callback) {
	        $.each($('.switchdatepicker'), function (index, value) {
	            SwitchDatePicker.initial(this, callback);
	        });
	    },

	    initial: function (obj, callback) {

	        SwitchDatePicker.refresh(obj, callback);

	        $(obj).on("change", "select[name$=type]", function () {
	            var parent = $(this).parents('.switchdatepicker');
	            var t = $(obj).find('select[name$=type]').val();
	            var v = $(obj).find('input[name$=value]').val();
	            parent.attr('type', t);
	            parent.attr('value', v);

	            SwitchDatePicker.refresh(parent, callback);
	        });
            
	        $(obj).on("change", "select[name$=day], select[name$=year], select[name$=month]", function () {
	            SwitchDatePicker.refreshDate(this, callback);
	        });
	    },

	    refresh: function (obj, callback) {
	        var value = $(obj).attr('value');
	        var type = $(obj).attr('type');
	        $.ajax
            ({
                type: "GET",
                cache: false,
                url: "/SwithDatePicker/Initial",
                data: { name: $(obj).attr('id'), type: type, datetime: value },
                success: function (data) {
                    $(obj).html(data);
                    var t = $(obj).find('select[name$=type]').val();
                    var v = $(obj).find('input[name$=value]').val();
                    $(obj).attr('type', t);
                    $(obj).attr('value', v);
                    if (callback) {
                        callback(obj);
                    }
                }
            });
	    },

	    refreshDate: function (obj, callback) {
	        var parent = $(obj).parents('.switchdatepicker');
	        var y = parent.find('select[name$=year]').val();
	        var m = parent.find('select[name$=month]').val();
	        var d = parent.find('select[name$=day]').val();
	        var type = parent.attr('type');

	        $.ajax
            ({
                type: "GET",
                cache: false,
                url: "/SwithDatePicker/RefreshDate",
                data: { type: type, y: y, m: m, d: d },
                success: function (data) {
                    parent.attr('value', data);
                    parent.find('input[name$=value]').val(data);

                    SwitchDatePicker.refresh(parent, callback);
                }
            });
	    }
	});
})(window);