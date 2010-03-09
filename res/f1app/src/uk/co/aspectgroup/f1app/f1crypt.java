package uk.co.aspectgroup.f1app;

public class f1crypt {

    private final int seed = 1431655765;
    private f1app theApp;
    private int mask;
    private int key;

    public f1crypt(f1app app, int newKey) {

        this.theApp = app;

        this.theApp.debug("$Id$");

        this.key = newKey;
        this.mask = 1431655765;

        this.theApp.debug("f1crypt loaded with key " + Integer.toHexString(newKey));
    }

    public void setKey(int newKey) {

        this.key = newKey;
        this.mask = 1431655765;

        this.theApp.debug("f1crypt reset with key " + Integer.toHexString(newKey));
    }

    public void reset() {

        this.mask = 1431655765;
        this.theApp.debug("f1crypt reset()");
    }

    public byte decrypt(byte b) {

        if(this.key == 0) {
            return b;
        }
        this.mask = (this.mask >> 1 & 0x7FFFFFFF ^ (((this.mask & 0x1) == 1) ? this.key : 0));

        return (byte)(b ^ this.mask & 0xFF);
    }
}