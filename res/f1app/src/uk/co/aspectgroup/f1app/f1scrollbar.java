package uk.co.aspectgroup.f1app;

import java.awt.Color;
import java.awt.Graphics;
import java.awt.Image;
import java.awt.Rectangle;

public class f1scrollbar {

    private f1app theApp;
    Image offscreen;
    private int screenSize;
    private int screenMax;
    private int scrollSize;
    private int scrollMax;
    private int scrollOffset;
    private Rectangle clipRect;

    public f1scrollbar(f1app app, Image offscreenC, Rectangle clip) {

        this.theApp = app;
        this.offscreen = offscreenC;
        this.clipRect = clip;

        this.scrollMax = clip.height;
        this.scrollSize = clip.height;
        this.scrollOffset = 0;

        this.theApp.debug("$Id$");
    }

    private void redrawScrollbar() {

        Graphics g = this.offscreen.getGraphics();
        g.setClip(this.clipRect);
        g.setColor(Color.white);
        g.fillRect(this.clipRect.x, this.clipRect.y, this.clipRect.width, this.clipRect.height);

        g.setColor(new Color(13487565));
        g.fillRect(this.clipRect.x + this.theApp.BORDERSIZE, this.clipRect.y, this.clipRect.width - this.theApp.BORDERSIZE, this.clipRect.height);
        g.setColor(new Color(6710886));
        g.fillRect(this.clipRect.x + this.theApp.BORDERSIZE, this.clipRect.y + this.scrollOffset, this.clipRect.width - this.theApp.BORDERSIZE, this.scrollSize);
        g.dispose();
    }

    public int getScrollOffset() {

        return this.scrollOffset;
    }

    public int getScrollSize() {

        return this.scrollSize;
    }

    public void setScrollBar(int size, int max, int offset) {

        if((size != this.screenSize) || (max != this.screenMax)) {
            this.screenSize = size;
            this.screenMax = max;

            this.scrollSize = (size * this.scrollMax / this.screenMax);
        }

        if(offset < 0)
            offset = 0;
        if(offset > this.scrollMax - this.scrollSize)
            offset = this.scrollMax - this.scrollSize;
        this.scrollOffset = offset;
        redrawScrollbar();
    }
}