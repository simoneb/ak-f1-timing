package uk.co.aspectgroup.f1app;

import java.applet.Applet;
import java.applet.AppletContext;
import java.awt.Color;
import java.awt.Cursor;
import java.awt.Font;
import java.awt.FontMetrics;
import java.awt.Graphics;
import java.awt.Image;
import java.awt.MediaTracker;
import java.awt.Rectangle;
import java.awt.event.KeyEvent;
import java.awt.event.MouseEvent;
import java.awt.image.ImageObserver;
import java.awt.image.MemoryImageSource;
import java.awt.image.PixelGrabber;
import java.io.IOException;
import java.io.InputStream;
import java.io.PrintStream;
import java.net.MalformedURLException;
import java.net.URL;

public class f1app extends Applet implements ImageObserver {

    static final String version = "5.1.5";
    private static final long serialVersionUID = 823783261L;
    static String copyrightMessage = "© Formula One Administration Limited. No reproduction without permission.";
    static final boolean debugging = false;
    static final int CIRCUITHEIGHT = 80;
    static final int CIRCUITWIDTH = 100;
    static final int SCREEN3CELL = 45;
    static final int NROWS = 29;
    static final int MAXSLOTS = 32;
    static final int MAXCOLUMNS = 13;
    static final int MAXLAPS = 100;
    static final int MAX_COMMENTARY_LINES = 4096;
    static final byte ALIGNED_R = 0;
    static final byte ALIGNED_L = 1;
    static final byte ALIGNED_C = 2;
    static final int RACEMODE_PRACTICE = 2;
    static final int RACEMODE_QUALIFYING = 3;
    static final int RACEMODE_QUALIFYING1 = 4;
    static final int RACEMODE_QUALIFYING2 = 5;
    static final int RACEMODE_RACE = 1;
    static final char SCROLL_UP = 1;
    static final char SCROLL_DOWN = 2;
    static final int SCROLL_OFF = 0;
    static final int SCROLL_ON = 1;
    static final int SCROLL_HI = 2;
    static final int ANY_LANGUAGE = 0;
    static final int ENGLISH = 1;
    static final int CHINESE = 2;
    static final int SCREEN_SIZE_NORMAL = 10;
    static final int SCREEN_SIZE_INLINE = 15;
    static final int SCREEN_SIZE_BIG = 20;
    static final int SCREEN_SIZE_BIGPOPUP = 25;
    private int WIDTH = 584;

    private int HEIGHT = 434;

    private int CWIDTH = 145;

    private int TABSIZE = 19;

    private int TABHEIGHT = 140;

    private int TABFUDGE = 17;

    private String TAB1LABEL = "Live Timing";

    private String TAB2LABEL = "Lap Chart";

    private String TAB3LABEL = "Weather & Speed";

    private int CELLHEIGHT = 13;

    private int CELLTOP = 20;

    private int CELLTOPMAGIC = 4;

    private int COMMENTHEAD = 45;

    private int COMMENTTAIL = 25;

    private int COMMENTPAD = 0;

    public int BORDERSIZE = 8;

    private int COPYRIGHTHEIGHT = 13;

    private int SCROLLBAR_WIDTH = 0;

    private int GWIDTH = 180;

    private int GHEIGHT = 120;

    private int SCREEN3SPACE = 15;

    private int SCREEN2LABELWIDTH = 100;
    private Color fgColour;
    private Color bgColour1;
    private Color bgColour2;
    private Color bgColour2a;
    private Color bgColour3;
    private Color bgColourC;
    private Color headColour;
    private Color navOnColour;
    private Color navOffColour;
    private Color navDisColour;
    private Color navHiColour;
    private Color LEDColour;
    private Color arrowColour1;
    private Color arrowColour2;
    private Color arrowColour3;
    private final int[] gColourRGBs = { 11957278, 9675188, 10317434, 16178806, 13566035, 33633, 5594286, 8155969, 12519994, 11316396, 41592, 42685, 11731067, 13936653, 9343357, 15298840, 6328996, 7100031, 12869138, 13603476, 13747854, 7829367, 16711680, 65280, 255, 16776960, 16777215, 16777215, 16777215 };
    private Font headFont;
    private Font tabFont;
    private Font dataFont;
    private Font timeFont;
    private Font commentaryFont;
    private Font commentaryHeadFont;
    private Font copyrightFont;
    private Image offscreen;
    private Image offscreen1;
    private Image offscreen2;
    private Image offscreen3;
    private Image offscreenC;
    private Image buttonsH;
    private Image sponsorLogo;
    private Image buttons1;
    private Image buttons2;
    private Image buttons3;
    private Image commentaryImage;
    private Image circuitImage;
    private Image[] wGraphs;
    public boolean isLoaded;
    public boolean loadingKeyframe;
    public boolean noCommentaryScroll;
    public int raceMode;
    public boolean modeSet;
    private boolean redraw;
    public boolean streaming;
    public int language;
    private boolean weather;
    private int season;
    private boolean isValid = true;
    private int screenNum;
    private boolean mouseOvering = false;

    private boolean handCursor = false;
    public f1process theProcess;
    private f1time countdownTimer;
    public f1crypt decrypter;
    private f1scrollbar scrollbar;
    private int timeNow;
    public int nCols;
    private int[] colWidths;
    private int[] colFastest;
    private final byte[] alignedL = { 0, 0, 1, 0, 0, 0, 0, 1, 0, 1, 0, 1, 1 };
    private Color[] colours;
    private final byte[] scalingWidths = { 32, 32, 16, 10, 8, 6, 5, 4, 3, 1 };

    private final short[] scalingLevels = { -1, 9, 18, 30, 37, 50, 60, 75, 100, 300 };

    private final byte[] scalingSteps = { 1, 1, 1, 2, 2, 3, 4, 4, 6, 16 };
    private byte scaling;
    private Color[] gColours;
    private byte[][] gRow;
    private byte[] gLap;
    private String[] gName;
    private byte gLapNo;
    private int gHighLight;
    private int[] offscreen2Pixels;
    private int screenGap;
    private Color[] wGColours;
    private int[] graphMin;
    private int[] graphMax;
    private String[] graphMinLabels;
    private String[] graphMaxLabels;
    private f1list[] graphData;
    private f1list[] graphTime;
    private String[] gTitles;
    private f1weather weatherTicker;
    private final int[] timePeriods = { 900, 1800, 3600, 7200, 10800, -1 };

    private final String[] timeLabels = { "period 15 minutes", "period 30 minutes", "period 1 hour", "period 2 hours", "period 3 hours", "session" };
    private int timePeriod;
    private String lastOfficialTime;
    private long lastOfficialTimeTime;
    private String fastestNumber;
    private String fastestName;
    private String fastestTime;
    private String fastestLap;
    private int commentaryHeight;
    private int commentaryRows;
    private int commentaryOffset;
    private int scrollLine;
    public char scrolling;
    public boolean scrollbaring = false;
    private int scrollbarStartY;
    private int scrollbarStartOffset;
    private String[] commentaryLines;
    private int nCommentaryLines;
    private int linkBoxEntries;
    private String[] linkBoxName;
    private URL[] linkBoxURL;
    private Color linkBoxColour;
    private Font linkBoxFont;
    static final int LINKBOXHEIGHT = 25;
    private String safetyMessage;
    private String[] messageLines;
    private int nMessageLines;
    private String userCreds;
    private String sessionID;
    private int encryptionKey;
    private int screenSize = 10;

    private boolean inlineLayout = false;

    private boolean bigScreenLayout = false;
    public boolean stopping;

    @Override
    public void init() {

        int trackerid = 0;

        System.out.println("f1app (c) 2003-2006 LB Icon (UK) - Version 5.1.5");
        System.out.println("http://www.lbicon.co.uk/");

        this.isLoaded = false;
        this.raceMode = 2;
        this.modeSet = false;

        this.screenSize = 10;
        String param = getParameter("size");
        debug("Size: " + param);
        if(param != null) {
            if(param.equalsIgnoreCase("big")) {
                debug("Large format for: " + param);
                this.screenSize = 20;
                this.bigScreenLayout = true;
            } else if(param.equalsIgnoreCase("inline")) {
                debug("Inline format for: " + param);
                this.screenSize = 15;
                this.inlineLayout = true;
            } else if(param.equalsIgnoreCase("bigpopup")) {
                debug("Big popup format for: " + param);
                this.screenSize = 25;
                this.inlineLayout = true;
                this.bigScreenLayout = true;
            }
        }
        this.streaming = true;
        param = getParameter("streaming");
        if(param != null) {
            try {
                if(Integer.parseInt(param) == 0)
                    this.streaming = false;
            } catch (NumberFormatException e) {
                this.streaming = false;
            }
        }

        this.language = 1;
        param = getParameter("language");
        if(param != null) {
            try {
                this.language = Integer.parseInt(param);
            } catch (NumberFormatException localNumberFormatException1) {
            }

        }

        this.linkBoxEntries = 0;
        param = getParameter("nlinks");
        if(param != null) {
            try {
                this.linkBoxEntries = Integer.parseInt(param);
            } catch (NumberFormatException localNumberFormatException2) {
            }
        }
        if(this.linkBoxEntries > 0) {
            this.linkBoxName = new String[this.linkBoxEntries];
            this.linkBoxURL = new URL[this.linkBoxEntries];

            for(int n = 0; n < this.linkBoxEntries; ++n) {
                debug("*** Getting param link" + n + "name = " + getParameter(new StringBuilder("link").append(n).append("name").toString()));
                this.linkBoxName[n] = getParameter("link" + n + "name");
                try {
                    this.linkBoxURL[n] = new URL(getCodeBase(), getParameter("link" + n + "url"));
                } catch (MalformedURLException localMalformedURLException) {
                }

            }

        }

        this.weather = false;

        param = getParameter("textcolour");
        if(param != null)
            this.fgColour = new Color(Integer.parseInt(param, 16));
        else {
            this.fgColour = Color.white;
        }

        String circuit = getParameter("circuit");
        if(circuit == null) {
            circuit = "img/circuit.gif";
        }

        param = getParameter("season");
        if(param != null)
            try {
                this.season = Integer.parseInt(param);
            } catch (NumberFormatException e) {
                this.season = 2004;
            }
        else {
            this.season = 2004;
        }

        debug("Season: " + this.season);

        if(this.season >= 2009) {
            copyrightMessage = "© Formula One Administration Ltd ";
        }

        param = getParameter("user");
        if(param != null)
            this.userCreds = param;
        else {
            this.userCreds = "";
        }

        System.out.println("User: " + this.userCreds);

        debug("Debug setting: false");

        if(this.screenSize == 10)
            initScreenNormal();
        else if(this.screenSize == 20)
            initScreenBig();
        else if(this.screenSize == 15)
            initScreenInline();
        else if(this.screenSize == 25) {
            initScreenBigPopup();
        }

        debug("Codebase: " + getCodeBase());

        MediaTracker tracker = new MediaTracker(this);

        if(this.inlineLayout) {
            this.buttons3 = (this.buttons2 = this.buttons1 = getImage(getCodeBase(), "img/navinline.gif"));
            tracker.addImage(this.buttons1, trackerid++);
        } else {
            this.buttons1 = getImage(getCodeBase(), "img/nav1.jpg");
            this.buttons2 = getImage(getCodeBase(), "img/nav2.jpg");
            this.buttons3 = getImage(getCodeBase(), "img/nav3.jpg");
            tracker.addImage(this.buttons2, trackerid++);
            tracker.addImage(this.buttons3, trackerid++);
        }

        if(this.season >= 2009) {
            this.sponsorLogo = getImage(getCodeBase(), "img/lglogo.gif");
            tracker.addImage(this.sponsorLogo, trackerid++);
        }

        if(this.screenSize == 20)
            this.commentaryImage = getImage(getCodeBase(), "img/commentary_big.gif");
        else if(this.screenSize == 10) {
            this.commentaryImage = getImage(getCodeBase(), "img/commentary.gif");
        }
        tracker.addImage(this.commentaryImage, trackerid++);

        this.circuitImage = getImage(getCodeBase(), circuit);
        tracker.addImage(this.circuitImage, trackerid++);
        try {
            tracker.waitForAll();
        } catch (InterruptedException localInterruptedException) {
        }

        this.headColour = new Color(13421772);

        this.bgColour1 = Color.black;

        this.LEDColour = Color.darkGray;

        this.bgColour2 = Color.black;
        this.bgColour2a = new Color(604676);

        this.bgColour3 = Color.black;

        this.colours = new Color[8];
        this.colours[0] = new Color(0);
        this.colours[1] = new Color(16777215);
        this.colours[2] = new Color(16711680);
        this.colours[3] = new Color(65280);
        this.colours[4] = new Color(16711935);
        this.colours[5] = new Color(65535);
        this.colours[6] = new Color(16776960);
        this.colours[7] = new Color(10066329);

        this.arrowColour1 = new Color(16776960);
        this.arrowColour2 = new Color(13421568);
        this.arrowColour3 = new Color(10066176);

        this.offscreen = createImage(this.WIDTH, this.HEIGHT);
        this.offscreen1 = createImage(this.WIDTH - this.CWIDTH, this.HEIGHT);
        this.offscreenC = createImage(this.CWIDTH, this.HEIGHT);
        this.buttonsH = createImage(this.TABSIZE, this.HEIGHT);

        debug("headFont: " + this.headFont.toString());
        debug("tabFont: " + this.tabFont.toString());
        debug("dataFont: " + this.dataFont.toString());
        debug("timeFont: " + this.timeFont.toString());
        debug("commentaryFont: " + this.commentaryFont.toString());
        debug("copyrightFont: " + this.copyrightFont.toString());

        initScreenC();
        initScreen1();
        if(this.weather) {
            initScreen3();
        }

        Graphics g = this.buttonsH.getGraphics();
        g.drawImage(this.buttons3, 0, 0, this.TABSIZE, this.HEIGHT / 2, 0, 0, this.TABSIZE, this.HEIGHT / 2, this);
        g.drawImage(this.buttons1, 0, this.HEIGHT / 2, this.TABSIZE, this.HEIGHT, 0, this.HEIGHT / 2, this.TABSIZE, this.HEIGHT, this);

        g.setFont(this.tabFont);
        g.setColor(this.navHiColour);
        drawVText(this.buttonsH, g, this.TABSIZE - 2, this.TABHEIGHT - this.TABFUDGE, 1, this.TABFUDGE / 2 + 1, this.TAB1LABEL);
        drawVText(this.buttonsH, g, this.TABSIZE - 2, this.TABHEIGHT - this.TABFUDGE, 1, this.TABHEIGHT + this.TABFUDGE / 2 + 1, this.TAB3LABEL);
        drawVText(this.buttonsH, g, this.TABSIZE - 2, this.TABHEIGHT - this.TABFUDGE, 1, this.TABHEIGHT * 2 + this.TABFUDGE / 2 + 1, this.TAB2LABEL);
        g.dispose();

        this.screenNum = 1;
        g = this.offscreen.getGraphics();

        g.drawImage(this.offscreenC, this.WIDTH - this.CWIDTH, 0, this);
        g.drawImage(this.offscreen1, 0, 0, this);

        g.dispose();

        drawLED();

        this.gColours = new Color[29];

        for(int n = 0; n < 29; ++n) {
            this.gColours[n] = new Color(this.gColourRGBs[n]);
        }

        this.safetyMessage = "Please Wait ...";

        debug("$Id$");

        this.redraw = true;
        this.isLoaded = true;
    }

