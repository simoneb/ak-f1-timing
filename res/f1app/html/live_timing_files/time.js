/*
Created by Carlos, 2004-02-25 for Timetable page but can be reused elsewhere.
*/
function Time_ParseISO8601 (sISO, bWithTimeZone) {
/*
    ISO 8601:
    1999-07-03T00:00:00+0100
    JavaScript's Date.parse() format:
    1999/07/03 00:00:00 UTC+0100
*/
    var sYear = sISO.substr(0, 4);
    var sMonth = sISO.substr(5, 2);
    var sDay = sISO.substr(8, 2);
    var sHour = sISO.substr(11, 2);
    var sMinute = sISO.substr(14, 2);
    var sZone;

    if (bWithTimeZone)
        sZone = sISO.substr(16, 3).toString() + sISO.substr(20, 2); // offset from UTC 
    else
        sZone = "+0000"; // make it a UTC time

    var sFormat = sYear + "/" + sMonth + "/" + sDay + " " + sHour + ":" + sMinute + ":00 UTC" + sZone

    return new Date(sFormat);
}

function Time_GetMonthName(iMonth) {
    switch (iMonth) {
        case 0: return "January"; break;
        case 1: return "February"; break;
        case 2: return "March"; break;
        case 3: return "April"; break;
        case 4: return "May"; break;
        case 5: return "June"; break;
        case 6: return "July"; break;
        case 7: return "August"; break;
        case 8: return "September"; break;
        case 9: return "October"; break;
        case 10: return "November"; break;
        case 11: return "December"; break;
        default: return ""; 
    }
}

function Time_GetWeekdayName(iDay) {
    switch (iDay) {
        case 0: return "Sunday"; break;
        case 1: return "Monday"; break;
        case 2: return "Tuesday"; break;
        case 3: return "Wednesday"; break;
        case 4: return "Thursday"; break;
        case 5: return "Friday"; break;
        case 6: return "Saturday"; break;
        default: return ""; 
    }
}

function CT_ToggleTimeZones(anchor) {
    var dTime, dOtherTime;
    var sZone = anchor.id.toString().toLowerCase();
    var iDay = 1;
    var iTime = 1;
    var obj;
    var bLocal = false;
    var iLastDay;

    switch (sZone) {
        case "race":
                bLocal = false;
                anchor.childNodes[0].nodeValue = "Convert To My Local Time";
                anchor.id = "local";
                break;
        default:
                bLocal = true;
                anchor.childNodes[0].nodeValue = "Convert To Race Local Time";
                anchor.id = "race";
    }

    obj = document.getElementById("CT_Time_" + iDay + "_" + iTime);
    
    while (obj != null) {
        iLastDay = -1;
        
        while (obj != null) {
			if ((iLastDay == -1) && (bLocal))
			{
				iLastDay = Time_ParseISO8601(obj.className, !bLocal).getUTCDay();
			}
			
            dTime = Time_ParseISO8601(obj.className, bLocal);
            
            if (iTime == 1)
                if (bLocal)
                    var eTime = document.getElementById("CT_Time_" + iDay);
                    if (eTime) eTime.innerHtml = Time_GetWeekdayName(dTime.getDay()) + "&nbsp;" + dTime.getDate() + "&nbsp;" + Time_GetMonthName(dTime.getMonth());
                else
                    var eTime = document.getElementById("CT_Time_" + iDay);
                    if (eTime) eTime.innerHtml = Time_GetWeekdayName(dTime.getUTCDay()) + "&nbsp;" + dTime.getUTCDate() + "&nbsp;" + Time_GetMonthName(dTime.getUTCMonth());
            
            obj.childNodes[0].nodeValue = "";
            
            if (bLocal) {
                if ((iLastDay != dTime.getDay()) && (iLastDay >= 0))
                    obj.childNodes[0].nodeValue = "(" + Time_GetWeekdayName(dTime.getDay()).substr(0, 3) + ") ";
                obj.childNodes[0].nodeValue += ZeroPadInteger(dTime.getHours()) + ":" + ZeroPadInteger(dTime.getMinutes());
            } else {
                if ((iLastDay != dTime.getUTCDay()) && (iLastDay >= 0))
                    obj.childNodes[0].nodeValue = "(" + Time_GetWeekdayName(dTime.getUTCDay()).substr(0, 3) + ") ";
                obj.childNodes[0].nodeValue += ZeroPadInteger(dTime.getUTCHours()) + ":" + ZeroPadInteger(dTime.getUTCMinutes());
            }
            
            if (bLocal)
                iLastDay = dTime.getDay();
            else
                iLastDay = dTime.getUTCDay();
            
            iTime++;
            obj = document.getElementById("CT_Time_" + iDay + "_" + iTime);
        }
        
        iTime = 1;
        iDay++;
        obj = document.getElementById("CT_Time_" + iDay + "_" + iTime);
    }

    return false;
}

/*
Created by Carlos, 2004-02-25 for Timetable page but can be reused elsewhere.
*/
function ZeroPadInteger(integer) {
    string = "";
    
    // Only zero pads up to 99.
    
    try {
        integer = parseInt(integer, 10);

        if (integer < 0)
            string = integer;
        else if (integer < 10)
            string = "0" + integer.toString();
        else 
            string = integer.toString();
    } catch (error) {
        // ignore it
    }

    return string;
}


