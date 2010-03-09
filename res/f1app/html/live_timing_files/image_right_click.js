//=================================================================================================
function fncFesGetEnvironment()
//=================================================================================================
{

// Get the Navigator properties

    this.useragent = navigator.userAgent;
    this.appname = navigator.appName;
    this.appversion = navigator.appVersion;

// Get the user agent string and convert all characters to lowercase to simplify testing

    var loc_usragt = navigator.userAgent.toLowerCase();

//=================================================================================================
// Browser Information
//=================================================================================================

// Get the browser major and minor version id's

    this.major_version = parseInt(navigator.appVersion);
    this.minor_version = parseFloat(navigator.appVersion);

// Set up browser values
//
// ns = Netscape Navigator

// Opera and WebTV spoof Navigator so have to eliminate them via specific search for the browser
// names in the user agent.

    this.ns  = ((loc_usragt.indexOf("mozilla")!= -1) &&
                (loc_usragt.indexOf("spoofer") == -1) &&
                (loc_usragt.indexOf("compatible") == -1) &&
                (loc_usragt.indexOf("opera") == -1) &&
                (loc_usragt.indexOf("webtv") == -1) &&
                (loc_usragt.indexOf("safari") == -1) &&
                (loc_usragt.indexOf("hotjava") == -1));

    this.ns2 = (this.ns &&
               (this.major_version == 2));

    this.ns3 = (this.ns &&
               (this.major_version == 3));

    this.ns4 = (this.ns &&
               (this.major_version == 4));

// Although Netscape 6/7 should have major versions of 6/7 they actually report a major version
// of 5

    this.ns6 = (this.ns &&
                this.major_version == 5 &&
                loc_usragt.indexOf("netscape 7") == -1 &&
                loc_usragt.indexOf("netscape/7") == -1);

    this.ns7 = (this.ns &&
                this.major_version == 5 &&
               (loc_usragt.indexOf("netscape 7") != -1 ||
                loc_usragt.indexOf("netscape/7") != -1));

    this.ns4up = (this.ns &&
                 (this.major_version >= 4));

    this.ns6up = (this.ns &&
                 (this.major_version >= 5));

    this.ns7up = (this.ns &&
                  this.major_version == 5 &&
                 (loc_usragt.indexOf("netscape 7") != -1 ||
                  loc_usragt.indexOf("netscape/7") != -1));

    this.nsonly =  (this.ns &&
                  ((loc_usragt.indexOf(";nav") != -1) ||
                   (loc_usragt.indexOf("; nav") != -1)));

    this.gecko = (loc_usragt.indexOf("gecko") != -1 &&
                  loc_usragt.indexOf("safari") != -1);

// ie = Internet Explorer

    this.ie      = ((loc_usragt.indexOf("msie") != -1) &&
                    (loc_usragt.indexOf("opera") == -1));

    this.ie3     = (this.ie &&
                   (this.major_version < 4));

// Although Internet Explorer 4, 5, 5.5 and 6 should have a major versions of 4, 5, 5 and 6
// respectively they all report a major version of 4 so need to search the user agent string for
// specific names for each of them.

    this.ie4     = (this.ie &&
                   (this.major_version == 4) &&
                   (loc_usragt.indexOf("msie 4")!=-1));

    this.ie5     = (this.ie &&
                   (this.major_version == 4) &&
                   (loc_usragt.indexOf("msie 5.0")!=-1));

    this.ie5_5   = (this.ie &&
                   (this.major_version == 4) &&
                   (loc_usragt.indexOf("msie 5.5") !=-1));

    this.ie6     = (this.ie &&
                   (this.major_version == 4) &&
                   (loc_usragt.indexOf("msie 6.")!=-1) );

    this.ie4up   = (this.ie  &&
                   (this.major_version >= 4));

    this.ie5up   = (this.ie  &&
                   !this.ie3 &&
                   !this.ie4);

    this.ie5_5up = (this.ie &&
                   !this.ie3 &&
                   !this.ie4 &&
                   !this.ie5);

    this.ie6up   = (this.ie  &&
                   !this.ie3 &&
                   !this.ie4 &&
                   !this.ie5 &&
                   !this.ie5_5);

// aol = America Online - note that on AOL4 the test returns false if IE3 is the embedded browser
// or if this is the first browser window opened. Therefore the aol, aol3 and aol4 variables are
// not completely reliable

    this.aol   = (loc_usragt.indexOf("aol") != -1);

    this.aol3  = (this.aol &&
                  this.ie3);

    this.aol4  = (this.aol &&
                  this.ie4);

    this.aol5  = (loc_usragt.indexOf("aol 5") != -1);

    this.aol6  = (loc_usragt.indexOf("aol 6") != -1);

// opera = Opera

    this.opera =  (loc_usragt.indexOf("opera") != -1);

    this.opera2 = (loc_usragt.indexOf("opera 2") != -1 ||
                   loc_usragt.indexOf("opera/2") != -1);

    this.opera3 = (loc_usragt.indexOf("opera 3") != -1 ||
                   loc_usragt.indexOf("opera/3") != -1);

    this.opera4 = (loc_usragt.indexOf("opera 4") != -1 ||
                   loc_usragt.indexOf("opera/4") != -1);

    this.opera5 = (loc_usragt.indexOf("opera 5") != -1 ||
                   loc_usragt.indexOf("opera/5") != -1);

    this.opera6 = (loc_usragt.indexOf("opera 6") != -1 ||
                   loc_usragt.indexOf("opera/6") != -1);

    this.opera7 = (loc_usragt.indexOf("opera 7") != -1 ||
                   loc_usragt.indexOf("opera/7") != -1);

    this.opera5up = (this.opera &&
                    !this.opera2 &&
                    !this.opera3 &&
                    !this.opera4);

    this.opera6up = (this.opera &&
                    !this.opera2 &&
                    !this.opera3 &&
                    !this.opera4 &&
                    !this.opera5);

    this.opera7up = (this.opera &&
                    !this.opera2 &&
                    !this.opera3 &&
                    !this.opera4 &&
                    !this.opera5 &&
                    !this.opera6);
// safari = Safari

    this.safari = (loc_usragt.indexOf("safari") != - 1);

// webtv = WebTV

    this.webtv = (loc_usragt.indexOf("webtv") != -1);

    this.tvnavigator = ((loc_usragt.indexOf("navio") != -1) ||
                        (loc_usragt.indexOf("navio_aoltv") != -1));

    this.aoltv = this.tvnavigator;

// hotjava - HotJava (Sun)

    this.hotjava = (loc_usragt.indexOf("hotjava") != -1);

    this.hotjava3 = (this.hotjava &&
                    (this.major_version == 3));

    this.hotjava3up = (this.hotjava &&
                      (this.major_version >= 3));

//=================================================================================================
//
// Javascript Information
//
//=================================================================================================

    if (this.ns2 ||
        this.ie3)
    {
        this.js = 1.0;

    }

    if (this.ns3)
    {
        this.js = 1.1;

    }

    if (this.opera)
    {
        this.js = 1.1;

    }

    if (this.opera5up)
    {
        this.js = 1.3;

    }

    if ((this.ns4 &&
        (this.minor_version <= 4.05)) ||
         this.ie4)
    {
        this.js = 1.2;

    }

    if ((this.ns4 &&
        (this.minor_version > 4.05)) ||
         this.ie5)
    {
        this.js = 1.3;

    }

    if (this.hotjava3up)
    {
        this.js = 1.4;

    }

    if (this.ns6 ||
       this.gecko)
    {
        this.js = 1.5;

    }

    if (this.ns6up)
    {
        this.js = 1.5;

    }

    if (this.ie5up&
        loc_usragt.indexOf("mac") == -1)
    {
        this.js = 1.3;

    }


// On the Mac platform Javascript is Version 1.4 under Internet Explorer 5 and above

    if (this.ie5up&
        loc_usragt.indexOf("mac") != -1)
    {
        this.js = 1.4;
    }

//=================================================================================================
//
// Platform/OS Information
//
//=================================================================================================

// Microsoft Windows

    this.win = ((loc_usragt.indexOf("win") != -1) ||
                (loc_usragt.indexOf("16bit") != -1));

// On Opera 3.0, the userAgent string includes "Windows 95/NT4" on all Win32 platforms so
// distnguishing between Windows 95 and NT is not possible

    this.win95 = ((loc_usragt.indexOf("win95") != -1) ||
                  (loc_usragt.indexOf("windows 95") != -1));

    this.win16 = ((loc_usragt.indexOf("win16") != -1) ||
                  (loc_usragt.indexOf("16bit") != -1) ||
                  (loc_usragt.indexOf("windows 3.1") != -1) ||
                  (loc_usragt.indexOf("windows 16-bit") != -1));

    this.win31 = ((loc_usragt.indexOf("windows 3.1") != -1) ||
                  (loc_usragt.indexOf("win16") != -1) ||
                  (loc_usragt.indexOf("windows 16-bit") != -1));

// Reliable detection of Windows 98 is not possible as on Ns4 and earlier you just get 'Windows'
// in the user agent string and on the Mercury client you get 'Win98' returned for the 32 bit
// version, but 'Win95' for the 16 bit version

    this.win98 = ((loc_usragt.indexOf("win98") != -1) ||
                  (loc_usragt.indexOf("windows 98") != -1));

    this.winnt = ((loc_usragt.indexOf("winnt") != -1) ||
                  (loc_usragt.indexOf("windows nt") != -1) ||
                  (loc_usragt.indexOf("windows 2000") != -1));

    this.win32 =  (this.win95 ||
                   this.winnt ||
                   this.win98 ||
                 ((this.major_version >= 4) &&
                  (navigator.platform == "Win32")) ||
                  (loc_usragt.indexOf("win32") != -1) ||
                  (loc_usragt.indexOf("32bit") != -1));

    this.winme = (loc_usragt.indexOf("win 9x 4.90") != -1);

    this.win2k = ((loc_usragt.indexOf("windows nt 5.0") != -1) ||
                  (loc_usragt.indexOf("windows 2000") != -1));

    this.winxp = (loc_usragt.indexOf("windows nt 5.1") != -1);

// IBM OS2

    this.os2   = ((loc_usragt.indexOf("os/2") != -1) ||
                  (navigator.appVersion.indexOf("OS/2") != -1) ||
                  (loc_usragt.indexOf("ibm-webexplorer") != -1));

// Apple Mac

    this.mac    = (loc_usragt.indexOf("mac") != -1);

    this.mac68k =  (this.mac &&
                  ((loc_usragt.indexOf("68k") != -1) ||
                   (loc_usragt.indexOf("68000") != -1)));

    this.macppc =  (this.mac &&
                  ((loc_usragt.indexOf("ppc") != -1) ||
                   (loc_usragt.indexOf("powerpc") != -1)));

// Various Unix and Linux derivatives

    this.sun   = (loc_usragt.indexOf("sunos") != -1);

    this.sun4  = (loc_usragt.indexOf("sunos 4") != -1);

    this.sun5  = (loc_usragt.indexOf("sunos 5") != -1);

    this.suni86 = (this.sun && (loc_usragt.indexOf("i86") != -1));

    this.irix  = (loc_usragt.indexOf("irix") != -1);

    this.irix5 = (loc_usragt.indexOf("irix 5") != -1);

    this.irix6 = ((loc_usragt.indexOf("irix 6") != -1) ||
                  (loc_usragt.indexOf("irix6") != -1));

    this.hpux  = (loc_usragt.indexOf("hp-ux")!=-1);

    this.hpux9 = (this.hpux &&
                 (loc_usragt.indexOf("09.") != -1));

    this.hpux10= (this.hpux &&
                 (loc_usragt.indexOf("10.") != -1));

    this.aix   = (loc_usragt.indexOf("aix") != -1);

    this.aix1  = (loc_usragt.indexOf("aix 1") != -1);

    this.aix2  = (loc_usragt.indexOf("aix 2") != -1);

    this.aix3  = (loc_usragt.indexOf("aix 3") != -1);

    this.aix4  = (loc_usragt.indexOf("aix 4") != -1);

    this.linux = (loc_usragt.indexOf("inux") != -1);

    this.sco   = (loc_usragt.indexOf("sco") != -1) ||
                 (loc_usragt.indexOf("unix_sv") != -1);

    this.unixware = (loc_usragt.indexOf("unix_system_v") != -1);

    this.mpras    = (loc_usragt.indexOf("ncr") != -1);

    this.reliant  = (loc_usragt.indexOf("reliantunix") != -1);

    this.dec   = ((loc_usragt.indexOf("dec") != -1) ||
                  (loc_usragt.indexOf("osf1") != -1) ||
                  (loc_usragt.indexOf("dec_alpha") != -1) ||
                  (loc_usragt.indexOf("alphaserver") != -1) ||
                  (loc_usragt.indexOf("ultrix") != -1) ||
                  (loc_usragt.indexOf("alphastation") != -1));

    this.sinix = (loc_usragt.indexOf("sinix") != -1);

    this.freebsd = (loc_usragt.indexOf("freebsd") != -1);

    this.bsd = (loc_usragt.indexOf("bsd") != -1);

    this.unix  = ((loc_usragt.indexOf("x11") != -1) ||
                   this.sun ||
                   this.irix ||
                   this.hpux ||
                   this.sco ||
                   this.unixware ||
                   this.mpras ||
                   this.reliant ||
                   this.dec ||
                   this.sinix ||
                   this.aix ||
                   this.linux ||
                   this.bsd ||
                   this.freebsd);

// Digital/Compaq VMS

    this.vms   = ((loc_usragt.indexOf("vax")!=-1) || (loc_usragt.indexOf("openvms")!=-1));
}

