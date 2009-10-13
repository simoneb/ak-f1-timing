package uk.co.aspectgroup.f1app;

import java.awt.Graphics;
import java.awt.Image;

public class f1scroller extends Thread {

    f1app theApp;
    Image offscreen;

    public f1scroller(f1app app, Image offscreenC) {

        this.theApp = app;
        this.offscreen = offscreenC;

        this.theApp.debug("$Id$");
        start();
    }

    public void run() {

        Graphics g = this.offscreen.getGraphics();
        try {
            Thread.sleep(500L);
        } catch (InterruptedException localInterruptedException) {
        }
        while(this.theApp.scrolling > 0) {
            switch(this.theApp.scrolling) {
                case '\1':
                    this.theApp.scrollUp(g);
                    break;
                case '\2':
                    this.theApp.scrollDown(g);
            }
            try {
                Thread.sleep(50L);
            } catch (InterruptedException localInterruptedException1) {
            }
        }
        g.dispose();
    }
}