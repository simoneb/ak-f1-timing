package uk.co.aspectgroup.f1app;

import java.awt.Color;
import java.io.BufferedInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.Date;

public class f1process extends Thread {

    f1app theApp;
    private f1stream theStream;
    private final String keyFrameURI;
    private String[][] data;
    private byte[] positions;
    private byte[][] colours;
    static final int delayTime = 1;
    static final int longRefreshRate = 30;
    static final byte PCF_COMPLETE = 1;
    static final byte PCF_UTF16 = 2;
    private String commentaryString;
    private int currentLap = 0;
    private int refreshRate;
    private int clockTimestamp;
    private boolean interpolateNext;
    private int keyFrameNumber;
    private boolean dontProcess;
    private long lastRead;
    private int keyFrameCount;

    public f1process(f1app app, String URI) {

        this.theApp = app;

        this.keyFrameURI = URI;
        this.keyFrameCount = 0;

        if(URI == null) {
            this.theApp.debug("No keyframe specified");
            return;
        }

        this.theApp.debug("$Id$");
        start();
    }

    private void initData() {

        this.currentLap = 0;

        this.data = new String[32][14];
        this.positions = new byte[32];
        this.colours = new byte[32][14];

        this.commentaryString = "";
    }

    @Override
    public void run() {

        if(!(this.theApp.streaming)) {
            loadKeyFrame(0);
            this.theApp.output("Frame loaded");
            this.theApp.setLED(Color.darkGray);
            this.theApp.redrawOffscreen(true);
            return;
        }

        this.refreshRate = 5;

        noPingPlease();

        while((this.refreshRate > 0) && (!(this.theApp.stopping))) {
            this.keyFrameNumber = 0;

            this.dontProcess = false;

            long startTime = new Date().getTime();

            while(!(loadKeyFrame(this.keyFrameNumber))) {
                this.theApp.debug("Can't load key frame");
                sleep(10 + (int)(Math.random() * 10.0D));
            }

            this.dontProcess = true;

            if(this.refreshRate > 0) {
                this.theStream = new f1stream(this.theApp);

                while((this.refreshRate > 0) && (this.theStream != null) && (this.theStream.isAlive())) {
                    sleep(1);

                    if(this.refreshRate > 0) {
                        this.theApp.setLED(Color.yellow);
                        if(new Date().getTime() - this.lastRead > this.refreshRate * 1000) {
                            this.theApp.debug("Pinging! (" + (new Date().getTime() - this.lastRead) + "ms)");
                            noPingPlease();
                            if((this.theStream == null) || (this.theStream.ping()))
                                continue;
                            this.theStream.terminate();
                            this.theStream = null;
                            this.theApp.setLED(Color.red);
                        }

                    }

                }

                long timeDiff = new Date().getTime() - startTime;
                if(timeDiff < 30000L) {
                    this.theApp.debug("Sleeping for " + Long.toString(30L - (timeDiff / 1000L)));
                    sleep((int)(30L - (timeDiff / 1000L)));
                }
            }
        }
        this.theApp.setLED(Color.darkGray);
    }

    public void setRefresh(int refresh) {

        this.refreshRate = refresh;
        if(refresh == 0) {
            this.theApp.output("End of Stream");
            this.theApp.setLED(Color.darkGray);
            cleanup();
        } else {
            this.theApp.output("Notify: " + refresh);
        }
    }

    public void noPingPlease() {

        this.lastRead = new Date().getTime();
    }

    public void cleanup() {

        if(this.theStream != null) {
            f1stream tmpStream = this.theStream;
            this.theStream = null;
            tmpStream.terminate();
        }
    }

    private boolean sleep(int s) {

        try {
            Thread.sleep(s * 1000);
        } catch (InterruptedException e) {
            return false;
        }
        return true;
    }

    private URL getKeyFrameURL(int n) {

        URL theURL;
        String URI;
        if(n == 0) {
            URI = new String(this.keyFrameURI + ".bin");
            if(this.keyFrameCount > 0) {
                URI = URI.concat("?" + Integer.toString(this.keyFrameCount));
            }
            this.keyFrameCount += 1;
        } else {
            String tmp = new String("00000" + Integer.toString(n));
            URI = new String(this.keyFrameURI + "_" + tmp.substring(tmp.length() - 5) + ".bin");
            this.keyFrameCount = 0;
        }
        try {
            theURL = new URL(this.theApp.getCodeBase(), URI);
        } catch (MalformedURLException e) {
            this.theApp.debug("Can't parse keyframe URI: " + this.keyFrameURI);
            return null;
        }
        return theURL;
    }

