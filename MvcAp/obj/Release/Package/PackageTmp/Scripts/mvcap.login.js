(function (app) {

    app.Login = {};

    jQuery.extend(app.Login,
	{
	    Initialize: function (actionUrls, errorMsgs) {
	        jQuery.extend(MvcAp.ActionUrls, actionUrls);
	        jQuery.extend(MvcAp.ErrorMsgs, errorMsgs);
	        $('#btn-login').click(function () {
	            var account = $('input[name=account]').val();
	            var password = $('input[name=password]').val();
	            if (!account || !password) {
	                alert(MvcAp.ErrorMsgs.Login);
	                return false;
	            }
	            $('#main-form').submit();
	            $(this).attr("disabled", true);
	        });

	        $('input[name=account], input[name=password]').keypress(function (e) {
	            if (e.which == '13') {
	                $('#btn-login').click();
	            }
	        });
	    }
	});
})(MvcAp);