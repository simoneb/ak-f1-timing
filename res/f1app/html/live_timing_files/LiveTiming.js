function  printProps(obj, objName) {
  var output = "" ;
  for (var prop in obj) {
    output += objName + "." + prop + " = " + obj[prop] + "\n" ;
  }
  return output ;
}

function hideHelp(){
    var sWidth = LT_sWidth( GetCookie(LT_LiveTimingCookieKey) );    
    $$(".helpContainer").addClass("hide");
	$("f1app").width= sWidth;
	$("f1app").style.width= sWidth;	
}
		
function showHelp(){
    var sWidth = LT_sWidth( GetCookie(LT_LiveTimingCookieKey) );
    
    if (LT_iJaveMode == null) {
        LT_iJaveMode = "small";
    }

	obj=($(this.innerHTML.toLowerCase())||$("error"))
	$$(".liveTimingMenu li a").removeClass("active");
	this.addClass("active");
	
	if (obj==null){
    	hideHelp();
		return;
	}
    
    $("sliderArea").style.display="";    
        
	if(obj.hasClass("hide")){
			$$(".helpContent").addClass("hide");
			$$(".helpContainer").removeClass("hide");
			
			//App Width
		    $("f1app").width = (LT_iJaveMode == "small" || LT_sMode == "inline" ? 450 : 643);
		    $("f1app").style.width = (LT_iJaveMode == "small" || LT_sMode == "inline" ? 450 : 643);	
			
			//Help Content Width
			$$(".helpInner").setStyle("width", (LT_iJaveMode == "small" || LT_sMode == "inline" ? 256 : 347));
			$$(".helpContent").setStyle("width", (LT_iJaveMode == "small" || LT_sMode == "inline" ? 228 : 318));
			
			//Help Content Height
			$$(".helpContent").setStyle("height", (LT_iJaveMode == "small" || LT_sMode == "inline" ? 420 : 551));
			
			if(!obj.mySlide){
			    setupSlider(obj, null, (LT_iJaveMode == "small" || LT_sMode == "inline" ? 432 : 564))
    	
	            /* mousewheel scroll */
	            obj.addEvents({
		            'wheelup': function(e) {
			            e = new Event(e).stop();
		            obj.mySlide.step=obj.mySlide.step-scrollSpeed;
		            obj.mySlide.set(obj.mySlide.step);
		            },
            	 
		            'wheeldown': function(e) {
			            e = new Event(e).stop();
		            obj.mySlide.step=obj.mySlide.step+scrollSpeed;
		            obj.mySlide.set(obj.mySlide.step);
		            }
	            });	
	        }			
        	
	        obj.toggleClass("hide");
	        obj.scrollTo(0,0);
	        obj.mySlide.step=0;
	        obj.mySlide.set(obj.mySlide.step);				
	}else{				
			$$(".helpContainer").addClass("hide");
			$$(".liveTimingMenu li a").removeClass("active");

			$$("firstItem").addClass("active");
			
			$("f1app").width=sWidth;
			$("f1app").style.width=sWidth;
	}
	
	//Remove scroller if needed
	if(obj.scrollHeight<=divHeight){
			$("sliderArea").style.display="none";
			//$$(".helpContent").setStyle("width", (LT_sMode == "inline" || LT_iJaveMode == "small" ? 242 : 332));
	}else{
			$("sliderArea").removeClass("hide");
			obj.removeClass("wider");	
	}
	return false;
}

function initTimingsHelpNav(){
	helpButtons = $$(".liveTimingMenu li a");
	for(x=0;x<helpButtons.length;x++){
		helpButtons[x].onclick=showHelp;	
	}	
}