    private void initScreenNormal() {

        this.headFont = new Font("SansSerif", 1, 10);
        this.tabFont = new Font("SansSerif", 1, 10);
        this.dataFont = new Font("SansSerif", 0, 11);
        this.timeFont = new Font("SansSerif", 1, 14);

        if(this.language == 2) {
            this.commentaryFont = new Font("Serif", 0, 15);

            this.commentaryHeadFont = new Font("SansSerif", 1, 12);
        } else {
            this.commentaryFont = new Font("SansSerif", 0, 11);
            this.commentaryHeadFont = new Font("SansSerif", 1, 12);
        }
        this.copyrightFont = new Font("SansSerif", 0, 10);

        this.navOnColour = Color.white;
        this.navOffColour = Color.white;
        this.navDisColour = Color.gray;
        this.navHiColour = Color.red;

        this.bgColourC = Color.white;
    }

    private void initScreenInline() {

        this.WIDTH = 710;
        this.CWIDTH = 263;
        this.TABSIZE = 27;

        this.TABHEIGHT = 100;
        this.TABFUDGE = 2;

        if(this.season >= 2009) {
            this.COPYRIGHTHEIGHT = 30;
        }

        this.headFont = new Font("SansSerif", 1, 10);
        this.tabFont = new Font("SansSerif", 1, 10);
        this.dataFont = new Font("SansSerif", 0, 11);
        this.timeFont = new Font("SansSerif", 1, 14);

        if(this.language == 2) {
            this.commentaryFont = new Font("Serif", 0, 15);

            this.commentaryHeadFont = new Font("SansSerif", 1, 12);
        } else {
            this.commentaryFont = new Font("SansSerif", 0, 12);
            this.commentaryHeadFont = new Font("SansSerif", 1, 12);
        }
        this.copyrightFont = new Font("SansSerif", 0, 10);

        this.navOnColour = Color.green;
        this.navOffColour = Color.white;
        this.navDisColour = Color.gray;
        this.navHiColour = Color.red;

        this.TAB1LABEL = "Live Timing";
        this.TAB2LABEL = "Lap Chart";
        this.TAB3LABEL = "Weather & Speed";

        this.BORDERSIZE = 5;
        this.COMMENTPAD = 5;

        this.COMMENTHEAD = 0;
        this.COMMENTTAIL = (this.linkBoxEntries * 25);

        this.SCROLLBAR_WIDTH = 15;

        this.bgColourC = new Color(15724527);
    }

    private void initScreenBig() {

        this.WIDTH = 880;
        this.CWIDTH = 245;
        this.TABSIZE = 19;

        this.HEIGHT = 564;
        this.CELLTOP = 20;
        this.CELLHEIGHT = 17;
        this.COMMENTHEAD = 45;
        this.COMMENTTAIL = 25;
        this.COPYRIGHTHEIGHT = 13;

        this.TABHEIGHT = 140;
        this.CELLTOPMAGIC = 4;

        this.GWIDTH = 270;
        this.GHEIGHT = 180;
        this.SCREEN3SPACE = 55;

        this.SCREEN2LABELWIDTH = 120;

        this.headFont = new Font("SansSerif", 1, 12);
        this.tabFont = new Font("SansSerif", 1, 12);
        this.dataFont = new Font("SansSerif", 0, 13);
        this.timeFont = new Font("SansSerif", 1, 16);

        if(this.language == 2) {
            this.commentaryFont = new Font("Serif", 0, 19);

            this.commentaryHeadFont = new Font("SansSerif", 1, 12);
        } else {
            this.commentaryFont = new Font("SansSerif", 0, 15);
            this.commentaryHeadFont = new Font("SansSerif", 1, 12);
        }
        this.copyrightFont = new Font("SansSerif", 0, 10);

        for(int n = 0; n < 10; ++n) {
            debug("Before - Scaling: up to " + this.scalingLevels[n] + ", max width: " + (this.scalingWidths[n] * this.scalingLevels[n]) + ", label width: " + (this.scalingSteps[n] * this.scalingWidths[n]));
        }

        for(int n = 0; n < 10; ++n) {
            this.scalingWidths[n] = (byte)(this.scalingWidths[n] * 145 / 100);
        }

        for(int n = 0; n < 10; ++n) {
            debug("Scaling: up to " + this.scalingLevels[n] + ", max width: " + (this.scalingWidths[n] * this.scalingLevels[n]) + ", label width: " + (this.scalingSteps[n] * this.scalingWidths[n]));
        }

        this.navOnColour = Color.white;
        this.navOffColour = Color.white;
        this.navDisColour = Color.gray;
        this.navHiColour = Color.red;

        this.bgColourC = Color.white;
    }

    private void initScreenBigPopup() {

        this.WIDTH = 1006;
        this.CWIDTH = 363;
        this.TABSIZE = 27;

        this.HEIGHT = 564;
        this.CELLTOP = 20;
        this.CELLHEIGHT = 17;

        if(this.season >= 2009)
            this.COPYRIGHTHEIGHT = 30;
        else {
            this.COPYRIGHTHEIGHT = 13;
        }

        this.TABHEIGHT = 100;
        this.TABFUDGE = 2;

        this.GWIDTH = 270;
        this.GHEIGHT = 180;
        this.SCREEN3SPACE = 55;

        this.SCREEN2LABELWIDTH = 120;

        this.headFont = new Font("SansSerif", 1, 12);
        this.tabFont = new Font("SansSerif", 1, 10);
        this.dataFont = new Font("SansSerif", 0, 13);
        this.timeFont = new Font("SansSerif", 1, 16);

        if(this.language == 2) {
            this.commentaryFont = new Font("Serif", 0, 19);

            this.commentaryHeadFont = new Font("SansSerif", 1, 12);
        } else {
            this.commentaryFont = new Font("SansSerif", 0, 15);
            this.commentaryHeadFont = new Font("SansSerif", 1, 12);
        }
        this.copyrightFont = new Font("SansSerif", 0, 10);

        for(int n = 0; n < 10; ++n) {
            debug("Before - Scaling: up to " + this.scalingLevels[n] + ", max width: " + (this.scalingWidths[n] * this.scalingLevels[n]) + ", label width: " + (this.scalingSteps[n] * this.scalingWidths[n]));
        }

        for(int n = 0; n < 10; ++n) {
            this.scalingWidths[n] = (byte)(this.scalingWidths[n] * 145 / 100);
        }

        for(int n = 0; n < 10; ++n) {
            debug("Scaling: up to " + this.scalingLevels[n] + ", max width: " + (this.scalingWidths[n] * this.scalingLevels[n]) + ", label width: " + (this.scalingSteps[n] * this.scalingWidths[n]));
        }

        this.navOnColour = Color.green;
        this.navOffColour = Color.white;
        this.navDisColour = Color.gray;
        this.navHiColour = Color.red;

        this.TAB1LABEL = "Live Timing";
        this.TAB2LABEL = "Lap Chart";
        this.TAB3LABEL = "Weather & Speed";

        this.BORDERSIZE = 5;
        this.COMMENTPAD = 5;

        this.COMMENTHEAD = 0;
        this.COMMENTTAIL = (this.linkBoxEntries * 25);

        this.SCROLLBAR_WIDTH = 15;

        this.bgColourC = new Color(15724527);
    }

    public void initMode(int c, boolean leaveCommentary) {

        debug("Race mode: " + Integer.toString(c) + ", " + leaveCommentary);

        this.raceMode = c;

        if(!(leaveCommentary)) {
            this.scrollLine = 0;
            this.scrolling = '\0';
            this.scrollbaring = false;
        } else {
            this.noCommentaryScroll = true;
        }

        this.commentaryLines = new String[4096];
        this.nCommentaryLines = 0;
        initScreenC();

        this.timeNow = 0;

        if((!(this.modeSet)) || (this.raceMode != c)) {
            if(c == 1)
                initRaceMode();
            else if(c == 3) {
                if(this.season < 2005)
                    initPracticeMode();
                else
                    initQualifyingMode();
            } else if(c == 4)
                initQualifyingMode(1);
            else if(c == 5)
                initQualifyingMode(2);
            else {
                initPracticeMode();
            }
        }
        this.modeSet = true;
    }

    private void initRaceMode() {

        int mx = 100;
        if(this.bigScreenLayout) {
            mx = 145;
        }
        this.nCols = 13;
        this.colWidths = new int[this.nCols];
        this.colWidths[0] = (18 * mx / 100);
        this.colWidths[1] = (18 * mx / 100);
        this.colWidths[2] = (97 * mx / 100);
        this.colWidths[3] = (29 * mx / 100);
        this.colWidths[4] = (31 * mx / 100);
        this.colWidths[5] = (50 * mx / 100);
        this.colWidths[6] = (32 * mx / 100);
        this.colWidths[7] = (22 * mx / 100);
        this.colWidths[8] = (32 * mx / 100);
        this.colWidths[9] = (22 * mx / 100);
        this.colWidths[10] = (32 * mx / 100);
        this.colWidths[11] = (22 * mx / 100);
        this.colWidths[12] = (11 * mx / 100);

        this.colFastest = new int[this.nCols];
        this.colFastest[0] = (36 * mx / 100);
        this.colFastest[1] = (99 * mx / 100);
        this.colFastest[2] = (20 * mx / 100);
        this.colFastest[3] = (95 * mx / 100);
        this.colFastest[4] = (46 * mx / 100);
        this.colFastest[5] = (56 * mx / 100);
        this.colFastest[6] = (54 * mx / 100);
        this.colFastest[7] = (10 * mx / 100);
        this.colFastest[8] = (10 * mx / 100);
        this.colFastest[9] = (10 * mx / 100);
        this.colFastest[10] = (10 * mx / 100);
        this.colFastest[11] = (10 * mx / 100);
        this.colFastest[12] = (10 * mx / 100);

        initScreen1();
        initScreen2();
        if(this.weather) {
            initScreen3();
        }

        this.scaling = 0;
        this.gLapNo = 1;
    }

    private void initQualifyingMode() {

        initQualifyingMode(0);
    }

    private void initQualifyingMode(int qType) {

        int mx = 100;
        if(this.bigScreenLayout) {
            mx = 145;
        }
        switch(qType) {
            case 0:
                if(this.season >= 2006) {
                    this.nCols = 10;
                    this.colWidths = new int[this.nCols];
                    this.colWidths[0] = (20 * mx / 100);
                    this.colWidths[1] = (20 * mx / 100);
                    this.colWidths[2] = (100 * mx / 100);
                    this.colWidths[3] = (48 * mx / 100);
                    this.colWidths[4] = (48 * mx / 100);
                    this.colWidths[5] = (48 * mx / 100);
                    this.colWidths[6] = (37 * mx / 100);
                    this.colWidths[7] = (37 * mx / 100);
                    this.colWidths[8] = (37 * mx / 100);
                    this.colWidths[9] = (20 * mx / 100);
                } else {
                    this.nCols = 10;
                    this.colWidths = new int[this.nCols];
                    this.colWidths[0] = (20 * mx / 100);
                    this.colWidths[1] = (20 * mx / 100);
                    this.colWidths[2] = (100 * mx / 100);
                    this.colWidths[3] = (55 * mx / 100);
                    this.colWidths[4] = (50 * mx / 100);
                    this.colWidths[5] = (30 * mx / 100);
                    this.colWidths[6] = (40 * mx / 100);
                    this.colWidths[7] = (40 * mx / 100);
                    this.colWidths[8] = (40 * mx / 100);
                    this.colWidths[9] = (20 * mx / 100);
                }
                break;
            case 1:
                this.nCols = 9;
                this.colWidths = new int[this.nCols];
                this.colWidths[0] = (20 * mx / 100);
                this.colWidths[1] = (20 * mx / 100);
                this.colWidths[2] = (100 * mx / 100);
                this.colWidths[3] = (55 * mx / 100);
                this.colWidths[4] = (50 * mx / 100);
                this.colWidths[5] = (50 * mx / 100);
                this.colWidths[6] = (50 * mx / 100);
                this.colWidths[7] = (50 * mx / 100);
                this.colWidths[8] = (20 * mx / 100);
                break;
            case 2:
                this.nCols = 11;
                this.colWidths = new int[this.nCols];
                this.colWidths[0] = (20 * mx / 100);
                this.colWidths[1] = (20 * mx / 100);
                this.colWidths[2] = (100 * mx / 100);
                this.colWidths[3] = (45 * mx / 100);
                this.colWidths[4] = (39 * mx / 100);
                this.colWidths[5] = (35 * mx / 100);
                this.colWidths[6] = (35 * mx / 100);
                this.colWidths[7] = (35 * mx / 100);
                this.colWidths[8] = (46 * mx / 100);
                this.colWidths[9] = (20 * mx / 100);
                this.colWidths[10] = (20 * mx / 100);
        }

        this.alignedL[7] = 0;
        this.alignedL[9] = 0;

        initScreen1();
        if(this.weather)
            initScreen3();
    }

    private void initPracticeMode() {

        int mx = 100;
        if(this.bigScreenLayout) {
            mx = 145;
        }
        this.nCols = 10;
        this.colWidths = new int[this.nCols];
        this.colWidths[0] = (20 * mx / 100);
        this.colWidths[1] = (20 * mx / 100);
        this.colWidths[2] = (100 * mx / 100);
        this.colWidths[3] = (55 * mx / 100);
        this.colWidths[4] = (50 * mx / 100);
        this.colWidths[5] = (40 * mx / 100);
        this.colWidths[6] = (40 * mx / 100);
        this.colWidths[7] = (40 * mx / 100);
        this.colWidths[8] = (30 * mx / 100);
        this.colWidths[9] = (20 * mx / 100);

        this.alignedL[7] = 0;
        this.alignedL[9] = 0;

        initScreen1();
        if(this.weather)
            initScreen3();
    }

    public void drawScroll(Graphics g, char updown, int state) {

        Color fgColour;
        g.setColor(new Color(15658734));

        int BOTTOMOFF = this.HEIGHT - 23;
        switch(state) {
            case 0:
                fgColour = new Color(13421772);
                break;
            case 1:
                fgColour = Color.red;
                break;
            case 2:
            default:
                fgColour = Color.black;
        }

        g.setColor(fgColour);

        switch(updown) {
            case '\1':
                g.drawLine(this.CWIDTH - 16 + 0, 34, this.CWIDTH - 16 + 0, 32);
                g.drawLine(this.CWIDTH - 16 + 1, 33, this.CWIDTH - 16 + 1, 30);
                g.drawLine(this.CWIDTH - 16 + 2, 31, this.CWIDTH - 16 + 2, 28);
                g.drawLine(this.CWIDTH - 16 + 3, 29, this.CWIDTH - 16 + 3, 27);
                g.drawLine(this.CWIDTH - 16 + 4, 31, this.CWIDTH - 16 + 4, 28);
                g.drawLine(this.CWIDTH - 16 + 5, 33, this.CWIDTH - 16 + 5, 30);
                g.drawLine(this.CWIDTH - 16 + 6, 34, this.CWIDTH - 16 + 6, 32);
                g.drawLine(this.CWIDTH - 16 + 0, 42, this.CWIDTH - 16 + 0, 40);
                g.drawLine(this.CWIDTH - 16 + 1, 41, this.CWIDTH - 16 + 1, 38);
                g.drawLine(this.CWIDTH - 16 + 2, 39, this.CWIDTH - 16 + 2, 36);
                g.drawLine(this.CWIDTH - 16 + 3, 37, this.CWIDTH - 16 + 3, 35);
                g.drawLine(this.CWIDTH - 16 + 4, 39, this.CWIDTH - 16 + 4, 36);
                g.drawLine(this.CWIDTH - 16 + 5, 41, this.CWIDTH - 16 + 5, 38);
                g.drawLine(this.CWIDTH - 16 + 6, 42, this.CWIDTH - 16 + 6, 40);

                break;
            case '\2':
                g.drawLine(this.CWIDTH - 16 + 0, BOTTOMOFF + 8, this.CWIDTH - 16 + 0, BOTTOMOFF + 10);
                g.drawLine(this.CWIDTH - 16 + 1, BOTTOMOFF + 9, this.CWIDTH - 16 + 1, BOTTOMOFF + 12);
                g.drawLine(this.CWIDTH - 16 + 2, BOTTOMOFF + 11, this.CWIDTH - 16 + 2, BOTTOMOFF + 14);
                g.drawLine(this.CWIDTH - 16 + 3, BOTTOMOFF + 13, this.CWIDTH - 16 + 3, BOTTOMOFF + 15);
                g.drawLine(this.CWIDTH - 16 + 4, BOTTOMOFF + 11, this.CWIDTH - 16 + 4, BOTTOMOFF + 14);
                g.drawLine(this.CWIDTH - 16 + 5, BOTTOMOFF + 9, this.CWIDTH - 16 + 5, BOTTOMOFF + 12);
                g.drawLine(this.CWIDTH - 16 + 6, BOTTOMOFF + 8, this.CWIDTH - 16 + 6, BOTTOMOFF + 10);
                g.drawLine(this.CWIDTH - 16 + 0, BOTTOMOFF + 0, this.CWIDTH - 16 + 0, BOTTOMOFF + 2);
                g.drawLine(this.CWIDTH - 16 + 1, BOTTOMOFF + 1, this.CWIDTH - 16 + 1, BOTTOMOFF + 4);
                g.drawLine(this.CWIDTH - 16 + 2, BOTTOMOFF + 3, this.CWIDTH - 16 + 2, BOTTOMOFF + 6);
                g.drawLine(this.CWIDTH - 16 + 3, BOTTOMOFF + 5, this.CWIDTH - 16 + 3, BOTTOMOFF + 7);
                g.drawLine(this.CWIDTH - 16 + 4, BOTTOMOFF + 3, this.CWIDTH - 16 + 4, BOTTOMOFF + 6);
                g.drawLine(this.CWIDTH - 16 + 5, BOTTOMOFF + 1, this.CWIDTH - 16 + 5, BOTTOMOFF + 4);
                g.drawLine(this.CWIDTH - 16 + 6, BOTTOMOFF + 0, this.CWIDTH - 16 + 6, BOTTOMOFF + 2);
        }

        redrawCommentary();
    }

