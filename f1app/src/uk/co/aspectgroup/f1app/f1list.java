package uk.co.aspectgroup.f1app;

public class f1list {

    static final int INITIAL_SIZE = 10;
    private int[] data;
    private int dataSize;
    private int maxsize;

    public f1list() {

        this.data = new int[10];
        this.dataSize = 0;
        this.maxsize = 10;
    }

    public void add(int element) {

        if(this.dataSize == this.maxsize) {
            int[] oldData = this.data;
            int[] newData = new int[this.maxsize + 10];

            System.arraycopy(oldData, 0, newData, 0, this.maxsize);

            this.data = newData;
            this.maxsize += 10;
        }

        this.data[(this.dataSize++)] = element;
    }

    public int get(int index) throws IndexOutOfBoundsException {

        if(index >= this.dataSize) {
            throw new IndexOutOfBoundsException("List size: " + Integer.toString(this.dataSize));
        }
        return this.data[index];
    }

    public int size() {

        return this.dataSize;
    }
}