function writeApplet(){
		var sCookie = GetCookie(LT_LiveTimingCookieKey);
		var sWidth = LT_sWidth(sCookie);
		var sHeight = LT_sHeight(sCookie);
	    var sJavaSize;
	    
	    if (LT_iJaveMode == null) LT_iJaveMode = "inline";
	    
	    if (LT_sMode=="popup"){
	        sJavaSize = (LT_iJaveMode != "bigpopup" ? "inline" : LT_iJaveMode);
	    }else{
	        sJavaSize = "inline";
	    }
	    
		sContent="";
		iContent="<iframe width='"+sWidth+"' height='"+sHeight+"' frameborder='0'  vspace='0'  hspace='0'  marginwidth='0' marginheight='0' scrolling='no' id='f1app' style='border:0px;padding:0px;z-index:-200;width:"+sWidth+";height:"+sHeight+";'>";
		sContent+="<object\n";
		sContent+="classid='clsid:8AD9C840-044E-11D1-B3E9-00805F499D93'\n";
		sContent+="codebase='http://java.sun.com/products/plugin/autodl/jinstall-1_4-windows-i586.cab#Version=1,4,0,0'\n";
		sContent+="WIDTH='"+sWidth+"' HEIGHT='"+sHeight+"' NAME='f1app' VIEWASTEXT>\n";
		sContent+="<PARAM NAME='CODE' VALUE='uk/co/aspectgroup/f1app/f1app.class'>\n";
		sContent+="<PARAM NAME='CODEBASE' VALUE='http://live-timing.formula1.com/java" + LT_iVersion + "/'>\n";
		sContent+="<PARAM NAME='ARCHIVE' VALUE='f1app.jar'>\n";
		sContent+="<PARAM NAME='NAME' VALUE='f1app'>\n";
		sContent+="<PARAM NAME='season' VALUE='" + LT_iSeasonYear + "'>\n";
		sContent+="<PARAM NAME='type' VALUE='application/x-java-applet;version=1.4'>\n";
		sContent+="<PARAM NAME='scriptable' VALUE='false'>\n";
		sContent+="<PARAM NAME='keyframe' VALUE='" + LT_KeyFrame + "'>\n";
		sContent+="<PARAM NAME='textcolour' VALUE='1a1a1a'>\n";
		sContent+="<PARAM NAME='bgcolour' VALUE='dfe2e3'>\n";
		sContent+="<PARAM NAME='streaming' VALUE='" + (LT_IsArchive == 1 ? 0 : 1) + "'>\n";
		sContent+="<param value='transparent' name='wmode' />\n";
		sContent+="<PARAM NAME='language' VALUE='1'>\n";
		sContent+="<PARAM NAME='size' VALUE='" + sJavaSize + "'>\n";
		sContent+="<PARAM NAME='circuit' VALUE='/circuit/" + LT_CircuitName + ".gif'>\n";
		sContent+="<PARAM NAME='user' value='" + LT_UserCookie + "'>\n";
		sContent+="<comment>\n";
		sContent+="<embed\n";
		sContent+="type='application/x-java-applet;version=1.3'\n";
		sContent+="CODE='uk/co/aspectgroup/f1app/f1app.class'\n";
		sContent+="JAVA_CODEBASE='http://live-timing.formula1.com/java" + LT_iVersion + "/'\n";
		sContent+="ARCHIVE='f1app.jar'\n";
		sContent+="NAME='f1app'\n";
		sContent+="season='" + LT_iSeasonYear + "'\n";
		sContent+="wmmode='transparent'\n";
		sContent+="WIDTH='"+sWidth+"'\n";
		sContent+="HEIGHT='"+sHeight+"'\n";
		sContent+="scriptable='false'\n";
		sContent+="keyframe='" + LT_KeyFrame + "'\n";
		sContent+="textcolour='1a1a1a'\n";
		sContent+="bgcolour='dfe2e3'\n";
		sContent+="streaming='" + (LT_IsArchive == 1 ? 0 : 1) + "'\n";
		sContent+="language='1'\n";
		sContent+="size='" + sJavaSize + "'\n";
		sContent+="circuit='/circuit/" + LT_CircuitName + ".gif'\n";
		sContent+="user='" + LT_UserCookie + "'\n";
		sContent+="pluginspage='http://java.sun.com/products/plugin/index.html#download'>\n";
		sContent+="<noembed>Java is required to view this page.</noembed>\n";
		sContent+="</embed>\n";
		sContent+="</comment></object>\n";
		iContent+="</iframe>\n";
		
		$('liveTimingsApplet').innerHTML=iContent;
		$('liveTimingsApplet').style.height=sHeight;
		
        var tFrame = $('f1app');
        var doc = tFrame.contentDocument;
        if (doc == undefined || doc == null)
            doc = tFrame.contentWindow.document;
        doc.open();
        doc.write(sContent);
        doc.close();
        }

function ShowHelpForSession(iSessionID)
{
    ShowHelpForSession(LT_iSessionTypeID);
}

function ShowHelpForSession(iSessionID)
{
    //Hide all
    var itemsToHide = $$("#ltnHelp li");
    for (i = 0; i < itemsToHide.length; i++) {
        itemsToHide[i].setStyle("display", "none");
    }
    
    var itemsToShow;
    itemsToShow = $$(".donotremoveItem");
    for (i = 0; i < itemsToShow.length; i++) {  
        itemsToShow[i].setStyle("display", "");
    }
    
    itemsToShow = $$(".session_" + iSessionID);
    for (i = 0; i < itemsToShow.length; i++) {  
        itemsToShow[i].setStyle("display", "");
    }        
}

function UpdateLiveTiming()
{
    var n, s, i, x;
    var sSessionTitle, iCurrentSessionID;
    
    x = 0;
    n = new Date;

    sSessionTitle = aSessions[0].SessionName;
	
    for (i = 0; i < aSessions.length; i++) {
        s = new Date(aSessions[i].start);
        s.setMinutes(s.getMinutes() - 5); // Change titles 5 minutes early.

        if (s < n) {
	        sSessionTitle = aSessions[i].SessionName;
	        LT_iSessionID = aSessions[i].SessionID;
	        LT_iSessionTypeID = aSessions[i].SessionTypeID;
        }else {
	        //get time for next update
	        if (x == 0) {
		        x = (s.getTime() - n.getTime()) + 1; // update at the start of the next session
		    }
        } 		    
	    
	    ShowHelpForSession(LT_iSessionTypeID);
        document.getElementById("LiveTimingSession").childNodes[0].nodeValue = "(" + sSessionTitle + ")";
        
        if (x == 0) {
            x = 60 * 60 * 24 * 1000; // try again an hour later.
        }
        setTimeout("UpdateLiveTiming()", x);
    }
}

window.addEvent('domready', initTimingsHelpNav);
window.addEvent('domready', writeApplet);