    public void initScreenC() {

        debug("Init Screen C");

        Graphics g = this.offscreenC.getGraphics();
        g.setColor(Color.white);
        g.fillRect(0, 0, this.CWIDTH, this.HEIGHT);

        if(this.commentaryImage != null) {
            g.drawImage(this.commentaryImage, 2, 2, this);
        }

        if(!(this.inlineLayout)) {
            g.setColor(this.navHiColour);
            g.drawLine(3, 21, this.CWIDTH - this.BORDERSIZE + 3 - 10, 21);
            g.drawLine(3, 21, 3, this.HEIGHT - 10 - 1);
            g.drawLine(13, this.HEIGHT - 1, this.CWIDTH - (this.BORDERSIZE - 3) - 10, this.HEIGHT - 1);
            g.drawLine(this.CWIDTH - 3 - 1, 31, this.CWIDTH - 3 - 1, this.HEIGHT - 10 - 1);

            g.drawArc(this.CWIDTH - this.BORDERSIZE + 3 - 20 + 1, 21, 20, 20, 0, 90);
            g.drawArc(3, this.HEIGHT - 20 - 1, 20, 20, 180, 90);
            g.drawArc(this.CWIDTH - (this.BORDERSIZE - 3) - 20 + 1, this.HEIGHT - 1 - 20, 20, 20, 270, 90);

            drawScroll(g, '\1', 0);
            drawScroll(g, '\2', 0);

            g.setColor(Color.white);
            g.setFont(this.commentaryHeadFont);

            g.drawString("Commentary", (this.CWIDTH - g.getFontMetrics().stringWidth("Commentary")) / 2, 14);
        }

        g.setFont(this.commentaryFont);
        this.commentaryHeight = g.getFontMetrics().getHeight();
        this.commentaryRows = ((this.HEIGHT - 2 - this.COMMENTHEAD - this.COMMENTTAIL) / (this.commentaryHeight + 1));
        this.commentaryOffset = ((this.HEIGHT - 2 - this.COMMENTHEAD - this.COMMENTTAIL - (this.commentaryRows * (this.commentaryHeight + 1)) - 1) / 2);

        g.setColor(this.bgColourC);
        g.fillRect(this.BORDERSIZE, this.COMMENTHEAD, this.CWIDTH - (this.BORDERSIZE * 2) - this.SCROLLBAR_WIDTH, this.HEIGHT - this.COMMENTHEAD - this.COMMENTTAIL);

        if((this.inlineLayout) && (this.linkBoxEntries > 0)) {
            this.linkBoxColour = new Color(11141120);
            this.linkBoxFont = new Font("SansSerif", 1, 11);
            g.setFont(this.linkBoxFont);
            FontMetrics fm = g.getFontMetrics();
            for(int n = 0; n < this.linkBoxEntries; ++n) {
                g.setColor(Color.black);
                g.drawString("»", this.BORDERSIZE, this.HEIGHT - ((this.linkBoxEntries - n) * 25) + (25 - fm.getHeight()) / 2 + fm.getAscent() - 1);
                if(this.linkBoxName[n] != null) {
                    g.setColor(this.linkBoxColour);
                    g.drawString(this.linkBoxName[n], this.BORDERSIZE + 10, this.HEIGHT - ((this.linkBoxEntries - n) * 25) + (25 - fm.getHeight()) / 2 + fm.getAscent());
                }
            }
        }

        g.dispose();

        g = this.offscreen.getGraphics();
        g.drawImage(this.offscreenC, this.WIDTH - this.CWIDTH, 0, this);
        g.dispose();

        if(this.inlineLayout) {
            this.scrollbar = new f1scrollbar(this, this.offscreenC, new Rectangle(this.CWIDTH - this.BORDERSIZE - this.SCROLLBAR_WIDTH, this.COMMENTHEAD, this.SCROLLBAR_WIDTH, this.HEIGHT - this.COMMENTHEAD - this.COMMENTTAIL));
            doScrollBar(true);
        }
    }

    private void initButtons1(Graphics g) {

        g.setColor(Color.white);
        g.fillRect(0, 0, this.buttons1.getWidth(this), this.HEIGHT);
        g.drawImage(this.buttons1, 0, 0, this);

        g.setFont(this.tabFont);
        g.setColor(this.navOnColour);
        drawVText(this.offscreen1, g, this.TABSIZE - 2, this.TABHEIGHT - this.TABFUDGE, 1, this.TABFUDGE / 2 + 1, this.TAB1LABEL);
        if(this.raceMode != 1)
            g.setColor(this.navDisColour);
        else {
            g.setColor(this.navOffColour);
        }
        drawVText(this.offscreen1, g, this.TABSIZE - 2, this.TABHEIGHT - this.TABFUDGE, 1, this.TABHEIGHT * 2 + this.TABFUDGE / 2 + 1, this.TAB2LABEL);
        if(!(this.weather))
            g.setColor(this.navDisColour);
        else {
            g.setColor(this.navOffColour);
        }
        drawVText(this.offscreen1, g, this.TABSIZE - 2, this.TABHEIGHT - this.TABFUDGE, 1, this.TABHEIGHT + this.TABFUDGE / 2 + 1, this.TAB3LABEL);
    }

    private void initButtons2(Graphics g) {

        g.setColor(Color.white);
        g.fillRect(0, 0, this.buttons1.getWidth(this), this.HEIGHT);
        g.drawImage(this.buttons3, 0, 0, this);

        g.setFont(this.tabFont);
        g.setColor(this.navOffColour);
        drawVText(this.offscreen2, g, this.TABSIZE - 2, this.TABHEIGHT - this.TABFUDGE, 1, this.TABFUDGE / 2 + 1, this.TAB1LABEL);
        g.setColor(this.navOnColour);
        drawVText(this.offscreen2, g, this.TABSIZE - 2, this.TABHEIGHT - this.TABFUDGE, 1, this.TABHEIGHT * 2 + this.TABFUDGE / 2 + 1, this.TAB2LABEL);
        if(!(this.weather))
            g.setColor(this.navDisColour);
        else {
            g.setColor(this.navOffColour);
        }
        drawVText(this.offscreen2, g, this.TABSIZE - 2, this.TABHEIGHT - this.TABFUDGE, 1, this.TABHEIGHT + this.TABFUDGE / 2 + 1, this.TAB3LABEL);
    }

    private void initButtons3(Graphics g) {

        g.setColor(Color.white);
        g.fillRect(0, 0, this.buttons1.getWidth(this), this.HEIGHT);
        g.drawImage(this.buttons2, 0, 0, this);

        g.setFont(this.tabFont);
        g.setColor(this.navOffColour);
        drawVText(this.offscreen3, g, this.TABSIZE - 2, this.TABHEIGHT - this.TABFUDGE, 1, this.TABFUDGE / 2 + 1, this.TAB1LABEL);
        g.setColor(this.navOnColour);
        drawVText(this.offscreen3, g, this.TABSIZE - 2, this.TABHEIGHT - this.TABFUDGE, 1, this.TABHEIGHT + this.TABFUDGE / 2 + 1, this.TAB3LABEL);
        if(this.raceMode != 1)
            g.setColor(this.navDisColour);
        else {
            g.setColor(this.navOffColour);
        }
        drawVText(this.offscreen3, g, this.TABSIZE - 2, this.TABHEIGHT - this.TABFUDGE, 1, this.TABHEIGHT * 2 + this.TABFUDGE / 2 + 1, this.TAB2LABEL);
    }

    private void initButtons() {

        Graphics g = this.offscreen1.getGraphics();
        initButtons1(g);
        g.dispose();
        if(this.raceMode == 1) {
            g = this.offscreen2.getGraphics();
            initButtons2(g);
            g.dispose();
        }
        if(this.weather) {
            g = this.offscreen3.getGraphics();
            initButtons3(g);
            g.dispose();
        }
    }

    private void initCopyrightMessage(Graphics g) {

        if(this.season >= 2009) {
            g.setColor(Color.white);
            g.setFont(this.dataFont);
        } else {
            g.setColor(this.headColour);
            g.setFont(this.copyrightFont);
        }
        FontMetrics fm = g.getFontMetrics();

        g.drawString(copyrightMessage, this.TABSIZE + (this.WIDTH - this.CWIDTH - this.TABSIZE - fm.stringWidth(copyrightMessage)) / 2, this.HEIGHT - this.COPYRIGHTHEIGHT + (this.COPYRIGHTHEIGHT - fm.getHeight()) / 2 + fm.getAscent());
        if(this.season >= 2009)
            if(this.screenSize == 25)
                g.drawImage(this.sponsorLogo, 480, this.HEIGHT - this.COPYRIGHTHEIGHT + (this.COPYRIGHTHEIGHT - this.sponsorLogo.getHeight(this)) / 2, this);
            else
                g.drawImage(this.sponsorLogo, 330, this.HEIGHT - this.COPYRIGHTHEIGHT + (this.COPYRIGHTHEIGHT - this.sponsorLogo.getHeight(this)) / 2, this);
    }

    private void initScreen1() {

        debug("Init Screen 1");

        Graphics g = this.offscreen1.getGraphics();

        g.setColor(Color.white);
        g.fillRect(0, 0, this.WIDTH - this.CWIDTH, this.HEIGHT);
        g.setColor(this.bgColour1);
        g.fillRect(this.TABSIZE, 0, this.WIDTH - this.CWIDTH - this.TABSIZE, this.HEIGHT);

        initCopyrightMessage(g);

        initButtons1(g);

        g.dispose();

        if(this.raceMode == 1) {
            updateHeader1(this.offscreen1, "P", 1, this.headColour, 0);
            updateHeader1(this.offscreen1, "Name", 3, this.headColour, 1);
            updateHeader1(this.offscreen1, "Gap", 4, this.headColour, 0);
            updateHeader1(this.offscreen1, "Interval", 5, this.headColour, 1);
            updateHeader1(this.offscreen1, "Sector 1", 7, this.headColour, 2);
            updateHeader1(this.offscreen1, "Sector 2", 9, this.headColour, 2);
            updateHeader1(this.offscreen1, "Sector 3", 11, this.headColour, 2);
            updateHeader1(this.offscreen1, "Pit", 13, this.headColour, 0);
        } else if(this.raceMode == 3) {
            if(this.season >= 2006) {
                updateHeader1(this.offscreen1, "P", 1, this.headColour, 0);
                updateHeader1(this.offscreen1, "Name", 3, this.headColour, 1);

                updateHeader1(this.offscreen1, "Q1", 4, this.headColour, 2);
                updateHeader1(this.offscreen1, "Q2", 5, this.headColour, 2);
                updateHeader1(this.offscreen1, "Q3", 6, this.headColour, 2);

                updateHeader1(this.offscreen1, "S1  ", 7, this.headColour, 0);
                updateHeader1(this.offscreen1, "S2  ", 8, this.headColour, 0);
                updateHeader1(this.offscreen1, "S3  ", 9, this.headColour, 0);
                updateHeader1(this.offscreen1, "L", 10, this.headColour, 2);
            } else if(this.season < 2005) {
                updateHeader1(this.offscreen1, "P", 1, this.headColour, 0);
                updateHeader1(this.offscreen1, "Name", 3, this.headColour, 1);
                updateHeader1(this.offscreen1, "Best", 4, this.headColour, 0);
                updateHeader1(this.offscreen1, "Gap", 5, this.headColour, 0);
                updateHeader1(this.offscreen1, "S1", 6, this.headColour, 2);
                updateHeader1(this.offscreen1, "S2", 7, this.headColour, 2);
                updateHeader1(this.offscreen1, "S3", 8, this.headColour, 2);
                updateHeader1(this.offscreen1, "Laps", 9, this.headColour, 2);
            } else {
                updateHeader1(this.offscreen1, "P", 1, this.headColour, 0);
                updateHeader1(this.offscreen1, "Name", 3, this.headColour, 1);
                updateHeader1(this.offscreen1, "Best", 4, this.headColour, 2);
                updateHeader1(this.offscreen1, "Gap", 5, this.headColour, 0);
                updateHeader1(this.offscreen1, "Q1", 6, this.headColour, 2);
                updateHeader1(this.offscreen1, "S1", 7, this.headColour, 2);
                updateHeader1(this.offscreen1, "S2", 8, this.headColour, 2);
                updateHeader1(this.offscreen1, "S3", 9, this.headColour, 2);
                updateHeader1(this.offscreen1, "Laps", 10, this.headColour, 2);
            }
        } else if(this.raceMode == 4) {
            updateHeader1(this.offscreen1, "P", 1, this.headColour, 0);
            updateHeader1(this.offscreen1, "Name", 3, this.headColour, 1);
            updateHeader1(this.offscreen1, "Best", 4, this.headColour, 2);
            updateHeader1(this.offscreen1, "Gap ", 5, this.headColour, 0);
            updateHeader1(this.offscreen1, "S1  ", 6, this.headColour, 0);
            updateHeader1(this.offscreen1, "S2  ", 7, this.headColour, 0);
            updateHeader1(this.offscreen1, "S3  ", 8, this.headColour, 0);
            updateHeader1(this.offscreen1, "Laps", 9, this.headColour, 2);
        } else if(this.raceMode == 5) {
            updateHeader1(this.offscreen1, "P", 1, this.headColour, 0);
            updateHeader1(this.offscreen1, "Name", 3, this.headColour, 1);
            updateHeader1(this.offscreen1, "Best", 4, this.headColour, 2);
            updateHeader1(this.offscreen1, "Q1", 5, this.headColour, 2);
            updateHeader1(this.offscreen1, "S1  ", 6, this.headColour, 0);
            updateHeader1(this.offscreen1, "S2  ", 7, this.headColour, 0);
            updateHeader1(this.offscreen1, "S3  ", 8, this.headColour, 0);
            updateHeader1(this.offscreen1, "Q2 time", 9, this.headColour, 0);
            updateHeader1(this.offscreen1, "P", 10, this.headColour, 0);
            updateHeader1(this.offscreen1, "L", 11, this.headColour, 0);
        } else {
            updateHeader1(this.offscreen1, "P", 1, this.headColour, 0);
            updateHeader1(this.offscreen1, "Name", 3, this.headColour, 1);
            updateHeader1(this.offscreen1, "Best", 4, this.headColour, 0);
            updateHeader1(this.offscreen1, "Gap", 5, this.headColour, 0);
            updateHeader1(this.offscreen1, "S1", 6, this.headColour, 2);
            updateHeader1(this.offscreen1, "S2", 7, this.headColour, 2);
            updateHeader1(this.offscreen1, "S3", 8, this.headColour, 2);
            updateHeader1(this.offscreen1, "Laps", 9, this.headColour, 2);
        }
    }

