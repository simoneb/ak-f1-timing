function GetCookie (name) {
    var arg = name + "=";
    var alen = arg.length;
    var clen = document.cookie.length;
    var i = 0;

    while (i < clen) {
        var j = i + alen;

        if (document.cookie.substring(i, j) == arg)
            return getCookieVal (j);

        i = document.cookie.indexOf(" ", i) + 1;

        if (i == 0) break;
    }

    return null;
}

function SetCookie (name, value) {

    var argv = SetCookie.arguments;
    var argc = SetCookie.arguments.length;
    var expires = (argc > 2) ? argv[2] : null;
    var path = (argc > 3) ? argv[3] : "/";
    var domain = (argc > 4) ? argv[4] : null;
    var secure = (argc > 5) ? argv[5] : false;

    var base = new Date(0)
    var expDate = new Date()
    var dateInSecs = expDate.getTime()
    var oneYrFromNow = (365 * 24 * 60 * 60 * 1000) + dateInSecs - (2 * base.getTime())
    expDate.setTime(oneYrFromNow)

    // alert(expDate.toGMTString());
    document.cookie = name + "=" + escape (value) +
        "; expires=" + expDate.toGMTString() +
//		((expires == null) ? "" : ("; expires=" + expires.toGMTString())) +
        ((path == null) ? "" : ("; path=" + path)) +
        ((domain == null) ? "" : ("; domain=" + domain)) +
        ((secure == true) ? "; secure" : "");

}

function AppendCookie (name, value) {
    var argv = AppendCookie.arguments;
    var argc = AppendCookie.arguments.length;
    var current = GetCookie(name);
    current = ((current == null) ? "" : current);
    var expires = (argc > 2) ? argv[2] : null;
    var path = (argc > 3) ? argv[3] : "/";;
    var domain = (argc > 4) ? argv[4] : null;
    var secure = (argc > 5) ? argv[5] : false;

    var base = new Date(0)
    var expDate = new Date()
    var dateInSecs = expDate.getTime()
    var oneYrFromNow = (365 * 24 * 60 * 60 * 1000) + dateInSecs - (2 * base.getTime())
    expDate.setTime(oneYrFromNow)

    if (current.indexOf(value) == -1) {
        document.cookie = name + "=" + ((current.length > 0) ? current + "," + escape(value) : escape(value))+
        "; expires=" + expDate.toGMTString() +
//            ((expires == null) ? "" : ("; expires=" + expires.toGMTString())) +
            ((path == null) ? "" : ("; path=" + path)) +
            ((domain == null) ? "" : ("; domain=" + domain)) +
            ((secure == true) ? "; secure" : "");
    }
}

function UnappendCookie (name, value) {
    var argv = UnappendCookie.arguments;
    var argc = UnappendCookie.arguments.length;
    var current = GetCookie(name);
    current = ((current == null) ? "" : current);
    var expires = (argc > 2) ? argv[2] : null;
    var path = (argc > 3) ? argv[3] : "/";;
    var domain = (argc > 4) ? argv[4] : null;
    var secure = (argc > 5) ? argv[5] : false;
    var reVal = new RegExp(value, "g");

    current = current.replace(reVal, "");
    reVal = new RegExp(",,");
    current = current.replace(reVal, ",");
    if (current.charAt(0) == ",")
        current = current.substring(1, current.length)
    if (current.charAt(current.length - 1) == ",")
        current = current.substring(0, current.length - 1)

    if (current.length <= 1)
        DeleteCookie(name);
    else
        document.cookie = name + "=" + current +
            ((expires == null) ? "" : ("; expires=" + expires.toGMTString())) +
            ((path == null) ? "" : ("; path=" + path)) +
            ((domain == null) ? "" : ("; domain=" + domain)) +
            ((secure == true) ? "; secure" : "");
}

function DeleteCookie (name) {
    var exp = new Date();

    exp.setTime (exp.getTime() - 1);

    var cval = GetCookie (name);

    document.cookie = name + "=" + cval + "; path=/; expires=" + exp.toGMTString();

}

function getCookieVal(offset) {
    var endstr = document.cookie.indexOf (";", offset);

    if (endstr == -1)
        endstr = document.cookie.length;

    return unescape(document.cookie.substring(offset, endstr));
}

// Will return the value of the desired item
// Just use the cookie name and the name of the parameter you want the value of

function findValue(cookieName,value)
{
	// Get the cookie
	var mycookie = GetCookie(cookieName);

	if(mycookie==null) return "";
	//alert("Cookie: "+mycookie);

	// Find the start of the variable
	var endstr = mycookie.indexOf(value);

	// Find the length of the variable
	var nextstr = value.length;

	//This should give us the start of the value, plus one beyond the equals sign
	var combstr = endstr + nextstr+1;

	// gives the value start to the end of the string
	var subValue = mycookie.substr(combstr);

	if(subValue.indexOf(',')==0)
	{
		return "";
	}

	// Finds the comma at the end of the substring, if there is one.
	var nextSub = subValue.indexOf(',');

	var valueString;

	//alert("Cookie: "+mycookie+"\n Index of item "+value+" : "+endstr+"\n Pos to find value : "+subValue+"\n Next string : "+nextSub);

	if(nextSub > 0)
	{
		valueString=subValue.substr(0,nextSub);
	}

	else
	{
		valueString =subValue;
	}

	return valueString;
}

