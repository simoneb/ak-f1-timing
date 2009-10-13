package uk.co.aspectgroup.f1app;

import java.awt.Graphics;
import java.awt.Image;

public class f1flasher extends Thread {

    f1app theApp;
    Image offscreen;

    public f1flasher(f1app app, Image offscreenC) {

        this.theApp = app;
        this.offscreen = offscreenC;

        this.theApp.debug("$Id$");
        start();
    }

    public void run() {

        Graphics g = this.offscreen.getGraphics();

        this.theApp.drawScroll(g, '\2', 2);
        try {
            Thread.sleep(500L);
        } catch (InterruptedException localInterruptedException) {
        }
        this.theApp.drawScroll(g, '\2', 1);
        g.dispose();
    }
}