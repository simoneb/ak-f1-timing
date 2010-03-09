package uk.co.aspectgroup.f1app;

public class f1time extends Thread {

    private f1app theApp;
    private boolean going;
    private long startTime;
    private long startSeconds;

    public f1time(f1app app, String strTime, long sysTime) {

        this.theApp = app;

        this.theApp.debug("$Id$");

        this.going = true;
        this.startSeconds = timeToTime(strTime);
        this.startTime = sysTime;
        start();
    }

    public void run() {

        while(this.going) {
            this.theApp.debug("decrement at " + (System.currentTimeMillis() - this.startTime));
            if(this.startSeconds - ((System.currentTimeMillis() - this.startTime + 200L) / 1000L) <= 0L) {
                this.theApp.drawSessionTime("0");
                this.going = false;
                return;
            }
            this.theApp.drawSessionTime(timeToTime(this.startSeconds - ((System.currentTimeMillis() - this.startTime + 200L) / 1000L)));
            long sleepFor = 1000L - ((System.currentTimeMillis() - this.startTime) % 1000L);
            try {
                Thread.sleep(sleepFor);
                this.theApp.debug("Slept for " + Long.toString(sleepFor));
            } catch (InterruptedException localInterruptedException) {
            }
        }
    }

    private static String twoDigits(String s) {

        s = "0" + s;
        return s.substring(s.length() - 2);
    }

    public static String timeToTime(long nSecs) {

        if(nSecs >= 3600L)
            return Long.toString(nSecs / 3600L) + ":" + twoDigits(Long.toString(nSecs / 60L % 60L)) + ":" + twoDigits(Long.toString(nSecs % 60L));
        if(nSecs >= 60L) {
            return Long.toString(nSecs / 60L) + ":" + twoDigits(Long.toString(nSecs % 60L));
        }
        return Long.toString(nSecs);
    }

    public static int timeToTime(String strTime) {

        int remain = 0;
        int l = strTime.length();
        int[] nUnit = { 1, 10, 60, 600, 3600, 36000 };
        int nChars = 0;
        byte[] bArr = strTime.getBytes();

        while((l-- > 0) && (nChars < 6)) {
            byte c = bArr[l];

            if((c >= 48) && (c <= 57)) {
                remain += (c - 48) * nUnit[nChars];
                ++nChars;
            } else if(nChars % 2 != 0) {
                ++nChars;
            }
        }

        return remain;
    }

    public void stopit() {

        this.going = false;
    }
}