    private void initScreen2() {

        debug("Init Screen 2");

        if(this.offscreen2 == null) {
            this.offscreen2 = createImage(this.WIDTH - this.CWIDTH, this.HEIGHT);
            this.offscreen2Pixels = new int[(this.WIDTH - this.CWIDTH) * this.HEIGHT];
        }

        this.gRow = new byte[32][100];
        this.gLap = new byte[32];
        this.gName = new String[32];

        Graphics g = this.offscreen2.getGraphics();

        g.setColor(this.bgColour2);
        g.fillRect(0, 0, this.WIDTH - this.CWIDTH, this.HEIGHT);

        g.setColor(this.bgColour2a);
        g.fillRect(this.TABSIZE + this.SCREEN2LABELWIDTH - 2, this.CELLTOP, this.WIDTH - this.CWIDTH - this.TABSIZE - this.SCREEN2LABELWIDTH, this.CELLHEIGHT * 8);

        initCopyrightMessage(g);

        initButtons2(g);

        g.dispose();
    }

    private void checkScreen3Init() {

        if(!(this.weather)) {
            initScreen3();
            this.weather = true;
            initButtons();
        }
    }

    private void initScreen3() {

        debug("Init Screen 3");

        if(this.offscreen3 == null) {
            this.offscreen3 = createImage(this.WIDTH - this.CWIDTH, this.HEIGHT);
        }

        if(this.wGraphs == null) {
            this.wGraphs = new Image[6];
        }

        if(this.gTitles == null) {
            this.gTitles = new String[6];
            this.gTitles[0] = "TRACK TEMP";
            this.gTitles[1] = "AIR TEMP";
            this.gTitles[2] = "WET/DRY";
            this.gTitles[3] = "WIND SPEED";
            this.gTitles[4] = "HUMIDITY";
            this.gTitles[5] = "PRESSURE";
        }

        this.graphMax = new int[6];
        this.graphMaxLabels = new String[6];
        this.graphMin = new int[6];
        this.graphMinLabels = new String[6];

        this.graphMin[0] = 0;
        this.graphMin[1] = 0;
        this.graphMin[2] = 0;
        this.graphMin[3] = 0;
        this.graphMin[4] = -1;
        this.graphMin[5] = -1;

        this.graphData = new f1list[6];
        this.graphTime = new f1list[6];
        this.wGColours = new Color[7];

        for(int n = 0; n < 6; ++n) {
            if(this.wGraphs[n] == null)
                this.wGraphs[n] = createImage(this.GWIDTH, this.GHEIGHT);
            g = this.wGraphs[n].getGraphics();
            g.setColor(this.bgColour3);
            g.fillRect(0, 0, this.GWIDTH, this.GHEIGHT);
            g.dispose();
            this.graphMax[n] = (this.graphMin[n] + 100);

            this.graphMaxLabels[n] = Integer.toString(this.graphMax[n] / 100);
            this.graphMinLabels[n] = Integer.toString(this.graphMin[n] / 100);

            this.graphData[n] = new f1list();
            this.graphTime[n] = new f1list();
        }

        this.wGColours[0] = Color.yellow;
        this.wGColours[1] = Color.magenta;
        this.wGColours[2] = Color.cyan;
        this.wGColours[3] = Color.green;
        this.wGColours[4] = Color.white;
        this.wGColours[5] = Color.orange;
        this.wGColours[6] = Color.red;

        this.screenGap = ((this.HEIGHT - this.COPYRIGHTHEIGHT - (this.CELLHEIGHT * 2) - (this.CELLHEIGHT * 13 + this.CELLTOP * 3 + 80)) / 4);

        debug("screenGap=" + this.screenGap);

        Graphics g = this.offscreen3.getGraphics();

        g.setColor(this.bgColour3);
        g.fillRect(0, 0, this.WIDTH - this.CWIDTH, this.HEIGHT);

        initCopyrightMessage(g);

        initButtons3(g);

        g.setFont(this.headFont);
        FontMetrics fm = g.getFontMetrics();

        g.setColor(this.headColour);
        g.drawString("SPEED (kph)", this.TABSIZE + this.SCREEN3SPACE, this.screenGap + this.CELLTOPMAGIC + (this.CELLHEIGHT - fm.getHeight()) / 2 + fm.getAscent());
        g.drawString("WEATHER", this.TABSIZE + this.SCREEN3SPACE, this.screenGap * 2 + this.CELLTOP + this.CELLHEIGHT * 7 + this.CELLTOPMAGIC + (this.CELLHEIGHT - fm.getHeight()) / 2 + fm.getAscent());
        g.drawString("WIND SOURCE", this.TABSIZE + this.SCREEN3SPACE, this.screenGap * 3 + this.CELLTOP * 2 + this.CELLHEIGHT * 13 + this.CELLTOPMAGIC + (this.CELLHEIGHT - fm.getHeight()) / 2 + fm.getAscent());

        for(int n = 0; n < 4; ++n) {
            g.drawString("Driver", this.TABSIZE + this.SCREEN3SPACE + (this.SCREEN3SPACE + 90) * n, this.screenGap + this.CELLTOP + (this.CELLHEIGHT - fm.getHeight()) / 2 + fm.getAscent() - 2);
            if(n == 3)
                g.drawString("Trap", this.TABSIZE + this.SCREEN3SPACE + 45 + (this.SCREEN3SPACE + 90) * n, this.screenGap + this.CELLTOP + (this.CELLHEIGHT - fm.getHeight()) / 2 + fm.getAscent() - 2);
            else {
                g.drawString("Sector " + Integer.toString(n + 1), this.TABSIZE + this.SCREEN3SPACE + 45 + (this.SCREEN3SPACE + 90) * n, this.screenGap + this.CELLTOP + (this.CELLHEIGHT - fm.getHeight()) / 2 + fm.getAscent() - 2);
            }
        }

        for(int n = 0; n < 6; ++n) {
            g.setColor(this.wGColours[n]);
            g.drawString(this.gTitles[n], this.TABSIZE + this.SCREEN3SPACE, this.screenGap * 2 + this.CELLTOP * 2 + this.CELLHEIGHT * (7 + n) + (this.CELLHEIGHT - fm.getHeight()) / 2 + fm.getAscent());
        }

        g.drawImage(this.circuitImage, this.TABSIZE + this.SCREEN3SPACE + (100 - this.circuitImage.getWidth(this)) / 2, this.screenGap * 3 + this.CELLTOP * 3 + this.CELLHEIGHT * 13 + (80 - this.circuitImage.getHeight(this)) / 2, this);

        g.setColor(Color.white);
        g.drawLine(this.WIDTH - this.CWIDTH - this.SCREEN3SPACE - this.GWIDTH - 1, this.screenGap * 2 + this.CELLTOP * 2 + this.CELLHEIGHT * 7, this.WIDTH - this.CWIDTH - this.SCREEN3SPACE - this.GWIDTH - 1, this.screenGap * 2 + this.CELLTOP * 2 + this.CELLHEIGHT * 7 + this.GHEIGHT);
        g.drawLine(this.WIDTH - this.CWIDTH - this.SCREEN3SPACE - this.GWIDTH - 1, this.screenGap * 2 + this.CELLTOP * 2 + this.CELLHEIGHT * 7 + this.GHEIGHT, this.WIDTH - this.CWIDTH - this.SCREEN3SPACE - 1, this.screenGap * 2 + this.CELLTOP * 2 + this.CELLHEIGHT * 7 + this.GHEIGHT);

        drawWGraph(g, 0);

        this.timePeriod = 0;
        drawWLabel(g);

        g.dispose();

        if(this.weatherTicker == null)
            this.weatherTicker = new f1weather(this);
    }

    private double toRad(int angle) {

        return (6.283185307179586D * angle / 360.0D);
    }

    private int twoDP(String strValue) {

        try {
            if(strValue.indexOf(".") == -1) {
                return (Integer.parseInt(strValue) * 100);
            }
            return (int)(Double.valueOf(strValue).doubleValue() * 100.0D);
        } catch (NumberFormatException e) {
        }
        return -1;
    }

    public void updateScreen3Weather(int c, String strValue) {

        checkScreen3Init();

        Graphics g = this.offscreen3.getGraphics();

        if(c == 7) {
            int angle;
            try {
                angle = Integer.parseInt(strValue);
            } catch (NumberFormatException e) {
                g.dispose();
                return;
            }

            g.setColor(this.bgColour3);
            g.fillRect(this.TABSIZE + this.SCREEN3SPACE, this.screenGap * 3 + this.CELLTOP * 3 + this.CELLHEIGHT * 13, 100, 80);
            g.drawImage(this.circuitImage, this.TABSIZE + this.SCREEN3SPACE + (100 - this.circuitImage.getWidth(this)) / 2, this.screenGap * 3 + this.CELLTOP * 3 + this.CELLHEIGHT * 13 + (80 - this.circuitImage.getHeight(this)) / 2, this);

            int cx = this.TABSIZE + this.SCREEN3SPACE + 50;
            int cy = this.screenGap * 3 + this.CELLTOP * 3 + this.CELLHEIGHT * 13 + 40;

            int px1 = cx + (int)(Math.sin(toRad(angle)) * 60.0D / 2.0D);
            int py1 = cy - (int)(Math.cos(toRad(angle)) * 60.0D / 2.0D);

            int px4 = cx - (int)(Math.sin(toRad(angle)) * 60.0D / 4.0D);
            int py4 = cy + (int)(Math.cos(toRad(angle)) * 60.0D / 4.0D);

            angle -= 160;
            if(angle < 0)
                angle += 360;

            int px2 = cx + (int)(Math.sin(toRad(angle)) * 60.0D / 2.0D);
            int py2 = cy - (int)(Math.cos(toRad(angle)) * 60.0D / 2.0D);

            angle -= 40;
            if(angle < 0)
                angle += 360;

            int px3 = cx + (int)(Math.sin(toRad(angle)) * 60.0D / 2.0D);
            int py3 = cy - (int)(Math.cos(toRad(angle)) * 60.0D / 2.0D);

            int[] xPoints = { px4, px1, px2 };
            int[] yPoints = { py4, py1, py2 };
            g.setColor(this.arrowColour1);
            g.fillPolygon(xPoints, yPoints, 3);

            xPoints = new int[] { px4, px3, px2 };
            yPoints = new int[] { py4, py3, py2 };
            g.setColor(this.arrowColour3);
            g.fillPolygon(xPoints, yPoints, 3);

            xPoints = new int[] { px4, px1, px3 };
            yPoints = new int[] { py4, py1, py3 };
            g.setColor(this.arrowColour2);
            g.fillPolygon(xPoints, yPoints, 3);
        } else {
            g.setColor(this.bgColour3);
            g.fillRect(105 + this.SCREEN3SPACE, this.screenGap * 2 + this.CELLTOP * 2 + this.CELLHEIGHT * (6 + c), 80, this.CELLHEIGHT);

            g.setFont(this.dataFont);
            FontMetrics fm = g.getFontMetrics();

            String extra = "";

            switch(c) {
                case 1:
                    extra = "°C";
                    break;
                case 2:
                    extra = "°C";
                    break;
                case 3:
                    break;
                case 4:
                    extra = " mps";
                    break;
                case 5:
                    extra = "%";
                    break;
                case 6:
                    extra = " mBar";
                    break;
                default:
                    extra = "";
            }
            g.setColor(this.wGColours[(c - 1)]);
            g.drawString(strValue + extra, 105 + this.SCREEN3SPACE, this.screenGap * 2 + this.CELLTOP * 2 + this.CELLHEIGHT * (6 + c) + (this.CELLHEIGHT - fm.getHeight()) / 2 + fm.getAscent());

            if(c <= 6) {
                int val = twoDP(strValue);

                this.graphData[(c - 1)].add(val);
                this.graphTime[(c - 1)].add(this.timeNow);

                if(this.graphMin[(c - 1)] == -1) {
                    debug("Setting graphMin[" + (c - 1) + "] to " + val);
                    this.graphMin[(c - 1)] = val;
                    this.graphMax[(c - 1)] = (val + 1);
                    this.graphMinLabels[(c - 1)] = strValue;
                    this.graphMaxLabels[(c - 1)] = strValue;
                } else {
                    if(val > this.graphMax[(c - 1)]) {
                        this.graphMax[(c - 1)] = val;
                        this.graphMaxLabels[(c - 1)] = strValue;
                    }
                    if(val < this.graphMin[(c - 1)]) {
                        this.graphMin[(c - 1)] = val;
                        this.graphMinLabels[(c - 1)] = strValue;
                    }
                }
                updateWGraph(c - 1);
            }
        }
        g.dispose();

        if(this.screenNum == 3) {
            redrawOffscreen(false);
            repaint(100L);
        }
    }

    private void drawWLabel() {

        Graphics g = this.offscreen3.getGraphics();
        drawWLabel(g);
        g.dispose();
    }

    private void drawWLabel(Graphics g) {

        g.setFont(this.headFont);
        FontMetrics fm = g.getFontMetrics();

        g.setColor(this.bgColour3);

        g.fillRect(this.WIDTH - this.CWIDTH - this.SCREEN3SPACE - this.GWIDTH, this.screenGap * 2 + this.CELLTOP + this.CELLHEIGHT * 7 + this.CELLTOP + this.GHEIGHT + 1, this.GWIDTH, this.CELLHEIGHT);

        g.setColor(Color.white);

        g.drawString(this.timeLabels[this.timePeriod], this.WIDTH - this.CWIDTH - this.SCREEN3SPACE - this.GWIDTH + (this.GWIDTH - fm.stringWidth(this.timeLabels[this.timePeriod])) / 2, this.screenGap * 2 + this.CELLTOP + this.CELLHEIGHT * 7 + this.CELLTOP + this.GHEIGHT + 1 + (this.CELLHEIGHT - fm.getHeight()) / 2 + fm.getAscent());
    }

    public void drawWGraph(int n) {

        Graphics g = this.offscreen3.getGraphics();
        drawWGraph(g, n);
        g.dispose();
    }

