package uk.co.aspectgroup.f1app;

public class f1weather extends Thread {

    private f1app theApp;
    private int wNum;
    private boolean delay;

    public f1weather(f1app app) {

        this.theApp = app;
        this.wNum = 0;

        this.theApp.debug("$Id$");
        this.delay = false;
        start();
    }

    public void delayNext() {

        this.delay = true;
    }

    public void run() {

        while(this.theApp != null) {
            try {
                Thread.sleep(5000L);
            } catch (InterruptedException localInterruptedException) {
            }
            if(this.delay) {
                this.delay = false;
            } else {
                this.wNum += 1;
                if(this.wNum == 6)
                    this.wNum = 0;

                this.theApp.updateWGraph(this.wNum);
                this.theApp.drawWGraph(this.wNum);
            }
        }
    }
}