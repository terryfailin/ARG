(function (app) {

    app.Validate = {};

    jQuery.extend(app.Validate,
	{
	    BindDigiOnly: function () {
	        $('.mvcap-digionly').keyup(function () {
	            var e = this;
	            var val = $(this).val();
	            if (!/^\d+$/.test(val)) {
	                var newValue = /^\d+/.exec(e.value);
	                if (newValue != null) {
	                    e.value = newValue;
	                }
	                else {
	                    e.value = "";
	                }
	            }
	            return false;
	        });
	    }
	});
})(MvcAp);