    private void drawWGraph(Graphics g, int n) {

        if(n >= 6)
            return;

        g.setFont(this.headFont);
        g.setColor(this.bgColour3);
        FontMetrics fm = g.getFontMetrics();

        g.fillRect(this.WIDTH - this.CWIDTH - this.SCREEN3SPACE - this.GWIDTH, this.screenGap * 2 + this.CELLTOP + this.CELLHEIGHT * 7 + this.CELLTOPMAGIC, this.GWIDTH, this.CELLHEIGHT);

        g.fillRect(this.WIDTH - this.CWIDTH - this.SCREEN3SPACE - this.GWIDTH - 2 - 50, this.screenGap * 2 + this.CELLTOP + this.CELLHEIGHT * 7 + this.CELLTOP - (this.CELLHEIGHT / 2), 50, this.CELLHEIGHT);
        g.fillRect(this.WIDTH - this.CWIDTH - this.SCREEN3SPACE - this.GWIDTH - 2 - 50, this.screenGap * 2 + this.CELLTOP + this.CELLHEIGHT * 7 + this.CELLTOP + this.GHEIGHT - (this.CELLHEIGHT / 2), 50, this.CELLHEIGHT);

        g.setColor(this.wGColours[n]);

        g.drawString(this.gTitles[n], this.WIDTH - this.CWIDTH - this.SCREEN3SPACE - this.GWIDTH, this.screenGap * 2 + this.CELLTOP + this.CELLHEIGHT * 7 + this.CELLTOPMAGIC + (this.CELLHEIGHT - fm.getHeight()) / 2 + fm.getAscent());

        g.setColor(Color.white);
        g.drawString(this.graphMaxLabels[n], this.WIDTH - this.CWIDTH - this.SCREEN3SPACE - this.GWIDTH - 2 - fm.stringWidth(this.graphMaxLabels[n]), this.screenGap * 2 + this.CELLTOP + this.CELLHEIGHT * 7 + this.CELLTOP - (this.CELLHEIGHT / 2) + (this.CELLHEIGHT - fm.getHeight()) / 2 + fm.getAscent());
        g.drawString(this.graphMinLabels[n], this.WIDTH - this.CWIDTH - this.SCREEN3SPACE - this.GWIDTH - 2 - fm.stringWidth(this.graphMinLabels[n]), this.screenGap * 2 + this.CELLTOP + this.CELLHEIGHT * 7 + this.CELLTOP + this.GHEIGHT - (this.CELLHEIGHT / 2) + (this.CELLHEIGHT - fm.getHeight()) / 2 + fm.getAscent());

        g.drawImage(this.wGraphs[n], this.WIDTH - this.CWIDTH - this.SCREEN3SPACE - this.GWIDTH, this.screenGap * 2 + this.CELLTOP * 2 + this.CELLHEIGHT * 7, this);

        g.setColor(this.bgColour3);
        g.fillRect(this.TABSIZE, this.screenGap * 2 + this.CELLTOP * 2 + this.CELLHEIGHT * 7, this.SCREEN3SPACE, this.CELLHEIGHT * 6);
        g.setFont(this.headFont);
        fm = g.getFontMetrics();

        g.setColor(this.wGColours[n]);
        g.drawString(">", this.TABSIZE + this.SCREEN3SPACE - 2 - fm.stringWidth(">"), this.screenGap * 2 + this.CELLTOP * 2 + this.CELLHEIGHT * (7 + n) + (this.CELLHEIGHT - fm.getHeight()) / 2 + fm.getAscent());

        if(this.screenNum == 3) {
            redrawOffscreen(false);
            repaint(100L);
        }
    }

    public void updateWGraph(int gr) {

        int scalingX;
        int y1;
        Graphics g = this.wGraphs[gr].getGraphics();

        while((this.timePeriods[this.timePeriod] > 0) && (this.timeNow > this.timePeriods[this.timePeriod])) {
            this.timePeriod += 1;
            drawWLabel();
        }

        g.setColor(this.bgColour3);
        g.fillRect(0, 0, this.GWIDTH, this.GHEIGHT);
        g.setColor(this.wGColours[gr]);

        if((this.graphMax[gr] <= 0) || (this.graphData[gr].size() == 0)) {
            return;
        }

        if(this.timeNow == 0)
            scalingX = 1;
        else {
            scalingX = this.GWIDTH * 100000 / this.timePeriods[this.timePeriod];
        }

        int scalingY = (this.GHEIGHT - 1) * 100000 / (this.graphMax[gr] - this.graphMin[gr]);

        int x1 = 0;

        for(int n = 0; n < this.graphData[gr].size() - 1; ++n) {
            int y;
            try {
                x = this.graphTime[gr].get(n) * scalingX / 100000;
                y = this.GHEIGHT - 1 - ((this.graphData[gr].get(n) - this.graphMin[gr]) * scalingY / 100000);
                x1 = this.graphTime[gr].get(n + 1) * scalingX / 100000;
                y1 = this.GHEIGHT - 1 - ((this.graphData[gr].get(n + 1) - this.graphMin[gr]) * scalingY / 100000);
            } catch (ArrayIndexOutOfBoundsException e) {
                return;
            }

            g.drawLine(x, y, x1, y);
            g.drawLine(x1, y, x1, y1);
        }
        try {
            y1 = this.GHEIGHT - 1 - ((this.graphData[gr].get(this.graphData[gr].size() - 1) - this.graphMin[gr]) * scalingY / 100000);
        } catch (ArrayIndexOutOfBoundsException e) {
            return;
        }

        int x = this.timeNow * scalingX / 100000;
        g.drawLine(x1, y1, x, y1);

        g.dispose();
    }

    public void updateTime(int t) {

        this.timeNow = t;
    }

    public int getTime() {

        return this.timeNow;
    }

    public void updateRaceStatus(int status) {

        int blatWidth = 100;
        String statusMessage = "";

        if(this.screenSize == 25) {
            blatWidth = 150;
        }

        Graphics g1 = this.offscreen1.getGraphics();
        g1.setFont(this.dataFont);

        Graphics g2 = g1;

        if(this.weather) {
            g2 = this.offscreen3.getGraphics();
            g2.setFont(this.dataFont);
        }
        FontMetrics fm = g1.getFontMetrics();

        g1.setColor(this.bgColour1);
        if(this.weather) {
            g2.setColor(this.bgColour3);
        }
        if(this.season >= 2009) {
            g1.fillRect(this.TABSIZE + this.SCREEN3SPACE, this.HEIGHT - this.COPYRIGHTHEIGHT, blatWidth, this.COPYRIGHTHEIGHT);
            if(this.weather)
                g2.fillRect(this.TABSIZE + this.SCREEN3SPACE, this.HEIGHT - this.COPYRIGHTHEIGHT, blatWidth, this.COPYRIGHTHEIGHT);
        } else {
            g1.fillRect(this.TABSIZE + this.SCREEN3SPACE, this.HEIGHT - this.COPYRIGHTHEIGHT - (this.CELLHEIGHT * 2) + 2, blatWidth, this.CELLHEIGHT * 2);
            if(this.weather) {
                g2.fillRect(this.TABSIZE + this.SCREEN3SPACE, this.HEIGHT - this.COPYRIGHTHEIGHT - (this.CELLHEIGHT * 2) + 2, blatWidth, this.CELLHEIGHT * 2);
            }
        }

        if(status > 0) {
            g1.setColor(Color.white);
            if(this.weather) {
                g2.setColor(Color.white);
            }

            int statusWidth = fm.stringWidth("Track Status: ");
            if(this.season >= 2009) {
                g1.drawString("Track Status: ", this.TABSIZE + this.SCREEN3SPACE, this.HEIGHT - this.COPYRIGHTHEIGHT + (this.COPYRIGHTHEIGHT - fm.getHeight()) / 2 + fm.getAscent());
                if(this.weather)
                    g2.drawString("Track Status: ", this.TABSIZE + this.SCREEN3SPACE, this.HEIGHT - this.COPYRIGHTHEIGHT + (this.COPYRIGHTHEIGHT - fm.getHeight()) / 2 + fm.getAscent());
            } else {
                g1.drawString("Track Status: ", this.TABSIZE + this.SCREEN3SPACE, this.HEIGHT - this.COPYRIGHTHEIGHT - (this.CELLHEIGHT * 2) + 2 + (this.CELLHEIGHT * 2 - fm.getHeight()) / 2 + fm.getAscent());
                if(this.weather) {
                    g2.drawString("Track Status: ", this.TABSIZE + this.SCREEN3SPACE, this.HEIGHT - this.COPYRIGHTHEIGHT - (this.CELLHEIGHT * 2) + 2 + (this.CELLHEIGHT * 2 - fm.getHeight()) / 2 + fm.getAscent());
                }
            }

            switch(status) {
                case 1:
                    g1.setColor(Color.green);
                    if(this.weather) {
                        g2.setColor(Color.green);
                    }
                    statusMessage = "�?";
                    break;
                case 2:
                    g1.setColor(Color.yellow);
                    if(this.weather) {
                        g2.setColor(Color.yellow);
                    }
                    statusMessage = "▲";
                    break;
                case 3:
                    g1.setColor(Color.yellow);
                    if(this.weather) {
                        g2.setColor(Color.yellow);
                    }
                    statusMessage = "SCS";
                    break;
                case 4:
                    g1.setColor(Color.yellow);
                    if(this.weather) {
                        g2.setColor(Color.yellow);
                    }
                    statusMessage = "SCD";
                    break;
                case 5:
                    g1.setColor(Color.red);
                    if(this.weather) {
                        g2.setColor(Color.red);
                    }
                    statusMessage = "■";
            }

            g1.setFont(this.timeFont);
            if(this.weather) {
                g2.setFont(this.timeFont);
            }
            fm = g1.getFontMetrics();

            if(this.season >= 2009) {
                g1.drawString(statusMessage, this.TABSIZE + this.SCREEN3SPACE + statusWidth, this.HEIGHT - this.COPYRIGHTHEIGHT + (this.COPYRIGHTHEIGHT - fm.getHeight()) / 2 + fm.getAscent());
                if(this.weather)
                    g2.drawString(statusMessage, this.TABSIZE + this.SCREEN3SPACE + statusWidth, this.HEIGHT - this.COPYRIGHTHEIGHT + (this.COPYRIGHTHEIGHT - fm.getHeight()) / 2 + fm.getAscent());
            } else {
                g1.drawString(statusMessage, this.TABSIZE + this.SCREEN3SPACE + statusWidth, this.HEIGHT - this.COPYRIGHTHEIGHT - (this.CELLHEIGHT * 2) + 2 + (this.CELLHEIGHT * 2 - fm.getHeight()) / 2 + fm.getAscent());
                if(this.weather) {
                    g2.drawString(statusMessage, this.TABSIZE + this.SCREEN3SPACE + statusWidth, this.HEIGHT - this.COPYRIGHTHEIGHT - (this.CELLHEIGHT * 2) + 2 + (this.CELLHEIGHT * 2 - fm.getHeight()) / 2 + fm.getAscent());
                }
            }
        }

        if((this.screenNum == 1) || (this.screenNum == 3)) {
            redrawOffscreen(false);
            repaint(100L, this.TABSIZE, this.HEIGHT - this.COPYRIGHTHEIGHT - this.CELLTOP, this.WIDTH - this.CWIDTH - this.TABSIZE, this.CELLTOP);
        }
    }

    public void updateSessionTimeInterpolate() {

        debug("SessionTime: starting to decrement");
        decrementTime(true);
    }

    public void updateSessionTime(String strValue) {

        debug("SessionTime: setting to " + strValue);

        decrementTime(false);

        this.lastOfficialTime = strValue;
        this.lastOfficialTimeTime = System.currentTimeMillis();

        drawSessionTime(strValue);
    }

    public void fudgeSessionTime(int nSecs) {

        debug("fudgeSessionTime(" + nSecs + ")");
        int realTime = f1time.timeToTime(this.lastOfficialTime) - nSecs;
        if(realTime < 0) {
            debug("fudgeSessionTime(" + nSecs + ") results in a time of " + realTime);
            realTime = 0;
        }

        debug("was " + this.lastOfficialTime);

        this.lastOfficialTimeTime = System.currentTimeMillis();
        this.lastOfficialTime = f1time.timeToTime(realTime);

        drawSessionTime(this.lastOfficialTime);
        debug("drawing " + this.lastOfficialTime);
    }

    private void drawFastest() {

        updateCell(this.colFastest, "BEST LAP:", 5, 24, 2, 1, true);
        updateCell(this.colFastest, this.fastestNumber, 4, 24, 3, 0, true);
        updateCell(this.colFastest, this.fastestName, 4, 24, 4, 1, true);
        updateCell(this.colFastest, this.fastestTime, 4, 24, 5, 1, true);
        updateCell(this.colFastest, "ON LAP", 5, 24, 6, 2, true);
        updateCell(this.colFastest, this.fastestLap, 4, 24, 7, 1, true);
    }

    public void drawSessionTime(String strValue) {

        if(this.raceMode != 1) {
            drawScreen1Time(strValue);
            drawScreen3Time(strValue);
        }
    }

    private void drawScreen1Time(String strValue) {

        int blatWidth = (this.WIDTH - this.CWIDTH - this.TABSIZE - (this.SCREEN3SPACE * 2)) / 2;

        Graphics g = this.offscreen1.getGraphics();

        g.setColor(this.bgColour1);
        g.fillRect(this.TABSIZE + this.SCREEN3SPACE + blatWidth, this.HEIGHT - this.COPYRIGHTHEIGHT - (this.CELLHEIGHT * 2) + 2, blatWidth, this.CELLHEIGHT * 2);

        g.setColor(Color.yellow);
        g.setFont(this.timeFont);
        FontMetrics fm = g.getFontMetrics();

        int timeWidth = fm.stringWidth(strValue);
        g.drawString(strValue, this.WIDTH - this.CWIDTH - timeWidth - this.SCREEN3SPACE, this.HEIGHT - this.COPYRIGHTHEIGHT - (this.CELLHEIGHT * 2) + 2 + (this.CELLHEIGHT * 2 - fm.getHeight()) / 2 + fm.getAscent());

        g.setColor(Color.white);
        g.setFont(this.dataFont);
        fm = g.getFontMetrics();

        timeWidth += fm.stringWidth("Session Time Remaining: ");

        g.drawString("Session Time Remaining: ", this.WIDTH - this.CWIDTH - timeWidth - this.SCREEN3SPACE, this.HEIGHT - this.COPYRIGHTHEIGHT - (this.CELLHEIGHT * 2) + 2 + (this.CELLHEIGHT * 2 - fm.getHeight()) / 2 + fm.getAscent());

        g.dispose();

        if(this.screenNum == 1) {
            redrawOffscreen(false);
            repaint(100L, this.TABSIZE + this.SCREEN3SPACE, this.HEIGHT - this.COPYRIGHTHEIGHT - (this.CELLHEIGHT * 2) + 2, this.WIDTH - this.CWIDTH - this.TABSIZE - (this.SCREEN3SPACE * 2), this.CELLHEIGHT * 2);
        }
    }

    private void drawScreen3Time(String strValue) {

        int blatWidth = (this.WIDTH - this.CWIDTH - this.TABSIZE - (this.SCREEN3SPACE * 2)) / 2;

        checkScreen3Init();

        Graphics g = this.offscreen3.getGraphics();

        g.setColor(this.bgColour3);
        g.fillRect(this.TABSIZE + this.SCREEN3SPACE + blatWidth, this.HEIGHT - this.COPYRIGHTHEIGHT - (this.CELLHEIGHT * 2) + 2, blatWidth, this.CELLHEIGHT * 2);

        g.setColor(Color.yellow);
        g.setFont(this.timeFont);
        FontMetrics fm = g.getFontMetrics();

        int timeWidth = fm.stringWidth(strValue);
        g.drawString(strValue, this.WIDTH - this.CWIDTH - timeWidth - this.SCREEN3SPACE, this.HEIGHT - this.COPYRIGHTHEIGHT - (this.CELLHEIGHT * 2) + 2 + (this.CELLHEIGHT * 2 - fm.getHeight()) / 2 + fm.getAscent());

        g.setColor(Color.white);
        g.setFont(this.dataFont);
        fm = g.getFontMetrics();

        timeWidth += fm.stringWidth("Session Time Remaining: ");

        g.drawString("Session Time Remaining: ", this.WIDTH - this.CWIDTH - timeWidth - this.SCREEN3SPACE, this.HEIGHT - this.COPYRIGHTHEIGHT - (this.CELLHEIGHT * 2) + 2 + (this.CELLHEIGHT * 2 - fm.getHeight()) / 2 + fm.getAscent());

        if(this.screenNum == 3) {
            redrawOffscreen(false);
            repaint(100L, this.TABSIZE + this.SCREEN3SPACE + blatWidth, this.HEIGHT - this.COPYRIGHTHEIGHT - (this.CELLHEIGHT * 2) + 2, blatWidth, this.CELLHEIGHT * 2);
        }
    }