    private boolean loadKeyFrame(int n) {

        URL u = getKeyFrameURL(n);

        if(u != null) {
            this.theApp.debug("Loading keyframe from " + u.toString());

            initData();

            this.theApp.loadingKeyframe = true;

            if(this.keyFrameCount <= 1) {
                this.theApp.initScreenC();
            }

            this.theApp.modeSet = false;
            try {
                InputStream iStream = u.openStream();
                BufferedInputStream bIStream = new BufferedInputStream(iStream);
                this.theApp.setRedraw(false);
                this.theApp.decrypter.reset();
                process(bIStream);
                this.theApp.setRedraw(true);
            } catch (FileNotFoundException e) {
                this.theApp.debug("404 Error - Keyframe not found");
                if(!(this.theApp.streaming)) {
                    this.theApp.setSafetyMessage("The Live Timing Archive for this session is not yet available");
                    this.theApp.setValid(false);
                    return true;
                }
                return false;
            } catch (IOException e) {
                this.theApp.debug("IO Exception during keyframe: " + e.toString());
                return false;
            }
            this.theApp.debug("Keyframe loaded");
            this.theApp.loadingKeyframe = false;
            this.theApp.noCommentaryScroll = false;
            return true;
        }
        return false;
    }

    public void drawRow(int slot) {

        for(int n = 1; n <= this.theApp.nCols; ++n)
            this.theApp.updateCell(this.data[slot][n], this.colours[slot][n], this.positions[slot], n, true);
    }

    public void blankRow(int row) {

        for(int n = 0; n < 32; ++n) {
            if(this.positions[n] == row) {
                return;
            }
        }

        for(int n = 1; n <= this.theApp.nCols; ++n)
            this.theApp.updateCell("", (byte)0, row, n, true);
    }

    private boolean readBytes(BufferedInputStream bis, byte[] barr, int len, boolean decrypt) throws IOException {

        int n;
        int total = 0;

        if(this.theApp.stopping) {
            return false;
        }
        do {
            n = bis.read(barr, total, len - total);
            if(n < 0) {
                this.theApp.debug("read returned: " + Integer.toString(n));
                return false;
            }
            total += n;
        } while(total < len);

        if(decrypt) {
            for(n = 0; n < len; ++n) {
                byte newb = this.theApp.decrypter.decrypt(barr[n]);

                barr[n] = newb;
            }
        }
        return true;
    }

