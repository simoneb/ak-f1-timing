package uk.co.aspectgroup.f1app;

import java.applet.AppletContext;
import java.io.UnsupportedEncodingException;
import java.net.MalformedURLException;
import java.net.URL;
import java.net.URLEncoder;

public class f1security extends Thread {

    f1app theApp;

    public f1security(f1app app) {

        this.theApp = app;

        this.theApp.debug("$Id$");
        start();
    }

    public void run() {

        try {
            Thread.sleep(5000L);
        } catch (InterruptedException localInterruptedException) {
        }
        try {
            this.theApp.getAppletContext().showDocument(new URL("http://www.formula1.com/timings/?" + URLEncoder.encode(this.theApp.getDocumentBase().toString(), "UTF-8")), "_top");
        } catch (MalformedURLException localMalformedURLException) {
        } catch (UnsupportedEncodingException localUnsupportedEncodingException) {
        }
    }
}