    public void updateScreen3Speed(int x, int y, String str) {

        checkScreen3Init();

        Graphics g = this.offscreen3.getGraphics();

        if((x <= 0) || (y <= 0) || (x > 8) || (y > 6)) {
            g.dispose();
            return;
        }

        g.setFont(this.dataFont);
        FontMetrics fm = g.getFontMetrics();

        int xPos = this.TABSIZE + (x - 1) * 45 + (x + 1) / 2 * this.SCREEN3SPACE;

        int yPos = this.screenGap + this.CELLTOP + this.CELLHEIGHT * y + (this.CELLHEIGHT - fm.getHeight()) / 2 + fm.getAscent();

        g.setClip(xPos, yPos - this.CELLHEIGHT, 45, this.CELLHEIGHT);
        g.setColor(this.bgColour3);
        g.fillRect(xPos, yPos - this.CELLHEIGHT, 45, this.CELLHEIGHT);

        g.setColor(Color.white);
        g.drawString(str, xPos, yPos);
        if(this.screenNum == 3) {
            redrawOffscreen(false);
            repaint(100L);
        }
        g.dispose();
    }

    public void updateScreen3Fastest(int column, String data) {

        if(this.raceMode == 1) {
            switch(column) {
                case 4:
                    this.fastestNumber = data;
                    break;
                case 5:
                    this.fastestName = data;
                    break;
                case 6:
                    this.fastestTime = data;
                    break;
                case 7:
                    this.fastestLap = data;
            }

            drawFastest();
        }
    }

    @Override
    public void stop() {

        debug("stop() called");
        this.stopping = true;
        if(this.theProcess == null)
            return;
        this.theProcess = null;
    }

    private boolean checkName(String hostname, String domain) {

        if((!(hostname.endsWith("." + domain))) && (!(hostname.endsWith("." + domain + "."))) && (hostname.compareTo(domain) != 0) && (hostname.compareTo(domain + ".") != 0)) {
            debug("Check: " + hostname + "=" + domain + ": false");
            return false;
        }
        debug("Check: " + hostname + "=" + domain + ": true");
        return true;
    }

    @Override
    public void start() {

        this.stopping = false;

        repaint();

        String hostname = getDocumentBase().getHost().toLowerCase();

        if((hostname != null) && (!(hostname.equals("")))) {
            output("Hostname: " + hostname);
            if((!(checkName(hostname, "formula1.com"))) && (!(checkName(hostname, "f1.com"))) && (!(checkName(hostname, "aspect-internet.com"))) && (!(checkName(hostname, "aspectgroup.co.uk"))) && (!(checkName(hostname, "lbi.co.uk"))) && (!(checkName(hostname, "localhost"))) && (hostname.compareTo(getCodeBase().getHost().toString().toLowerCase()) != 0)) {
                this.safetyMessage = "Please visit www.formula1.com for the live timing feed";
                setValid(false);

                new f1security(this);
                return;
            }

        }

        this.decrypter = new f1crypt(this, 0);

        this.theProcess = new f1process(this, getParameter("keyframe"));

        enableEvents(56L);
    }

    @Override
    protected void processKeyEvent(KeyEvent e) {

    }

    private void unMouseOver(Graphics g) {

        switch(this.screenNum) {
            case 1:
                g.drawImage(this.offscreen1, 0, 0, this.TABSIZE, this.HEIGHT, 0, 0, this.TABSIZE, this.HEIGHT, this);
                break;
            case 2:
                g.drawImage(this.offscreen2, 0, 0, this.TABSIZE, this.HEIGHT, 0, 0, this.TABSIZE, this.HEIGHT, this);
                break;
            case 3:
                g.drawImage(this.offscreen3, 0, 0, this.TABSIZE, this.HEIGHT, 0, 0, this.TABSIZE, this.HEIGHT, this);
        }

        this.mouseOvering = false;
        drawLED();
        repaint(100L);
    }

    @Override
    protected void processMouseMotionEvent(MouseEvent e)
  {
    int eID;
    Graphics g = null;
    this.scrolling = '\0';
    int x = e.getX();
    int y = e.getY();

    if (this.scrollbaring) {
      eID = e.getID();

      if (eID == 506) {
        doScrollBar(false, this.scrollbarStartOffset + y - this.scrollbarStartY);
        scrollTo(this.scrollbar.getScrollOffset() * (this.nCommentaryLines + 1) / (this.HEIGHT - this.COMMENTHEAD - this.COMMENTTAIL));
      }
      return;
    }

    if ((this.raceMode == 1) || (this.weather)) {
      eID = e.getID();

      if (this.mouseOvering) {
        g = this.offscreen.getGraphics();
        unMouseOver(g);
      }
      if ((x >= 0) && (x <= this.TABSIZE + 3)) {
        if (g == null) g = this.offscreen.getGraphics();
      }
      switch (y / this.TABHEIGHT + 1)
      {
      case 1:
        if (this.screenNum != 1) {
          g.drawImage(this.buttonsH, 0, this.TABFUDGE / 2, this.TABSIZE, this.TABHEIGHT - (this.TABFUDGE / 2), 0, this.TABFUDGE / 2, this.TABSIZE, this.TABHEIGHT - (this.TABFUDGE / 2), this);
          this.mouseOvering = true;
          drawLED();
          repaint(100L);
        }
        break;
      case 2:
        if ((this.screenNum != 3) && (this.weather)) {
          g.drawImage(this.buttonsH, 0, this.TABHEIGHT + this.TABFUDGE / 2, this.TABSIZE, this.TABHEIGHT + this.TABHEIGHT - (this.TABFUDGE / 2), 0, this.TABHEIGHT + this.TABFUDGE / 2, this.TABSIZE, this.TABHEIGHT + this.TABHEIGHT - (this.TABFUDGE / 2), this);
          this.mouseOvering = true;
          drawLED();
          repaint(100L);
        }
        break;
      case 3:
        if ((this.screenNum != 2) && (this.raceMode == 1)) {
          g.drawImage(this.buttonsH, 0, this.TABHEIGHT * 2 + this.TABFUDGE / 2, this.TABSIZE, this.TABHEIGHT * 2 + this.TABHEIGHT - (this.TABFUDGE / 2), 0, this.TABHEIGHT * 2 + this.TABFUDGE / 2, this.TABSIZE, this.TABHEIGHT * 2 + this.TABHEIGHT - (this.TABFUDGE / 2), this);
          this.mouseOvering = true;
          drawLED();
          repaint(100L);
        }
      default:
        break label815:

        if (this.screenNum == 2) {
          switch (eID)
          {
          case 505:
            if (this.gHighLight > 0) {
              this.gHighLight = 0;

              redrawOffscreen(true);
            }
            break;
          default:
            if ((x < this.TABSIZE) || (x > this.TABSIZE + 100) || 
              (y < this.CELLTOP) || (y > this.CELLTOP + 29 * this.CELLHEIGHT))
            {
              if (x < this.WIDTH - this.CWIDTH) {
                int colour = getPixelColour(x, y);
                int found = 0;

                if ((colour == 0) || (colour == 604676)) {
                  if (this.gHighLight > 0) {
                    this.gHighLight = 0;
                    redrawOffscreen(false);
                    repaint(100L);
                  }
                  return;
                }
                for (int n = 0; n < 29; ++n) {
                  if (this.gColourRGBs[n] == colour) {
                    found = n + 1;
                  }
                }

                if (found > 0) {
                  if (found != this.gHighLight) {
                    this.gHighLight = found;
                    redrawOffscreen(false);
                    repaint(100L);
                  }
                }
                else if (this.gHighLight > 0) {
                  this.gHighLight = 0;
                  redrawOffscreen(false);
                  repaint(100L);
                }

              }
              else if (this.gHighLight > 0) {
                this.gHighLight = 0;
                redrawOffscreen(false);
                repaint(100L);
              }
            }
            else
            {
              int h = (y - this.CELLTOP) / this.CELLHEIGHT + 1;
              if (h != this.gHighLight) {
                this.gHighLight = h;

                redrawOffscreen(true);
              }
            }
          }
        }

      }

      if (g != null) {
        label815: g.dispose();
        g = null;
      }
    }

    if ((this.inlineLayout) && 
      (x >= this.WIDTH - this.BORDERSIZE - this.SCROLLBAR_WIDTH) && 
      (x <= this.WIDTH - this.BORDERSIZE)) {
      if ((y < this.COMMENTHEAD) || (y >= this.HEIGHT - this.COMMENTTAIL) || 
        (this.handCursor)) return;
      setCursor(Cursor.getPredefinedCursor(12));
      this.handCursor = true;
    }
    else if ((this.inlineLayout) && 
      (x > this.WIDTH - this.CWIDTH) && 
      (y > this.HEIGHT - (this.linkBoxEntries * 25))) {
      int n = (y - (this.HEIGHT - (this.linkBoxEntries * 25))) % 25;
      if ((n > 6) && (n < 18)) {
        setCursor(Cursor.getPredefinedCursor(12));
        this.handCursor = true;
      } else {
        setCursor(Cursor.getPredefinedCursor(0));
        this.handCursor = false; }
    } else {
      if ((x >= 0) && (x <= this.TABSIZE + 3) && (y >= 0) && (y < this.TABHEIGHT * 3));
      switch (y / this.TABHEIGHT + 1)
      {
      case 1:
        if (this.screenNum == 1) {
          if (!(this.handCursor)) return;
          setCursor(Cursor.getPredefinedCursor(0));
          this.handCursor = false; return;
        }

        if (this.handCursor) return;
        setCursor(Cursor.getPredefinedCursor(12));
        this.handCursor = true;

        break;
      case 2:
        if ((this.screenNum == 3) || (!(this.weather))) {
          if (!(this.handCursor)) return;
          setCursor(Cursor.getPredefinedCursor(0));
          this.handCursor = false; return;
        }

        if (this.handCursor) return;
        setCursor(Cursor.getPredefinedCursor(12));
        this.handCursor = true;

        break;
      case 3:
        if ((this.screenNum == 2) || (this.raceMode != 1)) {
          if (!(this.handCursor)) return;
          setCursor(Cursor.getPredefinedCursor(0));
          this.handCursor = false; return;
        }

        if (this.handCursor) return;
        setCursor(Cursor.getPredefinedCursor(12));
        this.handCursor = true;
      default:
        return;
        if ((this.screenNum == 3) && 
          (y > this.screenGap * 2 + this.CELLTOP * 2 + this.CELLHEIGHT * 7) && (y < this.screenGap * 2 + this.CELLTOP * 2 + this.CELLHEIGHT * 13) && 
          (x > this.TABSIZE + this.SCREEN3SPACE) && (x < 200)) {
          if (this.handCursor) return;
          setCursor(Cursor.getPredefinedCursor(12));
          this.handCursor = true; return;
        }

        if (!(this.handCursor)) return;
        setCursor(Cursor.getDefaultCursor());
        this.handCursor = false;
      }
    }
  }

    private void grabScreen2() {

        PixelGrabber pg = new PixelGrabber(this.offscreen2, 0, 0, this.WIDTH - this.CWIDTH, this.HEIGHT, this.offscreen2Pixels, 0, this.WIDTH - this.CWIDTH);
        try {
            pg.grabPixels();
        } catch (InterruptedException localInterruptedException) {
        }
    }

    private int getPixelColour(int x, int y) {

        int colour = this.offscreen2Pixels[(y * (this.WIDTH - this.CWIDTH) + x)] & 0xFFFFFF;
        try {
            if((colour == 0) || (colour == 604676)) {
                colour = this.offscreen2Pixels[((y - 1) * (this.WIDTH - this.CWIDTH) + x)] & 0xFFFFFF;
                if((colour == 0) || (colour == 604676)) {
                    colour = this.offscreen2Pixels[((y + 1) * (this.WIDTH - this.CWIDTH) + x)] & 0xFFFFFF;
                    if((colour == 0) || (colour == 604676)) {
                        colour = this.offscreen2Pixels[((y - 2) * (this.WIDTH - this.CWIDTH) + x)] & 0xFFFFFF;
                        if((colour == 0) || (colour == 604676)) {
                            colour = this.offscreen2Pixels[((y + 2) * (this.WIDTH - this.CWIDTH) + x)] & 0xFFFFFF;
                        }
                    }
                }
            }
        } catch (ArrayIndexOutOfBoundsException localArrayIndexOutOfBoundsException) {
        }
        return colour;
    }

    @Override
    protected void processMouseEvent(MouseEvent e) {

        int eID = e.getID();

        int x = e.getX();
        int y = e.getY();

        if((eID == 505) && (this.mouseOvering)) {
            Graphics g = this.offscreen.getGraphics();
            unMouseOver(g);
            debug("mouse exited");
            g.dispose();
        }

        if(this.inlineLayout) {
            if(this.scrollbar != null) {
                if((x >= this.WIDTH - this.BORDERSIZE - this.SCROLLBAR_WIDTH) && (x <= this.WIDTH - this.BORDERSIZE) && (eID == 501) && (y >= this.COMMENTHEAD) && (y < this.HEIGHT - this.COMMENTTAIL)) {
                    if(y < this.COMMENTHEAD + this.scrollbar.getScrollOffset()) {
                        scrollUp();
                        doScrollBar(true);
                    } else if(y > this.COMMENTHEAD + this.scrollbar.getScrollOffset() + this.scrollbar.getScrollSize()) {
                        scrollDown();
                        doScrollBar(true);
                    } else {
                        this.scrollbarStartY = y;
                        this.scrollbarStartOffset = this.scrollbar.getScrollOffset();
                        this.scrollbaring = true;
                    }

                }

                if(eID == 502) {
                    this.scrollbaring = false;
                }
            }
            if((x > this.WIDTH - this.CWIDTH) && (y > this.HEIGHT - (this.linkBoxEntries * 25)) && (eID == 501)) {
                int n = (y - (this.HEIGHT - (this.linkBoxEntries * 25))) % 25;
                int nn = (y - (this.HEIGHT - (this.linkBoxEntries * 25))) / 25;

                if((n > 6) && (n < 18)) {
                    debug("Click on linkbox, n=" + n + ", nn=" + nn);
                    setCursor(Cursor.getPredefinedCursor(3));
                    this.handCursor = true;
                    getAppletContext().showDocument(this.linkBoxURL[nn], "_blank");
                }
            }

        } else if(eID == 501) {
            debug("mouse pressed");

            if((x >= this.WIDTH - this.BORDERSIZE - 15) && (x <= this.WIDTH - this.BORDERSIZE)) {
                if((y >= 27) && (y <= 42)) {
                    debug("scroll up");
                    scrollUp();
                    this.scrolling = '\1';
                    new f1scroller(this, this.offscreenC);
                }
                if((y >= this.HEIGHT - 23) && (y <= this.HEIGHT - 7)) {
                    debug("scroll down");
                    scrollDown();
                    this.scrolling = '\2';
                    new f1scroller(this, this.offscreenC);
                }
            }
        } else {
            this.scrolling = '\0';
        }

        if((eID == 501) && (x >= 0) && (x <= this.TABSIZE + 3) && (y >= 0) && (y < this.TABHEIGHT * 3)) {
            switch(y / this.TABHEIGHT + 1) {
                case 1:
                    this.screenNum = 1;
                    break;
                case 2:
                    if(this.weather) {
                        this.screenNum = 3;
                    }
                    break;
                case 3:
                    if(this.raceMode == 1) {
                        this.screenNum = 2;
                    }

            }

            redrawOffscreen(true);
        }

        if((eID != 501) || (this.screenNum != 3) || (y <= this.screenGap * 2 + this.CELLTOP * 2 + this.CELLHEIGHT * 7) || (y >= this.screenGap * 2 + this.CELLTOP * 2 + this.CELLHEIGHT * 13) || (x <= this.TABSIZE + this.SCREEN3SPACE) || (x >= 200))
            return;
        int c = (y - (this.screenGap * 2 + this.CELLTOP * 2 + this.CELLHEIGHT * 7)) / this.CELLHEIGHT;

        this.weatherTicker.delayNext();
        drawWGraph(c);
    }

