
//current window as a global
var winRef = '';

var LT_IsArchive = false;
var LT_LiveTimingCookieKey = "LTP_WINDOW_STATUS";

function LT_Window_sWidth(sCookie){
    return (LT_iJaveMode == "bigpopup" && LT_sMode != "inline" ? '1045' : '750'); //Window Width
}
function LT_Window_sHeight(sCookie){
	return (LT_iJaveMode == "bigpopup" && LT_sMode != "inline" ? '700' : ((LT_IsArchive == 0 ? false : true) ? '525' : '570'));  //Window Height
}
function LT_sWidth(sCookie){
	return (LT_iJaveMode == "bigpopup" && LT_sMode != "inline" ? '998' : '703'); //Java Width
}
function LT_sHeight(sCookie){
	return (LT_iJaveMode == "bigpopup" && LT_sMode != "inline" ? '565' : '434');  //Java Height 
}

function doResizeTo(sWidth, sHeight) {
	var x = sWidth;
	var y = sHeight;
	window.moveTo(0,0);
	window.resizeTo(x, y);

	var currentWin;
	if (winRef != ''){
		currentWin = winRef;	
	}else{
		currentWin = window;
	}
	tlp_movewindow(currentWin, x, y);
}

	
function tlp_movewindow(win, sWidth, sHeight){
	var winl = (screen.width-sWidth)/2;
	var wint = (screen.height-sHeight)/2;

	win.screenX = winl;
	win.screenY = wint;
}


function ltp_onload(sCookie){
	//resizing of window
	var ltp_cookie = GetCookie(LT_LiveTimingCookieKey);
	
	ltp_resize(ltp_cookie) //Resize Window
}

function ltp_onclick(sSize, bInScreen){
	SetCookie(LT_LiveTimingCookieKey, sSize);	
	
	if (bInScreen) {
		//resizing of window
		var ltp_cookie = GetCookie(LT_LiveTimingCookieKey);
		LT_iJaveMode = ltp_cookie;
		LT_iMode = "popup";
		ltp_resize(ltp_cookie) //Resize Window
	}
	
	//Do the redirection/refresh
	window.location.href = LT_LiveTimingPopupURL;
}

function ltp_resize(sCookie){
	var sWidth = LT_Window_sWidth(sCookie);
	var sHeight = parseInt(LT_Window_sHeight(sCookie)) + 57;

	doResizeTo(sWidth, sHeight);	
}

function IsLiveTimingValid(sRegURL){
    IsLiveTimingValid(sRegURL, false);
}

function IsLiveTimingValid(sRegURL, bOnload){
    var sCookie = GetCookie("USER");
    if (sCookie == null){
	    
	    if (bOnload){
	        window.location = sRegURL;
	    }else{
	        return false;
	    }
    }else{
        return true;
    }
}

function live_timing_action(MODE){
    live_timing_action(MODE, false);
}

function live_timing_action(MODE, bInScreen){   
    if (!IsLiveTimingValid()){
	    window.location = LT_LiveTimingRegURL;
    }else{
        window.location = LT_LiveTimingLaunchURL;
        
        open_live_timing_popup(LT_LiveTimingPopupURL, MODE);
    }
}

function open_live_timing_popup(url, MODE)
{
    var w = "";
    var h = "";
    var sCookie = MODE;
    
    SetCookie(LT_LiveTimingCookieKey, MODE);
    LT_sMode = "popup";
    LT_iJaveMode = MODE;
    
    
    w = LT_Window_sWidth(sCookie);
    h = LT_Window_sHeight(sCookie);
	    	
    //alert("mode: " + LT_sMode);
    //alert("mode j:" + LT_iJaveMode);   	    	
	    	
    var winl = (screen.width-w)/2;
    var wint = (screen.height-h)/2;		
    
    winRef = window.open(url, 'popup', 'top=0,left=0,width='+w+',height='+h+',history=no,resizable=no,status=no,scrollbars=yes,menubar=no');
    
    if (!winRef.opener) winRef.opener = self;	
    if(parseInt(navigator.appVersion) >= 4){winRef.focus();}
}

function ShowLinksForLiveTimingSize()
{
    var eReduce = document.getElementById("lkReduce");
    var eEnlarge = document.getElementById("lkEnlarge");
    var sCookie = GetCookie(LT_LiveTimingCookieKey);
    
    if (sCookie == "bigpopup")
    {
        if (eReduce != null) eReduce.style.display = "";
        if (eEnlarge != null) eEnlarge.style.display = "none";
    }
    else if (sCookie == "small") 
    {
        if (eReduce != null) eReduce.style.display = "none";
        if (eEnlarge != null) eEnlarge.style.display = "";
    }
    else
    {
        if (eReduce != null) eReduce.style.display = "none";
        if (eEnlarge != null) eEnlarge.style.display = "none";				                                
    }
}    