    public void process(BufferedInputStream inStream) throws IOException {

        boolean active = true;

        byte[] byteArray = new byte[128];

        while((active) && (!(this.theApp.stopping))) {
            try {
                active = readBytes(inStream, byteArray, 2, false);
            } catch (IOException e) {
                throw e;
            }

            if((!(active)) || (this.theApp.stopping)) {
                this.theApp.debug("Process no longer active or applet stopping");
                return;
            }

            noPingPlease();
            this.theApp.setLED(Color.green);

            byte b1 = byteArray[0];
            byte b2 = byteArray[1];

            byte id = (byte)(b1 & 0x1F);
            byte x = (byte)((b1 & 0xE0) >> 5 & 0x7 | (b2 & 0x1) << 3);
            byte c = (byte)((b2 & 0xE) >> 1);
            byte l = (byte)((b2 & 0xF0) >> 4);
            byte v = (byte)((b2 & 0xFE) >> 1);

            this.theApp.debug("(" + this.dontProcess + ") id=" + id + ", x=" + x + ", c=" + c + " , l=" + l + ", v=" + v);

            if(id > 0) {
                if(x == 0) {
                    if(this.dontProcess) {
                        continue;
                    }

                    if(v == 0) {
                        int oldv = this.positions[id];

                        this.positions[id] = v;
                        blankRow(oldv);
                    } else {
                        if((this.theApp.raceMode == 1) && (this.theApp.getRowZero(id) == 0)) {
                            this.theApp.initRow(v, id);
                        }
                        this.positions[id] = v;
                        drawRow(id);
                    }
                } else if(x == 15) {
                    try {
                        readBytes(inStream, byteArray, v, true);
                    } catch (IOException e) {
                        throw e;
                    }

                    if(!(this.dontProcess)) {
                        for(int n = 0; n < v; ++n) {
                            if(n == 0) {
                                this.theApp.initRow(byteArray[0], id);
                            } else if(byteArray[n] > 0)
                                this.theApp.updateGraph(id, byteArray[n], n);
                        }
                    }
                } else {
                    if((!(this.dontProcess)) && (x <= 13)) {
                        this.colours[id][x] = c;
                    }

                    if(l < 15) {
                        if(l == 0) {
                            if(!(this.dontProcess))
                                this.data[id][x] = null;
                        } else {
                            try {
                                readBytes(inStream, byteArray, l, true);
                            } catch (IOException n) {
                                throw n;
                            }

                            if((!(this.dontProcess)) && (x <= 13)) {
                                this.data[id][x] = new String(byteArray, 0, l, "ISO-8859-1");
                                this.theApp.debug("Updating data: (" + id + "," + x + ")=" + this.data[id][x]);
                            }

                            if((this.theApp.raceMode == 1) && (!(this.dontProcess))) {
                                if((x == 3) && (this.theApp.getRowName(id) == null)) {
                                    this.theApp.setRowName(id, this.data[id][x]);
                                } else if((x == 5) && (this.positions[id] == 1)) {
                                    this.currentLap = Integer.parseInt(this.data[id][x]);
                                    this.theApp.debug("Current lap: " + Integer.toString(this.currentLap));
                                } else if(x == 11) {
                                    int behind = 0;

                                    if((this.data[id][4] != null) && (((this.data[id][6] == null) || (!(this.data[id][6].equals("OUT")))))) {
                                        int ind = this.data[id][4].indexOf(76);
                                        if(ind > 0) {
                                            behind = Integer.parseInt(this.data[id][4].substring(0, ind));
                                        }
                                        this.theApp.updateGraph(id, this.positions[id], this.currentLap - behind);
                                    }
                                }
                            }
                        }

                    }

                    if((this.positions[id] <= 0) || (this.dontProcess) || (x > 13))
                        continue;
                    this.theApp.updateCell(this.data[id][x], c, this.positions[id], x, true);
                    this.theApp.debug("updating cell (" + x + "," + this.positions[id] + ") with colour " + c + ": " + this.data[id][x]);
                }

            } else {
                switch(x) {
                    case 0:
                        throw new IOException("Invalid domino in stream");
                    case 1:
                        try {
                            readBytes(inStream, byteArray, l, false);
                        } catch (IOException behind) {
                            throw behind;
                        }

                        this.theApp.setSessionID(new String(byteArray, 1, l - 1, "ISO-8859-1"));
                        this.theApp.debug("Feed mode: " + c + ", value: " + this.theApp.getSessionID());
                        if(!(this.dontProcess)) {
                            this.theApp.initMode(c, this.keyFrameCount > 1);
                        }

                        break;
                    case 2:
                        if(l != 2) {
                            throw new IOException("Long keyframe marker");
                        }
                        try {
                            readBytes(inStream, byteArray, l, false);
                        } catch (IOException behind) {
                            throw behind;
                        }

                        int kf = byteArray[1] << 8 & 0xFF00 | byteArray[0] & 0xFF;

                        if(this.dontProcess)
                            this.theApp.debug("Not processed keyframe: " + Integer.toString(kf));
                        else {
                            this.theApp.debug("Just processed keyframe: " + Integer.toString(kf));
                        }

                        if(kf == this.keyFrameNumber) {
                            this.theApp.debug("Keyframe: " + Integer.toString(kf));
                            this.dontProcess = false;
                        } else if((this.dontProcess) && (kf > this.keyFrameNumber)) {
                            this.dontProcess = false;
                            while(!(loadKeyFrame(kf))) {
                                this.theApp.debug("Can't load key frame :" + Integer.toString(kf));
                                sleep(1);
                            }
                        }
                        this.keyFrameNumber = kf;
                        this.theApp.decrypter.reset();
                        break;
                    case 3:
                        if(l != 0) {
                            throw new IOException("Long valid marker");
                        }

                        if(!(this.dontProcess)) {
                            this.theApp.setValid(c != 0);
                        }
                        break;
                    case 4:
                        try {
                            readBytes(inStream, byteArray, v, true);
                            this.theApp.debug("Read comment: " + new String(byteArray, 0, v, "ISO-8859-1"));
                        } catch (IOException e) {
                            throw e;
                        }

                        if(!(this.dontProcess)) {
                            if(byteArray[0] < 32) {
                                if((byteArray[1] & 0x2) > 0)
                                    this.commentaryString += new String(byteArray, 2, v - 2, "UTF-16LE");
                                else {
                                    this.commentaryString += new String(byteArray, 2, v - 2, "UTF-8");
                                }

                                if((byteArray[1] & 0x1) > 0) {
                                    this.theApp.addComment(byteArray[0], this.commentaryString);
                                    this.theApp.debug("New-style comment: " + this.commentaryString);
                                    this.commentaryString = "";
                                }
                            } else {
                                this.theApp.addComment(0, new String(byteArray, 0, v, "ISO-8859-1"));
                                this.theApp.debug("Old-style comment: " + new String(byteArray, 0, v, "ISO-8859-1"));
                            }
                        }
                        break;
                    case 5:
                        if(!(this.dontProcess)) {
                            setRefresh(v);
                        }
                        break;
                    case 6:
                        try {
                            readBytes(inStream, byteArray, v, true);
                        } catch (IOException e) {
                            throw e;
                        }

                        if(!(this.dontProcess)) {
                            this.theApp.setSafetyMessage(new String(byteArray, 0, v, "UTF-8"));

                            System.out.println("*");
                            this.theApp.debug("Setting safety message to: " + new String(byteArray, 0, v, "ISO-8859-1"));
                        }
                        break;
                    case 7:
                        try {
                            readBytes(inStream, byteArray, 2, true);
                        } catch (IOException e) {
                            throw e;
                        }

                        if(!(this.dontProcess)) {
                            int ts = byteArray[1] << 8 & 0xFF00 | byteArray[0] & 0xFF | v << 16 & 0xFF0000;
                            this.theApp.debug("Timestamp: " + Integer.toString(ts));
                            this.theApp.updateTime(ts);
                            if((this.interpolateNext) && (!(this.theApp.loadingKeyframe))) {
                                this.interpolateNext = false;
                                this.theApp.fudgeSessionTime(ts - this.clockTimestamp);
                                this.theApp.updateSessionTimeInterpolate();
                            }
                        }
                        break;
                    case 9:
                        if(l < 15) {
                            try {
                                readBytes(inStream, byteArray, l, true);
                            } catch (IOException ts) {
                                throw ts;
                            }

                            if(!(this.dontProcess)) {
                                if(c > 0) {
                                    this.theApp.debug("Weather: c=" + Byte.toString(c) + ", " + new String(byteArray, 0, l, "ISO-8859-1"));
                                    this.theApp.updateScreen3Weather(c, new String(byteArray, 0, l, "ISO-8859-1"));
                                } else if(l > 0) {
                                    this.theApp.updateSessionTime(new String(byteArray, 0, l, "ISO-8859-1"));
                                    if(this.theApp.loadingKeyframe)
                                        this.clockTimestamp = this.theApp.getTime();
                                }
                            }
                        } else {
                            this.theApp.debug("*** Decrement! c=" + Byte.toString(c) + ", l=15");
                            if(this.theApp.loadingKeyframe)
                                this.interpolateNext = true;
                            else {
                                this.theApp.updateSessionTimeInterpolate();
                            }
                        }
                        break;
                    case 10:
                        try {
                            readBytes(inStream, byteArray, v, true);
                        } catch (IOException ts) {
                            throw ts;
                        }

                        if(!(this.dontProcess)) {
                            String datastr = new String(byteArray, 1, v - 1, "ISO-8859-1");
                            int col = byteArray[0] - 1;

                            if((col >= 0) && (col <= 3)) {
                                int subcol = 1;
                                int row = 1;
                                int n = datastr.indexOf(13);
                                while(n > 0) {
                                    String label = datastr.substring(0, n);
                                    this.theApp.updateScreen3Speed(col * 2 + subcol, row, label);
                                    if(subcol == 1) {
                                        subcol = 2;
                                    } else {
                                        subcol = 1;
                                        ++row;
                                    }
                                    datastr = datastr.substring(n + 1);
                                    n = datastr.indexOf(13);
                                }
                            } else if((col >= 4) && (col <= 7)) {
                                this.theApp.debug("Fastest Lap column: " + col + ", value: " + datastr);
                                this.theApp.updateScreen3Fastest(col, datastr);
                            }

                        }

                        break;
                    case 11:
                        try {
                            readBytes(inStream, byteArray, l, true);
                        } catch (IOException datastr) {
                            throw datastr;
                        }

                        if(!(this.dontProcess)) {
                            if(c == 1) {
                                this.theApp.debug("Flag status: " + new String(byteArray, 0, l, "ISO-8859-1"));
                                this.theApp.updateRaceStatus(Integer.parseInt(new String(byteArray, 0, l, "ISO-8859-1")));
                            } else {
                                this.theApp.debug("Unknown misc status, type: " + Byte.toString(byteArray[0]));
                            }
                        }
                        break;
                    case 12:
                        try {
                            readBytes(inStream, byteArray, v, false);
                        } catch (IOException datastr) {
                            throw datastr;
                        }
                    case 8:
                    default:
                        this.theApp.debug("Error! Unhandled code: " + Byte.toString(x));
                        throw new IOException("Unhandled code");
                }
            }
        }
    }
}