    @Override
    public String[][] getParameterInfo() {

        String[][] info = { { "mode", "String", "Feed mode" } };

        return info;
    }

    @Override
    public void paint(Graphics g) {

        if(this.isLoaded) {
            g.drawImage(this.offscreen, 0, 0, this);
        } else {
            g.setColor(Color.white);
            g.fillRect(0, 0, this.WIDTH, this.HEIGHT);
            g.setColor(Color.black);
            g.drawString("Loading ...", 20, 40);
        }
    }

    @Override
    public void update(Graphics g) {

        paint(g);
    }

    public void initRow(int row, int slot) {

        this.gRow[slot][0] = (byte)row;
        this.gLap[slot] = 0;
    }

    public int getRowZero(int slot) {

        return this.gRow[slot][0];
    }

    public void setRowName(int slot, String name) {

        if(this.raceMode != 1) {
            return;
        }

        Graphics g = this.offscreen2.getGraphics();

        this.gName[slot] = name;

        int yPos = this.CELLTOP + (this.gRow[slot][0] - 1) * this.CELLHEIGHT;
        int xPos = this.TABSIZE + 3;

        g.setClip(xPos, yPos, this.SCREEN2LABELWIDTH - 3 - 2, this.CELLHEIGHT);
        g.setColor(this.bgColour2);
        g.fillRect(xPos, yPos, this.SCREEN2LABELWIDTH - 3 - 2, this.CELLHEIGHT);

        g.setColor(this.gColours[(this.gRow[slot][0] - 1)]);

        g.setFont(this.dataFont);
        FontMetrics fm = g.getFontMetrics();

        g.drawString(name, xPos, yPos + (this.CELLHEIGHT - fm.getHeight()) / 2 + fm.getAscent());

        g.dispose();

        if(this.screenNum == 2) {
            redrawOffscreen(false);

            repaint(250L, xPos, yPos, this.SCREEN2LABELWIDTH - 3 - 2, 15);
        }
    }

    public String getRowName(int slot) {

        return this.gName[slot];
    }

    private void drawGLine(Graphics g, int slot, int lap2, boolean highlight) {

        int[] xs;
        int[] ys;
        int lap1 = lap2 - 1;
        while(this.gRow[slot][lap1] == 0) {
            --lap1;
        }

        int sw = this.scalingWidths[this.scaling];

        int xPos1 = this.TABSIZE + this.SCREEN2LABELWIDTH + lap1 * sw;
        int xPos2 = this.TABSIZE + this.SCREEN2LABELWIDTH + lap2 * sw;

        int yPos1 = this.CELLTOP + 7 + (this.gRow[slot][lap1] - 1) * this.CELLHEIGHT;
        int yPos2 = this.CELLTOP + 7 + (this.gRow[slot][lap2] - 1) * this.CELLHEIGHT;

        if(highlight)
            g.setColor(Color.white);
        else {
            g.setColor(this.gColours[(this.gRow[slot][0] - 1)]);
        }

        if(yPos2 == yPos1) {
            xs = new int[] { xPos1 - 1, xPos2 + 1, xPos2 + 1, xPos1 - 1 };

            ys = new int[] { yPos1 - 1, yPos2 - 1, yPos2 + 1, yPos1 + 1 };

            g.fillPolygon(xs, ys, 4);
        } else if(yPos2 > yPos1) {
            xs = new int[] { xPos1 - 1, xPos1 + 1, xPos2 + 1, xPos2 + 1, xPos2 - 1, xPos1 - 1 };

            ys = new int[] { yPos1 - 1, yPos1 - 1, yPos2 - 1, yPos2 + 1, yPos2 + 1, yPos1 + 1 };

            g.fillPolygon(xs, ys, 6);
        } else {
            xs = new int[] { xPos1 - 1, xPos1 + 1, xPos2 + 1, xPos2 + 1, xPos2 - 1, xPos1 - 1 };

            ys = new int[] { yPos1 + 1, yPos1 + 1, yPos2 + 1, yPos2 - 1, yPos2 - 1, yPos1 - 1 };

            g.fillPolygon(xs, ys, 6);
        }
    }

    public void updateGraph(int slot, int row, int lap) {

        Graphics g;
        int sw;
        if(this.raceMode != 1) {
            return;
        }

        if(lap >= this.scalingLevels[this.scaling]) {
            f1app tmp23_22 = this;
            tmp23_22.scaling = (byte)(tmp23_22.scaling + 1);
            sw = this.scalingWidths[this.scaling];

            g = this.offscreen2.getGraphics();
            g.setColor(this.bgColour2);
            g.fillRect(this.TABSIZE + this.SCREEN2LABELWIDTH, 1, this.WIDTH - this.CWIDTH - this.TABSIZE - this.SCREEN2LABELWIDTH - 1, this.HEIGHT - this.COPYRIGHTHEIGHT - 2);
            g.setColor(this.bgColour2a);
            g.fillRect(this.TABSIZE + this.SCREEN2LABELWIDTH - 2, this.CELLTOP, this.WIDTH - this.CWIDTH - this.TABSIZE - this.SCREEN2LABELWIDTH, this.CELLHEIGHT * 8);

            g.setColor(Color.white);
            g.setFont(this.commentaryHeadFont);

            drawVText(this.offscreen2, g, 20, this.CELLHEIGHT * 8, this.WIDTH - this.CWIDTH - 22, this.CELLTOP, "Points");

            for(int l = 1; l <= lap; ++l) {
                for(int i = 0; i < 32; ++i) {
                    if(this.gRow[i][l] > 0) {
                        drawGLine(g, i, l, false);
                    }
                }
            }

            g.dispose();

            this.gLapNo = 1;
        } else {
            sw = this.scalingWidths[this.scaling];
        }

        while(this.gLapNo <= lap) {
            updateHeader2(this.offscreen2, Integer.toString(this.gLapNo), this.TABSIZE + this.SCREEN2LABELWIDTH + this.gLapNo * sw, this.headColour, 2);
            f1app tmp339_338 = this;
            tmp339_338.gLapNo = (byte)(tmp339_338.gLapNo + this.scalingSteps[this.scaling]);
        }

        if((this.isValid) && (lap > this.gLap[slot])) {
            g = this.offscreen2.getGraphics();

            this.gRow[slot][lap] = (byte)row;
            this.gLap[slot] = (byte)lap;

            drawGLine(g, slot, lap, false);

            g.dispose();

            if(this.screenNum == 2) {
                redrawOffscreen(false);
                repaint(100L);
            }
        }

        if(this.redraw)
            grabScreen2();
    }

    public void updateHeader2(Image img, String str, int xPos, Color colour, int aligned) {

        Graphics g = img.getGraphics();

        int yPos = this.CELLTOPMAGIC;

        g.setColor(colour);
        g.setFont(this.dataFont);
        FontMetrics fm = g.getFontMetrics();

        int sWidth = fm.stringWidth(str);
        int fHeight = fm.getHeight();

        if(aligned == 1)
            g.drawString(str, xPos, yPos + (this.CELLHEIGHT - fHeight) / 2 + fm.getAscent());
        else if(aligned == 2)
            g.drawString(str, xPos - (sWidth / 2), yPos + (this.CELLHEIGHT - fHeight) / 2 + fm.getAscent());
        else {
            g.drawString(str, xPos - sWidth, yPos + (this.CELLHEIGHT - fHeight) / 2 + fm.getAscent());
        }
        g.dispose();
    }

    public void updateHeader1(Image img, String str, int x, Color colour, int aligned) {

        Graphics g = img.getGraphics();

        if(x > this.nCols) {
            return;
        }

        int xPos = this.TABSIZE + 2;
        for(int n = 0; n < x - 1; ++n) {
            xPos += this.colWidths[n];
        }
        ++xPos;
        int yPos = this.CELLTOPMAGIC;

        g.setColor(this.bgColour1);
        g.fillRect(xPos, yPos, this.colWidths[(x - 1)] - 2, 14);

        g.setColor(colour);
        g.setFont(this.dataFont);
        FontMetrics fm = g.getFontMetrics();

        int sWidth = fm.stringWidth(str);
        int fHeight = fm.getHeight();

        if(aligned == 1)
            g.drawString(str, xPos, yPos + (this.CELLHEIGHT - fHeight) / 2 + fm.getAscent());
        else if(aligned == 2)
            g.drawString(str, xPos + (this.colWidths[(x - 1)] - sWidth) / 2, yPos + (this.CELLHEIGHT - fHeight) / 2 + fm.getAscent());
        else {
            g.drawString(str, xPos + this.colWidths[(x - 1)] - sWidth - 3, yPos + (this.CELLHEIGHT - fHeight) / 2 + fm.getAscent());
        }

        g.dispose();
    }

    public void updateCell(int[] cw, String str, byte c, int y, int x, byte aligned, boolean repaint) {

        Graphics g = this.offscreen1.getGraphics();

        if(x > this.nCols) {
            g.dispose();
            return;
        }

        int xPos = this.TABSIZE + 2;
        for(int n = 0; n < x - 1; ++n) {
            xPos += cw[n];
        }
        ++xPos;

        int yPos = this.CELLTOP + (y - 1) * this.CELLHEIGHT;

        g.setClip(xPos, yPos, cw[(x - 1)] - 2, this.CELLHEIGHT);
        g.setColor(this.bgColour1);
        g.fillRect(xPos, yPos, cw[(x - 1)] - 2, this.CELLHEIGHT);

        if((str != null) && (str.length() > 0)) {
            g.setFont(this.dataFont);
            FontMetrics fm = g.getFontMetrics();

            int sWidth = fm.stringWidth(str);
            int fHeight = fm.getHeight();
            g.setColor(this.colours[c]);

            if(aligned == 1)
                g.drawString(str, xPos, yPos + (this.CELLHEIGHT - fHeight) / 2 + fm.getAscent());
            else if(aligned == 2)
                g.drawString(str, xPos + (cw[(x - 1)] - sWidth) / 2, yPos + (this.CELLHEIGHT - fHeight) / 2 + fm.getAscent());
            else {
                g.drawString(str, xPos + cw[(x - 1)] - sWidth - 3, yPos + (this.CELLHEIGHT - fHeight) / 2 + fm.getAscent());
            }

        }

        g.dispose();

        if((repaint) && (this.screenNum == 1)) {
            redrawOffscreen(false);

            repaint(100L, xPos, yPos, this.colWidths[(x - 1)] - 2, this.CELLHEIGHT);
        }
    }

    public void updateCell(String str, byte c, int y, int x, boolean repaint) {

        updateCell(this.colWidths, str, c, y, x, this.alignedL[(x - 1)], repaint);
    }

    public void setRedraw(boolean newredraw) {

        if(newredraw) {
            this.redraw = true;
            redrawOffscreen(true);
            repaint();
            if(this.raceMode == 1)
                grabScreen2();
        } else {
            this.redraw = false;
        }
    }

    private void doSafetySetup(Graphics g, FontMetrics fm, String str) {

        int remaining = 400;
        int fw = fm.stringWidth(str);

        if(fw < remaining) {
            this.messageLines[this.nMessageLines] = str;
            this.nMessageLines += 1;
        } else {
            String str1 = str;
            String str2 = null;

            while(fw > remaining) {
                int lastspace = str1.lastIndexOf(32);
                if(lastspace != -1) {
                    str1 = str.substring(0, lastspace);
                    str2 = str.substring(lastspace + 1);
                    fw = fm.stringWidth(str1);
                } else {
                    fw = 0;
                }
            }

            this.messageLines[this.nMessageLines] = str1;
            this.nMessageLines += 1;
            if(str2 != null)
                doSafetySetup(g, fm, str2);
        }
    }

    public void drawSafety(Graphics g, int screen) {

        if(screen == 1)
            g.drawImage(this.offscreen1, 0, 0, this);
        else {
            g.drawImage(this.offscreen3, 0, 0, this);
        }
        g.setColor(this.bgColour1);
        g.fillRect(this.TABSIZE + 1, 1, this.WIDTH - this.CWIDTH - this.TABSIZE - 2, this.HEIGHT - 2);

        g.setFont(new Font("Helvetica", 1, 30));
        g.setColor(Color.red);

        FontMetrics fm = g.getFontMetrics();

        if(this.safetyMessage != null) {
            this.nMessageLines = 0;
            this.messageLines = new String[10];
            doSafetySetup(g, fm, this.safetyMessage);
            this.safetyMessage = null;
        }

        for(int n = 0; n < this.nMessageLines; ++n) {
            int lineHeight = fm.getHeight();
            int x = this.TABSIZE + (this.WIDTH - this.CWIDTH - this.TABSIZE - fm.stringWidth(this.messageLines[n])) / 2;
            int y = (this.HEIGHT - (lineHeight * this.nMessageLines)) / 2 + n * lineHeight + fm.getAscent();
            g.drawString(this.messageLines[n], x, y);
        }
    }

    public void redrawOffscreen(boolean repaint) {

        if(!(this.redraw)) {
            return;
        }

        Graphics g = this.offscreen.getGraphics();

        if(this.screenNum == 1) {
            if(this.isValid)
                g.drawImage(this.offscreen1, 0, 0, this);
            else
                drawSafety(g, 1);
        } else if(this.screenNum == 2) {
            g.drawImage(this.offscreen2, 0, 0, this);

            int gH = 0;

            if(this.gHighLight > 0) {
                for(int n = 0; n < 32; ++n) {
                    if(this.gRow[n][0] == this.gHighLight) {
                        gH = n;
                    }
                }

                if(gH > 0) {
                    int yPos = this.CELLTOP + (this.gRow[gH][0] - 1) * this.CELLHEIGHT;
                    int xPos = this.TABSIZE + 3;

                    g.setClip(xPos, yPos, 95, this.CELLHEIGHT);
                    g.setColor(this.bgColour2);
                    g.fillRect(xPos, yPos, 95, this.CELLHEIGHT);

                    g.setColor(Color.white);

                    g.setFont(this.dataFont);
                    FontMetrics fm = g.getFontMetrics();

                    g.drawString(this.gName[gH], xPos, yPos + (this.CELLHEIGHT - fm.getHeight()) / 2 + fm.getAscent());
                    g.setClip(0, 0, this.WIDTH, this.HEIGHT);

                    for(int l = 1; l <= this.gLap[gH]; ++l) {
                        if(this.gRow[gH][l] > 0) {
                            drawGLine(g, gH, l, true);
                        }
                    }

                }

            }

            for(int n = 0; n < 32; ++n)
                if(this.gRow[n][0] > 0) {
                    int x = this.TABSIZE + this.SCREEN2LABELWIDTH + this.gLap[n] * this.scalingWidths[this.scaling];
                    int y = this.CELLTOP + 7 + (this.gRow[n][this.gLap[n]] - 1) * this.CELLHEIGHT;

                    if(gH == n)
                        g.setColor(Color.white);
                    else {
                        g.setColor(this.gColours[(this.gRow[n][0] - 1)]);
                    }
                    g.fillOval(x - 4, y - 4, 8, 8);

                    g.setFont(this.dataFont);

                    if(this.gRow[n][this.gLap[n]] <= 8)
                        g.setColor(this.bgColour2a);
                    else {
                        g.setColor(this.bgColour2);
                    }
                    g.fillOval(x - 2, y - 2, 4, 4);
                }
        } else if(this.screenNum == 3) {
            if(this.isValid)
                g.drawImage(this.offscreen3, 0, 0, this);
            else {
                drawSafety(g, 2);
            }
        }

        g.dispose();

        drawLED();

        if(repaint)
            repaint();
    }

