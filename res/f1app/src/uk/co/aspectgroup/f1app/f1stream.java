package uk.co.aspectgroup.f1app;

import java.awt.Color;
import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.IOException;
import java.net.Socket;
import java.net.URL;

public class f1stream extends Thread {

    private f1app theApp;
    private Socket s;
    private BufferedInputStream inStream;
    private BufferedOutputStream outStream;

    public f1stream(f1app app) {

        this.theApp = app;

        this.theApp.debug("$Id$");

        String hostname = this.theApp.getCodeBase().getHost();
        if((hostname == null) || (hostname.equals(""))) {
            hostname = new String("dev-timing.formula1.com");

            this.theApp.debug("Hostname null, connecting to dev-timing");
        }
        this.theApp.debug("Stream connecting to " + hostname);
        try {
            this.s = new Socket(hostname, 4321);
            this.inStream = new BufferedInputStream(this.s.getInputStream());
            this.outStream = new BufferedOutputStream(this.s.getOutputStream());
            this.theApp.debug("inStream: " + this.inStream);
            this.theApp.debug("outStream: " + this.outStream);
        } catch (IOException e) {
            this.theApp.output("Can't connect to " + hostname + "'s stream on port 4321");
            try {
                this.theApp.setLED(Color.red);
                Thread.sleep(30000L);
                this.theApp.setLED(Color.yellow);
            } catch (InterruptedException ie) {
                return;
            }
            return;
        }

        start();
    }

    public void run() {

        this.theApp.debug("f1stream starting its run loop");
        try {
            this.theApp.debug("Calling process loop for live stream");
            this.theApp.theProcess.process(this.inStream);
        } catch (IOException e) {
            this.theApp.debug("IO Error during stream's process loop");
            return;
        }

        this.theApp.debug("f1stream ending its run loop");
    }

    public void terminate() {

        this.theApp.debug("f1stream: terminate called");
    }

    public boolean ping() {

        if(this.outStream != null) {
            try {
                this.outStream.write(16);
                this.outStream.flush();
            } catch (IOException e) {
                this.theApp.debug(e.toString());
                return false;
            }
            return true;
        }
        this.theApp.debug("outStream null in ping()");
        return false;
    }
}