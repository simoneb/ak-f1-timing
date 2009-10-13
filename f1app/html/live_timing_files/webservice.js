var buyImageLink = "/";

function loadXMLDoc(url)
{
    var xmlhttp = null;

    if (window.XMLHttpRequest) // code for Mozilla, etc.
    {
        xmlhttp=new XMLHttpRequest();
    }
    else if (window.ActiveXObject) // code for IE
    {
        xmlhttp=new ActiveXObject("Microsoft.XMLHTTP");
    }
    
    if (xmlhttp!=null)
    {
        xmlhttp.open("GET", url, false);
        //xmlhttp.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        xmlhttp.send(null);
        return xmlhttp.responseText;
    }
    else
    {
        return 'NOT_SUPPORTED';
    }
}


function requestImageDownload(un, pw, img)
{
    //return "ERROR";
    
    var ret = loadXMLDoc('https://secure.formula1.com/webservice/RequestDownload.aspx?login=' + escape(un) + '&password=' + escape(pw) + '&imagename=' + escape(img) + '&jscallback=responseImageDownload');
    var x = ret.indexOf('=');
    
    if (x >= 0)
        ret = ret.substring(0, x);
        
    return ret;
}

function responseImageDownload(sResult){
    var result;
    switch (sResult)
    {
        case "ERROR":
            result = "Sorry there has been an error, please try again later.";
            break;
        case "LOGIN_FAILED":
            result = "Your login details are incorrect, please go to the <a href='" + buyImageLink + "' target='_blank'>F1 Store</a> and check your account details or purchase download credits";
            break;
        case "MOBILE_INVALID":
            result = "Your mobile phone is invalid.";
            break;
        case "FUNDS_REQUIRED":
            result = "You do not have any remaining download credits, please go to the <a href='" + buyImageLink + "' target='_blank'>F1 Store</a> and purchase some more";
            break;
        case "OK":
            result = "The picture has been sent to your mobile";
            break;
        case "NOEMAIL":
            result = "No Email has been provided. Please try again.";
            break;
        case "NOPASSWORD":
            result = "No Password has been provided. Please try again.";
            break;
        case "NOIMG":
            result = "No Image has been provided. Please try again.";            
            break;
        default:
            result = "Sorry there has been an error, please try again later.";            
    }
    
    buyImageResponse(result);
}

function processImageDownload(tvImageSize, me)
{      
    //me.disabled=true;
        
    var eEmail = $("loginEmail");
    var ePassword = $("loginPassword");
    var eIMG = $("loginIMG");
    var sImageSrc
    var sResult = "";
    
    if (eEmail == null || eEmail.value == null) {
        sResult = "NOEMAIL";
    }else if (ePassword == null || ePassword.value == null) {
        ePassword = "NOPASSWORD";
    }else if (eIMG == null || eIMG.src == null) {
        sResult = "NOIMG";        
    }else {
        sImageSrc = eIMG.src.substring(eIMG.src.indexOf(tvImageSize) + tvImageSize.length + 1 + 9, eIMG.src.length);        
        
        var sPath = "https://secure.formula1.com/webservice/RequestDownload.aspx?login=" + escape(eEmail.value) + "&password=" + escape(ePassword.value) + "&imagename=" + escape(sImageSrc) + "&jscallback=responseImageDownload&t=" + new Date().getTime() + Math.random().toString(16)
        new Element('script').setProperties({type: 'text/javascript', src: sPath}).injectInside(document.body);
    } 
}