    public void setValid(boolean valid) {

        this.isValid = valid;
        redrawOffscreen(true);
    }

    private Rectangle commentaryClip(int first, int last) {

        int x = this.BORDERSIZE;
        int y = this.COMMENTHEAD + 1 + this.commentaryOffset + first * (this.commentaryHeight + 1);

        int w = this.CWIDTH - (this.BORDERSIZE * 2) - this.SCROLLBAR_WIDTH - 1;
        int h = (last - first + 1) * (this.commentaryHeight + 1);

        return new Rectangle(x, y, w, h);
    }

    private void redrawCommentary() {

        Graphics g = this.offscreen.getGraphics();
        g.drawImage(this.offscreenC, this.WIDTH - this.CWIDTH, 0, this);
        g.dispose();

        if(this.redraw)
            repaint();
    }

    private void doScrollButtons(Graphics g) {

        g.setClip(0, 0, this.CWIDTH, this.HEIGHT);
        if(this.scrollLine == 0)
            drawScroll(g, '\1', 0);
        else {
            drawScroll(g, '\1', 1);
        }

        if((this.scrollLine == this.nCommentaryLines - this.commentaryRows) || (this.nCommentaryLines < this.commentaryRows))
            drawScroll(g, '\2', 0);
        else
            drawScroll(g, '\2', 1);
    }

    private int convertOffset(int offset) {

        int height;
        if(this.nCommentaryLines > this.commentaryRows)
            height = this.nCommentaryLines * this.commentaryHeight;
        else {
            height = this.commentaryRows * this.commentaryHeight;
        }

        return (offset * this.commentaryHeight * (this.HEIGHT - this.COMMENTHEAD - this.COMMENTTAIL) / height);
    }

    private void doScrollBar(boolean redraw) {

        if(this.scrollbar == null)
            return;
        doScrollBar(redraw, convertOffset(this.scrollLine));
    }

    private void doScrollBar(boolean redraw, int offset) {

        if(this.scrollbar != null) {
            int height;
            if(this.nCommentaryLines > this.commentaryRows)
                height = this.nCommentaryLines * this.commentaryHeight;
            else {
                height = this.commentaryRows * this.commentaryHeight;
            }

            this.scrollbar.setScrollBar(this.commentaryRows * this.commentaryHeight, height, offset);

            if(redraw)
                redrawCommentary();
        }
    }

    public void scrollUp() {

        Graphics g = this.offscreenC.getGraphics();
        scrollUp(g);
        g.dispose();
    }

    public void scrollUp(Graphics g) {

        if(this.scrollLine > 0) {
            this.scrollLine -= 1;
            g.setClip(commentaryClip(0, this.commentaryRows - 1));

            for(int n = this.commentaryRows - 2; n >= 0; --n) {
                Rectangle dest = commentaryClip(n + 1, n + 1);
                Rectangle source = commentaryClip(n, n);

                g.drawImage(this.offscreenC, dest.x, dest.y, dest.x + dest.width, dest.y + dest.height, source.x, source.y, source.x + source.width, source.y + source.height, this);
            }

            Rectangle clip = commentaryClip(0, 0);
            g.setClip(clip);
            g.setColor(this.bgColourC);

            g.fillRect(clip.x, clip.y, clip.width, clip.height);

            g.setColor(this.fgColour);
            g.setFont(this.commentaryFont);
            FontMetrics fm = g.getFontMetrics();
            g.drawString(this.commentaryLines[this.scrollLine], this.BORDERSIZE + this.COMMENTPAD, clip.y + (this.commentaryHeight - fm.getHeight()) / 2 + fm.getAscent() - 2);

            if(!(this.inlineLayout)) {
                doScrollButtons(g);
            }
            redrawCommentary();
        }
    }

    public void scrollDown() {

        Graphics g = this.offscreenC.getGraphics();
        scrollDown(g);
        g.dispose();
    }

    public void scrollDown(Graphics g) {

        if(this.scrollLine < this.nCommentaryLines - this.commentaryRows) {
            this.scrollLine += 1;
            g.setClip(commentaryClip(0, this.commentaryRows - 1));

            Rectangle dest = commentaryClip(0, this.commentaryRows - 2);
            Rectangle source = commentaryClip(1, this.commentaryRows - 1);
            g.drawImage(this.offscreenC, dest.x, dest.y, dest.x + dest.width, dest.y + dest.height, source.x, source.y, source.x + source.width, source.y + source.height, this);

            Rectangle clip = commentaryClip(this.commentaryRows - 1, this.commentaryRows - 1);
            g.setClip(clip);
            g.setColor(this.bgColourC);

            g.fillRect(clip.x, clip.y, clip.width, clip.height);

            g.setColor(this.fgColour);
            g.setFont(this.commentaryFont);
            FontMetrics fm = g.getFontMetrics();
            g.drawString(this.commentaryLines[(this.scrollLine + this.commentaryRows - 1)], this.BORDERSIZE + this.COMMENTPAD, clip.y + (this.commentaryHeight - fm.getHeight()) / 2 + fm.getAscent() - 2);

            if(!(this.inlineLayout)) {
                doScrollButtons(g);
            }

            redrawCommentary();
        }
    }

    private void scrollTo(int toLine) {

        int limit;
        if(toLine < 0) {
            toLine = 0;
        }
        if(toLine > this.nCommentaryLines - this.commentaryRows) {
            toLine = this.nCommentaryLines - this.commentaryRows;
        }

        this.scrollLine = toLine;
        Graphics g = this.offscreenC.getGraphics();
        g.setColor(this.bgColourC);
        g.fillRect(this.BORDERSIZE, this.COMMENTHEAD, this.CWIDTH - this.SCROLLBAR_WIDTH - (this.BORDERSIZE * 2), this.HEIGHT - this.COMMENTHEAD - this.COMMENTTAIL);

        g.setColor(this.fgColour);
        g.setFont(this.commentaryFont);
        FontMetrics fm = g.getFontMetrics();

        if(this.nCommentaryLines < this.commentaryRows)
            limit = this.nCommentaryLines;
        else {
            limit = this.commentaryRows;
        }
        for(int n = 0; n < limit; ++n) {
            Rectangle clip = commentaryClip(n, n);
            g.drawString(this.commentaryLines[(this.scrollLine + n)], this.BORDERSIZE + this.COMMENTPAD, clip.y + (this.commentaryHeight - fm.getHeight()) / 2 + fm.getAscent() - 2);
        }
        redrawCommentary();
    }

    private void xaddrow(Graphics g, FontMetrics fm) {

        g.setClip(commentaryClip(0, this.commentaryRows - 1));
        Rectangle dest = commentaryClip(0, this.commentaryRows - 2);
        Rectangle source = commentaryClip(1, this.commentaryRows - 1);
        g.drawImage(this.offscreenC, dest.x, dest.y, dest.x + dest.width, dest.y + dest.height, source.x, source.y, source.x + source.width, source.y + source.height, this);

        Rectangle clip = commentaryClip(this.commentaryRows - 1, this.commentaryRows - 1);
        g.setClip(clip);
        g.setColor(this.bgColourC);

        g.fillRect(clip.x, clip.y, clip.width, clip.height);

        g.setColor(this.fgColour);
        g.drawString(this.commentaryLines[(this.nCommentaryLines - 1)], this.BORDERSIZE + this.COMMENTPAD, clip.y + (this.commentaryHeight - (fm.getDescent() + fm.getAscent())) / 2 + fm.getAscent() - 2);
    }

    private void addrow(Graphics g, FontMetrics fm, String str) {

        if((this.nCommentaryLines >= this.commentaryRows) && (this.scrollLine + this.commentaryRows == this.nCommentaryLines)) {
            if((this.streaming) && (!(this.noCommentaryScroll))) {
                this.scrollLine += 1;
            }
            if(this.inlineLayout)
                doScrollBar(true);
            else {
                drawScroll(g, '\2', 0);
            }
        }
        this.commentaryLines[(this.nCommentaryLines++)] = str;
        if((this.nCommentaryLines < this.commentaryRows) || (this.scrollLine + this.commentaryRows >= this.nCommentaryLines)) {
            xaddrow(g, fm);
        } else if((!(this.inlineLayout)) && (this.streaming) && (!(this.loadingKeyframe))) {
            new f1flasher(this, this.offscreenC);
        }

        if(this.inlineLayout)
            doScrollBar(true);
    }

    private void addtext(Graphics g, FontMetrics fm, String str) {

        int fw = fm.stringWidth(str);
        int remaining = this.CWIDTH - (this.BORDERSIZE * 2) - (this.COMMENTPAD * 2) - this.SCROLLBAR_WIDTH;
        int previous = str.length();

        if(fw < remaining) {
            addrow(g, fm, str);
        } else {
            String str1 = str;
            String str2 = null;

            while(fw > remaining) {
                int lastspace = str1.lastIndexOf(32);
                if(lastspace != -1) {
                    str1 = str.substring(0, lastspace);
                    str2 = str.substring(lastspace + 1);
                    fw = g.getFontMetrics().stringWidth(str1);
                    previous = lastspace;
                } else {
                    do {
                        --previous;
                        str1 = str.substring(0, previous);
                        str2 = str.substring(previous);
                        fw = g.getFontMetrics().stringWidth(str1);
                    } while(fw > remaining);
                }

            }

            addrow(g, fm, str1);
            if(str2 != null)
                addtext(g, fm, str2);
        }
    }

    public void addComment(int commentaryLang, String comment) {

        Graphics g = this.offscreenC.getGraphics();

        if((commentaryLang == this.language) || (commentaryLang == 0)) {
            g.setFont(this.commentaryFont);
            FontMetrics fm = g.getFontMetrics();

            addrow(g, fm, "");
            addtext(g, fm, comment);

            if(!(this.inlineLayout)) {
                doScrollButtons(g);
            }
            g.dispose();

            redrawCommentary();
        }
    }

    private void decrementTime(boolean dec) {

        if(this.countdownTimer != null) {
            this.countdownTimer.stopit();
            this.countdownTimer = null;
        }

        if(dec)
            this.countdownTimer = new f1time(this, this.lastOfficialTime, this.lastOfficialTimeTime);
    }

    private void drawVText(Image i, Graphics g, int w, int h, int dx, int dy, String str) {

        int x;
        int[] pixels1 = new int[w * h];
        int[] pixels2 = new int[h * w];

        PixelGrabber pg = new PixelGrabber(i, dx, dy, w, h, pixels1, 0, w);
        try {
            pg.grabPixels();
        } catch (InterruptedException e) {
            debug("grabPixels1 failed");
        }

        for(y = 0; y < h; ++y) {
            for(x = 0; x < w; ++x) {
                pixels2[(x * h + h - y - 1)] = pixels1[(y * w + x)];
            }
        }

        Image img1 = createImage(new MemoryImageSource(h, w, pixels2, 0, h));
        prepareImage(img1, this);

        Image img1a = createImage(h, w);
        Graphics g2 = img1a.getGraphics();
        g2.drawImage(img1, 0, 0, this);

        g2.setColor(g.getColor());
        g2.setFont(g.getFont());
        FontMetrics fm = g2.getFontMetrics();

        g2.setColor(g.getColor());
        g2.drawString(str, (h - fm.stringWidth(str)) / 2, (w - (fm.getAscent() + fm.getDescent())) / 2 + fm.getAscent());
        g2.dispose();

        pg = new PixelGrabber(img1a, 0, 0, h, w, pixels1, 0, h);
        try {
            pg.grabPixels();
        } catch (InterruptedException y) {
            debug("grabPixels2 failed");
        }

        for(y = 0; y < h; ++y) {
            for(x = 0; x < w; ++x) {
                pixels2[(y * w + x)] = pixels1[(x * h + h - y - 1)];
            }
        }

        Image img2 = createImage(new MemoryImageSource(w, h, pixels2, 0, w));
        prepareImage(img2, this);
        g.drawImage(img2, dx, dy, this);
    }

    public void setSafetyMessage(String str) {

        this.safetyMessage = str;
    }

    public void setLED(Color c) {

        this.LEDColour = c;
        drawLED();
    }

    private void drawLED() {

        Graphics g = this.offscreen.getGraphics();
        g.setColor(Color.black);
        g.fillOval(3, this.HEIGHT - 10, 6, 6);
        g.setColor(this.LEDColour);
        g.fillOval(4, this.HEIGHT - 9, 4, 4);

        g.dispose();

        repaint(50L, 3, this.HEIGHT - 10, 6, 6);
    }

    @Override
    public boolean imageUpdate(Image img, int infoflags, int x, int y, int width, int height) {

        return true;
    }

    public void debug(String str) {

    }

    public void output(String str) {

        System.out.println(str);
    }

    public String getSessionID() {

        return this.sessionID;
    }

    private String getData(InputStream iS) throws IOException {

        byte[] buffer = new byte[1024];
        String str = "";

        int n = iS.read(buffer, 0, 1024);
        while(n >= 0) {
            str = str + new String(buffer, 0, n, "ISO-8859-1");
            n = iS.read(buffer, 0, 1024);
        }
        return str;
    }

    private boolean fetchEncryptionKey() {

        String key;
        try {
            URL keyURL = new URL(getCodeBase(), "/reg/getkey/" + this.sessionID + ".asp?auth=" + this.userCreds);

            debug("Fetching " + keyURL.toString());
            key = getData(keyURL.openStream());
        } catch (IOException e) {
            debug("IO exception in fetchEncryptionKey: " + e.toString());
            return false;
        }

        if(key.equalsIgnoreCase("INVALID")) {
            this.stopping = true;
            this.safetyMessage = "Invalid credentials - please log off and back on again to view this stream";
            setValid(false);
        } else {
            if(key.length() <= 8) {
                try {
                    this.encryptionKey = (int)(Long.parseLong(key, 16) & 0xFFFFFFFF);
                    this.decrypter = new f1crypt(this, this.encryptionKey);
                } catch (NumberFormatException e) {
                    debug("Number Format Exception in fetchEncryptionKey");
                    return false;
                }
            }

            debug("Invalid response received from key server: " + key);
            return false;
        }
        return true;
    }

    public void setSessionID(String str) {

        boolean same;
        if(str.equals(this.sessionID))
            same = true;
        else {
            same = false;
        }

        debug("Session ID set to " + str + " (was " + this.sessionID + ")");
        this.sessionID = str;

        if(same)
            return;
        try {
            if(!(fetchEncryptionKey())) {
                setLED(Color.pink);
                Thread.sleep(3000L);
                if(!(fetchEncryptionKey())) {
                    Thread.sleep(10000L);
                    if(!(fetchEncryptionKey())) {
                        Thread.sleep(25000L);
                        if(!(fetchEncryptionKey())) {
                            this.stopping = true;
                            this.safetyMessage = "Key Server error ... please try again";
                            setValid(false);
                        }
                    }
                }
                setLED(Color.yellow);
            }
        } catch (InterruptedException e) {
            debug("Sleep interrupted while waiting for a key");
        }
    }
}