//=================================================================================================
function fncFesHandleContextMenuForImages(prm_event)
//=================================================================================================
//Function to disable right mouse click context menu for images only Arguments: prm_event - in this instance equates to mouse key pressed
{

// The script pops up an alert box with a 'message' or if no message is passed just returns without any action

// Internet Explorer 4 - test for mouse button press

    if (fes_environment.ie4)
    {
        if (event.button == 2 || event.button == 3)
        {
            if (event.srcElement.tagName == "IMG")
            {
                if (pub_contextmenu_message != "")
                {
                    alert(pub_contextmenu_message);

                }

                return false;

            }

        }

    }

// Internet Explorer 5 upwards

    else if (fes_environment.ie5up)
    {
        if (event.srcElement.tagName == "IMG")
        {
            if (pub_contextmenu_message != "")
            {
                alert(pub_contextmenu_message);

            }

            return false;

        }

    }

// Netscape 4 - test for mouse button press

    else if (fes_environment.ns4)
    {
        if (prm_event.which == 3)
        {
            if (pub_contextmenu_message != "")
            {
                alert(pub_contextmenu_message);

            }

            return false;

        }

    }

// Netscape 6

    else if (fes_environment.ns6up)
    {
        if (prm_event.target.tagName == "IMG")
        {
            if (pub_contextmenu_message != "")
            {
                alert(pub_contextmenu_message);

            }

            return false;

        }

    }

// Opera 6 upwards - not currently functional due to bug in Opera apparently

    else if (fes_environment.opera6up)
    {
        if (prm_event.which == 3 && prm_event.target.tagName == "IMG")
        {
            if (pub_contextmenu_message != "")
            {
                alert(pub_contextmenu_message);

            }

            return false;

        }

    }

}

//=================================================================================================
function fncFesAssociateImages()
//=================================================================================================
// Function to associate mousedown event on images with context menu handler - used for Netscape 4 only. Arguments:None

{
    for(loc_counter = 0; loc_counter < document.images.length; loc_counter++)
    {
       document.images[loc_counter].onmousedown = fncFesHandleContextMenuForImages;

    }

}

//=================================================================================================
function fncFesDisableContextMenuForImages()
//=================================================================================================
// Function to invoke the disabling of the right mouse click context menu for images only Arguments: None


{
    if (fes_environment.ie4)
    {
        document.onmousedown = fncFesHandleContextMenuForImages;

    }

    else if (fes_environment.ie5up)
    {
        document.oncontextmenu = fncFesHandleContextMenuForImages;

    }

    else if (fes_environment.ns6up)
    {
        document.oncontextmenu = fncFesHandleContextMenuForImages;

    }

    else if (fes_environment.opera6up)
    {
        document.onmouseup = fncFesHandleContextMenuForImages;

    }

    else if (fes_environment.ns4)
    {
        fncFesAssociateImages();

    }

}


//=================================================================================================
	// Set up environment object
    var fes_environment = new fncFesGetEnvironment();
//=================================================================================================

